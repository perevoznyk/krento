using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Laugris.Sage
{
    public sealed class InvokeObjectHandler
    {
        public WaitCallback Entry { get; set; }
        public object Parameter { get; set; }
        public string ErrorMessage { get; set; }

        public InvokeObjectHandler(WaitCallback entry)
        {
            this.Entry = entry;
        }

        public InvokeObjectHandler(WaitCallback entry, string errorMessage)
        {
            this.Entry = entry;
            this.ErrorMessage = errorMessage;
        }

        public InvokeObjectHandler(WaitCallback entry, object parameter)
        {
            this.Entry = entry;
            this.Parameter = parameter;
        }

        public InvokeObjectHandler(WaitCallback entry, object parameter, string errorMessage)
        {
            this.Entry = entry;
            this.Parameter = parameter;
            this.ErrorMessage = errorMessage;
        }

    }
}
