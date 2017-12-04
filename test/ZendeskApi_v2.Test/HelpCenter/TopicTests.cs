using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using ZendeskApi_v2;
using ZendeskApi_v2.Models.HelpCenter.Topics;

namespace Tests.HelpCenter
{
    [Category("HelpCenter")]
    public class TopicTests
    {
        private ZendeskApi api = new ZendeskApi(Settings.Site, Settings.AdminEmail, Settings.AdminPassword);

        [Fact]
        public void CanGetTopic()
        {
            var res = api.HelpCenter.Topics.GetTopic(200298245);
            Assert.NotNull(res?.Topic);
        }

        [Fact]
        public void CanGetTopics()
        {
            var res = api.HelpCenter.Topics.GetTopics();
            Assert.True(res.Topics.Count > 0);
        }

        [Fact]
        public void CanCreateUpdateAndDeleteTopic()
        {
            var topic = new Topic { Name = "This is a Test" };

            var res = api.HelpCenter.Topics.CreateTopic(topic);
            Assert.NotNull(res?.Topic);

            res.Topic.Description = "More Testing";
            var update = api.HelpCenter.Topics.UpdateTopic(res.Topic).Topic;
            Assert.Equal(update.Description, "More Testing");

            Assert.True(api.HelpCenter.Topics.DeleteTopic(res.Topic.Id.Value));
        }
    }
}
