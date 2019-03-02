﻿#region License information
/*
    Seeing# and all applications distributed together with it. 
	Exceptions are projects where it is noted otherwise.
    More info at 
     - https://github.com/RolandKoenig/SeeingSharp2 (sourcecode)
     - http://www.rolandk.de (the authors homepage, german)
    Copyright (C) 2019 Roland König (RolandK)
    
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published
    by the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
    
    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with this program.  If not, see http://www.gnu.org/licenses/.
*/
#endregion
namespace SeeingSharp.Util
{
    #region using

    using System;
    using Checking;

    #endregion

    /// <summary>
    /// This is a simple RingBuffer implementation for Seeing#,
    /// originally created for the Input event system.
    /// 
    /// More info on cyclic buffers: https://en.wikipedia.org/wiki/Circular_buffer
    /// </summary>
    public class RingBuffer<T>
    {
        #region all buffer properties
        private T[] m_buffer;
        private int m_itemStart;
        private int m_itemLength;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="RingBuffer{T}"/> class.
        /// </summary>
        /// <param name="maxItemCount">The maximum count of items.</param>
        public RingBuffer(int maxItemCount)
        {
            maxItemCount.EnsureInRange(2, Int32.MaxValue, nameof(maxItemCount));

            m_buffer = new T[maxItemCount];
            Count = maxItemCount;
            m_itemStart = 0;
            m_itemLength = 0;
        }

        /// <summary>
        /// Adds a new item to the buffer and overrides existing items
        /// if the count of items reached the maximum.
        /// </summary>
        /// <param name="newItem">The new item to be added.</param>
        public void AddWithOverride(T newItem)
        {
            if(m_itemLength < Count)
            {
                m_buffer[(m_itemStart + m_itemLength) % Count] = newItem;
                m_itemLength++;
            }
            else
            {
                m_itemStart = (m_itemStart + 1) % Count;

                int nextIndex = m_itemStart - 1;
                if(nextIndex < 0) { nextIndex = Count - 1; }
                m_buffer[nextIndex] = newItem;
            }
        }

        /// <summary>
        /// Adds a new item to the buffer and throws an exception
        /// if the count of items reached the maximum.
        /// </summary>
        /// <param name="newItem">The new item to be added.</param>
        public void AddWithException(T newItem)
        {
            if (m_itemLength < Count)
            {
                m_buffer[(m_itemStart + m_itemLength) % Count] = newItem;
                m_itemLength++;
            }
            else
            {
                throw new SeeingSharpException("The RingBuffer reached the maximum count of items (" + Count + ")!");
            }
        }

        /// <summary>
        /// Removes all items from the buffer and pushes them to the given observable collection.
        /// </summary>
        /// <param name="targetObservable">The target observable.</param>
        public void RemoveAndPushItemsTo(CustomObservable<T> targetObservable)
        {
            while(m_itemLength > 0)
            {
                targetObservable.PushNext(m_buffer[m_itemStart]);
                m_buffer[m_itemStart] = default(T);

                m_itemStart = (m_itemStart + 1) % Count;
                m_itemLength--;
            }
            m_itemLength--;
        }

        /// <summary>
        /// Clears this collection.
        /// </summary>
        public void Clear()
        {
            for(int loop=0; loop<Count; loop++)
            {
                m_buffer[loop] = default(T);
            }

            m_itemLength = 0;
            m_itemStart = 0;
        }

        /// <summary>
        /// Gets the object at the specified index.
        /// </summary>
        public T this[int index]
        {
            get
            {
                if (index >= m_itemLength) { throw new IndexOutOfRangeException(); }
                return m_buffer[(m_itemStart + index) % Count];
            }
        }

        /// <summary>
        /// Gets the total count of items.
        /// </summary>
        public int Count { get; }
    }
}