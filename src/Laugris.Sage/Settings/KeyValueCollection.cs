//===============================================================================
// Copyright © Serhiy Perevoznyk.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Collections;

namespace Laugris.Sage
{
    [Serializable]
    public class KeyValueCollection : NameObjectCollectionBase
    {
        public KeyValueCollection()
        {
        }

        // Gets a key-and-value pair (DictionaryEntry) using an index.
        public DictionaryEntry Entry(int index)
        {
            return (new DictionaryEntry(
                this.BaseGetKey(index), this.BaseGet(index)));
        }

        // Gets a value  using an index.
        public string this[int index]
        {
            get
            {
                return (string)(this.BaseGet(index));
            }
        }

        // Gets or sets the value associated with the specified key.
        public string this[string key]
        {
            get
            {
                return (string)(this.BaseGet(key));
            }
            set
            {
                this.BaseSet(key, value);
            }
        }

        public string Value(int index)
        {
            return this[index];
        }

        public string Value(string key)
        {
            return this[key];
        }

        //
        // Summary:
        //     Gets the key of the entry at the specified index of the collection
        //     instance.
        //
        // Parameters:
        //   index:
        //     The zero-based index of the key to get.
        //
        // Returns:
        //     A System.String that represents the key of the entry at the specified index.
        //
        // Exceptions:
        //   System.ArgumentOutOfRangeException:
        //     index is outside the valid range of indexes for the collection.
        public string Key(int index)
        {
            return BaseGetKey(index);
        }

        public bool HasKey(string key)
        {
            for (int i = 0; i < Count; i++)
            {
                if (TextHelper.SameText(key, BaseGetKey(i)))
                    return true;
            }
            return false;
        }

        public string Key(string value)
        {

            string key = null;

            for (int i = 0; i < Count; i++)
            {
                string s = (string)(BaseGet(i));
                if (TextHelper.SameText(s, value))
                {
                    key = BaseGetKey(i);
                    break;
                }
            }

            return key;
        }

        /// <summary>
        /// Gets a String array that contains all the keys in the collection.
        /// </summary>
        /// <value>All keys.</value>
        public string[] AllKeys()
        {
            return (this.BaseGetAllKeys());
        }


        /// <summary>
        /// Gets an Object array that contains all the values in the collection.
        /// </summary>
        /// <value>All values.</value>
        public string[] AllValues()
        {
            return ((String[])this.BaseGetAllValues(typeof(string)));
        }



        /// <summary>
        /// Gets a value indicating if the collection contains keys that are not null.
        /// </summary>
        /// <value><c>true</c> if this instance has keys; otherwise, <c>false</c>.</value>
        public bool HasKeys
        {
            get
            {
                return (this.BaseHasKeys());
            }
        }

        // Adds an entry to the collection.
        public void Add(string key, string value)
        {
            this.BaseAdd(key, value);
        }

        // Removes an entry with the specified key from the collection.
        public void Remove(string key)
        {
            this.BaseRemove(key);
        }

        /// <summary>
        /// Removes an entry in the specified index from the collection.
        /// </summary>
        /// <param name="index">The zero-based index of the key</param>
        public void Remove(int index)
        {
            this.BaseRemoveAt(index);
        }

        /// <summary>
        /// Clears all the elements in the collection.
        /// </summary>
        public void Clear()
        {
            this.BaseClear();
        }


    }
}
