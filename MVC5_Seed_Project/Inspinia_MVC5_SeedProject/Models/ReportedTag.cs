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
    
    public partial class ReportedTag
    {
        public int Id { get; set; }
        public int tagId { get; set; }
        public string reportedBy { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
        public virtual Tag Tag { get; set; }
    }
}
