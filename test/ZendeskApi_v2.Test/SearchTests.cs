using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Xunit;
using ZendeskApi_v2;
using ZendeskApi_v2.Models.Constants;
using ZendeskApi_v2.Models.Tickets;
using ZendeskApi_v2.Models.Users;

namespace Tests
{

    [Category("Search")]
    public class SearchTests
    {
        private ZendeskApi api = new ZendeskApi(Settings.Site, Settings.AdminEmail, Settings.AdminPassword);

        [Fact]
        public void CanSearch()
        {
            var res = api.Search.SearchFor(Settings.AdminEmail);
            Assert.Equal(res.Results[0].ResultType, "user");
            Assert.True(res.Results[0].Id > 0);
        }

        [Fact]
        public void BackwardCompatibilitAfterAddingPagination()
        {
            var res = api.Search.SearchFor("Effective", "created_at", "asc");
            Assert.True(res.Count > 0);
        }
        [Fact]
        public void TotalNumberOftickesShouldbeSameWhenReterivingNextPage()
        {
            var res = api.Search.SearchFor("Effective"); //search for a custom field - the results are more than one page
            var total = res.Count;

            Assert.True(res.Count > 0);
            Assert.True(res.Count > res.Results.Count); //result has more than one page
            Assert.True(!string.IsNullOrEmpty(res.NextPage)); //It has next page

            res = api.Search.SearchFor("Effective", "", "", 2); //fetch next page
            Assert.True(res.Count > 0);
            Assert.True(res.Count == total); //number of results should be same as page 1

        }
        [Fact]
        public void TicketHasSubject()
        {
            var res = api.Search.SearchFor("my printer is on fire");

            Assert.True(res != null);
            Assert.True(res.Results.Count > 0);
            Assert.True(!string.IsNullOrEmpty(res.Results[0].Subject));
        }

        [Fact]
        public void TicketSearchByTicketAnonymousType()
        {
            var res = api.Search.SearchFor<Ticket>("my printer is on fire");

            Assert.True(res != null);
            Assert.True(res.Results.Count > 10);
            Assert.True(!string.IsNullOrEmpty(res.Results[0].Subject));

            var noRes = api.Search.SearchFor<User>("my printer is on fire");

            Assert.True(noRes != null);
            Assert.True(noRes.Results.Count == 0);

            res = api.Search.SearchFor<Ticket>("my printer is on fire", perPage: 10);
            Assert.True(res != null);
            Assert.Equal(res.Results.Count, 10);
            Assert.Equal(res.Page, 1);
            Assert.True(res.Results[0] is Ticket);

        }

        [Fact]
        public async Task TicketSearchByTicketAnonymousTypeAsync()
        {
            var res = await api.Search.SearchForAsync<Ticket>("my printer is on fire");

            Assert.True(res != null);
            Assert.True(res.Results.Count > 10);
            Assert.True(!string.IsNullOrEmpty(res.Results[0].Subject));

            var noRes = await api.Search.SearchForAsync<User>("my printer is on fire");

            Assert.True(noRes != null);
            Assert.True(noRes.Results.Count == 0);

            res = await api.Search.SearchForAsync<Ticket>("my printer is on fire", perPage: 10);
            Assert.True(res != null);
            Assert.Equal(res.Results.Count, 10);
            Assert.Equal(res.Page, 1);
            Assert.True(res.Results[0] is Ticket);

        }

        [Fact]
        public void UserSearchByUserAnonymousType()
        {
            var res = api.Search.SearchFor<User>(Settings.AdminEmail);

            Assert.True(res != null);
            Assert.Equal(res.Results.Count, 1);
            Assert.Equal(res.Results[0].Id, Settings.UserId);
            Assert.True(res.Results[0] is User);
        }

        [Fact]
        public async Task UserSearchByUserAnonymousTypeAsync()
        {
            var res = await api.Search.SearchForAsync<User>(Settings.AdminEmail);

            Assert.True(res != null);
            Assert.Equal(res.Results.Count, 1);
            Assert.Equal(res.Results[0].Id, Settings.UserId);
            Assert.True(res.Results[0] is User);
        }

        [Fact]
        public void SearchSortIsWorking()
        {
            //desc asc 
            var res = api.Search.SearchFor<Ticket>("Effective", "created_at", "asc");
            Assert.True(res.Count > 2);
            var first = res.Results[0];
            var second = res.Results[1];
            Assert.True(second.CreatedAt > first.CreatedAt);

        }
    }
}