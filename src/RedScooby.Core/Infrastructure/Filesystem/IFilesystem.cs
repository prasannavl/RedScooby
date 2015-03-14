// Author: Prasanna V. Loganathar
// Created: 8:52 PM 13-11-2014

using System;
using System.IO;
using System.Threading.Tasks;

namespace RedScooby.Infrastructure.Filesystem
{
    public interface IFilesystem
    {
        Task<Stream> CreateFileAsync(string fileName);
        Task DeleteFileAsync(string fileName);
        Task<bool> FileExistsAsync(string fileName);
        Task<bool> DirectoryExistsAsync(string dirName);
        Task CreateDirectoryAsync(string dirName);
        Task EnsurePathExistsOrCreateAsync(string path);
        Task DeleteDirectoryAsync(string dirName, bool recursive = false);
        Task MoveFileAsync(string srcFileName, string destFileName);
        Task MoveDirectoryAsync(string srcDirName, string destDirName);
        Task<IFileProperty> GetPropertiesAsync(string fileName);
        Task<Stream> GetStreamAsync(string fileName, FileAccess accessMode);
    }

    public interface IFileProperty
    {
        string Name { get; }
        string Extension { get; }
        string Path { get; }
        ulong Size { get; }
        string Type { get; }
        DateTimeOffset LastModifiedDate { get; }
        DateTimeOffset CreatedDate { get; }
        DateTimeOffset LastAccessedDate { get; }
    }

    /// <summary>
    ///     Defines constants for read, write, or read/write access to a file.
    /// </summary>
    [Flags]
    public enum FileAccess : byte
    {
        /// <summary>
        ///     Read only access to the file.
        /// </summary>
        Read = 0x1,

        /// <summary>
        ///     Write only access to the file.
        /// </summary>
        Write = 0x02,

        /// <summary>
        ///     Read and write access.
        /// </summary>
        ReadWrite = Read | Write
    }
}