// Author: Prasanna V. Loganathar
// Created: 7:33 PM 15-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using RedScooby.Helpers;
using RedScooby.Infrastructure.Framework;
using RedScooby.Logging.Core;

namespace RedScooby.Logging
{
    public sealed class FileSystemLogger : IObserver<LogEvent>
    {
        public static int MaxLogFiles = 5;
        private readonly HybridInterlockedMonitor monitor = new HybridInterlockedMonitor();
        private StorageFolder logFolder;
        private StorageFile logFile;
        private LogLevel logLevels = LogLevel.Error | LogLevel.Critical;
        private StreamWriter writer;
        private FileSystemLogger() { }
        public static FileSystemLogger Current { get; private set; }

        public LogLevel LogLevels
        {
            get { return logLevels; }
            set { logLevels = value; }
        }

        public void OnCompleted() { }
        public void OnError(Exception error) { }

        public async void OnNext(LogEvent value)
        {
            if (!LogLevels.HasFlag(value.Level) || writer == null) return;
            await monitor.EnterAsync();
            try
            {
                var message = string.Format("{0} [{1}]: {2}", value.Timestamp.ToString("s"),
                    value.Level,
                    value.Message) + "---------------\r\n";

                await writer.WriteLineAsync(message).ConfigureAwait(false);
                await writer.FlushAsync().ConfigureAwait(false);
            }
            catch { }
            finally
            {
                monitor.Exit();
            }
        }

        public static async Task CreateAsync()
        {
            var logger = new FileSystemLogger();
            await logger.InitializeAsync().ConfigureAwait(false);
            Current = logger;
        }

        public async Task InitializeAsync()
        {
            await monitor.EnterAsync();
            try
            {
                logFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("logs",
                    CreationCollisionOption.OpenIfExists).AsTask().ConfigureAwait(false);

                var files = await logFolder.GetFilesAsync().AsTask().ConfigureAwait(false);
                var sorted = files.OrderByDescending(f => f.DateCreated);

                var currentFile = sorted.FirstOrDefault();
                var todaysLogName = DateTimeOffset.Now.ToString("yyyy-MM-dd") + ".log";

                if (currentFile == null || currentFile.Name != todaysLogName)
                {
                    currentFile =
                        await
                            logFolder.CreateFileAsync(todaysLogName,
                                CreationCollisionOption.OpenIfExists).AsTask().ConfigureAwait(false);
                }
                else if (files.Count > MaxLogFiles)
                {
                    var x = sorted.Skip(MaxLogFiles);
                    x.ForEach(f => f.DeleteAsync().AsTask().ContinueWithErrorHandling());
                }

                logFile = currentFile;

                var tcs = new TaskCompletionSource<bool>();
                Observable.FromAsync(
                    async () => await logFile.OpenAsync(FileAccessMode.ReadWrite).AsTask().ConfigureAwait(false))
                    .Retry(2)
                    .Subscribe(x =>
                    {
                        var stream = x.AsStream();
                        stream.Position = stream.Length;
                        writer = new StreamWriter(stream) {AutoFlush = false};
                        tcs.TrySetResult(true);
                    }, exception =>
                    {
                        var handler = ErrorHandler.Current;
                        if (handler != null)
                            handler.HandleError(exception);
                    });

                await tcs.Task;
            }
            catch (Exception ex)
            {
                ErrorHandler.Current.HandleError(ex);
            }
            finally
            {
                monitor.Exit();
            }
        }

        public async Task<IEnumerable<string>> GetSavedLogListAsync()
        {
            var list = await logFolder.GetFilesAsync().AsTask().ConfigureAwait(false);
            var errorLogFileNames = new List<string>();
            foreach (var file in list.Reverse())
            {
                var prop = await file.GetBasicPropertiesAsync();
                if (prop.Size > 0)
                    errorLogFileNames.Add(Path.GetFileNameWithoutExtension(file.Name));
            }
            return errorLogFileNames;
        }

        public async Task DeleteLogAsync(string log)
        {
            var file = await logFolder.GetFileAsync(log + ".log");
            if (file.Name == logFile.Name && writer != null)
            {
                await monitor.EnterAsync();
                try
                {
                    writer.BaseStream.Seek(0, SeekOrigin.Begin);
                    writer.BaseStream.SetLength(0);
                    await writer.FlushAsync();
                }
                finally
                {
                    monitor.Exit();
                }
            }
            else await file.DeleteAsync().AsTask().ConfigureAwait(false);
        }

        public async Task<string> GetSavedLogDataAsync(string log, int maxSize = 12000)
        {
            var file = await logFolder.GetFileAsync(log + ".log");
            if (file.Name == logFile.Name && writer != null)
            {
                return await GetCurrentLogTextData(maxSize);
            }

            using (
                var reader =
                    new StreamReader(
                        (await file.OpenSequentialReadAsync().AsTask().ConfigureAwait(false))
                            .AsStreamForRead()))
            {
                if (maxSize != -1)
                {
                    if (reader.BaseStream.Length > maxSize)
                        reader.BaseStream.Position = reader.BaseStream.Length - maxSize;
                }

                return await reader.ReadToEndAsync().ConfigureAwait(false);
            }
        }

        private async Task<string> GetCurrentLogTextData(int maxSize)
        {
            await monitor.EnterAsync();
            try
            {
                var baseStream = writer.BaseStream;

                long newPos = 0;
                if (maxSize != -1)
                {
                    newPos = baseStream.Position - maxSize;
                    if (newPos < 0) newPos = 0;
                }

                baseStream.Position = newPos;

                using (var textReader = new StreamReader(baseStream, Encoding.UTF8, true, 16*1024, true))
                    return await textReader.ReadToEndAsync().ConfigureAwait(false);
            }
            finally
            {
                monitor.Exit();
            }
        }
    }
}
