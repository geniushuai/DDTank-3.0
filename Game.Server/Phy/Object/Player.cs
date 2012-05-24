using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Game.Server.GameObjects;
using Game.Server.SceneGames;
using SqlDataProvider.Data;
using Game.Base.Packets;
using Game.Server;
using Game.Server.Statics;
using Game.Server.Managers;

namespace Phy.Object
{
    public class Player : Physics
    {
        private int _maxBlood;
        private int _blood;
        private int _endX;
        private int _endY;
        private GamePlayer _player;

        private int _offer;
        public int TurnNum;
        public int TotalHurt;
        public double TotalKill;
        public double HitNum;
        public double BoutNum;
        private int _dander;
        private int _delay;
        private int _energy;

        private int _isFrost;
        private int _isHide;
        private int _isNoHole;
        public bool NoHoleTurn;
        public bool IsTakeOut;
        private bool _isCaptain;
        private TankGameState state;

        public void Reset()
        {
            TurnNum = 0;
            state = TankGameState.FRIST;
            _maxBlood = (int)((950 + _player.PlayerCharacter.Grade * 50) * _player.BaseBlood);
            _blood = _maxBlood;
            TotalHurt = 0;
            _offer = 0;
            TotalKill = 0;
            HitNum = 0;
            BoutNum = 0;
            _dander = 0;
            //_delay = 0;
            _delay = (int)(1000 * _player.BaseAgility);
            _energy = 240;
            _isCaptain = false;

            _isFrost = 0;
            _isHide = 0;
            _isNoHole = 0;
            _isLiving = true;

        }

        public int EndX
        {
            get
            {
                return _endX;
            }
            set
            {
                _endX = value;
            }
        }

        public int EndY
        {
            get
            {
                return _endY;
            }
            set
            {
                _endY = value;
            }
        }

        public GamePlayer PlayerDetail
        {
            get
            {
                return _player;
            }
        }

        public int Blood
        {
            get
            {
                return _blood;
            }
            set
            {
                _blood += value;
                if (value > 0)
                {
                    if (IsCaptain)
                    {
                        if (_blood > _maxBlood * 2)
                            _blood = _maxBlood * 2;
                    }
                    else if (_blood > _maxBlood)
                        _blood = _maxBlood;

                    GSPacketIn pkg = _player.Out.SendPlayerHealth(_player, 0);
                    _player.CurrentGame.SendToPlayerExceptSelf(pkg, _player);

                }
                else if (_blood > 0)
                {
                    //Dander = -value * 2 / 5 + 5;
                    Dander = (-value * 2 / 5 + 5) / 2;
                }

            }
        }

        public int Dander
        {
            get
            {
                return _dander;
            }
            set
            {
                if (state != TankGameState.DEAD)
                {
                    if (value >= 0)
                    {
                        _dander += value;
                    }
                    else
                    {
                        _player.CurrentGame.Data.AddBall = 1;
                        _player.CurrentGame.Data.CurrentSpell = null;
                        _player.CurrentGame.Data.SetCurrentBall(_player.Ball2, true);

                        //Delay = 10;
                        _dander = 0;

                    }
                    //GSPacketIn pkg = _player.Out.SendPlayerDander(_player, _player.CurrentGame);
                    //_player.CurrentGame.SendToPlayerExceptSelf(pkg, _player);
                }
            }
        }

        public void SetDander(int dander)
        {
            Dander = dander;
            GSPacketIn pkg = _player.Out.SendPlayerDander(_player, _player.CurrentGame);
            _player.CurrentGame.SendToPlayerExceptSelf(pkg, _player);
        }

        public int Delay
        {
            get
            {
                return _delay;
            }
            set
            {
                _delay += value;
                //GSPacketIn pkg = _player.Out.SendPlayerDelay(_player, _player.CurrentGame);
                //_player.CurrentGame.SendToPlayerExceptSelf(pkg, _player);
            }
        }

        public int Energy
        {
            set
            {
                _energy = 240;
            }
        }

        public bool ReduceEnergy(int value)
        {
            if (state == TankGameState.DEAD)
                return true;

            if (value > _energy)
                return false;

            _energy -= value;
            return true;
        }

        public int IsFrost
        {
            get
            {
                return _isFrost;
            }
            set
            {
                if (_isFrost == 0 && value <= 0)
                    return;
                _isFrost = value;
                //if (_isFrost != 0)
                //{
                //    GSPacketIn pkg = _player.Out.SendPlayerFrost(_player, _isFrost != 0);
                //    _player.CurrentGame.SendToPlayerExceptSelf(pkg, _player);
                //}
            }
        }

        public void SetFrost(int frost)
        {
            IsFrost = frost;
            GSPacketIn pkg = _player.Out.SendPlayerFrost(_player, _isFrost != 0);
            _player.CurrentGame.SendToPlayerExceptSelf(pkg, _player);
        }

        public int IsNoHole
        {
            get
            {
                return _isNoHole;
            }
            set
            {
                if (_isNoHole == 0 && value <= 0)
                    return;
                _isNoHole = value;
            }
        }

        public void SetNoHole(int noHole)
        {
            IsNoHole = noHole;
            GSPacketIn pkg = _player.Out.SendPlayerNoHole(_player, _isNoHole != 0);
            _player.CurrentGame.SendToPlayerExceptSelf(pkg, _player);
        }

        public int IsHide
        {
            get
            {
                return _isHide;
            }
            set
            {
                if (_isHide == 0 && value <= 0)
                    return;
                _isHide = value;

            }
        }

        public int Offer
        {
            get
            {
                return _offer;
            }
            set
            {
                if (value > 0)
                {
                    value = (int)(value * _player.BuffInventory.OfferMultiple());
                }
                _offer += value;
                _player.SetOffer(value);
            }
        }

        public void SetHide(int hide)
        {
            IsHide = hide;
            GSPacketIn pkg = _player.Out.SendPlayerHide(_player, _isHide != 0);
            _player.CurrentGame.SendToPlayerExceptSelf(pkg, _player);
        }

        public bool IsCaptain
        {
            get
            {
                return _isCaptain;
            }
            set
            {
                if (value)
                {
                    _blood = _maxBlood * 2;
                    GSPacketIn pkg = _player.Out.SendCaption();
                    _player.CurrentGame.SendToPlayerExceptSelf(pkg, _player);
                }
                _isCaptain = value;
            }
        }

        public TankGameState State
        {
            get { return state; }
            set
            {
                if (value == TankGameState.LOSE || value == TankGameState.DEAD)
                {
                    _blood = _blood > 0 ? 0 : _blood;
                    _player.CurrentGame.Data.IsDead = _player.CurrentTeamIndex;
                    _player.CurrentGame.Data.LastDead = _player.CurrentTeamIndex;
                }
                state = value;
            }
        }

        public Player(GamePlayer player)
            : base(player.PlayerCharacter.ID)
        {
            _rect = new Rectangle(-15, -20, 30, 30);
            _player = player;
            _maxBlood = (int)((950 + _player.PlayerCharacter.Grade * 50) * _player.BaseBlood);
            _blood = _maxBlood;
            //Reset();
        }

        public override void SetXY(int x, int y)
        {
            if (state != TankGameState.DEAD)
            {
                _energy -= Math.Abs(_x - x);

            }
            //else
            //{
            //    _energy -= (int)Math.Sqrt(((_x - x) * (_x - x) + (_y - y) * (_y - y)));
            //}

            if (_energy < -20)
            {
                StatMgr.LogErrorPlayer(_player.PlayerCharacter.ID, _player.PlayerCharacter.UserName, _player.PlayerCharacter.NickName,
                    ItemRemoveType.MoveError, _x.ToString() + " to " + x.ToString() + ",MapID:" + _player.CurrentGame.Data.MapIndex);

                GameServer.log.Error("move is error,player state:" + state.ToString() + "  move energy:" + _energy);
                _player.Client.Disconnect();
            }

            _x = x;
            _y = y;
            //StartMoving();
        }


        public override void StartMoving()
        {
            if (_map != null)
            {
                Point p = _map.FindYLineNotEmptyPoint(_x, _y); ;
                if (p.IsEmpty)
                {
                    _y = _map.Ground.Height;
                }
                else
                {
                    _x = p.X;
                    _y = p.Y;
                }

                if (p.IsEmpty)
                {
                    Dead();
                }
            }
        }

        public void KillBy(Player bombOwner, int x, int y, double power)
        {
            //伤害={ 基础伤害*（1+攻击*0.1%）*[1-（基础护甲/200+防御*0.03%）] }*(1+道具攻击加成）*炸弹威力*连击系数
            TankData data = bombOwner.PlayerDetail.CurrentGame.Data;
            GamePlayer AttackPlayer = bombOwner.PlayerDetail;
            int baseDefence = 0;
            int defence = 0;
            if (!data.BreachDefence)
            {
                baseDefence = _player.BaseDefence;
                defence = _player.PlayerCharacter.Defence;
            }

            double bias = data.GetBiasLenght(_x - x, _y - y);
            //int lostHeath = (int)(data.CurrentBall.Power * AttackPlayer.BaseAttack * (1 + AttackPlayer.PlayerCharacter.Attack * 0.001 + AttackPlayer.CurrentGame.Data.AddWound) - _player.BaseDefence * (1 + _player.PlayerCharacter.Defence * 0.001));
            int lostHeath = (int)(data.CurrentBall.Power * AttackPlayer.BaseAttack * (1 + (double)AttackPlayer.PlayerCharacter.Attack * 0.001) * (1 - (double)baseDefence / 200 - (double)defence * 0.0003) * (1 + AttackPlayer.CurrentGame.Data.AddWound));
            lostHeath = (int)(lostHeath * (1 - bias / data.CurrentBall.Radii / 4) * data.AddMultiple * power * data.Modulus);//* ballInfo.BallPower);
            lostHeath = data.AddForceWound(lostHeath);
            lostHeath = Math.Abs(lostHeath);

            if (bombOwner.PlayerDetail.CurrentTeamIndex == _player.CurrentTeamIndex && Blood <= lostHeath)
            {
                lostHeath = Blood - 1;
            }
            Blood = -lostHeath;

            //if (bombOwner.Id != Id)
            if (bombOwner.PlayerDetail.CurrentTeamIndex != _player.CurrentTeamIndex)
            {
                bombOwner.TotalHurt += lostHeath;
                data.TotalHeathPoint += lostHeath;
                data.persons[bombOwner.PlayerDetail.CurrentTeamIndex].TotalKillHealth += lostHeath;
            }

            if (State != TankGameState.DEAD && Blood <= 0)
            {
                //this.Die();
                Dead();

            }
        }

        public void Forzen()
        {
            _player.CurrentGame.Data.CurrentSpell.Execute(_player, _player.CurrentGame.Data.CurrentPorp);
        }

        public void PickBox(Box box)
        {
            box.UserID = _player.PlayerCharacter.ID;
            box.Die();
        }

        public override void Die()
        {
            //State = TankGameState.DEAD;
            _y -= 70;
            base.Die();

            if (_player != _player.CurrentGame.Data.CurrentIndex && _player.CurrentGame.Data.CurrentFire != null)
            {
                _player.CurrentGame.Data.KillPerson(_player);
            }


            if (_player == _player.CurrentGame.Data.CurrentIndex && !_player.CurrentGame.Data.FireLogin)
            {
                _player.CurrentGame.SendPlayFinish(_player);
            }
            else if (_player.CurrentGame.Data.CurrentFire == null)
            {
                _player.CurrentGame.CanStopGame();
            }
        }

        public void Dead()
        {
            State = TankGameState.DEAD;
            Die();
        }
        public void Lose()
        {
            State = TankGameState.LOSE;
            //Die();
            if (_player.CurrentGame.GameState == eGameState.PLAY)
                base.Die();
        }
    }
}
