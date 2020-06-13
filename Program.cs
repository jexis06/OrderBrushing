using System;
using System.Collections.Generic;
using System.IO;

namespace OrderBrushing
{
    class Program
    {
        static void Main(string[] args)
        {
            string str = "";
            List<Record> records = new List<Record>();
            List<long> shop_ids = new List<long>();
            List<long> user_ids = new List<long>();
            DateTime startDate = DateTime.Now, endDate = new DateTime(2009, 01, 01);
            List<DateTime> date_times = new List<DateTime>();

            #region Data Retrieval
            ///Change this directory to the source CSV (shop-event_time ordered)
            using (var reader = new StreamReader(@"D:\jexis I\Profession\VSU\Operations\Shopee Code League\June 13 - Order Brushing\order_brush_ordered.csv"))
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
            #endregion

            #region Data Processing and Grouping
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
                    else
                        break;
                }
            }
            #endregion

            #region Data Summary
            List<long>[] user_ids_per_shop = new List<long>[shop_ids.Count];
            str += "shopid,userid\n";
            for (int i = 0; i < shop_ids.Count; i++)
            {
                user_ids_per_shop[i] = new List<long>();
                for (int d = 0; d < date_times.Count; d++)
                {
                    if (userlist[i][d].Count != 0 && ((c_time_numorders_pershop_perspan[i][d] / userlist[i][d].Count) >= 3))
                    {
                        for (int uid = 0; uid < userlist[i][d].Count; uid++)
                        {
                            if (!user_ids_per_shop[i].Contains(userlist[i][d][uid]))
                                user_ids_per_shop[i].Add(userlist[i][d][uid]);
                        }
                    }
                }
                str += shop_ids[i] + ",";
                if (user_ids_per_shop[i].Count > 0)
                {
                    for (int uid = 0; uid < user_ids_per_shop[i].Count; uid++)
                    {
                        str += user_ids_per_shop[i][uid] + "";
                        if (uid != user_ids_per_shop[i].Count - 1)
                            str += "&";
                    }
                }
                else
                    str += "0";
                str += "\n";
            }
            #endregion

            ///Change this directory to the destination CSV
            string dest = @"D:\jexis I\Profession\VSU\Operations\Shopee Code League\June 13 - Order Brushing\output.csv";
            StreamWriter sw = new StreamWriter(dest);
            sw.Write(str);
            sw.Close();
            Console.WriteLine("Finished! Your output is written at " + dest);
            Console.ReadKey();
        }
    }
}
