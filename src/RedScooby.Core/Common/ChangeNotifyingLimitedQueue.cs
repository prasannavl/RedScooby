// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace RedScooby.Common
{
    public class ChangeNotifyingLimitedQueueBase<T> : IEnumerable<T>, ICollection, INotifyPropertyChanged,
        INotifyCollectionChanged, IList
    {
        public const string CountString = "Count";
        private readonly Queue<T> queue;
        private readonly object syncRoot = new object();
        private int capacity;

        public ChangeNotifyingLimitedQueueBase(int capacity = 16)
        {
            if (capacity < 1) throw new ArgumentOutOfRangeException("capacity", "capacity must be greater than 0");
            queue = new Queue<T>(capacity);
            this.capacity = capacity;
        }

        public ChangeNotifyingLimitedQueueBase(Queue<T> existingQueue, int capacity)
        {
            if (existingQueue == null) throw new ArgumentNullException("existingQueue");
            if (capacity < 1) throw new ArgumentOutOfRangeException("capacity", "capacity must be greater than 0");

            this.capacity = capacity;
            queue = new Queue<T>(existingQueue);
            TrimExcess();
        }

        public int Capacity
        {
            get { return capacity; }
            set
            {
                if (capacity < 0) throw new ArgumentOutOfRangeException("value");
                lock (SyncRoot) ChangeCapacity(value);
            }
        }

        public void CopyTo(Array array, int index)
        {
            ((ICollection) queue).CopyTo(array, index);
        }

        public int Count
        {
            get { return queue.Count; }
        }

        public bool IsSynchronized
        {
            get { return true; }
        }

        public object SyncRoot
        {
            get { return syncRoot; }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return queue.GetEnumerator();
        }

        public object this[int index]
        {
            get { return queue.ElementAt(index); }
            set { throw new InvalidOperationException(); }
        }

        public int Add(object value)
        {
            throw new InvalidOperationException();
        }

        public void Clear()
        {
            lock (SyncRoot)
            {
                queue.Clear();
            }
            HandleCollectionChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public bool Contains(object value)
        {
            return false;
        }

        public int IndexOf(object value)
        {
            throw new InvalidOperationException();
        }

        public void Insert(int index, object value)
        {
            throw new InvalidOperationException();
        }

        public void Remove(object value)
        {
            throw new InvalidOperationException();
        }

        public void RemoveAt(int index)
        {
            throw new InvalidOperationException();
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void HandleCollectionChange(NotifyCollectionChangedEventArgs args)
        {
            HandlePropertyChanges();
            var handler = CollectionChanged;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        public virtual void HandlePropertyChanges()
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(CountString));
            }
        }

        public bool Contains(T item)
        {
            // OK to not lock due to error tolerance.
            return queue.Contains(item);
        }

        public T Dequeue()
        {
            T res;
            lock (SyncRoot)
            {
                res = queue.Dequeue();
                HandleCollectionChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, res, 0));
            }

            return res;
        }

        public void Enqueue(T item)
        {
            lock (SyncRoot)
            {
                if (capacity <= queue.Count)
                {
                    Dequeue();
                }
                queue.Enqueue(item);

                HandleCollectionChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item,
                    queue.Count - 1));
            }
        }

        protected NotifyCollectionChangedEventHandler GetCollectionChangedEventHandlerDelegate()
        {
            return CollectionChanged;
        }

        protected PropertyChangedEventHandler GetPropertyChangedHandlerDelegate()
        {
            return PropertyChanged;
        }

        private void ChangeCapacity(int value)
        {
            if (value < 0) throw new ArgumentOutOfRangeException("value");

            capacity = value;
            TrimExcess();
        }

        private void TrimExcess()
        {
            while (queue.Count > capacity)
            {
                queue.Dequeue();
            }
        }
    }

    public sealed class ChangeNotifyingLimitedQueue<T> : ChangeNotifyingLimitedQueueBase<T>
    {
        public ChangeNotifyingLimitedQueue(int capacity = 16) : base(capacity) { }
        public ChangeNotifyingLimitedQueue(Queue<T> existingQueue, int capacity) : base(existingQueue, capacity) { }
    }
}
