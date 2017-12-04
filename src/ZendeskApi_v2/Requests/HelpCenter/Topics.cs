using System.Threading.Tasks;
using ZendeskApi_v2.Models.HelpCenter.Topics;

namespace ZendeskApi_v2.Requests.HelpCenter
{

    public interface ITopics : ICore
    {
        GroupTopicResponse GetTopics(int? perPage = null, int? page = null);
        IndividualTopicResponse GetTopic(long topicId);
        IndividualTopicResponse CreateTopic(Topic topic);
        IndividualTopicResponse UpdateTopic(Topic topic);
        bool DeleteTopic(long topicId);
        Task<GroupTopicResponse> GetTopicsAsync(int? perPage = null, int? page = null);
        Task<IndividualTopicResponse> GetTopicAsync(long topicId);
        Task<IndividualTopicResponse> CreateTopicAsync(Topic topic);
        Task<IndividualTopicResponse> UpdateTopicAsync(Topic topic);
        Task<bool> DeleteTopicAsync(long topicId);

    }
    public class Topics : Core, ITopics
    {
        public Topics(string yourZendeskUrl, string user, string password, string apiToken, string p_OAuthToken)
            : base(yourZendeskUrl, user, password, apiToken, p_OAuthToken)
        {
        }
        public GroupTopicResponse GetTopics(int? perPage = null, int? page = null)
        {
            return GenericPagedGet<GroupTopicResponse>("community/topics.json", perPage, page);
        }

        public IndividualTopicResponse GetTopic(long topicId)
        {
            return GenericGet<IndividualTopicResponse>($"community/topics/{topicId}.json");
        }

        public IndividualTopicResponse CreateTopic(Topic topic)
        {
            var body = new { topic };
            return GenericPost<IndividualTopicResponse>("community/topics.json", body);
        }

        public IndividualTopicResponse UpdateTopic(Topic topic)
        {
            var body = new { topic };
            return GenericPut<IndividualTopicResponse>($"community/topics/{topic.Id.Value}.json", body);
        }

        public bool DeleteTopic(long topicId)
        {
            return GenericDelete($"community/topics/{topicId}.json");
        }
        public async Task<GroupTopicResponse> GetTopicsAsync(int? perPage = default(int?), int? page = default(int?))
        {
            return await GenericPagedGetAsync<GroupTopicResponse>("community/topics.json", perPage, page);
        }

        public async Task<IndividualTopicResponse> GetTopicAsync(long topicId)
        {
            return await GenericGetAsync<IndividualTopicResponse>($"community/topics/{topicId}.json");
        }

        public async Task<IndividualTopicResponse> CreateTopicAsync(Topic topic)
        {
            var body = new { topic };
            return await GenericPostAsync<IndividualTopicResponse>("community/topics.json", body);

        }

        public async Task<IndividualTopicResponse> UpdateTopicAsync(Topic topic)
        {
            var body = new { topic };
            return await GenericPutAsync<IndividualTopicResponse>($"community/topics/{topic.Id.Value}.json", body);
        }

        public async Task<bool> DeleteTopicAsync(long topicId)
        {
            return await GenericDeleteAsync($"community/topics/{topicId}.json)");
        }

    }
}
