using UnityEngine;
using System.Collections;

public class GenericImprovements : MonoBehaviour
{
	private ImprovementsBasic improvements;
	private bool checkValue;
	private TurnInfo thisPlayer;
	private HumanImprovements humanImprovements;
	private NereidesImprovements nereidesImprovements;
	private SelkiesImprovements selkiesImprovements;
	private SystemSIMData systemSIMData;

	public void Start()
	{
		humanImprovements = GameObject.Find ("ScriptsContainer").GetComponent<HumanImprovements> ();
		nereidesImprovements = GameObject.Find ("ScriptsContainer").GetComponent<NereidesImprovements> ();
		selkiesImprovements = GameObject.Find ("ScriptsContainer").GetComponent<SelkiesImprovements> ();
	}

	public void TechSwitch(int tech, int planet, ImprovementsBasic tempImprov, TurnInfo player, bool check)
	{
		improvements = tempImprov;

		systemSIMData = MasterScript.systemListConstructor.systemList [tempImprov.system].systemObject.GetComponent<SystemSIMData> ();

		checkValue = check;
		thisPlayer = player;

		improvements.planetToBuildOn.Clear ();
		
		improvements.tempCount = 0f;		
		
		switch (tech)
		{
		case 0: //Amplification
			T1I1();
			break;
		case 1: //Fertile Link
			T1I2();
			break;
		case 2: //Fortune
			T1I3();
			break;
		case 3: //Injection
			T2I1();
			break;
		case 4: //Custodians
			T2I2();
			break;
		case 5: //Isolation
			T2I3(planet);
			break;
		case 6: //Inertia
			T3I1(planet);
			break;
		case 7: //Nostalgia
			T3I2(planet);
			break;
		case 8: //Redundancy
			T3I3();
			break;
		case 9: //Convergence
			T4I1();
			break;
		case 10: //Foundation
			T4I2();
			break;
		case 11: //Perception
			T4I3();
			break;
		default:
			break;
		}
		
		if(thisPlayer.playerRace == "Humans")
		{
			humanImprovements.TechSwitch(tech, planet, tempImprov, thisPlayer, checkValue);
		}
		if(thisPlayer.playerRace == "Nereides")
		{
			nereidesImprovements.TechSwitch(tech, planet, tempImprov, thisPlayer, checkValue);
		}
		if(thisPlayer.playerRace == "Selkies")
		{
			selkiesImprovements.TechSwitch(tech, planet, tempImprov, thisPlayer, checkValue);
		}
	}

	private void T1I1() //Amplification
	{
		for(int i = 0; i < improvements.listOfImprovements.Count; ++i) //For all improvements
		{
			if(improvements.listOfImprovements[i].hasBeenBuilt == true) //If it has been built
			{
				improvements.knowledgePercentBonus += 0.025f; //Increase counter by 2.5%
				improvements.powerPercentBonus += 0.025f;
				improvements.tempCount += 0.025f;
			}
		}

		if(checkValue == false)
		{
			improvements.listOfImprovements[0].improvementMessage = ("+" + improvements.tempCount * 100f + "% Production from Improvements");
		}
	}

	private void T1I2() //Fertile Link
	{
		for(int i = 0; i < MasterScript.systemListConstructor.systemList[improvements.system].permanentConnections.Count; ++i) //For all connections
		{
			int k = MasterScript.RefreshCurrentSystem(MasterScript.systemListConstructor.systemList[improvements.system].permanentConnections[i]);
			
			if(MasterScript.systemListConstructor.systemList[k].systemOwnedBy == thisPlayer.playerRace) //If connected system is allied
			{
				improvements.powerPercentBonus += 0.075f; //Increase counter by 7.5%
				improvements.tempCount += 0.075f;
			}
		}

		if(checkValue == false)
		{
			improvements.listOfImprovements[1].improvementMessage = ("+" + improvements.tempCount * 100f + "% Power from nearby systems");
		}
	}

	private void T1I3() //Fortune
	{
		improvements.resourceYieldBonus += 0.5f;

		if(checkValue == false)
		{
			improvements.listOfImprovements[2].improvementMessage = ("+50% Yield on Secondary Resources");
		}
	}

	private void T2I1() //Injection
	{
		for(int i = 0; i < systemSIMData.secondaryResourceGeneratedSinceLastUpdate; ++i)
		{
			for(int j = 0; j < MasterScript.systemListConstructor.systemList[improvements.system].systemSize; ++j)
			{
				if(MasterScript.systemListConstructor.systemList[improvements.system].planetsInSystem[j].planetPower > MasterScript.systemListConstructor.systemList[improvements.system].planetsInSystem[j].planetKnowledge)
				{
					thisPlayer.power += MasterScript.systemListConstructor.systemList[improvements.system].planetsInSystem[j].planetPower;
				}
				if(MasterScript.systemListConstructor.systemList[improvements.system].planetsInSystem[j].planetPower < MasterScript.systemListConstructor.systemList[improvements.system].planetsInSystem[j].planetKnowledge)
				{
					thisPlayer.knowledge += MasterScript.systemListConstructor.systemList[improvements.system].planetsInSystem[j].planetKnowledge;
				}
				if(MasterScript.systemListConstructor.systemList[improvements.system].planetsInSystem[j].planetPower == MasterScript.systemListConstructor.systemList[improvements.system].planetsInSystem[j].planetKnowledge)
				{
					thisPlayer.power += MasterScript.systemListConstructor.systemList[improvements.system].planetsInSystem[j].planetPower / 2f;
					thisPlayer.knowledge += MasterScript.systemListConstructor.systemList[improvements.system].planetsInSystem[j].planetKnowledge / 2f;
				}
			}
		}

		if(checkValue == false)
		{
			improvements.listOfImprovements[3].improvementMessage = ("Resource Bonus on Secondary Resource Generation");
		}
	}

	private void T2I2() //Custodians
	{
		int tempCount = CheckNumberOfPlanetsWithImprovement(4, thisPlayer, improvements);
		
		improvements.powerPercentBonus += (tempCount * 0.005f);

		if(checkValue == false)
		{
			improvements.listOfImprovements[4].improvementMessage = ("+" + improvements.tempCount * 0.5f + "% Power from other Systems with this Improvement");
		}
	}

	private void T2I3(int planet) //Isolation
	{
		float temp = (MasterScript.systemListConstructor.systemList[improvements.system].planetsInSystem[planet].planetImprovementLevel + 1) * 10f;

		improvements.maxPopulationBonus += temp;

		if(checkValue == false)
		{
			improvements.listOfImprovements[5].improvementMessage = ("+10% * Planet Quality Max Population on Planets");
		}
	}

	private void T3I1(int planet) //Inertia
	{
		improvements.tempCount = 0f;

		for(int i = 0; i < MasterScript.systemListConstructor.systemList[improvements.system].planetsInSystem[planet].improvementsBuilt.Count; ++i)
		{
			if(MasterScript.systemListConstructor.systemList[improvements.system].planetsInSystem[planet].improvementsBuilt[i] == "")
			{
				improvements.knowledgePercentBonus += 0.05f;
			}
		}

		if(checkValue == false)
		{
			improvements.listOfImprovements[6].improvementMessage = ("+5% Knowledge per unused improvement slot on planets");
		}
	}

	private void T3I2(int planet) //Nostalgia
	{
		if(MasterScript.systemListConstructor.systemList[improvements.system].planetsInSystem[planet].planetType == thisPlayer.homePlanetType)
		{
			improvements.powerPercentBonus += 1f;
			improvements.knowledgePercentBonus += 1f;
		}

		if(checkValue == false)
		{
			improvements.listOfImprovements[7].improvementMessage = ("+100% Power and Knowledge on home type planets");
		}
	}

	private void T3I3() //Redundancy
	{
		//TODO
	}

	private void T4I1() //Convergence
	{
		improvements.tempCount = 0.0025f * (systemSIMData.totalSystemKnowledge + systemSIMData.totalSystemPower);
		
		improvements.knowledgePercentBonus += improvements.tempCount;
		improvements.upkeepModifier += improvements.tempCount;

		if(checkValue == false)
		{
			improvements.listOfImprovements[9].improvementMessage = ("+" + (improvements.tempCount * 100) + "% Knowledge from KP output On System");
		}
	}

	private void T4I2() //Foundation
	{
		improvements.upkeepModifier -= 0.5f;

		if(checkValue == false)
		{
			improvements.listOfImprovements[10].improvementMessage = "-50% Upkeep on Improvements";
		}
	}

	private void T4I3() //Perception
	{
		//TODO
	}

	private int CheckNumberOfPlanetsWithImprovement(int improvementNo, TurnInfo thisPlayer, ImprovementsBasic improvements)
	{
		int currentPlanets = 0;

		for(int i = 0; i < MasterScript.systemListConstructor.mapSize; ++i)
		{
			if(MasterScript.systemListConstructor.systemList[i].systemOwnedBy == null || MasterScript.systemListConstructor.systemList[i].systemOwnedBy == thisPlayer.playerRace)
			{
				continue;
			}
			
			if(improvements.listOfImprovements[improvementNo].hasBeenBuilt == true)
			{
				++currentPlanets;
			}
		}

		return currentPlanets;
	}
}
