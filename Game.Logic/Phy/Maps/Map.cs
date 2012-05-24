using System.Collections.Generic;
using System.Drawing;
using SqlDataProvider.Data;
using Game.Logic.Phy.Object;
using System;

namespace Game.Logic.Phy.Maps
{
    public class Map
    {
        private MapInfo _info;

        private float _wind = 0;

        public float wind
        {
            get { return _wind; }
            set { _wind = value; }
        }

        public float gravity
        {
            get { return _info.Weight; }
        }

        public float airResistance
        {
            get { return _info.DragIndex; }
        }

        private HashSet<Physics> _objects;

        protected Tile _layer1;

        protected Tile _layer2;

        protected Rectangle _bound;

        public Tile Ground
        {
            get { return _layer1; }
        }

        public MapInfo Info
        {
            get { return _info; }
        }

        public Rectangle Bound
        {
            get { return _bound; }
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
            if (_layer1 != null)
            {
                _layer1.Dig(cx, cy, surface, border);
            }
            if (_layer2 != null)
            {
                _layer2.Dig(cx, cy, surface, border);
            }
        }

        public bool IsEmpty(int x, int y)
        {
            return (_layer1 == null || _layer1.IsEmpty(x, y)) && (_layer2 == null || _layer2.IsEmpty(x, y));
        }

        public bool IsRectangleEmpty(Rectangle rect)
        {
            return (_layer1 == null || _layer1.IsRectangleEmptyQuick(rect)) && (_layer2 == null || _layer2.IsRectangleEmptyQuick(rect));
        }

        public Point FindYLineNotEmptyPoint(int x, int y, int h)
        {
            x = x < 0 ? 0 : (x >= _bound.Width) ? _bound.Width - 1 : x;
            y = y < 0 ? 0 : y;
            h = y + h >= _bound.Height ? _bound.Height - y - 1 : h;
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

        public Point FindNextWalkPoint(int x, int y, int direction, int stepX, int stepY)
        {
            if (direction != 1 && direction != -1) return Point.Empty;

            int tx = x + direction * stepX;
            if (tx < 0 || tx > _bound.Width) return Point.Empty;
            Point p = FindYLineNotEmptyPoint(tx, y - stepY - 1, stepY * 2 + 3);
            if (p != Point.Empty)
            {
                if (Math.Abs(p.Y - y) > stepY)
                    p = Point.Empty;
            }
            return p;
        }

        public bool canMove(int x, int y)
        {
            return IsEmpty(x, y) && !IsOutMap(x, y);
        }

        public bool IsOutMap(int x, int y)
        {
            if (x < 0 || x >= _bound.Width || y >= _bound.Height )
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
            phy.SetMap(this);
            lock (_objects)
            {
                _objects.Add(phy);
            }
        }

        public void RemovePhysical(Physics phy)
        {
            phy.SetMap(null);
            lock (_objects)
            {
                _objects.Remove(phy);
            }
        }

        public List<Physics> GetAllPhysicalSafe()
        {
            List<Physics> list = new List<Physics>();

            lock (_objects)
            {
                foreach (Physics p in _objects)
                {
                    list.Add(p);
                }
            }

            return list;
        }

        public List<PhysicalObj> GetAllPhysicalObjSafe()
        {
            List<PhysicalObj> list = new List<PhysicalObj>();

            lock (_objects)
            {
                foreach (Physics p in _objects)
                {
                    if (p is PhysicalObj)
                    {
                        list.Add(p as PhysicalObj);
                    }
                }
            }

            return list;
 
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
                        Rectangle t1 = phy.Bound1;
                        t.Offset(phy.X, phy.Y);
                        t1.Offset(phy.X, phy.Y);
                        if (t.IntersectsWith(rect) || t1.IntersectsWith(rect))
                        {
                            list.Add(phy);
                        }
                    }
                }
            }
            return list.ToArray();
        }

        public List<Player> FindPlayers(int x, int y, int radius)
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
            return list;
        }

        public List<Living> FindLivings(int x, int y, int radius)
        {
            List<Living> list = new List<Living>();
            lock (_objects)
            {
                foreach (Physics phy in _objects)
                {
                    if (phy is Living && phy.IsLiving && phy.Distance(x, y) < radius)
                    {
                        list.Add(phy as Living);
                    }
                }
            }
            return list;
        }

        public List<Living> FindPlayers(int fx, int tx, List<Player> exceptPlayers)
        {
            List<Living> list = new List<Living>();
            lock (_objects)
            {
                foreach (Physics phy in _objects)
                {
                    if (phy is Player && phy.IsLiving && phy.X > fx && phy.X < tx)
                    {
                        if (exceptPlayers != null)
                        {
                            foreach (Player player in exceptPlayers)
                            {
                                if (((Player)phy).DefaultDelay != player.DefaultDelay)
                                {
                                    list.Add(phy as Living);
                                }
                            }
                        }
                        else
                        {
                            list.Add(phy as Living);
                        }
                    }
                }
            }
            return list;
        }
        public List<Living> FindHitByHitPiont(Point p, int radius)
        {
            List<Living> list = new List<Living>();
            lock (_objects)
            {
                foreach (Physics phy in _objects)
                {
                    if (phy is Living && phy.IsLiving && (phy as Living).BoundDistance(p) < radius)
                    {
                        list.Add(phy as Living);
                    }
                }
            }
            return list;
        }

        public Living FindNearestEnemy(int x, int y, double maxdistance, Living except)
        {

            Living player = null;
            lock (_objects)
            {
                foreach (Physics phy in _objects)
                {
                    if (phy is Living && phy != except && phy.IsLiving && ((Living)phy).Team != except.Team)
                    {
                        double dis = phy.Distance(x, y);
                        if (dis < maxdistance)
                        {
                            player = phy as Living;
                            maxdistance = dis;
                        }
                    }
                }
            }

            return player;
        }

        public void Dispose()
        {
            foreach (Physics phy in _objects)
            {
                phy.Dispose();
            }
        }

        public Map Clone()
        {
            Tile layer1 = _layer1 != null ? _layer1.Clone() : null;
            Tile layer2 = _layer2 != null ? _layer2.Clone() : null;

            return new Map(_info, layer1, layer2);
        }
    }
}
