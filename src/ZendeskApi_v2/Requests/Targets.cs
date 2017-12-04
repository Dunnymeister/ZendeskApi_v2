using System.Collections.Generic;
using System.Threading.Tasks;
using ZendeskApi_v2.Models.Targets;

namespace ZendeskApi_v2.Requests
{
    public interface ITargets : ICore
    {
        GroupTargetResponse GetAllTargets();
        IndividualTargetResponse GetTarget(long id);
        IndividualTargetResponse CreateTarget(BaseTarget target);
        IndividualTargetResponse UpdateTarget(BaseTarget target);
        bool DeleteTarget(long id);

        Task<GroupTargetResponse> GetAllTargetsAsync();
        Task<IndividualTargetResponse> GetTargetAsync(long id);
        Task<IndividualTargetResponse> CreateTargetAsync(BaseTarget target);
        Task<IndividualTargetResponse> UpdateTargetAsync(BaseTarget target);
        Task<bool> DeleteTargetAsync(long id);
    }
    public class Targets : Core, ITargets
    {
        public Targets(string yourZendeskUrl, string user, string password, string apiToken, string p_OAuthToken)
            : base(yourZendeskUrl, user, password, apiToken, p_OAuthToken)
        {
        }

        public GroupTargetResponse GetAllTargets()
        {
            return GenericGet<GroupTargetResponse>("targets.json");
        }

        public IndividualTargetResponse GetTarget(long id)
        {
            return GenericGet<IndividualTargetResponse>($"targets/{id}.json");
        }

        public IndividualTargetResponse CreateTarget(BaseTarget target)
        {
            var body = new { target };
            return GenericPost<IndividualTargetResponse>("targets.json", body);
        }

        public IndividualTargetResponse UpdateTarget(BaseTarget target)
        {
            var body = new { target };
            return GenericPut<IndividualTargetResponse>($"targets/{target.Id}.json", body);
        }

        public bool DeleteTarget(long id)
        {
            return GenericDelete($"targets/{id}.json");
        }

        public async Task<GroupTargetResponse> GetAllTargetsAsync()
        {
            return await GenericGetAsync<GroupTargetResponse>("targets.json");
        }

        public async Task<IndividualTargetResponse> GetTargetAsync(long id)
        {
            return await GenericGetAsync<IndividualTargetResponse>($"targets/{id}.json");
        }

        public async Task<IndividualTargetResponse> CreateTargetAsync(BaseTarget target)
        {
            var body = new { target };
            return await GenericPostAsync<IndividualTargetResponse>("targets.json", body);
        }

        public async Task<IndividualTargetResponse> UpdateTargetAsync(BaseTarget target)
        {
            var body = new { target };
            return await GenericPutAsync<IndividualTargetResponse>($"targets/{target.Id}.json", body);
        }

        public async Task<bool> DeleteTargetAsync(long id)
        {
            return await GenericDeleteAsync($"targets/{id}.json");
        }
    }
}
