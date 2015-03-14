// Author: Prasanna V. Loganathar
// Created: 7:42 PM 22-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI;

namespace RedScooby.Helpers
{
    public static class BitmapHelpers
    {
        public static async Task<InMemoryRandomAccessStream> CreateInMemoryImageStream(Color fillColor,
            int heightInPixel,
            int widthInPixel)
        {
            var stream = new InMemoryRandomAccessStream();
            var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);

            var bytes = new List<byte>(heightInPixel*widthInPixel*4);
            for (var x = 0; x < widthInPixel; x++)
            {
                for (var y = 0; y < heightInPixel; y++)
                {
                    bytes.Add(fillColor.R);
                    bytes.Add(fillColor.G);
                    bytes.Add(fillColor.B);
                    bytes.Add(fillColor.A);
                }
            }

            encoder.SetPixelData(BitmapPixelFormat.Rgba8, BitmapAlphaMode.Ignore, (uint) widthInPixel,
                (uint) heightInPixel, 96, 96, bytes.ToArray());
            await encoder.FlushAsync();
            return stream;
        }
    }
}
