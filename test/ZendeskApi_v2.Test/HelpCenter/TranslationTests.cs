using System.ComponentModel;
using System.Threading.Tasks;
using Xunit;
using ZendeskApi_v2;
using ZendeskApi_v2.Models.Articles;
using ZendeskApi_v2.Models.HelpCenter.Categories;
using ZendeskApi_v2.Models.HelpCenter.Translations;
using ZendeskApi_v2.Models.Sections;

namespace Tests.HelpCenter
{

    [Category("HelpCenter")]
    public class TranslationTests
    {
        private ZendeskApi api = new ZendeskApi(Settings.Site, Settings.AdminEmail, Settings.AdminPassword);
        private long _articleId = 204838115; //https://csharpapi.zendesk.com/hc/en-us/articles/204838115-Thing-4?page=1#comment_200486479
        private long _sectionId = 201010935;
        private long _categoryId = 200382245;
        [Fact]
        public void CanListTranslations()
        {
            var res = api.HelpCenter.Translations.ListTranslationsForArticle(_articleId);
            Assert.Equal(2, res.Count);

            res = api.HelpCenter.Translations.ListTranslationsForSection(_sectionId);
            Assert.Equal(2, res.Count);

            res = api.HelpCenter.Translations.ListTranslationsForCategory(_categoryId);
            Assert.Equal(2, res.Count);
        }

        [Fact]
        public void CanShowTranslationForArticle()
        {
            var res = api.HelpCenter.Translations.ShowTranslationForArticle(_articleId, "en-us");
            Assert.Equal("en-us", res.Translation.Locale);
        }

        [Fact]
        public void CanListMissingCreateUpdateAndDeleteTranslationsForArticle()
        {
            //create an article with en-us locale.
            //verify that fr is missing.
            //add a translation and verify.
            //update translation and verify.
            //delete translation and verify.
            //delete new article.

            //prep
            var resSections = api.HelpCenter.Sections.GetSections();
            var new_article_res = api.HelpCenter.Articles.CreateArticle(resSections.Sections[0].Id.Value, new ZendeskApi_v2.Models.Articles.Article()
            {
                Title = "My Test article for translations",
                Body = "The body of my article",
                Locale = "en-us"
            });
            var article_id = new_article_res.Article.Id.Value;

            var missing_res = api.HelpCenter.Translations.ListMissingTranslationsForArticle(article_id);
            Assert.Equal(1, missing_res.Count);
            Assert.Equal("fr", missing_res[0]);

            var fr_translation = new Translation()
            {
                Body = "Je ne parle pas français.",
                Title = "Mon article de test pour les traductions",
                Locale = "fr"
            };

            //create translation
            var add_res = api.HelpCenter.Translations.CreateArticleTranslation(article_id, fr_translation);
            Assert.True(add_res.Translation.Id > 0);

            add_res.Translation.Body = "insérer plus français ici .";

            //update translation
            var update_res = api.HelpCenter.Translations.UpdateArticleTranslation(add_res.Translation);
            Assert.Equal("insérer plus français ici .", update_res.Translation.Body);

            //delete translation
            Assert.True(api.HelpCenter.Translations.DeleteTranslation(update_res.Translation.Id.Value));

            //teardown.
            Assert.True(api.HelpCenter.Articles.DeleteArticle(article_id));

        }

        [Fact]
        public void CanListMissingCreateUpdateAndDeleteTranslationsForSection()
        {
            //create a section with en-us locale.
            //verify that fr is missing.
            //add a translation and verify.
            //update translation and verify.
            //delete translation and verify.
            //delete new section.

            //prep
            var resCategoies = api.HelpCenter.Categories.GetCategories();
            var new_section_res = api.HelpCenter.Sections.CreateSection(new ZendeskApi_v2.Models.Sections.Section()
            {
                Name = "My Test section for translations",
                Description = "The body of my section (en-us)",
                Locale = "en-us",
                CategoryId = resCategoies.Categories[0].Id.Value
            });
            var section_id = new_section_res.Section.Id.Value;

            var missing_res = api.HelpCenter.Translations.ListMissingTranslationsForSection(section_id);
            Assert.Equal(1, missing_res.Count);
            Assert.Equal("fr", missing_res[0]);

            var fr_translation = new Translation()
            {
                Body = "Je ne parle pas français.",
                Title = "french category here",
                Locale = "fr"
            };

            //create translation
            var add_res = api.HelpCenter.Translations.CreateSectionTranslation(section_id, fr_translation);
            Assert.True(add_res.Translation.Id > 0);

            add_res.Translation.Body = "insérer plus français ici .";

            //update translation
            var update_res = api.HelpCenter.Translations.UpdateSectionTranslation(add_res.Translation);
            Assert.Equal("insérer plus français ici .", update_res.Translation.Body);

            //delete translation
            Assert.True(api.HelpCenter.Translations.DeleteTranslation(update_res.Translation.Id.Value));

            //teardown.
            Assert.True(api.HelpCenter.Sections.DeleteSection(section_id));

        }

        [Fact]
        public void CanListMissingCreateUpdateAndDeleteTranslationsForCategory()
        {
            //create a category with en-us locale.
            //verify that fr is missing.
            //add a translation and verify.
            //update translation and verify.
            //delete translation and verify.
            //delete new category.

            //prep
            var new_category_res = api.HelpCenter.Categories.CreateCategory(new ZendeskApi_v2.Models.HelpCenter.Categories.Category()
            {
                Name = "My Test category for translations",
                Description = "The body of my category (en-us)",
                Locale = "en-us"
            });
            var category_id = new_category_res.Category.Id.Value;

            var missing_res = api.HelpCenter.Translations.ListMissingTranslationsForCategory(category_id);
            Assert.Equal(1, missing_res.Count);
            Assert.Equal("fr", missing_res[0]);

            var fr_translation = new Translation()
            {
                Body = "Je ne parle pas français.",
                Title = "french for 'this is a french category'",
                Locale = "fr"
            };

            //create translation
            var add_res = api.HelpCenter.Translations.CreateCategoryTranslation(category_id, fr_translation);
            Assert.True(add_res.Translation.Id > 0);

            add_res.Translation.Body = "insérer plus français ici . (category)";

            //update translation
            var update_res = api.HelpCenter.Translations.UpdateCategoryTranslation(add_res.Translation);
            Assert.Equal("insérer plus français ici . (category)", update_res.Translation.Body);

            //delete translation
            Assert.True(api.HelpCenter.Translations.DeleteTranslation(update_res.Translation.Id.Value));

            //teardown.
            Assert.True(api.HelpCenter.Categories.DeleteCategory(category_id));

        }

        [Fact]
        public void CanListAllEnabledLocales()
        {
            //the only two locales enabled on the test site are us-en and fr. us-en is the default.
            //note: FR was already enabled in the Zendesk settings, however it had to be enabled again in the help center preferences.
            string default_locale;
            var res = api.HelpCenter.Translations.ListAllEnabledLocalesAndDefaultLocale(out default_locale);

            Assert.Equal(default_locale, "en-us");
            Assert.True(res.Contains("en-us"));
            Assert.True(res.Contains("fr"));
        }

        //Async tests:

        [Fact]
        public async Task CanListTranslationsAsync()
        {
            var res = await api.HelpCenter.Translations.ListTranslationsForArticleAsync(_articleId);
            Assert.Equal(res.Count, 2);

            res = await api.HelpCenter.Translations.ListTranslationsForSectionAsync(_sectionId);
            Assert.Equal(res.Count, 2);

            res = await api.HelpCenter.Translations.ListTranslationsForCategoryAsync(_categoryId);
            Assert.Equal(res.Count, 2);
        }

        [Fact]
        public async Task CanShowTranslationForArticleAsync()
        {
            var res = await api.HelpCenter.Translations.ShowTranslationForArticleAsync(_articleId, "en-us");
            Assert.Equal(res.Translation.Locale, "en-us");
        }

        [Fact]
        public async Task CanListMissingCreateUpdateAndDeleteTranslationsForArticleAsync()
        {
            //create an article with en-us locale.
            //verify that fr is missing.
            //add a translation and verify.
            //update translation and verify.
            //delete translation and verify.
            //delete new article.

            //prep
            var resSections = await api.HelpCenter.Sections.GetSectionsAsync();
            var new_article_res = await api.HelpCenter.Articles.CreateArticleAsync(resSections.Sections[0].Id.Value, new Article
            {
                Title = "My Test article for translations",
                Body = "The body of my article",
                Locale = "en-us"
            });
            var article_id = new_article_res.Article.Id.Value;

            var missing_res = await api.HelpCenter.Translations.ListMissingTranslationsForArticleAsync(article_id);
            Assert.Equal(missing_res.Count, 1);
            Assert.Equal(missing_res[0], "fr");

            var fr_translation = new Translation()
            {
                Body = "Je ne parle pas français.",
                Title = "Mon article de test pour les traductions",
                Locale = "fr"
            };

            //create translation
            var add_res = await api.HelpCenter.Translations.CreateArticleTranslationAsync(article_id, fr_translation);
            Assert.True(add_res.Translation.Id > 0);

            add_res.Translation.Body = "insérer plus français ici .";

            //update translation
            var update_res = await api.HelpCenter.Translations.UpdateArticleTranslationAsync(add_res.Translation);
            Assert.Equal(update_res.Translation.Body, "insérer plus français ici .");

            //delete translation
            Assert.True(await api.HelpCenter.Translations.DeleteTranslationAsync(update_res.Translation.Id.Value));

            //tear-down.
            Assert.True(await api.HelpCenter.Articles.DeleteArticleAsync(article_id));

        }

        [Fact]
        public async Task CanListMissingCreateUpdateAndDeleteTranslationsForSectionAsync()
        {
            //create a section with en-us locale.
            //verify that fr is missing.
            //add a translation and verify.
            //update translation and verify.
            //delete translation and verify.
            //delete new section.

            //prep
            var resCategoies = await api.HelpCenter.Categories.GetCategoriesAsync();
            var new_section_res = await api.HelpCenter.Sections.CreateSectionAsync(new Section
            {
                Name = "My Test section for translations",
                Description = "The body of my section (en-us)",
                Locale = "en-us",
                CategoryId = resCategoies.Categories[0].Id.Value
            });

            var section_id = new_section_res.Section.Id.Value;

            var missing_res = await api.HelpCenter.Translations.ListMissingTranslationsForSectionAsync(section_id);
            Assert.Equal(missing_res.Count, 1);
            Assert.Equal(missing_res[0], "fr");

            var fr_translation = new Translation
            {
                Body = "Je ne parle pas français.",
                Title = "french category here",
                Locale = "fr"
            };

            //create translation
            var add_res = await api.HelpCenter.Translations.CreateSectionTranslationAsync(section_id, fr_translation);
            Assert.True(add_res.Translation.Id > 0);

            add_res.Translation.Body = "insérer plus français ici .";

            //update translation
            var update_res = await api.HelpCenter.Translations.UpdateSectionTranslationAsync(add_res.Translation);
            Assert.Equal(update_res.Translation.Body, "insérer plus français ici .");

            //delete translation
            Assert.True(await api.HelpCenter.Translations.DeleteTranslationAsync(update_res.Translation.Id.Value));

            //tear-down.
            Assert.True(await api.HelpCenter.Sections.DeleteSectionAsync(section_id));
        }

        [Fact]
        public async Task CanListMissingCreateUpdateAndDeleteTranslationsForCategoryAsync()
        {
            //create a category with en-us locale.
            //verify that fr is missing.
            //add a translation and verify.
            //update translation and verify.
            //delete translation and verify.
            //delete new category.

            //prep
            var new_category_res = await api.HelpCenter.Categories.CreateCategoryAsync(new Category()
            {
                Name = "My Test category for translations",
                Description = "The body of my category (en-us)",
                Locale = "en-us"
            });
            var category_id = new_category_res.Category.Id.Value;

            var missing_res = await api.HelpCenter.Translations.ListMissingTranslationsForCategoryAsync(category_id);
            Assert.Equal(missing_res.Count, 1);
            Assert.Equal(missing_res[0], "fr");

            var fr_translation = new Translation()
            {
                Body = "Je ne parle pas français.",
                Title = "french for 'this is a french category'",
                Locale = "fr"
            };

            //create translation
            var add_res = await api.HelpCenter.Translations.CreateCategoryTranslationAsync(category_id, fr_translation);
            Assert.True(add_res.Translation.Id > 0);

            add_res.Translation.Body = "insérer plus français ici . (category)";

            //update translation
            var update_res = await api.HelpCenter.Translations.UpdateCategoryTranslationAsync(add_res.Translation);
            Assert.Equal(update_res.Translation.Body, "insérer plus français ici . (category)");

            //delete translation
            Assert.True(await api.HelpCenter.Translations.DeleteTranslationAsync(update_res.Translation.Id.Value));

            //tear-down.
            Assert.True(await api.HelpCenter.Categories.DeleteCategoryAsync(category_id));
        }

        [Fact]
        public async Task CanListAllEnabledLocalesAsync()
        {
            //the only two locales enabled on the test site are us-en and fr. us-en is the default.
            //note: FR was already enabled in the Zendesk settings, however it had to be enabled again in the help center preferences.
            var res = await api.HelpCenter.Translations.ListAllEnabledLocalesAndDefaultLocaleAsync();

            Assert.Equal(res.Item2, "en-us");
            Assert.True(res.Item1.Contains("en-us"));
            Assert.True(res.Item1.Contains("fr"));
        }
    }
}
