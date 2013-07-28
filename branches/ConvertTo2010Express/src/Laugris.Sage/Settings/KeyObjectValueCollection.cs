using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;

namespace Laugris.Sage
{
    [Serializable]
    public class KeyObjectValueCollection : NameObjectCollectionBase, IDisposable
    {

        public KeyObjectValueCollection()
        {
        }

        ~KeyObjectValueCollection()
        {
            Dispose(false);
        }

        // Gets a value  using an index.
        public KeyValueCollection this[int index]
        {
            get
            {
                return (KeyValueCollection)(this.BaseGet(index));
            }
        }

        // Gets or sets the value associated with the specified key.
        public KeyValueCollection this[string key]
        {
            get
            {
                return (KeyValueCollection)(this.BaseGet(key));
            }
            set
            {
                this.BaseSet(key, value);
            }
        }

        public KeyValueCollection Value(int index)
        {
            return this[index];
        }

        public KeyValueCollection Value(string key)
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


        /// <summary>
        /// Gets a String array that contains all the keys in the collection.
        /// </summary>
        /// <value>All keys.</value>
        public string[] AllKeys()
        {
            return (this.BaseGetAllKeys());
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

        public bool HasKey(string key)
        {
            return (this.Value(key) != null);
        }

        // Adds an entry to the collection.
        public void Add(string key, KeyValueCollection value)
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
            for (int i = 0; i < this.Count; i++)
            {
                this[i].Clear();
            }
            this.BaseClear();
        }


        #region IDisposable Members

        protected virtual void Dispose(bool disposing)
        {
            Clear();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        public KeyValueCollection Add(string key)
        {
            KeyValueCollection items = new KeyValueCollection();
            this.Add(key, items);
            return items;
        }
    }
}
