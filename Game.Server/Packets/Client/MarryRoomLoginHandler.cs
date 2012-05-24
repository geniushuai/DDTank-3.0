using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Bussiness;
using Game.Server.SceneMarryRooms;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
    [PacketHandler((int)ePackageType.MARRY_ROOM_LOGIN, "进入礼堂")]
    public class MarryRoomLoginHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            //if (client.Player.CurrentGame != null)
            //{
            //    client.Player.CurrentGame.RemovePlayer(client.Player);
            //}
            //if(client.Player.CurrentMarryRoom != null)
            //{
            //    client.Player.CurrentMarryRoom.RemovePlayer(client.Player);
            //}

            MarryRoom room = null;
            string msg = "";

            int id = packet.ReadInt();
            string pwd = packet.ReadString();
            int sceneID = packet.ReadInt();

            if (id != 0)
            {
                room = MarryRoomMgr.GetMarryRoombyID(id, pwd == null ? "" : pwd, ref msg);
            }
            else
            { 
                if(client.Player.PlayerCharacter.IsCreatedMarryRoom)
                {
                    MarryRoom[] rooms = MarryRoomMgr.GetAllMarryRoom();

                    foreach (MarryRoom r in rooms)
                    {
                        if (r.Info.GroomID == client.Player.PlayerCharacter.ID || r.Info.BrideID == client.Player.PlayerCharacter.ID)
                        {
                            room = r;
                            break;
                        }
                    }
                }

                if (room == null && client.Player.PlayerCharacter.SelfMarryRoomID != 0)
                {
                    client.Player.Out.SendMarryRoomLogin(client.Player, false);
                    MarryRoomInfo info = null;
                    using(PlayerBussiness db = new PlayerBussiness())
                    {
                        info = db.GetMarryRoomInfoSingle(client.Player.PlayerCharacter.SelfMarryRoomID);
                    }

                    if (info != null)
                    {
                        client.Player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("MarryRoomLoginHandler.RoomExist",info.ServerID,client.Player.PlayerCharacter.SelfMarryRoomID));
                        return 0;
                    }
                }
            }

            if (room != null)
            {
                if(room.CheckUserForbid(client.Player.PlayerCharacter.ID))
                {
                    client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("MarryRoomLoginHandler.Forbid"));
                    client.Player.Out.SendMarryRoomLogin(client.Player, false);
                    return 1;
                }

                if (room.RoomState == eRoomState.FREE)
                {
                    if (room.AddPlayer(client.Player))
                    {
                        client.Player.MarryMap = sceneID;

                        GSPacketIn pkg = client.Player.Out.SendMarryRoomLogin(client.Player, true);

                        room.SendMarryRoomInfoUpdateToScenePlayers(room);

                        return 0;
                    }
                }
                else
                {
                    client.Player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("MarryRoomLoginHandler.AlreadyBegin"));
                }

                client.Player.Out.SendMarryRoomLogin(client.Player, false);

            }
            else
            {
                client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation(string.IsNullOrEmpty(msg) ? "MarryRoomLoginHandler.Failed" : msg));
                client.Player.Out.SendMarryRoomLogin(client.Player, false);
            }

            return 1;
        }
    }
}
