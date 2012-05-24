using SqlDataProvider.Data;
using Game.Base.Packets;
using Phy.Object;

namespace Game.Server.Packets.Client
{
    [PacketHandler((byte)ePackageType.GAME_TAKE_OUT,"抽取物品")]
    public class GameTakeOutHandler:IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.CurrentGame == null)
                return 0;

            //GSPacketIn pkg = packet.Clone();

            //TankData data = client.Player.CurrentGame.Data;
            //byte index = packet.ReadByte();
            //if (index > 7)
            //{
            //    index = data.GetCards();
            //    pkg.ClearContext();
            //    pkg.WriteByte(index);
            //}

            //if (!data.Players[client.Player].IsTakeOut || data.Cards[index])
            //    return 0;

            //pkg.ClientID = (client.Player.PlayerCharacter.ID);
            //data.Cards[index] = true;
            //data.Players[client.Player].IsTakeOut = false;
            //data.TakeOutArk();



            //MapGoodsInfo info = MapMgr.GetRandomAward(data.MapIndex,(int)data.MapType);

            //int gold = data.GetRandomGold(client.Player.CurrentGame.RoomType);
            //client.Player.SetGold(gold, Game.Server.Statics.GoldAddType.Cards);
            //pkg.WriteByte(1);
            //pkg.WriteInt(gold);
            bool isItem = false;
            int value = 100;
            if (info != null)
            {
                if (info.GoodsID > 0)
                {
                    ItemTemplateInfo temp = Bussiness.Managers.ItemMgr.GetSingleGoods(info.GoodsID);
                    if (temp != null)
                    {
                        isItem = true;
                        value = info.GoodsID;
                        client.Player.TempInventory.AddItemTemplate(temp, info);
                    }
                }
                else if (info.GoodsID == -1)
                {
                    value = info.Value;
                }
            }

            //if (!isItem)
            //{
            //    value = (int)(value * AntiAddictionMgr.GetAntiAddictionCoefficient(client.Player.PlayerCharacter.AntiAddiction));
            //    client.Player.SetGold(value, Game.Server.Statics.GoldAddType.Cards);
            //}

            //pkg.WriteByte(isItem ? (byte)0 : (byte)1);
            //pkg.WriteInt(value);
            

            //client.Player.CurrentGame.SendToAll(pkg);


            //GamePlayer[] players = client.Player.CurrentGame.GetAllPlayers();
            //bool flag = true;
            //foreach (GamePlayer p in players)
            //{
            //    if (data.Players.ContainsKey(p) && data.Players[p].IsTakeOut)
            //    {
            //        flag = false;
            //        break;
            //    }
            //}

            //if (flag)
            //{
            //    client.Player.CurrentGame.ShowArk(client.Player.CurrentGame, client.Player);
            //}

            return 0;
        }
    }
}
