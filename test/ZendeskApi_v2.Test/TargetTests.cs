﻿
using System.Linq;
using Xunit;
using ZendeskApi_v2;
using ZendeskApi_v2.Models.Targets;

namespace Tests
{

    public class TargetTests
    {
        private ZendeskApi api = new ZendeskApi(Settings.Site, Settings.AdminEmail, Settings.AdminPassword);

        public TargetTests()
        {
            var targets = api.Targets.GetAllTargets();

            if (targets != null)
            {
                foreach (var target in targets.Targets.Where(o => o.Title.Contains("Test Email Target") || o.Title.Contains("Test Jira Target")))
                {
                    api.Targets.DeleteTarget(target.Id.Value);
                }
            }
        }

        [Fact]
        public void CanCreateUpdateAndDeleteTargets()
        {
            var target = new EmailTarget()
            {
                Title   = "Test Email Target",
                Active  = false,
                Email   = "test@test.com",
                Subject = "Test"
            };

            var emailResult = (EmailTarget)api.Targets.CreateTarget(target).Target;
            Assert.NotNull(emailResult);
            Assert.IsAssignableFrom<EmailTarget>(emailResult);
            Assert.Equal("email_target", emailResult.Type);
            Assert.Equal("test@test.com", emailResult.Email);
            Assert.Equal("Test", emailResult.Subject);

            emailResult.Subject = "Test Update";

            var update = (EmailTarget)api.Targets.UpdateTarget(emailResult).Target;
            Assert.Equal(emailResult.Subject, update.Subject);

            Assert.True(api.Targets.DeleteTarget(emailResult.Id.Value));
        }

        [Fact]
        public void CanRetrieveMultipleTargetTypes()
        {
            var emailTarget = new EmailTarget()
            {
                Title   = "Test Email Target",
                Active  = false,
                Email   = "test@test.com",
                Subject = "Test"
            };

            var emailResult = (EmailTarget)api.Targets.CreateTarget(emailTarget).Target;
            Assert.NotNull(emailResult);
            Assert.IsAssignableFrom<EmailTarget>(emailResult);

            var jiraTarget = new JiraTarget()
            {
                Title     = "Test Jira Target",
                Active    = false,
                TargetUrl = "http://test.com",
                Username  = "testuser",
                Password  = "testpassword"
            };

            var jiraResult = (JiraTarget)api.Targets.CreateTarget(jiraTarget).Target;
            Assert.NotNull(jiraResult);
            Assert.IsAssignableFrom<JiraTarget>(jiraResult);

            var targets = api.Targets.GetAllTargets();
            foreach (var target in targets.Targets)
            {
                if(target.Id == emailResult.Id)
                {
                    Assert.IsAssignableFrom<EmailTarget>(emailResult);
                }
                else if (target.Id == jiraResult.Id)
                {
                    Assert.IsAssignableFrom<JiraTarget>(jiraResult);
                }
            }

            Assert.True(api.Targets.DeleteTarget(emailResult.Id.Value));
            Assert.True(api.Targets.DeleteTarget(jiraResult.Id.Value));
        }
    }
}