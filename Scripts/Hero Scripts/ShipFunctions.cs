using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class ShipFunctions
{
	public static int stealthValue, primaryWeaponPower, artilleryPower, artilleryCollateral, bombPower, bombCollateral, dropshipPower, 
			   dropshipCollateral, engineValue, armourRating, logisticsRating;
	public static bool infiltratorEngine, soldierPrimary;

	public static void UpdateShips()
	{
		stealthValue = 0;
		primaryWeaponPower = 20;
		soldierPrimary = false;
		artilleryPower = 10;
		artilleryCollateral = 10;
		bombPower = 0;
		bombCollateral = 0;
		dropshipPower = 0;
		dropshipCollateral = 0;
		engineValue = 1;
		infiltratorEngine = false;
		armourRating = 100;
		logisticsRating = 0;

		for(int i = 0; i < HeroTechTree.heroTechList.Count; ++i)
		{
			if(HeroTechTree.heroTechList[i].isActive == true)
			{
				if(HeroTechTree.heroTechList[i].techName == "Miniature Warp Sphere")
				{
					infiltratorEngine = true;
					continue;
				}

				if(HeroTechTree.heroTechList[i].techName == "Full Broadside")
				{
					soldierPrimary = true;
					continue;
				}

				stealthValue += HeroTechTree.heroTechList[i].stealthRating;
				primaryWeaponPower += HeroTechTree.heroTechList[i].primaryOffenceRating;

				if(HeroTechTree.heroTechList[i].heroType == "Soldier")
				{
					artilleryPower += HeroTechTree.heroTechList[i].secondaryOffenceRating;
					artilleryCollateral += HeroTechTree.heroTechList[i].collateralRating;
				}
				if(HeroTechTree.heroTechList[i].heroType == "Infiltrator")
				{
					bombPower += HeroTechTree.heroTechList[i].secondaryOffenceRating;
					bombCollateral += HeroTechTree.heroTechList[i].collateralRating;
				}
				if(HeroTechTree.heroTechList[i].heroType == "Diplomat")
				{
					dropshipPower += HeroTechTree.heroTechList[i].secondaryOffenceRating;
					dropshipCollateral += HeroTechTree.heroTechList[i].collateralRating;
				}

				engineValue += HeroTechTree.heroTechList[i].engineRating;
				armourRating += HeroTechTree.heroTechList[i].armourRating;
				logisticsRating += HeroTechTree.heroTechList[i].logisticsRating;
			}
		}
	}
}
