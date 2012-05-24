using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.Managers;
using Game.Server.GameObjects;
using Game.Logic;
using Game.Server.Rooms;
using Game.Server.Games;
using Game.Server.Battle;
using Game.Base;

namespace Game.Server.Commands.Admin
{
    [CmdAttribute("&list",
        ePrivLevel.Player,
        "List the objects info in game",
        "   /list [Option1][Option2] ...",
        "eg:    /list -g :list all game objects",
        "       /list -c :list all client objects",
        "       /list -p :list all gameplaye objects",
        "       /list -r :list all room objects",
        "       /list -b :list all battle servers")]
    public class ListObjectsCommand:AbstractCommandHandler,ICommandHandler
    {
        public bool OnCommand(BaseClient client, string[] args)
        {
            if (args.Length > 1)
            {
                switch (args[1])
                {
                    case "-c":
                        Console.WriteLine("client list:");
                        Console.WriteLine("-------------------------------");
                        GameClient[] cs = GameServer.Instance.GetAllClients();
                        foreach (GameClient cl in cs)
                        {
                            Console.WriteLine(cl.ToString());
                        }
                        Console.WriteLine("-------------------------------");
                        Console.WriteLine(string.Format("total:{0}", cs.Length));
                        break;
                    case "-p":
                        Console.WriteLine("player list:");
                        Console.WriteLine("-------------------------------");
                        GamePlayer[] ps = WorldMgr.GetAllPlayers();
                        foreach (GamePlayer player in ps)
                        {
                            Console.WriteLine(player.ToString());
                        }
                        Console.WriteLine("-------------------------------");
                        Console.WriteLine(string.Format("total:{0}", ps.Length));
                        break;
                    case "-r":
                        Console.WriteLine("room list:");
                        Console.WriteLine("-------------------------------");
                        List<BaseRoom> rs = RoomMgr.GetAllUsingRoom();
                        foreach (BaseRoom room in rs)
                        {
                            Console.WriteLine(room.ToString());
                        }
                        Console.WriteLine("-------------------------------");
                        Console.WriteLine(string.Format("total:{0}", rs.Count));
                        break;
                    case "-g":
                        Console.WriteLine("game list:");
                        Console.WriteLine("-------------------------------");
                        List<BaseGame> gs = GameMgr.GetAllGame();
                        foreach (BaseGame g in gs)
                        {
                            Console.WriteLine(g.ToString());
                        }
                        Console.WriteLine("-------------------------------");
                        Console.WriteLine(string.Format("total:{0}", gs.Count));
                        break;
                    case "-b":
                        Console.WriteLine("battle list:");
                        Console.WriteLine("-------------------------------");
                        List<BattleServer> bs = BattleMgr.GetAllBattles();
                        foreach (BattleServer battleSvr in bs)
                        {
                            Console.WriteLine(battleSvr.ToString());
                        }
                        Console.WriteLine("-------------------------------");
                        Console.WriteLine(string.Format("total:{0}", bs.Count));
                        break;
                    default:
                        DisplaySyntax(client);
                        break;
                }
            }
            else
            {
                DisplaySyntax(client);
            }
            return true;
        }
    }
}
