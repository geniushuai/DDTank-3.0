using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Server.SceneGames
{
    public class GameProcessorAttribute:Attribute
    {
        private byte _code;

        private string _descript;

        public GameProcessorAttribute(byte code, string description)
        {
            _code = code;

            _descript = description;
 
        }

        public byte Code
        {
            get
            {
                return _code;
            }
        }

        public string Description
        {
            get
            {
                return _descript;
            }
        }
    }
}
