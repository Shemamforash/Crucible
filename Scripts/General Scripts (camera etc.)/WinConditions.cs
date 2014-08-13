using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WinConditions : MasterScript 
{
	private bool hasWon;
	private string winCondition, winPlayer;
	private string[] homeSystems = new string[3] {"Midgard", "Nephthys", "Samael"};
	private int expansionPercentage;
	private TurnInfo player;

	void Start()
	{
		expansionPercentage = (int)(systemListConstructor.mapSize * 0.75);
	}

	public void CheckWin(TurnInfo thisPlayer)
	{
		player = thisPlayer;

		InvasionWin ();
		ExpansionWin ();
		DiplomaticWin ();
		EconomicWin ();
		ScientificWin ();
		PointWin ();

		if(winPlayer != null)
		{
			Debug.Log (winPlayer + " | " + winCondition);
		}
	}

	void InvasionWin()
	{
		for(int j = 0; j < homeSystems.Length; ++j)
		{
			if(homeSystems[j] == player.homeSystem)
			{
				continue;
			}

			for(int i = 0; i < systemListConstructor.mapSize; ++i)
			{
				if(systemListConstructor.systemList[i].systemName == homeSystems[j])
				{
					if(systemListConstructor.systemList[i].systemOwnedBy == player.playerRace)
					{
						hasWon = true;
					}
					else
					{
						hasWon = false;
					}
				}
			}
		}

		if(hasWon == true)
		{
			winPlayer = player.playerRace;
			winCondition = "Invasion";
		}
	}

	void ExpansionWin()
	{
		int ownedSystems = 0;

		for(int i = 0; i < systemListConstructor.mapSize; ++i)
		{
			if(systemListConstructor.systemList[i].systemOwnedBy == player.playerRace)
			{
				++ownedSystems;
			}
		}

		if (ownedSystems >= expansionPercentage)
		{
			winPlayer = player.playerRace;
			winCondition = "Expansion";
		}
	}

	void DiplomaticWin()
	{
		//TODO
	}

	void EconomicWin()
	{
		float knowledgeRate = 0;
		float powerRate = 0;

		for(int i = 0; i < systemListConstructor.mapSize; ++i)
		{
			if(systemListConstructor.systemList[i].systemOwnedBy == player.playerRace)
			{
				systemSIMData = systemListConstructor.systemList[i].systemObject.GetComponent<SystemSIMData>();

				knowledgeRate += systemSIMData.totalSystemKnowledge;
				powerRate += systemSIMData.totalSystemPower;
			}
		}

		if(knowledgeRate >= 1000f && powerRate >= 1000f)
		{
			winPlayer = player.playerRace;
			winCondition = "Economic";
		}
	}

	void ScientificWin()
	{
		//TODO
	}

	void PointWin()
	{
		//TODO
	}
}
