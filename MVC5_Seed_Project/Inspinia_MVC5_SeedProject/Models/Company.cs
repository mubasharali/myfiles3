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
    
    public partial class Company
    {
        public Company()
        {
            this.CompanyAds = new HashSet<CompanyAd>();
            this.CompanyOffices = new HashSet<CompanyOffice>();
            this.CompanyQuestions = new HashSet<CompanyQuestion>();
            this.CompanyTags = new HashSet<CompanyTag>();
            this.FollowCompanies = new HashSet<FollowCompany>();
            this.CompanyImages = new HashSet<CompanyImage>();
            this.Reviews = new HashSet<Review>();
        }
    
        public int Id { get; set; }
        public string title { get; set; }
        public string shortabout { get; set; }
        public string longabout { get; set; }
        public Nullable<System.DateTime> since { get; set; }
        public string contactNo1 { get; set; }
        public string contactNo2 { get; set; }
        public string email { get; set; }
        public string fblink { get; set; }
        public string twlink { get; set; }
        public string websitelink { get; set; }
        public string owner { get; set; }
        public string logoextension { get; set; }
        public string category { get; set; }
        public string createdBy { get; set; }
        public System.DateTime time { get; set; }
        public string status { get; set; }
        public Nullable<int> cityId { get; set; }
        public Nullable<int> popularPlaceId { get; set; }
        public string exectLocation { get; set; }
        public string rating { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
        public virtual City City { get; set; }
        public virtual popularPlace popularPlace { get; set; }
        public virtual ICollection<CompanyAd> CompanyAds { get; set; }
        public virtual ICollection<CompanyOffice> CompanyOffices { get; set; }
        public virtual ICollection<CompanyQuestion> CompanyQuestions { get; set; }
        public virtual ICollection<CompanyTag> CompanyTags { get; set; }
        public virtual ICollection<FollowCompany> FollowCompanies { get; set; }
        public virtual ICollection<CompanyImage> CompanyImages { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
    }
}
