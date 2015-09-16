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
    
    public partial class AspNetUser
    {
        public AspNetUser()
        {
            this.AspNetUserClaims = new HashSet<AspNetUserClaim>();
            this.AspNetUserLogins = new HashSet<AspNetUserLogin>();
            this.AspNetRoles = new HashSet<AspNetRole>();
            this.Comments = new HashSet<Comment>();
            this.CommentReplies = new HashSet<CommentReply>();
            this.CommentReplyVotes = new HashSet<CommentReplyVote>();
            this.CommentVotes = new HashSet<CommentVote>();
            this.Answers = new HashSet<Answer>();
            this.AnswerReplies = new HashSet<AnswerReply>();
            this.AnswerReplyVotes = new HashSet<AnswerReplyVote>();
            this.AnswerVotes = new HashSet<AnswerVote>();
            this.Questions = new HashSet<Question>();
            this.QuestionReplies = new HashSet<QuestionReply>();
            this.QuestionReplyVotes = new HashSet<QuestionReplyVote>();
            this.QuestionViews = new HashSet<QuestionView>();
            this.QuestionVotes = new HashSet<QuestionVote>();
            this.Reporteds = new HashSet<Reported>();
            this.Bids = new HashSet<Bid>();
            this.AdViews = new HashSet<AdView>();
            this.Ads = new HashSet<Ad>();
            this.Chats = new HashSet<Chat>();
            this.Chats1 = new HashSet<Chat>();
            this.FollowQuestions = new HashSet<FollowQuestion>();
            this.ReportedQuestions = new HashSet<ReportedQuestion>();
        }
    
        public string Id { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public Nullable<System.DateTime> LockoutEndDateUtc { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        public string UserName { get; set; }
        public Nullable<System.DateTime> since { get; set; }
        public Nullable<int> reputation { get; set; }
        public Nullable<bool> hideEmail { get; set; }
        public Nullable<bool> hidePhoneNumber { get; set; }
        public string dpExtension { get; set; }
        public Nullable<System.DateTime> dateOfBirth { get; set; }
        public string shortAbout { get; set; }
        public string longAbout { get; set; }
        public string city { get; set; }
        public string gender { get; set; }
        public Nullable<bool> hideDateOfBirth { get; set; }
    
        public virtual ICollection<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual ICollection<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual ICollection<AspNetRole> AspNetRoles { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<CommentReply> CommentReplies { get; set; }
        public virtual ICollection<CommentReplyVote> CommentReplyVotes { get; set; }
        public virtual ICollection<CommentVote> CommentVotes { get; set; }
        public virtual ICollection<Answer> Answers { get; set; }
        public virtual ICollection<AnswerReply> AnswerReplies { get; set; }
        public virtual ICollection<AnswerReplyVote> AnswerReplyVotes { get; set; }
        public virtual ICollection<AnswerVote> AnswerVotes { get; set; }
        public virtual ICollection<Question> Questions { get; set; }
        public virtual ICollection<QuestionReply> QuestionReplies { get; set; }
        public virtual ICollection<QuestionReplyVote> QuestionReplyVotes { get; set; }
        public virtual ICollection<QuestionView> QuestionViews { get; set; }
        public virtual ICollection<QuestionVote> QuestionVotes { get; set; }
        public virtual ICollection<Reported> Reporteds { get; set; }
        public virtual ICollection<Bid> Bids { get; set; }
        public virtual ICollection<AdView> AdViews { get; set; }
        public virtual ICollection<Ad> Ads { get; set; }
        public virtual ICollection<Chat> Chats { get; set; }
        public virtual ICollection<Chat> Chats1 { get; set; }
        public virtual ICollection<FollowQuestion> FollowQuestions { get; set; }
        public virtual ICollection<ReportedQuestion> ReportedQuestions { get; set; }
    }
}
