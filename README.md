<h2>Linqdb's benchmark</h2>

Linqdb: http://linqdb.net

Inspired by: https://ayende.com/blog/175649/performance-analysis-the-cost-of-stackoverflow-indexes

Data from: https://archive.org/download/stackexchange

Data model: https://github.com/ren85/linqdb_benchmark/blob/master/StackData/Data.cs


Code:

1. Data import - imports Stack Overflow's data (2016 September) - https://github.com/ren85/linqdb_benchmark/blob/master/ImportStack/Program.cs
2. Signups per month - https://github.com/ren85/linqdb_benchmark/blob/master/Testing/SignupsPerMonth.cs
3. User activity per month - https://github.com/ren85/linqdb_benchmark/blob/master/Testing/ActiveUsersByMonth.cs
4. Tag activity per month - https://github.com/ren85/linqdb_benchmark/blob/master/Testing/TagsPerMonth.cs
5. Tag's info - https://github.com/ren85/linqdb_benchmark/blob/master/Testing/TagsInfo.cs
6. Tag activity per month, fast version - https://github.com/ren85/linqdb_benchmark/blob/master/Testing/TagsPerMonthFast.cs
7. Tag's info, fast version - https://github.com/ren85/linqdb_benchmark/blob/master/Testing/TagsInfoFast.cs

Embedded db, SSD disk:

1. Data import: 245 min, db size 46 Gb
2. Signups per month: 12 sec
3. User activity per month: 160 sec
4. Tag activity per month: 430 sec
5. Tag's info: 442 sec
6. Tag activity per month, fast version: 104 sec
7. Tag's info, fast version: 162 sec

Server db, server and client in the same geographic region (but not in the same LAN), SSD disk:

1. Data import: 257 min, db size 49 Gb
2. Signups per month: 26 sec
3. User activity per month: 265 sec
4. Tag activity per month: 635 sec
5. Tag's info: 755 sec
6. Tag activity per month, fast version: 243 sec
7. Tag's info, fast version: 368 sec


Difference between embedded-db code and server-db code:

1. <code>using LinqDb;</code> and <code>using LinqdbClient;</code>
2. <code>var db = new Db(path-to-folder);</code> and <code>var db = new Db(ip-address:port);</code>
