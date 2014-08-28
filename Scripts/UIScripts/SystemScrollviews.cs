using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System;

public class SystemScrollviews : MonoBehaviour 
{
	public GameObject improvementMessageLabel, availableImprovements, buttonLabel, improvementParent, improvementsWindow, improvementDetails;
	public int techTierToShow, selectedPlanet, selectedSystem, selectedSlot;
	public GameObject[] tabs = new GameObject[4];
	private string improvementText, currentImprovement;
	public UILabel improvementLabel, improvementWealthCost, improvementPowerCost, systemEffects, improvementWealthUpkeep, improvementPowerUpkeep, systemUpkeepPower, systemUpkeepWealth;

	public GameObject[] unbuiltImprovementList = new GameObject[10];
	public GameObject[] improvementsList = new GameObject[8];
	private ImprovementsBasic improvementsBasic;

	void Start()
	{		
		SetUpImprovementLabels ();
		selectedPlanet = -1;
	}

	private void SetUpImprovementLabels()
	{		
		for(int i = 0; i < improvementsList.Length; ++i)
		{
			EventDelegate.Add(improvementsList[i].GetComponent<UIButton>().onClick, OpenImprovementsWindow);
			
			NGUITools.SetActive(improvementsList[i], false); //Default set improvement to false so it won't be shown in scrollview unless needed
		}

		for(int i = 0; i < unbuiltImprovementList.Length; ++i)
		{
			NGUITools.SetActive(unbuiltImprovementList[i], false);

			EventDelegate.Add(unbuiltImprovementList[i].GetComponent<UIButton>().onClick, ShowDetails);
		}
	}

	private void OpenImprovementsWindow()
	{
		if(MasterScript.systemListConstructor.systemList[selectedSystem].systemOwnedBy == MasterScript.playerTurnScript.playerRace)
		{
			NGUITools.SetActive (improvementsWindow, true);
			NGUITools.SetActive (improvementDetails, false);
			currentImprovement = null;
		
			bool reset = false;

			for(int i = 0; i < tabs.Length; ++i)
			{
				if(tabs[i].GetComponent<UISprite>().spriteName == "Button Hover (Orange)")
				{
					UpdateImprovementsWindow (i);
					reset = true;
					break;
				}
			}

			if(reset == false)
			{
				tabs[0].GetComponent<UIButton>().enabled = false;
				tabs[0].GetComponent<UISprite>().spriteName = "Button Hover (Orange)";
				UpdateImprovementsWindow (0);
			}

			selectedSlot = -1;
			
			for(int i = 0; i < improvementsList.Length; ++i)
			{
				if(UIButton.current.gameObject == improvementsList[i])
				{
					selectedSlot = i;
					break;
				}
			}
		}
	}

	private void ShowDetails()
	{
		for(int i = 0; i < unbuiltImprovementList.Length; ++i)
		{
			if(UIButton.current.gameObject == unbuiltImprovementList[i])
			{
				unbuiltImprovementList[i].GetComponent<UIButton>().enabled = false;
				unbuiltImprovementList[i].GetComponent<UISprite>().spriteName = "Button Hover (Orange)";
				currentImprovement = UIButton.current.transform.Find ("Label").gameObject.GetComponent<UILabel>().text;
				continue;
			}

			else
			{
				unbuiltImprovementList[i].GetComponent<UISprite>().spriteName = "Button Click";
				unbuiltImprovementList[i].GetComponent<UIButton>().enabled = true;
			}
		}

		Vector3 tempPos = UIButton.current.transform.localPosition;

		improvementDetails.transform.localPosition = new Vector3 (tempPos.x + 265f, tempPos.y, tempPos.z); 

		for(int i = 0; i < MasterScript.systemListConstructor.basicImprovementsList.Count; ++i)
		{
			if(MasterScript.systemListConstructor.basicImprovementsList[i].name.ToUpper() == UIButton.current.transform.Find ("Label").GetComponent<UILabel>().text)
			{
				improvementLabel.text = MasterScript.systemListConstructor.basicImprovementsList[i].details;

				improvementPowerCost.text = MasterScript.systemListConstructor.basicImprovementsList[i].cost.ToString();
				improvementWealthCost.text = (MasterScript.systemListConstructor.basicImprovementsList[i].cost / 25).ToString();

				improvementPowerUpkeep.text = "-" + MasterScript.systemListConstructor.basicImprovementsList[i].powerUpkeep.ToString();
				improvementWealthUpkeep.text = "-" + MasterScript.systemListConstructor.basicImprovementsList[i].wealthUpkeep.ToString();
			}
		}

		NGUITools.SetActive (improvementDetails, true);
	}

	private void UpdateImprovementsWindow(int level)
	{
		for(int i = 0; i < unbuiltImprovementList.Length; ++i)
		{
			NGUITools.SetActive(unbuiltImprovementList[i], false);
		}

		int j = 0;

		for(int i = 0; i < improvementsBasic.listOfImprovements.Count; ++i)
		{
			if(improvementsBasic.listOfImprovements[i].improvementLevel == level)
			{
				if(improvementsBasic.listOfImprovements[i].improvementCategory == "Generic" || improvementsBasic.listOfImprovements[i].improvementCategory == "Defence" 
				   || improvementsBasic.listOfImprovements[i].improvementCategory == MasterScript.playerTurnScript.playerRace)
				{
					if(improvementsBasic.listOfImprovements[i].hasBeenBuilt == false)
					{
						NGUITools.SetActive(unbuiltImprovementList[j], true);
						
						unbuiltImprovementList[j].transform.Find("Label").GetComponent<UILabel>().text = improvementsBasic.listOfImprovements[i].improvementName.ToUpper();

						++j;
					}
				}
			}
		}

		for(int i = j; j < unbuiltImprovementList.Length; ++j)
		{
			NGUITools.SetActive(unbuiltImprovementList[i], false);
		}
	}

	public void UpdateBuiltImprovements()
	{
		for(int i = 0; i < improvementsList.Length; ++i) //For all improvement slots
		{
			if(i < MasterScript.systemListConstructor.systemList[selectedSystem].planetsInSystem[selectedPlanet].currentImprovementSlots) //If is equal to or less than planets slots
			{
				NGUITools.SetActive(improvementsList[i], true); //Activate

				if(MasterScript.systemListConstructor.systemList[selectedSystem].planetsInSystem[selectedPlanet].improvementsBuilt[i] != null) //If something built
				{
					improvementsList[i].transform.Find ("Name").GetComponent<UILabel>().text = MasterScript.systemListConstructor.systemList[selectedSystem].planetsInSystem[selectedPlanet].improvementsBuilt[i].ToUpper(); //Set text
					improvementsList[i].GetComponent<UIButton>().enabled = false;
					improvementsList[i].GetComponent<UISprite>().spriteName = "Button Normal";
				}

				else //Else say is empty
				{
					improvementsList[i].transform.Find ("Name").GetComponent<UILabel>().text = "Empty";

					if(selectedSlot == i)
					{
						improvementsList[i].GetComponent<UIButton>().enabled = false;
						improvementsList[i].GetComponent<UISprite>().spriteName = "Button Hover (Orange)";
					}

					else
					{
						improvementsList[i].GetComponent<UIButton>().enabled = true;
					}
				}

				continue;
			}

			else //Else deactivate
			{
				NGUITools.SetActive(improvementsList[i], false);
			}
		}
	}

	private void UpdateTabs()
	{
		for(int i = 0; i < tabs.Length; ++i)
		{
			if(i <= improvementsBasic.techTier)
			{
				if(tabs[i].GetComponent<UISprite>().spriteName == "Button Hover (Orange)")
				{
					continue;
				}
				else
				{
					tabs[i].GetComponent<UIButton>().enabled = true;
					tabs[i].GetComponent<UISprite>().spriteName = "Button Normal";
				}
			}
			else
			{
				tabs[i].GetComponent<UIButton>().enabled = false;
				tabs[i].GetComponent<UISprite>().spriteName = "Button Deactivated";
			}
		}
	}
	
	public void TabClick()
	{
		NGUITools.SetActive (improvementDetails, false);
		currentImprovement = null;

		for(int i = 0; i < tabs.Length; ++i)
		{
			if(tabs[i] == UIButton.current.gameObject)
			{
				tabs[i].GetComponent<UIButton>().enabled = false;
				tabs[i].GetComponent<UISprite>().spriteName = "Button Hover (Orange)";
				UpdateImprovementsWindow(i);
			}

			else
			{
				if(i <= improvementsBasic.techTier)
				{
					tabs[i].GetComponent<UIButton>().enabled = true;
					tabs[i].GetComponent<UISprite>().spriteName = "Button Normal";
				}
				else
				{
					tabs[i].GetComponent<UIButton>().enabled = false;
					tabs[i].GetComponent<UISprite>().spriteName = "Button Deactivated";
				}
			}
		}
	}

	private void UpdateUpkeep()
	{
		float upkeepWealth = 0, upkeepPower = 0;
		
		for(int i = 0; i < MasterScript.systemListConstructor.systemList[selectedSystem].planetsInSystem[selectedPlanet].improvementsBuilt.Count; ++i)
		{
			for(int j = 0; j < MasterScript.systemListConstructor.basicImprovementsList.Count; ++j)
			{
				if(improvementsBasic.listOfImprovements[j].hasBeenBuilt == false)
				{
					continue;
				}
				
				if(MasterScript.systemListConstructor.systemList[selectedSystem].planetsInSystem[selectedPlanet].improvementsBuilt[i] == MasterScript.systemListConstructor.basicImprovementsList[j].name)
				{
					upkeepWealth += MasterScript.systemListConstructor.basicImprovementsList[j].wealthUpkeep;
					upkeepPower += MasterScript.systemListConstructor.basicImprovementsList[j].powerUpkeep;
					continue;
				}
			}
		}
		
		systemUpkeepPower.text = upkeepPower.ToString();
		systemUpkeepWealth.text = upkeepWealth.ToString ();
	}
	
	void Update()
	{
		if(MasterScript.systemGUI.selectedSystem != selectedSystem)
		{
			NGUITools.SetActive(improvementsWindow, false);
			selectedSystem = MasterScript.systemGUI.selectedSystem;
			improvementsBasic = MasterScript.systemListConstructor.systemList [selectedSystem].systemObject.GetComponent<ImprovementsBasic> ();
		}

		if(MasterScript.cameraFunctionsScript.openMenu == true)
		{
			if(selectedPlanet != -1)
			{
				if(improvementsWindow.activeInHierarchy == true)
				{
					UpdateTabs();
				}

				UpdateBuiltImprovements();
				UpdateSystemEffects (selectedSystem, selectedPlanet);
				UpdateUpkeep();
			}
		}
		
		if(Input.GetKeyDown("c"))
		{
			NGUITools.SetActive(availableImprovements, false);
		}
	}

	public void UpdateSystemEffects(int system, int planet) //TODO this needs to be planet specific
	{
		improvementsBasic = MasterScript.systemListConstructor.systemList[selectedSystem].systemObject.GetComponent<ImprovementsBasic>();

		string temp = "";

		float knoTemp = (MasterScript.systemListConstructor.systemList[system].sysKnowledgeModifier + MasterScript.systemListConstructor.systemList[system].planetsInSystem[planet].knowledgeModifier) - 1;
		float powTemp = (MasterScript.systemListConstructor.systemList[system].sysPowerModifier + MasterScript.systemListConstructor.systemList[system].planetsInSystem[planet].powerModifier) - 1;
		float growTemp = MasterScript.systemListConstructor.systemList[system].sysGrowthModifier + MasterScript.systemListConstructor.systemList[system].planetsInSystem[planet].growthModifier;
		float popTemp = MasterScript.systemListConstructor.systemList[system].sysMaxPopulationModifier + MasterScript.systemListConstructor.systemList[system].planetsInSystem[planet].maxPopulationModifier;
		float amberPenalty = MasterScript.systemListConstructor.systemList[system].sysAmberPenalty + MasterScript.systemListConstructor.systemList[system].planetsInSystem[planet].amberPenalty;
		float amberProd = (MasterScript.systemListConstructor.systemList[system].sysAmberModifier + MasterScript.systemListConstructor.systemList[system].planetsInSystem[planet].amberModifier) - 1;

		if(knoTemp != 0f)
		{
			if(temp != "")
			{
				temp = temp + "\n+";
			}

			temp = temp + Math.Round(knoTemp * 100, 1) + "% Knowledge from Improvements";
		}
		if(powTemp != 0f)
		{
			if(temp != "")
			{
				temp = temp + "\n+";
			}

			temp = temp + Math.Round(powTemp * 100, 1) + "% Power from Improvements";
		}
		if(growTemp != 0f)
		{
			if(temp != "")
			{
				temp = temp + "\n+";
			}

			temp = temp + Math.Round(growTemp, 2) + "% Growth from Improvements";
		}
		if(popTemp != 0f)
		{
			if(temp != "")
			{
				temp = temp + "\n+";
			}

			temp = temp + Math.Round(popTemp, 1) + "% Population from Improvements";
		}

		int standardSize = MasterScript.systemListConstructor.systemList[selectedSystem].planetsInSystem[selectedPlanet].baseImprovementSlots;

		if(MasterScript.systemListConstructor.systemList[selectedSystem].planetsInSystem[selectedPlanet].currentImprovementSlots > standardSize)
		{
			if(temp != "")
			{
				temp = temp + "\n+";
			}

			temp = temp + (MasterScript.systemListConstructor.systemList[selectedSystem].planetsInSystem[selectedPlanet].currentImprovementSlots - standardSize).ToString() + " Improvement Slots on Planet";
		}
		if(amberPenalty != 1f)
		{
			if(temp != "")
			{
				temp = temp + "\n+";
			}

			temp = temp + Math.Round ((1 - amberPenalty) * 100, 1) + "% Amber Penalty on System";
		}
		if(amberProd != 0)
		{
			if(temp != "")
			{
				temp = temp + "\n+";
			}

			temp = temp + Math.Round (amberProd * 100, 1) + "% Amber Production on System";
		}
		if(improvementsBasic.improvementCostModifier != 0f)
		{
			if(temp != "")
			{
				temp = temp + "\n";
			}

			temp = temp + improvementsBasic.improvementCostModifier + " less Power required for Improvements";
		}
		if(improvementsBasic.researchCost != 0f)
		{
			if(temp != "")
			{
				temp = temp + "\n";
			}

			temp = temp + improvementsBasic.researchCost + " less Knowledge required for Research";
		}

		/*
		amberPointBonus;
		public float tempWealth, tempKnwlUnitBonus, tempPowUnitBonus, tempResearchCostReduction, tempImprovementCostReduction, 
		tempBonusAmbition;

		for(int i = 0; i < improvementsBasic.listOfImprovements.Count; ++i)
		{
			if(improvementsBasic.listOfImprovements[i].hasBeenBuilt == true)
			{
				if(temp == "")
				{
					temp = improvementsBasic.listOfImprovements[i].improvementMessage.ToUpper();
				}

				else
				{
					temp = temp + "\n" + improvementsBasic.listOfImprovements[i].improvementMessage.ToUpper();
				}
			}
		}
		*/

		if(temp == "")
		{
			temp = "NO EFFECTS ON SYSTEM";
		}

		systemEffects.text = temp;
		systemEffects.transform.parent.GetComponent<UISprite> ().height = systemEffects.height + 20;
	}

	public void BuildImprovement()
	{
		NGUITools.SetActive (improvementDetails, false);

		improvementsBasic = MasterScript.systemListConstructor.systemList[selectedSystem].systemObject.GetComponent<ImprovementsBasic>();
		
		for(int i = 0; i < improvementsBasic.listOfImprovements.Count; ++i)
		{
			if(improvementsBasic.listOfImprovements[i].improvementName.ToUpper () == currentImprovement)
			{
				for(int j = 0; j < MasterScript.systemListConstructor.systemList[selectedSystem].planetsInSystem[selectedPlanet].currentImprovementSlots; ++j)
				{
					if(MasterScript.systemListConstructor.systemList[selectedSystem].planetsInSystem[selectedPlanet].improvementsBuilt[j] == null)
					{
						if(improvementsBasic.ImproveSystem(i) == true)
						{
							improvementsBasic.ActiveTechnologies(selectedSystem, MasterScript.playerTurnScript);
							MasterScript.systemListConstructor.systemList[selectedSystem].planetsInSystem[selectedPlanet].improvementsBuilt[j] = improvementsBasic.listOfImprovements[i].improvementName;
							UpdateImprovementsWindow(improvementsBasic.listOfImprovements[i].improvementLevel);
							UpdateBuiltImprovements();
							currentImprovement = null;
							selectedSlot = -1;
							break;
						}
					}
				}
			}
		}

		for(int i = 0; i < unbuiltImprovementList.Length; ++i)
		{
			unbuiltImprovementList[i].GetComponent<UISprite>().spriteName = "Button Normal";
			unbuiltImprovementList[i].GetComponent<UIButton>().enabled = true;
		}

		NGUITools.SetActive(improvementsWindow, false);
	}
	
	private void CheckForTierUnlock()
	{
		for(int i = 0; i < 4; ++i)
		{
			UIButton temp = tabs[i].gameObject.GetComponent<UIButton>();

			if(improvementsBasic.techTier >= i && temp.enabled == false)
			{
				temp.enabled = true;
			}
			if(improvementsBasic.techTier < i && temp.enabled == true)
			{
				temp.enabled = false;
			}
		}
	}
}
