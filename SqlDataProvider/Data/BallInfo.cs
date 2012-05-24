using System;
using System.Collections.Generic;
using System.Text;

namespace SqlDataProvider.Data
{
    public class BallInfo
    {
        public int ID { set; get; }

        public string Name { set; get; }
        public string Crater { set; get; }
        public int AttackResponse { set; get; }
        public double Power { set; get; }

        public int Radii { set; get; }

        public int Amount { set; get; }

        public string FlyingPartical { set; get; }

        public string BombPartical { set; get; }

        public bool IsSpin { set; get; }

        public int Mass { set; get; }

        public double SpinVA { set; get; }

        public int SpinV { set; get; }

        public int Wind { set; get; }

        public int DragIndex { set; get; }

        public int Weight { set; get; }

        public bool Shake { set; get; }

        public int Delay { set; get; }

        public string ShootSound { get; set; }

        public string BombSound { get; set; }

        public int ActionType { get; set; }

        public bool HasTunnel { get; set; }

    }
}
