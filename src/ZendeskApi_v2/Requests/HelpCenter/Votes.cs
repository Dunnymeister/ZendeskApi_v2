using System;
using System.Threading.Tasks;
using ZendeskApi_v2.Extensions;
using ZendeskApi_v2.Models.HelpCenter.Votes;

namespace ZendeskApi_v2.Requests.HelpCenter
{
	public interface IVotes : ICore
	{

		GroupVoteResponse GetVotesForArticle(long? articleId);
		Task<GroupVoteResponse> GetVotesForArticleAsync(long? articleId);

	}

	public class Votes : Core, IVotes
	{
		public Votes(string yourZendeskUrl, string user, string password, string apiToken, string p_OAuthToken)
            : base(yourZendeskUrl, user, password, apiToken, p_OAuthToken)
		{
		}


		public GroupVoteResponse GetVotesForArticle(long? articleId)
		{ 
			return GenericGet<GroupVoteResponse>($"help_center/articles/{articleId}/votes.json");
		}
		

		public async Task<GroupVoteResponse> GetVotesForArticleAsync(long? articleId)
		{
			return await GenericGetAsync<GroupVoteResponse>($"help_center/articles/{articleId}/votes.json");
		}

	}
}
