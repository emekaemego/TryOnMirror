using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymaCord.TryOnMirror.Core.Util.ColorConverter
{
        using System;
        using System.Drawing;

        /// <summary>
        /// RGB components.
        /// </summary>
        /// 
        /// <remarks><para>The class encapsulates <b>RGB</b> color components.</para>
        /// <para><note><see cref="System.Drawing.Imaging.PixelFormat">PixelFormat.Format24bppRgb</see>
        /// actually means BGR format.</note></para>
        /// </remarks>
        /// 
        public class RGB
        {
            /// <summary>
            /// Index of red component.
            /// </summary>
            public const short R = 2;

            /// <summary>
            /// Index of green component.
            /// </summary>
            public const short G = 1;

            /// <summary>
            /// Index of blue component.
            /// </summary>
            public const short B = 0;

            /// <summary>
            /// Index of alpha component for ARGB images.
            /// </summary>
            public const short A = 3;

            /// <summary>
            /// Red component.
            /// </summary>
            public byte Red;

            /// <summary>
            /// Green component.
            /// </summary>
            public byte Green;

            /// <summary>
            /// Blue component.
            /// </summary>
            public byte Blue;

            /// <summary>
            /// Alpha component.
            /// </summary>
            public byte Alpha;

            /// <summary>
            /// <see cref="System.Drawing.Color">Color</see> value of the class.
            /// </summary>
            public System.Drawing.Color Color
            {
                get { return Color.FromArgb(Alpha, Red, Green, Blue); }
                set
                {
                    Red = value.R;
                    Green = value.G;
                    Blue = value.B;
                    Alpha = value.A;
                }
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="RGB"/> class.
            /// </summary>
            public RGB()
            {
                Red = 0;
                Green = 0;
                Blue = 0;
                Alpha = 255;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="RGB"/> class.
            /// </summary>
            /// 
            /// <param name="red">Red component.</param>
            /// <param name="green">Green component.</param>
            /// <param name="blue">Blue component.</param>
            /// 
            public RGB(byte red, byte green, byte blue)
            {
                this.Red = red;
                this.Green = green;
                this.Blue = blue;
                this.Alpha = 255;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="RGB"/> class.
            /// </summary>
            /// 
            /// <param name="red">Red component.</param>
            /// <param name="green">Green component.</param>
            /// <param name="blue">Blue component.</param>
            /// <param name="alpha">Alpha component.</param>
            /// 
            public RGB(byte red, byte green, byte blue, byte alpha)
            {
                this.Red = red;
                this.Green = green;
                this.Blue = blue;
                this.Alpha = alpha;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="RGB"/> class.
            /// </summary>
            /// 
            /// <param name="color">Initialize from specified <see cref="System.Drawing.Color">color.</see></param>
            /// 
            public RGB(System.Drawing.Color color)
            {
                this.Red = color.R;
                this.Green = color.G;
                this.Blue = color.B;
                this.Alpha = color.A;
            }
        }

        /// <summary>
        /// HSL components.
        /// </summary>
        /// 
        /// <remarks>The class encapsulates <b>HSL</b> color components.</remarks>
        /// 
        public class HSL
        {
            /// <summary>
            /// Hue component.
            /// </summary>
            /// 
            /// <remarks>Hue is measured in the range of [0, 359].</remarks>
            /// 
            public int Hue;

            /// <summary>
            /// Saturation component.
            /// </summary>
            /// 
            /// <remarks>Saturation is measured in the range of [0, 1].</remarks>
            /// 
            public float Saturation;

            /// <summary>
            /// Luminance value.
            /// </summary>
            /// 
            /// <remarks>Luminance is measured in the range of [0, 1].</remarks>
            /// 
            public float Luminance;

            /// <summary>
            /// Initializes a new instance of the <see cref="HSL"/> class.
            /// </summary>
            public HSL() { }

            /// <summary>
            /// Initializes a new instance of the <see cref="HSL"/> class.
            /// </summary>
            /// 
            /// <param name="hue">Hue component.</param>
            /// <param name="saturation">Saturation component.</param>
            /// <param name="luminance">Luminance component.</param>
            /// 
            public HSL(int hue, float saturation, float luminance)
            {
                this.Hue = hue;
                this.Saturation = saturation;
                this.Luminance = luminance;
            }

            /// <summary>
            /// Convert from RGB to HSL color space.
            /// </summary>
            /// 
            /// <param name="rgb">Source color in <b>RGB</b> color space.</param>
            /// <param name="hsl">Destination color in <b>HSL</b> color space.</param>
            /// 
            /// <remarks><para>See <a href="http://en.wikipedia.org/wiki/HSI_color_space#Conversion_from_RGB_to_HSL_or_HSV">HSL and HSV Wiki</a>
            /// for information about the algorithm to convert from RGB to HSL.</para></remarks>
            /// 
            public static void FromRGB(RGB rgb, HSL hsl)
            {
                float r = (rgb.Red / 255.0f);
                float g = (rgb.Green / 255.0f);
                float b = (rgb.Blue / 255.0f);

                float min = Math.Min(Math.Min(r, g), b);
                float max = Math.Max(Math.Max(r, g), b);
                float delta = max - min;

                // get luminance value
                hsl.Luminance = (max + min) / 2;

                if (delta == 0)
                {
                    // gray color
                    hsl.Hue = 0;
                    hsl.Saturation = 0.0f;
                }
                else
                {
                    // get saturation value
                    hsl.Saturation = (hsl.Luminance <= 0.5) ? (delta / (max + min)) : (delta / (2 - max - min));

                    // get hue value
                    float hue;

                    if (r == max)
                    {
                        hue = ((g - b) / 6) / delta;
                    }
                    else if (g == max)
                    {
                        hue = (1.0f / 3) + ((b - r) / 6) / delta;
                    }
                    else
                    {
                        hue = (2.0f / 3) + ((r - g) / 6) / delta;
                    }

                    // correct hue if needed
                    if (hue < 0)
                        hue += 1;
                    if (hue > 1)
                        hue -= 1;

                    hsl.Hue = (int)(hue * 360);
                }
            }

            /// <summary>
            /// Convert from RGB to HSL color space.
            /// </summary>
            /// 
            /// <param name="rgb">Source color in <b>RGB</b> color space.</param>
            /// 
            /// <returns>Returns <see cref="HSL"/> instance, which represents converted color value.</returns>
            /// 
            public static HSL FromRGB(RGB rgb)
            {
                HSL hsl = new HSL();
                FromRGB(rgb, hsl);
                return hsl;
            }

            /// <summary>
            /// Convert from HSL to RGB color space.
            /// </summary>
            /// 
            /// <param name="hsl">Source color in <b>HSL</b> color space.</param>
            /// <param name="rgb">Destination color in <b>RGB</b> color space.</param>
            /// 
            public static void ToRGB(HSL hsl, RGB rgb)
            {
                if (hsl.Saturation == 0)
                {
                    // gray values
                    rgb.Red = rgb.Green = rgb.Blue = (byte)(hsl.Luminance * 255);
                }
                else
                {
                    float v1, v2;
                    float hue = (float)hsl.Hue / 360;

                    v2 = (hsl.Luminance < 0.5) ?
                        (hsl.Luminance * (1 + hsl.Saturation)) :
                        ((hsl.Luminance + hsl.Saturation) - (hsl.Luminance * hsl.Saturation));
                    v1 = 2 * hsl.Luminance - v2;

                    rgb.Red = (byte)(255 * Hue_2_RGB(v1, v2, hue + (1.0f / 3)));
                    rgb.Green = (byte)(255 * Hue_2_RGB(v1, v2, hue));
                    rgb.Blue = (byte)(255 * Hue_2_RGB(v1, v2, hue - (1.0f / 3)));
                }
                rgb.Alpha = 255;
            }

            /// <summary>
            /// Convert the color to <b>RGB</b> color space.
            /// </summary>
            /// 
            /// <returns>Returns <see cref="RGB"/> instance, which represents converted color value.</returns>
            /// 
            public RGB ToRGB()
            {
                RGB rgb = new RGB();
                ToRGB(this, rgb);
                return rgb;
            }

            #region Private members
            // HSL to RGB helper routine
            private static float Hue_2_RGB(float v1, float v2, float vH)
            {
                if (vH < 0)
                    vH += 1;
                if (vH > 1)
                    vH -= 1;
                if ((6 * vH) < 1)
                    return (v1 + (v2 - v1) * 6 * vH);
                if ((2 * vH) < 1)
                    return v2;
                if ((3 * vH) < 2)
                    return (v1 + (v2 - v1) * ((2.0f / 3) - vH) * 6);
                return v1;
            }
            #endregion
        }

    /// <summary>
    /// YCbCr components.
    /// </summary>
    /// 
    /// <remarks>The class encapsulates <b>YCbCr</b> color components.</remarks>
    /// 
    public class YCbCr
    {
        /// <summary>
        /// Index of <b>Y</b> component.
        /// </summary>
        public const short YIndex = 0;

        /// <summary>
        /// Index of <b>Cb</b> component.
        /// </summary>
        public const short CbIndex = 1;

        /// <summary>
        /// Index of <b>Cr</b> component.
        /// </summary>
        public const short CrIndex = 2;

        /// <summary>
        /// <b>Y</b> component.
        /// </summary>
        public float Y;

        /// <summary>
        /// <b>Cb</b> component.
        /// </summary>
        public float Cb;

        /// <summary>
        /// <b>Cr</b> component.
        /// </summary>
        public float Cr;

        /// <summary>
        /// Initializes a new instance of the <see cref="YCbCr"/> class.
        /// </summary>
        public YCbCr()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="YCbCr"/> class.
        /// </summary>
        /// 
        /// <param name="y"><b>Y</b> component.</param>
        /// <param name="cb"><b>Cb</b> component.</param>
        /// <param name="cr"><b>Cr</b> component.</param>
        /// 
        public YCbCr(float y, float cb, float cr)
        {
            this.Y = Math.Max(0.0f, Math.Min(1.0f, y));
            this.Cb = Math.Max(-0.5f, Math.Min(0.5f, cb));
            this.Cr = Math.Max(-0.5f, Math.Min(0.5f, cr));
        }

        /// <summary>
        /// Convert from RGB to YCbCr color space (Rec 601-1 specification). 
        /// </summary>
        /// 
        /// <param name="rgb">Source color in <b>RGB</b> color space.</param>
        /// <param name="ycbcr">Destination color in <b>YCbCr</b> color space.</param>
        /// 
        public static void FromRGB(RGB rgb, YCbCr ycbcr)
        {
            float r = (float) rgb.Red/255;
            float g = (float) rgb.Green/255;
            float b = (float) rgb.Blue/255;

            ycbcr.Y = (float) (0.2989*r + 0.5866*g + 0.1145*b);
            ycbcr.Cb = (float) (-0.1687*r - 0.3313*g + 0.5000*b);
            ycbcr.Cr = (float) (0.5000*r - 0.4184*g - 0.0816*b);
        }

        /// <summary>
        /// Convert from RGB to YCbCr color space (Rec 601-1 specification).
        /// </summary>
        /// 
        /// <param name="rgb">Source color in <b>RGB</b> color space.</param>
        /// 
        /// <returns>Returns <see cref="YCbCr"/> instance, which represents converted color value.</returns>
        /// 
        public static YCbCr FromRGB(RGB rgb)
        {
            YCbCr ycbcr = new YCbCr();
            FromRGB(rgb, ycbcr);
            return ycbcr;
        }

        /// <summary>
        /// Convert from YCbCr to RGB color space.
        /// </summary>
        /// 
        /// <param name="ycbcr">Source color in <b>YCbCr</b> color space.</param>
        /// <param name="rgb">Destination color in <b>RGB</b> color spacs.</param>
        /// 
        public static void ToRGB(YCbCr ycbcr, RGB rgb)
        {
            // don't warry about zeros. compiler will remove them
            float r = Math.Max(0.0f, Math.Min(1.0f, (float) (ycbcr.Y + 0.0000*ycbcr.Cb + 1.4022*ycbcr.Cr)));
            float g = Math.Max(0.0f, Math.Min(1.0f, (float) (ycbcr.Y - 0.3456*ycbcr.Cb - 0.7145*ycbcr.Cr)));
            float b = Math.Max(0.0f, Math.Min(1.0f, (float) (ycbcr.Y + 1.7710*ycbcr.Cb + 0.0000*ycbcr.Cr)));

            rgb.Red = (byte) (r*255);
            rgb.Green = (byte) (g*255);
            rgb.Blue = (byte) (b*255);
            rgb.Alpha = 255;
        }

        /// <summary>
        /// Convert the color to <b>RGB</b> color space.
        /// </summary>
        /// 
        /// <returns>Returns <see cref="RGB"/> instance, which represents converted color value.</returns>
        /// 
        public RGB ToRGB()
        {
            RGB rgb = new RGB();
            ToRGB(this, rgb);
            return rgb;
        }
    }

    //public class ColorSpace
   // {
   //     #region HSL convert
   //     /// <summary>
   //     /// Converts HSL to RGB.
   //     /// </summary>
   //     /// <param name="h">Hue, must be in [0, 360].</param>
   //     /// <param name="s">Saturation, must be in [0, 1].</param>
   //     /// <param name="l">Luminance, must be in [0, 1].</param>
   //     public static RGB HSLtoRGB(double h, double s, double l)
   //     {
   //         if (s == 0)
   //         {
   //             // achromatic color (gray scale)
   //             return new RGB(
   //                 Convert.ToInt32(Double.Parse(String.Format("{0:0.00}", l * 255.0))),
   //                 Convert.ToInt32(Double.Parse(String.Format("{0:0.00}", l * 255.0))),
   //                 Convert.ToInt32(Double.Parse(String.Format("{0:0.00}", l * 255.0)))
   //                 );
   //         }
   //         else
   //         {
   //             double q = (l < 0.5) ? (l * (1.0 + s)) : (l + s - (l * s));
   //             double p = (2.0 * l) - q;

   //             double Hk = h / 360.0;
   //             double[] T = new double[3];
   //             T[0] = Hk + (1.0 / 3.0);	// Tr
   //             T[1] = Hk;				// Tb
   //             T[2] = Hk - (1.0 / 3.0);	// Tg

   //             for (int i = 0; i < 3; i++)
   //             {
   //                 if (T[i] < 0) T[i] += 1.0;
   //                 if (T[i] > 1) T[i] -= 1.0;

   //                 if ((T[i] * 6) < 1)
   //                 {
   //                     T[i] = p + ((q - p) * 6.0 * T[i]);
   //                 }
   //                 else if ((T[i] * 2.0) < 1) //(1.0/6.0)<=T[i] && T[i]<0.5
   //                 {
   //                     T[i] = q;
   //                 }
   //                 else if ((T[i] * 3.0) < 2) // 0.5<=T[i] && T[i]<(2.0/3.0)
   //                 {
   //                     T[i] = p + (q - p) * ((2.0 / 3.0) - T[i]) * 6.0;
   //                 }
   //                 else T[i] = p;
   //             }

   //             return new RGB(
   //                 Convert.ToInt32(Double.Parse(String.Format("{0:0.00}", T[0] * 255.0))),
   //                 Convert.ToInt32(Double.Parse(String.Format("{0:0.00}", T[1] * 255.0))),
   //                 Convert.ToInt32(Double.Parse(String.Format("{0:0.00}", T[2] * 255.0)))
   //                 );
   //         }
   //     }

   //     /// <summary>
   //     /// Converts HSL to RGB.
   //     /// </summary>
   //     /// <param name="hsl">The HSL structure to convert.</param>
   //     public static RGB HSLtoRGB(HSL hsl)
   //     {
   //         return HSLtoRGB(hsl.Hue, hsl.Saturation, hsl.Luminance);
   //     }


   //     /// <summary>
   //     /// Converts HSL to .net Color.
   //     /// </summary>
   //     /// <param name="hsl">The HSL structure to convert.</param>
   //     public static Color HSLtoColor(double h, double s, double l)
   //     {
   //         RGB rgb = HSLtoRGB(h, s, l);

   //         return Color.FromArgb(rgb.Red, rgb.Green, rgb.Blue);
   //     }

   //     /// <summary>
   //     /// Converts HSL to .net Color.
   //     /// </summary>
   //     /// <param name="hsl">The HSL structure to convert.</param>
   //     public static Color HSLtoColor(HSL hsl)
   //     {
   //         return HSLtoColor(hsl.Hue, hsl.Saturation, hsl.Luminance);
   //     }

   //     #endregion

   //     #region RGB convert
   //     /// <summary> 
   //     /// Converts RGB to HSL.
   //     /// </summary>
   //     /// <param name="red">Red value, must be in [0,255].</param>
   //     /// <param name="green">Green value, must be in [0,255].</param>
   //     /// <param name="blue">Blue value, must be in [0,255].</param>
   //     public static HSL RGBtoHSL(int red, int green, int blue)
   //     {
   //         double h = 0, s = 0, l = 0;

   //         // normalizes red-green-blue values
   //         double nRed = (double)red / 255.0;
   //         double nGreen = (double)green / 255.0;
   //         double nBlue = (double)blue / 255.0;

   //         double max = Math.Max(nRed, Math.Max(nGreen, nBlue));
   //         double min = Math.Min(nRed, Math.Min(nGreen, nBlue));

   //         // hue
   //         if (max == min)
   //         {
   //             h = 0; // undefined
   //         }
   //         else if (max == nRed && nGreen >= nBlue)
   //         {
   //             h = 60.0 * (nGreen - nBlue) / (max - min);
   //         }
   //         else if (max == nRed && nGreen < nBlue)
   //         {
   //             h = 60.0 * (nGreen - nBlue) / (max - min) + 360.0;
   //         }
   //         else if (max == nGreen)
   //         {
   //             h = 60.0 * (nBlue - nRed) / (max - min) + 120.0;
   //         }
   //         else if (max == nBlue)
   //         {
   //             h = 60.0 * (nRed - nGreen) / (max - min) + 240.0;
   //         }

   //         // luminance
   //         l = (max + min) / 2.0;

   //         // saturation
   //         if (l == 0 || max == min)
   //         {
   //             s = 0;
   //         }
   //         else if (0 < l && l <= 0.5)
   //         {
   //             s = (max - min) / (max + min);
   //         }
   //         else if (l > 0.5)
   //         {
   //             s = (max - min) / (2 - (max + min)); //(max-min > 0)?
   //         }

   //         return new HSL(
   //             Double.Parse(String.Format("{0:0.##}", h)),
   //             Double.Parse(String.Format("{0:0.##}", s)),
   //             Double.Parse(String.Format("{0:0.##}", l))
   //             );
   //     }

   //     /// <summary> 
   //     /// Converts RGB to HSL.
   //     /// </summary>
   //     public static HSL RGBtoHSL(RGB rgb)
   //     {
   //         return RGBtoHSL(rgb.Red, rgb.Green, rgb.Blue);
   //     }

   //     /// <summary> 
   //     /// Converts Color to HSL.
   //     /// </summary>
   //     public static HSL RGBtoHSL(Color c)
   //     {
   //         return RGBtoHSL(c.R, c.G, c.B);
   //     }

   //     #endregion

   //     //public static Bitmap Luminance(Bitmap source, float factor)
   //     //{
   //     //    int width = source.Width;
   //     //    int height = source.Height;

   //     //    Rectangle rc = new Rectangle(0, 0, width, height);

   //     //    if (source.PixelFormat != PixelFormat.Format24bppRgb) source = source.Clone(rc, PixelFormat.Format24bppRgb);

   //     //    Bitmap dest = new Bitmap(width, height, source.PixelFormat);

   //     //    BitmapData dataSrc = source.LockBits(rc, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
   //     //    BitmapData dataDest = dest.LockBits(rc, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

   //     //    int offset = dataSrc.Stride - (width * 3);

   //     //    unsafe
   //     //    {
   //     //        byte* bytesSrc = (byte*)(void*)dataSrc.Scan0;
   //     //        byte* bytesDest = (byte*)(void*)dataDest.Scan0;

   //     //        for (int y = 0; y < height; ++y)
   //     //        {
   //     //            for (int x = 0; x < width; ++x)
   //     //            {
   //     //                HSL hsl = RGBtoHSL(bytesSrc[2], bytesSrc[1], bytesSrc[0]); // Still BGR
   //     //                hsl.Luminance *= factor;

   //     //                Color c = hsl.RGB;

   //     //                bytesDest[0] = c.B;
   //     //                bytesDest[1] = c.G;
   //     //                bytesDest[2] = c.R;

   //     //                bytesSrc += 3;
   //     //                bytesDest += 3;
   //     //            }

   //     //            bytesDest += offset;
   //     //            bytesSrc += offset;
   //     //        }

   //     //        source.UnlockBits(dataSrc);
   //     //        dest.UnlockBits(dataDest);
   //     //    }

   //     //    return dest;
   //     //}
   // }




    /**
 *  The HSLColor class provides methods to manipulate HSL (Hue, Saturation
 *  Luminance) values to create a corresponding Color object using the RGB
 *  ColorSpace.
 *
 *  The HUE is the color, the Saturation is the purity of the color (with
 *  respect to grey) and Luminance is the brightness of the color (with respect
 *  to black and white)
 *
 *  The Hue is specified as an angel between 0 - 360 degrees where red is 0,
 *  green is 120 and blue is 240. In between you have the colors of the rainbow.
 *  Saturation is specified as a percentage between 0 - 100 where 100 is fully
 *  saturated and 0 approaches gray. Luminance is specified as a percentage
 *  between 0 - 100 where 0 is black and 100 is white.
 *
 *  In particular the HSL color space makes it easier change the Tone or Shade
 *  of a color by adjusting the luminance value.
 */
    public class HSLColor
    {
        private Color rgb;
        private float[] hsl;
        private float alpha;

        /**
         *  Create a HSLColor object using an RGB Color object.
         *
         *  @param rgb the RGB Color object
         */
        public HSLColor(Color rgb)
        {
            this.rgb = rgb;
            hsl = fromRGB(rgb);
            alpha = rgb.A / 255.0f;
        }

        /**
         *  Create a HSLColor object using individual HSL values and a default
         * alpha value of 1.0.
         *
         *  @param h is the Hue value in degrees between 0 - 360
         *  @param s is the Saturation percentage between 0 - 100
         *  @param l is the Lumanance percentage between 0 - 100
         */
        public HSLColor(float h, float s, float l):this(h, s, l, 1.0f) {}

        /**
         *  Create a HSLColor object using individual HSL values.
         *
         *  @param h     the Hue value in degrees between 0 - 360
         *  @param s     the Saturation percentage between 0 - 100
         *  @param l     the Lumanance percentage between 0 - 100
         *  @param alpha the alpha value between 0 - 1
         */
        public HSLColor(float h, float s, float l, float alpha)
        {
            hsl = new float[] { h, s, l };
            this.alpha = alpha;
            rgb = toRGB(hsl, alpha);
        }

        /**
         *  Create a HSLColor object using an an array containing the
         *  individual HSL values and with a default alpha value of 1.
         *
         *  @param hsl  array containing HSL values
         */
        public HSLColor(float[] hsl) : this(hsl, 1.0f) {}

        /**
         *  Create a HSLColor object using an an array containing the
         *  individual HSL values.
         *
         *  @param hsl  array containing HSL values
         *  @param alpha the alpha value between 0 - 1
         */
        public HSLColor(float[] hsl, float alpha)
        {
            this.hsl = hsl;
            this.alpha = alpha;
            rgb = toRGB(hsl, alpha);
        }

        /**
         *  Create a RGB Color object based on this HSLColor with a different
         *  Hue value. The degrees specified is an absolute value.
         *
         *  @param degrees - the Hue value between 0 - 360
         *  @return the RGB Color object
         */
        public Color adjustHue(float degrees)
        {
            return toRGB(degrees, hsl[1], hsl[2], alpha);
        }

        /**
         *  Create a RGB Color object based on this HSLColor with a different
         *  Luminance value. The percent specified is an absolute value.
         *
         *  @param percent - the Luminance value between 0 - 100
         *  @return the RGB Color object
         */
        public Color adjustLuminance(float percent)
        {
            return toRGB(hsl[0], hsl[1], percent, alpha);
        }

        /**
         *  Create a RGB Color object based on this HSLColor with a different
         *  Saturation value. The percent specified is an absolute value.
         *
         *  @param percent - the Saturation value between 0 - 100
         *  @return the RGB Color object
         */
        public Color adjustSaturation(float percent)
        {
            return toRGB(hsl[0], percent, hsl[2], alpha);
        }

        /**
         *  Create a RGB Color object based on this HSLColor with a different
         *  Shade. Changing the shade will return a darker color. The percent
         *  specified is a relative value.
         *
         *  @param percent - the value between 0 - 100
         *  @return the RGB Color object
         */
        public Color adjustShade(float percent)
        {
            float multiplier = (100.0f - percent) / 100.0f;
            float l = Math.Max(0.0f, hsl[2] * multiplier);

            return toRGB(hsl[0], hsl[1], l, alpha);
        }

        /**
         *  Create a RGB Color object based on this HSLColor with a different
         *  Tone. Changing the tone will return a lighter color. The percent
         *  specified is a relative value.
         *
         *  @param percent - the value between 0 - 100
         *  @return the RGB Color object
         */
        public Color adjustTone(float percent)
        {
            float multiplier = (100.0f + percent) / 100.0f;
            float l = Math.Min(100.0f, hsl[2] * multiplier);

            return toRGB(hsl[0], hsl[1], l, alpha);
        }

        /**
         *  Get the Alpha value.
         *
         *  @return the Alpha value.
         */
        public float getAlpha()
        {
            return alpha;
        }

        /**
         *  Create a RGB Color object that is the complementary color of this
         *  HSLColor. This is a convenience method. The complementary color is
         *  determined by adding 180 degrees to the Hue value.
         *  @return the RGB Color object
         */
        public Color getComplementary()
        {
            float hue = (hsl[0] + 180.0f) % 360.0f;
            return toRGB(hue, hsl[1], hsl[2]);
        }

        /**
         *  Get the Hue value.
         *
         *  @return the Hue value.
         */
        public float getHue()
        {
            return hsl[0];
        }

        /**
         *  Get the HSL values.
         *
         *  @return the HSL values.
         */
        public float[] getHSL()
        {
            return hsl;
        }

        /**
         *  Get the Luminance value.
         *
         *  @return the Luminance value.
         */
        public float getLuminance()
        {
            return hsl[2];
        }

        /**
         *  Get the RGB Color object represented by this HDLColor.
         *
         *  @return the RGB Color object.
         */
        public Color getRGB()
        {
            return rgb;
        }

        /**
         *  Get the Saturation value.
         *
         *  @return the Saturation value.
         */
        public float getSaturation()
        {
            return hsl[1];
        }

        public String toString()
        {
            String toString =
                "HSLColor[h=" + hsl[0] +
                ",s=" + hsl[1] +
                ",l=" + hsl[2] +
                ",alpha=" + alpha + "]";

            return toString;
        }

        /**
         *  Convert a RGB Color to it corresponding HSL values.
         *
         *  @return an array containing the 3 HSL values.
         */
        public static float[] fromRGB(Color color)
	{
		//  Get RGB values in the range 0 - 1

		//float[] rgb = color.getRGBColorComponents( null );
            float r = color.R; //rgb[0];
            float g = color.G; //rgb[1];
            float b = color.B; //rgb[2];

		//	Minimum and Maximum RGB values are used in the HSL calculations

		float min = Math.Min(r, Math.Min(g, b));
		float max = Math.Max(r, Math.Max(g, b));

		//  Calculate the Hue

		float h = 0;

		if (max == min)
			h = 0;
		else if (max == r)
			h = ((60 * (g - b) / (max - min)) + 360) % 360;
		else if (max == g)
			h = (60 * (b - r) / (max - min)) + 120;
		else if (max == b)
			h = (60 * (r - g) / (max - min)) + 240;

		//  Calculate the Luminance

		float l = (max + min) / 2;
		//System.out.println(max + " : " + min + " : " + l);

		//  Calculate the Saturation

		float s = 0;

		if (max == min)
			s = 0;
		else if (l <= .5f)
			s = (max - min) / (max + min);
		else
			s = (max - min) / (2 - max - min);

		return new float[] {h, s * 100, l * 100};
	}

        /**
         *  Convert HSL values to a RGB Color with a default alpha value of 1.
         *  H (Hue) is specified as degrees in the range 0 - 360.
         *  S (Saturation) is specified as a percentage in the range 1 - 100.
         *  L (Lumanance) is specified as a percentage in the range 1 - 100.
         *
         *  @param hsl an array containing the 3 HSL values
         *
         *  @returns the RGB Color object
         */
        public static Color toRGB(float[] hsl)
        {
            return toRGB(hsl, 1.0f);
        }

        /**
         *  Convert HSL values to a RGB Color.
         *  H (Hue) is specified as degrees in the range 0 - 360.
         *  S (Saturation) is specified as a percentage in the range 1 - 100.
         *  L (Lumanance) is specified as a percentage in the range 1 - 100.
         *
         *  @param hsl    an array containing the 3 HSL values
         *  @param alpha  the alpha value between 0 - 1
         *
         *  @returns the RGB Color object
         */
        public static Color toRGB(float[] hsl, float alpha)
        {
            return toRGB(hsl[0], hsl[1], hsl[2], alpha);
        }

        /**
         *  Convert HSL values to a RGB Color with a default alpha value of 1.
         *
         *  @param h Hue is specified as degrees in the range 0 - 360.
         *  @param s Saturation is specified as a percentage in the range 1 - 100.
         *  @param l Lumanance is specified as a percentage in the range 1 - 100.
         *
         *  @returns the RGB Color object
         */
        public static Color toRGB(float h, float s, float l)
        {
            return toRGB(h, s, l, 1.0f);
        }

        /**
         *  Convert HSL values to a RGB Color.
         *
         *  @param h Hue is specified as degrees in the range 0 - 360.
         *  @param s Saturation is specified as a percentage in the range 1 - 100.
         *  @param l Lumanance is specified as a percentage in the range 1 - 100.
         *  @param alpha  the alpha value between 0 - 1
         *
         *  @returns the RGB Color object
         */
        public static Color toRGB(float h, float s, float l, float alpha)
        {
            //if (s < 0.0f || s > 100.0f)
            //{
            //    String message = "Color parameter outside of expected range - Saturation";
            //    throw new IllegalArgumentException(message);
            //}

            //if (l < 0.0f || l > 100.0f)
            //{
            //    String message = "Color parameter outside of expected range - Luminance";
            //    throw new IllegalArgumentException(message);
            //}

            //if (alpha < 0.0f || alpha > 1.0f)
            //{
            //    String message = "Color parameter outside of expected range - Alpha";
            //    throw new IllegalArgumentException(message);
            //}

            //  Formula needs all values between 0 - 1.

            h = h % 360.0f;
            h /= 360f;
            s /= 100f;
            l /= 100f;

            float q = 0;

            if (l < 0.5)
                q = l * (1 + s);
            else
                q = (l + s) - (s * l);

            float p = 2 * l - q;

            float r = Math.Max(0, HueToRGB(p, q, h + (1.0f / 3.0f)));
            float g = Math.Max(0, HueToRGB(p, q, h));
            float b = Math.Max(0, HueToRGB(p, q, h - (1.0f / 3.0f)));

            r = Math.Min(r, 1.0f);
            g = Math.Min(g, 1.0f);
            b = Math.Min(b, 1.0f);

            var rgb = new RGB((byte)r, (byte)g, (byte)b, (byte)alpha);
            
            return rgb.Color;
        }

        private static float HueToRGB(float p, float q, float h)
        {
            if (h < 0) h += 1;

            if (h > 1) h -= 1;

            if (6 * h < 1)
            {
                return p + ((q - p) * 6 * h);
            }

            if (2 * h < 1)
            {
                return q;
            }

            if (3 * h < 2)
            {
                return p + ((q - p) * 6 * ((2.0f / 3.0f) - h));
            }

            return p;
        }
    }
    
}
