﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;
using ZendeskApi_v2;
using ZendeskApi_v2.Extensions;
using ZendeskApi_v2.Models.Constants;
using ZendeskApi_v2.Models.Shared;
using ZendeskApi_v2.Models.Tickets;
using ZendeskApi_v2.Models.Views;
using ZendeskApi_v2.Models.Views.Executed;


namespace Tests
{
    public class ViewTests
    {
        ZendeskApi api = new ZendeskApi(Settings.Site, Settings.AdminEmail, Settings.AdminPassword);

        [Fact]
        public void CanGetViews()
        {
            var views = api.Views.GetAllViews();
            Assert.True(views.Count > 0);
        }

        [Fact]
        public void CanGetActiveViews()
        {
            var views = api.Views.GetActiveViews();
            Assert.True(views.Count > 0);
        }

        [Fact]
        public void CanGetCompactViews()
        {
            var views = api.Views.GetCompactViews();
            Assert.True(views.Count > 0);
        }

        [Fact]
        public void CanGetViewById()
        {
            var views = api.Views.GetAllViews();
            var view = api.Views.GetView(views.Views.First().Id);
            Assert.True(view.View.Id > 0);
        }

        [Fact]
        public void CanExecuteViews()
        {
            var views = api.Views.GetAllViews();
            //var res = api.Views.ExecuteView(views.Views.First().Id);

            //id for all unsolved tickets
            var res = api.Views.ExecuteView(31559032);

            Assert.True(res.Rows.Count > 0);
            Assert.True(res.Columns.Count > 0);
        }

        [Fact]
        public void CanExecutePagedView()
        {
            var res = api.Views.ExecuteView(Settings.ViewId, "", true, 25, 2);

            Assert.Equal(25, res.Rows.Count);

            var nextPage = res.NextPage.GetQueryStringDict()
                    .Where(x => x.Key == "page")
                        .Select(x => x.Value)
                        .FirstOrDefault();

            Assert.NotNull(nextPage);

            Assert.Equal("3", nextPage);
        }

        [Fact]
        public void CanPreviewViews()
        {
            var preview = new PreviewViewRequest()
            {
                View = new PreviewView()
                {
                    All = new List<All> {new All {Field = "status", Value = "open", Operator = "is"}},
                    Output = new PreviewViewOutput { Columns = new List<string> { "subject" } }
                }
            };

            var previewRes = api.Views.PreviewView(preview);
            Assert.True(previewRes.Rows.Count > 0);
            Assert.True(previewRes.Columns.Count > 0);
        }

        [Fact]
        public void CanGetViewCounts()
        {
            var views = api.Views.GetAllViews();
            var res = api.Views.GetViewCounts(new List<long>() { views.Views[0].Id });
            Assert.True(res.ViewCounts.Count > 0);

            Assert.True(views.Count > 0);
        }

        [Fact]
        public void CanGetViewCount()
        {
            var views = api.Views.GetAllViews();
            var id = views.Views[0].Id;
            var res = api.Views.GetViewCount(id);

            Assert.True(res.ViewCount.ViewId == id);
        }
    }
}
