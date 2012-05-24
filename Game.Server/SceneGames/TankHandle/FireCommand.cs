using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using Game.Server.Packets.Client;
using Game.Server.Spells;
using SqlDataProvider.Data;
using Game.Base.Packets;
using Phy.Object;
using Phy.Maps;
using Phy.Actions;
using Game.Server.Packets;
using System.Drawing;
using Game.Server.Managers;
using Game.Server.Statics;

namespace Game.Server.SceneGames.TankHandle
{
    [CommandAttbute((byte)TankCmdType.FIRE)]
    public class FireCommand:ICommandHandler
    {
        public bool HandleCommand(TankGameLogicProcessor process, GamePlayer player, GSPacketIn packet)
        {
            if (player.CurrentGame.Data.CurrentIndex == player && player.CurrentGame.Data.Players[player].State != TankGameState.DEAD)
            {
                if (player.CurrentGame.Data.CurrentFire == null)
                    player.CurrentGame.Data.CurrentFire = player;

                int x = packet.ReadInt();
                int y = packet.ReadInt();

                if (Math.Abs(player.CurrentGame.Data.Players[player].X - x) > 100)
                {
                    //player.Out.SendMessage(eMessageType.ALERT, LanguageMgr.GetTranslation("Game.Server.SceneGames.TankHandle"));
                    StatMgr.LogErrorPlayer(player.PlayerCharacter.ID, player.PlayerCharacter.UserName, player.PlayerCharacter.NickName,
     ItemRemoveType.FireError, player.CurrentGame.Data.Players[player].X.ToString() + " to " + x.ToString() + ",MapID:" + player.CurrentGame.Data.MapIndex);
                    player.Client.Disconnect();
                    return false; ;
                }

                int force = packet.ReadInt();
                int angle = packet.ReadInt();

                TankData data = player.CurrentGame.Data;
                Tile shape = Managers.BallMgr.FindTile(data.CurrentBall.ID);
                GSPacketIn pkg = new GSPacketIn((byte)ePackageType.GAME_CMD, player.PlayerCharacter.ID);
                pkg.WriteByte((byte)TankCmdType.FIRE);
                pkg.WriteInt(data.AddBall);
                for (int i = 0; i < data.AddBall; i++)
                {
                    if (player.CurrentGame.Data.ReduceFireBombs)
                    {
                        if (data.IsFastSpeed())
                        {
                            StatMgr.LogErrorPlayer(player.PlayerCharacter.ID, player.PlayerCharacter.UserName, player.PlayerCharacter.NickName,
                                ItemRemoveType.FastError, "MapID:" + player.CurrentGame.Data.MapIndex);
                            player.Client.Disconnect();
                            return false; ;
                        }

                        data.FireLogin = true;
                        double reforce = 1;
                        int reangle = 0;
                        if (i == 1)
                        {
                            reforce = 0.9;
                            reangle = -5;
                        }
                        else if (i == 2)
                        {
                            reforce = 1.1;
                            reangle = 5;
                        }

                        int vx = (int)(force * reforce * Math.Cos((double)(angle + reangle) / 180 * Math.PI));
                        int vy = (int)(force * reforce * Math.Sin((double)(angle + reangle) / 180 * Math.PI));

                        data.PhyID++;

                        BombObject bomb = new BombObject(data.PhyID, BallMgr.GetBallType(data.CurrentBall.ID), data.Players[player], shape, data.CurrentBall.Radii, data.AddMultiple < 1,
                            data.CurrentBall.Mass, data.CurrentBall.Weight, data.CurrentBall.Wind, data.CurrentBall.DragIndex, data.BallPower);

                        bomb.SetXY(x, y);
                        bomb.setSpeedXY(vx, vy);
                        data.CurrentMap.AddPhysical(bomb);
                        pkg.WriteBoolean(bomb.IsHole);
                        pkg.WriteInt(bomb.Id);
                        pkg.WriteInt(x);
                        pkg.WriteInt(y);
                        pkg.WriteInt(vx);
                        pkg.WriteInt(vy);
                        pkg.WriteInt(bomb.Actions.Count);
                        foreach (BombAction action in bomb.Actions)
                        {
                            pkg.WriteInt(action.TimeInt);
                            pkg.WriteInt(action.Type);
                            pkg.WriteInt(action.Param1);
                            pkg.WriteInt(action.Param2);
                            pkg.WriteInt(action.Param3);
                            pkg.WriteInt(action.Param4);
                        }

                        data.SetRunTime((int)bomb.RunTime);
                    }
                }

                player.CurrentGame.SendToAll(pkg);

                data.FireLogin = false;
                if (data.Bombs || data.Players[player].State == TankGameState.DEAD)
                {
                    data.TotalDelay += data.CurrentBall.Delay;
                    process.SendArk(player.CurrentGame, player);
                    player.PropInventory.AddItemTemplate(PropItemMgr.GetRandomFightProp(data.MapIndex));
                    data.Players[player].SetDander(20);

                    process.SendPlayFinish(player.CurrentGame, player);
                    //GSPacketIn pkgMsg = new GSPacketIn((byte)ePackageType.GAME_CMD);
                    //pkgMsg.WriteByte((byte)TankCmdType.PLAYFINISH);
                    //pkgMsg.WriteInt(player.CurrentGame.Data.TurnNum);
                    //player.CurrentGame.SendToAll(pkgMsg);

                }
                return true;

            }
            return false;
        }
    }
}
