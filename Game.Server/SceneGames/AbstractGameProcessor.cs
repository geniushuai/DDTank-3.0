using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Game.Server.Packets;
using Game.Server.GameObjects;

namespace Game.Server.SceneGames
{
    public abstract class AbstractGameProcessor:IGameProcessor
    {

        public abstract int MaxPlayerCount
        {
            get;
        }

        public virtual void InitGame(BaseSceneGame game)
        { 
        }

        public virtual void OnGameData(BaseSceneGame game, Game.Server.GameObjects.GamePlayer player, GSPacketIn packet)
        {
        }

        public virtual void OnAddedPlayer(BaseSceneGame game, Game.Server.GameObjects.GamePlayer player)
        {
        }

        public virtual void OnRemovedPlayer(BaseSceneGame game, Game.Server.GameObjects.GamePlayer player)
        {
        }

        public virtual void OnAddedVistor(BaseSceneGame game, Game.Server.GameObjects.GamePlayer vistor, Game.Server.GameObjects.GamePlayer viewPlayer)
        {
        }

        public virtual void OnRemovedVistor(BaseSceneGame game, Game.Server.GameObjects.GamePlayer vistor)
        {
        }

        public virtual void OnPlayerStateChanged(BaseSceneGame game, Game.Server.GameObjects.GamePlayer player)
        {
        }

        public virtual void OnPlayerTeamChanged(BaseSceneGame game, Game.Server.GameObjects.GamePlayer player)
        {
        }

        public virtual bool OnCanStartGame(BaseSceneGame game, Game.Server.GameObjects.GamePlayer player)
        {
            return false;
        }

        public virtual bool OnCanStartPairUpGame(BaseSceneGame game, Game.Server.GameObjects.GamePlayer player)
        {
            return false;
        }

        public virtual void OnStarting(BaseSceneGame game, GSPacketIn data)
        {
        }

        public virtual void OnStarted(BaseSceneGame game)
        {
        }

        public virtual void OnStopping(BaseSceneGame game, GSPacketIn pkg)
        {
        }

        public virtual void OnStopped(BaseSceneGame game)
        {
        }

        public GSPacketIn CreatePacket()
        {
            return new GSPacketIn((int)ePackageType.GAME_CMD);
        }

        public virtual void OnTick(BaseSceneGame game)
        {
        }


        public virtual void OnRemovingPlayer(BaseSceneGame game, Game.Server.GameObjects.GamePlayer player)
        {
        }

        public virtual void OnShowArk(BaseSceneGame game, GamePlayer player)
        { }

        public virtual void SendPlayFinish(BaseSceneGame game, GamePlayer player)
        { }

        public virtual void SendPairUpWait(BaseSceneGame game)
        { }

        public virtual void SendPairUpFailed(BaseSceneGame game)
        { }

        public virtual bool CanStopGame(BaseSceneGame game, TankData data)
        {
            return false;
        }

    }
}
