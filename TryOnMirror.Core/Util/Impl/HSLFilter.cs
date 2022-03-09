using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymaCord.TryOnMirror.Core.Util.Impl
{
    public class HSLFilter : BasicFilter
    {

        private double _hue = 0.0;
        private double _saturation = 0.0;
        private double _lightness = 0.0;

        /// <summary>
        /// Get or set hue correction value.
        /// </summary>
        /// <value>Any double, will be scaled to range [0..360).</value>
        /// <returns>Double in range [0..360).</returns>
        public double Hue
        {
            get
            {
                return _hue;
            }
            set
            {
                _hue = value;
                while (_hue < 0.0)
                {
                    _hue += 360;
                }
                while (_hue >= 360.0)
                {
                    _hue -= 360;
                }
            }
        }

        /// <summary>
        /// Get or set saturation correction value.
        /// </summary>
        /// <value>Double in range [-100..+100]%.</value>
        /// <returns>Double in range [-100..+100]%.</returns>
        /// <remarks></remarks>
        public double Saturation
        {
            get
            {
                return _saturation;
            }
            set
            {
                if ((value >= -100.0) && (value <= 100.0))
                {
                    _saturation = value;
                }
            }
        }

        /// <summary>
        /// Get or set lightness correction value.
        /// </summary>
        /// <value>Double in range [-100..+100]%.</value>
        /// <returns>Double in range [-100..+100]%.</returns>
        /// <remarks></remarks>
        public double Lightness
        {
            get
            {
                return _lightness;
            }
            set
            {
                if ((value >= -100.0) && (value <= 100.0))
                {
                    _lightness = value;
                }
            }
        }

        /// <summary>
        /// Execute filter and return filtered image.
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public override System.Drawing.Image ExecuteFilter(System.Drawing.Image img)
        {
            switch (img.PixelFormat)
            {
                case PixelFormat.Format16bppGrayScale:
                    return img;
                case PixelFormat.Format24bppRgb:
                case PixelFormat.Format32bppArgb:
                case PixelFormat.Format32bppRgb:
                    return ExecuteRgb8(img);
                case PixelFormat.Format48bppRgb:
                    return img;
                default:
                    return img;
            }
        }

        /// <summary>
        /// Execute filter on (A)RGB image with 8 bits per color.
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private System.Drawing.Image ExecuteRgb8(System.Drawing.Image img)
        {
            const double c1o60 = 1 / 60;
            const double c1o255 = 1 / 255;
            Bitmap result = new Bitmap(img);
            result.SetResolution(img.HorizontalResolution, img.VerticalResolution);
            BitmapData bmpData = result.LockBits(new Rectangle(0, 0, result.Width, result.Height), ImageLockMode.ReadWrite, img.PixelFormat);
            int pixelBytes = System.Convert.ToInt32(System.Drawing.Image.GetPixelFormatSize(img.PixelFormat) / 8);
            //Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;
            int size = bmpData.Stride * result.Height;
            byte[] pixels = new byte[size - 1 + 1];
            int index = default(int);
            double R = default(double);
            double G = default(double);
            double B = default(double);
            double H = default(double);
            double S = default(double);
            double L = default(double);
            double H1 = default(double);
            double min = default(double);
            double max = default(double);
            double dif = default(double);
            double sum = default(double);
            double f1 = default(double);
            double f2 = default(double);
            double v1 = default(double);
            double v2 = default(double);
            double v3 = default(double);
            double sat = 127 * _saturation / 100;
            double lum = 127 * _lightness / 100;
            //Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(ptr, pixels, 0, size);
            //Main loop.
            for (int row = 0; row <= result.Height - 1; row++)
            {
                for (int col = 0; col <= result.Width - 1; col++)
                {
                    index = (row * bmpData.Stride) + (col * pixelBytes);
                    R = pixels[index + 2];
                    G = pixels[index + 1];
                    B = pixels[index + 0];
                    //Conversion to HSL space.
                    min = R;
                    if (G < min)
                    {
                        min = G;
                    }
                    if (B < min)
                    {
                        min = B;
                    }
                    max = R;
                    f1 = 0.0;
                    f2 = G - B;
                    if (G > max)
                    {
                        max = G;
                        f1 = 120.0;
                        f2 = B - R;
                    }
                    if (B > max)
                    {
                        max = B;
                        f1 = 240.0;
                        f2 = R - G;
                    }
                    dif = max - min;
                    sum = max + min;
                    L = 0.5 * sum;
                    if (dif == 0)
                    {
                        H = 0.0;
                        S = 0.0;
                    }
                    else
                    {
                        if (L < 127.5)
                        {
                            S = 255.0 * dif / sum;
                        }
                        else
                        {
                            S = 255.0 * dif / (510.0 - sum);
                        }
                        H = f1 + 60.0 * f2 / dif;
                        if (H < 0.0)
                        {
                            H += 360.0;
                        }
                        if (H >= 360.0)
                        {
                            H -= 360.0;
                        }
                    }
                    //Apply transformation.
                    H = H + _hue;
                    if (H >= 360.0)
                    {
                        H = H - 360.0;
                    }
                    S = S + sat;
                    if (S < 0.0)
                    {
                        S = 0.0;
                    }
                    if (S > 255.0)
                    {
                        S = 255.0;
                    }
                    L = L + lum;
                    if (L < 0.0)
                    {
                        L = 0.0;
                    }
                    if (L > 255.0)
                    {
                        L = 255.0;
                    }
                    //Conversion back to RGB space.
                    if (S == 0)
                    {
                        R = L;
                        G = L;
                        B = L;
                    }
                    else
                    {
                        if (L < 127.5)
                        {
                            v2 = c1o255 * L * (255 + S);
                        }
                        else
                        {
                            v2 = L + S - c1o255 * S * L;
                        }
                        v1 = 2 * L - v2;
                        v3 = v2 - v1;
                        H1 = H + 120.0;
                        if (H1 >= 360.0)
                        {
                            H1 -= 360.0;
                        }
                        if (H1 < 60.0)
                        {
                            R = v1 + v3 * H1 * c1o60;
                        }
                        else if (H1 < 180.0)
                        {
                            R = v2;
                        }
                        else if (H1 < 240.0)
                        {
                            R = v1 + v3 * (4 - H1 * c1o60);
                        }
                        else
                        {
                            R = v1;
                        }
                        H1 = H;
                        if (H1 < 60.0)
                        {
                            G = v1 + v3 * H1 * c1o60;
                        }
                        else if (H1 < 180.0)
                        {
                            G = v2;
                        }
                        else if (H1 < 240.0)
                        {
                            G = v1 + v3 * (4 - H1 * c1o60);
                        }
                        else
                        {
                            G = v1;
                        }
                        H1 = H - 120.0;
                        if (H1 < 0.0)
                        {
                            H1 += 360.0;
                        }
                        if (H1 < 60.0)
                        {
                            B = v1 + v3 * H1 * c1o60;
                        }
                        else if (H1 < 180.0)
                        {
                            B = v2;
                        }
                        else if (H1 < 240.0)
                        {
                            B = v1 + v3 * (4 - H1 * c1o60);
                        }
                        else
                        {
                            B = v1;
                        }
                    }
                    //Save new values.
                    pixels[index + 2] = (byte)R;
                    pixels[index + 1] = (byte)G;
                    pixels[index + 0] = (byte)B;
                }
            }
            //Copy the RGB values back to the bitmap
            System.Runtime.InteropServices.Marshal.Copy(pixels, 0, ptr, size);
            //Unlock the bits.
            result.UnlockBits(bmpData);
            return result;
        }









    }
}
