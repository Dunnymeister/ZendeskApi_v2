﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;
using ZendeskApi_v2;
using ZendeskApi_v2.Extensions;
using ZendeskApi_v2.Models.Constants;
using ZendeskApi_v2.Models.Shared;
using ZendeskApi_v2.Models.Tickets;
using ZendeskApi_v2.Requests;
using ZendeskApi_v2.Models.Brands;
using ZendeskApi_v2.Test.Util;

namespace Tests
{

    [Category("Tickets")]
    public class TicketTests
    {
        private const string ResourceName = "ZendeskApi_v2.Test.Resources.testupload.txt";

        private ZendeskApi api = new ZendeskApi(Settings.Site, Settings.AdminEmail, Settings.AdminPassword);
        private TicketSideLoadOptionsEnum ticketSideLoadOptions = TicketSideLoadOptionsEnum.Users | TicketSideLoadOptionsEnum.Organizations | TicketSideLoadOptionsEnum.Groups;

        public TicketTests()
        {
            var response =  api.Tickets.GetTicketFieldsAsync().Result;
            foreach (var item in response.TicketFields)
            {
                if (item.Title == "My Tagger 2")
                {
                    api.Tickets.DeleteTicketFieldAsync(item.Id.Value);
                }
            }
        }

        [Fact]
        public void CanGetTicketsAsync()
        {
            var tickets = api.Tickets.GetAllTicketsAsync();
            Assert.True(tickets.Result.Count > 0);
        }

        [Fact]
        public void CanGetTicketsAsyncWithSideLoad()
        {
            var tickets = api.Tickets.GetAllTicketsAsync(sideLoadOptions: ticketSideLoadOptions);
            Assert.True(tickets.Result.Count > 0);
            Assert.True(tickets.Result.Users.Any());
            Assert.True(tickets.Result.Organizations.Any());
        }

        [Fact]
        public void CanCanGetTicketsByOrganizationIDAsync()
        {
            var id = Settings.OrganizationId;
            var tickets = api.Tickets.GetTicketsByOrganizationIDAsync(id);
            Assert.True(tickets.Result.Count > 0);
        }

        [Fact]
        public void CanGetTickets()
        {
            var tickets = api.Tickets.GetAllTickets();
            Assert.True(tickets.Count > 0);

            var count = 50;
            var nextPage = api.Tickets.GetByPageUrl<GroupTicketResponse>(tickets.NextPage, count);
            Assert.Equal(nextPage.Tickets.Count, count);

            var ticketsByUser = api.Tickets.GetTicketsByUserID(tickets.Tickets[0].RequesterId.Value);
            Assert.True(ticketsByUser.Count > 0);
        }

        [Fact]
        public void CanGetTicketsWithSideLoad()
        {
            var tickets = api.Tickets.GetAllTickets(sideLoadOptions: ticketSideLoadOptions);
            Assert.True(tickets.Count > 0);
            Assert.True(tickets.Users.Any());
            Assert.True(tickets.Organizations.Any());
        }

        [Fact]
        public void CanGetTicketsPaged()
        {
            const int count = 50;
            var tickets = api.Tickets.GetAllTickets(count);

            Assert.Equal(count, tickets.Tickets.Count);  // 50
            Assert.NotEqual(tickets.Count, tickets.Tickets.Count);   // 50 != total count of tickets (assumption)

            const int page = 3;
            var thirdPage = api.Tickets.GetAllTickets(count, page);

            Assert.Equal(count, thirdPage.Tickets.Count);

            var nextPage = thirdPage.NextPage.GetQueryStringDict()
                    .Where(x => x.Key == "page")
                        .Select(x => x.Value)
                        .FirstOrDefault();

            Assert.NotNull(nextPage);

            Assert.Equal(nextPage, (page + 1).ToString());
        }

        [Fact]
        public void CanGetTicketById()
        {
            var id = Settings.SampleTicketId;
            var ticket = api.Tickets.GetTicket(id).Ticket;
            Assert.NotNull(ticket);
            Assert.Equal(ticket.Id, id);
        }

        [Fact]
        public void CanGetTicketByIdWithSideLoad()
        {
            var id = Settings.SampleTicketId;
            var ticket = api.Tickets.GetTicket(id, sideLoadOptions: ticketSideLoadOptions);
            Assert.NotNull(ticket);
            Assert.NotNull(ticket.Ticket);
            Assert.Equal(ticket.Ticket.Id, id);
            Assert.True(ticket.Users.Any());
            Assert.True(ticket.Organizations.Any());
        }

        [Fact]
        public void CanGetTicketsByOrganizationId()
        {
            var id = Settings.OrganizationId;
            var tickets = api.Tickets.GetTicketsByOrganizationID(id);
            Assert.True(tickets.Count > 0);
        }

        [Fact]
        public void CanGetTicketsByOrganizationIdPaged()
        {
            var id = Settings.OrganizationId;
            var ticketsRes = api.Tickets.GetTicketsByOrganizationID(id, 2, 3);

            Assert.Equal(3, ticketsRes.PageSize);
            Assert.Equal(3, ticketsRes.Tickets.Count);
            Assert.True(ticketsRes.Count > 0);

            var nextPage = ticketsRes.NextPage.GetQueryStringDict()
                    .Where(x => x.Key == "page")
                        .Select(x => x.Value)
                        .FirstOrDefault();

            Assert.NotNull(nextPage);

            Assert.Equal("3", nextPage);
        }

        [Fact]
        public void CanGetTicketsByViewIdPaged()
        {
            var ticketsRes = api.Tickets.GetTicketsByViewID(Settings.ViewId, 10, 2);

            Assert.Equal(10, ticketsRes.PageSize);
            Assert.Equal(10, ticketsRes.Tickets.Count);
            Assert.True(ticketsRes.Count > 0);

            var nextPage = ticketsRes.NextPage.GetQueryStringDict()
                    .Where(x => x.Key == "page")
                        .Select(x => x.Value)
                        .FirstOrDefault();

            Assert.NotNull(nextPage);

            Assert.Equal("3", nextPage);
        }

        [Fact]
        public void CanGetTicketsByViewIdPagedWithSideLoad()
        {
            CanGetTicketsByViewIdPaged();
            var ticketsRes = api.Tickets.GetTicketsByViewID(Settings.ViewId, 10, 2, sideLoadOptions: ticketSideLoadOptions);

            Assert.True(ticketsRes.Users.Any());
            Assert.True(ticketsRes.Users.Any());
        }

        [Fact]
        public void CanTicketsByUserIdPaged()
        {
            var ticketsRes = api.Tickets.GetTicketsByUserID(Settings.UserId, 5, 2);

            Assert.Equal(5, ticketsRes.PageSize);
            Assert.Equal(5, ticketsRes.Tickets.Count);
            Assert.True(ticketsRes.Count > 0);

            var nextPage = ticketsRes.NextPage.GetQueryStringDict()
                    .Where(x => x.Key == "page")
                        .Select(x => x.Value)
                        .FirstOrDefault();

            Assert.NotNull(nextPage);

            Assert.Equal("3", nextPage);
        }

        [Fact]
        public void CanTicketsByUserIdPagedWithSideLoad()
        {
            CanTicketsByUserIdPaged();
            var ticketsRes = api.Tickets.GetTicketsByUserID(Settings.UserId, 5, 2, sideLoadOptions: ticketSideLoadOptions);
            Assert.True(ticketsRes.Users.Any());
            Assert.True(ticketsRes.Organizations.Any());
        }

        [Fact]
        public void CanTicketsByUserIdPagedAsyncWithSideLoad()
        {
            var ticketsRes = api.Tickets.GetTicketsByUserIDAsync(Settings.UserId, 5, 2, sideLoadOptions: ticketSideLoadOptions);
            Assert.True(ticketsRes.Result.Users.Any());
            Assert.True(ticketsRes.Result.Organizations.Any());
        }

        [Fact]
        public void CanAssignedTicketsByUserIdPaged()
        {
            var ticketsRes = api.Tickets.GetAssignedTicketsByUserID(Settings.UserId, 5, 2);

            Assert.Equal(5, ticketsRes.PageSize);
            Assert.Equal(5, ticketsRes.Tickets.Count);
            Assert.True(ticketsRes.Count > 0);

            var nextPage = ticketsRes.NextPage.GetQueryStringDict()
                    .Where(x => x.Key == "page")
                        .Select(x => x.Value)
                        .FirstOrDefault();

            Assert.NotNull(nextPage);

            Assert.Equal("3", nextPage);
        }

        [Fact]
        public void CanAssignedTicketsByUserIdPagedWithSideLoad()
        {
            CanTicketsByUserIdPaged();
            var ticketsRes = api.Tickets.GetAssignedTicketsByUserID(Settings.UserId, 5, 2, sideLoadOptions: ticketSideLoadOptions);
            Assert.True(ticketsRes.Users.Any());
            Assert.True(ticketsRes.Organizations.Any());
        }

        [Fact]
        public void CanAssignedTicketsByUserIdPagedAsyncWithSideLoad()
        {
            var ticketsRes = api.Tickets.GetAssignedTicketsByUserIDAsync(Settings.UserId, 5, 2, sideLoadOptions: ticketSideLoadOptions);
            Assert.True(ticketsRes.Result.Users.Any());
            Assert.True(ticketsRes.Result.Organizations.Any());
        }

        [Fact]
        public void CanGetMultipleTickets()
        {
            var ids = new List<long>() { Settings.SampleTicketId, Settings.SampleTicketId2 };
            var tickets = api.Tickets.GetMultipleTickets(ids);
            Assert.NotNull(tickets);
            Assert.Equal(tickets.Count, ids.Count);
        }

        [Fact]
        public async Task CanGetMultipleTicketsAsync()
        {
            var ids = new List<long>() { Settings.SampleTicketId, Settings.SampleTicketId2 };
            var tickets = await api.Tickets.GetMultipleTicketsAsync(ids);
            Assert.NotNull(tickets);
            Assert.Equal(tickets.Count, ids.Count);
        }

        [Fact]
        public void CanGetMultipleTicketsWithSideLoad()
        {
            var ids = new List<long>() { Settings.SampleTicketId, Settings.SampleTicketId2 };
            var tickets = api.Tickets.GetMultipleTickets(ids, sideLoadOptions: ticketSideLoadOptions);
            Assert.NotNull(tickets);
            Assert.Equal(tickets.Count, ids.Count);
            Assert.True(tickets.Users.Any());
            Assert.True(tickets.Organizations.Any());
        }

        [Fact]
        public async Task CanGetMultipleTicketsAsyncWithSideLoad()
        {
            var ids = new List<long>() { Settings.SampleTicketId, Settings.SampleTicketId2 };
            var tickets = await api.Tickets.GetMultipleTicketsAsync(ids, sideLoadOptions: ticketSideLoadOptions);
            Assert.NotNull(tickets);
            Assert.Equal(tickets.Count, ids.Count);
            Assert.True(tickets.Users.Any());
            Assert.True(tickets.Organizations.Any());
        }

        [Fact]
        public void CanGetMultipleTicketsSingleTicket()
        {
            var ids = new List<long>() { Settings.SampleTicketId };
            var tickets = api.Tickets.GetMultipleTickets(ids);
            Assert.NotNull(tickets);
            Assert.Equal(tickets.Count, ids.Count);
        }

        [Fact]
        public async Task CanGetMultipleTicketsAsyncSingleTicket()
        {
            var ids = new List<long>() { Settings.SampleTicketId };
            var tickets = await api.Tickets.GetMultipleTicketsAsync(ids);
            Assert.NotNull(tickets);
            Assert.Equal(tickets.Count, ids.Count);
        }

        [Fact]
        public void BooleanCustomFieldValuesArePreservedOnUpdate()
        {
            var ticket = new Ticket()
            {
                Subject = "my printer is on fire",
                Comment = new Comment() { Body = "HELP" },
                Priority = TicketPriorities.Urgent,
            };

            ticket.CustomFields = new List<CustomField>()
                {
                    new CustomField()
                        {
                            Id = Settings.CustomFieldId,
                            Value = "testing"
                        },
                    new CustomField()
                        {
                            Id = Settings.CustomBoolFieldId,
                            Value = true
                        }
                };

            var res = api.Tickets.CreateTicket(ticket).Ticket;
            Assert.Equal(ticket.CustomFields[1].Value, res.CustomFields.Where(f => f.Id == Settings.CustomBoolFieldId).FirstOrDefault().Value);

            //var updateResponse = api.Tickets.UpdateTicket(res, new Comment() { Body = "Just trying to update it!", Public = true});
            //res.UpdatedAt = null;
            //res.CreatedAt = null;

            var updateResponse = api.Tickets.UpdateTicket(res, new Comment() { Body = "Just trying to update it!", Public = true });

            Assert.Equal(ticket.CustomFields[1].Value, updateResponse.Ticket.CustomFields[1].Value);

            Assert.True(api.Tickets.Delete(res.Id.Value));
        }

        [Fact]
        public void CanCreateUpdateAndDeleteTicket()
        {
            var ticket = new Ticket()
            {
                Subject = "my printer is on fire",
                Comment = new Comment() { Body = "HELP" },
                Priority = TicketPriorities.Urgent
            };

            ticket.CustomFields = new List<CustomField>()
                {
                    new CustomField()
                        {
                            Id = Settings.CustomFieldId,
                            Value = "testing"
                        }
                };

            var res = api.Tickets.CreateTicket(ticket).Ticket;

            Assert.NotNull(res);
            Assert.True(res.Id > 0);

            Assert.Equal(res.CreatedAt, res.UpdatedAt);
            Assert.True(res.CreatedAt - DateTimeOffset.UtcNow <= TimeSpan.FromMinutes(1.0));

            res.Status = TicketStatus.Solved;
            res.AssigneeId = Settings.UserId;

            res.CollaboratorIds.Add(Settings.CollaboratorId);
            var body = "got it thanks";

            res.CustomFields[0].Value = "updated";

            var updateResponse = api.Tickets.UpdateTicket(res, new Comment() { Body = body, Public = true, Uploads = new List<string>() });

            Assert.NotNull(updateResponse);
            //Assert.Equal(updateResponse.Audit.Events.First().Body, body);
            Assert.True(updateResponse.Ticket.CollaboratorIds.Count > 0);
            Assert.True(updateResponse.Ticket.UpdatedAt >= updateResponse.Ticket.CreatedAt);

            Assert.True(api.Tickets.Delete(res.Id.Value));
        }

        [Fact]
        public void CanCreateUpdateAndDeleteHTMLTicket()
        {
            var ticket = new Ticket()
            {
                Subject = "my printer is on fire",
                Comment = new Comment() { HtmlBody = "HELP</br>HELP On a New line." },
                Priority = TicketPriorities.Urgent
            };

            ticket.CustomFields = new List<CustomField>()
                {
                    new CustomField()
                        {
                            Id = Settings.CustomFieldId,
                            Value = "testing"
                        }
                };

            var res = api.Tickets.CreateTicket(ticket).Ticket;

            Assert.NotNull(res);
            Assert.True(res.Id > 0);

            Assert.Equal(res.CreatedAt, res.UpdatedAt);
            Assert.True(res.CreatedAt - DateTimeOffset.UtcNow <= TimeSpan.FromMinutes(1.0));

            res.Status = TicketStatus.Solved;
            res.AssigneeId = Settings.UserId;

            res.CollaboratorIds.Add(Settings.CollaboratorId);
            var htmlBody = "HELP</br>HELP On a New line.";

            res.CustomFields[0].Value = "updated";

            var updateResponse = api.Tickets.UpdateTicket(res, new Comment() { HtmlBody = htmlBody, Public = true, Uploads = new List<string>() });

            Assert.NotNull(updateResponse);
            //Assert.Equal(updateResponse.Audit.Events.First().Body, body);
            Assert.True(updateResponse.Ticket.CollaboratorIds.Count > 0);
            Assert.True(updateResponse.Ticket.UpdatedAt >= updateResponse.Ticket.CreatedAt);

            Assert.True(api.Tickets.Delete(res.Id.Value));
        }

        [Fact]
        public void CanGetTicketComments()
        {
            var comments = api.Tickets.GetTicketComments(2);
            Assert.NotEmpty(comments.Comments[1].Body);
        }

        [Fact]
        public void CanGetTicketHTMLComments()
        {
            var comments = api.Tickets.GetTicketComments(2);
            Assert.NotEmpty(comments.Comments[1].HtmlBody);
        }

        [Fact]
        public void CanGetTicketCommentsWithSideLoading()
        {
            var comments = api.Tickets.GetTicketComments(2, sideLoadOptions: ticketSideLoadOptions);
            Assert.NotEmpty(comments.Users);
            Assert.Null(comments.Organizations);
        }

        [Fact]
        public void CanGetTicketCommentsPaged()
        {
            const int perPage = 5;
            const int page = 2;
            var commentsRes = api.Tickets.GetTicketComments(2, perPage, page);

            Assert.Equal(perPage, commentsRes.Comments.Count);
            Assert.Equal(perPage, commentsRes.PageSize);
            Assert.Equal(page, commentsRes.Page);

            Assert.NotEmpty(commentsRes.Comments[1].Body);

            var nextPageValue = commentsRes.NextPage.GetQueryStringDict()
                    .Where(x => x.Key == "page")
                        .Select(x => x.Value)
                        .FirstOrDefault();

            Assert.NotNull(nextPageValue);

            Assert.Equal((page + 1).ToString(), nextPageValue);
        }

        [Fact]
        public void CanCreateTicketWithRequester()
        {
            var ticket = new Ticket()
            {
                Subject = "ticket with requester",
                Comment = new Comment() { Body = "testing requester" },
                Priority = TicketPriorities.Normal,
                Requester = new Requester() { Email = Settings.ColloboratorEmail }
            };

            var res = api.Tickets.CreateTicket(ticket).Ticket;

            Assert.NotNull(res);
            Assert.Equal(res.RequesterId, Settings.CollaboratorId);

            Assert.True(api.Tickets.Delete(res.Id.Value));
        }

        [Fact]
        public async Task CanCreateTicketWithRequesterAsync()
        {
            var ticket = new Ticket()
            {
                Subject = "ticket with requester",
                Comment = new Comment() { Body = "testing requester" },
                Priority = TicketPriorities.Normal,
                Requester = new Requester() { Email = Settings.ColloboratorEmail }
            };

            var res = await api.Tickets.CreateTicketAsync(ticket);

            Assert.NotNull(res);
            Assert.NotNull(res.Ticket);
            Assert.Equal(res.Ticket.RequesterId, Settings.CollaboratorId);

            Assert.True(api.Tickets.Delete(res.Ticket.Id.Value));
        }

        [Fact]
        public void CanCreateTicketWithDueDate()
        {
            //31 December 2020 2AM
            var dueAt = DateTimeOffset.Parse("12/31/2020 07:00:00 -05:00", new CultureInfo("en-us"));

            var ticket = new Ticket()
            {
                Subject = "ticket with due date",
                Comment = new Comment() { Body = "test comment" },
                Type = "task",
                Priority = TicketPriorities.Normal,
                DueAt = dueAt
            };

            var res = api.Tickets.CreateTicket(ticket).Ticket;

            Assert.NotNull(res);
            Assert.Equal(res.DueAt, dueAt);

            Assert.True(api.Tickets.Delete(res.Id.Value));
        }

        [Fact]
        public void CanCreateTicketWithTicketFormId()
        {
            var ticket = new Ticket()
            {
                Subject = "ticket with ticket form id",
                Comment = new Comment() { Body = "testing requester" },
                Priority = TicketPriorities.Normal,
                TicketFormId = Settings.TicketFormId
            };

            var res = api.Tickets.CreateTicket(ticket).Ticket;

            Assert.NotNull(res);
            Assert.Equal(Settings.TicketFormId, res.TicketFormId);

            Assert.True(api.Tickets.Delete(res.Id.Value));
        }

        [Fact]
        public void CanBulkUpdateTickets()
        {
            var t1 = api.Tickets.CreateTicket(new Ticket()
            {
                Subject = "testing bulk update",
                Comment = new Comment() { Body = "HELP" },
                Priority = TicketPriorities.Normal
            }).Ticket;
            var t2 = api.Tickets.CreateTicket(new Ticket()
            {
                Subject = "more testing for bulk update",
                Comment = new Comment() { Body = "Bulk UpdateTicket testing" },
                Priority = TicketPriorities.Normal
            }).Ticket;

            var res = api.Tickets.BulkUpdate(new List<long>() { t1.Id.Value, t2.Id.Value }, new BulkUpdate()
            {
                Status = TicketStatus.Solved,
                Comment = new Comment() { Public = true, Body = "check your email" },
                CollaboratorEmails = new List<string>() { Settings.ColloboratorEmail },
                AssigneeId = Settings.UserId
            });

            Assert.Equal(res.JobStatus.Status, "queued");

            //also test JobStatuses while we have a job here
            var job = api.JobStatuses.GetJobStatus(res.JobStatus.Id);
            Assert.Equal(job.JobStatus.Id, res.JobStatus.Id);

            Assert.True(api.Tickets.DeleteMultiple(new List<long>() { t1.Id.Value, t2.Id.Value }));
        }

        [Fact]
        public async Task CanAddAttachmentToTicketAsync()
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

            Assert.True(await api.Tickets.DeleteAsync(t1.Ticket.Id.Value));
        }

        [Fact]
        public void CanAddAttachmentToTicket()
        {
            var fileData = ResourceUtil.GetResource(ResourceName);

            var res = api.Attachments.UploadAttachment(new ZenFile()
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

            var t1 = api.Tickets.CreateTicket(ticket);
            Assert.Equal(t1.Audit.Events.First().Attachments.Count, 1);

            Assert.True(api.Tickets.Delete(t1.Ticket.Id.Value));
            Assert.True(api.Attachments.DeleteUpload(res));
        }

        [Fact]
        public void CanGetCollaborators()
        {
            var res = api.Tickets.GetCollaborators(Settings.SampleTicketId);
            Assert.True(res.Users.Count > 0);
        }

        [Fact]
        public void CanGetIncidents()
        {
            var t1 = api.Tickets.CreateTicket(new Ticket()
            {
                Subject = "test problem",
                Comment = new Comment() { Body = "testing incidents with problems" },
                Priority = TicketPriorities.Normal,
                Type = TicketTypes.Problem
            }).Ticket;

            var t2 = api.Tickets.CreateTicket(new Ticket()
            {
                Subject = "incident",
                Comment = new Comment() { Body = "testing incidents" },
                Priority = TicketPriorities.Normal,
                Type = TicketTypes.Incident,
                ProblemId = t1.Id
            }).Ticket;

            var res = api.Tickets.GetIncidents(t1.Id.Value);
            Assert.True(res.Tickets.Count > 0);

            Assert.True(api.Tickets.DeleteMultiple(new List<long>() { t1.Id.Value, t2.Id.Value }));
        }

        [Fact]
        public void CanGetProblems()
        {
            var t1 = api.Tickets.CreateTicket(new Ticket()
            {
                Subject = "test problem",
                Comment = new Comment() { Body = "testing incidents with problems" },
                Priority = TicketPriorities.Normal,
                Type = TicketTypes.Problem
            }).Ticket;

            var res = api.Tickets.GetProblems();
            Assert.True(res.Tickets.Count > 0);

            Assert.True(api.Tickets.Delete(t1.Id.Value));
        }

        [Fact]
        public void CanGetInrementalTicketExportTestOnly()
        {
            var res = api.Tickets.__TestOnly__GetIncrementalTicketExport(DateTime.Now.AddDays(-1));
            Assert.True(res.Count > 0);
        }

        [Fact]
        public void CanGetIncrementalTicketExportPaged()
        {
            const int maxTicketsPerPage = 1000;

            var res = api.Tickets.GetIncrementalTicketExport(DateTime.Now.AddDays(-365));

            Assert.Equal(maxTicketsPerPage, res.Tickets.Count);
            Assert.NotNull(res.NextPage);
        }

        [Fact]
        public void CanGetIncrementalTicketExportWithUsersSideLoadPaged()
        {
            const int maxTicketsPerPage = 1000;

            var res = api.Tickets.GetIncrementalTicketExport(DateTime.Now.AddDays(-365), TicketSideLoadOptionsEnum.Users);

            Assert.Equal(maxTicketsPerPage, res.Tickets.Count);
            Assert.True(res.Users.Count > 0);
            Assert.NotNull(res.NextPage);

            res = this.api.Tickets.GetIncrementalTicketExportNextPage(res.NextPage);

            Assert.True(res.Tickets.Count > 0);
            Assert.True(res.Users.Count > 0);
        }

        [Fact]
        public void CanGetIncrementalTicketExportWithGroupsSideLoadPaged()
        {
            const int maxTicketsPerPage = 1000;

            var res = api.Tickets.GetIncrementalTicketExport(DateTime.Now.AddDays(-700), TicketSideLoadOptionsEnum.Groups);

            Assert.Equal(maxTicketsPerPage, res.Tickets.Count);
            Assert.True(res.Groups.Count > 0);
            Assert.NotNull(res.NextPage);

            res = this.api.Tickets.GetIncrementalTicketExportNextPage(res.NextPage);

            Assert.True(res.Tickets.Count > 0);
            Assert.True(res.Groups.Count > 0);
        }

        [Fact]
        public void CanGetTicketFields()
        {
            var res = api.Tickets.GetTicketFields();
            Assert.True(res.TicketFields.Count > 0);
        }

        [Fact]
        public void CanGetTicketFieldById()
        {
            var id = Settings.CustomFieldId;
            var ticketField = api.Tickets.GetTicketFieldById(id).TicketField;
            Assert.NotNull(ticketField);
            Assert.Equal(ticketField.Id, id);
        }

        [Fact]
        public void CanGetTicketFieldByIdAsync()
        {
            var id = Settings.CustomFieldId;
            var ticketField = api.Tickets.GetTicketFieldByIdAsync(id).Result.TicketField;
            Assert.NotNull(ticketField);
            Assert.Equal(ticketField.Id, id);
        }

        [Fact]
        public void CanCreateUpdateAndDeleteTicketFields()
        {
            var tField = new TicketField()
            {
                Type = TicketFieldTypes.Text,
                Title = "MyField",
            };

            var res = api.Tickets.CreateTicketField(tField);
            Assert.NotNull(res.TicketField);

            var updatedTF = res.TicketField;
            updatedTF.Title = "My Custom Field";

            var updatedRes = api.Tickets.UpdateTicketField(updatedTF);
            Assert.Equal(updatedRes.TicketField.Title, updatedTF.Title);

            Assert.True(api.Tickets.DeleteTicketField(updatedTF.Id.Value));
        }

        [Fact]
        public void CanCreateAndDeleteTaggerTicketField()
        {
            var tField = new TicketField()
            {
                Type = TicketFieldTypes.Tagger,
                Title = "My Tagger",
                Description = "test description",
                TitleInPortal = "Test Tagger",
                CustomFieldOptions = new List<CustomFieldOptions>()
            };

            tField.CustomFieldOptions.Add(new CustomFieldOptions()
            {
                Name = "test entry",
                Value = "test value"
            });

            var res = api.Tickets.CreateTicketField(tField);
            Assert.NotNull(res.TicketField);

            Assert.True(api.Tickets.DeleteTicketField(res.TicketField.Id.Value));
        }

        [Fact]
        public void CanCreateUpdateOptionsAndDeleteTaggerTicketField()
        {
            // https://support.zendesk.com/hc/en-us/articles/204579973--BREAKING-Update-ticket-field-dropdown-fields-by-value-instead-of-id-

            var option1 = "test_value_a";
            var option1_Update = "test_value_a_newtag";
            var option2 = "test_value_b";
            var option3 = "test_value_c";

            var tField = new TicketField()
            {
                Type = TicketFieldTypes.Tagger,
                Title = "My Tagger 2",
                Description = "test description",
                TitleInPortal = "Test Tagger",
                CustomFieldOptions = new List<CustomFieldOptions>()
            };

            tField.CustomFieldOptions.Add(new CustomFieldOptions()
            {
                Name = "test entryA",
                Value = option1
            });

            tField.CustomFieldOptions.Add(new CustomFieldOptions()
            {
                Name = "test entryB",
                Value = option2
            });

            var res = api.Tickets.CreateTicketField(tField);
            Assert.NotNull(res.TicketField);
            Assert.NotNull(res.TicketField.Id);
            Assert.Equal(res.TicketField.CustomFieldOptions.Count, 2);
            Assert.Equal(res.TicketField.CustomFieldOptions[0].Value, option1);
            Assert.Equal(res.TicketField.CustomFieldOptions[1].Value, option2);

            var id = res.TicketField.Id.Value;

            var tFieldU = new TicketField()
            {
                Id = id,
                CustomFieldOptions = new List<CustomFieldOptions>()
            };

            //update CustomFieldOption A
            tFieldU.CustomFieldOptions.Add(new CustomFieldOptions()
            {
                Name = "test entryA newTitle",
                Value = option1_Update
            });
            //delete CustomFieldOption B
            //add CustomFieldOption C
            tFieldU.CustomFieldOptions.Add(new CustomFieldOptions()
            {
                Name = "test entryC",
                Value = option3
            });

            var resU = api.Tickets.UpdateTicketField(tFieldU);

            Assert.Equal(resU.TicketField.CustomFieldOptions.Count, 2);
            Assert.Equal(resU.TicketField.CustomFieldOptions[0].Value, option1_Update);
            Assert.NotEqual(resU.TicketField.CustomFieldOptions[1].Value, option2);

            Assert.True(api.Tickets.DeleteTicketField(id));
        }

        [Fact(Skip="Need to Create Suspended Ticket Working with Zendesk support Team")]
        public void CanGetSuspendedTickets()
        {
            var all = api.Tickets.GetSuspendedTickets();
            Assert.True(all.Count > 0);

            var ind = api.Tickets.GetSuspendedTicketById(all.SuspendedTickets[0].Id);
            Assert.Equal(ind.SuspendedTicket.Id, all.SuspendedTickets[0].Id);

            //There is no way to suspend a ticket so I can run a tests for recovering and deleting them
        }

        [Fact]
        public void CanGetTicketForms()
        {
            var res = api.Tickets.GetTicketForms();
            Assert.True(res.Count > 0);
        }

        [Fact]
        public void CanCreateUpdateAndDeleteTicketForms()
        {
            //api.Tickets.DeleteTicketForm(52523);
            //return;

            var res = api.Tickets.CreateTicketForm(new TicketForm()
            {
                Name = "Snowboard Problem",
                EndUserVisible = true,
                DisplayName = "Snowboard Damage",
                Position = 2,
                Active = true,
                Default = false
            });

            Assert.NotNull(res);
            Assert.True(res.TicketForm.Id > 0);

            var get = api.Tickets.GetTicketFormById(res.TicketForm.Id.Value);
            Assert.Equal(get.TicketForm.Id, res.TicketForm.Id);

            res.TicketForm.Name = "Snowboard Fixed";
            res.TicketForm.DisplayName = "Snowboard has been fixed";
            res.TicketForm.Active = false;

            var update = api.Tickets.UpdateTicketForm(res.TicketForm);
            Assert.Equal(update.TicketForm.Name, res.TicketForm.Name);

            Assert.True(api.Tickets.DeleteTicketForm(res.TicketForm.Id.Value));
        }

        [Fact]
        public void CanReorderTicketForms()
        {
        }

        [Fact]
        public void CanCloneTicketForms()
        {
        }

        [Fact]
        public void CanGetAllTicketMetrics()
        {
            var metrics = api.Tickets.GetAllTicketMetrics();
            Assert.True(metrics.Count > 0);
            var count = 50;
            var nextPage = api.Tickets.GetByPageUrl<GroupTicketMetricResponse>(metrics.NextPage, count);
            Assert.Equal(nextPage.TicketMetrics.Count, count);
        }

        [Fact]
        public void CanGetTicketMetricsAsync()
        {
            var tickets = api.Tickets.GetAllTicketMetricsAsync();
            Assert.True(tickets.Result.Count > 0);
        }

        [Fact]
        public void CanGetTicketMetricByTicketId()
        {
            var id = Settings.SampleTicketId;
            var metric = api.Tickets.GetTicketMetricsForTicket(id).TicketMetric;
            Assert.NotNull(metric);
            Assert.Equal(metric.TicketId, id);
        }

        [Fact]
        public void CanGetTicketMetricByTicketIdAsync()
        {
            var id = Settings.SampleTicketId;
            var metric = api.Tickets.GetTicketMetricsForTicketAsync(id).Result.TicketMetric;
            Assert.NotNull(metric);
            Assert.Equal(metric.TicketId, id);
        }

        [Fact]
        public void CanGetAllTicketsWithSideLoad()
        {
            var tickets =
                api.Tickets.GetAllTickets(sideLoadOptions: ticketSideLoadOptions);

            Assert.True(tickets.Users.Any());
            Assert.True(tickets.Organizations.Any());
        }

        [Fact]
        public void CanGetAllTicketsAsyncWithSideLoad()
        {
            var tickets =
                api.Tickets.GetAllTicketsAsync(sideLoadOptions: ticketSideLoadOptions);

            Assert.True(tickets.Result.Users.Any());
            Assert.True(tickets.Result.Organizations.Any());
        }

        [Fact]
        public void CanGetTicketsByOrganizationIDAsyncWithSideLoad()
        {
            var id = Settings.OrganizationId;
            var tickets = api.Tickets.GetTicketsByOrganizationIDAsync(id, sideLoadOptions: ticketSideLoadOptions);
            Assert.True(tickets.Result.Count > 0);
            Assert.True(tickets.Result.Users.Any());
            Assert.True(tickets.Result.Organizations.Any());
        }

        [Fact]
        public void CanCanGetTicketsByOrganizationIDWithSideLoad()
        {
            var id = Settings.OrganizationId;
            var tickets = api.Tickets.GetTicketsByOrganizationID(id, sideLoadOptions: ticketSideLoadOptions);
            Assert.True(tickets.Count > 0);
            Assert.True(tickets.Users.Any());
            Assert.True(tickets.Organizations.Any());
        }

        [Fact]
        public void CanImportTicket()
        {
            var ticket = new TicketImport()
            {
                Subject = "my printer is on fire",
                Comments = new List<TicketImportComment> { new TicketImportComment { AuthorId = Settings.UserId, Value = "HELP comment created in Import 1", Public = false, CreatedAt = DateTime.UtcNow.AddDays(-2) }, new TicketImportComment { AuthorId = Settings.UserId, Value = "HELP comment created in Import 2", Public = false, CreatedAt = DateTime.UtcNow.AddDays(-3) } },
                Priority = TicketPriorities.Urgent,
                CreatedAt = DateTime.Now.AddDays(-5),
                UpdatedAt = DateTime.Now.AddDays(-4),
                SolvedAt = DateTime.Now.AddDays(-3),
                Status = TicketStatus.Solved,
                AssigneeId = Settings.UserId,
                Description = "test description"
            };

            var res = api.Tickets.ImportTicket(ticket).Ticket;

            Assert.NotNull(res);
            Assert.True(res.Id.HasValue);
            Assert.True(res.Id.Value > 0);
            Assert.True(res.CreatedAt.Value.LocalDateTime < DateTime.Now.AddDays(-4));
            Assert.True(res.UpdatedAt.Value.LocalDateTime > res.CreatedAt.Value.LocalDateTime);
            Assert.Equal(res.Status, TicketStatus.Solved);
            Assert.Equal(res.Description, "test description");

            var resComments = api.Tickets.GetTicketComments(res.Id.Value);
            Assert.NotNull(resComments);
            Assert.Equal(resComments.Count, 3);

            api.Tickets.DeleteAsync(res.Id.Value);
            //Assert.True(res.SolvedAt.Value.LocalDateTime > res.UpdatedAt.Value.LocalDateTime);
        }

        [Fact]
        public void CanImportTicketAsync()
        {
            var ticket = new TicketImport()
            {
                Subject = "my printer is on fire",
                Comments = new List<TicketImportComment> { new TicketImportComment { AuthorId = Settings.UserId, Value = "HELP comment created in Import 1", Public = false, CreatedAt = DateTime.UtcNow.AddDays(-2) }, new TicketImportComment { AuthorId = Settings.UserId, Value = "HELP comment created in Import 2", Public = false, CreatedAt = DateTime.UtcNow.AddDays(-3) } },
                Priority = TicketPriorities.Urgent,
                CreatedAt = DateTime.Now.AddDays(-5),
                UpdatedAt = DateTime.Now.AddDays(-4),
                SolvedAt = DateTime.Now.AddDays(-3),
                Status = TicketStatus.Solved,
                AssigneeId = Settings.UserId,
                Description = "test description"
            };

            var res = api.Tickets.ImportTicketAsync(ticket);

            Assert.NotNull(res.Result);
            Assert.True(res.Result.Ticket.Id.Value > 0);
            Assert.True(res.Result.Ticket.CreatedAt.Value.LocalDateTime < DateTime.Now.AddDays(-4));
            Assert.True(res.Result.Ticket.UpdatedAt.Value.LocalDateTime > res.Result.Ticket.CreatedAt.Value.LocalDateTime);
            Assert.Equal(res.Result.Ticket.Status, TicketStatus.Solved);
            Assert.Equal(res.Result.Ticket.Description, "test description");

            var resComments = api.Tickets.GetTicketComments(res.Result.Ticket.Id.Value);
            Assert.NotNull(resComments);
            Assert.Equal(resComments.Count, 3);

            api.Tickets.DeleteAsync(res.Id);
        }

        [Fact]
        public void CanBulkImportTicket()
        {
            var test = new List<TicketImport>();

            for (var x = 0; x < 2; x++)
            {
                var ticket = new TicketImport()
                {
                    Subject = "my printer is on fire",
                    Comments = new List<TicketImportComment> { new TicketImportComment { AuthorId = Settings.UserId, Value = "HELP comment created in Import 1", CreatedAt = DateTime.UtcNow.AddDays(-2), Public = false }, new TicketImportComment { AuthorId = Settings.UserId, Value = "HELP comment created in Import 2", CreatedAt = DateTime.UtcNow.AddDays(-3), Public = false } },
                    Priority = TicketPriorities.Urgent,
                    CreatedAt = DateTime.Now.AddDays(-5),
                    UpdatedAt = DateTime.Now.AddDays(-4),
                    SolvedAt = DateTime.Now.AddDays(-3),
                    Status = TicketStatus.Solved,
                    AssigneeId = Settings.UserId,
                    Description = "test description"
                };
                test.Add(ticket);
            }

            var res = api.Tickets.BulkImportTickets(test);

            Assert.Equal(res.JobStatus.Status, "queued");

            var job = api.JobStatuses.GetJobStatus(res.JobStatus.Id);
            Assert.Equal(job.JobStatus.Id, res.JobStatus.Id);

            var count = 0;
            while (job.JobStatus.Status.ToLower() != "completed" && count < 10)
            {
                Thread.Sleep(1000);
                job = api.JobStatuses.GetJobStatus(res.JobStatus.Id);
                count++;
            }

            Assert.Equal(job.JobStatus.Status.ToLower(), "completed");

            foreach (var r in job.JobStatus.Results)
            {
                var ticket = api.Tickets.GetTicket(r.Id).Ticket;
                Assert.Equal(ticket.Description, "test description");
                var resComments = api.Tickets.GetTicketComments(r.Id);
                Assert.NotNull(resComments);
                Assert.Equal(resComments.Count, 3);
                foreach (var c in resComments.Comments)
                {
                    Assert.True(c.CreatedAt.HasValue);
                    Assert.True(c.CreatedAt.Value.LocalDateTime < DateTime.Now.AddDays(-1));
                }

                api.Tickets.DeleteAsync(r.Id);
            }
        }

        [Fact]
        public void CanCreateTicketWithPrivateComment()
        {
            var ticket = new Ticket { Comment = new Comment { Body = "This is a Test", Public = false } };

            var jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                DateParseHandling = DateParseHandling.DateTimeOffset,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
                ContractResolver = ZendeskApi_v2.Serialization.ZendeskContractResolver.Instance
            };

            var json = JsonConvert.SerializeObject(ticket, Formatting.None, jsonSettings);
            Assert.True(json.Contains("false"));
        }

        [Fact]
        public async Task ViaChannel_Set_To_API_Isseue_254()
        {
            // see https://github.com/mozts2005/ZendeskApi_v2/issues/254

            var ticket = new Ticket()
            {
                Subject = "my printer is on fire",
                Comment = new Comment() { Body = "HELP" },
                Priority = TicketPriorities.Urgent
            };

            ticket.CustomFields = new List<CustomField>()
                {
                    new CustomField()
                        {
                            Id = Settings.CustomFieldId,
                            Value = "testing"
                        }
                };

            var resp = await api.Tickets.CreateTicketAsync(ticket);
            var newTicket = resp.Ticket;

            Assert.Equal(newTicket.Via.Channel, "api");

            var comment = new Comment { Body = "New comment", Public = true };

            var resp2 = await api.Tickets.UpdateTicketAsync(newTicket, comment);
            var resp3 = await api.Tickets.GetTicketCommentsAsync(newTicket.Id.Value);

            Assert.False(resp3.Comments.Any(c => c.Via?.Channel != "api"));

            // clean up
            await api.Tickets.DeleteAsync(newTicket.Id.Value);
        }

        [Fact]
        public async Task TicketField()
        {
            var tField = new TicketField
            {
                Type = TicketFieldTypes.Tagger,
                Title = "My Tagger 2",
                Description = "test description",
                TitleInPortal = "Test Tagger",
                CustomFieldOptions = new List<CustomFieldOptions>
                {
                    new CustomFieldOptions
                    {
                        Name = "test entryA",
                        Value = "Test2"
                    },
                    new CustomFieldOptions
                    {
                        Name = "test entryB",
                        Value = "test3"
                    }
                }
            };

            var res = await api.Tickets.CreateTicketFieldAsync(tField);
            Assert.NotNull(res.TicketField);
            Assert.NotNull(res.TicketField.Id);
            Assert.Equal(res.TicketField.CustomFieldOptions.Count, 2);
        }

        [Fact]
        public async Task CanCreateUpdateOptionsAndDeleteTaggerTicketFieldAsync()
        {
            var option1 = "test_value_a";
            var option1_Update = "test_value_a_newtag";
            var option2 = "test_value_b";
            var option3 = "test_value_c";

            var tField = new TicketField()
            {
                Type = TicketFieldTypes.Tagger,
                Title = "My Tagger 2",
                Description = "test description",
                TitleInPortal = "Test Tagger",
                CustomFieldOptions = new List<CustomFieldOptions>()
            };

            tField.CustomFieldOptions.Add(new CustomFieldOptions()
            {
                Name = "test entryA",
                Value = option1
            });

            tField.CustomFieldOptions.Add(new CustomFieldOptions()
            {
                Name = "test entryB",
                Value = option2
            });

            var res = await api.Tickets.CreateTicketFieldAsync(tField);
            Assert.NotNull(res.TicketField);
            Assert.NotNull(res.TicketField.Id);
            Assert.Equal(res.TicketField.CustomFieldOptions.Count, 2);
            Assert.Equal(res.TicketField.CustomFieldOptions[0].Value, option1);
            Assert.Equal(res.TicketField.CustomFieldOptions[1].Value, option2);

            var id = res.TicketField.Id.Value;

            var tFieldU = new TicketField()
            {
                Id = id,
                CustomFieldOptions = new List<CustomFieldOptions>()
            };

            //update CustomFieldOption A
            tFieldU.CustomFieldOptions.Add(new CustomFieldOptions()
            {
                Name = "test entryA newTitle",
                Value = option1_Update
            });
            //delete CustomFieldOption B
            //add CustomFieldOption C
            tFieldU.CustomFieldOptions.Add(new CustomFieldOptions()
            {
                Name = "test entryC",
                Value = option3
            });

            var resU = await api.Tickets.UpdateTicketFieldAsync(tFieldU);

            Assert.Equal(resU.TicketField.CustomFieldOptions.Count, 2);
            Assert.Equal(resU.TicketField.CustomFieldOptions[0].Value, option1_Update);
            Assert.NotEqual(resU.TicketField.CustomFieldOptions[1].Value, option2);

            Assert.True(await api.Tickets.DeleteTicketFieldAsync(id));
        }

        [Fact]
        public async Task CanGetBrandId()
        {
            var brand = new Brand()
            {
                Name = "Test Brand",
                Active = true,
                Subdomain = $"test-{Guid.NewGuid()}"
            };

            var respBrand = api.Brands.CreateBrand(brand);

            brand = respBrand.Brand;

            var ticket = new Ticket { Comment = new Comment { Body = "This is a Brand id Test", Public = false }, BrandId = brand.Id };
            var respTicket = await api.Tickets.CreateTicketAsync(ticket);
            var respTikets = await api.Tickets.GetMultipleTicketsAsync(new List<long> { respTicket.Ticket.Id.Value });

            Assert.Equal(respTikets.Tickets[0].BrandId, brand.Id);

            // clean up
            Assert.True(api.Brands.DeleteBrand(brand.Id.Value));
            Assert.True(await api.Tickets.DeleteAsync(respTicket.Ticket.Id.Value));
        }

        [Fact]
        public async Task CanGetIsPublicAsync()
        {
            var ticket = new Ticket()
            {
                Subject = "my printer is on fire",
                Comment = new Comment { Body = "HELP", Public = true },
                Priority = TicketPriorities.Urgent
            };

            var resp1 = await api.Tickets.CreateTicketAsync(ticket);
            Assert.True(resp1.Ticket.IsPublic);

            ticket.Comment.Public = false;
            var resp2 = await api.Tickets.CreateTicketAsync(ticket);

            Assert.False(resp2.Ticket.IsPublic);

            Assert.True(await api.Tickets.DeleteAsync(resp1.Ticket.Id.Value));
            Assert.True(await api.Tickets.DeleteAsync(resp2.Ticket.Id.Value));
        }

        [Fact]
        public async Task CanGetSystemFieldOptions()
        {
            var resp = await api.Tickets.GetTicketFieldByIdAsync(21830872);

            Assert.NotNull(resp.TicketField.SystemFieldOptions);
        }

        [Fact]
        public async Task CanSetFollowupID()
        {
            var ticket = new Ticket { Comment = new Comment { Body = "This is a Test", Public = false } };

            var resp1 = await api.Tickets.CreateTicketAsync(ticket);

            var closedTicket = resp1.Ticket;

            closedTicket.Status = TicketStatus.Closed;

            var resp2 = await api.Tickets.UpdateTicketAsync(closedTicket, new Comment { Body = "Closing Ticket" });

            var ticket_Followup = new Ticket()
            {
                Subject = "This is a Test Follow up",
                Comment = new Comment { Body = "HELP", Public = true },
                Priority = TicketPriorities.Urgent,
                ViaFollowupSourceId = closedTicket.Id.Value
            };

            var resp3 = await api.Tickets.CreateTicketAsync(ticket_Followup);

            Assert.Equal(resp3.Ticket.Via.Source.Rel, "follow_up");

            Assert.True(await api.Tickets.DeleteAsync(resp3.Ticket.Id.Value));
            Assert.True(await api.Tickets.DeleteAsync(closedTicket.Id.Value));
        }
    }
}
