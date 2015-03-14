// Author: Prasanna V. Loganathar
// Created: 10:03 AM 18-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.IO;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;
using RedScooby.Helpers;
using RedScooby.Infrastructure.Framework;

namespace RedScooby.Views.Circles
{
    internal class ThumbnailProviderToSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var provider = value as Func<Task<Stream>>;
            if (provider != null)
            {
                var img = new BitmapImage();
                Task.Run(async () =>
                {
                    var stream = await provider();
                    if (stream != null)
                    {
                        DispatchHelper.Current.Dispatch(() =>
                        {
                            img.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                            using (stream)
                            {
                                using (var rtStream = stream.AsRandomAccessStream())
                                {
                                    img.SetSourceAsync(stream.AsRandomAccessStream())
                                        .AsTask().ContinueWithLogErrorOnException();
                                }
                            }
                        });
                    }
                    else
                    {
                        var placeHolder =
                            await
                                BitmapHelpers.CreateInMemoryImageStream(Color.FromArgb(0xFF, 0x40, 0x40, 0x40), 45, 45);
                        DispatchHelper.Current.Dispatch(() =>
                        {
                            using (placeHolder)
                            {
                                img.SetSourceAsync(placeHolder)
                                    .AsTask().ContinueWithLogErrorOnException();
                            }
                        });
                    }
                });

                return img;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
