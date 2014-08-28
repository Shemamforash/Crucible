using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class TurnInfo : MonoBehaviour
{
	[HideInInspector]
	public int planetsColonisedThisTurn, systemsColonisedThisTurn, savedIterator, researchCostModifier = 0, antimatter, blueCarbon, radioisotopes, liquidH2;
	public float raceKnowledge, racePower, knowledge, power, wealth, raceWealth, turn = 0, expansionPenaltyModifier;
	[HideInInspector]
	public string[,] planetRIM = new string[12,5];
	public List<PlanetPower> mostPowerfulPlanets = new List<PlanetPower>();
	[HideInInspector]
	public bool playerHasWon, startSteps, isPlayer;
	public Camera mainCamera;
	public Material nereidesMaterial, humansMaterial, selkiesMaterial, materialInUse, emptyMaterial;
	public Material nereidesLineMaterial, humansLineMaterial, selkiesLineMaterial, unownedLineMaterial;
	public string playerRace, homePlanetType, homePlanetCategory, playerHasWonRace, homeSystem;
	public int systemsInPlay = 0, heroCounter = 0;
	public List<GameObject> playerOwnedHeroes = new List<GameObject> ();
	public List<EnemyOne> allPlayers = new List<EnemyOne>();

	public void StartGame()
	{
		for(int i = 0; i < MasterScript.systemListConstructor.systemList.Count; ++i)
		{
			if(MasterScript.systemListConstructor.systemList[i].systemOwnedBy == MasterScript.playerTurnScript.playerRace)
			{
				Debug.Log ("skbdakbdj");
			}
		}

		if(MasterScript.turnInfoScript.startSteps == false && MasterScript.playerTurnScript.playerRace != null)
		{
			InvokeRepeating("EndTurnFunction", 0.0001f, 0.5f);
			MasterScript.turnInfoScript.startSteps = true;
		}
	}

	public void RefreshPlanetPower()
	{
		mostPowerfulPlanets.Clear ();

		MasterScript.turnInfoScript.savedIterator = 0;

		for(int i = 0; i < MasterScript.systemListConstructor.mapSize; i++)
		{
			if(MasterScript.systemListConstructor.systemList[i].systemOwnedBy == null)
			{
				continue;
			}

			SystemSIMData systemSIMData = MasterScript.systemListConstructor.systemList[i].systemObject.GetComponent<SystemSIMData>();
			
			systemSIMData.UpdatePlanetPowerArray();
		}

		SortSystemPower ();
	}

	public void CreateEnemyAI()
	{
		List<string> enemyRaces = new List<string> ();

		enemyRaces.Add (PlayerPrefs.GetString ("AI One"));

		if(PlayerPrefs.GetString ("AI Two") != "None")
		{
			enemyRaces.Add (PlayerPrefs.GetString("AI Two"));
		}

		for(int i = 0; i < enemyRaces.Count; ++i)
		{
			EnemyOne enemy = gameObject.AddComponent("EnemyOne") as EnemyOne;

			enemy.playerRace = enemyRaces[i];

			allPlayers.Add (enemy);
		}
	}

	public void PickRace() //Start of turn function. Race choice dictates starting planet and inherent bonuses as well as racial technologies.
	{
		float gameSpeedModifer = 30f / MasterScript.systemListConstructor.mapSize;

		if(playerRace == "Humans")
		{
			raceKnowledge = gameSpeedModifer;
			racePower = gameSpeedModifer;
			raceWealth = 0.6f;
			homeSystem = "Midgard";
			homePlanetType = "Ocean";
			homePlanetCategory = "Terran";
			materialInUse = MasterScript.turnInfoScript.humansMaterial;
		}
		if(playerRace == "Selkies")
		{
			raceKnowledge = gameSpeedModifer;
			racePower = 1.4f * gameSpeedModifer;
			raceWealth = 0.5f;
			homeSystem = "Samael";
			homePlanetType = "Prairie";
			homePlanetCategory = "Hot";
			materialInUse = MasterScript.turnInfoScript.selkiesMaterial;
		}
		if(playerRace == "Nereides")
		{
			raceKnowledge = 2f * gameSpeedModifer;
			racePower = gameSpeedModifer;
			raceWealth = 10.4f;
			homeSystem = "Nephthys";
			homePlanetType = "Boreal";
			homePlanetCategory = "Cold";
			materialInUse = MasterScript.turnInfoScript.nereidesMaterial;
		}
	}

	public void EndTurnFunction()
	{
		if(MasterScript.playerTurnScript.playerRace != null)
		{
			MasterScript.diplomacyScript.DiplomaticStateEffects();
			MasterScript.turnInfoScript.turn += 1.0f;
			MasterScript.turnInfoScript.TurnEnd (MasterScript.playerTurnScript);
			MasterScript.winConditions.CheckWin(MasterScript.playerTurnScript);
			MasterScript.systemInvasion.UpdateInvasions();

			for(int i = 0; i < allPlayers.Count; ++i)
			{
				allPlayers[i].Expand(allPlayers[i]);
				MasterScript.winConditions.CheckWin(allPlayers[i]);
			}
		}
	}

	private void CalculateExpansionModifier(TurnInfo player)
	{
		expansionPenaltyModifier = 1f;

		for(int i = 0; i < MasterScript.systemListConstructor.mapSize; ++i)
		{
			if(MasterScript.systemListConstructor.systemList[i].systemOwnedBy == player.playerRace)
			{
				for(int j = 0; j < MasterScript.systemListConstructor.systemList[i].planetsInSystem.Count; ++j)
				{
					if(MasterScript.systemListConstructor.systemList[i].planetsInSystem[j].planetColonised == true && MasterScript.systemListConstructor.systemList[i].planetsInSystem[j].expansionPenaltyTimer != 0f)
					{
						if(MasterScript.systemListConstructor.systemList[i].planetsInSystem[j].expansionPenaltyTimer + 30f < Time.time)
						{
							MasterScript.systemListConstructor.systemList[i].planetsInSystem[j].expansionPenaltyTimer = 0f;
							continue;
						}

						expansionPenaltyModifier -= 0.015f;
					}
				}
			}
		}
	}

	public void TurnEnd(TurnInfo selectedPlayer) //This function accumulates all the SIM generated by each system to give an empire SIM value
	{		
		selectedPlayer.researchCostModifier = 0;

		MasterScript.diplomacyScript.DiplomaticStateEffects ();

		CalculateExpansionModifier (selectedPlayer);

		for(int i = 0; i < MasterScript.systemListConstructor.mapSize; ++i)
		{
			if(MasterScript.systemListConstructor.systemList[i].systemOwnedBy != selectedPlayer.playerRace)
			{
				continue;
			}

			SystemSIMData systemSIMData = MasterScript.systemListConstructor.systemList[i].systemObject.GetComponent<SystemSIMData>();
			ImprovementsBasic improvementsBasic = MasterScript.systemListConstructor.systemList[i].systemObject.GetComponent<ImprovementsBasic>();
			SystemDefence systemDefence = MasterScript.systemListConstructor.systemList[i].systemObject.GetComponent<SystemDefence>();

			systemDefence.CalculateSystemDefence();

			improvementsBasic.ActiveTechnologies(i, selectedPlayer);
			systemSIMData.SystemSIMCounter(selectedPlayer);
			MasterScript.systemFunctions.CheckUnlockedTier(improvementsBasic, i);

			selectedPlayer.knowledge += systemSIMData.totalSystemKnowledge;
			selectedPlayer.power += systemSIMData.totalSystemPower;

			if(selectedPlayer.playerRace == "Selkies")
			{
				MasterScript.racialTraitScript.amber += systemSIMData.totalSystemAmber;
			}

			selectedPlayer.researchCostModifier += improvementsBasic.researchCost;
		}

		for(int j = 0; j < selectedPlayer.playerOwnedHeroes.Count; ++j)
		{				
			HeroScriptParent heroScript = selectedPlayer.playerOwnedHeroes[j].GetComponent<HeroScriptParent>();
			heroScript.HeroEndTurnFunctions(selectedPlayer);
		}
		
		MasterScript.racialTraitScript.RacialBonus (selectedPlayer);

		MasterScript.turnInfoScript.SortSystemPower();

		if(selectedPlayer.wealth > 10000.0f)
		{
			selectedPlayer.wealth = 10000.0f;
		}

		selectedPlayer.planetsColonisedThisTurn = 0;

		selectedPlayer.systemsColonisedThisTurn = 0;
	}

	public void SortSystemPower()
	{
		GameObject tempObject;
		float tempFloat;
		int tempInt;

		for(int i = MasterScript.turnInfoScript.mostPowerfulPlanets.Count - 1; i >= 0; --i)
		{
			bool swaps = false;

			for(int j = 1; j <= i; ++j)
			{
				if(mostPowerfulPlanets[j-1].simOutput < mostPowerfulPlanets[j].simOutput)
				{
					tempObject = mostPowerfulPlanets[j-1].system;
					tempFloat = mostPowerfulPlanets[j-1].simOutput;
					tempInt = mostPowerfulPlanets[j-1].planetPosition;

					mostPowerfulPlanets[j-1].system = mostPowerfulPlanets[j].system;
					mostPowerfulPlanets[j-1].simOutput = mostPowerfulPlanets[j].simOutput;
					mostPowerfulPlanets[j-1].planetPosition = mostPowerfulPlanets[j].planetPosition;

					mostPowerfulPlanets[j].system = tempObject;
					mostPowerfulPlanets[j].simOutput = tempFloat;
					mostPowerfulPlanets[j].planetPosition = tempInt;

					swaps = true;
				}
			}

			if(swaps == false)
			{
				break;
			}
		}
	}

	public void CheckIfCanHire(TurnInfo player, string heroType)
	{
		if(player.wealth >= 50 && player.playerOwnedHeroes.Count < 7)
		{
			int i = MasterScript.RefreshCurrentSystem(GameObject.Find(player.homeSystem));
			
			GameObject instantiatedHero = (GameObject)Instantiate (MasterScript.heroGUI.heroObject, MasterScript.systemListConstructor.systemList[i].systemObject.transform.position, 
			                                                       MasterScript.systemListConstructor.systemList[i].systemObject.transform.rotation);
			
			instantiatedHero.name = "Basic Hero_" + heroCounter;
			
			HeroScriptParent tempHero = instantiatedHero.GetComponent<HeroScriptParent>();
			
			tempHero.heroType = heroType;
			
			tempHero.heroLocation = MasterScript.systemListConstructor.systemList[i].systemObject;
			
			tempHero.heroOwnedBy = player.playerRace;
			
			HeroMovement tempMove = instantiatedHero.GetComponent<HeroMovement>();
			
			instantiatedHero.transform.position = tempMove.HeroPositionAroundStar(tempHero.heroLocation);
			
			++heroCounter;
			
			player.wealth -= 50;
			
			player.playerOwnedHeroes.Add (instantiatedHero);
			
			switch(tempHero.heroType)
			{
			case "Soldier":
				tempHero.classModifier = 1.75f;
				break;
			case "Infiltrator":
				tempHero.classModifier = 1f;
				break;
			case "Diplomat":
				tempHero.classModifier = 1.5f;
				break;
			}
		}
	}
}

public class PlanetPower
{
	public GameObject system;
	public float simOutput;
	public int planetPosition;
}