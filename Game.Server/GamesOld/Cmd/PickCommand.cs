using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using Game.Server.Packets.Client;
using Game.Server.Spells;
using Bussiness;
using SqlDataProvider.Data;
using Game.Server.Managers;
using Game.Base.Packets;
using Game.Server.Games.Cmd;
using Phy.Object;
using Bussiness.Managers;

namespace Game.Server.Games.Cmd
{
    /// <summary>
    /// 战斗中拾取宝箱
    /// </summary>
    [GameCommand((byte)TankCmdType.PICK,"战斗中拾取箱子")]
    public class PickCommand:ICommandHandler
    {
        public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
        {
            player.OpenBox(packet.ReadInt());
        }
    }
}
