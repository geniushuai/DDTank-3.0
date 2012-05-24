using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Logic.AI
{
    public abstract class AMissionControl
    {
        private PVEGame m_game;
        public PVEGame Game
        {
            get { return m_game; }
            set { m_game = value; }
        }

        //public virtual void OnPrepareGameOver() { }
        //public virtual void OnPrepareStartGame() { }
        //public virtual void OnPrepareNewGame(){}


        public virtual void OnPrepareNewSession() { }
        public virtual void OnStartMovie() { }
        public virtual void OnStartGame() { }
        public virtual void OnBeginNewTurn() { }
        public virtual void OnNewTurnStarted() { }
        public virtual bool CanGameOver() { return true; } 
        public virtual void OnGameOverMovie() { }
        public virtual void OnGameOver() { }
        public virtual int CalculateScoreGrade(int score) { return 0; }
        public virtual int UpdateUIData() { return 0; }
        public virtual void Dispose() { }
        public virtual void DoOther() { }
        public virtual void OnShooted() { }
    }
}
