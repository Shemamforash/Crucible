using UnityEngine;
using System.Collections;

public class SelkiesImprovements : MasterScript 
{
	private ImprovementsBasic improvements;
	private bool checkValue;
	private TurnInfo thisPlayer;
	
	public void TechSwitch(int tech, ImprovementsBasic tempImprov, TurnInfo player, bool check)
	{
		systemSIMData = systemListConstructor.systemList[improvements.system].systemObject.GetComponent<SystemSIMData>();

		improvements = tempImprov;
		checkValue = check;
		thisPlayer = player;
		
		switch (tech) //The order of these is important
		{
		case 28:
			TS1I1();
			break;
		case 29:
			TS1I2();
			break;
		case 30:
			TS2I2();
			break;
		case 31:
			TS3I2();
			break;
		case 32:
			TS4I1();
			break;
		case 33:
			TS4I2 ();
			break;
		case 34:
			TS2I1();
			break;
		case 35:
			TS3I1();
			break;
		case 36:
			TS1I0();
			break;
		default:
			break;
		}
	}

	private void TS1I0()
	{
		improvements.tempAmberPenalty = improvements.amberPenalty;

		if(checkValue == false)
		{
			improvements.listOfImprovements[28].improvementMessage = ("System is suffering -" + improvements.amberPenalty * 100 + "% Resource production from Amber Penalty");
		}
	}

	private void TS1I1()
	{
		improvements.tempImprovementCostReduction = systemSIMData.totalSystemAmber;

		if(checkValue == false)
		{
			improvements.improvementCostModifier += (int)improvements.tempImprovementCostReduction;
			improvements.listOfImprovements[29].improvementMessage = ("System Improvements cost " + systemSIMData.totalSystemAmber + " fewer Power from Amber production");
		}
	}

	private void TS1I2()
	{
		int adjacentSystems = 0;
		
		for(int i = 0; i < systemListConstructor.systemList[improvements.system].permanentConnections.Count; ++i)
		{
			int j = RefreshCurrentSystem(systemListConstructor.systemList[improvements.system].permanentConnections[i]);
			
			if(systemListConstructor.systemList[j].systemOwnedBy == thisPlayer.playerRace)
			{
				++adjacentSystems;
			}
		}

		improvements.tempAmberPenalty -= adjacentSystems * 0.05f;

		if(checkValue == false)
		{
			improvements.amberPenalty -= improvements.tempAmberPenalty;
			improvements.listOfImprovements[30].improvementMessage = ("-" + (adjacentSystems * 0.05f) + " Amber Penalty from adjacent Selkies Systems");
		}
	}

	private void TS2I1()
	{
		improvements.tempAmberPointBonus -= 4f;
		improvements.tempAmberPenalty = improvements.amberPenalty / 2f;

		if(checkValue == false)
		{
			improvements.amberPointBonus -= improvements.tempAmberPointBonus;
			improvements.amberPenalty = improvements.tempAmberPenalty;
			improvements.listOfImprovements[31].improvementMessage = "-4 Amber production, Amber penalty is halved";
		}
	}

	private void TS2I2()
	{
		improvements.tempAmberProductionBonus += systemSIMData.totalSystemPower * 0.01f;
		
		if(checkValue == false)
		{
			improvements.amberProductionBonus += improvements.tempAmberProductionBonus;
			improvements.listOfImprovements[32].improvementMessage = ("+" + (systemSIMData.totalSystemPower * 0.01f) + "% Amber production from Power production");
		}
	}

	private void TS3I1()
	{
		improvements.tempAmberPointBonus -= 2f;
		improvements.tempAmberPenalty = improvements.amberPenalty / 2f;

		if(checkValue == false)
		{
			improvements.amberPointBonus -= improvements.tempAmberPointBonus;
			improvements.amberPenalty = improvements.tempAmberPenalty;
			improvements.listOfImprovements[33].improvementMessage = "-2 Amber production, Amber penalty is halved";
		}
	}

	private void TS3I2()
	{
		if(systemSIMData.totalSystemAmber > 10.0f)
		{
			improvements.tempAmberProductionBonus += 0.5f;
		}
		
		if(checkValue == false)
		{
			improvements.amberProductionBonus += improvements.tempAmberProductionBonus;
			improvements.listOfImprovements[34].improvementMessage = ("+" + improvements.tempAmberProductionBonus * 100 + "% Amber production from Amber excess");
		}
	}

	private void TS4I1()
	{
		improvements.tempPopulationBonus += systemSIMData.totalSystemAmber;

		improvements.tempKnwlUnitBonus = systemSIMData.totalSystemKnowledge * (improvements.tempPopulationBonus / 66.666f);
		improvements.tempPowUnitBonus = systemSIMData.totalSystemPower * (improvements.tempPopulationBonus / 66.666f);

		if(checkValue == false)
		{
			improvements.maxPopulationBonus += improvements.tempPopulationBonus;
			improvements.listOfImprovements[35].improvementMessage = ("+" + systemSIMData.totalSystemAmber + "% Population Cap from Amber production");
		}
	}

	private void TS4I2() //This also needs a value
	{
		for(int i = 0; i < systemListConstructor.systemList[improvements.system].systemSize; ++i)
		{
			string tempString = systemListConstructor.systemList[improvements.system].planetsInSystem[i].planetType;
			
			if(tempString == "Molten")
			{
				improvements.tempImprovementSlots += 2;

				if(checkValue == false)
				{
					systemListConstructor.systemList[improvements.system].planetsInSystem[i].improvementSlots += 2;
					systemListConstructor.systemList[improvements.system].planetsInSystem[i].improvementsBuilt.Add (null);
					systemListConstructor.systemList[improvements.system].planetsInSystem[i].improvementsBuilt.Add (null);
				}
			}
			
			if(tempString == "Prairie")
			{
				improvements.tempImprovementSlots += 1;

				if(checkValue == false)
				{
					systemListConstructor.systemList[improvements.system].planetsInSystem[i].improvementSlots += 1;
					systemListConstructor.systemList[improvements.system].planetsInSystem[i].improvementsBuilt.Add (null);
				}
			}
		}

		improvements.planetToBuildOn.Add ("Molten");
		improvements.planetToBuildOn.Add ("Prairie");
	
		if(checkValue == false)
		{
			improvements.listOfImprovements[36].improvementMessage = ("+1/+2 Improvement Slot(s) on Prairie/Molten Planets");
		}
	}
}
