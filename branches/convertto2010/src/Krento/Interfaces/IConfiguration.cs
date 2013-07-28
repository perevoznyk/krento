using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Krento
{
    /// <summary>
    /// Defines methods for managing Krento configuration
    /// </summary>
    public interface IConfiguration : INotifyPropertyChanged
    {
        /// <summary>
        /// Clears all configuration parameters
        /// </summary>
        void Clear();
        /// <summary>
        /// Clears the property. The value of the property will be removed from Krento configuration.
        /// </summary>
        /// <param name="name">The name of the property to be removed.</param>
        void ClearProperty(string name);
        /// <summary>
        /// Gets the value of the property.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <returns>The value of the property</returns>
        string GetProperty(string name);
        /// <summary>
        /// Gets the value of the property or default value if the value is not defined
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="defaultValue">The default value of the property.</param>
        /// <returns>The value of the property or default value, if property is not found.</returns>
        string GetProperty(string name, string defaultValue);
        /// <summary>
        /// Determines whether the specified property exists
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <returns>
        /// 	<c>true</c> if the specified  property exists; otherwise, <c>false</c>.
        /// </returns>
        bool HasProperty(string name);
        /// <summary>
        /// Sets the value of the property.
        /// </summary>
        /// <param name="name">The name of the property name.</param>
        /// <param name="value">The value of the property.</param>
        void SetProperty(string name, string value);
        /// <summary>
        /// Sets the property value.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The value of the property.</param>
        /// <param name="defaultValue">The default value of the property.</param>
        void SetProperty(string name, string value, string defaultValue);
    }
}
