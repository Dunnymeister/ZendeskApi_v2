using System.ComponentModel;
using System.Threading.Tasks;
using Xunit;
using ZendeskApi_v2;
using ZendeskApi_v2.Models.Sections;
using ZendeskApi_v2.Requests.HelpCenter;

namespace Tests.HelpCenter
{

    [Category("HelpCenter")]
    class SectionTests
    {
        private ZendeskApi api = new ZendeskApi(Settings.Site, Settings.AdminEmail, Settings.AdminPassword);

        [Fact]
        public void CanGetSections()
        {
            var res = api.HelpCenter.Sections.GetSections();
            Assert.True(res.Count > 0);

            var res1 = api.HelpCenter.Sections.GetSectionById(res.Sections[0].Id.Value);
            Assert.Equal(res1.Section.Id, res.Sections[0].Id.Value);
        }

        [Fact]
        public void CanCreateUpdateAndDeleteSections()
        {
            //https://csharpapi.zendesk.com/hc/en-us/categories/200382245-Category-1
            long category_id = 200382245;

            var res = api.HelpCenter.Sections.CreateSection(new Section
            {
                Name = "My Test section",
                Position = 12,
                CategoryId = category_id
            });
            Assert.True(res.Section.Id > 0);

            res.Section.Position = 42;
            var update = api.HelpCenter.Sections.UpdateSection(res.Section);
            Assert.Equal(update.Section.Position, res.Section.Position);
            Assert.True(api.HelpCenter.Sections.DeleteSection(res.Section.Id.Value));
        }

        [Fact]
        public void CanGetSectionsAsync()
        {
            var res = api.HelpCenter.Sections.GetSectionsAsync().Result;
            Assert.True(res.Count > 0);

            var res1 = api.HelpCenter.Sections.GetSectionById(res.Sections[0].Id.Value);
            Assert.Equal(res1.Section.Id, res.Sections[0].Id.Value);
        }

        [Fact]
        public async Task CanCreateUpdateAndDeleteSectionsAsync()
        {
            //https://csharpapi.zendesk.com/hc/en-us/categories/200382245-Category-1
            long category_id = 200382245;

            var res = await api.HelpCenter.Sections.CreateSectionAsync(new Section
            {
                Name = "My Test section",
                Position = 12,
                CategoryId = category_id
            });

            Assert.True(res.Section.Id > 0);

            res.Section.Position = 42;
            var update = await api.HelpCenter.Sections.UpdateSectionAsync(res.Section);
            Assert.Equal(update.Section.Position, res.Section.Position);
            Assert.True(await api.HelpCenter.Sections.DeleteSectionAsync(res.Section.Id.Value));
        }
    }
}
