
using System;
using System.Collections.Generic;

public class PendingCommands
{
	public ICommand[] CurrentCommands;
	private ICommand[] NextCommands;
	private ICommand[] NextNextCommands;
	//incase other players advance to the next step and send their action before we advance a step
	private ICommand[] NextNextNextCommands;
	
	private int currentCommandsCount;
	private int nextCommandsCount;
	private int nextNextCommandsCount;
	private int nextNextNextCommandsCount;
	
	LockMan lsm;
	
	public PendingCommands (LockMan lsm) {
		this.lsm = lsm;
		
		CurrentCommands = new ICommand[lsm.numberOfPlayers];
		NextCommands = new ICommand[lsm.numberOfPlayers];
		NextNextCommands = new ICommand[lsm.numberOfPlayers];
		NextNextNextCommands = new ICommand[lsm.numberOfPlayers];
		
		currentCommandsCount = 0;
		nextCommandsCount = 0;
		nextNextCommandsCount = 0;
		nextNextNextCommandsCount = 0;
	}
	
	public void NextTurn() {
		//Finished processing this turns actions - clear it
		for(int i=0; i<CurrentCommands.Length; i++) {
			CurrentCommands[i] = null;
		}
		ICommand[] swap = CurrentCommands;
		
		//last turn's actions is now this turn's actions
		CurrentCommands = NextCommands;
		currentCommandsCount = nextCommandsCount;
		
		//last turn's next next actions is now this turn's next actions
		NextCommands = NextNextCommands;
		nextCommandsCount = nextNextCommandsCount;
		
		NextNextCommands = NextNextNextCommands;
		nextNextCommandsCount = nextNextNextCommandsCount;
		
		//set NextNextNextCommands to the empty list
		NextNextNextCommands = swap;
		nextNextNextCommandsCount = 0;
	}
	
	public void AddCommand(ICommand action, int playerID, int currentLockStepTurn, int actionsLockStepTurn) {
		//add action for processing later
		if(actionsLockStepTurn == currentLockStepTurn + 1) {
			NextNextNextCommands[playerID] = action;
			nextNextNextCommandsCount++;
		} else if(actionsLockStepTurn == currentLockStepTurn) {
			NextNextCommands[playerID] = action;
			nextNextCommandsCount++;
		} else if(actionsLockStepTurn == currentLockStepTurn - 1) {

			NextCommands[playerID] = action;
			nextCommandsCount++;
		} else {
			return;
		}
	}
	
	public bool ReadyForNextTurn() {
		if(nextNextCommandsCount == lsm.numberOfPlayers) {
			//if this is the 2nd turn, check if all the actions sent out on the 1st turn have been recieved
			if(lsm.TurnID == LockMan.FirstTurnID + 1) {
				return true;
			}
			
			//Check if all Commands that will be processed next turn have been recieved
			if(nextCommandsCount == lsm.numberOfPlayers) {
				return true;
			}
		}
		
		//if this is the 1st turn, no actions had the chance to be recieved yet
		if(lsm.TurnID == LockMan.FirstTurnID) {
			return true;
		}
		//if none of the conditions have been met, return false
		return false;
	}
}