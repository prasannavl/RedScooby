// Author: Prasanna V. Loganathar
// Created: 7:31 PM 15-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace RedScooby.Logging.FormatProvider
{
    public class DumpFormatProvider : IFormatProvider, ICustomFormatter, IDisposable
    {
        public const int StringBuilderMaxCacheSize = 360;
        [ThreadStatic] private static StringBuilder _cachedStringBuilder;
        [ThreadStatic] private static StringWriter _cachedStringWriter;

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            if (arg == null)
                return "null";

            var type = arg.GetType();
            if (type == typeof (string))
            {
                return (string) arg;
            }

            if (_cachedStringWriter == null)
            {
                _cachedStringWriter =
                    new StringWriter(_cachedStringBuilder ?? (_cachedStringBuilder = new StringBuilder()));
            }

            try
            {
                Dump(_cachedStringWriter, arg);
                return _cachedStringWriter.ToString();
            }
            catch (Exception ex)
            {
                return " { Logger: Exception => " + ex.Message + " } { Partial Message: " +
                       _cachedStringWriter.ToString() + " }";
            }
            finally
            {
                if (_cachedStringBuilder.Capacity > StringBuilderMaxCacheSize)
                {
                    _cachedStringBuilder.Clear();
                }
                else
                {
                    _cachedStringBuilder.Length = 0;
                }
            }
        }

        // Disposes only the current thread's resources. 
        // The others are picked up by the GC on next run. This can also be harmlessly called 
        // on each thread to dispose each thread's resources deterministically.
        public void Dispose()
        {
            if (_cachedStringWriter != null)
            {
                _cachedStringWriter.Dispose();
                _cachedStringBuilder = null;
            }
        }

        public object GetFormat(Type formatType)
        {
            return formatType == typeof (ICustomFormatter) ? this : null;
        }

        [DebuggerStepThrough]
        public static void Dump(StringWriter writer, object o, string prefix = null, int depth = 5,
            int errorTolerance = 3)
        {
            try
            {
                if (o == null)
                {
                    if (prefix != null) prefix = prefix + ": ";
                    else prefix = string.Empty;
                    writer.Write(prefix + "null");
                    return;
                }

                if (errorTolerance < 0)
                {
                    writer.Write(" ||=> Aborted due to too many read errors.");
                    return;
                }

                var t = o.GetType();
                var tInfo = t.GetTypeInfo();

                writer.Write((prefix ?? t.Name) + ": ");

                if (depth-- < 1 || t == typeof (string) || tInfo.IsValueType)
                {
                    if (tInfo.IsValueType)
                    {
                        if (t == typeof (int) || t == typeof (bool) ||
                            t == typeof (DateTime) || t == typeof (DateTimeOffset) || t == typeof (Guid) ||
                            t == typeof (double) || t == typeof (long) || t == typeof (float) || t == typeof (uint) ||
                            t == typeof (ulong) || t == typeof (decimal) || t == typeof (ushort) ||
                            tInfo.BaseType == typeof (Enum) || t == typeof (byte) ||
                            t == typeof (short) || t == typeof (IntPtr) || t == typeof (UIntPtr))
                        {
                            writer.Write(o.ToString());
                            return;
                        }
                    }
                    else
                    {
                        writer.Write(o.ToString());
                        return;
                    }
                }

                var enumerable = o as IEnumerable;
                if (enumerable != null)
                {
                    writer.Write(" [ ");
                    var addSepFlag1 = false;
                    foreach (var item in enumerable)
                    {
                        if (addSepFlag1)
                        {
                            writer.Write(", ");
                        }
                        else
                        {
                            addSepFlag1 = true;
                        }
                        Dump(writer, item, null, depth);
                    }
                    writer.Write(" ] ");
                    return;
                }


                var flag2 = false;

                foreach (var prop in t.GetRuntimeProperties())
                {
                    if (flag2)
                    {
                        writer.Write(", ");
                    }
                    else
                    {
                        writer.Write("{ ");
                        flag2 = true;
                    }
                    object value;
                    try
                    {
                        value = prop.GetValue(o);
                    }
                    catch
                    {
                        value = "{Unreadable}";
                        errorTolerance--;
                    }
                    Dump(writer, value, prop.Name, depth, errorTolerance);

                    if (errorTolerance < 0)
                        break;
                }

                writer.Write(flag2 ? " }" : o.ToString());
            }
            catch
            {
                if (writer != null)
                    writer.Write(" { FormatError: " + o.GetType().Name + "} ");

                // <= 'o' Potential errors from this block are not caught intentionally.
                // 1. o could be null
                // 2. Write could throw.
            }
        }
    }
}
