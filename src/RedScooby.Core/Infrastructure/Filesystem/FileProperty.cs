// Author: Prasanna V. Loganathar
// Created: 8:52 PM 13-11-2014

using System;

namespace RedScooby.Infrastructure.Filesystem
{
    public class FileProperty : IFileProperty
    {
        public FileProperty(string name, string extension, string path, ulong size, string type,
            DateTimeOffset lastModifiedDate, DateTimeOffset createdDate, DateTimeOffset lastAccessedDate)
        {
            this.Name = name;
            this.Extension = extension;
            this.Path = path;
            this.Size = size;
            this.Type = type;
            this.LastModifiedDate = lastModifiedDate;
            this.CreatedDate = createdDate;
            this.LastAccessedDate = lastAccessedDate;
        }

        public string Name { get; private set; }
        public string Extension { get; private set; }
        public string Path { get; private set; }
        public ulong Size { get; private set; }
        public string Type { get; private set; }
        public DateTimeOffset LastModifiedDate { get; private set; }
        public DateTimeOffset CreatedDate { get; private set; }
        public DateTimeOffset LastAccessedDate { get; private set; }
    }
}