using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Base
{
    public class ConsoleClient:BaseClient
    {
        public ConsoleClient()
            : base(null, null)
        {
        }

        public override void DisplayMessage(string msg)
        {
            Console.WriteLine(msg);
        }
    }
}
