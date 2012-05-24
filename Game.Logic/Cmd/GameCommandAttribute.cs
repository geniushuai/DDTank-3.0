using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Logic.Cmd
{
    public class GameCommandAttribute:Attribute
    {
        private int m_code;

        private string m_description;

        public int Code
        {
            get { return m_code; }
        }

        public string Description
        {
            get { return m_description; }
        }


        public GameCommandAttribute(int code,string description)
        {
            m_code = code;
            m_description = description;
        }

    }
}
