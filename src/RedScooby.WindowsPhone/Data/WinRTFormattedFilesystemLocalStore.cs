// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.IO;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using Windows.Storage;
using RedScooby.Data.Core;

namespace RedScooby.Data
{
    internal sealed class WinRtFormattedFilesystemLocalStore : AsyncKeyValueStoreBase
    {
        public string Root;
        public StorageFolder RootFolder;
        private readonly MediaTypeFormatter formatter;
        private bool isInitialized;

        public WinRtFormattedFilesystemLocalStore(MediaTypeFormatter formatter, string root = "")
        {
            this.formatter = formatter;
            Root = root;
        }

        public override async Task<bool> ExistsAsync(string key)
        {
            await EnsureInitializedAsync().ConfigureAwait(false);
            try
            {
                var item = await RootFolder.GetItemAsync(key).AsTask().ConfigureAwait(false);
                if (item != null)
                    return true;
            }
            catch (FileNotFoundException)
            {
                return false;
            }

            return false;
        }

        public override async Task<T> GetAsync<T>(string key)
        {
            await EnsureInitializedAsync().ConfigureAwait(false);
            var data = await RootFolder.GetFileAsync(key).AsTask().ConfigureAwait(false);
            using (var stream = await data.OpenStreamForReadAsync().ConfigureAwait(false))
            {
                return (T) (await formatter.ReadFromStreamAsync(typeof (T), stream, null, null).ConfigureAwait(false));
            }
        }

        public override async Task RemoveAsync(string key)
        {
            await EnsureInitializedAsync().ConfigureAwait(false);
            var data = await RootFolder.GetItemAsync(key).AsTask().ConfigureAwait(false);
            await data.DeleteAsync().AsTask().ConfigureAwait(false);
        }

        public override async Task SetAsync<T>(string key, T data, bool overwrite = true)
        {
            await EnsureInitializedAsync().ConfigureAwait(false);
            var creationOptions = CreationCollisionOption.FailIfExists;
            if (overwrite)
            {
                creationOptions = CreationCollisionOption.ReplaceExisting;
            }

            var file = await RootFolder.CreateFileAsync(key, creationOptions).AsTask().ConfigureAwait(false);

            using (var stream = await file.OpenStreamForWriteAsync().ConfigureAwait(false))
                await formatter.WriteToStreamAsync(typeof (T), data, stream, null, null).ConfigureAwait(false);
        }

        public override async Task InitializeStoreAsync()
        {
            if (string.IsNullOrWhiteSpace(Root))
            {
                RootFolder = ApplicationData.Current.LocalFolder;
                return;
            }

            RootFolder = await
                ApplicationData.Current.LocalFolder.CreateFolderAsync(Root, CreationCollisionOption.OpenIfExists)
                    .AsTask()
                    .ConfigureAwait(false);
        }

        public override async Task DestroyStoreAsync()
        {
            try
            {
                var folder =
                    await ApplicationData.Current.LocalFolder.GetFolderAsync(Root).AsTask().ConfigureAwait(false);
                await folder.DeleteAsync(StorageDeleteOption.Default).AsTask().ConfigureAwait(false);
            }
            catch (FileNotFoundException) { }
        }

        public async Task EnsureInitializedAsync()
        {
            if (!isInitialized)
            {
                await InitializeStoreAsync().ConfigureAwait(false);
                isInitialized = true;
            }
        }
    }
}
