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

            DateTime min_date = db.Table<Question>().OrderBy(f => f.CreationDate).Take(1).Select(f => new { CD = f.CreationDate }).First().CD;

            for (DateTime cd = min_date; ; cd = cd.AddMonths(1))
            {
                DateTime from = new DateTime(cd.Year, cd.Month, 1);
                DateTime to = new DateTime(cd.Year, cd.Month, DateTime.DaysInMonth(cd.Year, cd.Month), 23, 59, 59);
                var qs = db.Table<Question>().Between(f => f.CreationDate, from, to, BetweenBoundaries.BothInclusive)
                           .Select(f => new { QId = f.Id, Answer_count = f.AnswerCount, Has_accepted = f.AcceptedAnswerId != null });
                if (!qs.Any())
                {
                    break;
                }
                var qdic = new Dictionary<int, int[]>();
                foreach (var q in qs)
                {
                    qdic[q.QId] = new int[2] { q.Answer_count, q.Has_accepted ? 1 : 0 };
                }
                var tags = db.Table<QuestionTags>().Intersect(f => f.QuestionId, new HashSet<int?>(qs.Select(f => (int?)f.QId).AsEnumerable<int?>()))
                                                   .Select(f => new { Qid = f.QuestionId, Tid = f.TagId });
                foreach (var tag in tags)
                {
                    if (!result.ContainsKey(tag.Tid))
                    {
                        result[tag.Tid] = new int[3] { 1, qdic[tag.Qid][0], qdic[tag.Qid][1] };
                    }
                    else
                    {
                        result[tag.Tid][0] += 1;
                        result[tag.Tid][1] += qdic[tag.Qid][0];
                        result[tag.Tid][2] += qdic[tag.Qid][1];
                    }
                }
            }

            //pick popular most unanswered tags for display
            var hard_tags = result.Where(f => f.Value[0] > 1000).OrderByDescending(f => f.Value[2]).Take(5).ToList();
            foreach (var htag in hard_tags)
            {
                Console.WriteLine("tag {0} has unanswered {1}", db.Table<Tag>().Where(f => f.Id == htag.Key).SelectEntity().First().Name, htag.Value[2]);
            }
            sw.Stop();
            Console.WriteLine("Tag's info: {0} sec", sw.Elapsed.TotalSeconds);

            db.Dispose();
        }
    }
}
