using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.SceneGames;

namespace Game.Server.Packets.Client
{
    [PacketHandler((byte)ePackageType.GAME_PAIRUP_ROOM_SETUP, "撮合房间设置")]
    public class GameUserPairUpSetUpHandler:IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if(client.Player != client.Player.CurrentGame.Player)
            {
                return 0;
            }
            
            if ( client.Player.CurrentGame.GameState ==eGameState.PAIRUP)
            {
                eGameClass gameClass = (eGameClass)packet.ReadByte();

                if (gameClass == eGameClass.FREE_OR_CONSORTIA)
                {
                    if (client.Player.CurrentGame.Count != 1)
                    {
                        if (client.Player.CurrentGame.CheckConsortiaSame())
                        {
                            client.Player.CurrentGame.GameClass = eGameClass.FREE_OR_CONSORTIA;

                            ////client.Player.CurrentGame.SendRoomType();
                            //client.Player.CurrentGame.SendToAll(packet);
                            client.Player.CurrentGame.SendRoomInfo();
                        }
                    }
                }
                //int listType = packet.ReadInt();

                //if(gameClass == eGameClass.CONSORTIA)
                //{
                //    if(client.Player.CurrentGame.Count != 1)
                //    {
                //        if(client.Player.CurrentGame.CheckConsortiaSame())
                //        {
                //            client.Player.CurrentGame.GameClass = eGameClass.CONSORTIA;
                //            //client.Player.CurrentGame.ConsortiaID = packet.ReadInt();
                //            client.Player.CurrentGame.listType = listType;

                //            client.Player.CurrentGame.SendToAll(packet);
                //            client.Player.CurrentGame.SendRoomInfo();
                //        }
                //    }
                //}
                //else if(gameClass == eGameClass.FREE)
                //{
                //    if (client.Player.CurrentGame.pairUpState == 0)
                //    {
                //        client.Player.CurrentGame.GameClass = eGameClass.FREE;
                //        client.Player.CurrentGame.listType = 0;

                //        client.Player.CurrentGame.SendToAll(packet);
                //        client.Player.CurrentGame.SendRoomInfo();
                //    }
                //}
            }
            else if (client.Player.CurrentGame.GameState == eGameState.FREE)
            {
                eGameClass gameClass = (eGameClass)packet.ReadByte();

                if (gameClass == eGameClass.CONSORTIA)
                {
                    if (client.Player.CurrentGame.Count != 1)
                    {
                        if (client.Player.CurrentGame.CheckConsortiaSame())
                        {
                            client.Player.CurrentGame.ClassChangeMode = 1;
                            client.Player.CurrentGame.GameClass = eGameClass.CONSORTIA;

                            client.Player.CurrentGame.SendRoomInfo();
                        }
                    }
                }
                else if (gameClass == eGameClass.FREE)
                {
                    client.Player.CurrentGame.ClassChangeMode = 1;
                    client.Player.CurrentGame.GameClass = eGameClass.FREE;
                    client.Player.CurrentGame.SendRoomInfo();
                }
            }
            return 0;
        }
    }
}
