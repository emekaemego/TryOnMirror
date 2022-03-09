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
    
    public partial class Hairstyle
    {
        public int HairstyleId { get; set; }
        public string FileName { get; set; }
        public string Title { get; set; }
        public double LeftDistance { get; set; }
        public double TopDistance { get; set; }
        public double WidthPercentage { get; set; }
        public double HeightPercentage { get; set; }
        public int UserId { get; set; }
        public System.DateTime DateCreated { get; set; }
        public byte[] TimeStamp { get; set; }
        public string ProductUrl { get; set; }
        public Nullable<int> BrandId { get; set; }
        public string PackageImageFileName { get; set; }
    
        public virtual UserProfile UserProfile { get; set; }
        public virtual Brand Brand { get; set; }
    }
}