using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;

namespace Laugris.Sage
{
    public interface IKrentoMenuItem : INotifyPropertyChanged
    {
        // Properties
        bool Enabled { get; set; }
        Image Image { get; set; }
        string Caption { get; set; }
        string Data { get; set; }
        KrentoMenuTag Tag { get; set; }
        Keys ShortCut { get; set; }
        string Hint { get; set; }
        string Name { get; set; }
    }


}
