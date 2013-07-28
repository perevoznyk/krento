using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Laugris.Sage.Launcher
{
    public sealed class Catalog : IDisposable
    {
        private readonly List<CatalogItem> items;
        private readonly List<string> locations;
        private readonly Dictionary<string, CatalogItem> history;

        [NonSerialized]
        private object syncRoot;

        public Catalog()
        {
            items = new List<CatalogItem>();
            history = new Dictionary<string, CatalogItem>();
            locations = new List<string>();
            locations.Add(NativeMethods.GetFolderPath(CSIDL.COMMON_STARTMENU));
            locations.Add(NativeMethods.GetFolderPath(CSIDL.STARTMENU));
        }

        ~Catalog()
        {
            Dispose(false);
        }

        public int Limit { get; set; }


        public object SyncRoot
        {
            get
            {
                if (syncRoot == null)
                {
                    Interlocked.CompareExchange(ref syncRoot, new object(), null);
                }
                return syncRoot;
            }
        }

        public List<CatalogItem> Items
        {
            get { return items; }
        }

        public List<string> Locations
        {
            get { return locations; }
        }

        public Dictionary<string, CatalogItem> History
        {
            get { return history; }
        }

        private void ScanLocation(string location)
        {
            List<string> files = new List<string>();
            FileOperations.GetAllFiles(files, location, "*.lnk");

            //string[] files2 = Directory.GetFiles(location, "*.lnk", SearchOption.AllDirectories);

            foreach (string file in files)
            {
                CatalogItem item = new CatalogItem(file);
                item.Description = file.Substring(location.Length + 1, file.Length - location.Length - 5);

                lock (SyncRoot)
                {
                    items.Add(item);
                }
            }

            files.Clear();
        }

        public void Rebuild()
        {
            DisposeItems();
            for (int i = 0; i < locations.Count; i++)
            {
                ScanLocation(FileOperations.StripFileName(locations[i]));
            }
        }

        private static bool Matches(CatalogItem item, string searchText)
        {
            return NativeMethods.Matches(item.ShortName, searchText);
            
            //int size = item.LowName.Length;
            //int txtSize = searchText.Length;
            //int curChar = 0;

            //for (int i = 0; i < size; i++)
            //{
                
            //    if (item.LowName[i] == searchText[curChar])
            //    {
            //        curChar++;
            //        if (curChar >= txtSize)
            //        {
            //            return true;
            //        }
            //    }
            //}

            //return false;
        }

        private List<CatalogItem> Search(string searchText)
        {
            List<CatalogItem> result = new List<CatalogItem>();
            if (string.IsNullOrEmpty(searchText))
                return result;

            string lowSearch = searchText.ToLower();
            lock (SyncRoot)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    if (Matches(items[i], lowSearch))
                        result.Add(items[i]);
                }
            }
            return result;
        }


        public void Search(string searchText, List<CatalogItem> result)
        {
            List<CatalogItem> ret = Search(searchText);
            ret.Sort(new CatalogItemsComparer(searchText));
            int max = Limit > 0 ? Limit : ret.Count;
            if (max > ret.Count)
                max = ret.Count;
            for (int i = 0; i < max; i++)
                result.Add(ret[i]);

            if (history.ContainsKey(searchText))
            {
                CatalogItem item = history[searchText];
                result.Remove(item);
                result.Insert(0, item);
            }
            ret.Clear();
        }

        private void DisposeItems()
        {
            lock (SyncRoot)
            {
                for (int i = 0; i < items.Count; i++)
                    items[i].Dispose();
                items.Clear();
            }
        }

        #region IDisposable Members

        private void Dispose(bool disposing)
        {
            DisposeItems();
            locations.Clear();
            history.Clear();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
