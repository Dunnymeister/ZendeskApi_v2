using System.Threading.Tasks;
using ZendeskApi_v2.Models.SharingAgreements;

namespace ZendeskApi_v2.Requests
{
	public interface ISharingAgreements : ICore
	{
		GroupSharingAgreementResponse GetSharingAgreements();

		Task<GroupSharingAgreementResponse> GetSharingAgreementsAsync();
	}

	public class SharingAgreements : Core, ISharingAgreements
	{

        public SharingAgreements(string yourZendeskUrl, string user, string password, string apiToken, string p_OAuthToken)
            : base(yourZendeskUrl, user, password, apiToken, p_OAuthToken)
        {
        }

        public GroupSharingAgreementResponse GetSharingAgreements()
        {
            return GenericGet<GroupSharingAgreementResponse>("sharing_agreements.json");
        }

        public async Task<GroupSharingAgreementResponse> GetSharingAgreementsAsync()
        {
            return await GenericGetAsync<GroupSharingAgreementResponse>("sharing_agreements.json");
        }
    }
}