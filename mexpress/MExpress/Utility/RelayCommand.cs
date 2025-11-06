using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MExpress.Mex
{
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> mExecute = null;
        private readonly Predicate<T> mCanExecute = null;

        public RelayCommand(Action<T> execute)
          : this(execute, null)
        {
        }

        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
        {
            this.mExecute = execute ?? throw new ArgumentNullException("execute");
            this.mCanExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (this.mCanExecute != null)
                    CommandManager.RequerySuggested += value;
            }

            remove
            {
                if (this.mCanExecute != null)
                    CommandManager.RequerySuggested -= value;
            }
        }

        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return this.mCanExecute == null ? true : this.mCanExecute((T)parameter);
        }

        public void Execute(object parameter)
        {
            this.mExecute((T)parameter);
        }
    }
}
