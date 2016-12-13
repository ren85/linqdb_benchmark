using LinqDb;
using StackData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testing
{
    class TagsInfo : ITest
    {
        public void Do(string path)
        {
            var db = new Db(path);

            Stopwatch sw = new Stopwatch();
            sw.Start();

            Dictionary<int, int[]> result = new Dictionary<int, int[]>(); //int[]: [0] question count, [1] answer count, [2] unanswered count

            DateTime min_date = db.Table<Question>().OrderBy(f => f.CreationDate).Take(1).Select(f => new { f.CreationDate }).First().CreationDate;

            for (DateTime cd = min_date; ; cd = cd.AddMonths(1))
            {
                DateTime from = new DateTime(cd.Year, cd.Month, 1);
                DateTime to = new DateTime(cd.Year, cd.Month, DateTime.DaysInMonth(cd.Year, cd.Month), 23, 59, 59);
                var qs = db.Table<Question>().Between(f => f.CreationDate, from, to, BetweenBoundaries.BothInclusive)
                           .Select(f => new { f.Id, f.AnswerCount, f.AcceptedAnswerId });
                if (!qs.Any())
                {
                    break;
                }
                var qdic = new Dictionary<int, int[]>();
                foreach (var q in qs)
                {
                    qdic[q.Id] = new int[2] { q.AnswerCount, q.AcceptedAnswerId != null ? 1 : 0 };
                }
                var tags = db.Table<QuestionTags>().Intersect(f => f.QuestionId, qs.Select(f => f.Id).ToList())
                                                   .Select(f => new { f.QuestionId, f.TagId });
                foreach (var tag in tags)
                {
                    if (!result.ContainsKey(tag.TagId))
                    {
                        result[tag.TagId] = new int[3] { 1, qdic[tag.QuestionId][0], qdic[tag.QuestionId][1] };
                    }
                    else
                    {
                        result[tag.TagId][0] += 1;
                        result[tag.TagId][1] += qdic[tag.QuestionId][0];
                        result[tag.TagId][2] += qdic[tag.QuestionId][1];
                    }
                }
            }

            //pick popular most unanswered tags for display
            var hard_tags = result.Where(f => f.Value[0] > 1000).OrderByDescending(f => f.Value[2] / (double)f.Value[0]).Take(20).ToList();
            foreach (var htag in hard_tags)
            {
                Console.WriteLine("tag {0} (total {1}) has unanswered ratio {2} %", db.Table<Tag>().Where(f => f.Id == htag.Key).SelectEntity().First().Name, htag.Value[0], Math.Round(htag.Value[2] * 100 / (double)htag.Value[0]));
            }
            sw.Stop();
            Console.WriteLine("Tag's info: {0} sec", sw.Elapsed.TotalSeconds);

            db.Dispose();
        }
    }
}
