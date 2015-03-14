// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using Windows.UI.Xaml.Data;
using RedScooby.Utilities;

namespace RedScooby.Converters
{
    public class StringToSecretConverter : IValueConverter
    {
        private readonly char secretChar = '\ue007';
        public StringToSecretConverter() { }

        public StringToSecretConverter(char secretChar)
        {
            this.secretChar = secretChar;
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var res = (string) value;
            if (!string.IsNullOrEmpty(res))
            {
                var len = res.Length;
                var sb = StringBuilderCache.Acquire();
                foreach (var c in res)
                {
                    sb.Append(secretChar + " ");
                }
                return StringBuilderCache.GetStringAndRelease(sb);
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new InvalidOperationException();
        }
    }
}
