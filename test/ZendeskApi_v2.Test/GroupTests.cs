using System.Linq;
using Xunit;
using ZendeskApi_v2;
using ZendeskApi_v2.Models.Constants;
using ZendeskApi_v2.Models.Users;
using ZendeskApi_v2.Models.Groups;
using System.Threading.Tasks;
using ZendeskApi_v2.Models.Search;

namespace Tests
{
    public class GroupTests
    {
        private ZendeskApi api = new ZendeskApi(Settings.Site, Settings.AdminEmail, Settings.AdminPassword);

        public GroupTests()
        {
            var resp = api.Search.SearchForAsync<User>("test133@test.com").Result;

            foreach (var user in resp.Results)
            {
                api.Users.DeleteUserAsync(user.Id.Value);
            }
        }


        [Fact]
        public void CanGetGroups()
        {
            var res = api.Groups.GetGroups();
            Assert.True(res.Count > 0);
        }

        [Fact]
        public void CanGetAssignableGroups()
        {
            var res = api.Groups.GetAssignableGroups();
            Assert.True(res.Count > 0);
        }

        [Fact]
        public void CanGetGroup()
        {
            var res = api.Groups.GetGroups();
            var res1 = api.Groups.GetGroupById(res.Groups[0].Id.Value);

            Assert.Equal(res1.Group.Id.Value, res.Groups[0].Id.Value);
        }

        [Fact]
        public void CanCreateUpdateAndDeleteGroup()
        {
            var res = api.Groups.CreateGroup("Test Group");
            Assert.True(res.Group.Id > 0);

            res.Group.Name = "Updated Test Group";
            var res1 = api.Groups.UpdateGroup(res.Group);
            Assert.Equal(res1.Group.Name, res.Group.Name);

            Assert.True(api.Groups.DeleteGroup(res.Group.Id.Value));
        }

        [Fact]
        public void CanGetGroupMemberships()
        {
            var res = api.Groups.GetGroupMemberships();
            Assert.True(res.Count > 0);

            var res1 = api.Groups.GetGroupMembershipsByUser(Settings.UserId);
            Assert.True(res1.Count > 0);

            var groups = api.Groups.GetGroups();
            var res2 = api.Groups.GetGroupMembershipsByGroup(groups.Groups[0].Id.Value);
            Assert.True(res2.Count > 0);
        }

        [Fact]
        public void CanGetAssignableGroupMemberships()
        {
            var res = api.Groups.GetAssignableGroupMemberships();
            Assert.True(res.Count > 0);

            var groups = api.Groups.GetGroups();
            var res1 = api.Groups.GetAssignableGroupMembershipsByGroup(groups.Groups[0].Id.Value);
            Assert.True(res1.Count > 0);
        }

        [Fact]
        public void CanGetIndividualGroupMemberships()
        {
            var res = api.Groups.GetGroupMemberships();

            var res1 = api.Groups.GetGroupMembershipsByMembershipId(res.GroupMemberships[0].Id.Value);
            Assert.Equal(res1.GroupMembership.Id, res.GroupMemberships[0].Id);

            var res2 = api.Groups.GetGroupMembershipsByUserAndMembershipId(res1.GroupMembership.UserId, res.GroupMemberships[0].Id.Value);
            Assert.Equal(res2.GroupMembership.UserId, res1.GroupMembership.UserId);
        }

        [Fact]
        public void CanCreateUpdateAndDeleteMembership()
        {
            var group = api.Groups.CreateGroup("Test Group 2").Group;
            var user = api.Users.CreateUser(new User()
            {
                Name = "test user133",
                Email = "test133@test.com",
                Role = UserRoles.Agent
            }).User;

            var res = api.Groups.CreateGroupMembership(new GroupMembership()
            {
                UserId = user.Id.Value,
                GroupId = group.Id.Value
            });

            Assert.True(res.GroupMembership.Id > 0);

            var res2 = api.Groups.SetGroupMembershipAsDefault(user.Id.Value, res.GroupMembership.Id.Value);
            Assert.True(res2.GroupMemberships.First(x => x.Id == res.GroupMembership.Id).Default);

            Assert.True(api.Groups.DeleteGroupMembership(res.GroupMembership.Id.Value));
            Assert.True(api.Users.DeleteUser(user.Id.Value));
            Assert.True(api.Groups.DeleteGroup(group.Id.Value));
        }
    }
}