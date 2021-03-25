using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OOPLab6.Commands
{
    public class UndoCommand : IUndoCommands
    {
        private readonly Func<object, Device> execute;
        private readonly Action<object> unexecute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public UndoCommand(Func<object, Device> execute, Action<object> unexecute)
        {
            this.execute = execute;
            this.unexecute = unexecute;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
           var res = this.execute(parameter);
           SearchUserControl.undoCommandManager.Add(this, res);
        }

        public void Unexecute(object parameter)
        {
            this.unexecute(parameter);
        }
    }
}
