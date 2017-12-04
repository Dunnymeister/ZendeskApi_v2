using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using ZendeskApi_v2;
using ZendeskApi_v2.Models.Constants;
using ZendeskApi_v2.Models.Users;
using ZendeskApi_v2.Requests;
using ZendeskApi_v2.Models.Shared;
using System.IO;
using ZendeskApi_v2.Test.Util;

namespace Tests
{
    [Category("Users")]
    public class UserTests
    {
        private const string ResourceName = "ZendeskApi_v2.Test.Resources.gracehoppertocat3.jpg";
        private ZendeskApi api = new ZendeskApi(Settings.Site, Settings.AdminEmail, Settings.AdminPassword);

        [Fact]
        public void CanGetUsers()
        {
            var res = api.Users.GetAllUsers();
            Assert.True(res.Count > 0);
        }

        [Fact]
        public void CanGetAgents()
        {
            var res = api.Users.GetAllAgents();
            Assert.True(res.Count > 0);
        }

        [Fact]
        public void CanGetAdmins()
        {
            var res = api.Users.GetAllAdmins();
            Assert.True(res.Count > 0);
        }

        [Fact]
        public void CanGetEndUsers()
        {
            var res = api.Users.GetAllEndUsers();
            Assert.True(res.Count > 0);
        }

        [Fact]
        public void CanGetAllUsersInRoles()
        {
            var res = api.Users.GetAllUsersInRoles(agents: true, admins: true);
            Assert.True(res.Count > 0);
        }

        [Fact]
        public void CanGetUserByCustomField()
        {
            var res = api.Users.SearchByCustomUserField(Settings.FieldKey, Settings.FieldValue);
            var user = res.Users.FirstOrDefault();

            Assert.NotNull(user);
            Assert.Equal(1158278453, user.Id);
        }

        [Fact]
        public void CannotGetUserByCustomField()
        {
            var res = api.Users.SearchByCustomUserField(Settings.FieldKey, Settings.BadFieldValue);

            Assert.Equal(0, res.Users.Count);
            Assert.Null(res.Users.FirstOrDefault());
        }

        [Fact]
        public void CanGetUser()
        {
            var res = api.Users.GetUser(Settings.UserId);
            Assert.True(res.User.Id == Settings.UserId);
        }

        [Fact]
        public void CanGetUsersInGroup()
        {
            var res = api.Users.GetUsersInGroup(Settings.GroupId);
            Assert.True(res.Count > 0);
        }

        [Fact]
        public void CanGetUsersInOrg()
        {
            var res = api.Users.GetUsersInOrganization(Settings.OrganizationId);
            Assert.True(res.Count > 0);
        }

        [Fact]
        public void CanCreateUpdateSuspendAndDeleteUser()
        {
            var list = api.Users.GetAllUsers();
            var users = list.Users.Where(x => x.Email == "test772@tester.com");

            foreach (var u in users)
            {
                api.Users.DeleteUser(u.Id.Value);
            }

            var user = new User()
            {
                Name = "tester user72",
                Email = "test772@tester.com",
                Role = "end-user",
                Verified = true,
                CustomFields = new Dictionary<string, object>()
                                  {
                                      {"user_dropdown", "option_1"}
                                  }
            };

            var res1 = api.Users.CreateUser(user);
            var userId = res1.User.Id ?? 0;
            Assert.True(res1.User.Id > 0);

            Assert.True(api.Users.SetUsersPassword(userId, "t34sssting"));
            Assert.True(api.Users.ChangeUsersPassword(userId, "t34sssting", "newpassw33rd"));

            res1.User.Phone = "555-555-5555";
            res1.User.RemotePhotoUrl = "http://i.imgur.com/b2gxj.jpg";

            var res2 = api.Users.UpdateUser(res1.User);
            var blah = api.Users.GetUser(res1.User.Id.Value);
            Assert.Equal(res1.User.Phone, res2.User.Phone);

            var res3 = api.Users.SuspendUser(res2.User.Id.Value);
            Assert.True(res3.User.Suspended);

            var res4 = api.Users.DeleteUser(res3.User.Id.Value);
            Assert.True(res4);

            //check the remote photo url
            //Assert.Equal(res1.User.RemotePhotoUrl, res2.User.RemotePhotoUrl);
        }

        [Fact]
        public void CanFindUser()
        {
            //var res1 = api.Users.SearchByEmail(Settings.Email);
            var res1 = api.Users.SearchByEmail(Settings.ColloboratorEmail);
            Assert.True(res1.Users.Count > 0);
        }

        [Fact]
        public void CanFindUserByPhone()
        {
            var res1 = api.Users.SearchByPhone(Settings.Phone);
            Assert.True(res1.Users.Count > 0);
            Assert.Equal(Settings.Phone, res1.Users.First().Phone);
            Assert.Equal("0897c9c1f80646118a8194c942aa84cf 162a3d865f194ef8b7a2ad3525ea6d7c", res1.Users.First().Name);
        }

        [Fact]
        public void CanFindUserByFormattedPhone()
        {
            var res1 = api.Users.SearchByPhone(Settings.FormattedPhone);
            Assert.True(res1.Users.Count > 0);
            Assert.Equal(Settings.FormattedPhone, res1.Users.First().Phone);
            Assert.Equal("dc4d7cf57d0c435cbbb91b1d4be952fe 504b509b0b1e48dda2c8471a88f068a5", res1.Users.First().Name);
        }

        [Fact]
        public void CanFindUserByPhoneAsync()
        {
            var res1 = api.Users.SearchByPhoneAsync(Settings.Phone).Result;
            Assert.True(res1.Users.Count > 0);
            Assert.Equal(Settings.Phone, res1.Users.First().Phone);
            Assert.Equal("0897c9c1f80646118a8194c942aa84cf 162a3d865f194ef8b7a2ad3525ea6d7c", res1.Users.First().Name);
        }

        [Fact]
        public void CannotFindUserByPhone()
        {
            var res1 = api.Users.SearchByPhone(Settings.BadPhone);
            Assert.True(res1.Users.Count == 0);
        }

        [Fact]
        public void CannotFindUserByPhoneAsync()
        {
            var res1 = api.Users.SearchByPhoneAsync(Settings.BadPhone).Result;
            Assert.True(res1.Users.Count == 0);
        }

        [Fact]
        public void CanGetCurrentUser()
        {
            var res1 = api.Users.GetCurrentUser();
            Assert.True(res1.User.Id > 0);
        }

        [Fact]
        public void CanGetUserIdentities()
        {
            var res = api.Users.GetCurrentUser();

            var res1 = api.Users.GetUserIdentities(res.User.Id.Value);
            Assert.True(res1.Identities[0].Id > 0);

            var res2 = api.Users.GetSpecificUserIdentity(res.User.Id.Value, res1.Identities[0].Id.Value);
            Assert.True(res2.Identity.Id > 0);
        }

        [Fact]
        public void CanCreateUpdateAndDeleteIdentities()
        {
            var user = new User()
            {
                Name = "test user10",
                Email = "test10@test.com",
            };

            var existingUser = api.Users.SearchByEmail(user.Email);
            if (existingUser.Count > 0)
            {
                api.Users.DeleteUser(existingUser.Users[0].Id.Value);
            }

            var res1 = api.Users.CreateUser(user);
            var userId = res1.User.Id.Value;

            var res2 = api.Users.AddUserIdentity(userId, new UserIdentity()
            {
                Type = UserIdentityTypes.Email,
                Value = "moretest@test.com"
            });
            var identityId = res2.Identity.Id.Value;
            Assert.True(identityId > 0);

            var verfified = api.Users.SetUserIdentityAsVerified(userId, identityId);
            Assert.Equal(identityId, verfified.Identity.Id);

            var primaries = api.Users.SetUserIdentityAsPrimary(userId, identityId);
            Assert.Equal(identityId, primaries.Identities.First(x => x.Primary).Id);

            Assert.True(api.Users.DeleteUserIdentity(userId, identityId));
            Assert.True(api.Users.DeleteUser(userId));
        }

        [Fact]
        public async Task CanMergeUsersAsync()
        {
            var user1 = new User
            {
                Name = Guid.NewGuid().ToString("N") + " " + Guid.NewGuid().ToString("N"),
                Email = Guid.NewGuid().ToString("N") + "@" + Guid.NewGuid().ToString("N") + ".com"
            };

            var user2 = new User
            {
                Name = Guid.NewGuid().ToString("N") + " " + Guid.NewGuid().ToString("N"),
                Email = Guid.NewGuid().ToString("N") + "@" + Guid.NewGuid().ToString("N") + ".com"
            };

            var resultUser1 = api.Users.CreateUser(user1);
            var resultUser2 = api.Users.CreateUser(user2);

            var mergedUser = await api.Users.MergeUserAsync(resultUser1.User.Id.Value, resultUser2.User.Id.Value);

            await Task.Delay(1000);
            var mergedIdentities = await api.Users.GetUserIdentitiesAsync(mergedUser.User.Id.Value);

            Assert.Equal(resultUser2.User.Id, mergedUser.User.Id);
            Assert.True(mergedIdentities.Identities.Any(i => i.Value.ToLower() == user1.Email.ToLower()));
            Assert.True(mergedIdentities.Identities.Any(i => i.Value.ToLower() == user2.Email.ToLower()));

            api.Users.DeleteUser(resultUser1.User.Id.Value);
            api.Users.DeleteUser(resultUser2.User.Id.Value);
        }

        [Fact]
        public void CanGetMultipleUsers()
        {
            var userList = api.Users.GetAllUsers(10, 1).Users.Select(u => u.Id.Value).ToList();
            var result = api.Users.GetMultipleUsers(userList, UserSideLoadOptions.Organizations | UserSideLoadOptions.Identities | UserSideLoadOptions.Roles);

            Assert.Equal(userList.Count, result.Count);
            Assert.True((result.Organizations != null && result.Organizations.Any()) || (result.Identities != null && result.Identities.Any()));
        }

        [Fact]
        public void CanGetMultipleUsersAsync()
        {
            var userList = api.Users.GetAllUsersAsync(10, 1).Result.Users.Select(u => u.Id.Value).ToList();
            var result = api.Users.GetMultipleUsers(userList, UserSideLoadOptions.Organizations | UserSideLoadOptions.Identities);
            Assert.Equal(userList.Count, result.Count);
            Assert.True((result.Organizations != null && result.Organizations.Any()) || (result.Identities != null && result.Identities.Any()));
        }

        [Fact]
        public void CanSetUserPhoto()
        {
            var fileData = ResourceUtil.GetResource(ResourceName);

            var file = new ZenFile()
            {
                ContentType = "image/jpeg",
                FileName = "gracehoppertocat3.jpg",
                FileData = fileData
            };

            var user = api.Users.SetUserPhoto(Settings.UserId, file);
            Assert.NotNull(user.User.Photo.ContentUrl);
            Assert.True(user.User.Photo.Size != 0);
        }

        [Fact]
        public async Task CanSetUserPhotoAsync()
        {
            var fileData = ResourceUtil.GetResource(ResourceName);
            var file = new ZenFile()
            {
                ContentType = "image/jpeg",
                FileName = "gracehoppertocat3.jpg",
                FileData = fileData
            };

            var user = await api.Users.SetUserPhotoAsync(Settings.UserId, file);
            Assert.NotNull(user.User.Photo.ContentUrl);
            Assert.True(user.User.Photo.Size != 0);
        }

        [Fact]
        public void CanGetUserRelatedInformation()
        {
            //Arrange
            var userId = Settings.UserId;

            //Act
            var result = api.Users.GetUserRelatedInformation(userId);

            //Assert
            Assert.NotNull(result);
            Assert.IsAssignableFrom<IndividualUserRelatedInformationResponse>(result);
        }

        [Fact]
        public async Task CanGetUserRelatedInformationAsync()
        {
            //Arrange
            var userId = Settings.UserId;

            //Act
            var result = await api.Users.GetUserRelatedInformationAsync(userId);

            //Assert
            Assert.NotNull(result);
            Assert.IsAssignableFrom<IndividualUserRelatedInformationResponse>(result);
        }

        [Fact]
        public async Task CanCreateUpdateAndDeleteIdentitiesAsync()
        {
            var user = new User()
            {
                Name = "test user10",
                Email = "test10@test.com",
            };

            var existingUser = await api.Users.SearchByEmailAsync(user.Email);
            if (existingUser.Count > 0)
            {
                await api.Users.DeleteUserAsync(existingUser.Users[0].Id.Value);
            }

            var res1 = await api.Users.CreateUserAsync(user);
            var userId = res1.User.Id.Value;

            var res2 = await api.Users.AddUserIdentityAsync(userId, new UserIdentity
            {
                Type = UserIdentityTypes.Email,
                Value = "moretest@test.com"
            });

            var identityId = res2.Identity.Id.Value;
            Assert.True(identityId > 0);
            res2.Identity.Value = "moretest2@test.com";

            await api.Users.UpdateUserIdentityAsync(userId, res2.Identity);

            var res3 = await api.Users.GetSpecificUserIdentityAsync(userId, identityId);

            Assert.Equal(res3.Identity.Id, identityId);
            Assert.Equal(res3.Identity.Value, res2.Identity.Value);

            Assert.True(api.Users.DeleteUserIdentity(userId, identityId));
            Assert.True(api.Users.DeleteUser(userId));
        }
    }
}
