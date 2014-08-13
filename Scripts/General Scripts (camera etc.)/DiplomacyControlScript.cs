using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class DiplomacyControlScript : MasterScript 
{
	public string tempState;
	private float tempValue;

	public List<DiplomaticPosition> relationsList = new List<DiplomaticPosition>();

	void Start()
	{
		SetUpRelationsList ();
	}

	public void SetUpRelationsList() //Use to generate list of all diplomatic relationships between players
	{
		for(int i = 0; i < turnInfoScript.allPlayers.Count; ++i)
		{
			DiplomaticPosition relation = new DiplomaticPosition(); //Create new relation for AI and connect to players

			relation.playerOne = turnInfoScript.allPlayers[i];
			relation.playerTwo = playerTurnScript;
			relation.stateCounter = 49;
			relation.firstContact = false;

			relationsList.Add (relation);

			for(int j = 0; j < turnInfoScript.allPlayers.Count; ++j) //This ensures that all players are connected, and that no multiple connections exist
			{
				if(turnInfoScript.allPlayers[i].playerRace == turnInfoScript.allPlayers[j].playerRace)
				{
					continue;
				}

				bool skip = false;

				for(int k = 0; k < relationsList.Count; ++k)
				{
					if(relationsList[k].playerOne.playerRace == turnInfoScript.allPlayers[i].playerRace && relationsList[k].playerTwo.playerRace == turnInfoScript.allPlayers[j].playerRace)
					{
						skip = true;
						break;
					}
					if(relationsList[k].playerTwo.playerRace == turnInfoScript.allPlayers[i].playerRace && relationsList[k].playerOne.playerRace == turnInfoScript.allPlayers[j].playerRace)
					{
						skip = true;
						break;
					}
				}

				if(skip == false)
				{
					DiplomaticPosition relationTwo = new DiplomaticPosition(); //Connect AI players

					relationTwo.playerOne = turnInfoScript.allPlayers[i];
					relationTwo.playerTwo = turnInfoScript.allPlayers[j];
					relationTwo.stateCounter = 49;
					relationTwo.firstContact = false;
					
					relationsList.Add (relationTwo);
				}
			}
		}
	}

	public DiplomaticPosition ReturnDiplomaticRelation(string firstRace, string secondRace) //This function is used to return the relation shared by two races
	{
		for(int i = 0; i < relationsList.Count; ++i)
		{
			if(relationsList[i].playerOne.playerRace == firstRace && relationsList[i].playerTwo.playerRace == secondRace)
			{
				return relationsList[i];
			}
			if(relationsList[i].playerTwo.playerRace == firstRace && relationsList[i].playerOne.playerRace == secondRace)
			{
				return relationsList[i];
			}
		}
		return null;
	}

	public void PeaceTreaty(int i) //This function is responsible for timing peacetreaties and overriding other conditions if a peacetreaty is active
	{
		if(relationsList[i].peaceTreatyTimer != 0.0f)
		{
			if(relationsList[i].peaceTreatyTimer + 30.0f < Time.time)
			{
				relationsList[i].peaceTreatyTimer = 0.0f;
				relationsList[i].ceaseFireActive = false;
			}
			else
			{
				relationsList[i].ceaseFireActive = true;
			}
		}
	}

	private void CalculateOffDefModifier(int i) //Off/Def eq: y = (200 / (x + 14.14) ^2) + 0.5
	{
		tempValue = (float)Math.Pow(relationsList[i].stateCounter + 14.14f, 2f);
		
		tempValue = (200f / tempValue) + 0.5f;
		
		relationsList[i].offDefModifier = tempValue;
	}

	private void CalculateResourceModifier(int i) //Resource eq: y = 0.005x + 0.75
	{
		tempValue = (relationsList[i].stateCounter * 0.005f) + 0.75f;

		relationsList [i].resourceModifier = tempValue;
	}

	private void CalculateStealthModifier(int i) //Stealth eq: y = (-1(2)/10000 * (x - 50)^2) + 0.75
	{
		tempValue = (float)Math.Pow (relationsList [i].stateCounter - 50f, 2f);

		if(relationsList[i].stateCounter < 50)
		{
			tempValue = -0.0001f * tempValue;
		}

		if(relationsList[i].stateCounter >= 50)
		{
			tempValue = -0.0002f * tempValue;
		}

		relationsList [i].stealthModifier = tempValue + 0.75f;
	}

	private void CalculateGrowthModifier(int i) //Population eq: y = 0.3742log(x + 1) + 0.5
	{
		tempValue = (float)Math.Log (relationsList [i].stateCounter + 1);

		tempValue = (tempValue * 0.3742f) + 0.5f;

		relationsList [i].growthModifier = tempValue;
	}

	public void DiplomaticStateEffects() //This function calls all other diplomatic functions for each relation
	{
		for(int i = 0; i < relationsList.Count; ++i) 
		{
			ClampStateValues(i);
			CalculateOffDefModifier(i);
			CalculateResourceModifier(i);
			CalculateStealthModifier(i);
			CalculateGrowthModifier(i);
			relationsList[i].diplomaticState = UpdateDiplomaticPosition(i);
			PeaceTreaty(i);
		}
	}

	private string UpdateDiplomaticPosition (int i) //This updates the conditions that are allowed by different states
	{
		if(relationsList[i].stateCounter > 25)
		{
			relationsList[i].autoFight = false;
			relationsList[i].adjacencyBonus = true;
			relationsList[i].tradeAllowed = true;
			relationsList[i].peaceTreatyAllowed = false;

			if(relationsList[i].stateCounter >= 75)
			{
				return "Peace";
			}
			if(relationsList[i].stateCounter < 75)
			{
				return "Cold War";
			}
		}

		if(relationsList[i].stateCounter <= 25)
		{
			relationsList[i].autoFight = true;
			relationsList[i].adjacencyBonus = false;
			relationsList[i].tradeAllowed = false;
			relationsList[i].peaceTreatyAllowed = true;
			return "War";
		}

		return null;
	}

	private void ClampStateValues(int i) //This prevents the diplomacy counter from exceeding the range of 0-100
	{
		if(relationsList[i].stateCounter > 100)
		{
			relationsList[i].stateCounter = 100;
		}

		if(relationsList[i].stateCounter < 0)
		{
			relationsList[i].stateCounter = 0;
		}
	}
}

public class DiplomaticPosition
{
	public TurnInfo playerOne, playerTwo;
	public string diplomaticState;
	public int stateCounter;
	public float timeAtPeace, timeAtColdWar, timeAtWar, peaceTreatyTimer, offDefModifier, resourceModifier, stealthModifier, growthModifier;
	public bool ceaseFireActive, firstContact, adjacencyBonus, autoFight, tradeAllowed, peaceTreatyAllowed;
}
