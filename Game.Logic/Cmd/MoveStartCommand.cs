using Game.Base.Packets;
using Game.Logic.Phy.Object;
using System;

namespace Game.Logic.Cmd
{
    [GameCommand((byte)eTankCmdType.MOVESTART, "开始移动")]
    public class MoveStartCommand : ICommandHandler
    {
        public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
        {
            if (player.IsAttacking)
            {
                GSPacketIn pkg = packet.Clone();
                pkg.ClientID = player.PlayerDetail.PlayerCharacter.ID;
                pkg.Parameter1 = player.Id;
                game.SendToAll(pkg, player.PlayerDetail);

                byte type = packet.ReadByte();
                int tx = packet.ReadInt();
                int ty = packet.ReadInt();
                byte dir = packet.ReadByte();
                bool isLiving = packet.ReadBoolean();
                //Console.WriteLine("isLiving : {0}, tx : {1}, ty : {2}, type : {3}, playerId : {4}", isLiving, tx, ty, type, player.Id);
                switch (type)
                {
                    case 0:
                    case 1:
                        //p.X = tx;
                        //p.Y = ty;
                       // if (player.PlayerDetail.PlayerCharacter.NickName == "jacken123")
                           // Console.WriteLine(player.PlayerDetail.PlayerCharacter.NickName + "X坐标" + player.X.ToString() + "Y坐标" + player.Y.ToString());
                        player.SetXY(tx, ty);
                        player.StartMoving();
                       // if (player.PlayerDetail.PlayerCharacter.NickName == "jacken123")
                           // Console.WriteLine(string.Format("修正掉落: 动作类型  {0}    原始Y:{1}     最新Y:{2}", type, ty, player.Y));
                        if (player.Y - ty > 1 || player.IsLiving != isLiving)
                        {
                            //Console.WriteLine("player.IsLiving : {0}, playerId : {1}", player.IsLiving, player.Id);
                            //把服务器修正的掉落指令和客户掉落指令加以区分。
                            Console.WriteLine("玩家移动掉落："+player.IsLiving.ToString());
                            game.SendPlayerMove(player, 3, player.X, player.Y, 0, player.IsLiving, null);
                        }
                        break;
                    case 2:
                        break;
                }
            }
        }
    }
}
