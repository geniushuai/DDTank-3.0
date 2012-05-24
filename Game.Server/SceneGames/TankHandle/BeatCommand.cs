using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Phy.Maps;
using Phy.Object;
using Game.Server.Packets;
using SqlDataProvider.Data;
using Game.Server.Managers;

namespace Game.Server.SceneGames.TankHandle
{
    //[CommandAttbute((byte)TankCmdType.BEAT)]
    //public class BeatCommand : ICommandHandler
    //{
        //private static readonly int R = 5;
        //private static readonly int SINKER_POWER = 200;
        //private static readonly int ARM_LENGTH = 100;
        //private static readonly int ATTACK_RADIUS = 50;
        //private static readonly int SINKER_DELAY = 0;

        //public bool HandleCommand(TankGameLogicProcessor process, GamePlayer player, GSPacketIn packet)
        //{
        //    player.CurrentGame.ReturnPacket(player, packet);

        //    if (player.CurrentGame.Data.CurrentIndex == player && player.CurrentGame.Data.Players[player].State != TankGameState.DEAD)
        //    {
        //        if (player.CurrentGame.Data.CurrentFire == null)
        //            player.CurrentGame.Data.CurrentFire = player;

        //        player.CurrentGame.Data.FireLogin = true;

        //        short x = packet.ReadShort();
        //        //short y = packet.ReadShort();
        //        short angle = packet.ReadShort();

        //        //int distance = (int)Math.Sqrt(x * x + y * y);

        //        if (100 == x)
        //        {
        //            //GSPacketIn pkg = new GSPacketIn((byte)ePackageType.GAME_CMD, player.PlayerCharacter.ID);
        //            //pkg.WriteByte((byte)TankCmdType.BEAT);

        //            //player.CurrentGame.SendToAll(pkg);
        //        }
        //        else
        //        {
        //            int CurrentPower = SINKER_POWER * (1 - x / 100);
                    
        //            TankData data = player.CurrentGame.Data;
        //            int BeatX = (int)(player.CurrentGame.Data.Players[player].X + Math.Cos((double)(angle / 180 * Math.PI)) * ARM_LENGTH);
        //            int BeatY = (int)(player.CurrentGame.Data.Players[player].Y + Math.Sin((double)(angle / 180 * Math.PI)) * ARM_LENGTH);

        //            Player[] BeatPlayers = data.CurrentMap.FindPlayers(BeatX, BeatY, ATTACK_RADIUS);

        //            foreach(Player p in BeatPlayers)
        //            {
        //                if(p.IsFrost <= 0)
        //                {
        //                    Kill(player.CurrentGame.Data.Players[player], p, BeatX, BeatY, SINKER_POWER, ATTACK_RADIUS);

        //                    GSPacketIn pkg = player.Out.SendPlayerHealth(p.PlayerDetail, data.Isforce ? 2 : 1);
        //                    player.CurrentGame.SendToPlayerExceptSelf(pkg, player);
        //                }

        //                GSPacketIn RelievePkg = player.Out.SendRelieveSpells(p.PlayerDetail);
        //                player.CurrentGame.SendToPlayerExceptSelf(RelievePkg, player);
        //            }
        //        }

        //        player.CurrentGame.Data.FireLogin = false;

        //        player.CurrentGame.Data.TotalDelay += SINKER_DELAY;
        //        //process.SendArk(player.CurrentGame, player);
        //        player.PropInventory.AddItemTemplate(PropItemMgr.GetRandomFightProp(player.CurrentGame.Data.MapIndex));
        //        player.CurrentGame.Data.Players[player].SetDander(20);

        //        process.SendPlayFinish(player.CurrentGame, player);
        //    }

        //    return true;
        //}

        //public void Kill(Player killer, Player casualty, int x, int y, int power, int Radii)
        //{
        //    TankData data = killer.PlayerDetail.CurrentGame.Data;
        //    GamePlayer AttackPlayer = killer.PlayerDetail;
        //    GamePlayer BeatenPlayer = casualty.PlayerDetail;
        //    double bias = data.GetBiasLenght(casualty.X - x, casualty.Y - y);
        //    int lostHeath = (int)(power * (1 + AttackPlayer.PlayerCharacter.Attack * 0.001) - BeatenPlayer.BaseDefence * (1 + BeatenPlayer.PlayerCharacter.Defence * 0.001));
        //    lostHeath = (int)(lostHeath * (1 - bias / Radii / 4));
        //    lostHeath = data.AddForceWound(lostHeath);
        //    lostHeath = Math.Abs(lostHeath);
        //    casualty.Blood = -lostHeath;

        //    if (AttackPlayer.CurrentTeamIndex != BeatenPlayer.CurrentTeamIndex)
        //    {
        //        killer.TotalHurt += lostHeath;
        //        data.TotalHeathPoint += lostHeath;
        //        data.persons[AttackPlayer.CurrentTeamIndex].TotalKillHealth += lostHeath;
        //    }

        //    if (casualty.State != TankGameState.DEAD && casualty.Blood <= 0)
        //    {
        //        casualty.Dead();

        //    }
        //}
        
    //}
}
