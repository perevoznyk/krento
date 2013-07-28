using System;
using System.Collections.Generic;
using System.Text;

namespace Krento
{
    /// <summary>
    /// Communication interface for Krento packages (additional modules, loaded into main domain).
    /// Every Krento Plugin must implement this interface in order to be loaded by Krento.
    /// Krento plugin is a dll assembly file that contains package. 
    /// A package is a class that implements the IPackage interface which defines a <see cref="Load"/> and <see cref="Unload"/> methods. 
    /// An IServiceProvider is passed during loading and gives access to a set of services which are part of 
    /// the Krento object model. 
    /// </summary>
    /// <example>Complete Krento Plugin
    /// <code>
    /// using System;
    /// using System.Collections.Generic;
    /// using System.Text;
    /// using Krento;
    /// using Laugris.Sage;
    /// using System.Windows.Forms;
    /// using System.Drawing;
    ///
    /// namespace TestPlugin
    ///  {
    ///    public class HelloPlugin : IPackage
    ///    {
    ///        #region IPackage Members
    ///
    ///        public void Load(IServiceProvider serviceProvider)
    ///        {
    ///            IMenuManager menuManager;
    ///            IKrentoMenuItem item;
    ///
    ///            if (serviceProvider != null)
    ///            {
    ///                menuManager = (IMenuManager)serviceProvider.GetService(typeof(IMenuManager));
    ///                if (menuManager != null)
    ///                {
    ///                    item = menuManager.GetManagerMenu().AddMenuItem(ClickHandler);
    ///                    item.Caption = SR.GetCaption("HelloWorld");
    ///                    if (string.IsNullOrEmpty(item.Caption))
    ///                        item.Caption = "Not translated";
    ///                    item.ShortCut = Keys.Alt | Keys.Z;
    ///
    ///                    menuManager.GetTrayMenu()[0].Image = new Bitmap(@"info.png");
    ///                    menuManager.GetTrayMenu()[1].Image = new Bitmap(@"help.png");
    ///                }
    ///            }
    ///        }
    ///
    ///        public void ClickHandler(object sender, EventArgs e)
    ///        {
    ///            MessageBox.Show("Hello, world!");
    ///        }
    ///
    ///        public void Unload()
    ///        {
    ///            //Do something here
    ///        }
    ///
    ///        #endregion
    ///    }
    ///}
    /// </code>
    /// </example>
    public interface IPackage
    {
        /// <summary>
        /// This method is called by Krento after plugin is loaded.
        /// </summary>
        /// <param name="serviceProvider">The service provider, an object that provides custom support to other objects</param>
        void Load(IServiceProvider serviceProvider);
        /// <summary>
        /// This method is called by Krento before unloading the plugin. You can perform
        /// some specific actions (like saving your plugin settings at this moment).
        /// </summary>
        void Unload();
    }
}
