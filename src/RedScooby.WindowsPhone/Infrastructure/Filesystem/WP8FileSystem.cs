// Author: Prasanna V. Loganathar
// Project: RedScooby.WP8
// 
// Copyright 2014 Launchark. All Rights Reserved.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//  
// Created: 3:28 AM 06-07-2014

using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Threading.Tasks;
using RedScooby.Core.Helpers;
using RedScooby.Core.Infrastructure.Components.FileSystem;
using FileAccess = RedScooby.Core.Infrastructure.FileSystem.FileAccess;

namespace RedScooby.Infrastructure.Filesystem
{
    public class WP8Filesystem : IFilesystem
    {
        public Task<Stream> CreateFileAsync(string fileName)
        {
            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                return Task.FromResult((Stream)store.CreateFile(fileName));
        }

        public Task DeleteFileAsync(string fileName)
        {
            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                store.DeleteFile(fileName);
            return TaskHelpers.CompletedTask;
        }

        public Task<bool> FileExistsAsync(string fileName)
        {
            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                if (store.FileExists(fileName))
                    return TaskHelpers.CompletedTrueResult;

            return TaskHelpers.CompletedFalseResult;
        }

        public Task<bool> DirectoryExistsAsync(string dirName)
        {
            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                if (store.DirectoryExists(dirName))
                    return TaskHelpers.CompletedTrueResult;

            return TaskHelpers.CompletedFalseResult;
        }

        public Task CreateDirectoryAsync(string dirName)
        {
            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                store.CreateDirectory(dirName);

            return TaskHelpers.CompletedTask;
        }

        public Task DeleteDirectoryAsync(string dirName, bool recursive = false)
        {
            if (recursive)
            {
                string[] names = null;
                using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    names = store.GetDirectoryNames(Path.Combine(dirName, "*"));
                }

                foreach (var directory in names)
                {
                    this.DeleteDirectoryAsync(directory, true);
                }

                using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    foreach (var file in store.GetFileNames(Path.Combine(dirName, "*")))
                    {
                        store.DeleteFile(Path.Combine(dirName, file));
                    }
                    store.DeleteDirectory(dirName);
                }

            }
            else
            {
                using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    store.DeleteDirectory(dirName);
                }
            }

            return TaskHelpers.CompletedTask;
        }

        public Task MoveFileAsync(string srcFileName, string destFileName)
        {
            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                store.MoveFile(srcFileName, destFileName);

            return TaskHelpers.CompletedTask;
        }

        public Task MoveDirectoryAsync(string srcDirName, string destDirName)
        {
            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                store.MoveDirectory(srcDirName, destDirName);

            return TaskHelpers.CompletedTask;
        }

        public Task<IFileProperty> GetPropertiesAsync(string fileName)
        {
            throw new NotImplementedException();
        }

        public Task<Stream> GetStreamAsync(string fileName, FileAccess accessMode)
        {
            Stream res = null;
            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                switch (accessMode)
                {
                    case FileAccess.Read:
                        res = store.OpenFile(fileName, FileMode.Open, System.IO.FileAccess.Read, FileShare.ReadWrite);
                        break;
                    case FileAccess.Write:
                        res = store.OpenFile(
                            fileName,
                            FileMode.OpenOrCreate,
                            System.IO.FileAccess.Write,
                            FileShare.ReadWrite);
                        break;
                    case FileAccess.ReadWrite:
                        res = store.OpenFile(
                            fileName,
                            FileMode.OpenOrCreate,
                            System.IO.FileAccess.ReadWrite,
                            FileShare.ReadWrite);
                        break;

                    default:
                        throw new Exception("Invalid access mode");
                }

            return Task.FromResult(res);
        }

        public async Task EnsurePathExistsOrCreateAsync(string path)
        {
            if (await DirectoryExistsAsync(path))
                return;

            var acc = string.Empty;
            foreach (var x in path.Split(Path.DirectorySeparatorChar))
            {
                var currentPath = Path.Combine(acc, x);

                if (currentPath[currentPath.Length - 1] == Path.VolumeSeparatorChar)
                {
                    currentPath += Path.DirectorySeparatorChar;
                }

                if (!await DirectoryExistsAsync(currentPath))
                {
                    await CreateDirectoryAsync(currentPath);
                    if (currentPath == path) 
                        return;
                }

                acc = currentPath;
            }
        }
    }
}