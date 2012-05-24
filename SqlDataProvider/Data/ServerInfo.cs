using System;
using System.Collections.Generic;
using System.Text;

namespace SqlDataProvider.Data
{
    public class ServerInfo
    {
        public int ID { set; get; }

        public string Name { set; get; }

        public string IP { set; get; }

        public int Port { set; get; }

        public int State { set; get; }

        public int Online { set; get; }

        public int Total { set; get; }

        public int Room { set; get; }

        public string Remark { set; get; }

        public string RSA { set; get; }

        public int MustLevel { set; get; }

        public int LowestLevel { set; get; }

        public bool NewerServer { set; get; }
    }
}
