﻿// <copyright file="ILdapConnectionService.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2021 - 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.DirectoryServices.Protocols;


namespace Visus.DirectoryAuthentication {

    /// <summary>
    /// Interface of a service providing access to a configured LDAP connection.
    /// </summary>
    public interface ILdapConnectionService {

        /// <summary>
        /// Connect to the preconfigured LDAP service with the specified
        /// credentials.
        /// </summary>
        /// <param name="username">The user name used to perform the LDAP bind.
        /// If both, <paramref name="username"/> and <paramref name="password"/>
        /// are <c>null</c>, the service shall perform an anonymous bind.
        /// </param>
        /// <param name="password">The password used to perfom the LDAP bind.
        /// If both, <paramref name="username"/> and <paramref name="password"/>
        /// are <c>null</c>, the service shall perform an anonymous bind.
        /// </param>
        /// <returns>A new LDAP connection</returns>
        LdapConnection Connect(string? username, string? password);

        /// <summary>
        /// Connect to the preconfigured LDAP service with the preconfigured
        /// credentials from <see cref="Configuration.LdapOptions"/>.
        /// </summary>
        /// <returns>A new LDAP connection.</returns>
        LdapConnection Connect() => this.Connect(null, null);
    }
}
