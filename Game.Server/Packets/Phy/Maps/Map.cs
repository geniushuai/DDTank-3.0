using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Phy.Object;
using System.Drawing;
using SqlDataProvider.Data;

namespace Phy.Maps
{
    public class Map
    {
        private MapInfo _info;

        public float _wind = 3;

        //public float gravity = 9.8F;

        //public float airResistance = 2;
        public float wind
        {
            get
            {
                return _wind / 10;
            }
            set
            {

                _wind = value;
            }
        }

        public float gravity
        {
            get
            {
                return _info.Weight;
            }
        }

        public float airResistance
        {
            get
            {
                return _info.DragIndex;
            }
        }

        private HashSet<Physics> _objects;

        protected Tile _layer1;

        protected Tile _layer2;

        protected Rectangle _bound;

        public Tile Ground
        {
            get
            {
                return _layer1;
            }
        }

        public MapInfo Info
        {
            get
            {
                return _info;
            }
        }

        public Map(MapInfo info, Tile layer1, Tile layer2)
        {
            _info = info;

            _objects = new HashSet<Physics>();

            _layer1 = layer1;
            _layer2 = layer2;

            if (_layer1 != null)
            {
                _bound = new Rectangle(0, 0, _layer1.Width, _layer1.Height);
            }
            else
            {
                _bound = new Rectangle(0, 0, _layer2.Width, _layer2.Height);
            }
        }

        public void Dig(int cx, int cy, Tile surface, Tile border)
        {
            _layer1.Dig(cx, cy, surface, border);
            if (_layer2 != null)
            {
                _layer2.Dig(cx, cy, surface, border);
            }
        }

        public bool IsEmpty(int x, int y)
        {
            return (_layer1.IsEmpty(x, y)) && (_layer2 == null || _layer2.IsEmpty(x, y));
        }

        public bool IsRectangleEmpty(Rectangle rect)
        {
            return (_layer1.IsRectangleEmptyQuick(rect)) && (_layer2 == null || _layer2.IsRectangleEmptyQuick(rect));
        }

        public Point FindYLineNotEmptyPoint(int x, int y, int h)
        {
            x = x < 0 ? 0 : (x >= _layer1.Width) ? _layer1.Width - 1 : x;
            y = y < 0 ? 0 : y;
            h = y + h >= _layer1.Height ? _layer1.Height - y - 1 : h;
            for (int i = 0; i < h; i++)
            {
                if (!IsEmpty(x - 1, y) || !IsEmpty(x + 1, y))
                    return new Point(x, y);
                y++;
            }
            return Point.Empty;
        }

        public Point FindYLineNotEmptyPoint(int x, int y)
        {
            return FindYLineNotEmptyPoint(x, y, _bound.Height);
        }

        public bool canMove(int x, int y)
        {
            return IsEmpty(x, y) && !IsOutMap(x, y);
        }

        public bool IsOutMap(int x, int y)
        {
            if (x < 0 || x >= _bound.Width || y >= _bound.Height)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void AddPhysical(Physics phy)
        {

            phy.setMap(this);
            lock (_objects)
            {
                _objects.Add(phy);
            }
            phy.StartMoving();
        }

        public void RemovePhysical(Physics phy)
        {

            phy.setMap(null);
            lock (_objects)
            {
                _objects.Remove(phy);
            }
        }

        public Physics[] FindPhysicalObjects(Rectangle rect, Physics except)
        {
            List<Physics> list = new List<Physics>();
            lock (_objects)
            {
                foreach (Physics phy in _objects)
                {
                    if (phy.IsLiving && phy != except)
                    {
                        Rectangle t = phy.Bound;

                        t.Offset(phy.X, phy.Y);
                        if (t.IntersectsWith(rect))
                        {
                            list.Add(phy);
                        }
                    }
                }
            }
            return list.ToArray();
        }

        public Player[] FindPlayers(int x, int y, int radius)
        {

            List<Player> list = new List<Player>();
            lock (_objects)
            {
                foreach (Physics phy in _objects)
                {
                    if (phy is Player && phy.IsLiving && phy.Distance(x, y) < radius)
                    {
                        list.Add(phy as Player);
                    }
                }
            }
            return list.ToArray();
        }

        public Player FindNearestEnemy(int x, int y, int maxdistance, Player except)
        {

            Player player = null;
            lock (_objects)
            {
                foreach (Physics phy in _objects)
                {
                    if (phy is Player && phy != except && phy.IsLiving && ((Player)phy).PlayerDetail.CurrentTeamIndex != except.PlayerDetail.CurrentTeamIndex)
                    {
                        int dis = phy.Distance(x, y);
                        if (dis < maxdistance)
                        {
                            player = phy as Player;
                            maxdistance = dis;
                        }
                    }
                }
            }

            return player;
        }

        public Map Clone()
        {
            Tile layer1 = _layer1 != null ? _layer1.Clone() : null;
            Tile layer2 = _layer2 != null ? _layer2.Clone() : null;

            return new Map(_info, layer1, layer2);
            //return new Map(_info, _layer1.Clone(), _layer2.Clone());
        }
    }
}
