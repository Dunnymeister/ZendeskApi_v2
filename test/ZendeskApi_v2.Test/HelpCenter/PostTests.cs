
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using ZendeskApi_v2;
using ZendeskApi_v2.Models.HelpCenter.Post;

namespace Tests.HelpCenter
{

    [Category("HelpCenter")]
    public class PostTests
    {
        private ZendeskApi api = new ZendeskApi(Settings.Site, Settings.AdminEmail, Settings.AdminPassword);
        private const string postTitile = "Help me!";
        private const string postDetails = "My printer is on fire!";

        [OneTimeSetUpAttribute]
        public void setup()
        {
            var res = api.HelpCenter.Posts.GetPostsByTopicId(Settings.Topic_ID, 100);
            foreach (var post in res.Posts?.Where(x => x.Title == "Help me!"))
            {
                api.HelpCenter.Posts.DeletePost(post.Id.Value);
            }
        }

        [Fact]
        public void CanGetPosts()
        {
            var res = api.HelpCenter.Posts.GetPosts();
            Assert.True(res.Posts.Count > 0);
        }

        [Fact]
        public void CanCreatePost()
        {
            var post = new Post { Title = postTitile, Details = postDetails, TopicId = Settings.Topic_ID };
            var res = api.HelpCenter.Posts.CreatePost(post);
            Assert.NotNull(res?.Post);
        }

        [Fact]
        public void CanDeletePost()
        {
            var post = new Post { Title = postTitile, Details = postDetails, TopicId = Settings.Topic_ID };
            var res = api.HelpCenter.Posts.CreatePost(post);
            Assert.True(api.HelpCenter.Posts.DeletePost(res.Post.Id.Value));
        }

        [Fact]
        public void CanGetPost()
        {
            var post = new Post { Title = postTitile, Details = postDetails, TopicId = Settings.Topic_ID };
            var res = api.HelpCenter.Posts.CreatePost(post);
            var get = api.HelpCenter.Posts.GetPost(res.Post.Id.Value);
            Assert.Equal(get.Post.Id, res.Post.Id);
        }

        [Fact]
        public void CanGetPostForTopicId()
        {
            var res = api.HelpCenter.Posts.GetPostsByTopicId(Settings.Topic_ID);
            Assert.NotNull(res.Posts);
            Assert.True(res.Posts.Count > 0);
        }

        [Fact]
        public void CanUpdatePost()
        {
            var updatedPostDetails = "This has been updated";
            var post = new Post { Title = postTitile, Details = postDetails, TopicId = Settings.Topic_ID };
            var res = api.HelpCenter.Posts.CreatePost(post);

            res.Post.Details = updatedPostDetails;
            var updated = api.HelpCenter.Posts.UpdatePost(res.Post);

            Assert.NotNull(updated?.Post);
            Assert.Equal(updated.Post.Details, updatedPostDetails);
        }

        [Fact]
        public async Task CanGetPostsAsync()
        {
            var res = await api.HelpCenter.Posts.GetPostsAsync();
            Assert.True(res.Posts.Count > 0);
        }

        [Fact]
        public async Task CanCreatePostAsync()
        {
            var post = new Post { Title = postTitile, Details = postDetails, TopicId = Settings.Topic_ID };
            var res = await api.HelpCenter.Posts.CreatePostAsync(post);
            Assert.NotNull(res?.Post);
        }

        [Fact]
        public async Task CanDeletePostAsync()
        {
            var post = new Post { Title = postTitile, Details = postDetails, TopicId = Settings.Topic_ID };
            var res = await api.HelpCenter.Posts.CreatePostAsync(post);
            Assert.True( await api.HelpCenter.Posts.DeletePostAsync(res.Post.Id.Value));
        }

        [Fact]
        public async Task CanGetPostAsync()
        {
            var post = new Post { Title = postTitile, Details = postDetails, TopicId = Settings.Topic_ID };
            var res = await api.HelpCenter.Posts.CreatePostAsync(post);
            var get = await api.HelpCenter.Posts.GetPostAsync(res.Post.Id.Value);
            Assert.Equal(get.Post.Id, res.Post.Id);
        }

        [Fact]
        public async Task CanGetPostForTopicIdAsync()
        {
            var res = await api.HelpCenter.Posts.GetPostsByTopicIdAsync(Settings.Topic_ID);
            Assert.NotNull(res.Posts);
            Assert.True(res.Posts.Count > 0);
        }

        [Fact]
        public async Task CanUpdatePostAsync()
        {
            var updatedPostDetails = "This has been updated";
            var post = new Post { Title = postTitile, Details = postDetails, TopicId = Settings.Topic_ID };
            var res = await api.HelpCenter.Posts.CreatePostAsync(post);

            res.Post.Details = updatedPostDetails;
            var updated = await api.HelpCenter.Posts.UpdatePostAsync(res.Post);

            Assert.NotNull(updated?.Post);
            Assert.Equal(updated.Post.Details, updatedPostDetails);
        }
    }
}
