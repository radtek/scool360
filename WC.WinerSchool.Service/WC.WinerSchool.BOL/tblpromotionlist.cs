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
    public partial class tblpromotionlist: IObjectWithChangeTracker, INotifyPropertyChanged
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
        public Nullable<int> StudentId
        {
            get { return _studentId; }
            set
            {
                if (_studentId != value)
                {
                    _studentId = value;
                    OnPropertyChanged("StudentId");
                }
            }
        }
        private Nullable<int> _studentId;
    
        [DataMember]
        public string StudentName
        {
            get { return _studentName; }
            set
            {
                if (_studentName != value)
                {
                    _studentName = value;
                    OnPropertyChanged("StudentName");
                }
            }
        }
        private string _studentName;
    
        [DataMember]
        public Nullable<int> RollNo
        {
            get { return _rollNo; }
            set
            {
                if (_rollNo != value)
                {
                    _rollNo = value;
                    OnPropertyChanged("RollNo");
                }
            }
        }
        private Nullable<int> _rollNo;
    
        [DataMember]
        public string AdmissionNo
        {
            get { return _admissionNo; }
            set
            {
                if (_admissionNo != value)
                {
                    _admissionNo = value;
                    OnPropertyChanged("AdmissionNo");
                }
            }
        }
        private string _admissionNo;
    
        [DataMember]
        public int ClassId
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
        private int _classId;
    
        [DataMember]
        public int BatchId
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
        private int _batchId;
    
        [DataMember]
        public sbyte IsEligible
        {
            get { return _isEligible; }
            set
            {
                if (_isEligible != value)
                {
                    _isEligible = value;
                    OnPropertyChanged("IsEligible");
                }
            }
        }
        private sbyte _isEligible;
    
        [DataMember]
        public string Remarks
        {
            get { return _remarks; }
            set
            {
                if (_remarks != value)
                {
                    _remarks = value;
                    OnPropertyChanged("Remarks");
                }
            }
        }
        private string _remarks;
    
        [DataMember]
        public int ToClassId
        {
            get { return _toClassId; }
            set
            {
                if (_toClassId != value)
                {
                    _toClassId = value;
                    OnPropertyChanged("ToClassId");
                }
            }
        }
        private int _toClassId;
    
        [DataMember]
        public int ToBatchId
        {
            get { return _toBatchId; }
            set
            {
                if (_toBatchId != value)
                {
                    _toBatchId = value;
                    OnPropertyChanged("ToBatchId");
                }
            }
        }
        private int _toBatchId;
    
        [DataMember]
        public string Link
        {
            get { return _link; }
            set
            {
                if (_link != value)
                {
                    _link = value;
                    OnPropertyChanged("Link");
                }
            }
        }
        private string _link;

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
