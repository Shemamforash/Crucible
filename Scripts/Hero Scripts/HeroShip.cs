using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeroShip : MasterScript
{
	public bool canEmbargo, hasStealth = false, canPromote, canViewSystem;
	private int system, gridChildren, tempChildren;
	private List<TradeRoute> allTradeRoutes = new List<TradeRoute>();

	void Start()
	{
		heroScript = gameObject.GetComponent<HeroScriptParent> ();
		heroMovement = gameObject.GetComponent<HeroMovement> ();
	}

	private void CheckButtonsAllowed()
	{
		if(heroScript.heroType == "Diplomat")
		{
			NGUITools.SetActive(heroGUI.promoteButton, true);
			NGUITools.SetActive(heroGUI.embargoButton, true);
			tempChildren = tempChildren + 2;
		}

		else
		{
			NGUITools.SetActive(heroGUI.promoteButton, false);
			NGUITools.SetActive(heroGUI.embargoButton, false);
		}
	}

	public void UpdateButtons()
	{
		tempChildren = 0;
	
		if(heroScript.heroOwnedBy == playerTurnScript.playerRace)
		{
			if(heroMovement.TestForProximity (gameObject.transform.position, heroMovement.HeroPositionAroundStar(heroScript.heroLocation)) == true)
			{
				system = RefreshCurrentSystem(heroScript.heroLocation);

				if(heroScript.heroType == "Soldier")
				{
					NGUITools.SetActive(heroGUI.guardButton, true);
					if(systemListConstructor.systemList[system].systemOwnedBy == null)
					{
						heroGUI.guardButton.GetComponent<UILabel>().text = "Guard";
					}
					if(systemListConstructor.systemList[system].systemOwnedBy == heroScript.heroOwnedBy)
					{
						heroGUI.guardButton.GetComponent<UILabel>().text = "Protect";
					}
					++tempChildren;
				}

				if(systemListConstructor.systemList[system].systemOwnedBy != heroScript.heroOwnedBy && systemListConstructor.systemList[system].systemOwnedBy != null)
				{
					++tempChildren;

					CheckButtonsAllowed();

					if(canViewSystem == true)
					{
						heroGUI.invasionButton.GetComponent<UILabel>().text = "Enter System";
						NGUITools.SetActive(heroGUI.invasionButton, false);
					}
					else
					{
						NGUITools.SetActive(heroGUI.invasionButton, true);
						heroGUI.invasionButton.GetComponent<UILabel>().text = "Invade System";
					}
				}
			}
		}

		else
		{
			NGUITools.SetActive(heroGUI.guardButton, false);
			NGUITools.SetActive(heroGUI.embargoButton, false);
			NGUITools.SetActive(heroGUI.promoteButton, false);
			NGUITools.SetActive(heroGUI.invasionButton, false);
		}

		RepositionButtons();
	}

	private void RepositionButtons()
	{
		if(gridChildren != tempChildren)
		{
			gridChildren = tempChildren;
			
			float gridWidth = (gridChildren * heroGUI.buttonContainer.GetComponent<UIGrid>().cellWidth) / 2 - (heroGUI.buttonContainer.GetComponent<UIGrid>().cellWidth/2);
			
			heroGUI.buttonContainer.transform.localPosition = new Vector3(systemGUI.playerSystemInfoScreen.transform.localPosition.x - gridWidth / 2,  //Check
			                                                              heroGUI.turnInfoBar.transform.localPosition.y + 50.0f, 
			                                                              0.0f);
			
			heroGUI.buttonContainer.GetComponent<UIGrid>().repositionNow = true;
		}
	}

	public void ShipAbilities(TurnInfo thisPlayer)
	{
		heroScript = gameObject.GetComponent<HeroScriptParent> ();
		system = RefreshCurrentSystem(heroScript.heroLocation);

		ShipFunctions.UpdateShips ();
		heroScript.assaultDamage = ShipFunctions.primaryWeaponPower * heroScript.assaultMod;
		heroScript.maxHealth = ShipFunctions.armourRating * heroScript.healthMod;
		heroScript.movementSpeed = ShipFunctions.engineValue * heroScript.movementMod;

		if(heroScript.heroType == "Diplomat")
		{
			heroScript.auxiliaryDamage = ShipFunctions.dropshipPower * heroScript.auxiliaryDamage;

			int numberOfMerchants = 0;

				for(int i = 0; i < thisPlayer.playerOwnedHeroes.Count; i++)
				{
					HeroScriptParent tempScript = thisPlayer.playerOwnedHeroes[i].GetComponent<HeroScriptParent>();

					if(tempScript.heroType == "Merchant")
					{
						++numberOfMerchants;
					}
				}

			MerchantFunctions((ShipFunctions.logisticsRating + 1) * numberOfMerchants, thisPlayer);
		}

		if(heroScript.heroType == "Infiltrator")
		{
			heroScript.auxiliaryDamage = ShipFunctions.bombPower * heroScript.auxiliaryMod;

			canViewSystem = true;

			systemSIMData = systemListConstructor.systemList[system].systemObject.GetComponent<SystemSIMData>();

			if(systemListConstructor.systemList[system].systemOwnedBy != thisPlayer.playerRace)
			{
				if(ShipFunctions.stealthValue >= systemSIMData.antiStealthPower)
				{
					hasStealth = true;
				}

				else
				{
					hasStealth = false;
				}
			}

			if(ShipFunctions.infiltratorEngine == true)
			{
				heroScript.movementSpeed = 1000;
			}
		}

		if(heroScript.heroType == "Soldier")
		{
			heroScript.auxiliaryDamage = ShipFunctions.artilleryPower * heroScript.auxiliaryMod;

			if(ShipFunctions.soldierPrimary == true)
			{
				heroScript.assaultDamage = heroScript.assaultDamage * 2;
			}
		}
	}

	private void MakeNewTradeRoutes(TurnInfo thisPlayer)
	{
		float tempWlthKnwl = 0;
		int chosenEnemySystem = -1, chosenPlayerSystem = -1;

		for(int i = 0; i < systemListConstructor.systemList.Count; ++i) //For all systems
		{
			for(int j = 0; j < turnInfoScript.allPlayers.Count; ++j) //For all enemy players
			{
				if(systemListConstructor.systemList[i].systemOwnedBy == turnInfoScript.allPlayers[j].playerRace) //If system is owned by enemy
				{
					for(int k = 0; k < systemListConstructor.systemList[i].permanentConnections.Count; ++k) //For all connections in system
					{
						int system = RefreshCurrentSystem(systemListConstructor.systemList[i].permanentConnections[k]);

						if(systemListConstructor.systemList[system].systemOwnedBy == thisPlayer.playerRace) //If connection is owned by player
						{
							bool skip = false;

							for(int l = 0; l < allTradeRoutes.Count; ++l)
							{
								if(allTradeRoutes[l].playerSystem == systemListConstructor.systemList[system].systemObject)
								{
									skip = true;
								}
							}

							if(skip == false)
							{
								systemSIMData = systemListConstructor.systemList[system].systemObject.GetComponent<SystemSIMData>();

								float temp = systemSIMData.totalSystemPower + systemSIMData.totalSystemKnowledge; //Get the system output

								if(temp > tempWlthKnwl) //If its larger than the previous output
								{
									chosenEnemySystem = i; //Set the enemy system to connect to this system
								}
							}
						}
					}
				}
			}
		}

		if(chosenEnemySystem != -1)
		{
			tempWlthKnwl = 0;

			for(int i = 0; i < systemListConstructor.systemList[chosenEnemySystem].permanentConnections.Count; ++i) //For all connections in enemy system
			{
				int system = RefreshCurrentSystem(systemListConstructor.systemList[chosenEnemySystem].permanentConnections[i]);
				
				if(systemListConstructor.systemList[system].systemOwnedBy == thisPlayer.playerRace) //If connection is owned by player
				{
					bool skip = false;

					for(int l = 0; l < allTradeRoutes.Count; ++l)
					{
						if(allTradeRoutes[l].playerSystem == systemListConstructor.systemList[system].systemObject)
						{
							skip = true;
						}
					}

					if(skip == false)
					{
						systemSIMData = systemListConstructor.systemList[system].systemObject.GetComponent<SystemSIMData>();
						
						float temp = systemSIMData.totalSystemPower + systemSIMData.totalSystemKnowledge; //Get the system output
						
						if(temp >= tempWlthKnwl) //If its larger than the previous output
						{
							chosenPlayerSystem = system; //Set the player system to connect to this system
						}
					}
				}
			}

			TradeRoute route = new TradeRoute();
			
			route.playerSystem = systemListConstructor.systemList[chosenPlayerSystem].systemObject;
			route.enemySystem = systemListConstructor.systemList[chosenEnemySystem].systemObject;
			route.connectorObject = uiObjects.CreateConnectionLine(route.playerSystem, route.enemySystem);
			
			allTradeRoutes.Add (route);
		}
	}

	public void MerchantFunctions(int links, TurnInfo thisPlayer)
	{
		for(int i = 0; i < allTradeRoutes.Count; ++i)
		{
			int pSys = RefreshCurrentSystem(allTradeRoutes[i].playerSystem);

			if(systemListConstructor.systemList[pSys].systemOwnedBy != thisPlayer.playerRace)
			{
				int eSys = RefreshCurrentSystem(allTradeRoutes[i].enemySystem);
				bool notEnemyOwned = false;

				for(int j = 0; j < turnInfoScript.allPlayers.Count; ++j)
				{
					if(systemListConstructor.systemList[eSys].systemOwnedBy != turnInfoScript.allPlayers[i].playerRace)
					{
						notEnemyOwned = true;
						break;
					}
				}

				if(notEnemyOwned == true)
				{
					GameObject.Destroy(allTradeRoutes[i].connectorObject);
					allTradeRoutes.RemoveAt(i);
				}
			}
		}

		for(int i = 0; i < links; ++i)
		{
			if(i < allTradeRoutes.Count)
			{
				int pSys = RefreshCurrentSystem(allTradeRoutes[i].playerSystem);
				int eSys = RefreshCurrentSystem(allTradeRoutes[i].enemySystem);

				SystemSIMData pSysData = systemListConstructor.systemList[pSys].systemObject.GetComponent<SystemSIMData>();
				SystemSIMData eSysData = systemListConstructor.systemList[eSys].systemObject.GetComponent<SystemSIMData>();

				float pIndTransfer = pSysData.totalSystemPower / 2;
				float pSciTransfer = pSysData.totalSystemKnowledge / 2;

				float eIndTransfer = eSysData.totalSystemPower / 2;
				float eSciTransfer = eSysData.totalSystemKnowledge / 2;

				eSysData.totalSystemPower += pIndTransfer;
				eSysData.totalSystemKnowledge += pSciTransfer;
				pSysData.totalSystemPower += eIndTransfer;
				pSysData.totalSystemKnowledge += eSciTransfer;
			}

			else if(i >= allTradeRoutes.Count)
			{
				MakeNewTradeRoutes(thisPlayer);
			}
		}
	}
}

public class TradeRoute
{
	public GameObject playerSystem, enemySystem, connectorObject;
}
