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
    
    public partial class BikeAd
    {
        public int adId { get; set; }
        public Nullable<short> year { get; set; }
        public Nullable<int> kmDriven { get; set; }
        public int bikeModel { get; set; }
        public Nullable<short> noOfOwners { get; set; }
        public Nullable<int> registeredCity { get; set; }
    
        public virtual Ad Ad { get; set; }
        public virtual BikeModel BikeModel1 { get; set; }
        public virtual City City { get; set; }
    }
}
