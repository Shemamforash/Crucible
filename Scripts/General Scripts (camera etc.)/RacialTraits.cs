using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class RacialTraits : MonoBehaviour 
{
	public float ambitionCounter, ambitiongrowthModifier, amber;
	public int stacksGeneratedSinceLastUpdate, stacksDissolvedSinceLastUpdate, stackWealthBonus;
	public UILabel racialLabel;
	public List<StackOfElation> elationStacks = new List<StackOfElation>();
	
	void Start()
	{
		ambitionCounter = 40f;
	}

	public void Purge() //Nereides function to produce elation
	{
		while(MasterScript.playerTurnScript.knowledge >= 100 && MasterScript.playerTurnScript.power >= 100)
		{
			MasterScript.playerTurnScript.knowledge -= 100;
			MasterScript.playerTurnScript.power -= 100;
			StackOfElation newStack = new StackOfElation();
			newStack.creationTime = Time.time;
			newStack.maxAge = 60f;
			elationStacks.Add (newStack);
			++stacksGeneratedSinceLastUpdate;
			++stackWealthBonus;
		}
	}
	
	public float NereidesPowerModifer (TurnInfo thisPlayer) //Returns power modifier based on elation
	{
		if(thisPlayer.playerRace == "Nereides")
		{
			return (elationStacks.Count / 10) + 1;
		}
		
		return 1;
	}
	
	public void RacialBonus(TurnInfo player)
	{
		if(player.playerRace == "Humans")
		{
			if(player.systemsColonisedThisTurn > 0f)
			{
				ambitionCounter += player.systemsColonisedThisTurn * 4f * (60 / MasterScript.systemListConstructor.mapSize);
			}
			if(player.planetsColonisedThisTurn > 0f)
			{
				ambitionCounter += (player.planetsColonisedThisTurn - player.systemsColonisedThisTurn) * 2f * (60 / MasterScript.systemListConstructor.mapSize);
			}
			if(player.systemsColonisedThisTurn == 0 && player.planetsColonisedThisTurn == 0)
			{
				ambitionCounter -= 0.25f;
			}
			if(ambitionCounter < -100f)
			{
				ambitionCounter = -100f;
			}
			if(ambitionCounter > 100f)
			{
				ambitionCounter = 100f;
			}
		}

		if(player.playerRace == "Nereides")
		{
			player.researchCostModifier += elationStacks.Count;

			for(int i = 0; i < elationStacks.Count; ++i)
			{
				if(elationStacks[i].creationTime + elationStacks[i].maxAge < Time.time)
				{
					elationStacks.RemoveAt(i);
					i = -1;
					++stacksDissolvedSinceLastUpdate;
				}
			}
		}
	}
	
	public void IncreaseAmber (int system)
	{
		SystemSIMData systemSIMData = MasterScript.systemListConstructor.systemList [system].systemObject.GetComponent<SystemSIMData> ();
		ImprovementsBasic improvementsBasic = MasterScript.systemListConstructor.systemList [system].systemObject.GetComponent<ImprovementsBasic> ();
		
		systemSIMData.totalSystemAmber = 0f;
		
		if(improvementsBasic.listOfImprovements[28].hasBeenBuilt == true)
		{
			float tempMod = 0.1f;
			
			if(improvementsBasic.IsBuiltOnPlanetType(system, 28, "Molten") == true)
			{
				tempMod = 0.15f;
			}
			
			for(int i = 0; i < MasterScript.systemListConstructor.systemList[system].systemSize; ++i)
			{
				string tempString = MasterScript.systemListConstructor.systemList[system].planetsInSystem[i].planetType;
				
				if(tempString == "Molten" || tempString == "Chasm" || tempString == "Waste")
				{
					systemSIMData.totalSystemAmber += (tempMod * 2f) * improvementsBasic.amberProductionBonus;
				}
				else
				{
					systemSIMData.totalSystemAmber += tempMod * improvementsBasic.amberProductionBonus;
				}
			}
		}
		
		systemSIMData.totalSystemAmber += improvementsBasic.amberPointBonus;
		
		MasterScript.racialTraitScript.amber += systemSIMData.totalSystemAmber;
	}
	
	void Update()
	{
		if(MasterScript.playerTurnScript.playerRace == "Humans")
		{
			racialLabel.text = ("Ambition: " + ((int)ambitionCounter).ToString());
		}
		
		if(MasterScript.playerTurnScript.playerRace == "Nereides")
		{
			racialLabel.text = elationStacks.Count + " stacks";
		}
		
		if(MasterScript.playerTurnScript.playerRace == "Selkies")
		{
			racialLabel.text = Math.Round(amber, 2) + " Amber";
		}
	}
}

public class StackOfElation
{
	public float creationTime, maxAge;
}