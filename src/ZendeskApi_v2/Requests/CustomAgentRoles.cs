using System.Threading.Tasks;
using ZendeskApi_v2.Models.CustomRoles;

namespace ZendeskApi_v2.Requests
{
	public interface ICustomAgentRoles : ICore
	{
		CustomRoles GetCustomRoles();

		Task<CustomRoles> GetCustomRolesAsync();
	}

	public class CustomAgentRoles : Core, ICustomAgentRoles
	{
        public CustomAgentRoles(string yourZendeskUrl, string user, string password, string apiToken, string p_OAuthToken)
            : base(yourZendeskUrl, user, password, apiToken, p_OAuthToken)
        {
        }

        public CustomRoles GetCustomRoles()
        {
            return GenericGet<CustomRoles>("custom_roles.json");
        }

        public async Task<CustomRoles> GetCustomRolesAsync()
        {
            return await GenericGetAsync<CustomRoles>("custom_roles.json");
        }
    }
}