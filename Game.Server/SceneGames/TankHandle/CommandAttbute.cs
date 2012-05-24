using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Server.SceneGames.TankHandle
{
    public class CommandAttbute:Attribute
    {
        public byte Code
        {
            get;
            private set;
        }
        public CommandAttbute(byte code)
        {
            Code = code;
        }
    }
}
