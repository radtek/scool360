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
    public partial class tblinv_transaction: IObjectWithChangeTracker, INotifyPropertyChanged
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
        public int ItemId
        {
            get { return _itemId; }
            set
            {
                if (_itemId != value)
                {
                    _itemId = value;
                    OnPropertyChanged("ItemId");
                }
            }
        }
        private int _itemId;
    
        [DataMember]
        public string ItemName
        {
            get { return _itemName; }
            set
            {
                if (_itemName != value)
                {
                    _itemName = value;
                    OnPropertyChanged("ItemName");
                }
            }
        }
        private string _itemName;
    
        [DataMember]
        public int CategoryId
        {
            get { return _categoryId; }
            set
            {
                if (_categoryId != value)
                {
                    _categoryId = value;
                    OnPropertyChanged("CategoryId");
                }
            }
        }
        private int _categoryId;
    
        [DataMember]
        public int Quantity
        {
            get { return _quantity; }
            set
            {
                if (_quantity != value)
                {
                    _quantity = value;
                    OnPropertyChanged("Quantity");
                }
            }
        }
        private int _quantity;
    
        [DataMember]
        public Nullable<double> TotalCost
        {
            get { return _totalCost; }
            set
            {
                if (_totalCost != value)
                {
                    _totalCost = value;
                    OnPropertyChanged("TotalCost");
                }
            }
        }
        private Nullable<double> _totalCost;
    
        [DataMember]
        public Nullable<double> PurchasingCost
        {
            get { return _purchasingCost; }
            set
            {
                if (_purchasingCost != value)
                {
                    _purchasingCost = value;
                    OnPropertyChanged("PurchasingCost");
                }
            }
        }
        private Nullable<double> _purchasingCost;
    
        [DataMember]
        public string ActionType
        {
            get { return _actionType; }
            set
            {
                if (_actionType != value)
                {
                    _actionType = value;
                    OnPropertyChanged("ActionType");
                }
            }
        }
        private string _actionType;
    
        [DataMember]
        public Nullable<System.DateTime> ActionDate
        {
            get { return _actionDate; }
            set
            {
                if (_actionDate != value)
                {
                    _actionDate = value;
                    OnPropertyChanged("ActionDate");
                }
            }
        }
        private Nullable<System.DateTime> _actionDate;
    
        [DataMember]
        public string ReferenceId
        {
            get { return _referenceId; }
            set
            {
                if (_referenceId != value)
                {
                    _referenceId = value;
                    OnPropertyChanged("ReferenceId");
                }
            }
        }
        private string _referenceId;
    
        [DataMember]
        public string ReferenceType
        {
            get { return _referenceType; }
            set
            {
                if (_referenceType != value)
                {
                    _referenceType = value;
                    OnPropertyChanged("ReferenceType");
                }
            }
        }
        private string _referenceType;
    
        [DataMember]
        public Nullable<int> StoreId
        {
            get { return _storeId; }
            set
            {
                if (_storeId != value)
                {
                    _storeId = value;
                    OnPropertyChanged("StoreId");
                }
            }
        }
        private Nullable<int> _storeId;
    
        [DataMember]
        public string Description
        {
            get { return _description; }
            set
            {
                if (_description != value)
                {
                    _description = value;
                    OnPropertyChanged("Description");
                }
            }
        }
        private string _description;
    
        [DataMember]
        public string Comment
        {
            get { return _comment; }
            set
            {
                if (_comment != value)
                {
                    _comment = value;
                    OnPropertyChanged("Comment");
                }
            }
        }
        private string _comment;
    
        [DataMember]
        public string CreatedUser
        {
            get { return _createdUser; }
            set
            {
                if (_createdUser != value)
                {
                    _createdUser = value;
                    OnPropertyChanged("CreatedUser");
                }
            }
        }
        private string _createdUser;
    
        [DataMember]
        public int VendorId
        {
            get { return _vendorId; }
            set
            {
                if (_vendorId != value)
                {
                    _vendorId = value;
                    OnPropertyChanged("VendorId");
                }
            }
        }
        private int _vendorId;
    
        [DataMember]
        public int Valuetype
        {
            get { return _valuetype; }
            set
            {
                if (_valuetype != value)
                {
                    _valuetype = value;
                    OnPropertyChanged("Valuetype");
                }
            }
        }
        private int _valuetype;
    
        [DataMember]
        public Nullable<int> StockBal
        {
            get { return _stockBal; }
            set
            {
                if (_stockBal != value)
                {
                    _stockBal = value;
                    OnPropertyChanged("StockBal");
                }
            }
        }
        private Nullable<int> _stockBal;
    
        [DataMember]
        public Nullable<int> IssueUserTypeId
        {
            get { return _issueUserTypeId; }
            set
            {
                if (_issueUserTypeId != value)
                {
                    _issueUserTypeId = value;
                    OnPropertyChanged("IssueUserTypeId");
                }
            }
        }
        private Nullable<int> _issueUserTypeId;

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
