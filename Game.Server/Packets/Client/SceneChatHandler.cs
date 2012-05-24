using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using Game.Base.Packets;
using Game.Server.Managers;
using Bussiness;
using Game.Logic;
using Game.Server.Battle;

namespace Game.Server.Packets.Client
{
    [PacketHandler((int)ePackageType.SCENE_CHAT, "用户场景聊天")]
    public class SceneChatHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            packet.ClientID = (client.Player.PlayerCharacter.ID);

            byte channel = packet.ReadByte();
            bool team = packet.ReadBoolean();
            string nick = packet.ReadString();
            string msg = packet.ReadString();

            GSPacketIn pkg = packet.Clone();
            pkg.ClearContext();
            pkg.ClientID = (client.Player.PlayerCharacter.ID);
            pkg.WriteInt(4);
            pkg.WriteByte(channel);
            pkg.WriteBoolean(team);
            pkg.WriteString(client.Player.PlayerCharacter.NickName);
            pkg.WriteString(msg);

            if (client.Player.CurrentRoom != null)
            {
                if (client.Player.CurrentRoom.RoomType == eRoomType.Match)
                {
                    if (client.Player.CurrentRoom.Game != null)
                    {
                        client.Player.CurrentRoom.BattleServer.Server.SendChatMessage(msg, client.Player,team);
                        return 1;
                    }                  
                }
            }
            //3公会
            if (channel == 3)
            {
                if (client.Player.PlayerCharacter.ConsortiaID == 0)
                    return 0;

                if (client.Player.PlayerCharacter.IsBanChat)
                {
                    client.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("ConsortiaChatHandler.IsBanChat"));
                    return 1;
                }

                //packet.ClientID = (client.Player.PlayerCharacter.ID);
                //string nick = packet.ReadString();
                //string msg = packet.ReadString();
                pkg.WriteInt(client.Player.PlayerCharacter.ConsortiaID);

                GamePlayer[] players = WorldMgr.GetAllPlayers();
                foreach (GamePlayer p in players)
                {
                    if (p.PlayerCharacter.ConsortiaID == client.Player.PlayerCharacter.ConsortiaID && !p.IsBlackFriend(client.Player.PlayerCharacter.ID))
                        p.Out.SendTCP(pkg);
                }

                GameServer.Instance.LoginServer.SendPacket(pkg);
            }
            else if (channel == 9)
            {
                if (client.Player.CurrentMarryRoom == null)
                {
                    return 1;
                }

                //0 client.Player.CurrentMarryRoom.SendToAll(pkg);
                client.Player.CurrentMarryRoom.SendToAllForScene(pkg, client.Player.MarryMap);
            }
            else
            {

                if (client.Player.CurrentRoom != null)
                {
                    if (team)
                    {
                        client.Player.CurrentRoom.SendToTeam(pkg, client.Player.CurrentRoomTeam, client.Player);
                    }
                    else
                    {
                        client.Player.CurrentRoom.SendToAll(pkg);
                    }
                }
                else
                {
                    if (DateTime.Compare(client.Player.LastChatTime.AddSeconds(1), DateTime.Now) > 0 && channel == 5)
                        return 1;

                    if (team)
                        return 1;
                    if (DateTime.Compare(client.Player.LastChatTime.AddSeconds(30), DateTime.Now) > 0)
                    {
                        client.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("SceneChatHandler.Fast"));
                        return 1;
                    }
                    client.Player.LastChatTime = DateTime.Now;

                    GamePlayer[] list = Managers.WorldMgr.GetAllPlayers();
                    foreach (GamePlayer p in list)
                    {
                        if (p.CurrentRoom == null && p.CurrentMarryRoom == null && !p.IsBlackFriend(client.Player.PlayerCharacter.ID))
                            p.Out.SendTCP(pkg);
                    }
                }
            }


            return 1;
        }
    }
}
