using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AForge.Imaging;
using AForge.Imaging.Filters;
using SymaCord.TryOnMirror.Core.Util.ColorConverter;
using HSL = AForge.Imaging.HSL;

namespace SymaCord.TryOnMirror.Core.Imaging.AforgeFilters
{
    public class LuminanceModifier : BaseInPlacePartialFilter
    {
        private double luminance = 0;

        // private format translation dictionary
        private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

        /// <summary>
        /// Format translations dictionary.
        /// </summary>
        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get { return formatTranslations; }
        }
        
        public double Luminance
        {
            get { return luminance; }
            set
            {
                if ((value >= -0) && (value <= 1))
                {
                    luminance = value;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HueModifier"/> class.
        /// </summary>
        /// 
        public LuminanceModifier()
        {
            formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
            formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
            formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
            formatTranslations[PixelFormat.Format32bppPArgb] = PixelFormat.Format32bppPArgb;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LuminanceModifier"/> class.
        /// </summary>
        /// 
        /// <param name="Luminance">Luminance value to set.</param>
        /// 
        public LuminanceModifier(double luminance) : this()
        {
            this.luminance = luminance;
        }


        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="image">Source image data.</param>
        /// <param name="rect">Image rectangle for processing by the filter.</param>
        ///
        protected override unsafe void ProcessFilter(UnmanagedImage image, Rectangle rect)
        {
            int pixelSize = Bitmap.GetPixelFormatSize(image.PixelFormat) / 8;

            int width = rect.Width;
            int height = rect.Height;

            //Rectangle rc = new Rectangle(0, 0, width, height);

            //if (source.PixelFormat != PixelFormat.Format24bppRgb) source = source.Clone(rc, PixelFormat.Format24bppRgb);

            //Bitmap dest = new Bitmap(width, height, source.PixelFormat);
            
            //BitmapData dataSrc = source.LockBits(rc, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            //BitmapData dataDest = dest.LockBits(rc, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            
            int offset = image.Stride - (width * 3);

            //unsafe
            //{
            byte* bytesSrc = (byte*)image.ImageData.ToPointer(); //(byte*)(void*)dataSrc.Scan0;
            byte* bytesDest = (byte*)image.ImageData.ToPointer(); //(byte*)(void*)dataDest.Scan0;

                for (int y = 0; y < height; ++y)
                {
                    for (int x = 0; x < width; ++x)
                    {
                        var hsl = ColorSpace.RGBtoHSL(bytesSrc[2], bytesSrc[1], bytesSrc[0]); // Still BGR
                        hsl.Luminance *= luminance;

                        var c = hsl.RGB;

                        bytesDest[0] = (byte)c.Blue;
                        bytesDest[1] = (byte)c.Green;
                        bytesDest[2] = (byte)c.Red;

                        bytesSrc += 3;
                        bytesDest += 3;
                    }

                    bytesDest += offset;
                    bytesSrc += offset;
                }

            //    source.UnlockBits(dataSrc);
            //    dest.UnlockBits(dataDest);
            //}
        }
    }
}
