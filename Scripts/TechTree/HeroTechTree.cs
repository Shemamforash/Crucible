using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System;

public static class HeroTechTree
{
	public static List<HeroTech> heroTechList = new List<HeroTech>();

	public static void ReadTechFile()
	{
		using(XmlReader reader = XmlReader.Create ("GearList.xml"))
		{
			while(reader.Read ())
			{
				if(reader.Name == "Row")
				{
					HeroTech tech = new HeroTech();

					tech.techName = reader.GetAttribute("A");

					if(tech.techName == "Basic Components")
					{
						tech.isActive = true;
					}

					tech.primaryOffenceRating = Convert.ToInt32 (reader.GetAttribute("B"));
					tech.secondaryOffenceRating = Convert.ToInt32 (reader.GetAttribute("C"));
					tech.collateralRating = Convert.ToInt32 (reader.GetAttribute("D"));
					tech.engineRating = Convert.ToInt32 (reader.GetAttribute("E"));
					tech.armourRating = Convert.ToInt32 (reader.GetAttribute("F"));
					tech.stealthRating = Convert.ToInt32 (reader.GetAttribute("G"));
					tech.logisticsRating = Convert.ToInt32 (reader.GetAttribute("H"));
					tech.heroType = reader.GetAttribute("I");
					tech.techType = reader.GetAttribute("J");
					tech.knowledgeCost = Convert.ToInt32 (reader.GetAttribute("K"));
					tech.prerequisite = reader.GetAttribute("L");

					tech.techDetails = tech.techName + "\nResearch Cost: " + tech.knowledgeCost;

					if(tech.primaryOffenceRating != 0)
					{
						tech.techDetails += "\nPrimary Weapon Power: " + tech.primaryOffenceRating;
					}
					if(tech.secondaryOffenceRating != 0)
					{
						tech.techDetails += "\nInvasion Weapon Power: " + tech.secondaryOffenceRating;
					}
					if(tech.collateralRating != 0)
					{
						tech.techDetails += "\nCollateral Damage: " + tech.collateralRating;
					}
					if(tech.engineRating != 0)
					{
						tech.techDetails += "\nEngine Power: " + tech.engineRating;
					}
					if(tech.armourRating != 0)
					{
						tech.techDetails += "\nArmour rating: " + tech.armourRating;
					}
					if(tech.stealthRating != 0)
					{
						tech.techDetails += "\nInfiltrator cloaks are now level " + tech.stealthRating;
					}
					if(tech.logisticsRating != 0)
					{
						tech.techDetails += "\nTraders now support " + tech.logisticsRating + "Trade Routes";
					}
					if(tech.heroType != "All")
					{
						tech.techDetails += "\nOnly for " + tech.heroType + "s";
					}

					heroTechList.Add (tech);
				}
			}
		}
	}
}

public class HeroTech
{
	public int knowledgeCost = 0, armourRating = 0, primaryOffenceRating = 0, secondaryOffenceRating = 0, engineRating = 0, collateralRating = 0, stealthRating = 0, logisticsRating = 0;
	public string heroType, techType, techName, prerequisite = null, techDetails;
	public bool canPurchase = false, isActive = false;
}
