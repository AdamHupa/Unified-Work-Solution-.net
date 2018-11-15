using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// link: https://msdn.microsoft.com/en-us/magazine/dd419663.aspx#id0090030
// note: simplest possible implementation + CanExecuteChangedInternal

namespace Client_Application.Views.Tools
{
    public class RelayCommand : System.Windows.Input.ICommand
    {
        readonly Action<object> _executeMethod;
        readonly Predicate<object> _canExecuteMethod;


        public RelayCommand(Action<object> executeMethod) : this(executeMethod, null) { }

        public RelayCommand(Action<object> executeMethod, Predicate<object> canExecuteMethod)
        {
            if (executeMethod == null)
                throw new ArgumentNullException("executeMethod"); // nameof()

            _executeMethod = executeMethod;
            _canExecuteMethod = canExecuteMethod;
        }


        private event EventHandler CanExecuteChangedInternal;


        public event EventHandler CanExecuteChanged
        {
            add
            {
                System.Windows.Input.CommandManager.RequerySuggested += value;
                CanExecuteChangedInternal += value;
            }
            remove
            {
                System.Windows.Input.CommandManager.RequerySuggested -= value;
                CanExecuteChangedInternal += value;
            }
        }


        [System.Diagnostics.DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return _canExecuteMethod == null ? true : _canExecuteMethod(parameter);
        }

        public void Execute(object parameter)
        {
            _executeMethod(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChangedInternal != null)
                CanExecuteChangedInternal(this, EventArgs.Empty);
        }
    }
}
