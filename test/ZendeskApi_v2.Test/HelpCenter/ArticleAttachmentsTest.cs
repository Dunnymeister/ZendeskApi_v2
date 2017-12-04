
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using ZendeskApi_v2;
using ZendeskApi_v2.Models.Articles;
using ZendeskApi_v2.Models.Shared;
using ZendeskApi_v2.Test.Util;

namespace Tests.HelpCenter
{

    [Category("HelpCenter")]
    public class ArticleAttachmentsTest
    {
        private const string ResourceName = "ZendeskApi_v2.Test.Resources.testupload.txt";
        private ZendeskApi api = new ZendeskApi(Settings.Site, Settings.AdminEmail, Settings.AdminPassword);

        [Fact]
        public void CanGetAttachmentsForArticle()
        {
            var res = api.HelpCenter.ArticleAttachments.GetAttachments(204838115);
            Assert.NotNull(res.Attachments);
        }

        [Fact]
        public void CanUploadAttachmentsForArticle()
        {
            var fileData = ResourceUtil.GetResource(ResourceName);
            var file = new ZenFile()
            {
                ContentType = "text/plain",
                FileName = "testupload.txt",
                FileData = fileData
            };

            var respSections = api.HelpCenter.Sections.GetSections();
            var articleResponse = api.HelpCenter.Articles.CreateArticle(respSections.Sections[0].Id.Value, new Article
            {
                Title = "My Test article",
                Body = "The body of my article",
                Locale = "en-us"
            });

            var resp = api.HelpCenter.ArticleAttachments.UploadAttchment(articleResponse.Article.Id, file);

            Assert.NotNull(resp.Attachment);
            Assert.True(api.HelpCenter.ArticleAttachments.DeleteAttchment(resp.Attachment.Id));
            Assert.True(api.HelpCenter.Articles.DeleteArticle(articleResponse.Article.Id.Value));
        }

        [Fact]
        public async Task CanGetAttachmentsForArticleAsync()
        {
            var res = await api.HelpCenter.ArticleAttachments.GetAttachmentsAsync(204838115);
            Assert.NotNull(res.Attachments);
        }

        [Fact]
        public async Task CanUploadAttachmentsForArticleAsync()
        {
            var fileData = ResourceUtil.GetResource(ResourceName);

            var file = new ZenFile()
            {
                ContentType = "text/plain",
                FileName = "testupload.txt",
                FileData = fileData
            };

            var respSections = await api.HelpCenter.Sections.GetSectionsAsync();
            var articleResponse = await api.HelpCenter.Articles.CreateArticleAsync(respSections.Sections[0].Id.Value, new Article
            {
                Title = "My Test article",
                Body = "The body of my article",
                Locale = "en-us"
            });

            var resp = await api.HelpCenter.ArticleAttachments.UploadAttchmentAsync(articleResponse.Article.Id, file, true);

            Assert.NotNull(resp.Attachment);
            Assert.True(resp.Attachment.Inline);

            Assert.True(await api.HelpCenter.ArticleAttachments.DeleteAttchmentAsync(resp.Attachment.Id));
            Assert.True(await api.HelpCenter.Articles.DeleteArticleAsync(articleResponse.Article.Id.Value));
        }
    }
}
