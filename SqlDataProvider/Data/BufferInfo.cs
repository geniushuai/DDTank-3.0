using System;
using System.Collections.Generic;
using System.Text;

namespace SqlDataProvider.Data
{
    public class BufferInfo : DataObject
    {
        //public BuffInfo(ItemInfo item)
        //{
        //    _type = item.Template.Property1;
        //    Value = item.Template.Property2; 
        //    BeginDate = DateTime.Now;
        //    ValidDate = item.Template.Property3;
        //}

        private int _userID;
        public int UserID 
        {
            get { return _userID; }
            set { _userID = value; _isDirty = true; }
        }

        private int _type;
        public int Type
        {
            get { return _type; }
            set { _type = value; _isDirty = true; }
        }

        private int _value;
        public int Value
        {
            get { return _value; }
            set { _value = value; _isDirty = true; }
        }

        private DateTime _beginDate;
        public DateTime BeginDate
        {
            get { return _beginDate; }
            set { _beginDate = value; _isDirty = true; }
        }

        private int _validDate;
        public int ValidDate
        {
            get { return _validDate; }
            set { _validDate = value; _isDirty = true; }
        }

        private string _data;
        public string Data
        {
            get { return _data; }
            set { _data = value; _isDirty = true; }
        }

        private bool _isExist;
        public bool IsExist
        {
            get { return _isExist; }
            set { _isExist = value; _isDirty = true; }
        }

        public DateTime GetEndDate()
        {
            return _beginDate.AddMinutes(_validDate);
        }

        public bool IsValid()
        {
            return _beginDate.AddMinutes(_validDate) > DateTime.Now; 
        }
    }
}
