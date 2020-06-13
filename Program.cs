using System;
using System.Collections.Generic;
using System.IO;

namespace OrderBrushing
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Record> records = new List<Record>();
            List<long> shop_ids = new List<long>();
            List<long> user_ids = new List<long>();
            DateTime startDate = DateTime.Now, endDate = new DateTime(2009, 01, 01);
            List<DateTime> date_times = new List<DateTime>();
            using (var reader = new StreamReader(@"D:\jexis I\Profession\VSU\Operations\Shopee Code League\June 13 - Order Brushing\order_brushing_p1.csv"))
            {
                reader.ReadLine();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');
                    Record r = new Record();
                    r.order_id = Convert.ToInt64(values[0]);
                    r.shop_id = Convert.ToInt64(values[1]);
                    r.user_id = Convert.ToInt64(values[2]);
                    r.dateTime = Convert.ToDateTime(values[3]);
                    records.Add(r);
                    if (!shop_ids.Contains(r.shop_id))
                        shop_ids.Add(r.shop_id);
                    if (!user_ids.Contains(r.user_id))
                        user_ids.Add(r.user_id);
                    if (!date_times.Contains(r.dateTime))
                        date_times.Add(r.dateTime);
                    if (r.dateTime < startDate)
                        startDate = r.dateTime;
                    if (r.dateTime > endDate)
                        endDate = r.dateTime;

                    line = null;
                    values = null;
                    r = null;
                }
            }

            Console.WriteLine("Number of shops: " + shop_ids.Count);
            Console.WriteLine("Number of users: " + user_ids.Count);
            Console.WriteLine("Start: " + startDate);
            Console.WriteLine("End: " + endDate);
            int[] dec_27_2019_usercnt = new int[shop_ids.Count];

            ///Analytics code
            date_times.Sort();
            long[][] c_time_numorders_pershop_perspan = new long[shop_ids.Count][];
            List<long>[][] userlist = new List<long>[shop_ids.Count][];
            for (int i = 0; i < shop_ids.Count; i++)
            {
                c_time_numorders_pershop_perspan[i] = new long[date_times.Count];
                userlist[i] = new List<long>[date_times.Count];
                for (int d = 0; d < date_times.Count; d++)
                    userlist[i][d] = new List<long>();
            }
            Console.WriteLine("Number of spans: " + date_times.Count);
            for (int d = 0; d < date_times.Count; d++)
            {
                foreach (Record r in records)
                {
                    if (r.dateTime.Subtract(date_times[d]).TotalMinutes <= 60)
                    {
                        int sid = shop_ids.IndexOf(r.shop_id);
                        c_time_numorders_pershop_perspan[sid][d]++;
                        if (!userlist[sid][d].Contains(r.user_id))
                            userlist[sid][d].Add(r.user_id);
                    }
                }
            }

            int r_cnt = 0;

            for (int i = 0; i < shop_ids.Count; i++)
            {
                for (int d = 0; d < date_times.Count; d++)
                {
                    if (userlist[i][d].Count != 0 && ((c_time_numorders_pershop_perspan[i][d] / userlist[i][d].Count) >= 3))
                    {
                        Console.WriteLine("ShopID (" + shop_ids[i] + ") - CT(" + (c_time_numorders_pershop_perspan[i][d] / userlist[i][d].Count));
                    }
                }
            }

            Console.WriteLine("Row count: " + r_cnt);
            Console.WriteLine("Finished...");
            Console.ReadKey();
        }
    }
}
