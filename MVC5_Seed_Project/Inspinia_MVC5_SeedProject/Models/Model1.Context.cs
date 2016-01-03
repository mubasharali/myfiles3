﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class Entities : DbContext
    {
        public Entities()
            : base("name=Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<C__MigrationHistory> C__MigrationHistory { get; set; }
        public virtual DbSet<Ad> Ads { get; set; }
        public virtual DbSet<AdImage> AdImages { get; set; }
        public virtual DbSet<AdsLocation> AdsLocations { get; set; }
        public virtual DbSet<AdTag> AdTags { get; set; }
        public virtual DbSet<Answer> Answers { get; set; }
        public virtual DbSet<AnswerReply> AnswerReplies { get; set; }
        public virtual DbSet<AnswerReplyVote> AnswerReplyVotes { get; set; }
        public virtual DbSet<AnswerVote> AnswerVotes { get; set; }
        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<Bid> Bids { get; set; }
        public virtual DbSet<CarAd> CarAds { get; set; }
        public virtual DbSet<CarBrand> CarBrands { get; set; }
        public virtual DbSet<CarModel> CarModels { get; set; }
        public virtual DbSet<Chat> Chats { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<CommentReply> CommentReplies { get; set; }
        public virtual DbSet<CommentReplyVote> CommentReplyVotes { get; set; }
        public virtual DbSet<CommentVote> CommentVotes { get; set; }
        public virtual DbSet<FollowQuestion> FollowQuestions { get; set; }
        public virtual DbSet<FollowTag> FollowTags { get; set; }
        public virtual DbSet<Friend> Friends { get; set; }
        public virtual DbSet<LaptopAd> LaptopAds { get; set; }
        public virtual DbSet<LaptopBrand> LaptopBrands { get; set; }
        public virtual DbSet<LaptopModel> LaptopModels { get; set; }
        public virtual DbSet<Mobile> Mobiles { get; set; }
        public virtual DbSet<MobileAd> MobileAds { get; set; }
        public virtual DbSet<MobileModel> MobileModels { get; set; }
        public virtual DbSet<popularPlace> popularPlaces { get; set; }
        public virtual DbSet<Question> Questions { get; set; }
        public virtual DbSet<QuestionReply> QuestionReplies { get; set; }
        public virtual DbSet<QuestionReplyVote> QuestionReplyVotes { get; set; }
        public virtual DbSet<QuestionTag> QuestionTags { get; set; }
        public virtual DbSet<QuestionVote> QuestionVotes { get; set; }
        public virtual DbSet<Reported> Reporteds { get; set; }
        public virtual DbSet<ReportedQuestion> ReportedQuestions { get; set; }
        public virtual DbSet<ReportedTag> ReportedTags { get; set; }
        public virtual DbSet<Tag> Tags { get; set; }
        public virtual DbSet<City> Cities { get; set; }
        public virtual DbSet<SaveAd> SaveAds { get; set; }
        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<CompanyAnswerReply> CompanyAnswerReplies { get; set; }
        public virtual DbSet<CompanyImage> CompanyImages { get; set; }
        public virtual DbSet<CompanyOffice> CompanyOffices { get; set; }
        public virtual DbSet<CompanyQuestion> CompanyQuestions { get; set; }
        public virtual DbSet<FollowCompany> FollowCompanies { get; set; }
        public virtual DbSet<Review> Reviews { get; set; }
        public virtual DbSet<ReviewReply> ReviewReplies { get; set; }
        public virtual DbSet<ReviewVote> ReviewVotes { get; set; }
        public virtual DbSet<CompanyAnswer> CompanyAnswers { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<CompanyTag> CompanyTags { get; set; }
    }
}
