using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

public class ImprovementsBasic : MasterScript 
{
	public float knowledgePercentBonus, powerPercentBonus, amberPenalty, amberProductionBonus, amberPointBonus, knowledgeTechModifier, growthModifier, maxPopulationBonus, resourceYieldBonus, wealthBonus;
	public float researchCostReduction, improvementCostReduction, upkeepModifier, tempCount, tempBonusAmbition, expansionPenaltyModifier, improvementSlotsBonus;
	public List<string> planetToBuildOn;
	public GameObject tooltip;
	public int techTier = 0, improvementCostModifier = 0, researchCost, system;
	private GenericImprovements genericImprovements;
	public float upkeepPower, upkeepWealth;

	public List<ImprovementClass> listOfImprovements = new List<ImprovementClass>();

	void Start()
	{
		planetToBuildOn = new List<string>();

		knowledgePercentBonus = 0; powerPercentBonus = 0;

		systemSIMData = gameObject.GetComponent<SystemSIMData>(); //References to scripts again.
		lineRenderScript = gameObject.GetComponent<LineRenderScript>();
		heroScript = gameObject.GetComponent<HeroScriptParent>();
		genericImprovements = GameObject.Find ("ScriptsContainer").GetComponent<GenericImprovements> ();

		LoadNewTechTree();
	}

	public bool ImproveSystem(int improvement) //Occurs if button of tech is clicked.
	{
		if(playerTurnScript.power >= (listOfImprovements[improvement].improvementCost - improvementCostModifier)) //Checks cost of tech and current power
		{
			playerTurnScript.power -= (listOfImprovements[improvement].improvementCost - improvementCostModifier);
			listOfImprovements[improvement].hasBeenBuilt = true;
			return true;
		}
		else
		{
			return false;
		}
	}

	private void LoadNewTechTree() //Loads tech tree into two arrays (whether tech has been built, and the cost of each tech)
	{		
		for(int i = 0; i < systemListConstructor.basicImprovementsList.Count; ++i)
		{
			ImprovementClass newImprovement = new ImprovementClass();

			newImprovement.improvementName = systemListConstructor.basicImprovementsList[i].name;
			newImprovement.improvementCategory = systemListConstructor.basicImprovementsList[i].category;
			newImprovement.improvementCost = systemListConstructor.basicImprovementsList[i].cost;
			newImprovement.improvementLevel = systemListConstructor.basicImprovementsList[i].level;
			newImprovement.improvementMessage = "";
			newImprovement.hasBeenBuilt = false;

			listOfImprovements.Add(newImprovement);
		}
	}

	private void ResetModifiers()
	{
		knowledgePercentBonus = 1f; //Bonus knowledge % for system
		powerPercentBonus = 1f; //Bonus power % for system
		improvementCostModifier = 1; //Modifier of cost of improvements on system
		knowledgeTechModifier = 1f; //Modifier of bonus knowledge
		growthModifier = 1f; //Modifier of max population
		amberPenalty = 1f; //Modifer of all resources due to amber production
		amberPointBonus = 1f; //Bonus to amber production in units
		amberProductionBonus = 1f; //Bonus to amber production in %
		researchCost = 1; //Modifier of research costs
		maxPopulationBonus = 0f; 
		resourceYieldBonus = 1f;
		tempCount = 0.0f;
		improvementSlotsBonus = 0f;
	}

	private void AssignSystemModifierValues(int system, int planet)
	{
		if(planet == -1)
		{
			systemListConstructor.systemList[system].sysKnowledgeModifier = knowledgePercentBonus;
			systemListConstructor.systemList[system].sysPowerModifier = powerPercentBonus;
			systemListConstructor.systemList[system].sysGrowthModifier = growthModifier;
			systemListConstructor.systemList[system].sysAmberPenalty = amberPenalty;
			systemListConstructor.systemList[system].sysAmberModifier = amberProductionBonus;
			systemListConstructor.systemList[system].sysMaxPopulationModifier = maxPopulationBonus;
			systemListConstructor.systemList[system].sysResourceModifier = resourceYieldBonus;
		}
		else
		{
			systemListConstructor.systemList[system].planetsInSystem[planet].knowledgeModifier = knowledgePercentBonus;
			systemListConstructor.systemList[system].planetsInSystem[planet].powerModifier = powerPercentBonus;
			systemListConstructor.systemList[system].planetsInSystem[planet].growthModifier = growthModifier;
			systemListConstructor.systemList[system].planetsInSystem[planet].amberPenalty = amberPenalty;
			systemListConstructor.systemList[system].planetsInSystem[planet].amberModifier = amberProductionBonus;
			systemListConstructor.systemList[system].planetsInSystem[planet].maxPopulationModifier = maxPopulationBonus;
			systemListConstructor.systemList[system].planetsInSystem[planet].resourceModifier = resourceYieldBonus;
		}
	}
	
	public void ActiveTechnologies(int curSystem, TurnInfo thisPlayer) //Contains reference to all technologies. Will activate relevant functions etc. if tech is built. Should be turned into a switch rather than series of ifs.
	{
		upkeepPower = 0f;
		upkeepWealth = 0f;
		upkeepModifier = 1f;
		expansionPenaltyModifier = 1f;
		wealthBonus = 0;

		ResetModifiers();

		for(int i = 0; i < listOfImprovements.Count; ++i) //For every improvement
		{
			if(listOfImprovements[i].hasBeenBuilt == true) //If it has been built
			{
				if(systemListConstructor.basicImprovementsList[i].influence == "System") //If the improvement influence is system wide
				{
					genericImprovements.TechSwitch(i, -1, this, thisPlayer, false); //Add the effects from the improvement to the modifier
				}

				upkeepPower += systemListConstructor.basicImprovementsList[i].powerUpkeep * upkeepModifier; //Total the upkeep of all built improvements
				upkeepWealth += systemListConstructor.basicImprovementsList[i].wealthUpkeep * upkeepModifier;
			}
		}

		AssignSystemModifierValues(curSystem, -1);

		for(int j = 0; j < systemListConstructor.systemList[curSystem].systemSize; ++j)
		{
			system = curSystem;

			ResetModifiers();

			for(int i = 0; i < listOfImprovements.Count; ++i)
			{
				if(listOfImprovements[i].hasBeenBuilt == true && systemListConstructor.basicImprovementsList[i].influence == "Planet")
				{
					genericImprovements.TechSwitch(i, j, this, thisPlayer, false);
				}
			}

			AssignSystemModifierValues(curSystem, j);
		}

		thisPlayer.wealth -= upkeepWealth;
		thisPlayer.power -= upkeepPower;

		knowledgePercentBonus = knowledgePercentBonus * knowledgeTechModifier;

		racialTraitScript.stacksGeneratedSinceLastUpdate = 0;
		racialTraitScript.stacksDissolvedSinceLastUpdate = 0;
		systemSIMData.secondaryResourceGeneratedSinceLastUpdate = 0;
	}
	
	public int CheckDiplomaticStateOfAllPlayers(TurnInfo thisPlayer, string state)
	{
		int noOfPlayersInState = 0;

		for(int i = 0; i < diplomacyScript.relationsList.Count; ++i)
		{
			if(diplomacyScript.relationsList[i].playerOne.playerRace == thisPlayer.playerRace || diplomacyScript.relationsList[i].playerTwo.playerRace == thisPlayer.playerRace)
			{
				diplomacyScript.relationsList[i].diplomaticState = state;
				++noOfPlayersInState;
			}
		}

		return noOfPlayersInState;
	}

	public bool IsBuiltOnPlanetType(int system, int improvementNo, string planetType)
	{
		for(int i = 0; i < systemListConstructor.systemList[system].systemSize; ++i)
		{
			for(int j = 0; j < systemListConstructor.systemList[system].planetsInSystem[i].improvementsBuilt.Count; ++j)
			{
				if(systemListConstructor.systemList[system].planetsInSystem[i].improvementsBuilt[j] == listOfImprovements[improvementNo].improvementName)
				{
					if(systemListConstructor.systemList[system].planetsInSystem[i].planetType == planetType)
					{
						return true;
					}
				}
			}
		}

		return false;
	}
}

public class ImprovementClass
{
	public string improvementName, improvementCategory;
	public float improvementCost;
	public int improvementLevel;
	public string improvementMessage;
	public bool hasBeenBuilt;
}


