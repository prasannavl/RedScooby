// Author: Prasanna V. Loganathar
// Created: 5:44 PM 11-03-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using SQLite.Net.Attributes;

namespace RedScooby.Data.Tables
{
    [Table("SyncLog")]
    public class SyncLogEntry
    {
        public SyncLogEntry(SyncUnit unit, SyncAction action, int unitId)
            : this(Guid.NewGuid().ToString(), unit, action, unitId, DateTimeOffset.Now) { }

        public SyncLogEntry(string id, SyncUnit unit, SyncAction action, int unitId, DateTimeOffset timestamp)
        {
            Id = id;
            Unit = unit;
            Action = action;
            UnitId = unitId;
            Timestamp = timestamp;
        }

        public string Id { get; set; }
        public SyncAction Action { get; set; }
        public SyncUnit Unit { get; set; }
        public int UnitId { get; set; }
        public DateTimeOffset Timestamp { get; set; }
    }

    public enum SyncUnit
    {
        UserData,
        UserSession,
        UserSettings,
        Circle,
        CircleProjection,
        CircleContact,
    }


    public enum SyncAction
    {
        Add,
        Remove,
        Modify,
    }
}
