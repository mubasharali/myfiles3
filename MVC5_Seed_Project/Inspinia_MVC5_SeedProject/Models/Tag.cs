//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Inspinia_MVC5_SeedProject.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Tag
    {
        public Tag()
        {
            this.FollowTags = new HashSet<FollowTag>();
            this.QuestionTags = new HashSet<QuestionTag>();
        }
    
        public int Id { get; set; }
        public string name { get; set; }
        public string info { get; set; }
        public Nullable<System.DateTime> time { get; set; }
        public string createdBy { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
        public virtual ICollection<FollowTag> FollowTags { get; set; }
        public virtual ICollection<QuestionTag> QuestionTags { get; set; }
    }
}
