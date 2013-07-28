using System;
using System.Collections.Generic;
using System.Text;
using Laugris.Sage;

namespace Krento
{
    /// <summary>
    /// Defines methods for managing Krento menus
    /// </summary>
    public interface IMenuManager
    {
        /// <summary>
        /// Gets one of the Krento's menus by name. The possible values for menu's name are:
        /// <list type="bullet">
        /// <item>Pulsar</item>
        /// <item>Tray</item>
        /// <item>Manager</item>
        /// </list>
        /// </summary>
        /// <param name="name">The name of the menu.</param>
        /// <returns>Interface to one of Krento menus or null if the specified menu name is wrong</returns>
        IKrentoMenu GetMenu(string name);
        /// <summary>
        /// Gets the Pulsar menu interface.
        /// </summary>
        /// <returns>Interface for Krento Pulsar menu</returns>
        IKrentoMenu GetPulsarMenu();
        /// <summary>
        /// Gets the Manager menu interface.
        /// </summary>
        /// <returns>Interface for Krento Stones Manager menu</returns>
        IKrentoMenu GetManagerMenu();
        /// <summary>
        /// Gets the tray icon menu interface.
        /// </summary>
        /// <returns>Interface for Krento tray icon popup menu</returns>
        IKrentoMenu GetTrayMenu();
    }
}
