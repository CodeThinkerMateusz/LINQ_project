using System;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;

//using System.IO.DirectoryInfo.EnumerateDirectories;
//using System.IO.DirectoryInfo.EnumerateFiles;


var database_path = @"C:\Users\mw110\Downloads\2013-citibike-tripdata\2013-citibike-tripdata";
try
{
    var csv_files = Directory.EnumerateFiles(database_path, "*.csv", SearchOption.AllDirectories);
    var dict = new Dictionary<int, long>();
    foreach (var file in csv_files)
    {
        IEnumerable<string> line = File.ReadLines(file).Skip(1);
        var query = line.Select(l => l.Split(','));
        var boxes = query.Select(colums => new
        {
            id = colums[11].Trim('"'),
            Time = colums[0].Trim('"')
        }
            );
        foreach(var  box  in boxes)
        {
            if (int.TryParse(box.id, out int Id) && long.TryParse(box.Time, out long time))
            {
                if (dict.ContainsKey(Id) == false)
                {
                    dict.Add(Id, time);
                }
                else
                {
                    dict[Id] += time;
                }
            }
        }
    }
    
    var maks = dict.OrderByDescending(x=>x.Value).First();
    var time = TimeSpan.FromSeconds(maks.Value);
    Console.WriteLine($"The bike with Id:{maks.Key} traveled {(int)time.TotalHours} hours {time.Minutes} minutes and {time.Seconds} seconds");
    //var time_in_hours = maks.Value / 3600;
    //var minutes = (maks.Value - time_in_hours*3600) / 60;
    //var seconds = (maks.Value - time_in_hours * 3600 - minutes * 60);
    //Console.WriteLine($"The bike with Id:{maks.Key} traveled {time_in_hours} hours {minutes} minutes and {seconds} seconds");
    
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
}

