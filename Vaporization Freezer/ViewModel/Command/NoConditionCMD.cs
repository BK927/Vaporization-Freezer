using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace VF.ViewModel.Command
{
    class NoConditionCMD : ICommand
    {
        readonly Action<object> _execute;

        public NoConditionCMD(Action<object> execute)
        {
            if (execute == null)
            {
                throw new NullReferenceException("execute can not null");
            }
            _execute = execute;
        }
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _execute.Invoke(parameter);
        }
    }
}
