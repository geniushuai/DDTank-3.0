using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Logic.Actions;

namespace Game.Logic.Phy.Object
{
    public class PhysicalObj:Physics
    {
        private string m_model;
        private string m_currentAction;
        private int m_scale;
        private int m_rotation;
        private BaseGame m_game;
        private bool m_canPenetrate;
        private string m_name;

        public PhysicalObj(int id, string name, string model,string defaultAction,int scale,int rotation) : base(id) 
        {
            m_name = name;
            m_model = model;
            m_currentAction = defaultAction;
            m_scale = scale;
            m_rotation = rotation;
            m_canPenetrate = false;
        }

        public virtual int Type
        {
            get { return 0; }
        }

        public string Model
        {
            get { return m_model; }
        }

        public string CurrentAction
        {
            get { return m_currentAction; }
            set { m_currentAction = value; }
        }

        public int Scale
        {
            get { return m_scale; }
        }

        public int Rotation
        {
            get { return m_rotation; }
        }

        public bool CanPenetrate
        {
            get { return m_canPenetrate; }
            set { m_canPenetrate = value; }
        }

        public void SetGame(BaseGame game)
        {
            m_game = game;
        }

        public void PlayMovie(string action, int delay, int movieTime)
        {
            if (m_game != null)
            {
                m_game.AddAction(new PhysicalObjDoAction(this, action, delay, movieTime));
            }
        }

        public override void CollidedByObject(Physics phy)
        {
            if (m_canPenetrate == false && phy is SimpleBomb)
            {
                ((SimpleBomb)phy).Bomb();
            }
        }

        public string Name
        {
            get { return m_name; }
        }
    }
}
