using System;
using System.Collections.Generic;
using System.Text;

namespace SqlDataProvider.Data
{
    /// <summary>
    /// 用户的任务
    /// </summary>
    public class QuestDataInfo:DataObject
    {

        private int _userID;
        /// <summary>
        /// 用户编号
        /// </summary>
        public int UserID
        {
            set
            {
                _userID = value;
                _isDirty = true;
            }
            get
            {
                return _userID;
            }
        }

        private int _questID;
        /// <summary>
        /// 任务编号
        /// </summary>
        public int QuestID 
        {
            set
            {
                _questID = value;
                _isDirty = true;
            }
            get
            {
                return _questID;
            }
        }

        private int _condition1;
        /// <summary>
        /// 任务条件一
        /// </summary>
        public int Condition1
        {
            set
            {
                _condition1 = value;
                _isDirty = true;
            }
            get 
            {
                return _condition1;
            }
        }

        private int _condition2;
        /// <summary>
        /// 任务条件二
        /// </summary>
        public int Condition2
        {
            set
            {
                _condition2 = value;
                _isDirty = true;
            }
            get
            {
                return _condition2;
            }
        }

        private int _condition3;
        /// <summary>
        /// 任务条件三
        /// </summary>
        public int Condition3
        {
            set
            {
                _condition3 = value;
                _isDirty = true;
            }
            get
            {
                return _condition3;
            }
        }

        private int _condition4;
        /// <summary>
        /// 任务条件四
        /// </summary>
        public int Condition4
        {
            set
            {
                _condition4 = value;
                _isDirty = true;
            }
            get
            {
                return _condition4;
            }
        }

        private bool _isComplete;
        /// <summary>
        /// 是否完成
        /// </summary>
        public bool IsComplete 
        {
            set
            {

                _isComplete = value;
                _isDirty = true;
            }
            get
            {
                return _isComplete;
            }
        }

        private DateTime _completeDate;
        /// <summary>
        /// 完成日期
        /// </summary>
        public DateTime CompletedDate 
        {
            set
            {
                _completeDate = value;
                _isDirty = true;
            }
            get
            {
                return _completeDate;
            }
        }

        private bool _isExist;
        /// <summary>
        /// 是否存在
        /// </summary>
        public bool IsExist
        {
            set
            {
                _isExist = value;
                _isDirty = true;
            }
            get
            {
                return _isExist;
            }
        }
        private int _repeatFinish;
        /// <summary>
        /// 可重复接收次数
        /// </summary>
        public int RepeatFinish
        {
            set
            {
                _repeatFinish = value;
                _isDirty = true;
            }
            get
            {
                return _repeatFinish;
            }
        }

        private int _randDobule;
        /// <summary>
        /// 获取任务比例
        /// </summary>
        public int RandDobule
        {
            set
            {
                _randDobule = value;
                _isDirty = true;
            }
            get
            {
                return _randDobule;
            }
        }
 
        public int GetConditionValue(int index)
        {
            switch (index)
            {
                case 0:
                    return Condition1;
                case 1:
                    return Condition2;
                case 2:
                    return Condition3;
                case 3:
                    return Condition4;
                default:
                    throw new Exception("Quest condition index out of range.");
            }
        }

        public void SaveConditionValue(int index, int value)
        {
            switch (index)
            {
                case 0:
                    Condition1 = value;
                    break;
                case 1:
                    Condition2 = value;
                    break;
                case 2:
                    Condition3 = value;
                    break;
                case 3:
                    Condition4 = value;
                    break;
                default:
                    throw new Exception("Quest condition index out of range.");
            }
        }      

    }
}
