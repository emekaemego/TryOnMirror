using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
using SymaCord.TryOnMirror.Core.Imaging;
using SymaCord.TryOnMirror.Core.Imaging.AforgeFilters;
using SymaCord.TryOnMirror.UI.Web.ViewModels;

namespace SymaCord.TryOnMirror.UI.Web.Utils.Impl
{
    public class HairColoration
    {
        public static string ColorHair(string imagePath, int h, double s, double l)
        {
            string saveFullPath;

            using (var img = new Bitmap(imagePath))
            {
                // create filter sequence
                var filter = new FiltersSequence();

                var temp = img.Clone() as Bitmap; //Clone image to keep original image

                var hairIntensity = temp.GetDominantColor().GetBrightness();
                var inRange = new IntRange(0, 255);
                var outRange = new IntRange(0, 255);
                HSLImageTintFilter blackShade = null;

                if (hairIntensity >= 0.4)
                {
                    inRange = new IntRange(Convert.ToInt32(hairIntensity), 255);

                    if (hairIntensity >= 1)
                    {
                        blackShade = new HSLImageTintFilter(0, 0, 0.1f);
                    }
                    else if (hairIntensity >= 0.6)
                    {
                        blackShade = new HSLImageTintFilter(0, 0, 0.13f);
                    }
                    else if (hairIntensity >= 0.5)
                    {
                        blackShade = new HSLImageTintFilter(0, 0, 0.16f);
                    }
                    else if (hairIntensity >= 0.4)
                    {
                        blackShade = new HSLImageTintFilter(0, 0, 0.20f);
                    }
                }
                else if (hairIntensity >= 0.35f)
                {
                    inRange = new IntRange(0, 250);
                    outRange = new IntRange(7, 255);

                    blackShade = new HSLImageTintFilter(0, 0, 0.22f);
                }
                else if (hairIntensity >= 0.3)
                {
                    inRange = new IntRange(0, 240);
                    outRange = new IntRange(10, 255);

                    blackShade = new HSLImageTintFilter(0, 0, 0.25f);
                }
                else if (hairIntensity >= 0.25)
                {
                    inRange = new IntRange(0, 230);
                    outRange = new IntRange(15, 255);

                    blackShade = new HSLImageTintFilter(0, 0, 0.265f);
                }
                else if (hairIntensity >= 0.2)
                {
                    inRange = new IntRange(0, 220);
                    outRange = new IntRange(20, 255);

                    blackShade = new HSLImageTintFilter(0, 0, 0.28f);
                }
                else if (hairIntensity >= 0.15)
                {
                    inRange = new IntRange(0, 200);
                    outRange = new IntRange(35, 255);

                    blackShade = new HSLImageTintFilter(0, 0, 0.275f);
                }
                else if (hairIntensity >= 0.1)
                {
                    inRange = new IntRange(0, 180);
                    outRange = new IntRange(50, 255);

                    blackShade = new HSLImageTintFilter(0, 0, 0.30f);
                }
                else if (hairIntensity >= 0.01)
                {
                    inRange = new IntRange(0, 150);
                    outRange = new IntRange(60, 255);

                    blackShade = new HSLImageTintFilter(0, 0, 0.32f);
                }

                //if (sat.Mean < 0.05)
                //{
                var levelsFilter = new LevelsLinear();

                //input values
                levelsFilter.InRed = inRange; // new IntRange(50, 255);
                levelsFilter.InGreen = inRange; //new IntRange(50, 255);
                levelsFilter.InBlue = inRange; //new IntRange(50, 255);

                //output values
                levelsFilter.OutRed = outRange; //new IntRange(0, 255);
                levelsFilter.OutGreen = outRange; //new IntRange(0, 255);
                levelsFilter.OutBlue = outRange; //new IntRange(0, 255);

                //filter.Add(new BrightnessCorrection(-20));

                filter.Add(levelsFilter);

                if (l >= 0.5)
                {
                    filter.Add(blackShade);
                    //filter.Add(new HistogramEqualization());
                    temp = filter.Apply(temp);

                    var ou = 40;

                    var lFilter = new LevelsLinear
                    {
                        InRed = new IntRange(0, 170),
                        InGreen = new IntRange(0, 170),
                        InBlue = new IntRange(0, 170),

                        OutRed = new IntRange(ou, 255),
                        OutGreen = new IntRange(ou, 255),
                        OutBlue = new IntRange(ou, 255)
                    };

                    lFilter.ApplyInPlace(temp);
                }

                //temp.Save(HttpContext.Current.Server.MapPath("~/images/" + Guid.NewGuid() + ".png"), ImageFormat.Png);

                var hslFilter = new HSLImageTintFilter(h, (float)s, (float)l);
                hslFilter.ApplyInPlace(temp);

                //var texture = new Bitmap(HttpContext.Current.Server.MapPath("~/images/hair-texture.jpg"));

                //var tf = new FiltersSequence();
                //tf.Add(new HSLImageTintFilter(h, (float)s, (float)l));
                //tf.Add(new ResizeBicubic(50, 50));
                //texture = tf.Apply(texture);

                var fileName = Path.GetFileNameWithoutExtension(imagePath);
                saveFullPath = Helper.ConstructFilePath(fileName + "_" + h + "_" + s + "_" + ".png",
                                                            Helper.HairColorFilePath, false);

                Helper.CreateDirectoryIfNotExist(Path.GetDirectoryName(saveFullPath));

                if (File.Exists(saveFullPath))
                {
                    return saveFullPath;
                }

                //texture.Save(string.Format(HttpContext.Current.Server.MapPath("~/images/{0}-{1}-{2}.jpg"), h, s, l), ImageFormat.Jpeg);

                //HSLImageTintFilter.Logger(String.Format("Hair color brightness: {0}", hairIntensity));

                //filter.Add(new HueModifier(h));

                //if (filter.Count > 0)
                //{
                //    img = filter.Apply(img);
                //}

                //var rgb = new RGB();
                //HSL.ToRGB(new HSL(h, s, l), rgb);

                //temp = ColorMatrixImageTint.TintImage(temp, rgb);

                temp.Save(saveFullPath, ImageFormat.Png);
                temp.Dispose();
            }

            return saveFullPath;
        }

        public void Colorize(string imagePath)
        {
            //HttpContext.Current.Server.MapPath("~/images/hair-texture.jpg")
            var img = new Bitmap(imagePath);

            var filter = new FiltersSequence();
            // create filter
            //Add and set channels filter

            // gather statistics
            var stat = new ImageStatisticsHSL(img);
            var lum = stat.Luminance;
            var sat = stat.Saturation;

            var mean = sat.Mean; //HSLImageTintFilter.CalculateAverageLightness(img);

            //// get red channel's histogram

            if (sat.Mean < 0.05)
            {
                var levelsFilter = new LevelsLinear();

             //input values
            levelsFilter.InRed = new IntRange(0, 140);
            levelsFilter.InGreen = new IntRange(0, 140);
            levelsFilter.InBlue = new IntRange(0, 140);

            //    // output values
            levelsFilter.OutRed = new IntRange(0, 255);
            levelsFilter.OutGreen = new IntRange(0, 255);
            levelsFilter.OutBlue = new IntRange(0, 255);

            filter.Add(levelsFilter);

            //img = CorrectGamma(img, 0.8f); //SetGamma(1.87, img);
            }

            //img = CorrectGamma(img, 0.8f); //SetGamma(1.87, img);
            
            //filter.Add(new HueModifier(60));
            //filter.Add(new SaturationCorrection(0.86f));
            filter.Add(new HSLImageTintFilter(359, 0.53f, 0.22f));
            //filter.Add(new LuminanceModifier(0.93));
            //filter.Add(new HSLLinear { InLuminance = new Range(0, 0.63f) });

            // apply the filter
            if (filter.Count > 0)
            {
                img = filter.Apply(img);
            }

            //var hslFilter = new HSLImageTint.HSLFilter { Hue = 35, Saturation = 60, Lightness = 1 };
            //img = (Bitmap)hslFilter.ExecuteFilter(img);

            img.Save(HttpContext.Current.Server.MapPath("~/Images/" + Guid.NewGuid().ToString() + "_" + mean + "_.png"),
                     ImageFormat.Png);
            img.Dispose();
        }

        //public string SetColor(string imagePath, float hue, float saturation, float luminance)
        //{
            
        //}

        //public Bitmap SetGamma(double ratio, Bitmap image)
        //{
        //    return SetGamma(ratio, ratio, ratio, image);
        //}

        //public Bitmap SetGamma(double redRatio, double greenRatio, double blueRatio, Bitmap image)
        //{
        //    var bitmap = new Bitmap(image);

        //    var rectangle = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

        //    var bitmapData = bitmap.LockBits(rectangle, ImageLockMode.ReadWrite, bitmap.PixelFormat);
        //    IntPtr ptr = bitmapData.Scan0;
        //    int size = Math.Abs(bitmapData.Stride) * bitmap.Height;
        //    byte[] rgbValues = new byte[size];
        //    Marshal.Copy(ptr, rgbValues, 0, size);

        //    byte[] redGamma = new byte[256];
        //    byte[] greenGamma = new byte[256];
        //    byte[] blueGamma = new byte[256];

        //    double red = redRatio, blue = blueRatio, green = greenRatio;

        //    for (int i = 0; i < 256; ++i)
        //    {
        //        redGamma[i] = (byte)Math.Min(255, (int)((255.0
        //            * Math.Pow(i / 255.0, 1.0 * red)) + 0.5));
        //        greenGamma[i] = (byte)Math.Min(255, (int)((255.0
        //            * Math.Pow(i / 255.0, 1.0 * green)) + 0.5));
        //        blueGamma[i] = (byte)Math.Min(255, (int)((255.0
        //            * Math.Pow(i / 255.0, 1.0 * blue)) + 0.5));
        //    }

        //    for (int i = 0; i < rgbValues.Length; i++)
        //    {
        //        if (i % 3 == 0)
        //        { rgbValues[i] = redGamma[rgbValues[i]]; }
        //        else if (i % 3 == 2)
        //        { rgbValues[i] = greenGamma[rgbValues[i]]; }
        //        else
        //        { rgbValues[i] = blueGamma[rgbValues[i]]; }
        //    }

        //    Marshal.Copy(rgbValues, 0, ptr, size);

        //    bitmap.UnlockBits(bitmapData);

        //    return bitmap;
        //}

        public static Bitmap CorrectGamma(System.Drawing.Image source, float gamma)
        {
            var intermediate = new Bitmap(source.Width, source.Height, PixelFormat.Format32bppArgb);

            // Create an ImageAttributes object and set the gamma
            var imageAttr = new ImageAttributes();
            imageAttr.SetGamma(Convert.ToSingle(gamma));

            var rect = new Rectangle(0, 0, source.Width, source.Height);
            using (Graphics g = Graphics.FromImage(intermediate))
            {
                g.DrawImage(source, rect, 0, 0, source.Width, source.Height, GraphicsUnit.Pixel);
            }

            var corrected = new Bitmap(source.Width, source.Height, PixelFormat.Format32bppArgb);
            using (var g = Graphics.FromImage(corrected))
            {
                g.DrawImage(intermediate, rect, 0, 0, intermediate.Width, intermediate.Height, GraphicsUnit.Pixel, imageAttr);
            }

            intermediate.Dispose();
            return corrected;
        }

        public List<HairHslColorModel> GetHairHslColors()
        {
            var colors = new List<HairHslColorModel>
                {
                    new HairHslColorModel {Name = "", Hue = 0, Saturation = 10}
                };

            return colors;
        }

        //public void ColorImg(string imagePath)
        //{
        //    float blueShade = 218;
        //    float greenShade = 193;
        //    float redShade = 162;

        //    var sourceBitmap = new Bitmap(imagePath);

        //    BitmapData sourceData = sourceBitmap.LockBits(new Rectangle(0, 0,
        //                                                                sourceBitmap.Width, sourceBitmap.Height),
        //                                                  ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

        //    byte[] pixelBuffer = new byte[sourceData.Stride*sourceData.Height];

        //    Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);

        //    sourceBitmap.UnlockBits(sourceData);

        //    float blue = 0;
        //    float green = 0;
        //    float red = 0;

        //    for (int k = 0; k + 4 < pixelBuffer.Length; k += 4)
        //    {
        //        blue = pixelBuffer[k]*blueShade;
        //        green = pixelBuffer[k + 1]*greenShade;
        //        red = pixelBuffer[k + 2]*redShade;

        //        if (blue < 0)
        //        {
        //            blue = 0;
        //        }

        //        if (green < 0)
        //        {
        //            green = 0;
        //        }

        //        if (red < 0)
        //        {
        //            red = 0;
        //        }

        //        pixelBuffer[k] = (byte) blue;
        //        pixelBuffer[k + 1] = (byte) green;
        //        pixelBuffer[k + 2] = (byte) red;

        //    }

        //    Bitmap resultBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height);

        //    BitmapData resultData = resultBitmap.LockBits(new Rectangle(0, 0,
        //                                                                resultBitmap.Width, resultBitmap.Height),
        //                                                  ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

        //    Marshal.Copy(pixelBuffer, 0, resultData.Scan0, pixelBuffer.Length);
        //    resultBitmap.UnlockBits(resultData);
        //    resultBitmap.Save(HttpContext.Current.Server.MapPath("~/Images/" + Guid.NewGuid().ToString() + ".png"),
        //                      ImageFormat.Png);
        //}

        //private void MapImageLevels(string imagePath, int inLLimit, int inMLimit, int inRLimit,
        //                            int outLLimit, int outRLimit)
        //{
        //    //#if def_MapImageLevels

        //    // This array will hold the image's pixel data
        //    var bmp = new Bitmap(imagePath);

        //    byte[] ImageData;

        //    // Coordinate variables
        //    int x, y;
        //    //Constants required for creating a gamma curve from .1 to 10

        //    const double MAXGAMMA = 1.8460498941512;
        //    const double MIDGAMMA = 0.68377223398334;
        //    const double ROOT10 = 3.16227766;

        //    // Image dimensions
        //    int iWidth, iHeight;

        //    // Instantiate a FastDrawing class and gather the image's data (into ImageData())
        //    //FastDrawing fDraw = new FastDrawing();
        //    iWidth = bmp.Width; //fDraw.GetImageWidth(frmMain.InstancePtr.picBack);
        //    iHeight = bmp.Height; //fDraw.GetImageHeight(frmMain.InstancePtr.picBack);
        //    //fDraw.GetImageData2D(frmMain.InstancePtr.picBack, ImageData);

        //    // These variables will hold temporary pixel color values
        //    // VBto upgrade warning: R As int	OnWrite(byte)
        //    // VBto upgrade warning: G As int	OnWrite(byte)
        //    // VBto upgrade warning: B As int	OnWrite(byte)
        //    int R, G, B, L;

        //    // Look-up table for the midtone (gamma) leveled values
        //    double[] gValues = new double[255 + 1];

        //    // WARNING: This next chunk of code is a lot of messy math.  Don't worry too much
        //    // if you can't make sense of it ;)

        //    // Fill the gamma table with appropriate gamma values (from 10 to .1, ranged quadratically)
        //    // NOTE: This table is constant, and could be loaded from file instead of generated mathematically every time we run this function
        //    double gStep;
        //    gStep = (MAXGAMMA + MIDGAMMA)/127;
        //    for (x = 0; x <= 127; x++)
        //    {
        //        gValues[x] = (Convert.ToDouble(x)/127)*MIDGAMMA;
        //    } // x
        //    for (x = 128; x <= 255; x++)
        //    {
        //        gValues[x] = MIDGAMMA + (Convert.ToDouble(x - 127)*gStep);
        //    } // x
        //    for (x = 0; x <= 255; x++)
        //    {
        //        gValues[x] = 1/(Math.Pow((gValues[x] + 1/ROOT10), 2));
        //    } // x

        //    // Because we've built our look-up tables on a 0-255 scale, correct the inMLimit
        //    // value (from the midtones scroll bar) to simply represent a ratio on that scale
        //    double tRatio;
        //    tRatio = (double) (inMLimit - inLLimit)/(inRLimit - inLLimit);
        //    tRatio *= 255;
        //    // Then convert that ratio into a byte (so we can access a look-up table with it)
        //    byte bRatio;
        //    bRatio = Convert.ToByte(Convert.ToBoolean(tRatio)); //VBtoConverter.CByte(Convert.ToBoolean(tRatio));

        //    // Calculate a look-up table of gamma-corrected values based on the midtones scrollbar
        //    // VBto upgrade warning: gLevels As byte	OnWrite(double)
        //    byte[] gLevels = new byte[255 + 1];
        //    double tmpGamma;
        //    for (x = 0; x <= 255; x++)
        //    {
        //        tmpGamma = Convert.ToDouble(x)/255;
        //        tmpGamma = Math.Pow(tmpGamma, (1/gValues[bRatio]));
        //        tmpGamma *= 255;
        //        if (tmpGamma > 255)
        //        {
        //            tmpGamma = 255;
        //        }
        //        else if (tmpGamma < 0)
        //        {
        //            tmpGamma = 0;
        //        }
        //        gLevels[x] = (byte)tmpGamma;
        //    } // x

        //    // Look-up table for the input leveled values
        //    // VBto upgrade warning: newLevels As byte	OnWrite(int, byte)
        //    byte[] newLevels = new byte[255 + 1];

        //    // Fill the look-up table with appropriately mapped input limits
        //    float pStep;
        //    pStep = 255/(Convert.ToSingle(inRLimit) - Convert.ToSingle(inLLimit));
        //    for (x = 0; x <= 255; x++)
        //    {
        //        if (x < inLLimit)
        //        {
        //            newLevels[x] = 0;
        //        }
        //        else if (x > inRLimit)
        //        {
        //            newLevels[x] = 255;
        //        }
        //        else
        //        {
        //            newLevels[x] = ByteMe(Convert.ToInt32(((Convert.ToSingle(x) - Convert.ToSingle(inLLimit))*pStep)));
        //        }
        //    } // x

        //    // Now run all input-mapped values through our midtone-correction look-up
        //    for (x = 0; x <= 255; x++)
        //    {
        //        newLevels[x] = gLevels[newLevels[x]];
        //    } // x

        //    // Last of all, remap all image values to match the user-specified output limits
        //    // VBto upgrade warning: oStep As double	OnWrite(float)

        //    //double oStep;
        //    //oStep = (Convert.ToSingle(outRLimit) - Convert.ToSingle(outLLimit))/255;
        //    //for (x = 0; x <= 255; x++)
        //    //{
        //    //    newLevels[x] =
        //    //        ByteMe(Convert.ToInt32(Convert.ToSingle(outLLimit) + (Convert.ToSingle(newLevels[x])*oStep)));
        //    //} // x

        //    var srcData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
        //                                                  ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

        //    ImageData = new byte[srcData.Stride * srcData.Height];

        //    Marshal.Copy(srcData.Scan0, ImageData, 0, ImageData.Length);

        //    bmp.UnlockBits(srcData);


        //    // Now run a quick loop through the image, adjusting pixel values with the look-up tables
        //    int QuickX;
        //    for (x = 0; x <= iWidth - 1; x++)
        //    {
        //        QuickX = x*3;
        //        for (y = 0; y <= iHeight - 1; y++)
        //        {
        //            // Grab red, green, and blue
        //            R = ImageData[QuickX + 2, y];
        //            G = ImageData[QuickX + 1, y];
        //            B = ImageData[QuickX, y];
        //            // Correct them all
        //            ImageData[QuickX + 2, y] = newLevels[R];
        //            ImageData[QuickX + 1, y] = newLevels[G];
        //            ImageData[QuickX, y] = newLevels[B];
        //        } // y
        //    } // x

        //    // Draw the new image data to the screen
        //    //fDraw.SetImageData2D(picMain, iWidth, iHeight, ImageData);

        //    //#endif	// def_MapImageLevels
        //}

        //// Used to restrict values to the (0-255) range
        //// VBto upgrade warning: val As int	OnWrite(double)
        //private byte ByteMe(long val)
        //{
        //    byte ByteMe = 0;
        //    //#if def_ByteMe
        //    if (val > 255)
        //    {
        //        ByteMe = 255;
        //    }
        //    else if (val < 0)
        //    {
        //        ByteMe = 0;
        //    }
        //    else
        //    {
        //        ByteMe = (byte)val;
        //    }
        //    //#endif
        //    // def_ByteMe
        //    return ByteMe;
        //}
    }
}