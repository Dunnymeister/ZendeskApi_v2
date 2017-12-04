using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using ZendeskApi_v2;
using ZendeskApi_v2.Models.Articles;
using ZendeskApi_v2.Models.Sections;
using ZendeskApi_v2.Models.Users;
using ZendeskApi_v2.Requests.HelpCenter;

namespace Tests.HelpCenter
{
    [Category("HelpCenter")]
    public class ArticleTests
    {
        private ZendeskApi api = new ZendeskApi(Settings.Site, Settings.AdminEmail, Settings.AdminPassword);
        private long _articleIdWithComments = 204838115; //https://csharpapi.zendesk.com/hc/en-us/articles/204838115-Thing-4?page=1#comment_200486479
        private long _categoryWithSideloadedSectionsId = 200382245;
        [Fact]
        public void CanGetSingleArticle()
        {
            var res = api.HelpCenter.Articles.GetArticle(_articleIdWithComments);
            Assert.NotNull(res.Article);
        }

        [Fact]
        public void CanGetSingleArticleWithTranslations()
        {
            var res = api.HelpCenter.Articles.GetArticle(_articleIdWithComments, ArticleSideLoadOptionsEnum.Translations);
            Assert.NotNull(res.Article);
            Assert.True(res.Article.Translations.Count > 0);
        }

        [Fact]
        public void CanGetArticles()
        {
            var res = api.HelpCenter.Articles.GetArticles();
            Assert.True(res.Count > 0);

            var resSections = api.HelpCenter.Sections.GetSections();
            var res1 = api.HelpCenter.Articles.GetArticlesBySectionId(202119686);
            Assert.Equal(res1.Articles[0].SectionId, 202119686);
        }

        public void CanGetArticleSideloadedWith()
        {
            var res = api.HelpCenter.Articles.GetArticles(ArticleSideLoadOptionsEnum.Sections | ArticleSideLoadOptionsEnum.Categories | ArticleSideLoadOptionsEnum.Users);

            Assert.True(res.Articles.Count > 0);
            Assert.True(res.Categories.Count > 0);
            Assert.True(res.Sections.Count > 0);
            Assert.True(res.Users.Count > 0);
        }

        [Fact]
        public void CanGetArticleSideloadedWithUsers()
        {
            var res = api.HelpCenter.Articles.GetArticles(ArticleSideLoadOptionsEnum.Users);

            Assert.True(res.Articles.Count > 0);
            Assert.True(res.Users.Count > 0);
        }

        [Fact]
        public void CanGetArticleSideloadedWithSections()
        {
            var res = api.HelpCenter.Articles.GetArticles(ArticleSideLoadOptionsEnum.Sections);

            Assert.True(res.Articles.Count > 0);
            Assert.True(res.Sections.Count > 0);
        }

        [Fact]
        public void CanGetArticleSideloadedWithCategories()
        {
            var res = api.HelpCenter.Articles.GetArticles(ArticleSideLoadOptionsEnum.Categories);

            Assert.True(res.Articles.Count > 0);
            Assert.True(res.Categories.Count > 0);
        }

        [Fact]
        public void CanGetArticleSideloadedWithTranslations()
        {
            var res = api.HelpCenter.Articles.GetArticles(ArticleSideLoadOptionsEnum.Categories | ArticleSideLoadOptionsEnum.Sections | ArticleSideLoadOptionsEnum.Users | ArticleSideLoadOptionsEnum.Translations);

            Assert.True(res.Categories[0].Translations.Count > 0);
            Assert.True(res.Articles[0].Translations.Count > 0);
            Assert.True(res.Sections[0].Translations.Count > 0);
        }

        [Fact]
        public void CanGetArticleByCategoryWithSideloadedSections()
        {
            var res = api.HelpCenter.Articles.GetArticlesByCategoryId(_categoryWithSideloadedSectionsId, ArticleSideLoadOptionsEnum.Sections);
            Assert.True(res.Sections.Count > 0);
        }

        [Fact]
        public void CanGetArticlesSorted()
        {
            var articlesAscending = api.HelpCenter.Articles.GetArticles(ArticleSideLoadOptionsEnum.None, new ArticleSortingOptions() { SortBy = ArticleSortEnum.Title });
            var articlesDescending = api.HelpCenter.Articles.GetArticles(ArticleSideLoadOptionsEnum.None, new ArticleSortingOptions() { SortBy = ArticleSortEnum.Title, SortOrder = ArticleSortOrderEnum.Desc });

            Assert.True(articlesAscending.Articles[0].Title != articlesDescending.Articles[0].Title);
        }

        [Fact]
        public void CanGetArticlesSortedInASection()
        {
            var section = api.HelpCenter.Sections.GetSectionById(201010935).Section;

            var articlesAscending = api.HelpCenter.Articles.GetArticlesBySectionId(section.Id.Value, ArticleSideLoadOptionsEnum.None,
                new ArticleSortingOptions() { SortBy = ArticleSortEnum.Title });
            var articlesDescending = api.HelpCenter.Articles.GetArticlesBySectionId(section.Id.Value, ArticleSideLoadOptionsEnum.None,
                new ArticleSortingOptions() { SortBy = ArticleSortEnum.Title, SortOrder = ArticleSortOrderEnum.Desc });

            Assert.True(articlesAscending.Articles[0].Title != articlesDescending.Articles[0].Title);
        }

        /// <summary>
        /// This throws a 500 error, no idea why, ticket into Zendesk
        /// </summary>
        [Fact]
        public void CanGetArticlesSortedInACategory()
        {
            var articlesAscending = api.HelpCenter.Articles.GetArticlesByCategoryId(_categoryWithSideloadedSectionsId, ArticleSideLoadOptionsEnum.None, new ArticleSortingOptions() { SortBy = ArticleSortEnum.Title });
            var articlesDescending = api.HelpCenter.Articles.GetArticlesByCategoryId(_categoryWithSideloadedSectionsId, ArticleSideLoadOptionsEnum.None, new ArticleSortingOptions() { SortBy = ArticleSortEnum.Title, SortOrder = ArticleSortOrderEnum.Desc });

            // Poor choice of test - should verify that the lists are in reverse order and not that the titles aren't equal
            Assert.True(articlesAscending.Articles[0].Title != articlesDescending.Articles[0].Title);
        }

        [Fact]
        public void CanCreateUpdateAndDeleteArticles()
        {
            var resSections = api.HelpCenter.Sections.GetSections();
            var res = api.HelpCenter.Articles.CreateArticle(resSections.Sections[0].Id.Value, new Article()
            {
                Title = "My Test article",
                Body = "The body of my article",
                Locale = "en-us"
            });
            Assert.True(res.Article.Id > 0);

            res.Article.LabelNames = new string[] { "updated" };
            var update = api.HelpCenter.Articles.UpdateArticleAsync(res.Article).Result;
            Assert.Equal(update.Article.LabelNames, res.Article.LabelNames);

            Assert.True(api.HelpCenter.Articles.DeleteArticle(res.Article.Id.Value));
        }

        [Fact]
        public void CanGetSingleArticleWithTranslationsAsync()
        {
            var res = api.HelpCenter.Articles.GetArticleAsync(_articleIdWithComments, ArticleSideLoadOptionsEnum.Translations).Result;
            Assert.NotNull(res.Article);
            Assert.True(res.Article.Translations.Count > 0);
        }

        [Fact]
        public async Task CanGetArticlesAsync()
        {
            var res = await api.HelpCenter.Articles.GetArticlesAsync();
            Assert.True(res.Count > 0);

            var resSections = await api.HelpCenter.Sections.GetSectionsAsync();
            var res1 = await api.HelpCenter.Articles.GetArticlesBySectionIdAsync(202119686);
            Assert.Equal(res1.Articles[0].SectionId, 202119686);
        }

        [Fact]
        public async Task CanCreateUpdateAndDeleteArticlesAsync()
        {
            var resSections = await api.HelpCenter.Sections.GetSectionsAsync();
            var res = await api.HelpCenter.Articles.CreateArticleAsync(resSections.Sections[0].Id.Value, new Article
            {
                Title = "My Test article",
                Body = "The body of my article",
                Locale = "en-us"
            });

            Assert.True(res.Article.Id > 0);

            res.Article.LabelNames = new string[] { "photo", "tripod" };
            var update = await api.HelpCenter.Articles.UpdateArticleAsync(res.Article);
            Assert.Equal(update.Article.LabelNames, res.Article.LabelNames);

            Assert.True(await api.HelpCenter.Articles.DeleteArticleAsync(res.Article.Id.Value));
        }

        [Fact]
        public async Task CanGetSecondPageUisngGetByPageUrl()
        {
            var pageSize = 3;

            var res = await api.HelpCenter.Articles.GetArticlesAsync(perPage: pageSize);
            Assert.Equal(res.PageSize, pageSize);

            var resp = await api.HelpCenter.Articles.GetByPageUrlAsync<GroupArticleResponse>(res.NextPage, pageSize);
            Assert.Equal(resp.Page, 2);
        }

        [Fact]
        public async Task CanSearchForArticlesAsync()
        {
            var resp = await api.HelpCenter.Articles.SearchArticlesForAsync("Test", createdBefore: DateTime.Now);

            Assert.True(resp.Count > 0);
        }

        [Fact]
        public void CanSearchForArticles()
        {
            var resp = api.HelpCenter.Articles.SearchArticlesFor("Test", createdBefore: DateTime.Now);

            Assert.True(resp.Count > 0);
        }
    }
}
