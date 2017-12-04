using System.Threading.Tasks;
using ZendeskApi_v2.Models.Shared;

namespace ZendeskApi_v2.Requests
{
	public interface IJobStatuses : ICore
	{
		JobStatusResponse GetJobStatus(string id);

		Task<JobStatusResponse> GetJobStatusAsync(string id);
	}

	public class JobStatuses : Core, IJobStatuses
	{

        public JobStatuses(string yourZendeskUrl, string user, string password, string apiToken, string p_OAuthToken)
            : base(yourZendeskUrl, user, password, apiToken, p_OAuthToken)
        {
        }

        public JobStatusResponse GetJobStatus(string id)
        {
            return GenericGet<JobStatusResponse>($"job_statuses/{id}.json");
        }

        public async Task<JobStatusResponse> GetJobStatusAsync(string id)
        {
            return await GenericGetAsync<JobStatusResponse>($"job_statuses/{id}.json");
        }
    }
}