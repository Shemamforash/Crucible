using UnityEngine;
using System.Collections;

public class NereidesImprovements : MasterScript 
{
	private ImprovementsBasic improvements;
	private bool checkValue;
	private TurnInfo player;
	
	public void TechSwitch(int tech, int planet, ImprovementsBasic tempImprov, TurnInfo thisPlayer, bool check)
	{
		improvements = tempImprov;

		systemSIMData = systemListConstructor.systemList [improvements.system].systemObject.GetComponent<SystemSIMData> ();

		checkValue = check;
		player = thisPlayer;
		
		switch (tech)
		{
		case 32:
			TN1I1();
			break;
		case 33:
			TN1I2();
			break;
		case 34:
			TN1I3();
			break;
		case 35:
			TN1I4();
			break;
		case 36:
			TN1I5(planet);
			break;
		case 37:
			TN2I1(planet);
			break;
		case 38:
			TN2I2();
			break;
		case 39:
			TN2I3();
			break;
		case 40:
			TN2I4(planet);
			break;
		case 41:
			TN2I5();
			break;
		case 42:
			TN3I1();
			break;
		case 43:
			TN3I2(planet);
			break;
		case 44:
			TN3I3();
			break;
		case 45:
			TN3I4(planet);
			break;
		case 46:
			TN3I5();
			break;
		case 47:
			TN4I1();
			break;
		case 48:
			TN4I2(planet);
			break;
		case 49:
			TN4I3(planet);
			break;
		case 50:
			TN4I4();
			break;
		case 51:
			TN4I5();
			break;
		default:
			break;
		}
	}

	private void TN1I1()
	{
		improvements.improvementCostReduction = racialTraitScript.nereidesStacks;

		if(checkValue == false)
		{
			improvements.improvementCostModifier += (int)improvements.tempImprovementCostReduction;
			improvements.listOfImprovements[32].improvementMessage = ("-" + racialTraitScript.nereidesStacks + " Power Cost for Improvements");
		}
	}

	private void TN1I2()
	{
		//TODO
	}

	private void TN1I3()
	{
		//TODO
	}

	private void TN1I4()
	{
		improvements.tempCount = 0.001 * racialTraitScript.nereidesStacks;
		improvements.knowledgePercentBonus += improvements.tempCount;

		if(checkValue == false)
		{
			improvements.listOfImprovements[35].improvementMessage = ("+" + improvements.tempCount * 100 + "% Knowledge from Stacks of Elation");
		}
	}

	private void TN1I5()
	{
		//TODO
	}

	private void TN2I1(int planet)
	{
		int wealthBonus = 1;

		if(improvements.IsBuiltOnPlanetType(improvements.system, 37, "Boreal") || improvements.IsBuiltOnPlanetType(improvements.system, 37, "Tundra") ||
		   improvements.IsBuiltOnPlanetType(improvements.system, 37, "Desolate"))
		{
			wealthBonus = 2;
		}

		improvements.upkeepWealth -= wealthBonus;

		if(checkValue == false)
		{
			improvements.listOfImprovements[37].improvementMessage = ("+" + wealthBonus + " Wealth on Planets");
		}
	}

	private void TN4I1()
	{
		if(improvements.IsBuiltOnPlanetType(improvements.system, 26, "Boreal") == true 
		   || improvements.IsBuiltOnPlanetType(improvements.system, 26, "Tundra") == true 
		   || improvements.IsBuiltOnPlanetType(improvements.system, 26, "Desolate") == true)
		{
			improvements.tempCount = 0.15f;
		}

		improvements.tempPopulationBonus = improvements.tempCount * racialTraitScript.nereidesStacks;
		improvements.tempKnwlUnitBonus = systemSIMData.totalSystemKnowledge * (improvements.tempPopulationBonus / 66.666f);
		improvements.tempPowUnitBonus = systemSIMData.totalSystemPower * (improvements.tempPopulationBonus / 66.666f);

		improvements.planetToBuildOn.Add ("Boreal");
		improvements.planetToBuildOn.Add ("Tundra");
		improvements.planetToBuildOn.Add ("Desolate");

		if(checkValue == false)
		{
			improvements.growthModifier += improvements.tempPopulationBonus;
			improvements.listOfImprovements[26].improvementMessage = ("+" + improvements.tempCount * racialTraitScript.nereidesStacks + "Population from Elation");
		}
	}

	private void TN4I2()
	{
		if(systemListConstructor.systemList[improvements.system].systemDefence < systemDefence.maxSystemDefence)
		{
			improvements.tempKnwlBonus = 1f;
			improvements.tempPowBonus = 1f;
		}

		improvements.tempPowUnitBonus = systemSIMData.totalSystemPower * improvements.tempPowBonus;
		improvements.tempKnwlUnitBonus = systemSIMData.totalSystemKnowledge * improvements.tempKnwlBonus;

		if(checkValue == false)
		{
			improvements.knowledgePercentBonus += 1f;
			improvements.powerPercentBonus += 1f;
			improvements.listOfImprovements[27].improvementMessage = ("+100% Resource Production from Invasion");
		}
	}
}
