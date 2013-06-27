using System;
using System.Collections.Generic;
using System.Text;
using Laugris.Sage;

namespace Krento.RollingStones
{
    public class HistoryList : List<HistoryEntry>
    {
        public HistoryEntry Add(string fileName)
        {
            HistoryEntry entry = new HistoryEntry(fileName);
            Add(entry);
            return entry;
        }

        public HistoryEntry this[string fileName]
        {
            get
            {
                int idx = IndexOf(fileName);
                if (idx == -1)
                    return null;
                else
                    return this[idx];
            }
        }

        public int IndexOf(string value)
        {
            for (int i = 0; i < Count; i++)
            {
                if (TextHelper.SameText(value, this[i].FileName))
                    return i;
            }
            return -1;
        }

        public void Remove(string fileName)
        {
            int idx = IndexOf(fileName);
            if (idx > -1)
            {
                HistoryEntry entry = this[idx];
                Remove(entry);
                entry.Dispose();
            }
        }
    }
}
