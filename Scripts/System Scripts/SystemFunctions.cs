using UnityEngine;
using System.Collections;

public class SystemFunctions : MonoBehaviour
{
	private SystemSIMData systemSIMData;

	public void CheckImprovement(int system, int planet) //Contains data on the quality of planets and the bonuses they receive
	{
		systemSIMData = MasterScript.systemListConstructor.systemList [system].systemObject.GetComponent<SystemSIMData> ();
		
		if(systemSIMData.improvementNumber == 0)
		{
			systemSIMData.improvementLevel = "Poor";
			MasterScript.systemListConstructor.systemList[system].planetsInSystem[planet].maxPopulation = 25;
			systemSIMData.canImprove = true;
			systemSIMData.improvementCost = MasterScript.systemListConstructor.systemList[system].planetsInSystem[planet].wealthValue / 3;
		}
		if(systemSIMData.improvementNumber == 1)
		{
			systemSIMData.improvementLevel = "Normal";
			MasterScript.systemListConstructor.systemList[system].planetsInSystem[planet].maxPopulation = 50;
			systemSIMData.canImprove = true;
			systemSIMData.improvementCost = (MasterScript.systemListConstructor.systemList[system].planetsInSystem[planet].wealthValue * 2) / 3;
		}
		if(systemSIMData.improvementNumber == 2)
		{
			systemSIMData.improvementLevel = "Good";
			MasterScript.systemListConstructor.systemList[system].planetsInSystem[planet].maxPopulation = 75;
			systemSIMData.canImprove = true;
			systemSIMData.improvementCost = MasterScript.systemListConstructor.systemList[system].planetsInSystem[planet].wealthValue + (MasterScript.systemListConstructor.systemList[system].planetsInSystem[planet].wealthValue / 3);
		}
		if(systemSIMData.improvementNumber == 3)
		{
			systemSIMData.improvementLevel = "Superb";
			MasterScript.systemListConstructor.systemList[system].planetsInSystem[planet].maxPopulation = 100;
			systemSIMData.canImprove = false;
		}
	}

	public float PowerCost(int level, int system, int planet)
	{
		float temp = MasterScript.systemListConstructor.systemList [system].planetsInSystem [planet].planetPower + 
			MasterScript.systemListConstructor.systemList [system].planetsInSystem [planet].planetKnowledge;
		
		switch(level)
		{
		case 0:
			return temp * 2f;
		case 1:
			return temp * 4;
		case 2:
			return temp * 8f;
		default:
			return -1;
		}
	}

	
	public void CheckUnlockedTier(ImprovementsBasic improvements, int system)
	{
		systemSIMData = MasterScript.systemListConstructor.systemList [system].systemObject.GetComponent<SystemSIMData> ();
		
		systemSIMData.totalSystemSIM += systemSIMData.totalSystemKnowledge + systemSIMData.totalSystemPower;
		
		if(systemSIMData.totalSystemSIM >= 1600.0f && systemSIMData.totalSystemSIM < 3200 && improvements.techTier != 1)
		{
			improvements.techTier = 1;
		}
		if(systemSIMData.totalSystemSIM >= 3200.0f && systemSIMData.totalSystemSIM < 6400 && improvements.techTier != 2)
		{
			improvements.techTier = 2;
		}
		if(systemSIMData.totalSystemSIM >= 6400.0f && improvements.techTier != 3)
		{
			improvements.techTier = 3;
		}
	}
}
