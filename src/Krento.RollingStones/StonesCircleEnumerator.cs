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

namespace Krento.RollingStones
{
    public class StonesCircleEnumerator : ICircleEnumerator
    {
        private StonesManager manager;
        private int current = -1;

        internal StonesCircleEnumerator(StonesManager manager)
        {
            this.manager = manager;
        }

        #region ICircleEnumerator Members

        public bool JumpTo(int stoneIndex)
        {
            if (manager == null)
                return false;
            if (stoneIndex >= manager.Stones.Count)
                return false;
            if (stoneIndex < 0)
                return false;
            current = stoneIndex;
            return true;
        }

        public bool MoveNext()
        {
            if (manager == null)
                return false;
            if (manager.Stones.Count == 0)
                return false;
            current++;
            if (current == manager.Stones.Count)
                current = 0;
            return true;
        }

        public bool MovePrevious()
        {
            if (manager == null)
                return false;

            if (manager.Stones.Count == 0)
                return false;
            current--;
            if (current == -1)
                current = manager.Stones.Count - 1;
            return true;
        }

        public object Current
        {
            get 
            {
                if (current == -1)
                    return null;
                else
                {
                    if (manager == null)
                        return null;

                    return manager.Stones[current];
                }
            }
        }

        public void Reset()
        {
            current = -1;
        }

        #endregion

    }
}
