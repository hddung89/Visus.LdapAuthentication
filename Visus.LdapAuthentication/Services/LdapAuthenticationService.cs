﻿// <copyright file="LdapAuthenticationService.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2021 - 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Novell.Directory.Ldap;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Visus.Ldap;
using Visus.Ldap.Claims;
using Visus.Ldap.Extensions;
using Visus.Ldap.Mapping;
using Visus.LdapAuthentication.Configuration;
using Visus.LdapAuthentication.Extensions;


namespace Visus.LdapAuthentication.Services {

    /// <summary>
    /// Implements an <see cref="IAuthenticationService"/> using Novell's LDAP
    /// library.
    /// </summary>
    /// <typeparam name="TUser">The type of the user object to be returned on
    /// login. This is typically something derived from
    /// <see cref="LdapUserBase"/> rather than a custom implementation of
    /// <see cref="ILdapUser"/>.</typeparam>
    public sealed class LdapAuthenticationService<TUser, TGroup>
            : ILdapAuthenticationService<TUser>
            where TUser : class, new ()
            where TGroup : class, new () {

        #region Public constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="connectionService"></param>
        /// <param name="mapper"></param>
        /// <param name="claimsBuilder"></param>
        /// <param name="logger">A logger for presisting important messages like
        /// login failures.</param>
        /// <exception cref="ArgumentNullException">If any of the parameters is
        /// <c>null</c>.</exception>
        public LdapAuthenticationService(IOptions<LdapOptions> options,
                ILdapConnectionService connectionService,
                ILdapMapper<LdapEntry, TUser, TGroup> mapper,
                IClaimsBuilder<TUser, TGroup> claimsBuilder,
                ILogger<LdapAuthenticationService<TUser, TGroup>> logger) {
            this._claimsBuilder = claimsBuilder
                ?? throw new ArgumentNullException(nameof(claimsBuilder));
            this._connectionService = connectionService
                ?? throw new ArgumentNullException(nameof(connectionService));
            this._logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
            this._options = options?.Value
                ?? throw new ArgumentNullException(nameof(options));
            this._mapper = mapper
                ?? throw new ArgumentNullException(nameof(mapper));

            Debug.Assert(this._options.Mapping != null);
            this._userAttributes = this._mapper.RequiredUserAttributes
                .Append(this._options.Mapping.PrimaryGroupAttribute)
                .Append(this._options.Mapping.GroupsAttribute)
                .ToArray();
        }
        #endregion

        #region Public methods
        /// <inheritdoc />
        public ClaimsPrincipal? LoginPrincipal(string username,
                string password,
                string? authenticationType,
                string? nameType,
                string? roleType,
                ClaimFilter? filter) {
            // Note: It is important to pass a non-null password to make sure
            // that end users do not authenticate as the server process.
            var user = this.LoginUser(username, password);

            // TODO: use direct mapper instead.
            if (user != null) {
                var claims = this._claimsBuilder.GetClaims(user, filter);
                var identity = new ClaimsIdentity(claims,
                    authenticationType, nameType, roleType);
                return new ClaimsPrincipal(identity);
            } else {
                return null;
            }
        }

        /// <inheritdoc />
        public async Task<ClaimsPrincipal?> LoginPrincipalAsync(string username,
                string password,
                string? authenticationType,
                string? nameType,
                string? roleType,
                ClaimFilter? filter) {
            // Note: It is important to pass a non-null password to make sure
            // that end users do not authenticate as the server process.
            var user = await this.LoginUserAsync(username, password);

            // TODO: use direct mapper instead.
            if (user != null) {
                var claims = this._claimsBuilder.GetClaims(user, filter);
                var identity = new ClaimsIdentity(claims,
                    authenticationType, nameType, roleType);
                return new ClaimsPrincipal(identity);
            } else {
                return null;
            }
        }

        /// <inheritdoc />
        public TUser? LoginUser(string username, string password) {
            // Note: It is important to pass a non-null password to make sure
            // that end users do not authenticate as the server process.
            var connection = this._connectionService.Connect(
                username ?? string.Empty,
                password ?? string.Empty);

            Debug.Assert(this._options.Mapping != null);
            var filter = string.Format(this._options.Mapping.UserFilter,
                username.EscapeLdapFilterExpression());

            var retval = new TUser();

            foreach (var b in this._options.SearchBases) {
                var entry = connection.Search(b, filter, this._userAttributes)
                    .FirstOrDefault();
                if (entry != null) {
                    this._mapper.MapUser(entry, retval);
                    var groups = entry.GetGroups(connection,
                        this._mapper,
                        this._options);
                    this._mapper.SetGroups(retval, groups);
                    return retval;
                }
            }

            // Not found at this point, although authentication succeeded.
            {
                var msg = Properties.Resources.ErrorUserNotFound;
                msg = string.Format(msg, username);
                throw new KeyNotFoundException(msg);
            }
        }

        /// <inheritdoc />
        public async Task<TUser?> LoginUserAsync(string username,
                string password) {
            // Note: It is important to pass a non-null password to make sure
            // that end users do not authenticate as the server process.
            var connection = this._connectionService.Connect(
                username ?? string.Empty,
                password ?? string.Empty);

            Debug.Assert(this._options.Mapping != null);
            var filter = string.Format(this._options.Mapping.UserFilter,
                username.EscapeLdapFilterExpression());

            var retval = new TUser();

            foreach (var b in this._options.SearchBases) {
                var entry = (await connection.SearchAsync(b, filter,
                    this._userAttributes, this._options.PollingInterval))
                    .FirstOrDefault();
                if (entry != null) {
                    this._mapper.MapUser(entry, retval);
                    var groups = entry.GetGroups(connection,
                        this._mapper,
                        this._options);
                    this._mapper.SetGroups(retval, groups);
                    return retval;
                }
            }

            // Not found at this point, although authentication succeeded.
            {
                var msg = Properties.Resources.ErrorUserNotFound;
                msg = string.Format(msg, username);
                throw new KeyNotFoundException(msg);
            }
        }
        #endregion

        #region Private fields
        private readonly IClaimsBuilder<TUser, TGroup> _claimsBuilder;
        private readonly ILdapConnectionService _connectionService;
        private readonly ILogger _logger;
        private readonly LdapOptions _options;
        private readonly ILdapMapper<LdapEntry, TUser, TGroup> _mapper;
        private readonly string[] _userAttributes;
        #endregion
    }
}
