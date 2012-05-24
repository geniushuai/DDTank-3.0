using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;
using Bussiness;
using System.Configuration;
using Bussiness.Interface;

namespace Game.Server.Packets.Client
{
    [PacketHandler((int)ePackageType.LOGIN, "User Login handler")]
    public class UserLoginHandler : IPacketHandler
    {
        //修改:  Xiaov 
        //时间:  2009-11-9
        //描述:  登陆请求
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            try
            {
                GSPacketIn pkg = packet.Clone();
                pkg.ClearContext();

                if (client.Player == null)
                {
                    int version = packet.ReadInt();
                    int clientType = packet.ReadInt();
                    byte[] tempKey = new byte[8];

                    byte[] src = packet.ReadBytes();
                  
                    //解密
                    try
                    {
                        src = WorldMgr.RsaCryptor.Decrypt(src,false);
                    }
                    catch (ExecutionEngineException e)
                    {
                        client.Out.SendKitoff(LanguageMgr.GetTranslation("UserLoginHandler.RsaCryptorError"));
                        client.Disconnect();
                        GameServer.log.Error("ExecutionEngineException", e);
                        return 0;
                    }
                    catch (Exception ex)
                    {
                        //防止攻击,如果解密出错,直接断开连接
                        client.Out.SendKitoff(LanguageMgr.GetTranslation("UserLoginHandler.RsaCryptorError"));
                        client.Disconnect();
                        GameServer.log.Error("RsaCryptor", ex);
                        return 0;
                    }
                    //DateTime date = new DateTime(src[0] * 256 + src[1], src[2], src[3], src[4], src[5], src[6]);
                    //int fms_key = (src[7] << 8) + src[8];
                    //client.SetFsm(fms_key, version);

                    //string edition = GameServer.Instance.Configuration.Edition;
                    string edition = GameServer.Edition;
                    //if (version.ToString() != edition && edition != "0")
                    //{
                    //    client.Out.SendKitoff(LanguageMgr.GetTranslation("UserLoginHandler.EditionError"));
                    //    //client.Out.SendEditionError(LanguageMgr.GetTranslation("UserLoginHandler.EditionError"));
                    //    client.Disconnect();
                    //    return 0;
                    //}
                    for (int i = 0; i < 8; i++)
                    {
                        tempKey[i] = src[i+7];
                    }
                    client.setKey(tempKey);
                
                    string[] temp = Encoding.UTF8.GetString(src, 15, src.Length - 15).Split(',');
                    if (temp.Length == 2)
                    {
                        string user = temp[0];
                        string pass = temp[1];

                        //TimeSpan span = date - DateTime.UtcNow;
                        //if (Math.Abs(span.TotalMinutes) < 5)
                        //{
                        if (!LoginMgr.ContainsUser(user))
                        {
                            bool isFirst = false;
                            BaseInterface inter = BaseInterface.CreateInterface();
                            PlayerInfo cha = inter.LoginGame(user, pass, ref isFirst);

                            if (cha != null && cha.ID != 0)
                            {
                                if (cha.ID == -2)
                                {
                                    //帐号被禁用
                                    client.Out.SendKitoff(LanguageMgr.GetTranslation("UserLoginHandler.Forbid"));
                                    client.Disconnect();
                                    return 0;
                                }

                                if (!isFirst)
                                {
                                    client.Player = new GamePlayer(cha.ID,user, client, cha);
                                    LoginMgr.Add(cha.ID, client);
                                    client.Server.LoginServer.SendAllowUserLogin(cha.ID);
                                    client.Version = version;
                                }
                                else
                                {
                                    client.Out.SendKitoff(LanguageMgr.GetTranslation("UserLoginHandler.Register"));
                                    client.Disconnect();
                                }
                            }
                            else
                            {
                                //client.Out.SendLoginFailed("用户名密码错误");
                                client.Out.SendKitoff(LanguageMgr.GetTranslation("UserLoginHandler.OverTime"));
                                client.Disconnect();
                            }
                        }
                        else
                        {
                            //避免攻击,有另外的客户端在登陆,则断掉次客户端。
                            client.Out.SendKitoff(LanguageMgr.GetTranslation("UserLoginHandler.LoginError"));
                            client.Disconnect();
                        }
                    }
                    else
                    {
                        client.Out.SendKitoff(LanguageMgr.GetTranslation("UserLoginHandler.LengthError"));
                        //验证格式错误，端口连接
                        client.Disconnect();
                    }
                }
            }
            catch (Exception ex)
            {
                client.Out.SendKitoff(LanguageMgr.GetTranslation("UserLoginHandler.ServerError"));
                client.Disconnect();
                GameServer.log.Error(LanguageMgr.GetTranslation("UserLoginHandler.ServerError"), ex);
            }

            return 1;
        }
    }
}
