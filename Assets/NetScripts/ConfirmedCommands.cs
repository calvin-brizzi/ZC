using System;
using System.Collections.Generic;
using UnityEngine;

public class ConfirmedCommands
{
	
	public List<NetworkPlayer> playersConfirmedCurrentCommand;
	public List<NetworkPlayer> playersConfirmedPriorCommand;
	
	private LockMan lsm;
	
	public ConfirmedCommands (LockMan lsm)
	{
		this.lsm = lsm;
		playersConfirmedCurrentCommand = new List<NetworkPlayer>(lsm.numberOfPlayers);
		playersConfirmedPriorCommand = new List<NetworkPlayer>(lsm.numberOfPlayers);
	}
	
	public void NextTurn() {
		//clear prior Commands
		playersConfirmedPriorCommand.Clear ();
		
		List<NetworkPlayer> swap = playersConfirmedPriorCommand;
		
		//last turns Commands is now this turns prior Commands
		playersConfirmedPriorCommand = playersConfirmedCurrentCommand;
		
		//set this turns confirmation Commands to the empty list
		playersConfirmedCurrentCommand = swap;
	}
	
	public bool ReadyForNextTurn() {
		//check that the Command that is going to be processed has been confirmed
		if(playersConfirmedPriorCommand.Count == lsm.numberOfPlayers) {
			return true;
		}
		//if 2nd turn, check that the 1st turns Command has been confirmed
		if(lsm.TurnID == LockMan.FirstTurnID + 1) {
			return playersConfirmedCurrentCommand.Count == lsm.numberOfPlayers;
		}
		//no Command has been sent out prior to the first turn
		if(lsm.TurnID == LockMan.FirstTurnID) {
			return true;
		}
		//if none of the conditions have been met, return false
		return false;
	}
}