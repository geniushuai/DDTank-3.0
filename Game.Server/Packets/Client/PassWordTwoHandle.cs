using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Bussiness;
namespace Game.Server.Packets.Client
{
    [PacketHandler((byte)ePackageType.PASSWORD_TWO, "二级密码")]
    public class PassWordTwoHandle : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            string msg = "";
            bool result = false;
            int re_Type = 0;
            bool addInfo = false;
            int Count = 0;
            string PasswordTwo = packet.ReadString();
            string PasswordTwo_new = packet.ReadString();
            int Type = packet.ReadInt();
            string PasswordQuestion1 = packet.ReadString();
            string PasswordAnswer1 = packet.ReadString();
            string PasswordQuestion2 = packet.ReadString();
            string PasswordAnswer2 = packet.ReadString();
            switch (Type)
            {
                case 1:
                    {
                        re_Type = 1;
                        if (string.IsNullOrEmpty(client.Player.PlayerCharacter.PasswordTwo))
                            using (PlayerBussiness db = new PlayerBussiness())
                            {
                                if (PasswordTwo != "")
                                {
                                    if (db.UpdatePasswordTwo(client.Player.PlayerCharacter.ID, PasswordTwo))
                                    {
                                        client.Player.PlayerCharacter.PasswordTwo = PasswordTwo;
                                        client.Player.PlayerCharacter.IsLocked = false;
                                        msg = "SetPassword.success";
                                        //string db_PasswordQuestion1 = "";
                                        //string db_PasswordQuestion2 = "";
                                        //string db_PasswordAnswer1 = "";
                                        //string db_PasswordAnswer2 = "";
                                        //db.GetPasswordInfo(client.Player.PlayerCharacter.ID, ref db_PasswordQuestion1, ref db_PasswordAnswer1, ref db_PasswordQuestion2, ref db_PasswordAnswer2, ref Count);
                                        //{
                                        //    if (db_PasswordQuestion1 != "" && db_PasswordQuestion2 != "" && db_PasswordAnswer1 != "" && db_PasswordAnswer2 != "")
                                        //    {
                                        //        result = true;
                                        //        addInfo = false;
                                        //    }
                                        //    else
                                        //    {

                                        //    }
                                        //}
                                    }
                                }
                                if (PasswordQuestion1 != "" && PasswordAnswer1 != "" && PasswordQuestion2 != "" && PasswordAnswer2 != "")
                                {

                                    if (db.UpdatePasswordInfo(client.Player.PlayerCharacter.ID, PasswordQuestion1, PasswordAnswer1, PasswordQuestion2, PasswordAnswer2, 5))
                                    {
                                        result = true;
                                        addInfo = false;
                                        msg = "UpdatePasswordInfo.Success";
                                    }
                                    else
                                    {
                                        result = false;
                                    }
                                }
                                else
                                {
                                    result = true;
                                    addInfo = true;
                                }

                            }
                        else
                        {
                            msg = "SetPassword.Fail";
                            result = false;
                            addInfo = false;
                        }

                    }
                    break;
                case 2:
                    {
                        re_Type = 2;
                        if (PasswordTwo == client.Player.PlayerCharacter.PasswordTwo)
                        {
                            client.Player.PlayerCharacter.IsLocked = false;
                            msg = "BagUnlock.success";
                            result = true;
                            //using (PlayerBussiness db = new PlayerBussiness())
                            //{
                            //    db.GetPasswordInfo(client.Player.PlayerCharacter.ID, ref PasswordQuestion1, ref PasswordAnswer1, ref PasswordQuestion2, ref PasswordAnswer2, ref Count);
                            //    {
                            //        if (PasswordQuestion1 != "" && PasswordQuestion2 != "" && PasswordAnswer1 != "" && PasswordAnswer2 != "")
                            //        {
                            //            addInfo = false;
                            //        }

                            //        else
                            //        {
                            //            addInfo = true;
                            //        }
                            //    }
                            //}

                        }
                        else
                        {
                            msg = "PasswordTwo.error";
                            result = false;
                            addInfo = false;
                        }
                    }
                    break;

                case 3:
                    {
                        re_Type = 3;
                        using (PlayerBussiness db = new PlayerBussiness())
                        {
                            db.GetPasswordInfo(client.Player.PlayerCharacter.ID, ref PasswordQuestion1, ref PasswordAnswer1, ref PasswordQuestion2, ref PasswordAnswer2, ref Count);
                            Count--;
                            db.UpdatePasswordInfo(client.Player.PlayerCharacter.ID, PasswordQuestion1, PasswordAnswer1, PasswordQuestion2, PasswordAnswer2, Count);

                            if (PasswordTwo == client.Player.PlayerCharacter.PasswordTwo)
                            {

                                if (db.UpdatePasswordTwo(client.Player.PlayerCharacter.ID, PasswordTwo_new))
                                {
                                    client.Player.PlayerCharacter.IsLocked = false;
                                    client.Player.PlayerCharacter.PasswordTwo = PasswordTwo_new;
                                    msg = "UpdatePasswordTwo.Success";
                                    result = true;

                                    //if (PasswordAnswer1 == "" || PasswordAnswer2 == "")
                                    //{
                                    //    addInfo = true;
                                    //}
                                    //else
                                    //{
                                    addInfo = false;
                                    // }
                                }
                                else
                                {
                                    msg = "UpdatePasswordTwo.Fail";
                                    result = false;
                                    addInfo = false;
                                }
                            }

                            else
                            {
                                msg = "PasswordTwo.error";
                                result = false;
                                addInfo = false;
                            }
                        }
                    }
                    break;
                case 4:
                    {
                        re_Type = 4;

                        string db_PasswordAnswer1 = "";
                        string PassWordTwo = "";
                        string db_PasswordAnswer2 = "";
                        using (PlayerBussiness db = new PlayerBussiness())
                        {
                            db.GetPasswordInfo(client.Player.PlayerCharacter.ID, ref PasswordQuestion1, ref db_PasswordAnswer1, ref PasswordQuestion2, ref db_PasswordAnswer2, ref Count);
                            Count--;
                            db.UpdatePasswordInfo(client.Player.PlayerCharacter.ID, PasswordQuestion1, PasswordAnswer1, PasswordQuestion2, PasswordAnswer2, Count);
                            if (db_PasswordAnswer1 == PasswordAnswer1 && db_PasswordAnswer2 == PasswordAnswer2 && db_PasswordAnswer1 != "" && db_PasswordAnswer2 != "")
                            {

                                if (db.UpdatePasswordTwo(client.Player.PlayerCharacter.ID, PassWordTwo))
                                {
                                    client.Player.PlayerCharacter.PasswordTwo = PassWordTwo;
                                    client.Player.PlayerCharacter.IsLocked = false;
                                    msg = "DeletePassword.success";
                                    result = true;
                                    addInfo = false;

                                }
                                else
                                {

                                    msg = "DeletePassword.Fail";
                                    result = false;
                                }
                            }
                            else
                            {
                                if (PasswordTwo == client.Player.PlayerCharacter.PasswordTwo)
                                {
                                    if (db.UpdatePasswordTwo(client.Player.PlayerCharacter.ID, PassWordTwo))
                                    {
                                        client.Player.PlayerCharacter.PasswordTwo = PassWordTwo;
                                        client.Player.PlayerCharacter.IsLocked = false;

                                        msg = "DeletePassword.success";
                                        result = true;
                                        addInfo = false;
                                    }

                                }
                                else
                                {

                                    msg = "DeletePassword.Fail";
                                    result = false;
                                }
                            }
                        }

                    }
                    break;

                case 5:
                    {
                        re_Type = 5;

                        if (client.Player.PlayerCharacter.PasswordTwo != null)
                        {
                            if (PasswordQuestion1 != "" && PasswordAnswer1 != "" && PasswordQuestion2 != "" && PasswordAnswer2 != "")
                            {
                                using (PlayerBussiness db = new PlayerBussiness())
                                {
                                    if (db.UpdatePasswordInfo(client.Player.PlayerCharacter.ID, PasswordQuestion1, PasswordAnswer1, PasswordQuestion2, PasswordAnswer2, 5))
                                    {
                                        result = true;
                                        addInfo = false;
                                        msg = "UpdatePasswordInfo.Success";
                                    }
                                    else
                                    {
                                        result = false;
                                    }
                                }
                            }
                        }
                    }
                    break;
                //case 6:
                //    {
                //        re_Type = 6;
                //        if (client.Player.PlayerCharacter.PasswordTwo != null)
                //        {
                //            using (PlayerBussiness db = new PlayerBussiness())
                //            {
                //                db.GetPasswordInfo(client.Player.PlayerCharacter.ID, ref PasswordQuestion1, ref PasswordAnswer1, ref PasswordQuestion2, ref PasswordAnswer2, ref Count);

                //                result = true;
                //                addInfo = true;

                //            }
                //        }
                //    }
                //    break;


            }
            GSPacketIn pkg = packet.Clone();
            pkg.ClearContext();
            pkg.WriteInt(client.Player.PlayerCharacter.ID);
            pkg.WriteInt(re_Type);
            pkg.WriteBoolean(result);
            pkg.WriteBoolean(addInfo);
            pkg.WriteString(LanguageMgr.GetTranslation(msg));
            pkg.WriteInt(Count);
            pkg.WriteString(PasswordQuestion1);
            pkg.WriteString(PasswordQuestion2);

            client.Out.SendTCP(pkg);
            return 0;
        }

    }


}
