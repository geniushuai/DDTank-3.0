using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Server.SceneMarryRooms.TankHandle
{
    public class MarryCommandAttbute:Attribute
    {
        public byte Code
        {
            get;
            private set;
        }
        public MarryCommandAttbute(byte code)
        {
            Code = code;
        }
    }
}
