using System;
using System.Threading;
using System.Xml;
using Laugris.Sage;
using Krento.Properties;
using System.Reflection;
using System.Windows.Forms;

namespace Krento
{
    internal class VersionChecker
    {
        private string url;

        public string Url
        {
            get
            {
                if (!string.IsNullOrEmpty(url))
                    return url;
                else
                    return string.Empty;
            }
        }
        private string version;

        public string Version
        {
            get
            {
                if (!string.IsNullOrEmpty(version))
                    return version;
                else
                    return string.Empty;
            }
        }


        private void ExecuteAsync()
        {
            Version clientVersion;
            try
            {
                AssemblyName krentoName = new AssemblyName(Assembly.GetEntryAssembly().FullName);
                if (krentoName != null)
                {
                    clientVersion = krentoName.Version;
                }
                else
                {
                    clientVersion = new Version(Application.ProductVersion);
                }
            }
            catch
            {
                clientVersion = new Version("3.0");
                TraceDebug.Trace("Error in getting the Krento assembly version. By default assume 3.0");
            }
            Version serverVersion = null;

#if DEBUG
            string configFile = "http://localhost:8080";
#else
            string configFile = Settings.Default.Website;
#endif
            if (!configFile.EndsWith("/", StringComparison.OrdinalIgnoreCase))
                configFile = configFile + "/";
            configFile = configFile + "app_version.xml";

            XmlTextReader reader = null;
            try
            {
                try
                {
                    // provide the XmlTextReader with the URL of  
                    // our xml document  
                    string xmlURL = configFile;
                    reader = new XmlTextReader(xmlURL);
                    // simply (and easily) skip the junk at the beginning  
                    reader.MoveToContent();
                    // internal - as the XmlTextReader moves only  
                    // forward, we save current xml element name  
                    // in elementName variable. When we parse a  
                    // text node, we refer to elementName to check  
                    // what was the node name  
                    string elementName = "";
                    // we check if the xml starts with a proper  
                    // "ourfancyapp" element node  
                    if ((reader.NodeType == XmlNodeType.Element) &&
                        (reader.Name == "krento"))
                    {
                        while (reader.Read())
                        {
                            // when we find an element node,  
                            // we remember its name  
                            if (reader.NodeType == XmlNodeType.Element)
                                elementName = reader.Name;
                            else
                            {
                                // for text nodes...  
                                if ((reader.NodeType == XmlNodeType.Text) &&
                                    (reader.HasValue))
                                {
                                    // we check what the name of the node was  
                                    switch (elementName)
                                    {
                                        case "version":
                                            // thats why we keep the version info  
                                            // in xxx.xxx.xxx.xxx format  
                                            // the Version class does the  
                                            // parsing for us  
                                            serverVersion = new Version(reader.Value);
                                            break;
                                        case "url":
                                            url = reader.Value;
                                            break;
                                    }
                                }
                            }
                        }
                    }
                }

                finally
                {
                    if (reader != null)
                        reader.Close();
                }

            }
            catch
            {
                serverVersion = null;
            }

            if (serverVersion != null)
            {
                version = serverVersion.ToString();
                if (clientVersion < serverVersion)
                {
                    NativeMethods.PostMessage(InteropHelper.MainWindow, NativeMethods.CM_CHECKUPDATE, IntPtr.Zero, IntPtr.Zero);
                }
            }
        }

        public void CheckNewVersion()
        {
            if (InteropHelper.IsConnectedToInternet)
            {
                Thread t = new Thread(new ThreadStart(ExecuteAsync));
                t.Start();
                Thread.Sleep(0);
            }
        }
    }
}
