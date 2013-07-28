using System;
using System.Collections.Generic;
using System.Text;

namespace Laugris.Sage
{
    public class IniFileAccessArgs : EventArgs
    {
        private MemIniFile iniFile;

        public MemIniFile IniFile
        {
            get { return this.iniFile; }
        }

        public IniFileAccessArgs(MemIniFile iniFile)
        {
            this.iniFile = iniFile;
        }
    }
}
