﻿using System.ComponentModel;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;
using ZendeskApi_v2;
using ZendeskApi_v2.Models.HelpCenter.Topics;
using ZendeskApi_v2.Models.Sections;
using ZendeskApi_v2.Models.UserSegments;
using ZendeskApi_v2.Requests.HelpCenter;

namespace Tests.HelpCenter
{

    [Category("HelpCenter")]
    class UserSegmentTests
    {
        private ZendeskApi api = new ZendeskApi(Settings.Site, Settings.AdminEmail, Settings.AdminPassword);

        [Fact]
        public void CanGetUserSegments()
        {
            var res = api.HelpCenter.UserSegments.GetUserSegments();
            Assert.True(res.Count > 0);

            var res1 = api.HelpCenter.UserSegments.GetUserSegment(res.UserSegments[0].Id.Value);
            Assert.Equal(res1.UserSegment.Id, res.UserSegments[0].Id.Value);
        }

        [Fact]
        public void CanGetUserSegmentsApplicable()
        {
            var res = api.HelpCenter.UserSegments.GetUserSegmentsApplicable();
            Assert.True(res.Count > 0);

            var res1 = api.HelpCenter.UserSegments.GetUserSegment(res.UserSegments[0].Id.Value);
            Assert.Equal(res1.UserSegment.Id, res.UserSegments[0].Id.Value);
        }

        [Fact]
        public void CanCreateUpdateAndDeleteUserSegments()
        {
            var userSegment = new UserSegment() {
                Name = "My Test User Segment",
                UserType = UserType.signed_in_users
            };
            var res = api.HelpCenter.UserSegments.CreateUserSegment(userSegment);
            Assert.True(res.UserSegment.Id > 0);

            res.UserSegment.UserType = UserType.staff;
            var update = api.HelpCenter.UserSegments.UpdateUserSegment(res.UserSegment);
            Assert.Equal(update.UserSegment.UserType, res.UserSegment.UserType);
            Assert.True(api.HelpCenter.UserSegments.DeleteUserSegment(res.UserSegment.Id.Value));
        }

        [Fact]
        public void CanGetSecondPageUisngGetByPageUrl()
        {
            var pageSize = 3;

            var res = api.HelpCenter.UserSegments.GetUserSegments(perPage: pageSize);
            Assert.Equal(res.PageSize, pageSize);

            var resp = api.HelpCenter.UserSegments.GetByPageUrl<GroupUserSegmentResponse>(res.NextPage, pageSize);
            Assert.Equal(resp.Page, 2);
        }

        [Fact]
        public void CanGetSectionsByUserSegment()
        {
            var res = api.HelpCenter.UserSegments.GetUserSegments();
            long category_id = 200382245;

            var sectionRes = api.HelpCenter.Sections.CreateSection(new Section {
                Name = "My Test section",
                CategoryId = category_id,
                UserSegmentId = res.UserSegments[0].Id
            });

            var res1 = api.HelpCenter.UserSegments.GetSectionsByUserSegmentId(res.UserSegments[0].Id.Value);

            Assert.True(res1.Sections.Count > 0);
        }

        [Fact]
        public void CanGetTopicsByUserSegment()
        {
            var res = api.HelpCenter.UserSegments.GetUserSegments();

            var topicRes = api.HelpCenter.Topics.CreateTopic(new Topic {
                Name = "My Test Topic",
                UserSegmentId = res.UserSegments[0].Id
            });

            var res1 = api.HelpCenter.UserSegments.GetTopicsByUserSegmentId(res.UserSegments[0].Id.Value);

            Assert.True(res1.Topics.Count > 0);
        }

        [Fact]
        public void CanGetUserSegmentsByUserId()
        {

            var res = api.HelpCenter.UserSegments.GetUserSegmentsByUserId(Settings.UserId);
            Assert.True(res.UserSegments.Count > 0);
        }

        [Fact]
        public async Task CanGetUserSegmentsAsync()
        {
            var res = await api.HelpCenter.UserSegments.GetUserSegmentsAsync();
            Assert.True(res.Count > 0);

            var res1 = await api.HelpCenter.UserSegments.GetUserSegmentAsync(res.UserSegments[0].Id.Value);
            Assert.Equal(res1.UserSegment.Id, res.UserSegments[0].Id.Value);
        }

        [Fact]
        public async Task CanGetUserSegmentsApplicableAsync()
        {
            var res = await api.HelpCenter.UserSegments.GetUserSegmentsApplicableAsync();
            Assert.True(res.Count > 0);

            var res1 = await api.HelpCenter.UserSegments.GetUserSegmentAsync(res.UserSegments[0].Id.Value);
            Assert.Equal(res1.UserSegment.Id, res.UserSegments[0].Id.Value);
        }

        [Fact]
        public async Task CanCreateUpdateAndDeleteUserSegmentsAsync()
        {
            var userSegment = new UserSegment() {
                Name = "My Test User Segment Async",
                UserType = UserType.signed_in_users
            };
            var res = await api.HelpCenter.UserSegments.CreateUserSegmentAsync(userSegment);
            Assert.True(res.UserSegment.Id > 0);

            res.UserSegment.UserType = UserType.staff;
            var update = await api.HelpCenter.UserSegments.UpdateUserSegmentAsync(res.UserSegment);
            Assert.Equal(update.UserSegment.UserType, res.UserSegment.UserType);
            Assert.True(await api.HelpCenter.UserSegments.DeleteUserSegmentAsync(res.UserSegment.Id.Value));
        }

        [Fact]
        public async Task CanGetSecondPageUisngGetByPageUrlAsync()
        {
            var pageSize = 3;

            var res = await api.HelpCenter.UserSegments.GetUserSegmentsAsync(perPage: pageSize);
            Assert.Equal(res.PageSize, pageSize);

            var resp = await api.HelpCenter.UserSegments.GetByPageUrlAsync<GroupUserSegmentResponse>(res.NextPage, pageSize);
            Assert.Equal(resp.Page, 2);
        }

        [Fact]
        public async Task CanGetSectionsByUserSegmentAsync()
        {
            var res = await api.HelpCenter.UserSegments.GetUserSegmentsAsync();
            long category_id = 200382245;

            var sectionRes = await api.HelpCenter.Sections.CreateSectionAsync(new Section {
                Name = "My Test section",
                CategoryId = category_id,
                UserSegmentId = res.UserSegments[0].Id
            });

            var res1 = await api.HelpCenter.UserSegments.GetSectionsByUserSegmentIdAsync(res.UserSegments[0].Id.Value);

            Assert.True(res1.Sections.Count > 0);
        }

        [Fact]
        public async Task CanGetTopicsByUserSegmentAsync()
        {
            var res = await api.HelpCenter.UserSegments.GetUserSegmentsAsync();

            var topicRes = await api.HelpCenter.Topics.CreateTopicAsync(new Topic {
                Name = "My Test Topic",
                UserSegmentId = res.UserSegments[0].Id
            });

            var res1 = await api.HelpCenter.UserSegments.GetTopicsByUserSegmentIdAsync(res.UserSegments[0].Id.Value);

            Assert.True(res1.Topics.Count > 0);
        }

        [Fact]
        public async Task CanGetUserSegmentsByUserIdAsync()
        {

            var res = await api.HelpCenter.UserSegments.GetUserSegmentsByUserIdAsync(Settings.UserId);
            Assert.True(res.UserSegments.Count > 0);
        }
    }
}