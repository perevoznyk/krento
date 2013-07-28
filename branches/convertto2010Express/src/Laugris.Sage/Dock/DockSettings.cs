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
using System.ComponentModel;

namespace Laugris.Sage
{

    /// <summary>
    /// Default dock settings. <see cref="DefaultDockSettings"/> is a static class that holds default 
    /// values for the dock settings.
    /// </summary>
    [Description("Default dock settings")]
    public static class DefaultDockSettings
    {
        /// <summary>
        /// Default icons spacing in pixels
        /// </summary>
        public const int IconsSpacing = 10;
        /// <summary>
        /// Default icons size in pixels
        /// </summary>
        public const int IconSize = 64;
        /// <summary>
        /// Default margin from the left side of the dock to the first item
        /// </summary>
        public const int LeftMargin = 0;
        /// <summary>
        /// Default margin from the top side of the dock to the first item
        /// </summary>
        public const int TopMargin = 0;
        /// <summary>
        /// Default mirror reflection depth in pixels
        /// </summary>
        public const int ReflectionDepth = 24;
        /// <summary>
        /// Multiplier is used for calclulating of image scale.
        /// </summary>
        public const double Multiplier = 60;
        /// <summary>
        /// Maximum icon scale (times). Keep this number small enough
        /// </summary>
        public const double MaxScale = 3.0;
        /// <summary>
        /// The distance between the border of the icon and mouse cursor when
        /// the item can be considered as selected
        /// </summary>
        public const int SelectionThreshold = 4;
    }

    /// <summary>
    /// Setting used by the dock for representing dock items
    /// </summary>
    public class DockSettings
    {
        #region Private fields
        private int leftMargin;
        private int topMargin;
        private int iconSize;
        private int iconsSpacing;
        private int reflectionDepth;
        private bool scaleCaption;
        private bool showCaptions;
        private int selectionThreshold;
        private double maxScale;
        private double multiplier;
        #endregion

        #region Events
        /// <summary>
        /// Occurs when settings are changed.
        /// </summary>
        public event EventHandler Changed;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="DockSettings"/> class.
        /// </summary>
        public DockSettings()
        {
            iconsSpacing = DefaultDockSettings.IconsSpacing;
            maxScale = DefaultDockSettings.MaxScale;
            iconSize = DefaultDockSettings.IconSize;
            reflectionDepth = DefaultDockSettings.ReflectionDepth;
            selectionThreshold = DefaultDockSettings.SelectionThreshold;
            multiplier = DefaultDockSettings.Multiplier;
            showCaptions = true;
        }

        /// <summary>
        /// Raises the <see cref="E:Changed"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnChanged(EventArgs e)
        {
            if (Changed != null)
            {
                Changed(this, e);
            }
        }

        private void DoChange()
        {
            OnChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Gets or sets the reflection depth.
        /// </summary>
        /// <value>The reflection depth.</value>
        [DefaultValue(24)]
        public int ReflectionDepth
        {
            get
            {
                return reflectionDepth;
            }
            set
            {
                if (reflectionDepth != value)
                {
                    reflectionDepth = value;
                    DoChange();
                }
            }
        }

        /// <summary>
        /// Gets or sets the left margin.
        /// </summary>
        /// <value>The left margin.</value>
        [DefaultValue(0)]
        public int LeftMargin
        {
            get
            {
                return leftMargin;
            }
            set
            {
                if (leftMargin != value)
                {
                    leftMargin = value;
                    DoChange();
                }
            }
        }

        /// <summary>
        /// Gets or sets the top margin.
        /// </summary>
        /// <value>The top margin.</value>
        [DefaultValue(0)]
        public int TopMargin
        {
            get
            {
                return topMargin;
            }
            set
            {
                if (topMargin != value)
                {
                    topMargin = value;
                    DoChange();
                }
            }
        }

        public double MaxScale
        {
            get { return maxScale; }
            set
            {
                if (maxScale != value)
                {
                    maxScale = value;
                    DoChange();
                }
            }
        }

        public bool ShowCaptions
        {
            get { return showCaptions; }
            set { showCaptions = value; }
        }

        public double Multiplier
        {
            get { return multiplier; }
            set
            {
                if (multiplier != value)
                {
                    multiplier = value;
                    DoChange();
                }
            }
        }

        [DefaultValue(4)]
        public int SelectionThreshold
        {
            get { return selectionThreshold; }
            set
            {
                if (selectionThreshold != value)
                {
                    selectionThreshold = value;
                    DoChange();
                }
            }
        }

        /// <summary>
        /// Gets or sets the size of the icon.
        /// </summary>
        /// <value>The size of the icon.</value>
        [DefaultValue(48)]
        public int IconSize
        {
            get
            {
                return iconSize;
            }
            set
            {
                if (iconSize != value)
                {
                    iconSize = value;
                    DoChange();
                }
            }
        }

        /// <summary>
        /// Gets or sets the icons spacing.
        /// </summary>
        /// <value>The icons spacing.</value>
        [DefaultValue(10)]
        public int IconsSpacing
        {
            get
            {
                return iconsSpacing;
            }
            set
            {
                if (iconsSpacing != value)
                {
                    iconsSpacing = value;
                    DoChange();
                }
            }
        }

        [DefaultValue(false)]
        public bool ScaleCaption
        {
            get { return scaleCaption; }
            set
            {
                if (scaleCaption != value)
                {
                    scaleCaption = value;
                    DoChange();
                }
            }
        }

    }
}
