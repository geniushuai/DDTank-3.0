using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Buffer
{
    public class PropsBuffer : AbstractBuffer
    {
        public PropsBuffer(BufferInfo buffer):base(buffer){}

        public override void Start(GamePlayer player)
        {
            PropsBuffer buffer = player.BufferList.GetOfType(typeof(PropsBuffer)) as PropsBuffer;
            if (buffer != null)
            {
                buffer.Info.ValidDate += Info.ValidDate;
                player.BufferList.UpdateBuffer(buffer);
            }
            else
            {
                base.Start(player);
                player.CanUseProp = true;
            }
        }

        public override void Stop()
        {
            m_player.CanUseProp = false;
            base.Stop();
        }
    }
}
