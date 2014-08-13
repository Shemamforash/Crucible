using UnityEngine;
using System.Collections;

public class SelkiesImprovements : MasterScript 
{
	private ImprovementsBasic improvements;
	private bool checkValue;
	private TurnInfo player;
	
	public void TechSwitch(int tech, int planet, ImprovementsBasic tempImprov, TurnInfo thisPlayer, bool check)
	{
		systemSIMData = systemListConstructor.systemList[improvements.system].systemObject.GetComponent<SystemSIMData>();

		improvements = tempImprov;
		checkValue = check;
		player = thisPlayer;
		
		switch (tech) //The order of these is important
		{
		case 52:
			TS1I1(planet);
			break;
		case 53:
			TS1I2();
			break;
		case 54:
			TS1I3();
			break;
		case 55:
			TS1I4();
			break;
		case 56:
			TS1I5();
			break;
		case 57:
			TS2I1();
			break;
		case 58:
			TS2I2();
			break;
		case 59:
			TS2I3();
			break;
		case 60:
			TS2I4();
			break;
		case 61:
			TS2I5();
			break;
		case 62:
			TS3I1();
			break;
		case 63:
			TS3I2();
			break;
		case 64:
			TS3I3();
			break;
		case 65:
			TS3I4();
			break;
		case 66:
			TS3I5();
			break;
		case 67:
			TS4I1();
			break;
		case 68:
			TS4I2();
			break;
		case 69:
			TS4I3();
			break;
		case 70:
			TS4I4(planet);
			break;
		case 71:
			TS4I5();
			break;
		default:
			break;
		}
	}

	private void TS1I1()
	{
		improvements.amberPenalty -= systemSIMData.totalSystemAmber * 0.05f;

		int temp = 0;

		if(improvements.IsBuiltOnPlanetType(improvements.system, 52, "Waste") || improvements.IsBuiltOnPlanetType(improvements.system, 52, "Chasm") || improvements.IsBuiltOnPlanetType(improvements.system, 52, "Molten"))
		{
			temp = 1;
		}
		if(improvements.IsBuiltOnPlanetType(improvements.system, 52, "Prairie"))
		{
			temp = 2;
		}

		temp = temp + systemListConstructor.systemList[improvements.system].systemSize;

		improvements.amberPointBonus += temp;

		if(checkValue == false)
		{
			improvements.listOfImprovements[52].improvementMessage = ("System is suffering -" + systemSIMData.totalSystemAmber * 5 + "% Resource production from Amber Penalty, +" + temp + "Amber On System");
		}
	}

	private void TS1I2()
	{
		improvements.improvementSlotsBonus -= 1;

		if(checkValue == false)
		{
			improvements.listOfImprovements[53].improvementMessage = "-1 Improvement Slots on Planets (min 1). +75% Effectiveness on remaining slots.";
		}
	}

	private void TS1I3()
	{
		int temp = 1 * systemSIMData.totalSystemAmber;
		improvements.improvementCostReduction += temp;

		if(checkValue == false)
		{
			improvements.listOfImprovements[54].improvementMessage = ("-" + temp + " Power Cost of Improvements from Amber Production");
		}
	}

	private void TS1I4()
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

		improvements.amberPenalty -= adjacentSystems * 0.05f;

		if(checkValue == false)
		{
			improvements.listOfImprovements[55].improvementMessage = ("-" + adjacentSystems * 0.05f + " Amber Penalty from adjacent Selkies Systems");
		}
	}

	private void T12I5()
	{
		//TODO
	}

	private void TS2I1()
	{
		improvements.tempCount = 0.02f * systemSIMData.totalSystemAmber;
		improvements.expansionPenaltyModifier += improvements.tempCount;
		
		if(checkValue == false)
		{
			improvements.listOfImprovements[56].improvementMessage = ("-" + improvements.tempCount * 100 + "% Expansion Disapproval from Amber Production");
		}
	}

	private void TS2I2()
	{
		float rnd = Random.Range (0.25f, 0.4f);

		improvements.amberProductionBonus -= rnd;
		improvements.amberPenalty -= 0.5f;

		if(checkValue == false)
		{
			improvements.listOfImprovements[57].improvementMessage = "-25-40% Amber production, Amber penalty reduced by 50%";
		}
	}

	private void TS2I3()
	{
		improvements.tempCount = 0.05f * systemSIMData.totalSystemAmber;
		improvements.resourceYieldBonus += improvements.tempCount;
		
		if(checkValue == false)
		{
			improvements.listOfImprovements[58].improvementMessage = ("+" + improvements.tempCount * 100 + "% Secondary Resource Yield from Amber Production");
		}
	}

	private void TS2I4()
	{
		improvements.tempCount = 0.01f * systemSIMData.totalSystemPower;
		improvements.amberProductionBonus += improvements.tempCount;

		if(checkValue == false)
		{
			improvements.listOfImprovements[59].improvementMessage = ("+" + improvements.tempCount + "% Amber Production from Power Output");
		}
	}

	private void TS2I5() //This also needs a value
	{
		//TODO
	}

	private void TS3I1()
	{
		float rnd = Random.Range (0.2f, 0.3f);
		
		improvements.amberProductionBonus -= rnd;
		improvements.amberPenalty -= 0.5f;
		
		if(checkValue == false)
		{
			improvements.listOfImprovements[61].improvementMessage = "-20-30% Amber production, Amber penalty is reduced by 50%";
		}
	}

	private void TS3I2(int planet)
	{
		int temp = systemListConstructor.systemList[improvements.system].planetsInSystem[planet].planetImprovementLevel;

		improvements.growthModifier += temp * 0.01f;
		improvements.maxPopulationBonus += temp * 0.1f;

		if(checkValue == false)
		{
			improvements.listOfImprovements[62].improvementMessage = "+1 * Improvement Level% Growth on Planets, +10 * Improvement Level% Max Population on Planets";
		}
	}

	private void TS3I3()
	{
		improvements.powerPercentBonus += 1f;
		improvements.knowledgePercentBonus -= 0.75f;

		int rnd = Random.Range (0,3);

		if(rnd == 0)
		{
			improvements.knowledgePercentBonus = improvements.knowledgePercentBonus * 2;
		}

		rnd = Random.Range(0, 3);

		if(rnd == 0)
		{
			improvements.powerPercentBonus = improvements.powerPercentBonus * 2;
		}

		if(checkValue == false)
		{
			improvements.listOfImprovements[63].improvementMessage = "+100% Power Production, -75% Knowledge Production, 25% Chance for Knowledge and/or Power Production to double";
		}
	}

	private void TS3I4()
	{
		improvements.amberProductionBonus += 0.5f;

		int rnd = Random.Range (0,1);

		if(rnd == 0)
		{
			improvements.amberProductionBonus = improvements.amberProductionBonus * 2;
		}

		if(checkValue == false)
		{
			improvements.listOfImprovements[64].improvementMessage = "+50% Amber Production, 50% Chance for Amber Production to Double";
		}
	}

	private void TS3I5()
	{
		//TODO
	}

	private void TS4I1()
	{
		improvements.tempCount = 0.01f * systemSIMData.totalSystemAmber;
		improvements.researchCost -= improvements.tempCount;

		if(checkValue == false)
		{
			improvements.listOfImprovements[66].improvementMessage = "-" + improvements.tempCount + "% Research Cost from Amber Production";
		}
	}

	private void TS4I2()
	{
		//TODO
	}

	private void TS4I3()
	{
		improvements.amberProductionBonus += 1f;
		improvements.amberPenalty = 0f;

		int rnd = Random.Range (0, 4);

		if(rnd == 0)
		{
			improvements.amberProductionBonus = 0f;
		}

		if(checkValue == false)
		{
			improvements.listOfImprovements[68].improvementMessage = "+100% Amber Production, 20% Chance for Amber Drought";
		}
	}

	private void TS4I4(int planet)
	{
		if(systemListConstructor.systemList[improvements.system].planetsInSystem[planet].planetType == "Prairie")
		{
			improvements.improvementSlotsBonus += 1;
		}
		if(systemListConstructor.systemList[improvements.system].planetsInSystem[planet].planetType == "Molten")
		{
			improvements.improvementSlotsBonus += 2;
		}
		if(checkValue == false)
		{
			improvements.listOfImprovements[69].improvementMessage = "+1 Improvement Slots on Prairie Type Planets, +2 Improvement Slots on Molten Type Planets";
		}
	}

	private void TS4I5()
	{
		//TODO
	}
}
