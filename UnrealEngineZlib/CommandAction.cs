using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace UnrealEngineZlib
{
	internal class CommandAction : ICommand
	{
		public event EventHandler? CanExecuteChanged;
		private readonly Action<Object?>? mAction;
		
		public CommandAction(Action<Object?> action) => mAction = action;
		public bool CanExecute(object? parameter) => true;
		public void Execute(object? parameter) => mAction?.Invoke(parameter);
	}
}
