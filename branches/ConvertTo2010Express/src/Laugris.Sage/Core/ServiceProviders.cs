using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Laugris.Sage
{
    [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
    public class ServiceProviders : IServiceProvider
    {
        private Dictionary<Type, object> _objDict = new Dictionary<Type, object>();

        public void AddService(Type serviceType, object service)
        {
            if (serviceType == null)
            {
                return;
            }
            if (service == null)
            {
                return;
            }
            if (!this._objDict.ContainsKey(serviceType))
            {
                this._objDict.Add(serviceType, service);
            }
            else if (this._objDict[serviceType] != service)
            {
                return;
            }
        }

        public object GetService(Type serviceType)
        {
            if (this._objDict.ContainsKey(serviceType))
            {
                return this._objDict[serviceType];
            }
            return null;
        }
    }
}
