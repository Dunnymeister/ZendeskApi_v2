using System;
using System.IO;
using Xunit;
using ZendeskApi_v2;
using ZendeskApi_v2.Models.Shared;
using System.Threading.Tasks;
using ZendeskApi_v2.Models.Tickets;
using ZendeskApi_v2.Models.Constants;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using ZendeskApi_v2.Test.Util;

namespace Tests
{
    public class AttachmentTests
    {
        private const string ResourceName = "ZendeskApi_v2.Test.Resources.testupload.txt";

        ZendeskApi api = new ZendeskApi(Settings.Site, Settings.AdminEmail, Settings.AdminPassword);

        [Fact]
        public void CanUploadAttachments()
        {
            var fileData = ResourceUtil.GetResource(ResourceName);
            var res = api.Attachments.UploadAttachment(new ZenFile()
            {
                ContentType = "text/plain",
                FileName = "testupload.txt",
                FileData = fileData
            });
            Assert.True(!string.IsNullOrEmpty(res.Token));
        }


        [Fact]
        public async Task CanDowloadAttachment()
        {
            var fileData = ResourceUtil.GetResource(ResourceName);

            var res = await api.Attachments.UploadAttachmentAsync(new ZenFile()
            {
                ContentType = "text/plain",
                FileName = "testupload.txt",
                FileData = fileData
            });

            var ticket = new Ticket()
            {
                Subject = "testing attachments",
                Priority = TicketPriorities.Normal,
                Comment = new Comment()
                {
                    Body = "comments are required for attachments",
                    Public = true,
                    Uploads = new List<string>() { res.Token }
                },
            };

            var t1 = await api.Tickets.CreateTicketAsync(ticket);
            Assert.Equal(t1.Audit.Events.First().Attachments.Count, 1);

            var test = t1.Audit.Events.First().Attachments.First();
            var file = await api.Attachments.DownloadAttachmentAsync(test);

            Assert.NotNull(file.FileData);

            Assert.True(api.Tickets.Delete(t1.Ticket.Id.Value));
            Assert.True(api.Attachments.DeleteUpload(res));
        }
    }
}
