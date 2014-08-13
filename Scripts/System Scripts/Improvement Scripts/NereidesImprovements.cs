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
			TN3I3(planet);
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
			TN4I2();
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
		improvements.improvementCostReduction += racialTraitScript.nereidesStacks;

		if(checkValue == false)
		{
			improvements.listOfImprovements[32].improvementMessage = ("-" + racialTraitScript.nereidesStacks + " Power Cost for Improvements");
		}
	}

	private void TN1I2()
	{
		if(checkValue == false)
		{
			if(racialTraitScript.stacksGeneratedSinceLastUpdate != 0)
			{
				for(int j = 0; j < racialTraitScript.stacksGeneratedSinceLastUpdate; ++j)
				{
					for(int i = 0; i < systemListConstructor.systemList[improvements.system].systemSize; ++i)
					{
						systemSIMData.CheckForSecondaryResourceIncrease(i, player);
					}
				}
			}

			improvements.listOfImprovements[33].improvementMessage = "If Stack of Elation was Created, all planets generate a secondary resource if they are able to do so"; 
		}
	}

	private void TN1I3()
	{
		if(checkValue == false)
		{
			if(racialTraitScript.stacksDissolvedSinceLastUpdate != 0)
			{
				for(int j = 0; j < racialTraitScript.stacksDissolvedSinceLastUpdate; ++j)
				{
					int rnd = Random.Range(0, 9);

					if(rnd == 0)
					{
						player.knowledge += 100f;
					}
				}
			}

			improvements.listOfImprovements[34].improvementMessage = "If Stack Of Elation Dissolved, System Provides Knowledge Reimbursement of Stack Formation Cost";
		}
	}

	private void TN1I4()
	{
		improvements.tempCount = 0.001f * racialTraitScript.nereidesStacks;
		improvements.knowledgePercentBonus += improvements.tempCount;

		if(checkValue == false)
		{
			improvements.listOfImprovements[35].improvementMessage = ("+" + improvements.tempCount * 100f + "% Knowledge from Stacks of Elation");
		}
	}

	private void TN1I5(int planet)
	{
		//TODO
	}

	private void TN2I1(int planet)
	{
		int temp = 1;

		if(improvements.IsBuiltOnPlanetType(improvements.system, 37, "Boreal") || improvements.IsBuiltOnPlanetType(improvements.system, 37, "Tundra") ||
		   improvements.IsBuiltOnPlanetType(improvements.system, 37, "Desolate"))
		{
			temp = 2;
		}

		improvements.wealthBonus -= temp;

		if(checkValue == false)
		{
			improvements.listOfImprovements[37].improvementMessage = ("+" + temp + " Wealth on Planets");
		}
	}

	private void TN2I2()
	{
		//TODO
	}

	private void TN2I3()
	{
		if(checkValue == false)
		{
			for(int i = 0; i < systemSIMData.secondaryResourceGeneratedSinceLastUpdate; ++i)
			{
				int rnd = Random.Range(0, 3);
				
				if(rnd == 0)
				{
					StackOfElation newStack = new StackOfElation();
					newStack.creationTime = Time.time;
					newStack.maxAge = 60f;
					racialTraitScript.elationStacks.Add (newStack);
					++racialTraitScript.stacksGeneratedSinceLastUpdate;
					++racialTraitScript.stackWealthBonus;
				}
			}
			
			improvements.listOfImprovements[38].improvementMessage = "25% Chance to Generate Elation on Secondary Resource Generation";
		}
	}

	private void TN2I4(int planet)
	{
		if(systemListConstructor.systemList[improvements.system].planetsInSystem[planet].planetCategory == "Cold")
		{
			improvements.improvementSlotsBonus += 1;
		}

		if(checkValue == false)
		{
			improvements.listOfImprovements[40].improvementMessage = "+1 Improvement Slot on Cold Type Planets";
		}
	}

	private void TN2I5()
	{
		//TODO
	}

	private void TN3I1()
	{
		improvements.tempCount = 0.001f * systemSIMData.totalSecondaryResourcesGeneratedInSystem;
		improvements.maxPopulationBonus += improvements.tempCount;

		if(checkValue == false)
		{
			improvements.listOfImprovements[42].improvementMessage = ("+" + improvements.tempCount + " Max Population on System from Secondary Resources Generated in this System");
		}
	}

	private void TN3I2(int planet)
	{
		if(systemListConstructor.systemList[improvements.system].planetsInSystem[planet].planetCategory == "Hot")
		{
			improvements.powerPercentBonus += 0.5f;
		}

		if(checkValue == false)
		{
			improvements.listOfImprovements[43].improvementMessage = "+50% Power on Hot Planets, 0% Science on Hot Planets";
		}
	}

	private void TN3I3(int planet)
	{
		float temp = 0.3f;

		for(int i = 0; i < systemListConstructor.systemList[improvements.system].planetsInSystem[planet].improvementSlots; ++i)
		{
			if(systemListConstructor.systemList[improvements.system].planetsInSystem[planet].improvementsBuilt[i] == "")
			{
				temp = 0.1f;
				break;
			}
		}

		improvementsBasic.powerPercentBonus += temp;

		if(checkValue == false)
		{
			improvements.listOfImprovements[44].improvementMessage = "+30% Power on Planets with full Improvement Slots, +10% Power on other Planets";
		}
	}

	private void TN3I4(int planet)
	{
		if(systemListConstructor.systemList[improvements.system].planetsInSystem[planet].planetCategory == "Cold")
		{
			//TODO
		}
	}

	private void TN3I5()
	{
		//TODO
	}

	private void TN4I1()
	{
		if(checkValue == false)
		{
			improvements.wealthBonus += racialTraitScript.stackWealthBonus;
			--racialTraitScript.stackWealthBonus;
			
			improvements.listOfImprovements[46].improvementMessage = ("+" + racialTraitScript.stackWealthBonus + " Bonus Wealth from Stack Generation");
		}
	}

	private void TN4I2()
	{
		float temp = 0.01f;

		if(improvements.IsBuiltOnPlanetType(improvements.system, 48, "Boreal") || improvements.IsBuiltOnPlanetType(improvements.system, 48, "Tundra") ||
		   improvements.IsBuiltOnPlanetType(improvements.system, 48, "Desolate"))
		{
			temp = 0.015f;
		}

		improvements.tempCount = temp * racialTraitScript.nereidesStacks;

		improvements.growthModifier += improvements.tempCount;

		if(checkValue == false)
		{
			improvements.listOfImprovements[48].improvementMessage = "+" + improvements.tempCount + "% Growth on Planets";
		}
	}

	private void TN4I3(int planet)
	{
		if(systemListConstructor.systemList[improvements.system].planetsInSystem[planet].planetCategory == "Cold")
		{
			improvements.resourceYieldBonus += 2.0f;
		}

		if(checkValue == false)
		{
			improvements.listOfImprovements[49].improvementMessage = "+200% Secondary Resource Yield on Cold Planets";
		}
	}

	private void TN4I4()
	{
		systemDefence = systemListConstructor.systemList[improvements.system].systemObject.GetComponent<SystemDefence>();

		if(systemDefence.underInvasion == true)
		{
			improvements.powerPercentBonus += 1.0f;
			improvements.knowledgePercentBonus += 1.0f;
		}

		if(checkValue == false)
		{
			if(systemDefence.underInvasion == true)
			{
				improvements.listOfImprovements[50].improvementMessage = "+100% Resource Yield from Invasion";
			}
		}
	}

	private void TN4I5()
	{
		//TODO
	}
}
