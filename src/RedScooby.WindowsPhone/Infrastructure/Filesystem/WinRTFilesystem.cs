using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.UI.Xaml;
using RedScooby.Infrastructure.Filesystem;

namespace RedScooby.Infrastructure.Filesystem
{
    class WinRTFilesystem : IFilesystem
    {
        private StorageFolder fsRoot;
        public WinRTFilesystem(StorageFolder storageRoot)
        {
            Contract.Requires(storageRoot != null);
            fsRoot = storageRoot;
        }
        public async Task<Stream> CreateFileAsync(string fileName)
        {
            var file = await fsRoot.CreateFileAsync(fileName);
            var stream = await file.OpenAsync(FileAccessMode.ReadWrite);
            return stream.AsStream();
        }

        public async Task DeleteFileAsync(string fileName)
        {
            var file = await fsRoot.GetFileAsync(fileName);
            await file.DeleteAsync();
        }

        public async Task<bool> FileExistsAsync(string fileName)
        {
            try
            {
                var res = await fsRoot.GetFileAsync(fileName);
                return true;
            }
            catch (FileNotFoundException)
            {
                return false;
            }
        }

        public async Task<bool> DirectoryExistsAsync(string dirName)
        {
            try
            {
                var res = await fsRoot.GetFolderAsync(dirName);
                return true;
            }
            catch (FileNotFoundException)
            {
                return false;
            }
        }

        public Task CreateDirectoryAsync(string dirName)
        {
            return fsRoot.CreateFolderAsync(dirName).AsTask();
        }

        public async Task EnsurePathExistsOrCreateAsync(string path)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteDirectoryAsync(string dirName, bool recursive = false)
        {
            var folder = await fsRoot.GetFolderAsync(dirName);
            await folder.DeleteAsync();
        }

        public async Task MoveFileAsync(string srcFileName, string destFileName)
        {
            var file = await fsRoot.GetFileAsync(srcFileName);
            var folder = await fsRoot.GetFolderAsync(Path.GetDirectoryName(destFileName));
            await file.MoveAsync(folder, Path.GetFileName(destFileName));
        }

        public async Task MoveDirectoryAsync(string srcDirName, string destDirName)
        {
            throw new NotImplementedException();
        }

        public Task<IFileProperty> GetPropertiesAsync(string fileName)
        {
            throw new NotImplementedException();
        }

        public async Task<Stream> GetStreamAsync(string fileName, FileAccess accessMode)
        {
            var rtAccessMode = accessMode == FileAccess.Read ? FileAccessMode.Read : FileAccessMode.ReadWrite;
            var file = await fsRoot.GetFileAsync(fileName);
            var stream = await file.OpenAsync(rtAccessMode);
            return stream.AsStream();
        }
    }
}
