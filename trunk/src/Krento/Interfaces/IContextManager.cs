using System;
using System.Collections.Generic;
using System.Text;
using Laugris.Sage;
using Krento.RollingStones;

namespace Krento
{
    /// <summary>
    /// Defines methods to access the core components of Krento
    /// </summary>
    public interface IContextManager
    {
        /// <summary>
        /// Gets the pulsar instance.
        /// </summary>
        /// <returns>The Pulsar instance</returns>
        Pulsar GetPulsarInstance();
        /// <summary>
        /// Gets the stones manager instance.
        /// </summary>
        /// <returns>The Krento Stones Manager instance</returns>
        StonesManager GetStonesManagerInstance();
        /// <summary>
        /// Hides the manager.
        /// </summary>
        void HideManager();
        /// <summary>
        /// Shows the manager.
        /// </summary>
        void ShowManager();
        /// <summary>
        /// Changes the visibility of the stones manager (shows or hides it depends of the current state).
        /// </summary>
        void ChangeVisibility();
        /// <summary>
        /// Hides the pulsar.
        /// </summary>
        void HidePulsar();
        /// <summary>
        /// Shows the pulsar.
        /// </summary>
        void ShowPulsar();
        /// <summary>
        /// Closes Krento.
        /// </summary>
        void CloseKrento();
        /// <summary>
        /// Selects the next circle.
        /// </summary>
        void SelectNextCircle();
        /// <summary>
        /// Selects the previous circle.
        /// </summary>
        void SelectPreviousCircle();
        /// <summary>
        /// Changes the current circle.
        /// </summary>
        /// <param name="circleName">Name of the circle to load. This parameter must be the circle file name with full path specidied</param>
        void ChangeCurrentCircle(string circleName);
        /// <summary>
        /// Saves the current circle.
        /// </summary>
        void SaveCurrentCircle();
        /// <summary>
        /// Saves the manager settings.
        /// </summary>
        void SaveManagerSettings();
        /// <summary>
        /// Shows the help pages.
        /// </summary>
        void ShowHelpPages();
        /// <summary>
        /// Modifies the settings.
        /// </summary>
        void ModifySettings();
        /// <summary>
        /// Suppresses the hook message.
        /// </summary>
        /// <param name="value">if set to <c>true</c> the hook message for activating the manager is supressed.</param>
        void SuppressHookMessage(bool value);
        /// <summary>
        /// Kills Krento. Call KillKrento to terminate the program in case of problem
        /// </summary>
        void KillKrento();
    }
}
