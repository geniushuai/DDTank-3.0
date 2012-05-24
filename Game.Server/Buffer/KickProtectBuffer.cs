using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Buffer
{
    public class KickProtectBuffer : AbstractBuffer
    {
        public KickProtectBuffer(BufferInfo info) : base(info) { }

        public override void Start(GamePlayer player)
        {
            KickProtectBuffer buffer = player.BufferList.GetOfType(typeof(KickProtectBuffer)) as KickProtectBuffer;
            if (buffer != null)
            {
                buffer.Info.ValidDate += Info.ValidDate;
                player.BufferList.UpdateBuffer(buffer);
            }
            else
            {
                base.Start(player);
                player.KickProtect = true;
            }
        }

        public override void Stop()
        {
            m_player.KickProtect = false;
            base.Stop();
        }
    }
}
