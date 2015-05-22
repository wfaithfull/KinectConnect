using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KinectConnect.Util
{

    public class AsyncDelegateCommand : AsyncDelegateCommand<object>, IAsyncCommand
    {
        public AsyncDelegateCommand(Func<Task> executeMethod)
            : base(o => executeMethod())
        {
        }

        public AsyncDelegateCommand(Func<Task> executeMethod, Func<bool> canExecuteMethod)
            : base(o => executeMethod(), o => canExecuteMethod())
        {
        }
    }

    public class AsyncDelegateCommand<TArgType> : IAsyncCommand
    {
        protected readonly Predicate<TArgType> _canExecute;
        protected Func<TArgType, Task> _asyncExecute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public AsyncDelegateCommand(Func<TArgType, Task> asyncExecute, Predicate<TArgType> canExecute = null)
        {
            _asyncExecute = asyncExecute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute == null)
            {
                return true;
            }

            return _canExecute((TArgType)parameter);
        }

        public async void Execute(object parameter)
        {
            await AsyncRunner(parameter);
        }

        public async Task ExecuteAsync(object parameter)
        {
            await AsyncRunner(parameter);
        }

        protected virtual async Task AsyncRunner(object parameter)
        {
            await _asyncExecute((TArgType)parameter);
        }
    }


    public interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync(object parameter);
    }
}
