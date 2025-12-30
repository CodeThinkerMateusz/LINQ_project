using System;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;

//using System.IO.DirectoryInfo.EnumerateDirectories;
//using System.IO.DirectoryInfo.EnumerateFiles;

Console.WriteLine("Choose a question 1, 2 lub 3");
Console.WriteLine("1 - Which bike was the longest on the road");
Console.WriteLine("2 - Which tour was most popular on wednesday between 6 and 12");
Console.WriteLine("3 - On average when tours were longer on weekdays or weekends");
var database_path = @"C:\Users\mw110\Downloads\2013-citibike-tripdata\2013-citibike-tripdata";

var choosing = Console.ReadLine();
switch (choosing)
{
    case "1":
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
                foreach (var box in boxes)
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

            var maks = dict.OrderByDescending(x => x.Value).First();

            var time_in_hours = maks.Value / 3600;
            var minutes = (maks.Value - time_in_hours * 3600) / 60;
            var seconds = (maks.Value - time_in_hours * 3600 - minutes * 60);
            Console.WriteLine($"The bike with Id:{maks.Key} traveled {time_in_hours} hours {minutes} minutes and {seconds} seconds");

        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        break;


    case "2":
        try
        {
            var csv_files = Directory.EnumerateFiles(database_path, "*.csv", SearchOption.AllDirectories);
            var dict = new Dictionary<string, int>();
            foreach (var file in csv_files)
            {
                IEnumerable<string> line = File.ReadLines(file).Skip(1);
                var query = line.Select(l => l.Split(','));
                var boxes = query.Select(colums => new
                {
                    start_station = colums[4].Trim('"'),
                    end_station = colums[8].Trim('"'),
                    start_date = colums[1].Trim('"'),
                    end_date = colums[2].Trim('"')
                }
                    );

                foreach(var box in boxes)
                {
                    DateTime date_and_time_start = DateTime.Parse(box.start_date);
                    DayOfWeek dayOfWeek = date_and_time_start.DayOfWeek;
                    var timeOnly = date_and_time_start.Hour;

                    DateTime end_date_and_time = DateTime.Parse(box.end_date);
                    var end_time = end_date_and_time.Hour;

                    if( (dayOfWeek == DayOfWeek.Wednesday) && (timeOnly >= 6 && timeOnly <= 12) && (end_time >= 6 && end_time <= 12) && (date_and_time_start.Date == end_date_and_time.Date))
                    {
                        var path = box.start_station + " to " + box.end_station;
                        if(dict.ContainsKey(path) == false)
                        {
                            dict.Add(path, 1);
                        }
                        else
                        {
                            dict[path] += 1;
                        }
                    }

                }
            }

            var maks = dict.OrderByDescending(x => x.Value).First();
            //string[] substrings = maks.Key.Split("");
            //var start = substrings[0];
            //var end = substrings[1];
           
            Console.WriteLine($"On wednesday between 6 and 12 the most popular route was from {maks.Key} used {maks.Value} times");
            

        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        break;

    case "3":
        try
        {
            var csv_files = Directory.EnumerateFiles(database_path, "*.csv", SearchOption.AllDirectories);
            var dict = new Dictionary<string, (long dist ,int count)>();
            foreach (var file in csv_files)
            {
                IEnumerable<string> line = File.ReadLines(file).Skip(1);
                var query = line.Select(l => l.Split(','));
                var boxes = query.Select(colums => new
                {
                    duration = colums[0].Trim('"'),
                    start_date = colums[1].Trim('"'),
                    end_date = colums[2].Trim('"')
                }
                    );

                foreach (var box in boxes)
                {
                    DateTime date_and_time_start = DateTime.Parse(box.start_date);
                    DayOfWeek dayOfWeek = date_and_time_start.DayOfWeek;


                    DateTime end_date_and_time = DateTime.Parse(box.end_date);
                    DayOfWeek day_of_week_end = end_date_and_time.DayOfWeek;

                    if (int.TryParse(box.duration, out var result))
                    {
                        if ((dayOfWeek == DayOfWeek.Saturday || dayOfWeek == DayOfWeek.Sunday) && (day_of_week_end == DayOfWeek.Saturday || day_of_week_end == DayOfWeek.Sunday))
                        {
                            if (dict.ContainsKey("weekend") == false)
                            {
                                dict.Add("weekend", (result, 1));
                            }

                            else
                            {
                                var curr = dict["weekend"];
                                dict["weekend"] = (curr.dist + result, curr.count + 1);
                            }
                        }
                        else
                        {
                            if (day_of_week_end != DayOfWeek.Saturday && day_of_week_end != DayOfWeek.Sunday)
                            {
                                if (dict.ContainsKey("weekday") == false)
                                {
                                    dict.Add("weekday", (result, 1));
                                }
                                else
                                {
                                    var curr = dict["weekday"];
                                    dict["weekday"] = (curr.dist + result, curr.count + 1);
                                }
                            }
                        }
                    }
                }
                    
                }


            float weekend_avr = (float)dict["weekend"].dist / (float)dict["weekend"].count;
            float weekday_avr = (float)dict["weekday"].dist / (float)dict["weekday"].count;

            int weekend_min = (int)weekend_avr / 60;
            int weekend_sec = (int)weekend_avr - (weekend_min * 60);

            int weekday_min = (int)weekday_avr / 60;
            int weekday_sec = (int)weekday_avr - (weekday_min * 60);

            if (weekday_avr > weekend_avr)

                Console.WriteLine($"Weekday trips with average duration ({weekday_min} minutes and {weekday_sec} seconds) are longer than the weekend trips with average ({weekend_min} minutes and {weekend_sec} seconds)");

            else
                Console.WriteLine($"Weekend trips with average duration ({weekend_min} minutes and {weekend_sec} seconds) are  longer than weekday trips with average ({weekday_min} minutes and {weekday_sec} seconds)");

        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        break;

    default:
        Console.WriteLine("No such a  question"); 
        break;


}