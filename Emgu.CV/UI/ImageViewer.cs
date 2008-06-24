using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Emgu.CV.UI
{
    /// <summary>
    /// The Image viewer that display IImage
    /// </summary>
    public partial class ImageViewer : Form
    {
        /// <summary>
        /// Create a ImageViewer from the specific <paramref name="img"/>
        /// </summary>
        /// <param name="image">The image to be displayed in this viewer</param>
        public ImageViewer(IImage image)
        {
            InitializeComponent();
            imageBox1.Image = image;
        }

        /// <summary>
        /// Create a ImageViewer from the specific <paramref name="img"/>, using <paramref name="windowName"/> as window name
        /// </summary>
        /// <param name="image">The image to be displayed</param>
        /// <param name="windowName">The name of the window</param>
        public ImageViewer(IImage image, string windowName)
            : this(image)
        {
            this.Text = windowName;
        }
    }
}