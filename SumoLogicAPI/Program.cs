using System;
using System.Collections.Generic;


namespace SumoLogicAPI
{
    class Program
    {
        static void Main(string[] args)
        {
            string username = "accessID";
            string password = "accessPassword";
            string Url = "https://api.us2.sumologic.com/api/v1/search/jobs";
            List<string> NameServer = new List<string>();
            List<int> CountServer = new List<int>();
            string DateTimenow = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            string DateTimethen = DateTime.Now.AddDays(-7).ToString("yyyy-MM-ddTHH:mm:ss");
            
            string TimeZone = "PST";
            QueryObj queryObj = new QueryObj
            {
                Query = " count by host",//(eample of wrong query you need to change the query)
                From = DateTimethen,
                To = DateTimenow,
                TimeZone = TimeZone
            };

            Request request = new Request(); // new instance of the query it return a tuple for my example machine and count
            var Result=request.PostJsonDataToApi(queryObj, Url, username, password);
            for (int m = 0; m < Result.Item1.Count; m++)
            {
                Console.WriteLine("server : " + Result.Item1[m] + " number : " + Result.Item2[m]);
            }
            Console.ReadLine();

        }
    }
}
