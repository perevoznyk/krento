using System;
using System.Collections.Generic;
using System.Text;

namespace Laugris.Sage.Launcher
{
    internal class CatalogItemsComparer : IComparer<CatalogItem>
    {
        private string searchText;

        public CatalogItemsComparer(string searchText)
        {
            this.searchText = searchText;
        }

        #region IComparer<CatalogItem> Members

        public int Compare(CatalogItem x, CatalogItem y)
        {
            bool localEqual =  TextHelper.SameText(x.ShortName,  searchText);
            bool otherEqual =  TextHelper.SameText(y.ShortName,  searchText);

            if (localEqual && !otherEqual)
                return -1;
            if (!localEqual && otherEqual)
                return 1;


            if (x.Usage > y.Usage)
                return -1;
            if (x.Usage < y.Usage)
                return 1;



            int localFind = x.ShortName.IndexOf(searchText, StringComparison.OrdinalIgnoreCase);
            int otherFind = y.ShortName.IndexOf(searchText, StringComparison.OrdinalIgnoreCase);

            if (localFind != -1 && otherFind == -1)
                return -1;
            else if (localFind == -1 && otherFind != -1)
                return 1;

            if (localFind != -1 && otherFind != -1)
            {
                if (localFind < otherFind)
                    return -1;
                else if (otherFind < localFind)
                    return 1;
            }

            int localLen = x.ShortName.Length;
            int otherLen = y.ShortName.Length;

            if (localLen < otherLen)
                return -1;
            if (localLen > otherLen)
                return 1;


            // Absolute tiebreaker to prevent loops
            if (x.FullPath.Length < y.FullPath.Length)
                return -1;
            return 1;
        }

        #endregion
    }
}
