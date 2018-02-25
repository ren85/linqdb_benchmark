using System;
using LinqdbClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using StackData;

namespace Testing
{
    class Various : ITest
    {
        public void Do(string path)
        {
            var db = new Db(path, "admin", "admin");

            Stopwatch sw = new Stopwatch();
            sw.Start();

            //db.Table<Question>().RemovePropertyMemoryIndex(f => f.CreationDate);
            //db.Table<Question>().RemovePropertyMemoryIndex(f => f.OwnerUserId);
            //db.Table<Answer>().RemovePropertyMemoryIndex(f => f.CreationDate);
            //db.Table<Answer>().RemovePropertyMemoryIndex(f => f.OwnerUserId);
            //db.Table<Question>().RemovePropertyMemoryIndex(f => f.AnswerCount);
            //db.Table<Question>().RemovePropertyMemoryIndex(f => f.AcceptedAnswerId);

            //db.Table<QuestionTags>().RemovePropertyMemoryIndex(f => f.QuestionId);
            //db.Table<QuestionTags>().RemovePropertyMemoryIndex(f => f.TagId);

            var res = db.GetExistingIndexes();
            
            sw.Stop();
            Console.WriteLine("Various: {0} sec", sw.Elapsed.TotalSeconds);

            db.Dispose();
        }
    }
}
