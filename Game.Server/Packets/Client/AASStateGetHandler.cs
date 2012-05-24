//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Game.Base.Packets;
//using Bussiness;
//using SqlDataProvider.Data;

//namespace Game.Server.Packets.Client
//{
//    [PacketHandler((int)ePackageType.AAS_STATE_GET, "获取防沉迷系统状态")]
//    class AASStateGetHandler : IPacketHandler
//    {
//        public int HandlePacket(GameClient client, GSPacketIn packet)
//        {
//            int userID = client.Player.PlayerCharacter.ID;
//            bool result = false;

            //int userID = client.Player.PlayerCharacter.ID;
 
//            using (ProduceBussiness db = new ProduceBussiness())
//            {

            //using (ProduceBussiness db = new ProduceBussiness())
            //{
            //    int count = db.GetASSInfoSingle(userID);
            //    if(count != 0)
            //    {
            //        result = true;
            //    }
            //}
//                string ID = db.GetASSInfoSingle(userID);
//                if (ID != null)
//                {
//                    result = true;
//                }
//            }

//            client.Out.SendAASState(result);
//            return 0;
//        }
//    }
//}

            //client.Out.SendAASState(result);

