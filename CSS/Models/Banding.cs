//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CSS.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Banding
    {
        public int BandingId { get; set; }
        public Nullable<int> VolumeId { get; set; }
        public Nullable<int> Min { get; set; }
        public Nullable<int> Max { get; set; }
    
        public virtual Volume Volume { get; set; }
    }
}