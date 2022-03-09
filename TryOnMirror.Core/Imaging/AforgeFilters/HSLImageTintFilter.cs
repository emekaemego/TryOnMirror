using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using AForge.Imaging;
using AForge.Imaging.Filters;
using RGB = AForge.Imaging.RGB;

namespace SymaCord.TryOnMirror.Core.Imaging.AforgeFilters
{
    /// <para>The filter accepts 24 and 32 bpp color images for processing.</para>
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// HueModifier filter = new HueModifier( 180 );
    /// // apply the filter
    /// filter.ApplyInPlace( image );
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/sample1.jpg" width="480" height="361" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/hue_modifier.jpg" width="480" height="361" />
    /// </remarks>
    /// 
    public class HSLImageTintFilter : BaseInPlacePartialFilter
    {
        private int hue = 0;
        private float saturation = 0;
        private float luminance = 0;

        // private format translation dictionary
        private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

        /// <summary>
        /// Format translations dictionary.
        /// </summary>
        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get { return formatTranslations; }
        }

        /// <summary>
        /// Hue value to set, [0, 359].
        /// </summary>
        /// 
        /// <remarks><para>Default value is set to <b>0</b>.</para></remarks>
        /// 
        public int Hue
        {
            get { return hue; }
            set { hue = Math.Max(0, Math.Min(359, value)); }
        }

        public float Saturation
        {
            get { return saturation; }
            set
            {
                if ((value >= -0) && (value <= 1))
                {
                    saturation = value;
                }
            }
        }

        public float Luminance
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
        public HSLImageTintFilter()
        {
            formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
            formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
            formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
            formatTranslations[PixelFormat.Format32bppPArgb] = PixelFormat.Format32bppPArgb;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HueModifier"/> class.
        /// </summary>
        /// 
        /// <param name="hue">Hue value to set.</param>
        /// 
        public HSLImageTintFilter(int hue, float saturation, float luminance) : this()
        {
            this.hue = hue;
            this.saturation = saturation;
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
            int pixelSize = Bitmap.GetPixelFormatSize(image.PixelFormat)/8;

            int startX = rect.Left;
            int startY = rect.Top;
            int stopX = startX + rect.Width;
            int stopY = startY + rect.Height;
            int offset = image.Stride - rect.Width*pixelSize;

            var rgb = new Util.ColorConverter.RGB();
            var hsl = new Util.ColorConverter.HSL();

            // do the job
            byte* ptr = (byte*) image.ImageData.ToPointer();

            // allign pointer to the first pixel to process
            ptr += (startY*image.Stride + startX*pixelSize);
            //var frequency = new Dictionary<Color, int>();
            //long sumA = 0, sumR = 0, sumG = 0, sumB = 0;
            
            // for each row
            for (int y = startY; y < stopY; y++)
            {
                // for each pixel
                for (int x = startX; x < stopX; x++, ptr += pixelSize)
                {
                    rgb.Red = ptr[RGB.R];
                    rgb.Green = ptr[RGB.G];
                    rgb.Blue = ptr[RGB.B];

                    //sumA += Convert.ToInt32(ptr[RGB.A]);
                    //sumR += Convert.ToInt32(rgb.Red);
                    //sumG += Convert.ToInt32(rgb.Green);
                    //sumB += Convert.ToInt32(rgb.Blue);
                    
                    // convert to HSL
                    Util.ColorConverter.HSL.FromRGB(rgb, hsl);
                    
                    // modify hsl values
                    hsl.Hue = hue;
                    hsl.Saturation = saturation;
                    hsl.Luminance = Math.Min(0.97f, hsl.Luminance * (120 * luminance / 65));
                    
                    // convert back to RGB
                    Util.ColorConverter.HSL.ToRGB(hsl, rgb);

                    ptr[RGB.R] = (byte)rgb.Red;
                    ptr[RGB.G] = (byte)rgb.Green;
                    ptr[RGB.B] = (byte)rgb.Blue;
                }
                ptr += offset;
            }

            //var result = new RGB();
            //result.Alpha = (byte)(sumA / (rect.Height * rect.Width));
            //result.Red = (byte)(sumR / (rect.Height * rect.Width));
            //result.Green = (byte)(sumG / (rect.Height * rect.Width));
            //result.Blue = (byte)(sumB / (rect.Height * rect.Width));

            //Logger("#" + result.Red.ToString("X2") + result.Green.ToString("X2") + result.Blue.ToString("X2"));

            //int totalPixels = image.Width * image.Height;
            //foreach (var kvp in frequency)
            //{
            //    Logger(String.Format("Color (R={0},G={1},B={2}): {3}", kvp.Key.R, kvp.Key.G, kvp.Key.B, kvp.Value / (double)totalPixels));
            //}
        }
        
        public static void Logger(String lines)
        {
            // Write the string to a file.append mode is enabled so that the log
            // lines get appended to  test.txt than wiping content and writing the log

            var file = new System.IO.StreamWriter("c:\\test.txt", true);
            file.WriteLine(lines);

            file.Close();
        }

        public static double CalculateAverageLightness(Bitmap bm)
        {
            double lum = 0;
            var tmpBmp = new Bitmap(bm);
            var width = bm.Width;
            var height = bm.Height;
            var bppModifier = bm.PixelFormat == PixelFormat.Format24bppRgb ? 3 : 4;

            var srcData = tmpBmp.LockBits(new Rectangle(0, 0, bm.Width, bm.Height), ImageLockMode.ReadOnly,
                                          bm.PixelFormat);
            var stride = srcData.Stride;
            var scan0 = srcData.Scan0;

            //Luminance (standard, objective): (0.2126*R) + (0.7152*G) + (0.0722*B)
            //Luminance (perceived option 1): (0.299*R + 0.587*G + 0.114*B)
            //Luminance (perceived option 2, slower to calculate): sqrt( 0.241*R^2 + 0.691*G^2 + 0.068*B^2 )

            unsafe
            {
                byte* p = (byte*) (void*) scan0;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int idx = (y*stride) + x*bppModifier;
                        lum += (0.299*p[idx + 2] + 0.587*p[idx + 1] + 0.114*p[idx]);
                    }
                }
            }

            tmpBmp.UnlockBits(srcData);
            tmpBmp.Dispose();
            var avgLum = lum/(width*height);

            return avgLum/255.0;
        }
    }



    public struct ColorRGB
    {
        public byte R;
        public byte G;
        public byte B;

        public ColorRGB(Color value)
        {
            this.R = value.R;
            this.G = value.G;
            this.B = value.B;
        }

        public static implicit operator Color(ColorRGB rgb)
        {
            Color c = Color.FromArgb(rgb.R, rgb.G, rgb.B);
            return c;
        }

        public static explicit operator ColorRGB(Color c)
        {
            return new ColorRGB(c);
        }
    }

    public class ColorConvertr
    {
        // Given H,S,L in range of 0-1
        // Returns a Color (RGB struct) in range of 0-255
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

            ColorRGB rgb;

            rgb.R = Convert.ToByte(r * 255.0f);

            rgb.G = Convert.ToByte(g * 255.0f);

            rgb.B = Convert.ToByte(b * 255.0f);

            return rgb;

        }


        // Given a Color (RGB Struct) in range of 0-255
        // Return H,S,L in range of 0-1
        public static void RGB2HSL(ColorRGB rgb, out double h, out double s, out double l)
        {
            double r = rgb.R / 255.0;
            double g = rgb.G / 255.0;
            double b = rgb.B / 255.0;

            double v;

            double m;

            double vm;

            double r2, g2, b2;



            h = 0; // default to black

            s = 0;

            l = 0;

            v = Math.Max(r, g);

            v = Math.Max(v, b);

            m = Math.Min(r, g);

            m = Math.Min(m, b);

            l = (m + v) / 2.0;

            if (l <= 0.0)
            {

                return;

            }

            vm = v - m;

            s = vm;

            if (s > 0.0)
            {

                s /= (l <= 0.5) ? (v + m) : (2.0 - v - m);

            }

            else
            {

                return;

            }

            r2 = (v - r) / vm;

            g2 = (v - g) / vm;

            b2 = (v - b) / vm;

            if (r == v)
            {

                h = (g == m ? 5.0 + b2 : 1.0 - g2);

            }

            else if (g == v)
            {

                h = (b == m ? 1.0 + r2 : 3.0 - b2);

            }

            else
            {

                h = (r == m ? 3.0 + g2 : 5.0 - r2);

            }

            h /= 6.0;

        }
    }

}