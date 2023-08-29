# ASP.NET Core LDAP Authentication Middleware

[![Build Status](https://visualisierungsinstitut.visualstudio.com/Visus.LdapAuthentication/_apis/build/status/UniStuttgart-VISUS.Visus.LdapAuthentication?branchName=main)](https://visualisierungsinstitut.visualstudio.com/Visus.LdapAuthentication/_build/latest?definitionId=6&branchName=main)
[![Visus.LdapAuthenticationVersion](https://buildstats.info/nuget/Visus.LdapAuthentication)](https://www.nuget.org/packages/Visus.LdapAuthentication)


This library implements middleware for ASP.NET Core that enables authenticating users against LDAP directories like Active Directory via an LDAP bind. The library is using [Novell's C#-only LDAP library](https://github.com/dsbenghe/Novell.Directory.Ldap.NETStandard) rather than the Windows-only DirectoryServices and is therefore running on Windows and Linux.

Built-in user objects are automatically mapped to Active Directory attributes and include commonly used claims like user name, actual names, e-mail addresses and group memberships. If necessary, you can also provide your own user object that uses a completely different mapping of LDAP attributes to claims.

[Visus.DirectoryAuthentication](Visus.DirectoryAuthentication) is a drop-in replacement for [Visus.LdapAuthentication](Visus.LdapAuthentication) using [System.DirectorySerices.Protocols](https://learn.microsoft.com/en-gb/dotnet/api/system.directoryservices.protocols) which is a platform-independent implementation of LDAP services since .NET 5.


# Usage
## Add the authentication service
The authentication functionality is added in `ConfigureServices` via the following statements:

```C#
public void ConfigureServices(IServiceCollection services) {
    // ...
    
    // Add LDAP authentication with default LdapUser object.
    {
        var options = new LdapOptions();
        this.Configuration.GetSection("LdapConfiguration").Bind(options);
        services.AddLdapAuthenticationService(options);
    }
    
    // ...
}
```

The above code uses the default `LdapUser` object from the library, which provides the most commonly used user claims. If you need additional claims or differently mapped claims, you can create your own user class either by inheriting from `LdapUserBase` and customising its behaviour or by implementing `ILdapUser` from scratch. The configuration would look like the following in this case:

```C#
public void ConfigureServices(IServiceCollection services) {
    // ...
    
    // Add LDAP authentication with customised user object.
    {
        var options = new LdapOptions();
        this.Configuration.GetSection("LdapConfiguration").Bind(options);
        services.AddLdapAuthenticationService<CustomApplicationUser>(options);
    }
    
    // ...
}
```

## Configure the LDAP server
The configuration section can have any name of your choice as long as it can be bound to [`LdapOptions`](Visus.LdapAuthentication/LdapOptions.cs). Alternatively, you can use your own implementation of [`ILdapOptions`](Visus.LdapAuthentication/ILdapOptions.cs). The following example illustrates a fairly minimal configuration for an Active Directory using SSL, but no certificate validation (this is what you would use for development purposes):

```JSON
{
    "LdapConfiguration": {
        "Server": "dc.your-domain.de",
        "SearchBase": "DC=your-domain,DC=de",
        "Schema": "Active Directory",
        "IsRecursiveGroupMembership": true,
        "Port": 636,
        "IsSsl": true,
        "IsNoCertificateCheck": true
    }
}
```

While you can fully customise the properties and claims the library loads for a user (see below), there are certain things that must be provided. This is controlled via the `Schema` property in the JSON above. The schema selects the [`LdapMapping`](Visus.LdapAuthentication/LdapMapping.cs) the library uses the select users and determine group membership. We provide several built-in schemas for frequently used LDAP servers, namely "Active Directory" for Active Directory Domain Services, "IDMU" for Active Directory with Identity Management for Unix installed and "RFC 2307" for this RFC, which is the schema typically used be OpenLDAP servers.

The built-in schemas are hard-coded in the library like this:
```C#
new LdapMapping() {
    DistinguishedNameAttribute = "distinguishedName",
    GroupIdentityAttribute = "objectSid",
    GroupIdentityConverter = typeof(SidConverter).FullName,
    GroupsAttribute = "memberOf",
    PrimaryGroupAttribute = "primaryGroupID",
    UserFilter = "(|(sAMAccountName={0})(userPrincipalName={0}))",
    UsersFilter = "(&(objectClass=user)(objectClass=person)(!(objectClass=computer)))"
}
```

You can, however, provide your own mapping in the JSON configuration like this:
```JSON
{
    "Mapping": {
        "DistinguishedNameAttribute": "distinguishedName",
        "GroupIdentityAttribute": "objectSid",
        "GroupIdentityConverter": "Visus.LdapAuthentication.SidConverter",
        "GroupsAttribute": "memberOf",
        "PrimaryGroupAttribute": "primaryGroupID",
        "UserFilter": "(|(sAMAccountName={0})(userPrincipalName={0}))",
        "UsersFilter": "(&(objectClass=user)(objectClass=person)(!(objectClass=computer)))"
    }
}
```
Alternatively, it is also possible to customise the `Mappings` property and select a custom schema by its name (the key in `Mappings`). Finally, if you set your `LdapOptions` in code, you can customise `LdapOptions.Mapping` or `LdapOptions.Mappings` from there.

The following properties must/can be set via JSON:
| Property | Description |
|----------|-------------|
| DistinguishedNameAttribute | The name of the LDAP attribute holding the distinguished name. This property is required and should be "distinguishedName". |
| GroupIdentityAttribute | The name of the LDAP attribute holding the unique identifier of a group. For Active Directory, this is typically the SID, whereas for POSIX, it is the GID number. This property is required. |
| GroupIdentityConverter | If the group identity needs some conversion to be usable, provide the full path to your class implementing [`ILdapAttributeConverter`](Visus.LdapAuthentication/ILdapAttributeConverter.cs). |
| GroupsAttribute | The name of the LDAP attribute holding the list of groups a user is member of. This is "memberOf" in most scenarios. This property is required to create group claims. |
| PrimaryGroupAttribute | The name of the LDAP attribute storing the primary group of a user. Both, Active Directory and OpenLDAP, distinguish between the primary group and other groups, so both attributes must be provided for all group claims to be found. This property is required to create group claims. |
| RequiredGroupAttributes | An array of the attributes the library must load for group objects. This should typically not be customised as the library composes it from the other attributes set in the object. |
| UserFilter | The LDAP filter that allows the library to select the user by the user name that is input into the login field. This should cover all inputs that allow the user to bind to the LDAP server. For instance, Active Directory does not only allow for binding via the user name (`sAMAccountName`), but also via user@domain (`userPrincipalName`), so both ways need to be specified in the `UserFilter`. Technically, users could also bind via the distinguished name, but this is typcially not relevant for real-world scenarios, so our built-in mapping does not include this. If you fail to specify the correct filter here, users might be able to authenticate (bind to the LDAP server), but the authentication in the library will fail because the user object cannot be retrieved. This property is required. |
| UsersFilter | The LDAP filter that allows for selecting all users. Please note that for both, Active Directory and OpenLDAP, users are people and machines, so you want to filter on people only here. This property is required if you want to user [`ILdapSearchService`](Visus.LdapAuthentication/ILdapSearchService.cs). |

## Authenticate a user
Once configured, the middleware can be used in controllers to implement cookie-based or JWT-based authorisation. An example for a cookie-based login method looks like:
```C#
// Inject ILdapAuthenticationService to _authService field in constructor.

[HttpPost]
[AllowAnonymous]
public async Task<ActionResult<ILdapUser>> Login([FromForm] string username, [FromForm] string password) {
    try {
        var retval = this._authService.Login(username, password);
        await this.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, retval.ToClaimsPrincipal(CookieAuthenticationDefaults.AuthenticationScheme));
        return this.Ok(retval);
    } catch {
        return this.Unauthorized();
    }
}
```

## Customising the user object
The built-in [`LdapUser`](Visus.LdapAuthentication/LdapUser.cs) object provides a reasonably mapping of attributes in an Active Directory to user claims. There are two ways you can customise this behaviour.

The first one is by inheriting from [`LdapUserBase`](Visus.LdapAuthentication/LdapUserBase.cs), which actually implements all of the behaviour of `LdapUser`. This way enables you to inherit most of this behaviour and override the mapping on a per-property base. As the mapping configured via attributes is not inherited, you can simply override a property and attach a new mapping like this:

```C#
public sealed class CustomApplicationUser : LdapUserBase {

    /// <summary>
    /// The user's account name.
    /// </summary>
    /// <remarks>
    /// Here, the &quot;userPrincipalName&quot; is used instead of
    /// &quot;sAMAccountName&quot; used by <see cref="LdapUser" />. Furthermore,
    /// only the <see cref="ClaimTypes.WindowsAccountName" /> claim is set to
    /// this property, whereas <see cref="LdapUser" /> also sets
    /// <see cref="ClaimTypes.Name" />. All other attribute mappings and claim
    /// mappings are inherited from <see cref="LdapUserBase" /> and therefore
    /// behave like the default <see cref="LdapUser" />.
    /// </remarks>
    [LdapAttribute(Schema.ActiveDirectory, "userPrincipalName")]
    [Claim(ClaimTypes.WindowsAccountName)]
    public override string AccountName => base.AccountName;
}
```

If you need an even higher level of customisation, you can provide a completely new implementation of `ILdapUser` and fully control the whole mapping of LDAP attributes to properties and claims. Before doing so, you should also consider whether you can achieve your goals by overriding one or more of `LdapUserBase.AddGroupClaims` and `LdapUserBase.AddPropertyClaims`.

## Searching users
In some cases, you might want to search users objects without authenticating the user of your application. One of these cases might be restoring the user object from the claims stored in a cookie. A service account specified in `ILdapOptions.User` with a password stored in `ILdapOptions.Password` can be used in conjuction with a [`ILdapSearchService`](Visus.LdapAuthentication/ILdapSearchService.cs) to implement such a behaviour. First, configure the service:

```C#
public void ConfigureServices(IServiceCollection services) {
    // ...
    
    // Add LDAP search service using service account.
    {
        var options = new LdapOptions();
        this.Configuration.GetSection("LdapConfiguration").Bind(options);
        services.AddLdapSearchService<LdapUser>(options);
    }
    
    // ...
}
```

Assuming that you have the embedded the user SID in the claims of an authentication cookie, you then can restore the user object from the cookie as follows:

```C#
// Inject ILdapSearchService to _ldapSearchService field in constructor.

[HttpGet]
[Authorize]
public ActionResult<ILdapUser> GetUser() {
    if (this.User != null) {
        // Determine the claims we know that the authentication service has
        // stored the SID to.
        var claims = ClaimAttribute.GetClaims<LdapUser>(nameof(LdapUser.Identity));

        // Return the first valid SID that allows for reconstructing the user.
        foreach (var c in claims) {
            var sid = this.User.FindFirstValue(c);
            if (!string.IsNullOrEmpty(sid)) {
                var retval = this._ldapSearchService.GetUserByIdentity(sid);

                if (retval != null) {
                    return this.Ok(retval);
                }
            }
        }
    }

    // If something went wrong, we assume that the (anonymous) user must not
    // access the user details.
    return this.Unauthorized();
}
```
