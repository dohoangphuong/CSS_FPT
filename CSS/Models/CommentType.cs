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
    
    public partial class CommentType
    {
        public CommentType()
        {
            this.Comments = new HashSet<Comment>();
        }
    
        public int CommentTypeId { get; set; }
        public string CommentType1 { get; set; }
    
        public virtual ICollection<Comment> Comments { get; set; }
    }
}
