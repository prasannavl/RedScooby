// Author: Prasanna V. Loganathar
// Created: 12:13 AM 11-03-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using SQLite.Net.Attributes;

namespace RedScooby.Data.Tables
{
    [Table("Circles")]
    public class Circle
    {
        [PrimaryKey, AutoIncrement]
        public int? Id { get; set; }

        [Indexed]
        public string Name { get; set; }

        public string RemoteId { get; set; }
    }

    [Table("Contacts")]
    public class Contact
    {
        [PrimaryKey, AutoIncrement]
        public int? Id { get; set; }

        public string LocalStoreId { get; set; }
        public string RemoteId { get; set; }

        [Indexed]
        public string Name { get; set; }

        public string Notes { get; set; }
    }

    [Table("CircleItems")]
    public class CircleItem
    {
        [PrimaryKey, AutoIncrement]
        public int? Id { get; set; }

        [Indexed]
        public int CircleId { get; set; }

        public int ItemId { get; set; }
        public CircleItemType ItemType { get; set; }
    }

    public enum CircleItemType
    {
        Contact,
        Circle,
    }

    [Table("ContactPhoneNumbers")]
    public class ContactPhoneNumber
    {
        [PrimaryKey, AutoIncrement]
        public int? Id { get; set; }

        [Indexed]
        public int ContactId { get; set; }

        public string Number { get; set; }
    }


    [Table("ContactEmails")]
    public class ContactEmail
    {
        [PrimaryKey, AutoIncrement]
        public int? Id { get; set; }

        [Indexed]
        public int ContactId { get; set; }

        public string Email { get; set; }
    }
}
