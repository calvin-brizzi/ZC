using System;

[Serializable]
public class NoCommand : ICommand
{
	public void ProcessCommand() {}
}
