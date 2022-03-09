using System;
using System.Drawing;

namespace SymaCord.TryOnMirror.Core.Imaging
{
    /// <summary>
    /// ImageManipulator class, developed by Fabio Costa http://www.fabiocanada.ca
    /// 2013-4-4
    /// </summary>
    public sealed class ImageManipulator
    {
        public static System.IO.MemoryStream ShadeColor(byte[] imageContent, Color newColor)
        {
            if (imageContent == null || imageContent.Length == 0)
                return null;

            //convert bytes to bitmap
            var bitmap = new Bitmap(new System.IO.MemoryStream(imageContent));

            //create the Graphics object
            var g = System.Drawing.Graphics.FromImage(bitmap);

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

            var colorMatrix = ToColorMatrix(newColor); //refer to my other article for this method

            //create the image attributes
            var attributes = new System.Drawing.Imaging.ImageAttributes();

            //set the color matrix attribute
            attributes.SetColorMatrix(colorMatrix);

            g.DrawImage(bitmap,
                new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                0, 0, bitmap.Width, bitmap.Height,
                System.Drawing.GraphicsUnit.Pixel, attributes);

            //save bitmap
            var imageResult = new System.IO.MemoryStream(bitmap.Width * bitmap.Height);
            bitmap.Save(imageResult, System.Drawing.Imaging.ImageFormat.Png);

            return imageResult;

        }

        public static System.IO.MemoryStream ShadeColor(byte[] imageContent, string htmlColorCode)
        {
            return ShadeColor(imageContent, ToColor(htmlColorCode));
        }

        public static Color ToColor(string newColorInHtml)
        {
            var newColor = Color.Transparent;
            if (!string.IsNullOrWhiteSpace(newColorInHtml))
            {
                if (newColorInHtml[0] != '#')
                    newColorInHtml = "#" + newColorInHtml;

                newColor = ColorTranslator.FromHtml(newColorInHtml);
            }

            return newColor;
        }

        /// <summary>
        /// HSL to RGB conversion.
        /// </summary>
        /// <param name="h">The h.</param>
        /// <param name="sl">The sl.</param>
        /// <param name="l">The l.</param>
        /// <returns></returns>
        public static ColorRGB HSL2RGB(double h, double sl, double l)
        {

            double v;
            double r, g, b;
            r = l;   // default to gray
            g = l;
            b = l;
            v = (l <= 0.5) ? (l * (1.0 + sl)) : (l + sl - l * sl);
            if (v > 0)
            {
                double m;
                double sv;
                int sextant;
                double fract, vsf, mid1, mid2;

                m = l + l - v;
                sv = (v - m) / v;
                h *= 6.0;
                sextant = (int)h;
                fract = h - sextant;
                vsf = v * sv * fract;
                mid1 = m + vsf;
                mid2 = v - vsf;
                switch (sextant)
                {
                    case 0:
                        r = v;
                        g = mid1;
                        b = m;
                        break;
                    case 1:
                        r = mid2;
                        g = v;
                        b = m;
                        break;
                    case 2:
                        r = m;
                        g = v;
                        b = mid1;
                        break;
                    case 3:
                        r = m;
                        g = mid2;
                        b = v;
                        break;
                    case 4:
                        r = mid1;
                        g = m;
                        b = v;
                        break;
                    case 5:
                        r = v;
                        g = m;
                        b = mid2;
                        break;
                }
            }
            ColorRGB rgb = new ColorRGB();
            rgb.R = Convert.ToByte(r * 255.0f);
            rgb.G = Convert.ToByte(g * 255.0f);
            rgb.B = Convert.ToByte(b * 255.0f);
            
            return rgb;

        }

        public static System.Drawing.Imaging.ColorMatrix ToColorMatrix(Color color)
        {
            //convert the RGB values to scale of 0-1
            float r = (1.0f / 255) * color.R;
            float b = (1.0f / 255) * color.B;
            float g = (1.0f / 255) * color.G;

            var colorMatrix = new System.Drawing.Imaging.ColorMatrix(
                new float[][]
        {
            new float[] {r, g, b, 0, 0},
            new float[] {0,  0,  0,  0, 0},
            new float[] {0,  0, 0,  0, 0},
            new float[] {0,  0,  0,  1, 0},
            new float[] {0, 0, 0, 0, 1}
        });

            return colorMatrix;
        }

        public static System.Drawing.Imaging.ColorMatrix ToColorMatrix(ColorRGB color)
        {
            //convert the RGB values to scale of 0-1
            float r = (0.5f / 255) * color.R;
            float b = (0.5f / 255) * color.B;
            float g = (0.5f / 255) * color.G;

            var colorMatrix = new System.Drawing.Imaging.ColorMatrix(
                new float[][]
        {
            new float[] {r, g, b, 0, 0},
            new float[] {0,  0,  0,  0, 0},
            new float[] {0,  0, 0,  0, 0},
            new float[] {0,  0,  0,  1, 0},
            new float[] {0, 0, 0, 0, 1}
        });

            return colorMatrix;
        }
    }



    /// <summary>
    /// Utility struct for color conversions
    /// </summary>
    public struct ColorRGB
    {
        #region Fields

        private byte r;
        private byte g;
        private byte b;

        #endregion

        /// <summary>
        /// Gets or sets the Red value.
        /// </summary>
        /// <value>The R.</value>
        public byte R
        {
            get { return r; }
            set { r = value; }
        }
        
        /// <summary>
        /// Gets or sets the Green value.
        /// </summary>
        /// <value>The G.</value>
        public byte G
        {
            get { return g; }
            set { g = value; }
        }
        
        /// <summary>
        /// Gets or sets the Blue value.
        /// </summary>
        /// <value>The B.</value>
        public byte B
        {
            get { return b; }
            set { b = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ColorRGB"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public ColorRGB(Color value)
        {
            this.r = value.R;
            this.g = value.G;
            this.b = value.B;
        }

        /// <summary>
        /// Implicit conversion of the specified RGB.
        /// </summary>
        /// <param name="rgb">The RGB.</param>
        /// <returns></returns>
        public static implicit operator Color(ColorRGB rgb)
        {
            Color c = Color.FromArgb(255, rgb.R, rgb.G, rgb.B);
            return c;
        }

        /// <summary>
        /// Explicit conversion of the specified c.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <returns></returns>
        public static explicit operator ColorRGB(Color c)
        {
            return new ColorRGB(c);
        }
    }
}
