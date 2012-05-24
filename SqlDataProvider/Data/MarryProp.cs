using System;
using System.Collections.Generic;
using System.Text;

namespace SqlDataProvider.Data
{
    public class MarryProp
    {
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
                _spouseID = value;
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
                _spouseName = value;
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
                _isCreatedMarryRoom = value;
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
                _selfMarryRoomID = value;
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
                _isGotRing = value;
            }
        }
    }
}
