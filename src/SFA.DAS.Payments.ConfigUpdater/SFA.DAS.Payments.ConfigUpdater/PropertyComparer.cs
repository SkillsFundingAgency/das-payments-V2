using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace SFA.DAS.Payments.ConfigUpdater
{
    public class PropertyComparer<T> : IComparer<T>
    {
        private readonly IComparer _comparer;
        private PropertyDescriptor _propertyDescriptor;
        private int _reverse;

        public PropertyComparer(PropertyDescriptor property, ListSortDirection direction)
        {
            this._propertyDescriptor = property;
            var comparerForPropertyType = typeof(Comparer<>).MakeGenericType(property.PropertyType);
            this._comparer = (IComparer)comparerForPropertyType.InvokeMember("Default", BindingFlags.Static | BindingFlags.GetProperty | BindingFlags.Public, null, null, null);
            this.SetListSortDirection(direction);
        }

        #region IComparer<T> Members

        public int Compare(T x, T y)
        {
            return this._reverse * this._comparer.Compare(this._propertyDescriptor.GetValue(x), this._propertyDescriptor.GetValue(y));
        }

        #endregion

        private void SetPropertyDescriptor(PropertyDescriptor descriptor)
        {
            this._propertyDescriptor = descriptor;
        }

        private void SetListSortDirection(ListSortDirection direction)
        {
            this._reverse = direction == ListSortDirection.Ascending ? 1 : -1;
        }

        public void SetPropertyAndDirection(PropertyDescriptor descriptor, ListSortDirection direction)
        {
            this.SetPropertyDescriptor(descriptor);
            this.SetListSortDirection(direction);
        }
    }
}