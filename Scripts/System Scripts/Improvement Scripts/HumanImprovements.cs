using UnityEngine;
using System.Collections;

public class HumanImprovements : MasterScript 
{
	private ImprovementsBasic improvements;
	private bool checkValue;
	private TurnInfo player;

	public void TechSwitch(int tech, int planet, ImprovementsBasic tempImprov, TurnInfo thisPlayer, bool check)
	{
		systemSIMData = systemListConstructor.systemList [tempImprov.system].systemObject.GetComponent<SystemSIMData> ();

		improvements = tempImprov;
		checkValue = check;
		player = thisPlayer;
		
		switch (tech)
		{
		case 12: //Earth's Bounty
			TH1I1(planet);
			break;
		case 13: //Expertise
			TH1I2();
			break;
		case 14: //Summer's Song
			TH1I3(planet);
			break;
		case 15: //Nurture
			TH1I4();
			break;
		case 16: //Horizon
			TH1I5();
			break;
		case 17: //Skyward
			TH2I1(planet);
			break;
		case 18: //Deep Source
			TH2I2(planet);
			break;
		case 19: //Winter's Dirge
			TH2I3(planet);
			break;
		case 20: //Overflow
			TH2I4();
			break;
		case 21: //Rainfall
			TH2I5();
			break;
		case 22: //Creation
			TH3I1();
			break;
		case 23: //Autumn's Air
			TH3I2(planet);
			break;
		case 24: //Orbital
			TH3I3(planet);
			break;
		case 25: //Waters of Life
			TH3I4();
			break;
		case 26: //Enclave
			TH3I5();
			break;
		case 27: //Insight
			TH4I1();
			break;
		case 28: //Spring's Serenade
			TH4I2(planet);
			break;
		case 29: //Ascension
			TH4I3();
			break;
		case 30: //Opulence
			TH4I4();
			break;
		case 31: //Expulsion
			TH4I5();
			break;
		default:
			break;
		}
	}

	private void TH1I1(int planet) //Earth's Bounty
	{
		if(systemListConstructor.systemList[improvements.system].planetsInSystem[planet].planetCategory == "Terran")
		{
			improvements.growthModifier += 0.3f;
		}
		else
		{
			improvements.growthModifier += 0.1f;
		}

		if(checkValue == false)
		{
			improvements.listOfImprovements[12].improvementMessage = "+10% Growth on Planets, or +30% on Terran Planets";
		}
	}

	private void TH1I2() //Expertise
	{
		improvements.growthModifier -= 0.2f;
		improvements.resourceYieldBonus += 0.5f;

		if(checkValue == false)
		{
			improvements.listOfImprovements[13].improvementMessage = "-20% Growth on Planets, +50% Secondary Resource Yield";
		}
	}

	private void TH1I3(int planet) //Summer's Song
	{
		if(systemListConstructor.systemList[improvements.system].planetsInSystem[planet].planetCategory == "Hot")
		{
			improvements.powerPercentBonus += 0.5f;
		}
		if(systemListConstructor.systemList[improvements.system].planetsInSystem[planet].planetCategory == "Cold")
		{
			improvements.powerPercentBonus -= 0.5f;
		}

		if(checkValue == false)
		{
			improvements.listOfImprovements[14].improvementMessage = "+50% Power on Hot Planets, -50% Power on Cold Planets";
		}
	}

	private void TH1I4() //Nurute
	{
		improvements.tempCount = 0.02f * racialTraitScript.ambitionCounter;

		improvements.expansionPenaltyModifier -= improvements.tempCount;

		if(checkValue == false)
		{
			improvements.listOfImprovements[15].improvementMessage = ("-" + improvements.tempCount * 100f + " Expansion Penalty Reduction from Ambition");
		}
	}

	private void TH1I5() //Horizon
	{
		//TODO
	}

	private void TH2I1(int planet) //Skyward
	{
		if(systemListConstructor.systemList[improvements.system].planetsInSystem[planet].planetCategory == player.homePlanetCategory)
		{
			improvements.maxPopulationBonus += 0.4f;
		}
		else
		{
			improvements.maxPopulationBonus += 0.2f;
		}

		if(checkValue == false)
		{
			improvements.listOfImprovements[17].improvementMessage = "+40% Population on Home Category Planets, +20% Population on other Planets";
		}
	}

	private void TH2I2(int planet) //Deep Source
	{
		if(systemListConstructor.systemList[improvements.system].planetsInSystem[planet].rareResourceType == "")
		{
			int rnd = Random.Range(0, 2);

			if(rnd == 0f)
			{
				rnd = Random.Range (0, 3);

				switch(rnd)
				{
				case 0:
					systemListConstructor.systemList[improvements.system].planetsInSystem[planet].rareResourceType = "Antimatter";
					break;
				case 1:
					systemListConstructor.systemList[improvements.system].planetsInSystem[planet].rareResourceType = "Liquid Hydrogen";
					break;
				case 2:
					systemListConstructor.systemList[improvements.system].planetsInSystem[planet].rareResourceType = "Blue Carbon";
					break;
				case 3:
					systemListConstructor.systemList[improvements.system].planetsInSystem[planet].rareResourceType = "Radioisotopes";
					break;
				default:
					break;
				}
			}
		}

		if(checkValue == false)
		{
			improvements.listOfImprovements[18].improvementMessage = ("30% Chance of planet Generating resource");
		}
	}

	private void TH2I3(int planet) //Winter's Dirge
	{		
		if(systemListConstructor.systemList[improvements.system].planetsInSystem[planet].planetCategory == "Hot")
		{
			improvements.knowledgePercentBonus -= 0.5f;
		}
		if(systemListConstructor.systemList[improvements.system].planetsInSystem[planet].planetCategory == "Cold")
		{
			improvements.knowledgePercentBonus += 0.5f;
		}

		if(checkValue == false)
		{	
			improvements.listOfImprovements[19].improvementMessage = "-50% Knowledge on Hot Planets, +50% Knowledge on Cold Planets";
		}
	}

	private void TH2I4() //Overflow
	{
		if(checkValue == false)
		{
			improvements.wealthBonus -= systemSIMData.totalSystemWealth * 0.3f;
			racialTraitScript.ambitionCounter += systemSIMData.totalSystemWealth * 0.3f;
			improvements.listOfImprovements[20].improvementMessage = (systemSIMData.totalSystemWealth * 0.3f + " Wealth converted to " + systemSIMData.totalSystemWealth * 0.3f + " Ambition");
		}
	}

	private void TH2I5() //Rainfall
	{
		//TODO
	}

	private void TH3I1()//Creation
	{
		improvements.improvementSlotsBonus += 1;

		if(checkValue == false)
		{
			racialTraitScript.ambitionCounter -= 1;
			improvements.listOfImprovements[22].improvementMessage = "+1 Improvement Slot on planets, -1 Ambition upkeep on each slot";
		}
	}

	private void TH3I2(int planet) //Autumn's Air
	{
		if(systemListConstructor.systemList[improvements.system].planetsInSystem[planet].planetCategory == "Giant")
		{
			improvements.growthModifier -= 0.5f;
		}
		if(systemListConstructor.systemList[improvements.system].planetsInSystem[planet].planetCategory == "Terran")
		{
			improvements.growthModifier += 0.5f;
		}

		if(checkValue == false)
		{
			improvements.listOfImprovements[23].improvementMessage = "-50% Growth on Giant Planets, +50% Growth on Terran Planets";
		}
	}

	private void TH3I3(int planet) //Orbital
	{
		if(systemListConstructor.systemList[improvements.system].planetsInSystem[planet].planetCategory == "Terran")
		{
			improvements.maxPopulationBonus += 0.5f;
		}
		else
		{
			improvements.maxPopulationBonus += 0.33f;
		}

		if(checkValue == false)
		{
			improvementsBasic.listOfImprovements[24].improvementMessage = "+50% Population on Terran Planets, +33% Population on all other Planets";
		}
	}

	private void TH3I4() //Waters of Life
	{
		for(int i = 0; i < systemListConstructor.systemList[improvements.system].systemSize; ++i)
		{
			if(systemListConstructor.systemList[improvements.system].planetsInSystem[i].planetCategory == "Terran")
			{
				racialTraitScript.ambitionCounter += 2f;
			}
		}

		if(checkValue == false)
		{
			improvementsBasic.listOfImprovements[25].improvementMessage = "+2 Ambition on Terran Planets";
		}
	}

	private void TH3I5() //Enclave
	{
		//TODO
	}

	private void TH4I1() //Insight
	{
		improvements.tempCount = 0.0001f * racialTraitScript.ambitionCounter;
		improvements.researchCostReduction -= improvements.tempCount;

		if(checkValue == false)
		{
			improvementsBasic.listOfImprovements[27].improvementMessage = ("-" + improvements.tempCount * 100 + "% Research Cost from Ambition");
		}
	}

	private void TH4I2(int planet) //Spring's Serenade
	{
		if(systemListConstructor.systemList[improvements.system].planetsInSystem[planet].planetCategory == "Terran")
		{
			improvements.upkeepModifier -= 0.5f;
			improvements.wealthBonus += 1;
		}

		if(checkValue == false)
		{
			improvementsBasic.listOfImprovements[28].improvementMessage = "+1 Wealth on Terran Planets, -50% Upkeep on Terran Planets";
		}
	}

	private void TH4I3() //Ascension
	{
		if(checkValue == false)
		{
			improvements.ambitionPenalty = 0f;
			improvementsBasic.listOfImprovements[29].improvementMessage = "No effect from Ambition Penalties";
		}
	}

	private void TH4I4() //Opulence
	{
		if(racialTraitScript.ambitionCounter >= 75)
		{
			improvementsBasic.knowledgePercentBonus += 0.25f;
			improvementsBasic.powerPercentBonus += 0.25f;
			improvementsBasic.upkeepModifier -= 0.5f;
			improvements.tempCount = 0.25f;
		}
		if(racialTraitScript.ambitionCounter <= 25)
		{
			improvementsBasic.knowledgePercentBonus -= 0.25f;
			improvementsBasic.powerPercentBonus -= 0.25f;
			improvementsBasic.upkeepModifier += 0.5f;
			improvements.tempCount = -0.25f;
		}

		if(checkValue == false)
		{
			improvementsBasic.listOfImprovements[30].improvementMessage = (improvements.tempCount + "% Bonus to Knowledge and Power production, " + improvements.tempCount * 2 + "% Bonus to Upkeep");
		}
	}

	private void TH4I5() //Expulsion
	{
		//TODO
	}
}
