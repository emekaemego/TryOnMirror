using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using AForge.Imaging;
using Kaliko.ImageLibrary;
using Image = System.Drawing.Image;

namespace SymaCord.TryOnMirror.Core.Imaging
{
    public static class Extension
    {
        public static Image ResizeByWidth(this Image image, int width)
        {
            float percentW = (image.Width/(float) width);
            int height = (int) (image.Height/percentW);

            var img = new KalikoImage(image);
            img = img.GetThumbnailImage(width, height, ThumbnailMethod.Fit);
            
            return img.Image;
        }

        public static Image Resize(this Image image, int width, int height)
        {
            int w = width;
            int h = height;

            var img = new KalikoImage(image);

            if (img.Height > img.Width)
            {
                float ratio = (img.Height/(float) height);
                w = (int) (img.Width/ratio);

                //Check if the width is divisible by 4, if not, make it to be divisible by 4
                if (w%4 != 0)
                {
                    w = (w/4)*4;
                }
            }
            else if (img.Width > img.Height)
            {
                float ratio = (img.Width / (float)width);
                h = (int)(img.Height / ratio);
            }

            var result = img.GetThumbnailImage(w, h, ThumbnailMethod.Fit);

            return result.Image;
        }

        public static Image ResizeByWidth(Bitmap img, int width)
        {
            float PercentW = ((float)img.Width / (float)width);

            Bitmap bmp = new Bitmap(width, (int)(img.Height / PercentW));
            Graphics g = Graphics.FromImage(bmp);

            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.DrawImage(img, 0, 0, bmp.Width, bmp.Height);
            g.Dispose();

            return bmp;
        }

        public static void SaveJpg(this Image image, string filePath, long quality)
        {
            var img = new KalikoImage((Bitmap)image.Clone());
            image.Dispose();

            img.SaveJpg(filePath, quality);
        }

        public static void SavePng(this Image image, string filePath, long quality)
        {
            var img = new KalikoImage((Bitmap)image.Clone());
            image.Dispose();

            img.SavePng(filePath, quality);
        }

        public static Image GetThumbnail(this Image image, int width, int height, ThumbnailMethod method)
        {
            var img = new KalikoImage(image);
            img = img.GetThumbnailImage(width, height, method);

            return img.Image;
        }

        public static Image GetThumbnail(this Image image, int width, int height, ThumbnailMethod method, Color backgroundColor)
        {
            var img = new KalikoImage(image);
            
            img.BackgroundColor = backgroundColor;
            img = img.GetThumbnailImage(width, height, method);

            return img.Image;
        }

        public static Color GetDominantColor(this Bitmap bmp)
        {
            //Lock Bits for processing
            BitmapData imageData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), 
                ImageLockMode.ReadOnly, bmp.PixelFormat);
            //Used for tally
            int r = 0;
            int g = 0;
            int b = 0;
            int count = 0;

            unsafe
            {
                //Count red and black pixels
                try
                {
                    var img = new UnmanagedImage(imageData);

                    int height = img.Height;
                    int width = img.Width;
                    int pixelSize = (img.PixelFormat == PixelFormat.Format24bppRgb) ? 3 : 4;
                    byte* p = (byte*)img.ImageData.ToPointer();

                    // for each line
                    for (int y = 0; y < height; y++)
                    {
                        // for each pixel
                        for (int x = 0; x < width; x++, p += pixelSize)
                        {
                            if(p[RGB.A] == 255)
                            {
                                r += p[RGB.R]; //Red pixel value
                                g += p[RGB.G]; //Green pixel value
                                b += p[RGB.B]; //Blue pixel value

                                count++;
                            }
                        }
                    }
                }
                finally
                {
                    bmp.UnlockBits(imageData); //Unlock
                }
            }

            //Calculate average
            r /= count;
            g /= count;
            b /= count;

            return Color.FromArgb(r, g, b);
        }
    }
}
