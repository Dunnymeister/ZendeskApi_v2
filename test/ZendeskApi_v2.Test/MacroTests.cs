using System.Collections.Generic;
using Xunit;
using ZendeskApi_v2;
using ZendeskApi_v2.Models.Constants;
using ZendeskApi_v2.Models.Macros;
using ZendeskApi_v2.Models.Tickets;

namespace Tests
{

    public class MacroTests
    {
        private ZendeskApi api = new ZendeskApi(Settings.Site, Settings.AdminEmail, Settings.AdminPassword);

        [Fact]
        public void CanGetMacros()
        {
            var all = api.Macros.GetAllMacros();
            Assert.True(all.Count > 0);

            var ind = api.Macros.GetMacroById(all.Macros[0].Id.Value);
            Assert.True(all.Count > 0);

            var active = api.Macros.GetActiveMacros();
            Assert.True(active.Count > 0);
        }

        [Fact]
        public void CanCreateUpdateAndDeleteMacros()
        {
            var create = api.Macros.CreateMacro(new Macro
            {
                Title = "Roger Wilco",
                Actions = new List<Action> { new Action { Field = "status", Value = new List<string> { "open" } } }
            });

            Assert.True(create.Macro.Id > 0);

            create.Macro.Title = "Roger wilco 2";
            var update = api.Macros.UpdateMacro(create.Macro);
            Assert.Equal(update.Macro.Id, create.Macro.Id);

            //Test apply macro
            var ticket = api.Tickets.CreateTicket(new Ticket
            {
                Subject = "macro test ticket",
                Comment = new Comment { Body = "Testing macros" },
                Priority = TicketPriorities.Normal
            }).Ticket;

            var applyToTicket = api.Macros.ApplyMacroToTicket(ticket.Id.Value, create.Macro.Id.Value);
            Assert.Equal(applyToTicket.Result.Ticket.Id, ticket.Id);
            Assert.True(api.Tickets.Delete(ticket.Id.Value));
            Assert.True(api.Macros.DeleteMacro(create.Macro.Id.Value));
        }

        [Fact]
        public void CanGetMacroByID()
        {
            var macro = api.Macros.GetMacroById(45319945);

            Assert.NotNull(macro);
        }
    }
}