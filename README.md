# ASP.NET Core LDAP Authentication Middleware

[![Build Status](https://visualisierungsinstitut.visualstudio.com/Visus.LdapAuthentication/_apis/build/status/UniStuttgart-VISUS.Visus.LdapAuthentication?branchName=main)](https://visualisierungsinstitut.visualstudio.com/Visus.LdapAuthentication/_build/latest?definitionId=6&branchName=main)
[![Visus.LdapAuthenticationVersion](https://buildstats.info/nuget/Visus.LdapAuthentication)](https://www.nuget.org/packages/Visus.LdapAuthentication)
[![Visus.DirectoryAuthenticationVersion](https://buildstats.info/nuget/Visus.DirectoryAuthentication)](https://www.nuget.org/packages/Visus.DirectoryAuthentication)

This project implements middleware for ASP.NET Core that enables authenticating users against LDAP directories like Active Directory via an LDAP bind.

There are two flavours, the first being [Visus.LdapAuthentication](Visus.LdapAuthentication/README.md), which uses [Novell's C#-only LDAP library](https://github.com/dsbenghe/Novell.Directory.Ldap.NETStandard) rather than the Windows-only DirectoryServices and is therefore running on Windows and Linux.

The second, [Visus.DirectoryAuthentication](Visus.DirectoryAuthentication/README.md), is a drop-in replacement using [System.DirectorySerices.Protocols](https://learn.microsoft.com/en-gb/dotnet/api/system.directoryservices.protocols), which is a platform-independent implementation of LDAP services since .NET 5, but requires native LDAP libraries for P/Invoke being installed.

Built-in user objects are automatically mapped to Active Directory attributes based on code annotations and include commonly used claims like user name, actual names, e-mail addresses and group memberships. If necessary, you can also provide your own user object that uses a completely different mapping of LDAP attributes to claims.


# Usage
## Visus.LdapAuthentication
See [README for Visus.LdapAuthentication](Visus.LdapAuthentication/README.md).

## Visus.DirectoryAuthentication
See [README for Visus.DirectoryAuthentication](Visus.DirectoryAuthentication/README.md)

## Differences between LdapAuthentication and DirectoryAuthentication
[Visus.DirectoryAuthentication](Visus.DirectoryAuthentication) and [Visus.LdapAuthentication](Visus.LdapAuthentication) can mostly be used interchangeably with a few exceptions:
1. Visus.DirectoryAuthentication requires .NET 8, which contains a series of bug fixes we rely on. We could not get it work on .NET 5 like Visus.LdapAuthentication.
1. `System.DirectorySerices.Protocols` requires native LDAP libraries for P/Invoke being installed. This should be the case for all Windows platforms by default, but on Linux, `libldap` must be installed.
1. `ILdapOptions` is not available. Services are configured using the `Add...(... Action<LdapOptions> options ...)` method. See  [README for Visus.DirectoryAuthentication](Visus.DirectoryAuthentication/README.md).  **This is a breaking change compared to version 0.10.0!** As of 1.15.0, all configuration methods besides the action-based one are considered obsolete in Visus.LdapAuthentication as well.
1. The `LdapOptions.Timeout` property is a `System.TimeSpan` rather than a number representing milliseconds. When configuring from JSON, use a string in the format "hh:mm:ss".
1. `LdapOptions.RootCaThumbprint` is not supported. You can, however, check the immediate issuer of the server's certificate using `ILdapOptions.ServerCertificateIssuer`.
1. `LdapOptions` does not provide the legacy string-based `SearchBase` option, but must be configured with the `IDictionary<string, System.DirectoryServices.Protocols.SearchScope>` variant. **This is a breaking change compared to version 0.4.0!**.
1. `LdapOptions` does not provide the single `Server` option, but uses an array of servers. **This is a breaking change compared to version 0.16.0!**.
1. TODO: Bind using Windows credentials.
