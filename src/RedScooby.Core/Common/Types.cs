// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;

namespace RedScooby.Common
{
    public struct TupleStruct<T1, T2>
    {
        public T1 Item1;
        public T2 Item2;

        public TupleStruct(T1 item1, T2 item2)
        {
            Item1 = item1;
            Item2 = item2;
        }
    }

    public struct AsyncResult<T>
    {
        public T Result;
        public Exception Error;

        public AsyncResult(T result)
        {
            Result = result;
            Error = null;
        }

        public AsyncResult(Exception ex, T partialResult)
        {
            if (ex == null)
                throw new ArgumentNullException("ex");

            Result = partialResult;
            Error = ex;
        }

        public bool Success
        {
            get { return Error == null; }
        }
    }

    public enum ActivityState
    {
        Active,
        Inactive,
        InProgress,
    }
}
