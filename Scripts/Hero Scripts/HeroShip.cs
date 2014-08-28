using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeroShip : MonoBehaviour
{
	public bool canEmbargo, hasStealth = false, canPromote, canViewSystem;
	private int system, gridChildren, tempChildren;
	private List<TradeRoute> allTradeRoutes = new List<TradeRoute>();
	private HeroScriptParent heroScript;
	private SystemSIMData systemSIMData;

	void Start()
	{
		heroScript = gameObject.GetComponent<HeroScriptParent> ();
	}

	public void ShipAbilities(TurnInfo thisPlayer)
	{
		heroScript = gameObject.GetComponent<HeroScriptParent> ();
		system = MasterScript.RefreshCurrentSystem(heroScript.heroLocation);

		ShipFunctions.UpdateShips ();
		heroScript.assaultDamage = ShipFunctions.primaryWeaponPower * heroScript.assaultMod;
		heroScript.maxHealth = ShipFunctions.armourRating * heroScript.healthMod;
		heroScript.movementSpeed = ShipFunctions.engineValue * heroScript.movementMod;

		if(heroScript.heroType == "Diplomat")
		{
			heroScript.auxiliaryDamage = ShipFunctions.dropshipPower * heroScript.auxiliaryMod;

			DiplomatFunctions((ShipFunctions.logisticsRating + 10), thisPlayer);

			canViewSystem = true;
		}

		if(heroScript.heroType == "Infiltrator")
		{
			heroScript.auxiliaryDamage = ShipFunctions.bombPower * heroScript.auxiliaryMod;

			canViewSystem = true;

			systemSIMData = MasterScript.systemListConstructor.systemList[system].systemObject.GetComponent<SystemSIMData>();

			if(MasterScript.systemListConstructor.systemList[system].systemOwnedBy != thisPlayer.playerRace)
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
		float tempSystemSI = 0;
		int chosenEnemySystem = -1, chosenPlayerSystem = -1;
		string tempOwner = null;

		for(int i = 0; i < MasterScript.systemListConstructor.systemList.Count; ++i) //For all systems
		{
			if(MasterScript.systemListConstructor.systemList[i].systemOwnedBy == null) //If system is not owned ignore it
			{
				continue;
			}
			if(MasterScript.systemListConstructor.systemList[i].systemOwnedBy == thisPlayer.playerRace) //If system is owned by the player
			{
				for(int j = 0; j < MasterScript.systemListConstructor.systemList[i].permanentConnections.Count; ++j) //For all systems connected to this system
				{
					int sys = MasterScript.RefreshCurrentSystem(MasterScript.systemListConstructor.systemList[i].permanentConnections[j]);

					if(MasterScript.systemListConstructor.systemList[sys].systemOwnedBy == null || MasterScript.systemListConstructor.systemList[sys].systemOwnedBy == thisPlayer.playerRace) //If the system is owned by this player or not at all ignore it
					{
						continue;
					}

					GameObject enemySystem = MasterScript.systemListConstructor.systemList[sys].systemObject;

					bool routeExists = false; //Say the proposed route between these systems does not exist

					for(int k = 0; k < allTradeRoutes.Count; ++k) //For all existing trade routes
					{
						if((allTradeRoutes[k].enemySystem == sys && allTradeRoutes[k].playerSystem == i) || 
						   (allTradeRoutes[k].playerSystem == sys && allTradeRoutes[k].enemySystem == i)) //Check to see if the proposed one exists
						{
							routeExists = true;
						}
					}

					if(routeExists == false) //If the route doesn't exist
					{
						systemSIMData = enemySystem.GetComponent<SystemSIMData>(); //Get a reference to the SI output data

						float temp = systemSIMData.totalSystemPower + systemSIMData.totalSystemKnowledge; //Calculate the system power plus it's knowledge

						if(temp > tempSystemSI) //If the calculated value is greater than the stored value, this trade route is more valuable than the cached one
						{
							tempSystemSI = temp; //So cache this one over it!

							chosenEnemySystem = sys;
							chosenPlayerSystem = i;
							tempOwner = MasterScript.systemListConstructor.systemList[sys].systemOwnedBy;
						}
					}
				}
			}
		}

		if(chosenEnemySystem != -1)
		{
			TradeRoute route = new TradeRoute();
			
			route.playerSystem = chosenPlayerSystem;
			route.enemySystem = chosenEnemySystem;
			route.connectorObject = MasterScript.uiObjects.CreateConnectionLine(MasterScript.systemListConstructor.systemList[route.playerSystem].systemObject, MasterScript.systemListConstructor.systemList[route.enemySystem].systemObject);
			route.enemySystemOwner = tempOwner;

			for(int i = 0; i < MasterScript.turnInfoScript.allPlayers.Count; ++i)
			{
				if(MasterScript.turnInfoScript.allPlayers[i].playerRace == tempOwner)
				{
					route.enemyPlayer = MasterScript.turnInfoScript.allPlayers[i];
				}
			}

			allTradeRoutes.Add (route);
		}
	}

	private void CheckTradeRoutesAreValid(TurnInfo thisPlayer)
	{
		for(int i = 0; i < allTradeRoutes.Count; ++i) //For all existing trade routes
		{
			int playerSystem = allTradeRoutes[i].playerSystem; //Get the player system
			int enemySystem = allTradeRoutes[i].enemySystem;
			bool invalidRoute = false;
			
			if(MasterScript.systemListConstructor.systemList[playerSystem].systemOwnedBy != thisPlayer.playerRace) //If this system is not owned by this player
			{
				invalidRoute = true; //This trade route is invalidated
			}
			if(MasterScript.systemListConstructor.systemList[enemySystem].systemOwnedBy != allTradeRoutes[i].enemySystemOwner) //Or the enemy system has changed owned
			{
				invalidRoute = true; //This trade route is invalidated
			}
			if(invalidRoute == true) //If the trade route is no longer valid
			{
				GameObject.Destroy(allTradeRoutes[i].connectorObject); //Destroy it
				allTradeRoutes.RemoveAt(i);
			}
		}
	}

	private void ActivateCurrentTradeRoutes(int i, TurnInfo thisPlayer)
	{
		if(i < allTradeRoutes.Count)
		{
			int pSys = allTradeRoutes[i].playerSystem;
			int eSys = allTradeRoutes[i].enemySystem;
			
			SystemSIMData playerSystemData = MasterScript.systemListConstructor.systemList[pSys].systemObject.GetComponent<SystemSIMData>();
			SystemSIMData enemySystemData = MasterScript.systemListConstructor.systemList[eSys].systemObject.GetComponent<SystemSIMData>();
			
			float playerPowerTransfer = playerSystemData.totalSystemPower / 2;
			float playerKnowledgeTransfer = playerSystemData.totalSystemKnowledge / 2;
			
			float enemyPowerTransfer = enemySystemData.totalSystemPower / 2;
			float enemyKnowledgeTransfer = enemySystemData.totalSystemKnowledge / 2;
			
			playerSystemData.totalSystemPower += playerPowerTransfer;
			playerSystemData.totalSystemKnowledge += playerKnowledgeTransfer;
			enemySystemData.totalSystemPower += enemyPowerTransfer;
			enemySystemData.totalSystemKnowledge += enemyKnowledgeTransfer;
		}
	}

	public void DiplomatFunctions(int links, TurnInfo thisPlayer)
	{
		CheckTradeRoutesAreValid(thisPlayer);

		for(int i = 0; i < links; ++i)
		{
			ActivateCurrentTradeRoutes(i, thisPlayer);

			if(i >= allTradeRoutes.Count)
			{
				MakeNewTradeRoutes(thisPlayer);
			}
		}

		if(heroScript.heroOwnedBy == thisPlayer.playerRace)
		{
			systemSIMData = heroScript.heroLocation.GetComponent<SystemSIMData>();
			systemSIMData.totalSystemPower += systemSIMData.totalSystemPower * (0.2f + thisPlayer.racePower);
			systemSIMData.totalSystemKnowledge += systemSIMData.totalSystemKnowledge * (0.2f + thisPlayer.raceKnowledge);
		}
	}
}

public class TradeRoute
{
	public int playerSystem, enemySystem;
	public string enemySystemOwner;
	public GameObject connectorObject;
	public TurnInfo enemyPlayer;
}
