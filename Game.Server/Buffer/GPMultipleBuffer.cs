using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Buffer
{
    public class GPMultipleBuffer : AbstractBuffer
    {   
        public GPMultipleBuffer(BufferInfo info):base(info){}

        public override void Start(GamePlayer player)
        {
            GPMultipleBuffer buffer = player.BufferList.GetOfType(typeof(GPMultipleBuffer)) as GPMultipleBuffer;
            if (buffer != null)
            {
                buffer.Info.ValidDate += Info.ValidDate;
                player.BufferList.UpdateBuffer(buffer);
            }
            else
            {
                base.Start(player);
                player.GPAddPlus *= Info.Value;
            }
        }

        public override void Stop()
        {
            if (m_player != null)
            {
                m_player.GPAddPlus /= Info.Value;
                base.Stop();
            }
        }
    }
}
