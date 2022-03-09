using System.Collections.Generic;
using System.Drawing;
using SymaCord.TryOnMirror.Entities;

namespace SymaCord.TryOnMirror.CV
{
    public interface ICvStasmDetection
    {
        Dictionary<string, CvResult> SearchFacialFeatures(string imagePath, Rectangle roi);
        Dictionary<string, CvResult> SearchFacialFeatures(string imagePath);
        //List<CvSearchResult> SearchFacialFeatures(Image image, string imagePath);
        //List<CvSearchResult> SearchFacialFeatures(Image image, string imagePath, Rectangle roi);
    }
}