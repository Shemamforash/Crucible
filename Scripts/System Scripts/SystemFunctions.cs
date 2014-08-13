using UnityEngine;
using System.Collections;

public class SystemFunctions : MasterScript
{
	public void CheckImprovement(int system, int planet) //Contains data on the quality of planets and the bonuses they receive
	{
		systemSIMData = systemListConstructor.systemList [system].systemObject.GetComponent<SystemSIMData> ();
		
		if(systemSIMData.improvementNumber == 0)
		{
			systemSIMData.improvementLevel = "Poor";
			systemListConstructor.systemList[system].planetsInSystem[planet].maxPopulation = 25;
			systemSIMData.canImprove = true;
			systemSIMData.improvementCost = systemListConstructor.systemList[system].planetsInSystem[planet].wealthValue / 3;
		}
		if(systemSIMData.improvementNumber == 1)
		{
			systemSIMData.improvementLevel = "Normal";
			systemListConstructor.systemList[system].planetsInSystem[planet].maxPopulation = 50;
			systemSIMData.canImprove = true;
			systemSIMData.improvementCost = (systemListConstructor.systemList[system].planetsInSystem[planet].wealthValue * 2) / 3;
		}
		if(systemSIMData.improvementNumber == 2)
		{
			systemSIMData.improvementLevel = "Good";
			systemListConstructor.systemList[system].planetsInSystem[planet].maxPopulation = 75;
			systemSIMData.canImprove = true;
			systemSIMData.improvementCost = systemListConstructor.systemList[system].planetsInSystem[planet].wealthValue + (systemListConstructor.systemList[system].planetsInSystem[planet].wealthValue / 3);
		}
		if(systemSIMData.improvementNumber == 3)
		{
			systemSIMData.improvementLevel = "Superb";
			systemListConstructor.systemList[system].planetsInSystem[planet].maxPopulation = 100;
			systemSIMData.canImprove = false;
		}
	}

	public float PowerCost(int level, int system, int planet)
	{
		float temp = systemListConstructor.systemList [system].planetsInSystem [planet].planetPower + 
			systemListConstructor.systemList [system].planetsInSystem [planet].planetKnowledge;
		
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
		systemSIMData = systemListConstructor.systemList [system].systemObject.GetComponent<SystemSIMData> ();
		
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
