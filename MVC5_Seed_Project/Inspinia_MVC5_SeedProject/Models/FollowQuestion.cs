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
    
    public partial class FollowQuestion
    {
        public int Id { get; set; }
        public string followedBy { get; set; }
        public int questionId { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
        public virtual Question Question { get; set; }
    }
}
