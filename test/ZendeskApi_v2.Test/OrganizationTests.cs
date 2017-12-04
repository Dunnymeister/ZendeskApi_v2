using System.Collections.Generic;
using System.Linq;

using ZendeskApi_v2;
using ZendeskApi_v2.Models.Organizations;
using ZendeskApi_v2.Models.Tags;
using ZendeskApi_v2.Models.Users;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{

    public class OrganizationTests
    {
        ZendeskApi api = new ZendeskApi(Settings.Site, Settings.AdminEmail, Settings.AdminPassword);

        public OrganizationTests()
        {
            var orgs = api.Organizations.GetOrganizations();
            if (orgs != null)
            {
                foreach (var org in orgs.Organizations.Where(o => o.Name != "CsharpAPI"))
                {
                    api.Organizations.DeleteOrganization(org.Id.Value);
                }
            }

            var users = api.Users.SearchByEmail("test_org_mem@test.com");
            if (users != null)
            {
                foreach (var user in users.Users.Where(o => o.Name.Contains("Test User Org Mem")))
                {
                    api.Users.DeleteUser(user.Id.Value);
                }
            }
        }

        [Fact]
        public void CanAddAndRemoveTagsFromOrganization()
        {
            var tag = new Tag();
            var organization = api.Organizations.GetOrganizations().Organizations.First();
            tag.Name = "MM";
            organization.Tags.Add(tag.Name);

            var org = api.Organizations.UpdateOrganization(organization);
            org.Organization.Tags.Add("New");

            var org2 = api.Organizations.UpdateOrganization(org.Organization);
            org2.Organization.Tags.Remove("MM");
            org2.Organization.Tags.Remove("New");
            api.Organizations.UpdateOrganization(org2.Organization);
        }

        [Fact]
        public void CanGetOrganizations()
        {
            var res = api.Organizations.GetOrganizations();
            Assert.True(res.Count > 0);

            var org = api.Organizations.GetOrganization(res.Organizations[0].Id.Value);
            Assert.Equal(org.Organization.Id, res.Organizations[0].Id);
        }

        [Fact]
        public void CanSearchForOrganizations()
        {
            var res = api.Organizations.GetOrganizationsStartingWith(Settings.DefaultOrg.Substring(0, 3));
            Assert.True(res.Count > 0);

            var search = api.Organizations.SearchForOrganizationsByExternalId(Settings.DefaultExternalId);
            Assert.True(search.Count > 0);
        }

        [Fact]
        public void CanGetMultipleOrganizations()
        {
            var org = api.Organizations.CreateOrganization(new Organization()
            {
                Name = "Test Org"
            });

            Assert.True(org.Organization.Id > 0);

            var org2 = api.Organizations.CreateOrganization(new Organization()
            {
                Name = "Test Org2"
            });

            var orgs = api.Organizations.GetMultipleOrganizations(new[] { org.Organization.Id.Value, org2.Organization.Id.Value });
            Assert.Equal(orgs.Organizations.Count, 2);
        }

        [Fact]
        public void CanCreateUpdateAndDeleteOrganizations()
        {
            var res = api.Organizations.CreateOrganization(new Organization()
            {
                Name = "Test Org"
            });

            Assert.True(res.Organization.Id > 0);

            res.Organization.Notes = "Here is a sample note";
            var update = api.Organizations.UpdateOrganization(res.Organization);
            Assert.Equal(update.Organization.Notes, res.Organization.Notes);

            Assert.True(api.Organizations.DeleteOrganization(res.Organization.Id.Value));
        }

        [Fact]
        public void CanCreateAndDeleteOrganizationMemberships()
        {
            var org = api.Organizations.CreateOrganization(new Organization()
            {
                Name = "Test Org"
            });

            var user = new User()
            {
                Name = "Test User Org Mem",
                Email = "test_org_mem@test.com",
                Role = "end-user"
            };

            var res = api.Users.CreateUser(user);

            var org_membership = new OrganizationMembership() { UserId = res.User.Id, OrganizationId = org.Organization.Id };

            var res2 = api.Organizations.CreateOrganizationMembership(org_membership);

            Assert.True(res2.OrganizationMembership.Id > 0);
            Assert.True(api.Organizations.DeleteOrganizationMembership(res2.OrganizationMembership.Id.Value));
            Assert.True(api.Users.DeleteUser(res.User.Id.Value));
            Assert.True(api.Organizations.DeleteOrganization(org.Organization.Id.Value));
        }

        [Fact(Skip="Support ticket opend will update when I(Elizabeth) have a fix ")]
        public void CanCreateManyAndDeleteOrganizationMemberships()
        {
            var org = api.Organizations.CreateOrganization(new Organization()
            {
                Name = "Test Org"
            });

            Assert.True(org.Organization.Id > 0);

            var org2 = api.Organizations.CreateOrganization(new Organization()
            {
                Name = "Test Org2"
            });

            Assert.True(org2.Organization.Id > 0);

            var res = api.Users.CreateUser(new User()
            {
                Name = "Test User Org Mem",
                Email = "test_org_mem@test.com",
                Role = "end-user"
            });

            Assert.True(res.User.Id > 0);

            var memberships = new List<OrganizationMembership>();
            memberships.Add(new OrganizationMembership() { UserId = res.User.Id, OrganizationId = org.Organization.Id });
            memberships.Add(new OrganizationMembership() { UserId = res.User.Id, OrganizationId = org2.Organization.Id });

            var job = api.Organizations.CreateManyOrganizationMemberships(memberships).JobStatus;

            var sleep = 2000;
            var retries = 0;
            while (!job.Status.Equals("completed") && retries < 7)
            {
                System.Threading.Thread.Sleep(sleep);
                job = api.JobStatuses.GetJobStatus(job.Id).JobStatus;
                sleep = (sleep < 64000 ? sleep *= 2 : 64000);
                retries++;
            }

            Assert.True(job.Results.Count() > 0);

            Assert.True(api.Organizations.DeleteOrganizationMembership(job.Results[0].Id));
            Assert.True(api.Organizations.DeleteOrganizationMembership(job.Results[1].Id));

            Assert.True(api.Users.DeleteUser(res.User.Id.Value));
            Assert.True(api.Organizations.DeleteOrganization(org.Organization.Id.Value));
            Assert.True(api.Organizations.DeleteOrganization(org2.Organization.Id.Value));
        }

        [Fact]
        public async Task CanSearchForOrganizationsAsync()
        {
            var search = await api.Organizations.SearchForOrganizationsAsync(Settings.DefaultExternalId);
            Assert.True(search.Count > 0);
        }
    }
}