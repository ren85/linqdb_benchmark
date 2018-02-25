using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using System.Xml.Linq;
using System.Xml;
using StackData;
//using LinqDb;
using LinqdbClient;

namespace ImportStack
{
    class ImportStackoverflow
    {
        public static Question GetQuestion(XElement xe)
        {
            return new Question()
            {
                Id = Convert.ToInt32(xe.Attribute("Id").Value),
                Title = xe.Attribute("Title").Value.ToString(),
                Body = Encoding.UTF8.GetBytes(xe.Attribute("Body").Value.ToString()),
                CreationDate = Convert.ToDateTime(xe.Attribute("CreationDate").Value),
                Score = Convert.ToInt32(xe.Attribute("Score").Value),
                ViewCount = Convert.ToInt32(xe.Attribute("ViewCount").Value),
                OwnerUserId = xe.Attribute("OwnerUserId") != null ? Convert.ToInt32(xe.Attribute("OwnerUserId").Value) : (int?)null,
                LastEditorUserId = xe.Attribute("LastEditorUserId") != null ? Convert.ToInt32(xe.Attribute("LastEditorUserId").Value) : (int?)null,
                LastEditorDisplayName = xe.Attribute("LastEditorDisplayName") != null ? xe.Attribute("LastEditorDisplayName").Value.ToString() : (string)null,
                LastEditDate = xe.Attribute("LastEditDate") != null ? Convert.ToDateTime(xe.Attribute("LastEditDate").Value) : (DateTime?)null,
                LastActivityDate = xe.Attribute("LastActivityDate") != null ? Convert.ToDateTime(xe.Attribute("LastActivityDate").Value) : (DateTime?)null,
                AnswerCount = Convert.ToInt32(xe.Attribute("AnswerCount").Value),
                CommentCount = Convert.ToInt32(xe.Attribute("CommentCount").Value),
                FavoriteCount = xe.Attribute("FavoriteCount") != null ? Convert.ToInt32(xe.Attribute("FavoriteCount").Value) : (int?)null,
                CommunityOwnedDate = xe.Attribute("CommunityOwnedDate") != null ? Convert.ToDateTime(xe.Attribute("CommunityOwnedDate").Value) : (DateTime?)null,
                AcceptedAnswerId = xe.Attribute("AcceptedAnswerId") != null ? Convert.ToInt32(xe.Attribute("AcceptedAnswerId").Value) : (int?)null,
                Tags = xe.Attribute("Tags").Value
            };
        }
        public static Answer GetAnswer(XElement xe)
        {
            return new Answer()
            {
                Id = Convert.ToInt32(xe.Attribute("Id").Value),
                ParentId = Convert.ToInt32(xe.Attribute("ParentId").Value),
                Body = Encoding.UTF8.GetBytes(xe.Attribute("Body").Value.ToString()),
                CreationDate = Convert.ToDateTime(xe.Attribute("CreationDate").Value),
                Score = Convert.ToInt32(xe.Attribute("Score").Value),
                OwnerUserId = xe.Attribute("OwnerUserId") != null ? Convert.ToInt32(xe.Attribute("OwnerUserId").Value) : (int?)null,
                LastEditorUserId = xe.Attribute("LastEditorUserId") != null ? Convert.ToInt32(xe.Attribute("LastEditorUserId").Value) : (int?)null,
                LastEditDate = xe.Attribute("LastEditDate") != null ? Convert.ToDateTime(xe.Attribute("LastEditDate").Value) : (DateTime?)null,
                LastActivityDate = xe.Attribute("LastActivityDate") != null ? Convert.ToDateTime(xe.Attribute("LastActivityDate").Value) : (DateTime?)null,
                CommentCount = Convert.ToInt32(xe.Attribute("CommentCount").Value)
            };
        }
        public static Tag GetTag(XElement xe)
        {
            return new Tag()
            {
                Id = Convert.ToInt32(xe.Attribute("Id").Value),
                Name = xe.Attribute("TagName").Value.ToString(),
                Count = Convert.ToInt32(xe.Attribute("Count").Value),
                ExcerptPostId = xe.Attribute("ExcerptPostId") != null ? Convert.ToInt32(xe.Attribute("ExcerptPostId").Value) : (int?)null,
                WikiPostId = xe.Attribute("WikiPostId") != null ? Convert.ToInt32(xe.Attribute("WikiPostId").Value) : (int?)null
            };
        }
        public static User GetUser(XElement xe)
        {
            return new User()
            {
                Id = Convert.ToInt32(xe.Attribute("Id").Value),
                Reputation = Convert.ToInt32(xe.Attribute("Reputation").Value),
                DisplayName = xe.Attribute("DisplayName").Value.ToString(),
                AccountId = xe.Attribute("AccountId") != null ? Convert.ToInt32(xe.Attribute("AccountId").Value) : (int?)null,
                AboutMe = xe.Attribute("AboutMe") != null ? xe.Attribute("AboutMe").Value.ToString() : null,
                CreationDate = Convert.ToDateTime(xe.Attribute("CreationDate").Value),
                DownVotes = Convert.ToInt32(xe.Attribute("DownVotes").Value),
                UpVotes = Convert.ToInt32(xe.Attribute("UpVotes").Value),
                LastAccessDate = Convert.ToDateTime(xe.Attribute("LastAccessDate").Value),
                Location = xe.Attribute("Location") != null ?  xe.Attribute("Location").Value.ToString() : null,
                Views = Convert.ToInt32(xe.Attribute("Views").Value),
                WebsiteUrl = xe.Attribute("WebsiteUrl") != null ? xe.Attribute("WebsiteUrl").Value.ToString() : null
            };
        }
        public static Comment GetComment(XElement xe)
        {
            return new Comment()
            {
                Id = Convert.ToInt32(xe.Attribute("Id").Value),
                CreationDate = Convert.ToDateTime(xe.Attribute("CreationDate").Value),
                PostId = Convert.ToInt32(xe.Attribute("PostId").Value),
                Score = Convert.ToInt32(xe.Attribute("Score").Value),
                Text = Encoding.UTF8.GetBytes(xe.Attribute("Text").Value.ToString()),
                UserId = xe.Attribute("UserId") != null ? Convert.ToInt32(xe.Attribute("UserId").Value) : (int?)null
            };
        }
        

        static void Main(string[] args)
        {
            Import(@"C:\Users\Administrator\Downloads\", @"");
        }

        static void Import(string base_path, string DB_DATA)
        {
            var db = new Db("5.175.13.5:2055", "admin", "admin");
            var questions = new List<Question>();
            var answers = new List<Answer>();
            int totalq = 0, totala = 0;
            DateTime start = DateTime.Now;

            //questions, answers
            foreach (var row in EnumerateRows(Path.Combine(base_path, "Posts.xml")))
            {
                var type = Convert.ToInt32(row.Attribute("PostTypeId").Value);
                if (type == 1)
                {
                    totalq++;
                    questions.Add(GetQuestion(row));
                    if (questions.Count() > 30000)
                    {
                        db.Table<Question>().SaveBatch(questions);
                        questions = new List<Question>();
                    }
                }
                else if (type == 2)
                {
                    totala++;
                    answers.Add(GetAnswer(row));
                    if (answers.Count() > 30000)
                    {
                        db.Table<Answer>().SaveBatch(answers);
                        answers = new List<Answer>();
                    }
                }
            }
            if (questions.Any())
            {
                db.Table<Question>().SaveBatch(questions);
                questions = new List<Question>();
            }
            if (answers.Any())
            {
                db.Table<Answer>().SaveBatch(answers);
                answers = new List<Answer>();
            }

            Console.WriteLine("QA done");
            //tags
            Dictionary<string, int> tag_dic = new Dictionary<string, int>();
            var tags = new List<Tag>();
            foreach (var row in EnumerateRows(Path.Combine(base_path, "Tags.xml")))
            {
                tags.Add(GetTag(row));
                if (tags.Count() > 30000)
                {
                    db.Table<Tag>().SaveBatch(tags);
                    foreach (var t in tags)
                    {
                        tag_dic[t.Name] = t.Id;
                    }
                    tags = new List<Tag>();
                }
            }
            if (tags.Any())
            {
                db.Table<Tag>().SaveBatch(tags);
                foreach (var t in tags)
                {
                    tag_dic[t.Name] = t.Id;
                }
                tags = new List<Tag>();
            }

            //question's tags
            int bsize = 250000;
            var qt = new List<QuestionTags>();
            for (int qid = 0; ; qid += bsize)
            {
                var stags = db.Table<Question>()
                              .Between(f => f.Id, qid, qid + bsize, BetweenBoundaries.FromInclusiveToExclusive)
                              .Select(f => new { QuestionId = f.Id, Tags = f.Tags });
                if (!stags.Any())
                {
                    break;
                }

                var dic_tags = new Dictionary<int, string>();
                var dic_count = new Dictionary<int, int?>();
                foreach (var t in stags.Where(f => f.Tags != null))
                {
                    string tags_string = "";
                    int tag_count = 0;
                    var ptags = Utils.ParseTags(t.Tags);
                    foreach (var tag in ptags)
                    {
                        if (!tag_dic.ContainsKey(tag))
                        {
                            throw new Exception("Tag not found: " + tag);
                        }
                        var tags_id = tag_dic[tag];
                        tags_string += tags_id + "|";
                        tag_count++;
                        qt.Add(new QuestionTags()
                        {
                            Id = 0,
                            QuestionId = t.QuestionId,
                            TagId = tags_id
                        });
                    }
                    dic_tags[t.QuestionId] = tags_string.Trim("|".ToCharArray());
                    dic_count[t.QuestionId] = tag_count;
                }
                db.Table<QuestionTags>().SaveBatch(qt);
                db.Table<Question>().Update(f => f.TagIds, dic_tags);
                db.Table<Question>().Update(f => f.TagCount, dic_count);
                qt = new List<QuestionTags>();
            }
            if (qt.Any())
            {
                db.Table<QuestionTags>().SaveBatch(qt);
                qt = new List<QuestionTags>();
            }

            Console.WriteLine("Tags done");

            //users
            var users = new List<User>();
            foreach (var row in EnumerateRows(Path.Combine(base_path, "Users.xml")))
            {
                var user = GetUser(row);
                if (user.Id == -1)
                {
                    continue;
                }
                users.Add(user);
                if (users.Count() > 30000)
                {
                    db.Table<User>().SaveBatch(users);
                    users = new List<User>();
                }
            }
            if (users.Any())
            {
                db.Table<User>().SaveBatch(users);
                users = new List<User>();
            }

            Console.WriteLine("Users done");

            //comments
            var comments = new List<Comment>();
            foreach (var row in EnumerateRows(Path.Combine(base_path, "Comments.xml")))
            {
                var comment = GetComment(row);
                comments.Add(comment);
                if (comments.Count() > 30000)
                {
                    db.Table<Comment>().SaveBatch(comments);
                    comments = new List<Comment>();
                }
            }
            if (comments.Any())
            {
                db.Table<Comment>().SaveBatch(comments);
                comments = new List<Comment>();
            }

            Console.WriteLine("Comments done");

            Console.WriteLine("Time: {0} min", ((DateTime.Now) - start).TotalMinutes);
            Console.ReadLine();
            db.Dispose();
        }

        static IEnumerable<XElement> EnumerateRows(string path)
        {
            var postsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
            using (var textReader = new StreamReader(postsFilePath, Encoding.UTF8))
            using (var reader = new XmlTextReader(textReader))
            {
                reader.MoveToContent();
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        yield return XNode.ReadFrom(reader) as XElement;
                    }
                }
            }
        }
    }
}
