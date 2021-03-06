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
    
    public partial class TracedPhoto
    {
        public long TraceId { get; set; }
        public string FileName { get; set; }
        public string PhotoTitle { get; set; }
        public string FaceCoordinates { get; set; }
        public string LeftEyeCoordinates { get; set; }
        public string LeftEyeBallPupilCoord { get; set; }
        public Nullable<int> LeftEyeballRadius { get; set; }
        public string LeftEyeBrowCoordinates { get; set; }
        public string RightEyeCoordinates { get; set; }
        public string RightEyeBallPupilCoord { get; set; }
        public Nullable<int> RightEyeballRadius { get; set; }
        public string NoseCoordinates { get; set; }
        public string LipsCoordinates { get; set; }
        public string OpenLipsCoodinate { get; set; }
        public bool IsModel { get; set; }
        public Nullable<int> UserId { get; set; }
        public Nullable<System.Guid> AnonymouseId { get; set; }
        public System.DateTime DateCreated { get; set; }
        public byte[] TimeStamp { get; set; }
        public bool OpenLips { get; set; }
        public string RightEyeBrowCoordinates { get; set; }
        public string GlassCoordinates { get; set; }
    
        public virtual UserProfile UserProfile { get; set; }
    }
}
