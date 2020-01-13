//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.Serialization;

namespace WC.WinerSchool.BOL
{
    [DataContract(IsReference = true)]
    public partial class tblinv_bookissue: IObjectWithChangeTracker, INotifyPropertyChanged
    {
        #region Primitive Properties
    
        [DataMember]
        public int Id
        {
            get { return _id; }
            set
            {
                if (_id != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'Id' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    _id = value;
                    OnPropertyChanged("Id");
                }
            }
        }
        private int _id;
    
        [DataMember]
        public Nullable<int> ScheduleId
        {
            get { return _scheduleId; }
            set
            {
                if (_scheduleId != value)
                {
                    _scheduleId = value;
                    OnPropertyChanged("ScheduleId");
                }
            }
        }
        private Nullable<int> _scheduleId;
    
        [DataMember]
        public Nullable<int> BookId
        {
            get { return _bookId; }
            set
            {
                if (_bookId != value)
                {
                    _bookId = value;
                    OnPropertyChanged("BookId");
                }
            }
        }
        private Nullable<int> _bookId;
    
        [DataMember]
        public Nullable<int> StudId
        {
            get { return _studId; }
            set
            {
                if (_studId != value)
                {
                    _studId = value;
                    OnPropertyChanged("StudId");
                }
            }
        }
        private Nullable<int> _studId;
    
        [DataMember]
        public Nullable<int> BatchId
        {
            get { return _batchId; }
            set
            {
                if (_batchId != value)
                {
                    _batchId = value;
                    OnPropertyChanged("BatchId");
                }
            }
        }
        private Nullable<int> _batchId;
    
        [DataMember]
        public Nullable<int> ClassId
        {
            get { return _classId; }
            set
            {
                if (_classId != value)
                {
                    _classId = value;
                    OnPropertyChanged("ClassId");
                }
            }
        }
        private Nullable<int> _classId;
    
        [DataMember]
        public Nullable<System.DateTime> IssueDate
        {
            get { return _issueDate; }
            set
            {
                if (_issueDate != value)
                {
                    _issueDate = value;
                    OnPropertyChanged("IssueDate");
                }
            }
        }
        private Nullable<System.DateTime> _issueDate;
    
        [DataMember]
        public Nullable<int> Count
        {
            get { return _count; }
            set
            {
                if (_count != value)
                {
                    _count = value;
                    OnPropertyChanged("Count");
                }
            }
        }
        private Nullable<int> _count;
    
        [DataMember]
        public Nullable<double> Cost
        {
            get { return _cost; }
            set
            {
                if (_cost != value)
                {
                    _cost = value;
                    OnPropertyChanged("Cost");
                }
            }
        }
        private Nullable<double> _cost;
    
        [DataMember]
        public Nullable<int> IsSpcBook
        {
            get { return _isSpcBook; }
            set
            {
                if (_isSpcBook != value)
                {
                    _isSpcBook = value;
                    OnPropertyChanged("IsSpcBook");
                }
            }
        }
        private Nullable<int> _isSpcBook;

        #endregion
        #region ChangeTracking
    
        protected virtual void OnPropertyChanged(String propertyName)
        {
            if (ChangeTracker.State != ObjectState.Added && ChangeTracker.State != ObjectState.Deleted)
            {
                ChangeTracker.State = ObjectState.Modified;
            }
            if (_propertyChanged != null)
            {
                _propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    
        protected virtual void OnNavigationPropertyChanged(String propertyName)
        {
            if (_propertyChanged != null)
            {
                _propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    
        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged{ add { _propertyChanged += value; } remove { _propertyChanged -= value; } }
        private event PropertyChangedEventHandler _propertyChanged;
        private ObjectChangeTracker _changeTracker;
    
        [DataMember]
        public ObjectChangeTracker ChangeTracker
        {
            get
            {
                if (_changeTracker == null)
                {
                    _changeTracker = new ObjectChangeTracker();
                    _changeTracker.ObjectStateChanging += HandleObjectStateChanging;
                }
                return _changeTracker;
            }
            set
            {
                if(_changeTracker != null)
                {
                    _changeTracker.ObjectStateChanging -= HandleObjectStateChanging;
                }
                _changeTracker = value;
                if(_changeTracker != null)
                {
                    _changeTracker.ObjectStateChanging += HandleObjectStateChanging;
                }
            }
        }
    
        private void HandleObjectStateChanging(object sender, ObjectStateChangingEventArgs e)
        {
            if (e.NewState == ObjectState.Deleted)
            {
                ClearNavigationProperties();
            }
        }
    
        protected bool IsDeserializing { get; private set; }
    
        [OnDeserializing]
        public void OnDeserializingMethod(StreamingContext context)
        {
            IsDeserializing = true;
        }
    
        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            IsDeserializing = false;
            ChangeTracker.ChangeTrackingEnabled = true;
        }
    
        protected virtual void ClearNavigationProperties()
        {
        }

        #endregion
    }
}
