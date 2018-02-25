using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using LinqDb;
using LinqdbClient;
using System.Diagnostics;
using StackData;

namespace Testing
{
    class ThreadsTest : ITest
    {
        public void Do(string path)
        {
            //var db = new Db(path, "reader", "reader");
            var db = new Db(path);

            var sw = new Stopwatch();
            sw.Start();


            var all_ids = db.Table<Question>().Where(f => f.Id > 4).GetIds();
            var rg = new Random();
            var jobs = new List<Job>();
            for (int i = 0; i < 100000; i++)
            {
                jobs.Add(new Job() { db = db, id = all_ids.Ids[rg.Next(0, all_ids.Ids.Count - 1)] });
            }

            Parallel.ForEach(jobs, new ParallelOptions() { MaxDegreeOfParallelism = 10 }, f =>
            {
                f.Do();
            });

            sw.Stop();
            Console.WriteLine("Total: " + sw.ElapsedMilliseconds + " ms");

            Console.WriteLine("Total: {0}, errors {1}, min {2} ms, max {3} ms, avg {4} ms", jobs.Count,
                              jobs.Count(f => f.Error != null), jobs.Min(f => f.TimeMs), jobs.Max(f => f.TimeMs), jobs.Average(f => f.TimeMs));

            db.Dispose();
        }
    }

    public class Job 
    {
        public Db db { get; set; }
        public int id { get; set; }
        public long TimeMs { get; set; }
        public bool Done { get; set; }
        public string Error { get; set; }
        public bool NoSuchQuestion { get; set; }
        public static object _lock = new object();
        public static int Counter { get; set; }
        public void Do()
        {
            var sw = new Stopwatch();
            sw.Start();
            try
            {
                var q = db.Table<Question>().Where(f => f.Id == (int)id).SelectEntity().FirstOrDefault();
                if (q != null)
                {
                    var answers = db.Table<Answer>().Where(f => f.ParentId == q.Id).SelectEntity();
                    var all_posts_ids = new HashSet<int>(answers.Select(f => f.Id));
                    all_posts_ids.Add(q.Id);
                    var comments = db.Table<Comment>().Intersect(f => f.PostId, all_posts_ids).SelectEntity();
                }
                else
                {
                    NoSuchQuestion = true;
                }
            }
            catch (Exception ex)
            {
                Done = true;
                Error = ex.Message;
            }
            finally
            {
                sw.Stop();
                TimeMs = sw.ElapsedMilliseconds;
                lock (_lock)
                {
                    Counter++;
                    if (Counter % 100 == 0)
                    {
                        //Console.WriteLine(Counter);
                    }
                }
            }
        }
    }
}
