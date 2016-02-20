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
    
    public partial class Ad
    {
        public Ad()
        {
            this.AdImages = new HashSet<AdImage>();
            this.AdTags = new HashSet<AdTag>();
            this.Bids = new HashSet<Bid>();
            this.Comments = new HashSet<Comment>();
            this.Reporteds = new HashSet<Reported>();
            this.SaveAds = new HashSet<SaveAd>();
            this.JobSkills = new HashSet<JobSkill>();
        }
    
        public int Id { get; set; }
        public string category { get; set; }
        public string postedBy { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public System.DateTime time { get; set; }
        public Nullable<int> price { get; set; }
        public string isnegotiable { get; set; }
        public string subcategory { get; set; }
        public Nullable<bool> type { get; set; }
        public string condition { get; set; }
        public string status { get; set; }
        public int views { get; set; }
    
        public virtual ICollection<AdImage> AdImages { get; set; }
        public virtual AdsLocation AdsLocation { get; set; }
        public virtual ICollection<AdTag> AdTags { get; set; }
        public virtual ICollection<Bid> Bids { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual LaptopAd LaptopAd { get; set; }
        public virtual MobileAd MobileAd { get; set; }
        public virtual ICollection<Reported> Reporteds { get; set; }
        public virtual ICollection<SaveAd> SaveAds { get; set; }
        public virtual AspNetUser AspNetUser { get; set; }
        public virtual CompanyAd CompanyAd { get; set; }
        public virtual JobAd JobAd { get; set; }
        public virtual ICollection<JobSkill> JobSkills { get; set; }
        public virtual CarAd CarAd { get; set; }
        public virtual BikeAd BikeAd { get; set; }
        public virtual House House { get; set; }
    }
}
