//===============================================================================
// Copyright (c) Serhiy Perevoznyk.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.Reflection;
using Laugris.Sage;

namespace Krento
{
    /// <summary>
    /// Internal helper class
    /// </summary>
    [Serializable]
    internal static class InternalHelper
    {

        internal static Assembly FindLoadedAssembly(string assemblyRef)
        {
            if (string.IsNullOrEmpty(assemblyRef))
                return null;

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (TextHelper.SameText(assemblyRef, assembly.FullName))
                {
                    return assembly;
                }

                AssemblyName refName = new AssemblyName(assemblyRef);
                AssemblyName curName = assembly.GetName();

                if ((refName.Name == curName.Name) && (refName.GetPublicKeyToken().ToString() == curName.GetPublicKeyToken().ToString()))
                    
                    //(refName.Version.Major == curName.Version.Major) &&
                    //(refName.Version.MajorRevision == curName.Version.MajorRevision))
                {
                    return assembly;
                }

            }
            return null;
        }

    }
}
