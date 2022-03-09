using System.Drawing;

namespace SymaCord.TryOnMirror.UI.Web.ViewModels
{
    public class DetectionFeature
    {
        public string Id { get; set; }

        public Rectangle Rect { get; set; }

        public Point[] Points { get; set; }
    }
}