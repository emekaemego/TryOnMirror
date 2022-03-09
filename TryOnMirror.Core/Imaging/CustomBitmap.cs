using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace SymaCord.TryOnMirror.Core.Imaging
{
    public class CustomBitmap : IDisposable
    {
        public CustomBitmap(string filepath)
        {
            using (var bmp = new Bitmap(filepath)) this._bitmap = new Bitmap(bmp);
            this._path = System.IO.Path.GetFullPath(filepath);
            this._format = _bitmap.RawFormat;
        }

        public Bitmap Bitmap
        {
            get { return _bitmap; }
        }

        public string Path
        {
            get { return _path; }
        }

        private Bitmap _bitmap;
        private ImageFormat _format;
        private string _path;

        public void Save(string path)
        {
            _bitmap.Save(path, _format);
            this.Dispose();
        }

        public void Dispose()
        {
            if (_bitmap != null)
            {
                _bitmap.Dispose();
                _bitmap = null;
            }
        }
    }
}
