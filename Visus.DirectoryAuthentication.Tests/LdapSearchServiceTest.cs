﻿// <copyright file="LdapSearchServiceTest.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2021 - 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;
using Visus.Ldap;
using Visus.Ldap.Configuration;
using Visus.Ldap.Mapping;


namespace Visus.DirectoryAuthentication.Tests {

    [TestClass]
    public class LdapSearchServiceTest {

        #region Public constructors
        public LdapSearchServiceTest() {
            if (this._testSecrets.CanRun) {
                var configuration = TestExtensions.CreateConfiguration();
                var collection = new ServiceCollection().AddMockLoggers();
                collection.AddLdapAuthentication(o => {
                    var section = configuration.GetSection("LdapOptions");
                    section.Bind(o);
                });
                this._services = collection.BuildServiceProvider();
            }
        }
        #endregion

        [TestMethod]
        public void TestGetGroupByAccountName() {
            if (this._testSecrets.CanRun) {
                Assert.IsNotNull(this._services);
                var service = this._services.GetService<ILdapSearchService<LdapUser, LdapGroup>>();
                Assert.IsNotNull(service);

                Assert.AreEqual(GroupCaching.None, this._testSecrets.LdapOptions.GroupCaching, "Caching disabled.");
                var group = service.GetGroupByName(this._testSecrets.ExistingGroupAccount!);
                Assert.IsNotNull(group, "Search returned existing group account.");
            }
        }

        [TestMethod]
        public async Task TestGetGroupByAccountNameAsync() {
            if (this._testSecrets.CanRun) {
                Assert.IsNotNull(this._services);
                var service = this._services.GetService<ILdapSearchService<LdapUser, LdapGroup>>();
                Assert.IsNotNull(service);

                Assert.AreEqual(GroupCaching.None, this._testSecrets.LdapOptions.GroupCaching, "Caching disabled.");
                var group = await service.GetGroupByNameAsync(this._testSecrets.ExistingGroupAccount!);
                Assert.IsNotNull(group, "Search returned existing group account.");
            }
        }

        [TestMethod]
        public void TestGetGroupByIdentity() {
            if (this._testSecrets.CanRun) {
                Assert.IsNotNull(this._services);
                var service = this._services.GetService<ILdapSearchService<LdapUser, LdapGroup>>();
                Assert.IsNotNull(service);

                Assert.AreEqual(GroupCaching.None, this._testSecrets.LdapOptions.GroupCaching, "Caching disabled.");
                var group = service.GetGroupByIdentity(this._testSecrets.ExistingGroupIdentity!);
                Assert.IsNotNull(group, "Search returned existing group identity.");
            }
        }

        [TestMethod]
        public async Task TestGetGroupByIdentityAsync() {
            if (this._testSecrets.CanRun) {
                Assert.IsNotNull(this._services);
                var service = this._services.GetService<ILdapSearchService<LdapUser, LdapGroup>>();
                Assert.IsNotNull(service);

                Assert.AreEqual(GroupCaching.None, this._testSecrets.LdapOptions.GroupCaching, "Caching disabled.");
                var group = await service.GetGroupByIdentityAsync(this._testSecrets.ExistingGroupIdentity!);
                Assert.IsNotNull(group, "Search returned existing group identity.");
            }
        }

        [TestMethod]
        public void TestGetUserByAccountName() {
            if (this._testSecrets.CanRun) {
                Assert.IsNotNull(this._services);
                var service = this._services.GetService<ILdapSearchService<LdapUser, LdapGroup>>();
                Assert.IsNotNull(service);

                var user = service.GetUserByAccountName(this._testSecrets.ExistingUserAccount!);
                Assert.IsNotNull(user, "Search returned existing user account.");
                Assert.IsNotNull(user.Groups);
                Assert.IsTrue(user.Groups.Count() > 1);
            }
        }

        [TestMethod]
        public async Task TestGetUserByAccountNameAsync() {
            if (this._testSecrets.CanRun) {
                Assert.IsNotNull(this._services);
                var service = this._services.GetService<ILdapSearchService<LdapUser, LdapGroup>>();
                Assert.IsNotNull(service);

                var user = await service.GetUserByAccountNameAsync(this._testSecrets.ExistingUserAccount!);
                Assert.IsNotNull(user, "Search returned existing user account.");
            }
        }

        [TestMethod]
        public void TestGetUserByIdentity() {
            if (this._testSecrets.CanRun) {
                Assert.IsNotNull(this._services);
                var service = this._services.GetService<ILdapSearchService<LdapUser, LdapGroup>>();
                Assert.IsNotNull(service);

                {
                    var user = service.GetUserByIdentity(this._testSecrets.ExistingUserIdentity!);
                    Assert.IsNotNull(user, "Search returned existing user identity.");
                }

                {
                    var user = service.GetUserByIdentity(this._testSecrets.NonExistingUserIdentity!);
                    Assert.IsNull(user, "Search returned no non-existing user identity.");
                }
            }
        }

        [TestMethod]
        public async Task TestGetUserByIdentityAsync() {
            if (this._testSecrets.CanRun) {
                Assert.IsNotNull(this._services);
                var service = this._services.GetService<ILdapSearchService<LdapUser, LdapGroup>>();
                Assert.IsNotNull(service);

                {
                    var user = await service.GetUserByIdentityAsync(this._testSecrets.ExistingUserIdentity!);
                    Assert.IsNotNull(user, "Search returned existing user identity.");
                }

                {
                    var user = await service.GetUserByIdentityAsync(this._testSecrets.NonExistingUserIdentity!);
                    Assert.IsNull(user, "Search returned no non-existing user identity.");
                }
            }
        }

        [TestMethod]
        public void TestGetUsersCached() {
            if (this._testSecrets.CanRun) {
                var configuration = TestExtensions.CreateConfiguration();
                var collection = new ServiceCollection().AddMockLoggers();
                collection.AddLdapAuthentication(o => {
                    var section = configuration.GetSection("LdapOptions");
                    section.Bind(o);
                    o.GroupCacheDuration = TimeSpan.FromMinutes(5);
                    o.GroupCaching = GroupCaching.SlidingExpiration;
                });
                var services = collection.BuildServiceProvider();

                var service = services.GetService<ILdapSearchService<LdapUser, LdapGroup>>();
                Assert.IsNotNull(service);

                var users = service.GetUsers();
                Assert.IsTrue(users.Any(), "Directory search returns any user.");

                foreach (var user in users) {
                    Assert.IsNotNull(user);
                }
            }
        }

        [TestMethod]
        public void TestGetUsersNonCached() {
            if (this._testSecrets.CanRun) {
                var configuration = TestExtensions.CreateConfiguration();
                var collection = new ServiceCollection().AddMockLoggers();
                collection.AddLdapAuthentication(o => {
                    var section = configuration.GetSection("LdapOptions");
                    section.Bind(o);
                    o.GroupCaching = GroupCaching.None;
                });
                var services = collection.BuildServiceProvider();

                var service = services.GetService<ILdapSearchService<LdapUser, LdapGroup>>();
                Assert.IsNotNull(service);

                var users = service.GetUsers();
                Assert.IsTrue(users.Any(), "Directory search returns any user.");
            }
        }

        [TestMethod]
        public async Task TestGetUsersAsyncCached() {
            if (this._testSecrets.CanRun) {
                var configuration = TestExtensions.CreateConfiguration();
                var collection = new ServiceCollection().AddMockLoggers();
                collection.AddLdapAuthentication(o => {
                    var section = configuration.GetSection("LdapOptions");
                    section.Bind(o);
                    o.GroupCacheDuration = TimeSpan.FromMinutes(5);
                    o.GroupCaching = GroupCaching.SlidingExpiration;
                });
                var services = collection.BuildServiceProvider();

                var service = services.GetService<ILdapSearchService<LdapUser, LdapGroup>>();
                Assert.IsNotNull(service);

                var users = await service.GetUsersAsync();
                Assert.IsTrue(users.Any(), "Directory search returns any user.");

                foreach (var user in users) {
                    Assert.IsNotNull(user);
                }
            }
        }

        [TestMethod]
        public async Task TestGetUsersAsyncNonCached() {
            if (this._testSecrets.CanRun) {
                var configuration = TestExtensions.CreateConfiguration();
                var collection = new ServiceCollection().AddMockLoggers();
                collection.AddLdapAuthentication(o => {
                    var section = configuration.GetSection("LdapOptions");
                    section.Bind(o);
                    o.GroupCaching = GroupCaching.None;
                });
                var services = collection.BuildServiceProvider();

                var service = services.GetService<ILdapSearchService<LdapUser, LdapGroup>>();
                Assert.IsNotNull(service);

                var users = await service.GetUsersAsync();
                Assert.IsTrue(users.Any(), "Directory search returns any user.");
            }
        }

        [TestMethod]
        public void TestGetUsersFiltered() {
            if (this._testSecrets.CanRun) {
                Assert.IsNotNull(this._services);
                var service = this._services.GetService<ILdapSearchService<LdapUser, LdapGroup>>();
                Assert.IsNotNull(service);

                var map = this._services.GetService<ILdapAttributeMap<LdapUser>>();
                Assert.IsNotNull(map);

                var users = service.GetUsers($"({map.AccountNameAttribute!.Name}={this._testSecrets.ExistingUserAccount})");
                Assert.IsNotNull(users.Any(), "Filtered user was found.");
            }
        }

        [TestMethod]
        public async Task TestGetUsersFilteredAsync() {
            if (this._testSecrets.CanRun) {
                Assert.IsNotNull(this._services);
                var service = this._services.GetService<ILdapSearchService<LdapUser, LdapGroup>>();
                Assert.IsNotNull(service);

                var map = this._services.GetService<ILdapAttributeMap<LdapUser>>();
                Assert.IsNotNull(map);

                var users = await service.GetUsersAsync($"({map.AccountNameAttribute!.Name}={this._testSecrets.ExistingUserAccount})");
                Assert.IsNotNull(users.Any(), "Filtered user was found.");
            }
        }

        private readonly ServiceProvider? _services;
        private readonly TestSecrets _testSecrets = TestExtensions.CreateSecrets();
    }
}
