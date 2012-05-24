using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Phy.Maps;
using Phy.Maths;
using Phy.Actions;
using System.Drawing;
using Phy.Object;
using Game.Server;

namespace Phy.Object
{
    public class BombObject : Physics
    {
        protected float _mass;

        protected float _gravityFactor;

        protected float _windFactor;

        protected float _airResitFactor;

        protected EulerVector _vx;

        protected EulerVector _vy;

        protected Tile _shape;

        protected int _radius;

        protected Player _owner;

        protected double _power;

        protected List<BombAction> _actions;

        protected BombType _type;

        protected bool _controled;

        protected bool _isHole;

        public List<BombAction> Actions
        {
            get
            {
                return _actions;
            }
        }

        public bool IsHole
        {
            get
            {
                return _isHole;
            }
        }

        public BombObject(int id,BombType type, Player owner, Tile shape, int radius, bool controled,double power)
            :this(id,type,owner,shape,radius,controled,10,100,1,1,power)
        {
        }

        public BombObject(int id,BombType type, Player owner, Tile shape, int radius, bool controled, float mass, float gravityFactor, float windFactor, float airResitFactor,double power)
            : base(id)
        {
            _power = power;
            _owner = owner;
            _controled = controled;
            _mass = mass;
            _gravityFactor = gravityFactor;
            _windFactor = windFactor;
            _airResitFactor = airResitFactor;
            _type = type;
            _isHole = true;

            _radius = radius;

            _vx = new EulerVector(0, 0, 0);
            _vy = new EulerVector(0, 0, 0);

            _shape = shape;
            _rect = new Rectangle(-3,-3,6,6);
        }

        public void setSpeedXY(int vx,int vy)
        {
            _vx.x1 = vx;
            _vy.x1 = vy;
        }

        public override void SetXY(int x, int y)
        {
            base.SetXY(x, y);
            _vx.x0 = x;
            _vy.x0 = y;
        }

        protected float _arf = 0;
        protected float _gf = 0;
        protected float _wf = 0;
        public override void setMap(Map map)
        {
            base.setMap(map);
            if (_map != null)
            {
                _arf = _map.airResistance * _airResitFactor;
                _gf = _map.gravity * _gravityFactor * _mass;
                _wf = _map.wind * _windFactor;
                //Console.WriteLine(string.Format("_arf {0}  _gf {1}  _wf {2}", _arf, _gf, _wf));
            }
        }

        protected Point CompleteNextMovePoint(float dt)
        {
            _vx.ComputeOneEulerStep(_mass, _arf, _wf, dt);
            _vy.ComputeOneEulerStep(_mass, _arf, _gf, dt);
            return new Point((int)_vx.x0, (int)_vy.x0);
        }

        private float _currentTime;

        public float RunTime
        {
            get
            {
                return _currentTime;
            }
        }
        public override void StartMoving()
        {
            _actions = new List<BombAction>();

            _currentTime = 0;
            while (_isLiving)
            {
                _currentTime += 0.04F;
                Point pos = CompleteNextMovePoint(0.04F);
                //GameServer.log.Error(string.Format("bomb current{0},{1} move to pos:{2},{3}   x distane:{4}, v:{5},{6}",X,Y,pos.X, pos.Y,Math.Abs(1000-_vx.x0),_vx.x0,_vy.x0));
                MoveTo((int)pos.X, (int)pos.Y);

                if (_isLiving)
                {
                    if (Math.Round(_currentTime * 100) % 40 == 0 && pos.Y > 0)
                        _owner.PlayerDetail.CurrentGame.Data.TempPoint.Add(pos);

                    if (_controled && _vy.x1 > 0)
                    {
                        Player player = _map.FindNearestEnemy(_x, _y, 300, _owner);

                        if (player != null)
                        {
                            Point v = new Point(player.X - _x, player.Y - _y);
                            v = PointHelper.Normalize(v, 1000);
                            setSpeedXY(v.X, v.Y);
                            _actions.Add(new BombAction(_currentTime, ActionType.CHANGE_SPEED, v.X, v.Y, 0,0));
                            //使炮弹不受重力和风力的影响
                            _wf = 0;
                            _gf = 0;
                            _controled = false;
                        }
                    }
                }
            }
        }

        public void MoveTo(int px, int py)
        {
            //Console.WriteLine(string.Format("lifttime:{0}  {1}  {2}", _currentTime, px, py));
            if (px != _x || py != _y)
            {
                int dx = px - _x;
                int dy = py - _y;
                int count = 0;
                int dt = 1;
                bool useX = true;
                if (Math.Abs(dx) > Math.Abs(dy))
                {
                    useX = true;
                    count = Math.Abs(dx);
                    dt = dx / count;
                }
                else
                {
                    useX = false;
                    count = Math.Abs(dy);
                    dt = dy / count;
                }

                
                Point dest = new Point(_x,_y);
                for (int i = 1; i <= count; i += 3)
                {
                    if (useX)
                    {
                        dest = GetNextPointByX(_x, px, _y, py, _x + i * dt);
                    }
                    else
                    {
                        dest = GetNextPointByY(_x, px, _y, py, _y + i * dt);
                    }

                    Rectangle rect = _rect;
                    rect.Offset(dest.X, dest.Y);
                    Physics[] list = _map.FindPhysicalObjects(rect,this);
                    if (list.Length > 0)
                    {
                        base.SetXY(dest.X, dest.Y);
                        CollideObjects(list);
                    }
                    else if (!_map.IsRectangleEmpty(rect))
                    {
                        base.SetXY(dest.X, dest.Y);
                        CollideGround();
                    }
                    else if (_map.IsOutMap(dest.X, dest.Y))
                    {
                        base.SetXY(dest.X, dest.Y);
                        FlyoutMap();
                    }
                    if (!_isLiving)
                    {
                        return;
                    }
                   
                }
                base.SetXY(px, py);
            }
        }

        protected void CollideObjects(Physics[] list)
        {
            bool flag = false;
            foreach (Physics phy in list)
            {
                if (phy is Box)
                {
                    _actions.Add(new BombAction(_currentTime, ActionType.PICK, phy.Id, 0,0,0));
                    _owner.PickBox(phy as Box);
                }
                else
                {
                    flag = true;
                }
            }
            if (flag)
            {
                Bomb();
            }
        }

        protected void CollideGround()
        {
            Bomb();
        }

        public void Bomb()
        {
            _actions.Add(new BombAction(_currentTime, ActionType.BOMB, _x, _y,0,0));
            //Console.WriteLine(string.Format(">>>>>>{0}  {1}", _x, _y));

            Player[] list = _map.FindPlayers(_x, _y, _radius);
            foreach (Player p in list)
            {
                if (p.IsNoHole > 0 || p.NoHoleTurn)
                {
                    p.NoHoleTurn = true;
                    _isHole = false;
                }
            }

            if (_isHole)
            {
                _map.Dig(_x, _y, _shape, null);
            }

            switch (_type)
            {
                case BombType.FORZEN:

                    //Player[] list = _map.FindPlayers(_x, _y, _radius);
                    foreach (Player p in list)
                    {
                        p.Forzen();
                        _actions.Add(new BombAction(_currentTime, ActionType.FORZEN, p.Id, 0, 0,0));
                    }
                    break;

                case BombType.TRANFORM:
                    _owner.X = _x;
                    _owner.Y = _y;
                    _owner.StartMoving();
                    _actions.Add(new BombAction(_currentTime, ActionType.TRANSLATE, _x, _y, 0,0));
                    _actions.Add(new BombAction(_currentTime, ActionType.START_MOVE, _owner.Id, _owner.X, _owner.Y, _owner.IsLiving ? 1 : 0));
                    break;

                default:
                    //Player[] temp = _map.FindPlayers(_x, _y, _radius);
                    foreach (Player p in list)
                    {
                        //if (p.IsNoHole > 0)
                        //{
                        //    _isHole = false;
                        //}
                        if (p.IsFrost > 0)
                        {
                            _actions.Add(new BombAction(_currentTime, ActionType.UNFORZEN, p.Id, 0, 0,0));
                        }
                        else
                        {
                            int old = p.Blood;
                            p.KillBy(_owner, _x, _y,_power);
                            _actions.Add(new BombAction(_currentTime, ActionType.KILL_PLAYER, p.Id,old - p.Blood, _owner.PlayerDetail.CurrentGame.Data.Isforce ? 2 : 1,0));
                            _actions.Add(new BombAction(_currentTime, ActionType.DANDER, p.Id, p.Dander, 0,0));
                        }
                        p.IsFrost = 0;
                        p.IsHide = 0;
                        p.IsNoHole = 0;
                        //被炸死后不需要移动
                        if (p.IsLiving)
                        {
                            p.StartMoving();
                            _actions.Add(new BombAction(_currentTime, ActionType.START_MOVE, p.Id, p.X, p.Y, p.IsLiving ? 1 : 0));
                        }
                    }
                    break;
            }
            //if (_isHole)
            //{
            //    _map.Dig(_x, _y, _shape, null);
            //}

            this.Die();
        }

        protected void FlyoutMap()
        {

            _actions.Add(new BombAction(_currentTime, ActionType.FLY_OUT, 0, 0, 0, 0));

            if (_isLiving)
            {
                Die();
            }
        }

        protected Point GetNextPointByX(int x1,int x2,int y1,int y2, int x)
        {
            if (x2 == x1)
                return new Point(x, y1);

            return new Point(x, (int)((x - x1) * (y2 - y1) / (x2 - x1) + y1));
        }

        protected Point GetNextPointByY(int x1,int x2,int y1,int y2, int y)
        {
            if (y2 == y1)
                return new Point(x1, y);

            return new Point((int)((y - y1) * (x2 - x1) / (y2 - y1) + x1), y);
        }

    }
}
