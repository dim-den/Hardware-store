using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OOPLab6.Commands
{
    public interface IUndoCommands : ICommand
    {
        void Unexecute(object obj);
    }
}
