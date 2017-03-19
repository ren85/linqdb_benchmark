using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using LinqdbClient;
using StackData;

namespace Testing
{
    class SignupsPerMonth : ITest
    {
        public void Do(string path)
        {
            var db = new Db(path);

            Stopwatch sw = new Stopwatch();
            sw.Start();

            DateTime min_date = db.Table<User>().OrderBy(f => f.CreationDate).Take(1).Select(f => new { f.CreationDate }).First().CreationDate;
            DateTime max_date = db.Table<User>().OrderByDescending(f => f.CreationDate).Take(1).Select(f => new { f.CreationDate }).First().CreationDate;

            Dictionary<DateTime, int> result = new Dictionary<DateTime,int>();
            for (DateTime cd = min_date; cd < max_date; cd = cd.AddMonths(1))
            {
                DateTime from = new DateTime(cd.Year, cd.Month, 1);
                DateTime to = new DateTime(cd.Year, cd.Month, DateTime.DaysInMonth(cd.Year, cd.Month), 23, 59, 59);
                result[from] = db.Table<User>().BetweenDate(f => f.CreationDate, from, to, BetweenBoundaries.BothInclusive).Select(f => new { f.Id }).Count();
            }
            var last_key = new DateTime(max_date.Year, max_date.Month, 1);
            if (!result.ContainsKey(last_key))
            {
                result[last_key] = db.Table<User>()
                                     .BetweenDate(f => f.CreationDate, new DateTime(max_date.Year, max_date.Month, 1), max_date, BetweenBoundaries.BothInclusive)
                                     .Select(f => new { f.Id }).Count();
            }

            foreach (var entry in result.OrderBy(f => f.Key))
            {
                Console.WriteLine("{0}-{1} - {2} signups", entry.Key.Year, entry.Key.Month, entry.Value);
            }
            sw.Stop();
            Console.WriteLine("Signups per month: {0} sec", sw.Elapsed.TotalSeconds);

            db.Dispose();
        }
    }
}
