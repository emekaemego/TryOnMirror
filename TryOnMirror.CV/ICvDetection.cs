using System.Drawing;

namespace SymaCord.TryOnMirror.CV
{
    public interface ICvDetection
    {
        Rectangle DetectFace(Bitmap image, Rectangle roi);
        Rectangle DetectFace(Bitmap image);
        Rectangle DetectFace(string imageFilePath);
        Rectangle DetectLeftEye(Bitmap image, Rectangle roi);
        Rectangle DetectLeftEye(Bitmap image);
        Rectangle DetectLeftEye(string imageFilePath);
        Rectangle DetectRightEye(Bitmap image, Rectangle roi);
        Rectangle DetectRightEye(Bitmap image);
        Rectangle DetectRightEye(string imageFilePath);
        Rectangle DetectNose(Bitmap image, Rectangle roi);
        Rectangle DetectNose(Bitmap image);
        Rectangle DetectNose(string imageFilePath);
        Rectangle DetectMouth(Bitmap image, Rectangle roi);
        Rectangle DetectMouth(Bitmap image);
        Rectangle DetectMouth(string imageFilePath);
    }
}