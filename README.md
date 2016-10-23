<h2>Linqdb's benchmark</h2>

Linqdb: https://github.com/ren85/linqdb

Inspired by: https://ayende.com/blog/175649/performance-analysis-the-cost-of-stackoverflow-indexes

Data from: https://archive.org/download/stackexchange

Data model: https://github.com/ren85/linqdb_benchmark/blob/master/StackData/Data.cs

Machine disk: TOSHIBA DT01ACA100 7200 RPM
Machine processor: Intel Core i5-6600 3.31Ghz
Machine ram: 8 Gb

Results:

1. Data import (https://github.com/ren85/linqdb_benchmark/blob/master/ImportStack/Program.cs): 308 min, db size 44,8 Gb
2. Signups per month (https://github.com/ren85/linqdb_benchmark/blob/master/Testing/SignupsPerMonth.cs): 33 sec
3. User activity per month (https://github.com/ren85/linqdb_benchmark/blob/master/Testing/ActiveUsersByMonth.cs): 436 sec
4. Tag activity per month (https://github.com/ren85/linqdb_benchmark/blob/master/Testing/TagsPerMonth.cs): 1154 sec
5. Tag's info (https://github.com/ren85/linqdb_benchmark/blob/master/Testing/TagsInfo.cs): 629 sec

