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
    
    public partial class CompanyOffice
    {
        public int Id { get; set; }
        public int companyId { get; set; }
        public Nullable<System.DateTime> since { get; set; }
        public Nullable<int> cityId { get; set; }
        public Nullable<int> popularPlaceId { get; set; }
        public string contactNo1 { get; set; }
        public string contactNo2 { get; set; }
        public string exectLocation { get; set; }
    
        public virtual City City { get; set; }
        public virtual Company Company { get; set; }
        public virtual popularPlace popularPlace { get; set; }
    }
}
