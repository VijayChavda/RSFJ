﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RSFJ.ViewModels
{
    /// <summary>
    /// Base class for all view models.
    /// </summary>
    public class ViewModelBase : INotifyPropertyChanged
    {
        /// <summary>
        /// This event is fired when a property is set by calling the SetProperty().
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Fires a PropertyChanged event if the new value is different than the old one.
        /// </summary>
        /// <param name="Property">A reference to the property whose value is to be set.</param>
        /// <param name="NewValue">New value for the property.</param>
        /// <param name="PropertyName">Leave this to default, to automatically fetch the caller member name.</param>
        protected void SetProperty<T>(ref T Property, T NewValue, [CallerMemberName] string PropertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(Property, NewValue) == false)
            {
                Property = NewValue;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
            }
        }
    }
}
