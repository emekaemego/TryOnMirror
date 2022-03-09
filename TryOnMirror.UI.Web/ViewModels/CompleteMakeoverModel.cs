using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SymaCord.TryOnMirror.UI.Web.ViewModels
{
    public class CompleteMakeoverModel
    {
        public string PhotoFileName { get; set; }

        public byte[] ImageData { get; set; }

        public string Provider { get; set; }
    }
}