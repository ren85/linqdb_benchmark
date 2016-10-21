using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackData
{
    public class Question
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public byte[] Body { get; set; }
        public DateTime CreationDate { get; set; }
        public int Score { get; set; }
        public int ViewCount { get; set; }
        public int? OwnerUserId { get; set; }
        public int? LastEditorUserId { get; set; }
        public string LastEditorDisplayName { get; set; }
        public DateTime? LastEditDate { get; set; }
        public DateTime? LastActivityDate { get; set; }
        public string Tags { get; set; }
        public int AnswerCount { get; set; }
        public int CommentCount { get; set; }
        public int? FavoriteCount { get; set; }
        public DateTime? CommunityOwnedDate { get; set; }
        public int? AcceptedAnswerId { get; set; }
    }

    public class Answer
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public byte[] Body { get; set; }
        public DateTime CreationDate { get; set; }
        public int Score { get; set; }
        public int? OwnerUserId { get; set; }
        public int? LastEditorUserId { get; set; }
        public DateTime? LastEditDate { get; set; }
        public DateTime? LastActivityDate { get; set; }
        public int CommentCount { get; set; }
    }

    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
        public int? ExcerptPostId { get; set; }
        public int? WikiPostId { get; set; }
    }

    public class QuestionTags
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public int TagId { get; set; }
    }

    public class User
    {
        public int Id { get; set; }
        public int Reputation { get; set; }
        public DateTime CreationDate { get; set; }
        public string DisplayName { get; set; }
        public DateTime LastAccessDate { get; set; }
        public string WebsiteUrl { get; set; }
        public string Location { get; set; }
        public byte[] AboutMe { get; set; }
        public int Views { get; set; }
        public int UpVotes { get; set; }
        public int DownVotes { get; set; }
        public int? AccountId { get; set; }
    }

    public class Comment
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public int Score { get; set; }
        public byte[] Text { get; set; }
        public DateTime CreationDate { get; set; }
        public int? UserId { get; set; }
    } 
}
