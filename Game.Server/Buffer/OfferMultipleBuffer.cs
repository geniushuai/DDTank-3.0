using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using SqlDataProvider.Data;


namespace Game.Server.Buffer
{
    public class OfferMultipleBuffer : AbstractBuffer
    {
        public OfferMultipleBuffer(BufferInfo info):base(info){}

        public override void Start(GamePlayer player)
        {
            OfferMultipleBuffer old = player.BufferList.GetOfType(typeof(OfferMultipleBuffer)) as OfferMultipleBuffer;
            if (old != null)
            {
                old.Info.ValidDate += Info.ValidDate;
                player.BufferList.UpdateBuffer(old);
            }
            else
            {
                base.Start(player);
                player.OfferAddPlus *= Info.Value;
            }
        }

        public override void Stop()
        {
            m_player.OfferAddPlus /= m_info.Value;
            base.Stop();           
        }
    }
}
