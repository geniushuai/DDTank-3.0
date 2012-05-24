using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlDataProvider.Data;
using log4net;
using System.Reflection;

namespace Game.Server.Managers
{
    public class MarryMatchMgr
    {
        /// <summary>
        /// Defines a logger for this class.
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static List<MarryInfo> _mmInfo;

        public static bool Init()
        {
            _mmInfo = new List<MarryInfo>();
            return true;
        }


    }
}
