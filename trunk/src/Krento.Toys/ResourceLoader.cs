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
using System.Drawing;
using System.Reflection;
using Karna.Windows;

/// <summary>
/// This class helps to load embedded resources (images) by image name
/// </summary>
internal sealed class ResourceLoader
{
    private static string assemblyName;

    static ResourceLoader()
    {
        assemblyName = Assembly.GetAssembly(typeof(ResourceLoader)).GetName().Name;
    }

    /// <summary>
    /// Loads the image from the specified resource name.
    /// </summary>
    /// <param name="resourceName">Name of the resource.</param>
    /// <returns>Image</returns>
    public static Image Load(string resourceName)
    {
        string fullResourceName;

        fullResourceName = assemblyName + ".Images." + resourceName;

        return FastBitmap.FromResource(typeof(ResourceLoader), fullResourceName);
        //old way
        //return new Bitmap(typeof(ResourceLoader), fullResourceName);
    }
}
