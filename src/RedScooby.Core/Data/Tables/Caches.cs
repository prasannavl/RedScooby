// Author: Prasanna V. Loganathar
// Created: 7:10 PM 04-03-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using SQLite.Net.Attributes;

namespace RedScooby.Data.Tables
{
    public static class DbIntCacheIdentifiers
    {
        public const int SchemaVersion = 0;
    }

    internal class BlobCache
    {
        [PrimaryKey]
        public string Id { get; set; }

        public byte[] Data { get; set; }
    }

    internal class TextCache
    {
        [PrimaryKey]
        public string Id { get; set; }

        public string Data { get; set; }
    }

    internal class IntCache
    {
        [PrimaryKey]
        public int Id { get; set; }

        public int Data { get; set; }
    }
}
