using UnityEngine;
using System.Collections;

public class AIHeroBehaviour : MasterScript 
{
	private TurnInfo player;

	public void HeroDecisionStart(TurnInfo thisPlayer)
	{
		player = thisPlayer;

		for(int i = 0; i < diplomacyScript.relationsList.Count; ++i)
		{
			if(diplomacyScript.relationsList[i].playerOne.playerRace == player.playerRace || diplomacyScript.relationsList[i].playerTwo.playerRace == player.playerRace)
			{
				string enemyRace = null;

				if(diplomacyScript.relationsList[i].playerTwo.playerRace == player.playerRace)
				{
					enemyRace = diplomacyScript.relationsList[i].playerOne.playerRace;
				}
				if(diplomacyScript.relationsList[i].playerOne.playerRace == player.playerRace)
				{
					enemyRace = diplomacyScript.relationsList[i].playerTwo.playerRace;
				}

				if(diplomacyScript.relationsList[i].diplomaticState == "War")
				{
					float protectSystemValue = 0f, invadeSystemValue = 0f;
					int systemToProtect = -1, systemToInvade = -1;

					for(int j = 0; j < systemListConstructor.systemList.Count; ++j)
					{
						systemDefence = systemListConstructor.systemList[j].systemObject.GetComponent<SystemDefence>();
						systemSIMData = systemListConstructor.systemList[j].systemObject.GetComponent<SystemSIMData>();

						if(systemListConstructor.systemList[j].systemOwnedBy == player.playerRace && systemDefence.underInvasion == true && systemSIMData.totalSystemSIM > protectSystemValue)
						{
							protectSystemValue = systemSIMData.totalSystemSIM;
							systemToProtect = j;
						}

						if(systemListConstructor.systemList[j].systemOwnedBy == enemyRace && systemDefence.underInvasion != true)
						{
							float tempSIM = systemSIMData.totalSystemSIM;
							float tempDefence = systemDefence.maxSystemDefence;

							float simToDefRatio = tempSIM/tempDefence;

							if(simToDefRatio > invadeSystemValue)
							{
								invadeSystemValue = simToDefRatio;
								systemToInvade = j;
							}
						}
					}

					if(systemToProtect != -1 && systemToInvade == -1)
					{
						SetDestinationSystem(systemToProtect, "Protect");
					}
					if(systemToProtect == -1 && systemToInvade != -1)
					{
						SetDestinationSystem(systemToInvade, "Invade");
					}
					if(systemToProtect != -1 && systemToInvade != -1)
					{
						systemSIMData = systemListConstructor.systemList[systemToProtect].systemObject.GetComponent<SystemSIMData>();
						systemDefence = systemListConstructor.systemList[systemToProtect].systemObject.GetComponent<SystemDefence>();

						float tempSIM = systemSIMData.totalSystemSIM;
						float tempDefence = systemDefence.maxSystemDefence;
						float simToDefRatio = tempSIM/tempDefence;

						if(simToDefRatio >= invadeSystemValue)
						{
							SetDestinationSystem(systemToProtect, "Protect");
						}
						else
						{
							SetDestinationSystem(systemToInvade, "Invade");
						}
					}
				}
			}
		}
	}

	public string SetSpecialisation()
	{
		float dipMod = 0;
		string type = null;

		for(int i = 0; i < diplomacyScript.relationsList.Count; ++i)
		{
			if(diplomacyScript.relationsList[i].playerOne.playerRace == player.playerRace || diplomacyScript.relationsList[i].playerTwo.playerRace == player.playerRace)
			{
				switch(diplomacyScript.relationsList[i].diplomaticState)
				{
				case "War":
					dipMod += 3;
					break;
				case "Cold War":
					dipMod += 2;
					break;
				case "Peace":
					dipMod += 1;
					break;
				}
			}
		}

		dipMod = dipMod / diplomacyScript.relationsList.Count;

		for(int i = 0; i < player.playerOwnedHeroes.Count; ++i)
		{
			heroScript = player.playerOwnedHeroes[i].GetComponent<HeroScriptParent>();

			if(heroScript.heroType == "")
			{
				int randomNo = Random.Range (0, 100);

				if(dipMod <= 1.0f)
				{
					if(randomNo < 25)
					{
						type = "Soldier";
					}
					if(randomNo < 50 && randomNo >= 25)
					{
						type = "Infiltrator";
					}
					if(randomNo > 50)
					{
						type = "Diplomat";
					}
				}

				if(dipMod > 1.0f && dipMod <= 2.0f)
				{
					if(randomNo < 25)
					{
						type = "Soldier";
					}
					if(randomNo < 50 && randomNo >= 25)
					{
						type = "Diplomat";
					}
					if(randomNo > 50)
					{
						type = "Infiltrator";
					}
				}

				if(dipMod > 2.0f)
				{
					if(randomNo < 25)
					{
						type = "Diplomat";
					}
					if(randomNo < 50 && randomNo >= 25)
					{
						type = "Infiltrator";
					}
					if(randomNo > 50)
					{
						type = "Soldier";
					}
				}
			}
		}

		return type;
	}

	private void SetDestinationSystem(int targetSystem, string task)
	{
		float offence = 0f;
		int hero = -1;
		bool foundHero = false;
		
		for(int k = 0; k < player.playerOwnedHeroes.Count; ++k)
		{
			heroScript = player.playerOwnedHeroes[k].GetComponent<HeroScriptParent>();
			
			if(offence < heroScript.assaultDamage && heroScript.isBusy == false)
			{
				offence = heroScript.assaultDamage;
				hero = k;
				foundHero = true;
			}
		}

		if(foundHero == true)
		{
			heroMovement = player.playerOwnedHeroes[hero].GetComponent<HeroMovement>();
			heroScript = player.playerOwnedHeroes[hero].GetComponent<HeroScriptParent>();
			
			heroMovement.FindPath(heroScript.heroLocation, systemListConstructor.systemList[targetSystem].systemObject, true);

			heroScript.isBusy = true;

			if(task == "Invade")
			{
				heroScript.aiInvadeTarget = targetSystem;
			}
		}
	}
}
