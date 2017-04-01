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
    class TagsInfoFast : ITest
    {
        public void Do(string path)
        {
            var db = new Db(path);

            Stopwatch sw = new Stopwatch();
            sw.Start();

            Dictionary<string, int> tag_cache = db.Table<Tag>()
                                                  .SelectEntity()
                                                  .ToList()
                                                  .ToDictionary(f => f.Name, z => z.Id);

            Dictionary<int, int[]> result = new Dictionary<int, int[]>(); //int[]: [0] question count, [1] answer count, [2] unanswered count

            DateTime min_date = db.Table<Question>().OrderBy(f => f.CreationDate).Take(1).Select(f => new { f.CreationDate }).First().CreationDate;

            for (DateTime cd = min_date; ; cd = cd.AddMonths(1))
            {
                DateTime from = new DateTime(cd.Year, cd.Month, 1);
                DateTime to = new DateTime(cd.Year, cd.Month, DateTime.DaysInMonth(cd.Year, cd.Month), 23, 59, 59);
                var qs = db.Table<Question>().BetweenDate(f => f.CreationDate, from, to, BetweenBoundaries.BothInclusive)
                           .Select(f => new { f.Id, f.AnswerCount, f.AcceptedAnswerId, f.Tags });
                if (!qs.Any())
                {
                    break;
                }
                var qdic = new Dictionary<int, int[]>();
                foreach (var q in qs)
                {
                    qdic[q.Id] = new int[2] { q.AnswerCount, q.AcceptedAnswerId != null ? 1 : 0 };
                }

                foreach (var q in qs)
                {
                    var tags = Utils.ParseTags(q.Tags);
                    foreach (var tag in tags)
                    {
                        int tag_id = tag_cache[tag];
                        if (!result.ContainsKey(tag_id))
                        {
                            result[tag_id] = new int[3] { 1, qdic[q.Id][0], qdic[q.Id][1] };
                        }
                        else
                        {
                            result[tag_id][0] += 1;
                            result[tag_id][1] += qdic[q.Id][0];
                            result[tag_id][2] += qdic[q.Id][1];
                        }
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
            Console.WriteLine("Tag's info (fast): {0} sec", sw.Elapsed.TotalSeconds);

            db.Dispose();
        }
    }
}
