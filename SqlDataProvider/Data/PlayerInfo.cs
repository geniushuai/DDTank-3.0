using System;
using System.Collections.Generic;
using System.Text;

namespace SqlDataProvider.Data
{
    public class PlayerInfo : DataObject
    {
        public PlayerInfo()
        {
            _isLocked = true;
        }

        private int _id;
        public int ID
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
                _isDirty = true;
            }
        }

        private string _userName;
        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                _userName = value;
                _isDirty = true;
            }
        }

        private string _nickName;
        public string NickName
        {
            get
            {
                return _nickName;
            }
            set
            {
                _nickName = value;
                _isDirty = true;
            }
        }

        private bool _sex;
        public bool Sex
        {
            get
            {
                return _sex;
            }
            set
            {
                _sex = value;
                _isDirty = true;
            }
        }

        private int _attack;
        public int Attack
        {
            get
            {
                return _attack;
            }
            set
            {
                _attack = value;
                _isDirty = true;
            }
        }

        private int _defence;
        public int Defence
        {
            get
            {
                return _defence;
            }
            set
            {
                _defence = value;
                _isDirty = true;
            }
        }

        private int _luck;
        public int Luck
        {
            get
            {
                return _luck;
            }
            set
            {
                _luck = value;
                _isDirty = true;
            }
        }

        private int _agility;
        public int Agility
        {
            get
            {
                return _agility;
            }
            set
            {
                _agility = value;
                _isDirty = true;
            }
        }

        private int _gold;
        public int Gold
        {
            get
            {
                return _gold;
            }
            set
            {
                _gold = value;
                _isDirty = true;
            }
        }

        private int _money;
        public int Money
        {
            get
            {
                return _money;
            }
            set
            {
                _money = value;
                _isDirty = true;
            }
        }

        private string _style;
        public string Style
        {
            get
            {
                return _style;
            }
            set
            {
                _style = value;
                _isDirty = true;
            }
        }

        private string _colors;
        public string Colors
        {
            get
            {
                return _colors;
            }
            set
            {
                _colors = value;
                _isDirty = true;
            }
        }

        private int _hide;
        public int Hide
        {
            get
            {
                return _hide;
            }
            set
            {
                _hide = value;
                _isDirty = true;
            }
        }

        private int _grade;
        public int Grade
        {
            get
            {
                return _grade;
            }
            set
            {
                _grade = value;
                _isDirty = true;
            }
        }

        private int _gp;
        public int GP
        {
            get
            {
                return _gp;
            }
            set
            {
                _gp = value;
                _isDirty = true;
            }
        }

        private int _state;
        public int State
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;
                _isDirty = true;
            }
        }

        private int _consortiaID;
        public int ConsortiaID
        {
            get
            {
                return _consortiaID;
            }
            set
            {
                if (_consortiaID == 0 || value == 0)
                {
                    _richesRob = 0;
                    _richesOffer = 0;
                }
                _consortiaID = value;
            }

        }

        private int _repute;
        public int Repute
        {
            get
            {
                return _repute;
            }
            set
            {
                _repute = value;
                _isDirty = true;
            }
        }

        private System.Nullable<DateTime> _expendDate;
        public System.Nullable<DateTime> ExpendDate
        {
            get
            {
                return _expendDate;
            }
            set
            {
                _expendDate = value;
                _isDirty = true;
            }
        }

        private int _offer;
        public int Offer
        {
            get
            {
                return _offer;
            }
            set
            {
                _offer = value;
                _isDirty = true;
            }
        }

        private string _consortiaName;
        public string ConsortiaName
        {
            get
            {
                return _consortiaName;
            }
            set
            {
                _consortiaName = value;
            }
        }

        private int _win;
        public int Win
        {
            get
            {
                return _win;
            }
            set
            {
                _win = value;
                _isDirty = true;
            }
        }

        private int _total;
        public int Total
        {
            get
            {
                return _total;
            }
            set
            {
                _total = value;
                _isDirty = true;
            }
        }

        private int _escape;
        public int Escape
        {
            get
            {
                return _escape;
            }
            set
            {
                _escape = value;
                _isDirty = true;
            }
        }


        private string _skin;
        public string Skin
        {
            get
            {
                return _skin;
            }
            set
            {
                _skin = value;
                _isDirty = true;
            }
        }

        private bool _isConsortia;
        public bool IsConsortia
        {
            get
            {
                return _isConsortia;
            }
            set
            {
                _isConsortia = value;
            }
        }

        public bool IsBanChat { get; set; }

        public int ReputeOffer { get; set; }

        public int ConsortiaRepute { get; set; }

        public int ConsortiaLevel { get; set; }

        public int StoreLevel { get; set; }

        public int ShopLevel { get; set; }

        public int SmithLevel { get; set; }

        public int ConsortiaHonor { get; set; }

        public string ChairmanName { get; set; }

        private int _antiAddiction;
        public int AntiAddiction
        {
            get
            {
                return _antiAddiction + (int)((DateTime.Now - _antiDate).TotalMinutes);
            }
            set
            {
                _antiAddiction = value;
                _antiDate = DateTime.Now;
            }
        }

        private DateTime _antiDate;
        public DateTime AntiDate
        {
            get
            {
                return _antiDate;
            }
            set
            {
                _antiDate = value;
            }
        }

        private int _richesOffer;
        public int RichesOffer
        {
            get
            {
                return _richesOffer;
            }
            set
            {
                _richesOffer = value;
                _isDirty = true;
            }
        }

        private int _richesRob;
        public int RichesRob
        {
            get
            {
                return _richesRob;
            }
            set
            {
                _richesRob = value;
                _isDirty = true;
            }
        }

        public int DutyLevel { get; set; }

        public string DutyName { get; set; }

        public int Right { get; set; }

        public int AddDayGP { get; set; }

        public int AddWeekGP { get; set; }

        public int AddDayOffer { get; set; }

        public int AddWeekOffer { get; set; }

        public int ConsortiaRiches { get; set; }

        public void ClearConsortia()
        {
            ConsortiaID = 0;
            ConsortiaName = "";
            RichesOffer = 0;
            ConsortiaRepute = 0;
            ConsortiaLevel = 0;
            StoreLevel = 0;
            ShopLevel = 0;
            SmithLevel = 0;
            ConsortiaHonor = 0;
            RichesOffer = 0;
            RichesRob = 0;
            DutyLevel = 0;
            DutyName = "";
            Right = 0;
            AddDayGP = 0;
            AddWeekGP = 0;
            AddDayOffer = 0;
            AddWeekOffer = 0;
            ConsortiaRiches = 0;
        }

        private int _checkCount;
        public int CheckCount
        {
            get
            {
                return _checkCount;
            }
            set
            {
                if (value == 0)
                {
                    _checkCode = string.Empty;
                    _checkError = 0;
                }
                _checkCount = value;
                _isDirty = true;
            }
        }

        private string _checkCode;
        public string CheckCode
        {
            get
            {
                return _checkCode;
            }
            set
            {
                _checkDate = DateTime.Now;
                _checkCode = value;
            }
        }

        private int _checkError;
        public int CheckError
        {
            get
            {
                return _checkError;
            }
            set
            {
                _checkError = value;
            }
        }

        private DateTime _checkDate;
        public DateTime CheckDate
        {
            get
            {
                return _checkDate;
            }
        }

        private bool _isMarried;
        public bool IsMarried
        {
            get
            {
                return _isMarried;
            }
            set
            {
                _isMarried = value;
                _isDirty = true;
            }
        }

        private int _spouseID;
        public int SpouseID
        {
            get
            {
                return _spouseID;
            }
            set
            {
                if (_spouseID != value)
                {
                    _spouseID = value;
                    _isDirty = true;
                }
            }
        }

        private string _spouseName;
        public string SpouseName
        {
            get
            {
                return _spouseName;
            }
            set
            {
                if (_spouseName != value)
                {
                    _spouseName = value;
                    _isDirty = true;
                }
            }
        }


        private int _marryInfoID;
        public int MarryInfoID
        {
            get
            {
                return _marryInfoID;
            }
            set
            {
                if (_marryInfoID != value)
                {
                    _marryInfoID = value;
                    _isDirty = true;
                }
            }
        }

        private bool _isLocked;
        public bool IsLocked
        {
            get { return _isLocked; }
            set { _isLocked = value; }
        }

        public bool HasBagPassword
        {
            get
            {
                return !string.IsNullOrEmpty(_PasswordTwo);
            }
        }

        private string _PasswordTwo;
        public string PasswordTwo
        {
            get { return _PasswordTwo; }
            set
            {
                _PasswordTwo = value;
                _isDirty = true;
            }
        }
        private string _Password;
        public string Password
        {
            get { return _Password; }
            set
            {
                _Password = value;
                _isDirty = true;
            }
        }



        private int _dayLoginCount;
        public int DayLoginCount
        {
            get
            {
                return _dayLoginCount;
            }
            set
            {
                _dayLoginCount = value;
                _isDirty = true;
            }
        }

        private bool _isCreatedMarryRoom;
        public bool IsCreatedMarryRoom
        {
            get
            {
                return _isCreatedMarryRoom;
            }
            set
            {
                if (_isCreatedMarryRoom != value)
                {
                    _isCreatedMarryRoom = value;
                    _isDirty = true;
                }
            }
        }

        //private int riches;
        public int Riches
        {
            get
            {
                return RichesRob + RichesOffer;
            }

        }

        private int _selfMarryRoomID;
        public int SelfMarryRoomID
        {
            get
            {
                return _selfMarryRoomID;
            }
            set
            {
                if (_selfMarryRoomID != value)
                {
                    _selfMarryRoomID = value;
                    _isDirty = true;
                }
            }
        }

        private bool _isGotRing;
        public bool IsGotRing
        {
            get
            {
                return _isGotRing;
            }
            set
            {
                if (_isGotRing != value)
                {
                    _isGotRing = value;
                    _isDirty = true;
                }
            }
        }



        private bool _rename;
        public bool Rename
        {
            get
            {
                return _rename;
            }
            set
            {
                if (_rename != value)
                {
                    _rename = value;
                    _isDirty = true;
                }
            }
        }

        private bool _consortiaRename;
        public bool ConsortiaRename
        {
            get
            {
                return _consortiaRename;
            }
            set
            {
                if (_consortiaRename != value)
                {
                    _consortiaRename = value;
                    _isDirty = true;
                }
            }
        }

        private int _nimbus;
        public int Nimbus
        {
            get { return _nimbus; }
            set
            {
                if (_nimbus != value)
                {
                    _nimbus = value;
                    _isDirty = true;
                }
            }
        }

        private int _fightPower;
        public int FightPower
        {
            get { return _fightPower; }
            set
            {
                if (_fightPower != value)
                {
                    _fightPower = value;
                    _isDirty = true;
                }
            }
        }

        private int _IsFirst;
        public int IsFirst
        {
            get
            {

                return _IsFirst;
            }
            set
            {

                _IsFirst = value;
            }
        }

        private int _GiftToken;
        public int GiftToken
        {
            get
            {
                return _GiftToken;
            }
            set
            {
                _GiftToken = value;
            }
        }

        private DateTime _LastAward;
        /// <summary>
        /// 最近获奖时间
        /// </summary>
        public DateTime LastAward
        {
            get
            {
                return _LastAward;
            }
            set
            {
                _LastAward = value;
            }
        }

        private byte[] _QuestSite;
        /// <summary>
        /// 完成任务
        /// </summary>
        public byte[] QuestSite
        {
            get
            {
                return _QuestSite;
            }
            set
            {
                _QuestSite = value;
            }
        }

        private string m_pvePermission;
        public string PvePermission
        {
            get { return m_pvePermission; }
            set { m_pvePermission = value; }
        }

        private string m_PasswordQuest1;
        public string PasswordQuest1
        {
            get { return m_PasswordQuest1; }
            set { m_PasswordQuest1 = value; }
        }

        private string m_PasswordQuest2;
        public string PasswordQuest2
        {
            get { return m_PasswordQuest2; }
            set { m_PasswordQuest2 = value; }
        }

        private int m_FailedPasswordAttemptCount;
        public int FailedPasswordAttemptCount
        {
            get { return m_FailedPasswordAttemptCount; }
            set { m_FailedPasswordAttemptCount = value; }
        }

        private int m_AnswerSite;
        public int AnswerSite
        {
            get { return m_AnswerSite; }
            set { m_AnswerSite = value; }
        }
      
    }


}
