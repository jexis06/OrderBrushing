using System;
using System.Collections.Generic;
using System.Text;

namespace OrderBrushing
{
    class Record
    {
        public long order_id;
        public long shop_id;
        public long user_id;
        public DateTime dateTime;

        public override string ToString()
        {
            return order_id + " - " + shop_id + " => " + user_id + " } " + dateTime;
        }
    }
}
