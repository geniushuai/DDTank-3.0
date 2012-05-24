using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bussiness;
using Game.Server.GameObjects;

namespace Game.Server.Managers
{
    class AntiAddictionMgr
    {
        private static bool _isASSon;

        public static void SetASSState(bool ASSState)
        {
            _isASSon = ASSState;
        }

        public static  int count=0;

        public static bool ISASSon
        {
            get
            {
                return _isASSon;
            }
        }
        
        public static double GetAntiAddictionCoefficient(int onlineTime)
        {
            if (_isASSon)
            {
                if (0 <= onlineTime && onlineTime <= 240)
                {
                    return 1;
                }
                else if (240 < onlineTime && onlineTime <= 300)
                {
                    return 0.5;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 1;
            }

        }

        public static int AASStateGet(GamePlayer player)
        {
            int userID = player.PlayerCharacter.ID;
            bool result = true;
            player.IsAASInfo = false;
            player.IsMinor = true;

            using (ProduceBussiness db = new ProduceBussiness())
            {
                String ID = db.GetASSInfoSingle(userID);
                if (ID != "")
                {
                    player.IsAASInfo = true;
                    result = false;
                    int Age = Convert.ToInt32(ID.Substring(6, 4));
                    int month = Convert.ToInt32(ID.Substring(10, 2));

                    if (DateTime.Now.Year.CompareTo(Age + 18) > 0 || (DateTime.Now.Year.CompareTo(Age + 18) == 0 && DateTime.Now.Month.CompareTo(month) >= 0))
                        player.IsMinor = false;
                }
            }
            //int now=  DateTime.Now.Month;

            if (result && player.PlayerCharacter.IsFirst != 0 && player.PlayerCharacter.DayLoginCount < 1 && ISASSon)
            {
                player.Out.SendAASState(result);
            }

            //if (player.IsMinor || !player.IsAASInfo && ISASSon)
            //{
                player.Out.SendAASControl(AntiAddictionMgr.ISASSon, player.IsAASInfo, player.IsMinor);
            //}
            return 0;
        }
    }
}
