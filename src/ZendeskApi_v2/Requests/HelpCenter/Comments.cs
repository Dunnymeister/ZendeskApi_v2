using System;
using System.Threading.Tasks;
using ZendeskApi_v2.Extensions;
using ZendeskApi_v2.Models.HelpCenter.Comments;

namespace ZendeskApi_v2.Requests.HelpCenter
{
	public interface IComments : ICore
	{

		GroupCommentResponse GetCommentsForArticle(long? articleId, int? perPage = null, int? page = null);
        GroupCommentResponse GetCommentsForUser(long? userId);
        GroupCommentResponse GetCommentsForCurrentUser();
		Task<GroupCommentResponse> GetCommentsForArticleAsync(long? articleId);
        Task<GroupCommentResponse> GetCommentsForUserAsync(long? userId);
        Task<GroupCommentResponse> GetCommentsForCurrentUserAsync();

	}

    /// <summary>
    /// https://developer.zendesk.com/rest_api/docs/help_center/comments
    /// </summary>
	public class Comments : Core, IComments
	{
		public Comments(string yourZendeskUrl, string user, string password, string apiToken, string p_OAuthToken)
            : base(yourZendeskUrl, user, password, apiToken, p_OAuthToken)
		{
		}


		public GroupCommentResponse GetCommentsForArticle(long? articleId, int? perPage = null, int? page = null)
		{
			return GenericPagedGet<GroupCommentResponse>($"help_center/articles/{articleId}/comments.json", perPage, page);
		}

        public GroupCommentResponse GetCommentsForUser(long? userId)
		{
			return GenericGet<GroupCommentResponse>($"help_center/users/{userId}/comments.json");
		}

        public GroupCommentResponse GetCommentsForCurrentUser()
		{
			return GenericGet<GroupCommentResponse>("help_center/users/me/comments.json");
		}

		public async Task<GroupCommentResponse> GetCommentsForArticleAsync(long? articleId)
		{
			return await GenericGetAsync<GroupCommentResponse>($"help_center/articles/{articleId}/comments.json");
		}

        public async Task<GroupCommentResponse> GetCommentsForUserAsync(long? userId)
		{
			return await GenericGetAsync<GroupCommentResponse>($"help_center/users/{userId}/comments.json");
		}

        public async Task<GroupCommentResponse> GetCommentsForCurrentUserAsync()
		{
			return await GenericGetAsync<GroupCommentResponse>("help_center/users/me/comments.json");
		}
	}
}
