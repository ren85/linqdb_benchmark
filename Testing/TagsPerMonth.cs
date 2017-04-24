using LinqDb;
//using LinqdbClient;
using StackData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testing
{
    class TagsPerMonth : ITest
    {
        public void Do(string path)
        {
            //var db = new Db(path, "reader", "reader");
            var db = new Db(path);

            Stopwatch sw = new Stopwatch();
            sw.Start();
           

            Dictionary<DateTime, Dictionary<int, int>> result = new Dictionary<DateTime, Dictionary<int, int>>();
            DateTime min_date = db.Table<Question>().OrderBy(f => f.CreationDate).Take(1).Select(f => new { f.CreationDate }).First().CreationDate;

            for (DateTime cd = min_date; ; cd = cd.AddMonths(1))
            {
                DateTime from = new DateTime(cd.Year, cd.Month, 1);
                DateTime to = new DateTime(cd.Year, cd.Month, DateTime.DaysInMonth(cd.Year, cd.Month), 23, 59, 59);
                var qs = db.Table<Question>().BetweenDate(f => f.CreationDate, from, to, BetweenBoundaries.BothInclusive).Select(f => new { f.Id });

                if (!qs.Any())
                {
                    break;
                }

                var res = new Dictionary<int, int>(); //tag_id, count
                var tags = db.Table<QuestionTags>().IntersectListInt(f => f.QuestionId, qs.Select(f => f.Id).ToList())
                                                   .Select(f => new { f.QuestionId, f.TagId });

                foreach (var tag in tags)
                {
                    if (!res.ContainsKey(tag.TagId))
                    {
                        res[tag.TagId] = 1;
                    }
                    else
                    {
                        res[tag.TagId]++;
                    }
                }
                result[from] = res;
            }

            

            //pick some tags for display
            var display_tags = new Dictionary<string, int>() { { "c#", 0 }, { "java", 0 }, { "javascript", 0 }, { "jquery", 0 } };
            var display_ids = new List<int>();
            foreach (var t in display_tags.Keys.ToList())
            {
                var dt_id = db.Table<Tag>().Where(f => f.Name == t).SelectEntity().FirstOrDefault();
                if (dt_id == null)
                {
                    throw new Exception("Tag not found: " + t);
                }
                display_tags[t] = dt_id.Id;
            }

            foreach (var entry in result.OrderBy(f => f.Key))
            {
                Console.Write("{0}-{1} - ", entry.Key.Year, entry.Key.Month);
                foreach (var t in display_tags)
                {
                    Console.Write("{0}:{1}  ", t.Key, (entry.Value.ContainsKey(t.Value) ? entry.Value[t.Value] : 0));
                }
                Console.WriteLine();
            }
            sw.Stop();
            Console.WriteLine("Tag activity per month: {0} sec", sw.Elapsed.TotalSeconds);

            db.Dispose();
        }
    }
}
