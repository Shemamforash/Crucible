﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

public class ImprovementsBasic : MonoBehaviour 
{
	public float knowledgePercentBonus, powerPercentBonus, amberPenalty, amberProductionBonus, amberPointBonus, knowledgeTechModifier, growthModifier, maxPopulationBonus, resourceYieldBonus, wealthBonus;
	public float researchCostReduction, improvementCostReduction, upkeepModifier, tempCount, tempBonusAmbition, ambitionPenalty, expansionPenaltyModifier;
	public List<string> planetToBuildOn;
	public GameObject tooltip;
	public int techTier = 0, improvementCostModifier = 0, researchCost, system, improvementSlotsBonus;
	private GenericImprovements genericImprovements;
	private SystemSIMData systemSIMData;
	public float upkeepPower, upkeepWealth;

	public List<ImprovementClass> listOfImprovements = new List<ImprovementClass>();

	void Start()
	{
		planetToBuildOn = new List<string>();

		knowledgePercentBonus = 0; powerPercentBonus = 0;

		systemSIMData = gameObject.GetComponent<SystemSIMData>(); //References to scripts again.
		genericImprovements = GameObject.Find ("ScriptsContainer").GetComponent<GenericImprovements> ();

		LoadNewTechTree();
	}

	public bool ImproveSystem(int improvement) //Occurs if button of tech is clicked.
	{
		if(MasterScript.playerTurnScript.power >= (listOfImprovements[improvement].improvementCost - improvementCostModifier)) //Checks cost of tech and current power
		{
			MasterScript.playerTurnScript.power -= (listOfImprovements[improvement].improvementCost - improvementCostModifier);
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
		for(int i = 0; i < MasterScript.systemListConstructor.basicImprovementsList.Count; ++i)
		{
			ImprovementClass newImprovement = new ImprovementClass();

			newImprovement.improvementName = MasterScript.systemListConstructor.basicImprovementsList[i].name;
			newImprovement.improvementCategory = MasterScript.systemListConstructor.basicImprovementsList[i].category;
			newImprovement.improvementCost = MasterScript.systemListConstructor.basicImprovementsList[i].cost;
			newImprovement.improvementLevel = MasterScript.systemListConstructor.basicImprovementsList[i].level;
			newImprovement.improvementMessage = "";
			newImprovement.hasBeenBuilt = false;

			listOfImprovements.Add(newImprovement);
		}
	}

	private void ResetModifiers()
	{
		knowledgePercentBonus = 1f; //Bonus knowledge % for system
		powerPercentBonus = 1f; //Bonus power % for system
		improvementCostModifier = 0; //Modifier of cost of improvements on system
		knowledgeTechModifier = 0f; //Modifier of bonus knowledge
		growthModifier = 0f; //Modifier of max population
		amberPenalty = 1f; //Modifer of all resources due to amber production
		amberPointBonus = 1f; //Bonus to amber production in units
		amberProductionBonus = 1f; //Bonus to amber production in %
		researchCost = 1; //Modifier of research costs
		maxPopulationBonus = 0f; 
		resourceYieldBonus = 1f;
		tempCount = 0.0f;
		improvementSlotsBonus = 0;
		ambitionPenalty = 1f;
	}

	private void AssignSystemModifierValues(int system, int planet)
	{
		if(planet == -1)
		{
			MasterScript.systemListConstructor.systemList[system].sysKnowledgeModifier = knowledgePercentBonus;
			MasterScript.systemListConstructor.systemList[system].sysPowerModifier = powerPercentBonus;
			MasterScript.systemListConstructor.systemList[system].sysGrowthModifier = growthModifier * ambitionPenalty * MasterScript.racialTraitScript.ambitionCounter / 40f;
			MasterScript.systemListConstructor.systemList[system].sysAmberPenalty = amberPenalty;
			MasterScript.systemListConstructor.systemList[system].sysAmberModifier = amberProductionBonus;
			MasterScript.systemListConstructor.systemList[system].sysMaxPopulationModifier = maxPopulationBonus;
			MasterScript.systemListConstructor.systemList[system].sysResourceModifier = resourceYieldBonus;
		}
		else
		{
			MasterScript.systemListConstructor.systemList[system].planetsInSystem[planet].knowledgeModifier = (knowledgePercentBonus - 1) * knowledgeTechModifier;
			MasterScript.systemListConstructor.systemList[system].planetsInSystem[planet].powerModifier = powerPercentBonus - 1;
			MasterScript.systemListConstructor.systemList[system].planetsInSystem[planet].growthModifier = growthModifier * ambitionPenalty * MasterScript.racialTraitScript.ambitionCounter / 40f;
			MasterScript.systemListConstructor.systemList[system].planetsInSystem[planet].amberPenalty = amberPenalty - 1;
			MasterScript.systemListConstructor.systemList[system].planetsInSystem[planet].amberModifier = amberProductionBonus - 1;
			MasterScript.systemListConstructor.systemList[system].planetsInSystem[planet].maxPopulationModifier = maxPopulationBonus;
			MasterScript.systemListConstructor.systemList[system].planetsInSystem[planet].resourceModifier = resourceYieldBonus - 1;
		}
	}
	
	public void ActiveTechnologies(int curSystem, TurnInfo thisPlayer) //Contains reference to all technologies. Will activate relevant functions etc. if tech is built. Should be turned into a switch rather than series of ifs.
	{
		upkeepPower = 0f;
		upkeepWealth = 0f;
		upkeepModifier = 1f;
		expansionPenaltyModifier = 1f;
		wealthBonus = 0;
		knowledgeTechModifier = 1f;
		system = curSystem;

		ResetModifiers();

		for(int i = 0; i < listOfImprovements.Count; ++i) //For every improvement
		{
			if(listOfImprovements[i].hasBeenBuilt == true) //If it has been built
			{
				if(MasterScript.systemListConstructor.basicImprovementsList[i].influence == "System") //If the improvement influence is system wide
				{
					genericImprovements.TechSwitch(i, -1, this, thisPlayer, false); //Add the effects from the improvement to the modifier
				}

				upkeepPower += MasterScript.systemListConstructor.basicImprovementsList[i].powerUpkeep * upkeepModifier; //Total the upkeep of all built improvements
				upkeepWealth += MasterScript.systemListConstructor.basicImprovementsList[i].wealthUpkeep * upkeepModifier;
			}
		}

		AssignSystemModifierValues(curSystem, -1);

		for(int j = 0; j < MasterScript.systemListConstructor.systemList[curSystem].systemSize; ++j)
		{
			ResetModifiers();

			for(int i = 0; i < listOfImprovements.Count; ++i)
			{
				if(listOfImprovements[i].hasBeenBuilt == true && MasterScript.systemListConstructor.basicImprovementsList[i].influence == "Planet")
				{
					genericImprovements.TechSwitch(i, j, this, thisPlayer, false);
				}
			}

			AssignSystemModifierValues(curSystem, j);

			MasterScript.systemListConstructor.systemList[curSystem].planetsInSystem[j].currentImprovementSlots = improvementSlotsBonus + MasterScript.systemListConstructor.systemList[curSystem].planetsInSystem[j].baseImprovementSlots;

			if(MasterScript.systemListConstructor.systemList[curSystem].planetsInSystem[j].improvementsBuilt.Count > MasterScript.systemListConstructor.systemList[curSystem].planetsInSystem[j].currentImprovementSlots)
			{
				MasterScript.systemListConstructor.systemList[curSystem].planetsInSystem[j].improvementsBuilt.RemoveAt(MasterScript.systemListConstructor.systemList[curSystem].planetsInSystem[j].currentImprovementSlots - 1);
			}
			if(MasterScript.systemListConstructor.systemList[curSystem].planetsInSystem[j].improvementsBuilt.Count < MasterScript.systemListConstructor.systemList[curSystem].planetsInSystem[j].currentImprovementSlots)
			{
				while(MasterScript.systemListConstructor.systemList[curSystem].planetsInSystem[j].improvementsBuilt.Count != MasterScript.systemListConstructor.systemList[curSystem].planetsInSystem[j].currentImprovementSlots)
				{
					MasterScript.systemListConstructor.systemList[curSystem].planetsInSystem[j].improvementsBuilt.Add (null);
				}
			}
		}

		MasterScript.racialTraitScript.stacksGeneratedSinceLastUpdate = 0;
		MasterScript.racialTraitScript.stacksDissolvedSinceLastUpdate = 0;
		systemSIMData.secondaryResourceGeneratedSinceLastUpdate = 0;
	}
	
	public int CheckDiplomaticStateOfAllPlayers(TurnInfo thisPlayer, string state)
	{
		int noOfPlayersInState = 0;

		for(int i = 0; i < MasterScript.diplomacyScript.relationsList.Count; ++i)
		{
			if(MasterScript.diplomacyScript.relationsList[i].playerOne.playerRace == thisPlayer.playerRace || MasterScript.diplomacyScript.relationsList[i].playerTwo.playerRace == thisPlayer.playerRace)
			{
				MasterScript.diplomacyScript.relationsList[i].diplomaticState = state;
				++noOfPlayersInState;
			}
		}

		return noOfPlayersInState;
	}

	public bool IsBuiltOnPlanetType(int system, int improvementNo, string planetType)
	{
		for(int i = 0; i < MasterScript.systemListConstructor.systemList[system].systemSize; ++i)
		{
			for(int j = 0; j < MasterScript.systemListConstructor.systemList[system].planetsInSystem[i].improvementsBuilt.Count; ++j)
			{
				if(MasterScript.systemListConstructor.systemList[system].planetsInSystem[i].improvementsBuilt[j] == listOfImprovements[improvementNo].improvementName)
				{
					if(MasterScript.systemListConstructor.systemList[system].planetsInSystem[i].planetType == planetType)
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


