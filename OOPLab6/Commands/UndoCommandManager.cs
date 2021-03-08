using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OOPLab6.Commands
{
    public struct UndoInfo
    {
        public UndoCommand command;
        public Device deviceChanged;

        public UndoInfo(UndoCommand command, Device device)
        {
            this.command = command;
            this.deviceChanged = device;
        }
    }

    public class UndoCommandManager
    {
        public Stack<UndoInfo> UndoCommands = new Stack<UndoInfo>();
        public Stack<UndoInfo> RedoCommands = new Stack<UndoInfo>();

        public void Undo()
        {
            if(UndoCommands.Count > 0)
            {
                var info = UndoCommands.Pop();
                info.command.Unexecute(info.deviceChanged);
                RedoCommands.Push(info);
            }
        }

        public void Redo()
        {
            if(RedoCommands.Count > 0)
            {
                var info = RedoCommands.Pop();
                info.command.Execute(info.deviceChanged);
            }
        }

        public void Add(UndoCommand command, Device device)
        {
            if(device != null)
            {
                UndoCommands.Push(new UndoInfo(command, device ));
            }
        }
    }
}
