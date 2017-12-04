﻿using System.ComponentModel;
using Xunit;
using ZendeskApi_v2;
using ZendeskApi_v2.Models.Articles;
using ZendeskApi_v2.Requests.HelpCenter;

namespace Tests.HelpCenter
{
    [Category("HelpCenter")]
    public class VoteTests
    {
        private ZendeskApi api = new ZendeskApi(Settings.Site, Settings.AdminEmail, Settings.AdminPassword);
        private long _articleIdWithVotes = 204838115; //https://csharpapi.zendesk.com/hc/en-us/articles/204838115-Thing-4?page=1#comment_200486479

        [Fact]
        public void CanGetArticleVotes()
        {
            var votes = api.HelpCenter.Votes.GetVotesForArticle(_articleIdWithVotes);

            Assert.True(votes.Count > 0);
        }
    }
}