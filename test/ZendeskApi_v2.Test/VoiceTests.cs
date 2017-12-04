using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using ZendeskApi_v2;

namespace Tests
{
    [Category("Voice")]
    public class VoiceTests
    {
        private ZendeskApi api = new ZendeskApi(Settings.Site, Settings.AdminEmail, Settings.AdminPassword);
        private long agentid;
        private long ticketid;
        private long userid;

        [Fact]
        public void OpenTicketForAgent()
        {
            agentid = Settings.UserId;
            ticketid = Settings.SampleTicketId;

            var result = api.Voice.OpenTicketInAgentBrowser(agentid, ticketid);
            Assert.IsTrue(result);
        }

        [Fact]
        public void OpenTicketTabForAgentAsync()
        {
            agentid = Settings.UserId;
            ticketid = Settings.SampleTicketId;

            var result = api.Voice.OpenTicketInAgentBrowserAsync(agentid, ticketid);
            Assert.IsTrue(result.Result);
        }

        [Fact]
        public void OpenUserProfileInAgentBrowser()
        {
            agentid = Settings.UserId;
            userid = Settings.EndUserId;

            var result = api.Voice.OpenUserProfileInAgentBrowser(agentid, userid);
            Assert.IsTrue(result);
        }

        [Fact]
        public void OpenUserProfileInAgentBrowserAsync()
        {
            agentid = Settings.UserId;
            userid = Settings.EndUserId;

            var result = api.Voice.OpenUserProfileInAgentBrowserAsync(agentid, userid);
            Assert.IsTrue(result.Result);
        }

        [Fact]
        public void GetAllAgentAvailability()
        {
            var res = api.Voice.GetVoiceAgentActivity();

            var agent = res.AgentActivity.FirstOrDefault();
            Assert.NotNull(agent);
            Assert.AreEqual(2110053086, agent.AgentId); 
        }

        [Fact]
        public void GetAllAgentAvailabilityAsync()
        {
            var res = api.Voice.GetVoiceAgentActivityAsync();

            var agent = res.Result.AgentActivity.FirstOrDefault();
            Assert.NotNull(agent);
            Assert.AreEqual(2110053086, agent.AgentId); 
        }

        [Fact]
        public void GetHistoricalQueueActivity()
        {
            var res = api.Voice.GetHistoricalQueueActivity();

            Assert.NotNull(res);
        }

        [Fact]
        public void GetHistoricalQueueActivityAsync()
        {
            var res = api.Voice.GetHistoricalQueueActivityAsync();

            Assert.NotNull(res.Result.Details);
        }

    }
}
