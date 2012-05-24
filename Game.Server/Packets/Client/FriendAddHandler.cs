using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using Game.Base.Packets;
using SqlDataProvider.Data;
using Bussiness;

namespace Game.Server.Packets.Client
{
    [PacketHandler((byte)ePackageType.FRIEND_ADD, "添加好友")]
    public class FriendAddHandler : IPacketHandler
    {
        //0友好，1黑名单
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            GSPacketIn pkg = packet.Clone();
            pkg.ClearContext();

            string nickName = packet.ReadString();
            int relation = packet.ReadInt();
            if (relation < 0 || relation > 1)
                return 1;

            using (PlayerBussiness db = new PlayerBussiness())
            {
                PlayerInfo user = null;
                GamePlayer player = Managers.WorldMgr.GetClientByPlayerNickName(nickName);
                if (player != null)
                    user = player.PlayerCharacter;
                else
                    user = db.GetUserSingleByNickName(nickName);
                if (!string.IsNullOrEmpty(nickName) && user != null)
                {
                    if (!client.Player.Friends.ContainsKey(user.ID) || client.Player.Friends[user.ID] != relation)
                    {
                        FriendInfo friend = new FriendInfo();
                        friend.FriendID = user.ID;
                        friend.IsExist = true;
                        friend.Remark = "";
                        friend.UserID = client.Player.PlayerCharacter.ID;
                        friend.Relation = relation;
                        if (db.AddFriends(friend))
                        {
                            client.Player.FriendsAdd(user.ID, relation);
                            pkg.WriteInt(user.ID);
                            pkg.WriteString(user.NickName);
                            pkg.WriteBoolean(user.Sex);
                            pkg.WriteString(user.Style);
                            pkg.WriteString(user.Colors);
                            pkg.WriteString(user.Skin);
                            pkg.WriteInt(user.State == 1 ? 1 : 0);
                            pkg.WriteInt(user.Grade);
                            pkg.WriteInt(user.Hide);
                            pkg.WriteString(user.ConsortiaName);
                            pkg.WriteInt(user.Total);
                            pkg.WriteInt(user.Escape);
                            pkg.WriteInt(user.Win);
                            pkg.WriteInt(user.Offer);
                            pkg.WriteInt(user.Repute);
                            pkg.WriteInt(relation);
                            pkg.WriteString(user.UserName);
                            pkg.WriteInt(user.Nimbus);
                            pkg.WriteInt(user.FightPower);
                            pkg.WriteInt(500);
                            pkg.WriteString("Honor");
                            if (relation != 1 && user.State != 0)
                            {
                                GSPacketIn response = new GSPacketIn((byte)ePackageType.FRIEND_RESPONSE, client.Player.PlayerCharacter.ID);
                                response.WriteInt(user.ID);
                                response.WriteString(client.Player.PlayerCharacter.NickName);
                                if (player != null)
                                    player.Out.SendTCP(response);
                                else
                                    GameServer.Instance.LoginServer.SendPacket(response);
                            }
                            client.Out.SendTCP(pkg);
                        }
                    }
                    else
                    {
                        client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("FriendAddHandler.Falied"));
                    }
                }
                else
                {
                    client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("FriendAddHandler.Success"));
                }
            }
            return 0;
        }
    }
}
