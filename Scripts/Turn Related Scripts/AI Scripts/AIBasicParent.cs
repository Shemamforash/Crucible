﻿using UnityEngine;
using System.Collections;

public class AIBasicParent : TurnInfo
{
	public string selkiesHomeSystem, nereidesHomeSystem, humansHomeSystem;
	private float tempSIM, highestSIM, tempFloat, knwlRatio, powRatio;
	private int tempPlanet, tempSystem, tempPlanetB, tempSystemB, currentPlanet, currentSystem, checkHeroTimer = 0, tempTech;
	private bool saveForHero;
	private TurnInfo thisPlayer;
	private AIHeroBehaviour heroBehaviour;
	private GenericImprovements improvements;
	private SystemSIMData systemSIMData;
	private SystemDefence systemDefence;

	public void Start()
	{
		improvements = GameObject.Find ("ScriptsContainer").GetComponent<GenericImprovements> ();
	}

	public void Expand(TurnInfo player)
	{
		thisPlayer = player;

		for(float i = thisPlayer.wealth; i > 0; --i)
		{
			CheckToSaveForHero();

			if(saveForHero == false)
			{
				AIExpansion();
			}

			if(saveForHero == true)
			{
				string temp = heroBehaviour.SetSpecialisation();
				MasterScript.turnInfoScript.CheckIfCanHire(thisPlayer, temp);
			}
		}

		MasterScript.turnInfoScript.TurnEnd(thisPlayer);

		if(heroBehaviour == null)
		{
			heroBehaviour = GameObject.FindGameObjectWithTag ("ScriptContainer").GetComponent<AIHeroBehaviour> ();
		}

		heroBehaviour.HeroDecisionStart (player);
	}

	/*
	private void OptimumTechToBuild(int system)
	{
		systemSIMData = systemListConstructor.systemList [system].systemObject.GetComponent<SystemSIMData> ();
		improvementsBasic = systemListConstructor.systemList [system].systemObject.GetComponent<ImprovementsBasic> ();

		knwlRatio = (100 / systemSIMData.totalSystemSIM) * systemSIMData.totalSystemKnowledge;
		powRatio = (100 / systemSIMData.totalSystemSIM) * systemSIMData.totalSystemPower;

		tempFloat = 0f;
		tempTech = -1;
		tempPlanet = -1;

		for(int i = 0; i < improvementsBasic.listOfImprovements.Count; ++i)
		{
			if(improvementsBasic.listOfImprovements[i].improvementCost > thisPlayer.power)
			{
				continue;
			}

			if(improvementsBasic.listOfImprovements[i].improvementLevel <= improvementsBasic.techTier && improvementsBasic.listOfImprovements[i].hasBeenBuilt == false)
			{
				improvements.TechSwitch(i, improvementsBasic, thisPlayer, true);
				bool okToBuild = false;

				if(improvementsBasic.planetToBuildOn.Count > 0)
				{
					for(int j = 0; j < systemListConstructor.systemList[system].planetsInSystem.Count; ++j)
					{
						if(improvementsBasic.planetToBuildOn.Contains(systemListConstructor.systemList[system].planetsInSystem[j].planetType))
						{
							okToBuild = true;
							tempPlanet = j;
						}
					}
				}

				if(tempPlanet != -1)
				{
					int tempPlanSize = 0;

					for(int j = 0; j < systemListConstructor.systemList[system].planetsInSystem.Count; ++j)
					{
						int noSlots = 0;

						for(int k = 0; k < systemListConstructor.systemList[system].planetsInSystem[j].improvementSlots; ++k)
						{
							if(systemListConstructor.systemList[system].planetsInSystem[j].improvementsBuilt[k] == null)
							{
								++noSlots;
							}
						}

						if(noSlots > tempPlanSize)
						{
							tempPlanSize = noSlots;
							tempPlanet = j;
						}
					}
				}

				if(improvementsBasic.planetToBuildOn.Count == 0)
				{
					okToBuild = true;
				}

				if(okToBuild == true)
				{
					RacialTechCheck(i);
					GenericTechCheck(i);
				}
			}
		}

		if(tempTech != -1f)
		{
			if(improvementsBasic.ImproveSystem(system) == true)
			{
				for(int i = 0; i < systemListConstructor.systemList[systemGUI.selectedSystem].planetsInSystem[tempPlanet].improvementSlots; ++i)
				{
					if(systemListConstructor.systemList[systemGUI.selectedSystem].planetsInSystem[tempPlanet].improvementsBuilt[i] == null)
					{
						systemListConstructor.systemList[systemGUI.selectedSystem].planetsInSystem[tempPlanet].improvementsBuilt[i] = improvementsBasic.listOfImprovements[i].improvementName;
					}
				}
			}
		}
	}

	private void RacialTechCheck(int i, int system)
	{
		if(thisPlayer.playerRace == "Humans")
		{
			if(improvementsBasic.tempBonusAmbition * 50 * improvementsBasic.listOfImprovements[i].improvementLevel > improvementsBasic.listOfImprovements[i].improvementCost)
			{
				if(improvementsBasic.tempBonusAmbition * 50 * improvementsBasic.listOfImprovements[i].improvementLevel > tempFloat || tempTech == i)
				{
					tempFloat = improvementsBasic.tempBonusAmbition * 50 * improvementsBasic.listOfImprovements[i].improvementLevel;
					tempTech = i;
				}
			}
		}
		
		if(thisPlayer.playerRace == "Selkies")
		{
			if((improvementsBasic.amberPenalty > systemListConstructor.systemList[system].sysAmberPenalty) * 10 * improvementsBasic.listOfImprovements[i].improvementLevel > tempFloat || tempTech == i)
			{
				if(improvementsBasic.amberPenalty > systemListConstructor.systemList[system].sysAmberPenalty)
				{
					tempFloat = (systemListConstructor.systemList[system].sysAmberPenalty + improvementsBasic.amberPenalty) * 10 * improvementsBasic.listOfImprovements[i].improvementLevel;
					tempTech = i;
				}
			}
			
			if(improvementsBasic.amberPointBonus * 50 * improvementsBasic.listOfImprovements[i].improvementLevel > tempFloat || tempTech == i)
			{
				if(improvementsBasic.amberPointBonus * 50 * improvementsBasic.listOfImprovements[i].improvementLevel > improvementsBasic.listOfImprovements[i].improvementCost)
				{
					if(tempTech == i)
					{
						tempFloat += improvementsBasic.amberPointBonus * 50 * improvementsBasic.listOfImprovements[i].improvementLevel;
					}
					else
					{
						tempFloat = improvementsBasic.amberPointBonus * 50 * improvementsBasic.listOfImprovements[i].improvementLevel;
						tempTech = i;
					}
				}
			}
			
			if(improvementsBasic.amberProductionBonus * systemSIMData.totalSystemAmber * 100 * improvementsBasic.listOfImprovements[i].improvementLevel > tempFloat || tempTech == i)
			{
				if(improvementsBasic.amberProductionBonus * systemSIMData.totalSystemAmber * 100 * improvementsBasic.listOfImprovements[i].improvementLevel > improvementsBasic.listOfImprovements[i].improvementCost)
				{
					if(tempTech == i)
					{
						tempFloat += improvementsBasic.amberProductionBonus * systemSIMData.totalSystemAmber * 100 * improvementsBasic.listOfImprovements[i].improvementLevel;
					}
					else
					{
						tempFloat = improvementsBasic.amberProductionBonus * systemSIMData.totalSystemAmber * 100 * improvementsBasic.listOfImprovements[i].improvementLevel;
						tempTech = i;
					}
				}
			}
		}
	}

	private void GenericTechCheck(int i, int system)
	{
		if(systemListConstructor.systemList[system].sysPowerModifier * 4 * improvementsBasic.listOfImprovements[i].improvementLevel > improvementsBasic.listOfImprovements[i].improvementCost)
		{
			if(systemListConstructor.systemList[system].sysPowerModifier * powRatio > tempFloat)
			{
				if(tempTech == i)
				{
					tempFloat += systemListConstructor.systemList[system].sysPowerModifier * powRatio * improvementsBasic.listOfImprovements[i].improvementLevel;
				}
				else
				{
					tempFloat = systemListConstructor.systemList[system].sysPowerModifier * powRatio * improvementsBasic.listOfImprovements[i].improvementLevel;
					tempTech = i;
				}
			}
		}
		
		if(improvementsBasic.systemListConstructor.systemList[system].sysPowerModifier * 4 * improvementsBasic.listOfImprovements[i].improvementLevel > improvementsBasic.listOfImprovements[i].improvementCost)
		{
			if(improvementsBasic.tempKnwlUnitBonus * knwlRatio > tempFloat || tempTech == i)
			{
				if(tempTech == i)
				{
					tempFloat += improvementsBasic.tempKnwlUnitBonus * knwlRatio * improvementsBasic.listOfImprovements[i].improvementLevel;
				}
				else
				{
					tempFloat = improvementsBasic.tempKnwlUnitBonus * knwlRatio * improvementsBasic.listOfImprovements[i].improvementLevel;
					tempTech = i;
				}
			}
		}
		
		if(improvementsBasic.tempImprovementSlots * 100 * improvementsBasic.listOfImprovements[i].improvementLevel > improvementsBasic.listOfImprovements[i].improvementCost)
		{
			if(improvementsBasic.tempImprovementSlots * 100 * improvementsBasic.listOfImprovements[i].improvementLevel > tempFloat || tempTech == i)
			{
				if(tempTech == i)
				{
					tempFloat += improvementsBasic.tempImprovementSlots * 100 * improvementsBasic.listOfImprovements[i].improvementLevel;
				}
				else
				{
					tempFloat = improvementsBasic.tempImprovementSlots * 100 * improvementsBasic.listOfImprovements[i].improvementLevel;
					tempTech = i;
				}
			}
		}
		
		if(improvementsBasic.tempWealth * 25 * improvementsBasic.listOfImprovements[i].improvementLevel > improvementsBasic.listOfImprovements[i].improvementCost)
		{
			if(improvementsBasic.tempWealth * 25 * improvementsBasic.listOfImprovements[i].improvementLevel > tempFloat)
			{
				if(tempTech == i)
				{
					tempFloat += improvementsBasic.tempWealth * 25 * improvementsBasic.listOfImprovements[i].improvementLevel;
				}
				else
				{
					tempFloat = improvementsBasic.tempWealth * 25 * improvementsBasic.listOfImprovements[i].improvementLevel;
					tempTech = i;
				}
			}
		}
		
		if(improvementsBasic.tempImprovementCostReduction * 50 * improvementsBasic.listOfImprovements[i].improvementLevel > improvementsBasic.listOfImprovements[i].improvementCost)
		{
			if(improvementsBasic.tempImprovementCostReduction * 50 * improvementsBasic.listOfImprovements[i].improvementLevel > tempFloat)
			{
				if(tempTech == i)
				{
					tempFloat += improvementsBasic.tempImprovementCostReduction * 50 * improvementsBasic.listOfImprovements[i].improvementLevel;
				}
				else
				{
					tempFloat = improvementsBasic.tempImprovementCostReduction * 50 * improvementsBasic.listOfImprovements[i].improvementLevel;
					tempTech = i;
				}
			}
		}
		
		if(improvementsBasic.tempResearchCostReduction * 50 * improvementsBasic.listOfImprovements[i].improvementLevel > improvementsBasic.listOfImprovements[i].improvementCost)
		{
			if(improvementsBasic.tempResearchCostReduction * 50 * improvementsBasic.listOfImprovements[i].improvementLevel > tempFloat)
			{
				if(tempTech == i)
				{
					tempFloat += improvementsBasic.tempResearchCostReduction * 50 * improvementsBasic.listOfImprovements[i].improvementLevel;
				}
				else
				{
					tempFloat = improvementsBasic.tempResearchCostReduction * 50 * improvementsBasic.listOfImprovements[i].improvementLevel;
					tempTech = i;
				}
			}
		}
	}*/

	private void CheckToSaveForHero()
	{
		checkHeroTimer++;
		
		if(checkHeroTimer == 6)
		{
			float temp = 0;
			
			for(int j = 0; j < MasterScript.systemListConstructor.systemList.Count; ++j)
			{
				if(MasterScript.systemListConstructor.systemList[j].systemOwnedBy == thisPlayer.playerRace)
				{
					systemSIMData = MasterScript.systemListConstructor.systemList[j].systemObject.GetComponent<SystemSIMData>();
					temp += systemSIMData.totalSystemKnowledge + systemSIMData.totalSystemPower;
				}
				
				if(temp >= ((playerOwnedHeroes.Count * 20f) + 20f))
				{
					saveForHero = true;
				}

				else if(temp < ((playerOwnedHeroes.Count * 20f) + 20f))
				{
					saveForHero = false;
				}
			}

			checkHeroTimer = 0;
		}
	}

	public void AIExpansion()
	{
		if(thisPlayer.wealth > 1)
		{
			MasterScript.turnInfoScript.RefreshPlanetPower();

			currentPlanet = -1;
			currentSystem = -1;

			float planetSIM = CheckThroughPlanets (thisPlayer);
			
			float systemSIM = CheckThroughSystems (thisPlayer);
			
			if(planetSIM > systemSIM && thisPlayer.wealth >= MasterScript.systemListConstructor.systemList[tempSystem].planetsInSystem[tempPlanet].wealthValue)
			{
				currentPlanet = tempPlanet;
				
				currentSystem = tempSystem;

				thisPlayer.wealth -= MasterScript.systemListConstructor.systemList[tempSystem].planetsInSystem[tempPlanet].wealthValue;
			}
			
			if(systemSIM > planetSIM && thisPlayer.wealth >= 20.0f)
			{
				currentPlanet = tempPlanetB;
				
				currentSystem = tempSystemB;

				systemSIMData = MasterScript.systemListConstructor.systemList[currentSystem].systemObject.GetComponent<SystemSIMData>();

				systemSIMData.guardedBy = null;
				
				MasterScript.systemListConstructor.systemList[currentSystem].systemOwnedBy = thisPlayer.playerRace;
				
				MasterScript.voronoiGenerator.voronoiCells[currentSystem].renderer.material = thisPlayer.materialInUse;
				MasterScript.voronoiGenerator.voronoiCells[currentSystem].renderer.material.shader = Shader.Find("Transparent/Diffuse");

				++systemsInPlay;

				++thisPlayer.systemsColonisedThisTurn;

				thisPlayer.wealth -= 20.0f;
			}

			if(currentPlanet != -1 && currentSystem != -1)
			{
				MasterScript.systemListConstructor.systemList[currentSystem].planetsInSystem[currentPlanet].planetColonised = true;
				MasterScript.systemListConstructor.systemList [currentSystem].planetsInSystem [currentPlanet].expansionPenaltyTimer = Time.time;

				
				++thisPlayer.planetsColonisedThisTurn;
			}

			CheckToImprovePlanet (thisPlayer);
		}
	}

	public float CheckThroughPlanets(TurnInfo thisPlayer)
	{
		highestSIM = 0;

		for(int i = 0; i < MasterScript.systemListConstructor.mapSize; ++i)
		{
			if(MasterScript.systemListConstructor.systemList[i].systemOwnedBy == thisPlayer.playerRace)
			{
				systemDefence = MasterScript.systemListConstructor.systemList [i].systemObject.GetComponent<SystemDefence> ();
				
				if(systemDefence.underInvasion == true)
				{
					continue;
				}

				for(int j = 0; j < MasterScript.systemListConstructor.systemList[i].systemSize; ++j)
				{
					if(MasterScript.systemListConstructor.systemList[i].planetsInSystem[j].planetColonised == true || MasterScript.systemListConstructor.systemList[i].planetsInSystem[j].planetImprovementLevel == 3)
					{
						continue;
					}

					tempSIM = (MasterScript.systemListConstructor.systemList[i].planetsInSystem[j].planetKnowledge + MasterScript.systemListConstructor.systemList[i].planetsInSystem[j].planetPower)
						* (MasterScript.systemListConstructor.systemList[i].planetsInSystem[j].currentImprovementSlots * 1.5f);

					if(tempSIM > highestSIM)
					{
						highestSIM = tempSIM;

						tempPlanet = j;
						
						tempSystem = i;
					}
				}
			}
		}

		return highestSIM;
	}

	public float CheckThroughSystems(TurnInfo thisPlayer)
	{
		highestSIM = 0;

		for(int i = 0; i < MasterScript.systemListConstructor.mapSize; ++i)
		{
			if(MasterScript.systemListConstructor.systemList[i].systemOwnedBy == thisPlayer.playerRace)
			{
				for(int j = 0; j < MasterScript.systemListConstructor.systemList[i].permanentConnections.Count; ++j)
				{
					tempSIM = 0.0f;

					int k = MasterScript.RefreshCurrentSystem(MasterScript.systemListConstructor.systemList[i].permanentConnections[j]);

					systemSIMData = MasterScript.systemListConstructor.systemList[k].systemObject.GetComponent<SystemSIMData>();

					if(MasterScript.systemListConstructor.systemList[k].systemOwnedBy == null)
					{
						if(systemSIMData.guardedBy == "" || systemSIMData.guardedBy == thisPlayer.playerRace || systemSIMData.guardedBy == null)
						{
							float tempPlanetSIM = 0.0f;
							int tempPlanet = -1;
							float tempHighestPlanetSIM = 0.0f;

							for(int l = 0; l < MasterScript.systemListConstructor.systemList[k].systemSize; ++l)
							{
								tempPlanetSIM = (MasterScript.systemListConstructor.systemList[k].planetsInSystem[l].planetKnowledge + MasterScript.systemListConstructor.systemList[k].planetsInSystem[l].planetPower)  
									* (MasterScript.systemListConstructor.systemList[k].planetsInSystem[l].currentImprovementSlots);

								if(tempPlanetSIM > tempHighestPlanetSIM)
								{
									tempHighestPlanetSIM = tempPlanetSIM;

									tempPlanet = l;
								}

								tempSIM += tempPlanetSIM;
							}

							tempSIM = (tempSIM / MasterScript.systemListConstructor.systemList[k].systemSize);

							if(tempSIM > highestSIM)
							{
								highestSIM = tempSIM;

								tempSystemB = k;

								tempPlanetB = tempPlanet;
							}
						}
					}
				}
			}
		}

		return highestSIM;
	}

	public void CheckToImprovePlanet(TurnInfo thisPlayer)
	{
		for(int i = 0; i < MasterScript.turnInfoScript.mostPowerfulPlanets.Count - 1; i++)
		{
			if(thisPlayer.power < 0.8f && thisPlayer.wealth < 1.0f)
			{
				break;
			}

			int j = MasterScript.RefreshCurrentSystem(MasterScript.turnInfoScript.mostPowerfulPlanets[i].system);

			if(MasterScript.systemListConstructor.systemList[j].systemOwnedBy == thisPlayer.playerRace)
			{
				ImprovePlanet(MasterScript.turnInfoScript.mostPowerfulPlanets[i].planetPosition, j, thisPlayer);
			}
		}
	}
	
	public void ImprovePlanet(int planetPosition, int system, TurnInfo thisPlayer)
	{
		systemSIMData = MasterScript.systemListConstructor.systemList [system].systemObject.GetComponent<SystemSIMData> ();

		systemDefence = MasterScript.systemListConstructor.systemList [system].systemObject.GetComponent<SystemDefence> ();

		systemSIMData.improvementNumber = MasterScript.systemListConstructor.systemList[system].planetsInSystem[planetPosition].planetImprovementLevel;
		
		MasterScript.systemFunctions.CheckImprovement(system, planetPosition);
		
		if(systemSIMData.canImprove == true && systemDefence.underInvasion == false)
		{
			float powerImprovementCost = MasterScript.systemFunctions.PowerCost(systemSIMData.improvementNumber, system, planetPosition);

			if(thisPlayer.power >= powerImprovementCost && thisPlayer.wealth >= systemSIMData.improvementCost)
			{
				++MasterScript.systemListConstructor.systemList[system].planetsInSystem[planetPosition].planetImprovementLevel;
				
				thisPlayer.power -= powerImprovementCost;

				thisPlayer.wealth -= systemSIMData.improvementCost;
			}
		}
	}
}

