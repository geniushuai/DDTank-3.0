using System;
using System.Collections.Generic;
using System.Text;

namespace SqlDataProvider.Data
{
    public class ConsortiaInfo : DataObject
    {
        private int _consortiaID;
        public int ConsortiaID
        {
            get
            {
                return _consortiaID;
            }
            set
            {
                _consortiaID = value;
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
                _isDirty = true;
            }
        }

        private int _honor;
        public int Honor
        {
            get
            {
                return _honor;
            }
            set
            {
                _honor = value;
                _isDirty = true;
            }
        }

        private int _creatorID;
        public int CreatorID
        {
            get
            {
                return _creatorID;
            }
            set
            {
                _creatorID = value;
                _isDirty = true;
            }
        }

        private string _creatorName;
        public string CreatorName
        {
            get
            {
                return _creatorName;
            }
            set
            {
                _creatorName = value;
                _isDirty = true;
            }
        }

        private int _chairmanID;
        public int ChairmanID 
        {
            get
            {
                return _chairmanID;
            }
            set
            {
                _chairmanID = value;
                _isDirty = true;
            }
        }

        private string _chairmanName;
        public string ChairmanName
        {
            get
            {
                return _chairmanName;
            }
            set
            {
                _chairmanName = value;
                _isDirty = true;
            }
        }

        private string _description;
        public string Description 
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
                _isDirty = true;
            }
        }

        private string _placard;
        public string Placard 
        {
            get
            {
                return _placard;
            }
            set
            {
                _placard = value;
                _isDirty = true;
            }
        }

        private int _level;
        public int Level 
        {
            get
            {
                return _level;
            }
            set
            {
                _level = value;
                _isDirty = true;
            }
        }

        private int _maxCount;
        public int MaxCount 
        {
            get
            {
                return _maxCount;
            }
            set
            {
                _maxCount = value;
                _isDirty = true;
            }
        }

        private int _celebCount;
        public int CelebCount 
        {
            get
            {
                return _celebCount;
            }
            set
            {
                _celebCount = value;
                _isDirty = true;
            }
        }

        private DateTime _buildDate;
        public DateTime BuildDate 
        {
            get
            {
                return _buildDate;
            }
            set
            {
                _buildDate = value;
                _isDirty = true;
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

        private int _count;
        public int Count 
        {
            get
            {
                return _count;
            }
            set
            {
                _count = value;
                _isDirty = true;
            }
        }

        private string _ip;
        public string IP 
        {
            get
            {
                return _ip;
            }
            set
            {
                _ip = value;
                _isDirty = true;
            }
        }

        private int _port;
        public int Port 
        {
            get
            {
                return _port;
            }
            set
            {
                _port = value;
                _isDirty = true;
            }
        }

        private bool _isExist;
        public bool IsExist
        {
            get
            {
                return _isExist;
            }
            set
            {
                _isExist = value;
                _isDirty = true;
            }
        }

        private int _riches;
        public int Riches
        {
            get
            {
                return _riches;
            }
            set
            {
                _riches = value;
                _isDirty = true;
            }
        }

        private DateTime _deductDate;
        public DateTime DeductDate
        {
            get
            {
                return _deductDate;
            }
            set
            {
                _deductDate = value;
                _isDirty = true;
            }
        }

        public int AddDayRiches { get; set; }

        public int AddWeekRiches { get; set; }

        public int AddDayHonor { get; set; }

        public int AddWeekHonor { get; set; }

        public int LastDayRiches { get; set; }

        public bool OpenApply { get; set; }

        public int ShopLevel  { get; set; }

        public int SmithLevel { get; set; }

        public int StoreLevel { get; set; }

    }
}
