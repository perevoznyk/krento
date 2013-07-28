using Laugris.Sage;
using System.IO;

namespace Krento
{
    internal sealed class ToyThread
    {
        private string widgetEntry;

        public ToyThread(string widgetEntry)
        {
            this.widgetEntry = widgetEntry;
        }

        public void Load()
        {
            string LauncherLocation;

            try
            {
                if (!string.IsNullOrEmpty(this.widgetEntry))
                {
                    LauncherLocation = Path.Combine(GlobalConfig.ApplicationFolder, "Laugris.exe");
                    try
                    {
#if PORTABLE
                        FileExecutor.ProcessExecute(LauncherLocation, "-p " + "\"" + widgetEntry + "\"" + " /child", GlobalConfig.ApplicationFolder);
#else
                        FileExecutor.ProcessExecute(LauncherLocation, "\"" + widgetEntry + "\"" + " /child", GlobalConfig.ApplicationFolder);
#endif
                    }
                    catch
                    {
                        //it's not a critical problem
                    }

                }
            }
            catch
            {
            }
        }

    }
}
