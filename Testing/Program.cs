using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testing
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = @"E:\DATA";
            var list = new List<ITest>() 
            {
                new SignupsPerMonth(),
                new ActiveUsersByMonth(),
                new TagsPerMonth(),
                new TagsInfo(),
                new TagsPerMonthFast(),
                new TagsInfoFast()
            };
            foreach (var item in list)
            {
                item.Do(path);
            }
        }
    }
}
