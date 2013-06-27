using System;
using System.Collections.Generic;
using System.Text;

namespace Krento
{
    /// <summary>
    /// Defines methods for managing file drag and drop functionality of Pulsar.
    /// </summary>
    public interface IDropFileManager
    {
        /// <summary>
        /// Registers the drop file handler.
        /// </summary>
        /// <param name="extension">The file name extension.</param>
        /// <param name="handler">Represents the method that will handle an file drop event.</param>
        void RegisterDropFileHandler(string extension, PulsarDropFileHandler handler);
        /// <summary>
        /// Removes the drop file handler.
        /// </summary>
        /// <param name="extension">The file name extension.</param>
        void RemoveDropFileHandler(string extension);
    }
}
