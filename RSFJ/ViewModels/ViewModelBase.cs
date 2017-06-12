using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RSFJ.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        protected void SetProperty<T>(ref T Property, T Value, [CallerMemberName] string PropertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(Property, Value) == false)
            {
                T oldValue = Property;
                Property = Value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));

                AfterPropertyChanged(PropertyName, oldValue, Value);
                UpdateModel();
            }
        }

        protected virtual void AfterPropertyChanged<T>(string Property, T OldValue, T NewValue)
        {
            //Subclass may implement.
        }

        protected virtual void UpdateModel()
        {
            //Subclass may implement.
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
