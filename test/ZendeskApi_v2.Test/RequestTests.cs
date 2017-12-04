
using System.Linq;
using Xunit;
using ZendeskApi_v2;
using ZendeskApi_v2.Models.Requests;
using ZendeskApi_v2.Models.Tickets;

namespace Tests
{

    public class RequestTests
    {
        private readonly ZendeskApi _api = new ZendeskApi(Settings.Site, Settings.AdminEmail, Settings.AdminPassword);

        [Fact]
        public void CanGetAllRequests()
        {
            var res = _api.Requests.GetAllRequests();
            Assert.True(res.Count > 0);
        }

        [TestCase(1, 1)]
        [TestCase(1, 2)]
        public void CanGetAllRequestsPaged(int perPage, int page)
        {
            var res = _api.Requests.GetAllRequests(perPage: perPage, page: page);

            Assert.NotNull(res);
            Assert.NotNull(res.Requests);
            Assert.Equal(perPage, res.PageSize);
            Assert.Equal(page, res.Page);
        }

        [Fact]
        public void CanGetAllRequestsSorted()
        {
            var unsorted = _api.Requests.GetAllRequests();

            Assert.NotNull(unsorted);
            Assert.NotNull(unsorted.Requests);
            Assert.Equal(unsorted.Requests.AsQueryable(), unsorted.Requests.AsQueryable());
            Assert.NotEqual(unsorted.Requests.OrderBy(request => request.UpdatedAt).AsQueryable(),
                unsorted.Requests.AsQueryable());

            var sorted = _api.Requests.GetAllRequests(sortCol: "updated_at", sortAscending: true);

            Assert.NotNull(sorted);
            Assert.NotNull(sorted.Requests);
            Assert.Equal(sorted.Requests.AsQueryable(), sorted.Requests.AsQueryable());
            Assert.Equal(sorted.Requests.OrderBy(request => request.UpdatedAt).AsQueryable(),
                sorted.Requests.AsQueryable());
        }

        [Fact]
        public void CanGetOpenRequests()
        {
            var res = _api.Requests.GetAllOpenRequests();
            Assert.True(res.Count > 0);
        }

        [TestCase(1, 1)]
        [TestCase(1, 2)]
        public void CanGetAllOpenRequestsPaged(int perPage, int page)
        {
            var res = _api.Requests.GetAllOpenRequests(perPage: perPage, page: page);

            Assert.NotNull(res);
            Assert.NotNull(res.Requests);
            Assert.Equal(perPage, res.PageSize);
            Assert.Equal(page, res.Page);
        }

        [Fact]
        public void CanGetAllOpenRequestsSorted()
        {
            var unsorted = _api.Requests.GetAllOpenRequests();

            Assert.NotNull(unsorted);
            Assert.NotNull(unsorted.Requests);
            Assert.Equal(unsorted.Requests.AsQueryable(), unsorted.Requests.AsQueryable());
            Assert.NotEqual(unsorted.Requests.OrderBy(request => request.UpdatedAt).AsQueryable(),
                unsorted.Requests.AsQueryable());

            var sorted = _api.Requests.GetAllOpenRequests(sortCol: "updated_at", sortAscending: true);

            Assert.NotNull(sorted);
            Assert.NotNull(sorted.Requests);
            Assert.Equal(sorted.Requests.AsQueryable(), sorted.Requests.AsQueryable());
            Assert.Equal(sorted.Requests.OrderBy(request => request.UpdatedAt).AsQueryable(),
                sorted.Requests.AsQueryable());
        }

        [Fact]
        public void CanGetAllSolvedRequests()
        {
            var res = _api.Requests.GetAllSolvedRequests();
            Assert.True(res.Count > 0);
        }

        [TestCase(1, 1)]
        [TestCase(1, 2)]
        public void CanGetAllSolvedRequestsPaged(int perPage, int page)
        {
            var res = _api.Requests.GetAllSolvedRequests(perPage: perPage, page: page);

            Assert.NotNull(res);
            Assert.NotNull(res.Requests);
            Assert.Equal(perPage, res.PageSize);
            Assert.Equal(page, res.Page);
        }

        [Fact]
        public void CanGetAllSolvedRequestsSorted()
        {
            var unsorted = _api.Requests.GetAllSolvedRequests();

            Assert.NotNull(unsorted);
            Assert.NotNull(unsorted.Requests);
            Assert.Equal(unsorted.Requests.AsQueryable(), unsorted.Requests.AsQueryable());
            Assert.NotEqual(unsorted.Requests.OrderBy(request => request.UpdatedAt).AsQueryable(),
                unsorted.Requests.AsQueryable());

            var sorted = _api.Requests.GetAllSolvedRequests(sortCol: "updated_at", sortAscending: true);

            Assert.NotNull(sorted);
            Assert.NotNull(sorted.Requests);
            Assert.Equal(sorted.Requests.AsQueryable(), sorted.Requests.AsQueryable());
            Assert.Equal(sorted.Requests.OrderBy(request => request.UpdatedAt).AsQueryable(),
                sorted.Requests.AsQueryable());
        }

        [Fact]
        public void CanCreateAndUpdateRequests()
        {
            var req = new Request
            {
                Subject = "end user request test",
                Comment = new Comment {Body = "end user test", HtmlBody = "end user test with </br> new line", Public = true}
            };

            var res = _api.Requests.CreateRequest(req);
            Assert.NotNull(res);
            Assert.NotNull(res.Request);
            Assert.True(res.Request.Id.HasValue);
            Assert.True(res.Request.Id.Value > 0);

            var res1 = _api.Requests.GetRequestById(res.Request.Id.Value);
            Assert.Equal(res1.Request.Id, res.Request.Id);

            res1.Request.Subject = "new subject";
            res1.Request.Comment = new Comment
            {
                Body = "something more to say",
                Public = true
            };

            var res2 = _api.Requests.UpdateRequest(res1.Request);
            //var res2 = api.Requests.UpdateRequest(res.Request.Id.Value, new Comment() {Body = "something more to say"});
            var res3 = _api.Requests.GetRequestCommentsById(res.Request.Id.Value);

            Assert.Equal(res3.Comments.Last().Body.Replace("\n", ""), "something more to say");

            var res4 = _api.Requests.GetSpecificRequestComment(res.Request.Id.Value, res3.Comments.Last().Id.Value);

            res1.Request.RequesterId = 56766413L;
            var res5 = _api.Requests.UpdateRequest(res1.Request);
            var res6 = _api.Requests.GetRequestById(res.Request.Id.Value);

            Assert.Equal(res5.Request.RequesterId, res6.Request.RequesterId);
            Assert.Equal(res4.Comment.Id, res3.Comments.Last().Id);

            Assert.True(_api.Tickets.Delete(res1.Request.Id.Value));
        }
    }
}
