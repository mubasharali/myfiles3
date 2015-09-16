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
    
    public partial class Question
    {
        public Question()
        {
            this.Answers = new HashSet<Answer>();
            this.QuestionReplies = new HashSet<QuestionReply>();
            this.QuestionViews = new HashSet<QuestionView>();
            this.QuestionVotes = new HashSet<QuestionVote>();
            this.FollowQuestions = new HashSet<FollowQuestion>();
            this.ReportedQuestions = new HashSet<ReportedQuestion>();
        }
    
        public int Id { get; set; }
        public string category { get; set; }
        public string subCategory { get; set; }
        public string postedBy { get; set; }
        public System.DateTime time { get; set; }
        public string title { get; set; }
        public string description { get; set; }
    
        public virtual ICollection<Answer> Answers { get; set; }
        public virtual AspNetUser AspNetUser { get; set; }
        public virtual ICollection<QuestionReply> QuestionReplies { get; set; }
        public virtual ICollection<QuestionView> QuestionViews { get; set; }
        public virtual ICollection<QuestionVote> QuestionVotes { get; set; }
        public virtual ICollection<FollowQuestion> FollowQuestions { get; set; }
        public virtual ICollection<ReportedQuestion> ReportedQuestions { get; set; }
    }
}
