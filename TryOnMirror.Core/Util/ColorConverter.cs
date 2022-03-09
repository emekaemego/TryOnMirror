using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymaCord.TryOnMirror.Core.Util
{
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
    public class HSLColor
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
        public HSLColor() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="HSL"/> class.
        /// </summary>
        /// 
        /// <param name="hue">Hue component.</param>
        /// <param name="saturation">Saturation component.</param>
        /// <param name="luminance">Luminance component.</param>
        /// 
        public HSLColor(int hue, float saturation, float luminance)
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
        public static void FromRGB(RGB rgb, HSLColor hsl)
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
        public static HSLColor FromRGB(RGB rgb)
        {
            var hsl = new HSLColor();
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
        public static void ToRGB(HSLColor hsl, RGB rgb)
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


            double r = 0, g = 0, b = 0;
            if (hsl.Luminance != 0)
            {
                if (hsl.Saturation == 0)
                    r = g = b = hsl.Luminance;
                else
                {
                    double temp2 = GetTemp2(hsl);
                    double temp1 = 2.0 * hsl.Luminance - temp2;
                    double hue = hsl.Hue/100;
                    r = GetColorComponent(temp1, temp2, hue + 1.0 / 3.0);
                    g = GetColorComponent(temp1, temp2, hue);
                    b = GetColorComponent(temp1, temp2, hue - 1.0 / 3.0);
                }
            }

            rgb.Red = (byte) (255*r);
            rgb.Green = (byte) (255*g); 
            rgb.Blue = (byte)(255 * b);
        }

        private static double ColorCalc(double c, double t1, double t2)
        {

            if (c < 0) c += 1d;
            if (c > 1) c -= 1d;
            if (6.0d * c < 1.0d) return t1 + (t2 - t1) * 6.0d * c;
            if (2.0d * c < 1.0d) return t2;
            if (3.0d * c < 2.0d) return t1 + (t2 - t1) * (2.0d / 3.0d - c) * 6.0d;
            return t1;
        }

        private static double GetColorComponent(double temp1, double temp2, double temp3)
        {
            temp3 = MoveIntoRange(temp3);
            if (temp3 < 1.0 / 6.0)
                return temp1 + (temp2 - temp1) * 6.0 * temp3;
            else if (temp3 < 0.5)
                return temp2;
            else if (temp3 < 2.0 / 3.0)
                return temp1 + ((temp2 - temp1) * ((2.0 / 3.0) - temp3) * 6.0);
            else
                return temp1;
        }

        private static double MoveIntoRange(double temp3)
        {
            if (temp3 < 0.0)
                temp3 += 1.0;
            else if (temp3 > 1.0)
                temp3 -= 1.0;
            return temp3;
        }

        private static double GetTemp2(HSLColor hslColor)
        {
            double temp2;
            if (hslColor.Luminance < 0.5)  //<=??
                temp2 = hslColor.Luminance * (1.0 + hslColor.Saturation);
            else
                temp2 = hslColor.Luminance + hslColor.Saturation - (hslColor.Luminance * hslColor.Saturation);
            return temp2;
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
}
