using System.Drawing;

namespace SymaCord.TryOnMirror.CV
{
    public interface IBlobDetection
    {
        Rectangle FindBiggestBlob(Bitmap originalImage, Rectangle roi, out System.Drawing.Point[] points);
        Rectangle FindBiggestBlob(string imageFillPath, Rectangle roi, out System.Drawing.Point[] points );
    }
}