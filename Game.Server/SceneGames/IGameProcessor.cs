using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using Game.Server.Packets.Client;
using Game.Base.Packets;

namespace Game.Server.SceneGames
{
    public interface IGameProcessor
    {
        int MaxPlayerCount { get; }

        void InitGame(BaseSceneGame game);

        void OnGameData(BaseSceneGame game, GamePlayer player, GSPacketIn packet);

        void OnAddedPlayer(BaseSceneGame game, GamePlayer player);

        void OnRemovingPlayer(BaseSceneGame game, GamePlayer player);

        void OnRemovedPlayer(BaseSceneGame game, GamePlayer player);

        void OnAddedVistor(BaseSceneGame game, GamePlayer vistor, GamePlayer viewPlayer);

        void OnRemovedVistor(BaseSceneGame game, GamePlayer vistor);

        void OnPlayerStateChanged(BaseSceneGame game, GamePlayer player);

        void OnPlayerTeamChanged(BaseSceneGame game, GamePlayer player);

        bool OnCanStartGame(BaseSceneGame game, GamePlayer player);

        void OnStarting(BaseSceneGame game, GSPacketIn data);

        void OnStarted(BaseSceneGame game);

        void OnStopping(BaseSceneGame game, GSPacketIn pkg);

        void OnStopped(BaseSceneGame game);

        void OnTick(BaseSceneGame game);

        void OnShowArk(BaseSceneGame game, GamePlayer player);

        void SendPlayFinish(BaseSceneGame game, GamePlayer player);

        void SendPairUpWait(BaseSceneGame game);

        void SendPairUpFailed(BaseSceneGame game);

        bool CanStopGame(BaseSceneGame game, TankData data);

        bool OnCanStartPairUpGame(BaseSceneGame game, GamePlayer player);
    }
}
