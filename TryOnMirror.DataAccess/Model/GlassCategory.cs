//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SymaCord.TryOnMirror.DataAccess.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class GlassCategory
    {
        public GlassCategory()
        {
            this.Glasses = new HashSet<Glass>();
        }
    
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
    
        public virtual ICollection<Glass> Glasses { get; set; }
    }
}
