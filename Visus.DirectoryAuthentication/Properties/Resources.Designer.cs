﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Visus.DirectoryAuthentication.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Visus.DirectoryAuthentication.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The LDAP SSL certificate is not valid after {0}..
        /// </summary>
        internal static string ErrorCertificateExpired {
            get {
                return ResourceManager.GetString("ErrorCertificateExpired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The LDAP SSL certificate is not valid before {0}..
        /// </summary>
        internal static string ErrorCertificateNotYetValid {
            get {
                return ResourceManager.GetString("ErrorCertificateNotYetValid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The LDAP SSL certificate has not been issued by &quot;{0}&quot;, but by &quot;{1}&quot;..
        /// </summary>
        internal static string ErrorCertIssuerMismatch {
            get {
                return ResourceManager.GetString("ErrorCertIssuerMismatch", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The LDAP SSL certificate has the thumbprint &quot;{0}&quot;, which is not one of the expected ones..
        /// </summary>
        internal static string ErrorCertThumbprintMismatch {
            get {
                return ResourceManager.GetString("ErrorCertThumbprintMismatch", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An entry with a value of &quot;{1}&quot; for &quot;{0}&quot; does not exist in the directory..
        /// </summary>
        internal static string ErrorEntryNotFound {
            get {
                return ResourceManager.GetString("ErrorEntryNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The given byte sequence &quot;{0}&quot; does not represent a valid security identifier..
        /// </summary>
        internal static string ErrorInvalidSid {
            get {
                return ResourceManager.GetString("ErrorInvalidSid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The default LdapOptions cannot be registered as interface, because this would result in an ambiguous constructor call. Use the non-generic variant of AddLdapOptions to register the default options type..
        /// </summary>
        internal static string ErrorLdapOptionsRegistration {
            get {
                return ResourceManager.GetString("ErrorLdapOptionsRegistration", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Paging of LDAP search results must start at 0..
        /// </summary>
        internal static string ErrorLdapPage {
            get {
                return ResourceManager.GetString("ErrorLdapPage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The size of a page of LDAP search results must be at least 1..
        /// </summary>
        internal static string ErrorLdapPageSize {
            get {
                return ResourceManager.GetString("ErrorLdapPageSize", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The given property &quot;{0}&quot; was not found..
        /// </summary>
        internal static string ErrorPropertyNotFound {
            get {
                return ResourceManager.GetString("ErrorPropertyNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The given property &quot;{0}&quot; has no LDAP attribute mapped for the current schema &quot;{1}&quot;..
        /// </summary>
        internal static string ErrorPropertyNotMapped {
            get {
                return ResourceManager.GetString("ErrorPropertyNotMapped", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Login succeeded, but user information for &quot;{0}&quot; could not be found..
        /// </summary>
        internal static string ErrorUserNotFound {
            get {
                return ResourceManager.GetString("ErrorUserNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Bind using the current credentials..
        /// </summary>
        internal static string InfoBindCurrent {
            get {
                return ResourceManager.GetString("InfoBindCurrent", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Performing LDAP bind as user &quot;{0}&quot;..
        /// </summary>
        internal static string InfoBindingAsUser {
            get {
                return ResourceManager.GetString("InfoBindingAsUser", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Authenticated successfully as user &quot;{0}&quot;..
        /// </summary>
        internal static string InfoBoundAsUser {
            get {
                return ResourceManager.GetString("InfoBoundAsUser", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Bound using the current credentials..
        /// </summary>
        internal static string InfoBoundCurrent {
            get {
                return ResourceManager.GetString("InfoBoundCurrent", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Checking for LDAP server certificate &quot;{0}&quot; being issued by &quot;{1}&quot;..
        /// </summary>
        internal static string InfoCheckCertIssuer {
            get {
                return ResourceManager.GetString("InfoCheckCertIssuer", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Checking that LDAP server &quot;certificate {0} has one of the following thumbprints: {1}.
        /// </summary>
        internal static string InfoCheckCertThumbprint {
            get {
                return ResourceManager.GetString("InfoCheckCertThumbprint", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Connecting to LDAP server {0}:{1}..
        /// </summary>
        internal static string InfoConnectingLdap {
            get {
                return ResourceManager.GetString("InfoConnectingLdap", resourceCulture);
            }
        }
    }
}
