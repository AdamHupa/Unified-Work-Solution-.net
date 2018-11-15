using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel; // for: INotifyPropertyChanged, INotifyPropertyChanging

// overview of implementation of INotifyPropertyChanged interface: http://blog.amusedia.com/2013/06/inotifypropertychanged-implementation.html

namespace Client_Application.Tools
{
    /// <remarks>
    /// Designed for WPF and other XAML-based application without implementing INotifyPropertyChanging, which is required for: SQL, LinQ-to-SQL
    /// </remarks>
    public class BindableBase : INotifyPropertyChanged //, INotifyPropertyChanging
    {
        public event PropertyChangedEventHandler PropertyChanged;


        /// <remarks>Use with nameof().</remarks>
        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <remarks>It does not inform if property change was neccassary and realized (propertyField == value).</remarks>
        protected virtual void SetProperty<T>(ref T propertyField, T value, [System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(propertyField, value))
                return;

            propertyField = value;

            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
