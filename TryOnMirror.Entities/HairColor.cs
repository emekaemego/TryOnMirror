//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SymaCord.TryOnMirror.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class HairColor
    {
        public int HairColorId { get; set; }
        public string ColorName { get; set; }
        public int Hue { get; set; }
        public double Saturation { get; set; }
        public double Luminance { get; set; }
        public System.DateTime DateCreated { get; set; }
        public byte[] TimeStamp { get; set; }
        public string ImageFileName { get; set; }
    }
}
