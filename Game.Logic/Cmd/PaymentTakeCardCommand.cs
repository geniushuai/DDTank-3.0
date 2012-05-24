using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Logic.Phy.Object;
using Game.Base.Packets;

namespace Game.Logic.Cmd
{
    [GameCommand((byte)eTankCmdType.PAYMENT_TAKE_CARD, "付费翻牌")]
    public class PaymentTakeCardCommand : ICommandHandler
    {
        public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
        {
            if (player.HasPaymentTakeCard == false)
            {
                if (player.PlayerDetail.RemoveMoney(100) > 0&&player.PlayerDetail.RemoveGiftToken(429)>0)
                {
                    int index = packet.ReadByte();
                    player.CanTakeOut += 1;
                    player.FinishTakeCard = false;
                    player.HasPaymentTakeCard = true;
                    player.PlayerDetail.LogAddMoney(AddMoneyType.Game, AddMoneyType.Game_PaymentTakeCard, player.PlayerDetail.PlayerCharacter.ID, 100, player.PlayerDetail.PlayerCharacter.Money);

                    if (index < 0 || index > game.Cards.Length)
                    {
                        game.TakeCard(player);
                    }
                    else
                    {
                        game.TakeCard(player, index);
                    }
                }
                else
                {
                    player.PlayerDetail.SendInsufficientMoney((int)eBattleRemoveMoneyType.PaymentTakeCard);
                }
            }
        }
    }
}
