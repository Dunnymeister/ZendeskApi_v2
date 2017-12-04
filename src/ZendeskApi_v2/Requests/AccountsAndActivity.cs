using System.Threading.Tasks;
using ZendeskApi_v2.Models.AccountsAndActivities;


namespace ZendeskApi_v2.Requests
{
	public interface IAccountsAndActivity : ICore
	{
		SettingsResponse GetSettings();
		GroupActivityResponse GetActivities();
		IndividualActivityResponse GetActivityById(long activityId);

		Task<SettingsResponse> GetSettingsAsync();
		Task<GroupActivityResponse> GetActivitiesAync();
		Task<IndividualActivityResponse> GetActivityByIdAync(long activityId);
	}

	public class AccountsAndActivity : Core, IAccountsAndActivity
	{

        public AccountsAndActivity(string yourZendeskUrl, string user, string password, string apiToken, string p_OAuthToken)
            : base(yourZendeskUrl, user, password, apiToken, p_OAuthToken)
        {
        }
        public SettingsResponse GetSettings()
        {
            return GenericGet<SettingsResponse>("account/settings.json");
        }

        public GroupActivityResponse GetActivities()
        {
            return GenericGet<GroupActivityResponse>("activities.json");
        }

        public IndividualActivityResponse GetActivityById(long activityId)
        {
            return GenericGet<IndividualActivityResponse>($"activities/{activityId}.json");
        }

        public async Task<SettingsResponse> GetSettingsAsync()
        {
            return await GenericGetAsync<SettingsResponse>("account/settings.json");
        }        
        public async Task<GroupActivityResponse> GetActivitiesAync()
        {
            return await GenericGetAsync<GroupActivityResponse>("activities.json");
        }

        public async Task<IndividualActivityResponse> GetActivityByIdAync(long activityId)
        {
            return await GenericGetAsync<IndividualActivityResponse>($"activities/{activityId}.json");
        }
    }
}