using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using AForge.Imaging;
using Image = System.Drawing.Image;

namespace SymaCord.TryOnMirror.Core.Imaging
{
   public class ColorMatrixImageTint
    {
       public static Bitmap TintImage(Image image, RGB color)
       {
           using (var stream = new MemoryStream())
           {
               image.Save(stream, ImageFormat.Png);
               stream.Close();

               Byte[] bytes = stream.ToArray();
               
               //Call the method here
               var newImage = ImageManipulator.ShadeColor(bytes, color.Color);

               return new Bitmap(Image.FromStream(newImage));
           }  
       }
    }
}
