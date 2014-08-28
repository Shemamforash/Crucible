using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SystemSIMData : MonoBehaviour
{
	[HideInInspector]
	public int improvementNumber, antiStealthPower, thisSystem;
	[HideInInspector]
	public float knowledgeUnitBonus, powerUnitBonus, improvementCost, baseResourceBonus, adjacencyBonus, powerBuffModifier, knowledgeBuffModifier, embargoTimer, promotionTimer, populationToAdd;
	[HideInInspector]
	public string improvementLevel, promotedBy = null, embargoedBy = null, guardedBy = null;
	[HideInInspector]
	public List<PlanetUIInfo> allPlanetsInfo = new List<PlanetUIInfo>();	//Unique to object
	[HideInInspector]
	public bool canImprove, foundPlanetData;
	public GameObject protectedBy = null;
	public int secondaryResourceGeneratedSinceLastUpdate, totalSecondaryResourcesGeneratedInSystem;

	public float totalSystemKnowledge, totalSystemPower, totalSystemSIM, totalSystemAmber, totalSystemWealth;
	public float flResourceModifier, flgrowthModifier, flOffDefModifier;
	public float secRecPowerMod, secRecKnowledgeMod, secRecPopulationMod;
	public float planetKnowledgeModifier, planetPowerModifier;
	public float systemKnowledgeModifier, systemPowerModifier, systemgrowthModifier;
	private TurnInfo thisPlayer;
	private SystemDefence systemDefence;
	private ImprovementsBasic improvementsBasic;
	private SystemSIMData systemSIMData;

	void Start()
	{
		systemDefence = gameObject.GetComponent<SystemDefence> ();
		improvementsBasic = gameObject.GetComponent<ImprovementsBasic>();

		thisSystem = MasterScript.RefreshCurrentSystem (gameObject);

		for(int i = 0; i < MasterScript.systemListConstructor.systemList[thisSystem].systemSize; ++i)
		{
			PlanetUIInfo planetInfo = new PlanetUIInfo();

			planetInfo.generalInfo = null;
			planetInfo.knowledgeOutput = null;
			planetInfo.powerOutput = null;
			planetInfo.population = null;

			allPlanetsInfo.Add(planetInfo);
		}

		embargoedBy = null;
		promotedBy = null;
	}

	private void CheckSecRecBonus(int system)
	{
		for(int i = 0; i < MasterScript.systemListConstructor.systemList[system].systemSize; ++i)
		{
			if(MasterScript.systemListConstructor.systemList[system].planetsInSystem[i].rareResourceType != null)
			{
				switch(MasterScript.systemListConstructor.systemList[system].planetsInSystem[i].rareResourceType)
				{
				case "ANTIMATTER":
					break;
				case "LIQUID HYDROGEN":
					secRecKnowledgeMod += 0.01f * thisPlayer.liquidH2;
					break;
				case "BLUE CARBON":
					secRecPopulationMod += 0.01f * thisPlayer.blueCarbon;
					break;
				case "RADIOISOTOPES":
					secRecPowerMod += 0.01f * thisPlayer.radioisotopes;
					break;
				default:
					break;
				}
			}
		}
	}

	public void CheckForSecondaryResourceIncrease(int planet, TurnInfo player)
	{
		if(MasterScript.systemListConstructor.systemList[thisSystem].planetsInSystem[planet].rareResourceType != null)
		{
			int rnd = UnityEngine.Random.Range (0, 100);

			if(rnd < 4 * improvementsBasic.resourceYieldBonus)
			{
				switch(MasterScript.systemListConstructor.systemList[thisSystem].planetsInSystem[planet].rareResourceType)
				{
				case "ANTIMATTER":
					++player.antimatter;
					break;
				case "LIQUID HYDROGEN":
					++player.liquidH2;
					break;
				case "BLUE CARBON":
					++player.blueCarbon;
					break;
				case "RADIOISOTOPES":
					++player.radioisotopes;
					break;
				default:
					break;
				}

				++secondaryResourceGeneratedSinceLastUpdate;
				++totalSecondaryResourcesGeneratedInSystem;
			}
		}
	}

	public void SystemSIMCounter(TurnInfo player) //This functions is used to add up all resources outputted by planets within a system, with improvement and tech modifiers applied
	{
		float tempTotalSci = 0.0f, tempTotalInd = 0.0f;
		secRecPowerMod = 1f; secRecKnowledgeMod = 1f; secRecPopulationMod = 1f;
		thisPlayer = player;
		CheckFrontLineBonus ();
		CheckSecRecBonus(thisSystem);
		CalculateSystemModifierValues();
		int planetsColonised = 0;

		for(int j = 0; j < MasterScript.systemListConstructor.systemList[thisSystem].systemSize; ++j)
		{
			if(MasterScript.systemListConstructor.systemList[thisSystem].planetsInSystem[j].planetColonised == true)
			{
				++planetsColonised;

				CheckForSecondaryResourceIncrease(j, player);

				tempTotalSci += CheckPlanetValues(j, "Knowledge");
				tempTotalInd += CheckPlanetValues(j, "Power");
			}
		}

		totalSystemWealth = player.raceWealth * planetsColonised * MasterScript.turnInfoScript.expansionPenaltyModifier;
		totalSystemKnowledge = (tempTotalSci + knowledgeUnitBonus)  * MasterScript.turnInfoScript.expansionPenaltyModifier;
		totalSystemPower = (tempTotalInd + powerUnitBonus) * MasterScript.turnInfoScript.expansionPenaltyModifier;

		if(thisPlayer.playerRace == "Selkies")
		{
			MasterScript.racialTraitScript.IncreaseAmber(thisSystem);
		}

		IncreasePopulation ();

		totalSystemWealth -= improvementsBasic.upkeepWealth * improvementsBasic.upkeepModifier;
		totalSystemPower -= improvementsBasic.upkeepPower * improvementsBasic.upkeepModifier;
		totalSystemWealth += improvementsBasic.wealthBonus;
		player.wealth += totalSystemWealth;
	}

	private void CalculateSystemModifierValues()
	{
		systemKnowledgeModifier =  MasterScript.systemListConstructor.systemList[thisSystem].sysKnowledgeModifier 
			* MasterScript.systemListConstructor.systemList[thisSystem].sysAmberPenalty * EmbargoPenalty() * PromoteBonus() * flResourceModifier;
		systemPowerModifier =  MasterScript.systemListConstructor.systemList[thisSystem].sysPowerModifier 
			* MasterScript.systemListConstructor.systemList[thisSystem].sysAmberPenalty * MasterScript.racialTraitScript.NereidesPowerModifer (thisPlayer) * EmbargoPenalty () * PromoteBonus () * flResourceModifier;
		systemgrowthModifier = MasterScript.systemListConstructor.systemList[thisSystem].sysGrowthModifier 
			* MasterScript.systemListConstructor.systemList[thisSystem].sysAmberPenalty * flgrowthModifier;
	}

	public float CheckPlanetValues(int planet, string resource)
	{
		CalculatePlanetModifierValues (planet);
		
		float tempSci = 0, tempInd = 0;
		
		improvementNumber = MasterScript.systemListConstructor.systemList[thisSystem].planetsInSystem[planet].planetImprovementLevel;
		
		MasterScript.systemFunctions.CheckImprovement(thisSystem, planet);
		
		tempSci = MasterScript.systemListConstructor.systemList [thisSystem].planetsInSystem [planet].planetKnowledge * 
			(systemKnowledgeModifier + MasterScript.systemListConstructor.systemList[thisSystem].planetsInSystem[planet].knowledgeModifier) * MasterScript.turnInfoScript.expansionPenaltyModifier * planetKnowledgeModifier;
		tempInd = MasterScript.systemListConstructor.systemList [thisSystem].planetsInSystem [planet].planetPower * 
			(systemKnowledgeModifier + MasterScript.systemListConstructor.systemList[thisSystem].planetsInSystem[planet].knowledgeModifier) * MasterScript.turnInfoScript.expansionPenaltyModifier * planetPowerModifier;
		
		if(MasterScript.systemListConstructor.systemList[thisSystem].planetsInSystem[planet].planetColonised == true)
		{
			string sOut = Math.Round(tempSci, 1).ToString();
			string iOut = Math.Round (tempInd,1).ToString();
			string curPop = Math.Round (MasterScript.systemListConstructor.systemList[thisSystem].planetsInSystem[planet].planetPopulation, 1).ToString () + "%";

			allPlanetsInfo[planet].generalInfo = improvementLevel;
			allPlanetsInfo[planet].knowledgeOutput = sOut;
			allPlanetsInfo[planet].powerOutput = iOut;
			allPlanetsInfo[planet].population = curPop;
		}
		
		switch(resource)
		{
		case "Knowledge":
			return tempSci;
		case "Power":
			return tempInd;
		default:
			return 0;
		}
	}

	private void CalculatePlanetModifierValues(int planet)
	{
		systemDefence.CheckStatusEffects(planet);
		
		baseResourceBonus = MasterScript.systemListConstructor.systemList[thisSystem].planetsInSystem[planet].planetPopulation / 6.666f;
		planetKnowledgeModifier = (thisPlayer.raceKnowledge + secRecKnowledgeMod) * baseResourceBonus * knowledgeBuffModifier;
		planetPowerModifier = (thisPlayer.racePower + secRecPowerMod) * baseResourceBonus * powerBuffModifier;
	}	

	public void IncreasePopulation() //Used to increase the population of planets by their growth rate
	{
		for(int j = 0; j < MasterScript.systemListConstructor.systemList[thisSystem].systemSize; ++j) //For all planets in system
		{
			if(MasterScript.systemListConstructor.systemList[thisSystem].planetsInSystem[j].planetColonised == true) //If it has been colonised
			{
				if(systemDefence.underInvasion == false) //If system is not being invaded allow growth
				{
					improvementNumber = MasterScript.systemListConstructor.systemList[thisSystem].planetsInSystem[j].planetImprovementLevel; //Get the planets current improvement number
					
					MasterScript.systemFunctions.CheckImprovement(thisSystem, j); //Check the planets max ownership level

					float maxPopPlanet = MasterScript.systemListConstructor.systemList[thisSystem].sysMaxPopulationModifier + MasterScript.systemListConstructor.systemList[thisSystem].planetsInSystem[j].maxPopulationModifier
						+ MasterScript.systemListConstructor.systemList[thisSystem].planetsInSystem[j].maxPopulation;

					maxPopPlanet = Mathf.RoundToInt(maxPopPlanet);

					populationToAdd = (0.1f + systemgrowthModifier + MasterScript.systemListConstructor.systemList[thisSystem].planetsInSystem[j].growthModifier) * secRecPopulationMod; //Growth is the standard growth rate for the planets in the system multiplied by secondary resource modifiers

					if(MasterScript.systemListConstructor.systemList[thisSystem].planetsInSystem[j].planetPopulation < 0) //If population is less than 0, the planet must be reset
					{
						MasterScript.systemListConstructor.systemList[thisSystem].planetsInSystem[j].planetPopulation = 0;
						MasterScript.WipePlanetInfo(thisSystem, j);
						continue;
					}

					MasterScript.systemListConstructor.systemList[thisSystem].planetsInSystem[j].planetPopulation += populationToAdd; //Add the growth to the population

					if(MasterScript.systemListConstructor.systemList[thisSystem].planetsInSystem[j].planetPopulation >= maxPopPlanet) //If the current population is greater than the maximum allowed population
					{
						MasterScript.systemListConstructor.systemList[thisSystem].planetsInSystem[j].planetPopulation = maxPopPlanet; //Set the current population to equal max
						continue;
					}
				}
			}
		}
	}

	private void CheckFrontLineBonus()
	{
		flResourceModifier = 1f;
		flgrowthModifier = 1f;
		flOffDefModifier = 1f;

		int noSystems = 0;

		for(int i = 0; i < MasterScript.systemListConstructor.systemList[thisSystem].permanentConnections.Count; ++i)
		{
			int neighbour = MasterScript.RefreshCurrentSystem(MasterScript.systemListConstructor.systemList[thisSystem].permanentConnections[i]);

			if(MasterScript.systemListConstructor.systemList[neighbour].systemOwnedBy != null && MasterScript.systemListConstructor.systemList[neighbour].systemOwnedBy != thisPlayer.playerRace)
			{
				DiplomaticPosition temp = MasterScript.diplomacyScript.ReturnDiplomaticRelation(MasterScript.systemListConstructor.systemList[thisSystem].systemOwnedBy, MasterScript.systemListConstructor.systemList[neighbour].systemOwnedBy);

				flResourceModifier += temp.resourceModifier;
				flgrowthModifier += temp.growthModifier;
				flOffDefModifier += temp.offDefModifier;
				++noSystems;
			}
		}

		if(noSystems != 0)
		{
			flResourceModifier = flResourceModifier / noSystems;
			flgrowthModifier = flgrowthModifier / noSystems;
			flOffDefModifier = flOffDefModifier / noSystems;
		}
	}

	private float PromoteBonus() //Calculates resource bonus from promotions on enemy systems
	{
		float totalAdjacencyBonus = 1f;

		if(promotedBy == null)
		{
			for(int i = 0; i < MasterScript.systemListConstructor.systemList[thisSystem].permanentConnections.Count; ++i)
			{
				int j = MasterScript.RefreshCurrentSystem(MasterScript.systemListConstructor.systemList[thisSystem].permanentConnections[i]);

				systemSIMData = MasterScript.systemListConstructor.systemList[j].systemObject.GetComponent<SystemSIMData>();

				if(systemSIMData.promotedBy != null)
				{
					if(MasterScript.systemListConstructor.systemList[j].systemOwnedBy == thisPlayer.playerRace)
					{
						totalAdjacencyBonus += 0.05f;
					}

					else
					{
						totalAdjacencyBonus += 0.1f;
					}
				}
			}
		}
		
		else if(promotedBy != null)
		{
			if(promotionTimer + 30.0f < Time.time)
			{
				promotedBy = null;
			}

			DiplomaticPosition temp = MasterScript.diplomacyScript.ReturnDiplomaticRelation (thisPlayer.playerRace, promotedBy);

			++temp.stateCounter;

			totalAdjacencyBonus = 1.5f;
		}

		return totalAdjacencyBonus;
	}

	private float EmbargoPenalty() //Calculates penalties from Embargoes
	{
		if(embargoedBy != null)
		{
			DiplomaticPosition temp = MasterScript.diplomacyScript.ReturnDiplomaticRelation (thisPlayer.playerRace, embargoedBy);

			--temp.stateCounter;

			if(embargoTimer + 20.0f < Time.time)
			{
				embargoedBy = null;
			}

			float embargoPenalty = 1 - temp.resourceModifier;

			return embargoPenalty;
		}

		return 1;
	}

	public void UpdatePlanetPowerArray()
	{
		for(int i = 0; i < MasterScript.systemListConstructor.systemList[thisSystem].systemSize; ++i)
		{
			PlanetPower planet = new PlanetPower();

			planet.system = gameObject;

			improvementNumber = MasterScript.systemListConstructor.systemList[thisSystem].planetsInSystem[i].planetImprovementLevel;
			
			MasterScript.systemFunctions.CheckImprovement(thisSystem, i);

			float tempSIM = (MasterScript.systemListConstructor.systemList[thisSystem].planetsInSystem[i].planetKnowledge + MasterScript.systemListConstructor.systemList[thisSystem].planetsInSystem[i].planetPower)
							* MasterScript.systemListConstructor.systemList[thisSystem].planetsInSystem[i].planetPopulation / 66.6666f;

			planet.simOutput = tempSIM;

			planet.planetPosition = i;
				
			MasterScript.turnInfoScript.mostPowerfulPlanets.Add (planet);

			++MasterScript.turnInfoScript.savedIterator;
		}
	}
}

public class PlanetUIInfo
{
	public string generalInfo, knowledgeOutput, powerOutput, population;
}
