using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Logic.Actions;


namespace Game.Logic.Phy.Object
{
    public class Layer : PhysicalObj
    {
        public Layer(int id, string name, string model, string defaultAction, int scale, int rotation)
            : base(id, name, model, defaultAction, scale, rotation)
        {

        }

        public override int Type
        {
            get { return 2; }
        }
    }
}
