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
    class ActiveUsersByMonth : ITest //active user - one that either asked or answered at least once
    {
        public void Do(string path)
        {
            //var db = new Db(path, "reader", "reader");
            var db = new Db(path);

            //var count = db.Table<Answer>().BetweenDate(f => f.CreationDate, Convert.ToDateTime("2015-10-01"), Convert.ToDateTime("2015-11-01"), BetweenBoundaries.BothInclusive).Count();


            Stopwatch sw = new Stopwatch();
            sw.Start();

            Dictionary<DateTime, int> result = new Dictionary<DateTime, int>();
            DateTime min_date = db.Table<Question>().OrderBy(f => f.CreationDate).Take(1).Select(f => new { f.CreationDate }).First().CreationDate;
            
            for (DateTime cd = min_date; ; cd = cd.AddMonths(1))
            {
                DateTime from = new DateTime(cd.Year, cd.Month, 1);
                DateTime to = new DateTime(cd.Year, cd.Month, DateTime.DaysInMonth(cd.Year, cd.Month), 23, 59, 59);
                var users_asked_l = db.Table<Question>()
                                      .BetweenDate(f => f.CreationDate, from, to, BetweenBoundaries.BothInclusive)
                                      .Select(f => new { f.OwnerUserId })
                                      .Select(f => f.OwnerUserId)
                                      .Where(f => f != null);
                if (!users_asked_l.Any())
                {
                    break;
                }
                var users_asked = new HashSet<int?>(users_asked_l);
                result[from] = users_asked.Count();
                
                //answers
                var users_answered = db.Table<Answer>()
                                       .BetweenDate(f => f.CreationDate, from, to, BetweenBoundaries.BothInclusive)
                                       .Select(f => new { f.OwnerUserId })
                                       .Select(f => f.OwnerUserId)
                                       .Where(f => f != null);
                result[from] += users_answered.Where(f => !users_asked.Contains((int)f)).Distinct().Count();
            }
           

            foreach (var entry in result.OrderBy(f => f.Key))
            {
                Console.WriteLine("{0}-{1} - {2} active users", entry.Key.Year, entry.Key.Month, entry.Value);
            }
            sw.Stop();
            Console.WriteLine("User activity per month: {0} sec", sw.Elapsed.TotalSeconds);

            db.Dispose();
        }
    }
}
