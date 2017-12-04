﻿using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using ZendeskApi_v2;
using ZendeskApi_v2.Models.Schedules;

namespace Tests
{
    public class ScheduleTests
    {
        private ZendeskApi api = new ZendeskApi(Settings.Site, Settings.AdminEmail, Settings.AdminPassword);

        public ScheduleTests()
        {
            var schedules = api.Schedules.GetAllSchedules();
            if (schedules != null)
            {
                foreach (var schedule in schedules.Schedules.Where(o => o.Name.Contains("Test Schedule")))
                {
                    api.Schedules.DeleteSchedule(schedule.Id.Value);
                }

                foreach (var schedule in schedules.Schedules.Where(o => o.Name.Contains("Master Test Schedule")))
                {
                    api.Schedules.DeleteSchedule(schedule.Id.Value);
                }
            }

            var res = api.Schedules.CreateSchedule(new Schedule()
            {
                Name = "Master Test Schedule",
                TimeZone = "Pacific Time (US & Canada)"
            });
        }

        [Fact]
        public void CanGetSchedules()
        {
            var res = api.Schedules.GetAllSchedules();
            Assert.True(res.Schedules.Count > 0);

            var org = api.Schedules.GetSchedule(res.Schedules[0].Id.Value);
            Assert.Equal(org.Schedule.Id, res.Schedules[0].Id);
        }

        [Fact]
        public void CanCreateUpdateAndDeleteSchedule()
        {
            var res = api.Schedules.CreateSchedule(new Schedule()
            {
                Name = "Test Schedule",
                TimeZone = "Pacific Time (US & Canada)"
            });

            Assert.True(res.Schedule.Id > 0);

            res.Schedule.TimeZone = "Central Time (US & Canada)";
            var update = api.Schedules.UpdateSchedule(res.Schedule);
            Assert.Equal(update.Schedule.TimeZone, res.Schedule.TimeZone);

            Assert.True(api.Schedules.DeleteSchedule(res.Schedule.Id.Value));
        }

        [Fact]
        public void CanUpdateIntervals()
        {
            var res = api.Schedules.CreateSchedule(new Schedule()
            {
                Name = "Test Schedule",
                TimeZone = "Pacific Time (US & Canada)"
            });

            Assert.True(res.Schedule.Id > 0);

            var work = new WorkWeek();
            work.Intervals = res.Schedule.Intervals;

            work.Intervals[0].StartTime = 1860;
            work.Intervals[0].EndTime = 2460;
            var update = api.Schedules.UpdateIntervals(res.Schedule.Id.Value, work);

            Assert.True(update.WorkWeek.Intervals.Count > 0);

            Assert.Equal(work.Intervals[0].EndTime, update.WorkWeek.Intervals[0].EndTime);
            Assert.True(api.Schedules.DeleteSchedule(res.Schedule.Id.Value));
        }

        [Fact]
        public void CanCreateUpdateAndDeleteHoliday()
        {
            var res = api.Schedules.CreateSchedule(new Schedule()
            {
                Name = "Test Schedule",
                TimeZone = "Pacific Time (US & Canada)"
            });

            var res2 = api.Schedules.CreateHoliday(res.Schedule.Id.Value, new Holiday()
            {
                Name = "Test Holiday",
                StartDate = DateTimeOffset.Parse("2016-02-05"),
                EndDate = DateTimeOffset.Parse("2016-02-05")
            });

            Assert.True(res2.Holiday.Id > 0);

            res2.Holiday.EndDate = DateTimeOffset.Parse("2016-02-06");
            var update = api.Schedules.UpdateHoliday(res.Schedule.Id.Value, res2.Holiday);
            Assert.Equal(update.Holiday.Name, res2.Holiday.Name);
            Assert.Equal(update.Holiday.EndDate, res2.Holiday.EndDate);

            Assert.True(api.Schedules.DeleteHoliday(res.Schedule.Id.Value, res2.Holiday.Id.Value));
            Assert.True(api.Schedules.DeleteSchedule(res.Schedule.Id.Value));
        }
    }
}