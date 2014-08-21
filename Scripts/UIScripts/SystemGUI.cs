using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SystemGUI : MasterScript 
{ 
	private SystemScrollviews systemScrollviews;
	public UILabel systemPower, systemKnowledge, systemWealth, systemName, systemSize, planetHeaderName, planetHeaderClass, planetHeaderOwner, improveButtonPower, improveButtonWealth, rareResourceLabel, population, 
		growth, defence, offence;
	
	public int selectedSystem, selectedPlanet, numberOfHeroes;
	private List<PlanetElementDetails> planetElementList = new List<PlanetElementDetails>();
	public GameObject playerSystemInfoScreen, heroChooseScreen, planetInfoWindow, improveButton;
	public GameObject[] planetObjects = new GameObject[6];

	void Start()
	{
		systemScrollviews = gameObject.GetComponent<SystemScrollviews> ();
		SetUpPlanets ();
		selectedPlanet = -1;
	}

	private void CheckActiveElements()
	{
		if(playerSystemInfoScreen.activeInHierarchy == false)
		{
			NGUITools.SetActive(playerSystemInfoScreen, true);
		}
		
		if(selectedPlanet == -1)
		{
			NGUITools.SetActive(planetInfoWindow, false);
		}

		else if(selectedPlanet != -1)
		{
			if(systemListConstructor.systemList[selectedSystem].planetsInSystem[selectedPlanet].planetColonised == true)
			{
				NGUITools.SetActive(planetInfoWindow, true);
			}
			else
			{
				NGUITools.SetActive(planetInfoWindow, false);
			}
		}
	}

	private void CheckSystemSize() //Used to enable or disable parts of the UI depending on the system size
	{
		for(int i = 0; i < 6; i++) //For all possible planets
		{	
			if(i < systemListConstructor.systemList[selectedSystem].systemSize) //If the planet is in the system
			{
				NGUITools.SetActive(planetObjects[i], true); //Enable it's relevant UI part on the far left hand side
				
				if(systemListConstructor.systemList[selectedSystem].planetsInSystem[i].planetColonised == true) //If it is also colonised
				{
					UpdateColonisedPlanetDetails(i, selectedSystem); //Update the relevant details
				}
				else //If it's not
				{
					UpdateUncolonisedPlanetDetails(i); //Update those details
				}
			}
			
			if(i >= systemListConstructor.systemList[selectedSystem].systemSize) //If the possible planet is not within the number of planets in the system
			{
				NGUITools.SetActive(planetObjects[i], false); //Disable the corresponding ui object
			}
		}
	}

	private void Update()
	{
		if(cameraFunctionsScript.openMenu == true) //If the menu should be open
		{
			NGUITools.SetActive(systemPopup.overlayContainer, false); //Disable all the overlays

			if(playerTurnScript.tempObject != null) //If there is a selected system
			{
				selectedSystem = RefreshCurrentSystem(cameraFunctionsScript.selectedSystem); //Get references to the required scripts
				systemSIMData = playerTurnScript.tempObject.GetComponent<SystemSIMData>();
				improvementsBasic = playerTurnScript.tempObject.GetComponent<ImprovementsBasic>();
			}

			CheckActiveElements();
			UpdateOverview();
			CheckSystemSize();
		}

		if(cameraFunctionsScript.openMenu == false) //If the menu should be closed
		{
			NGUITools.SetActive(systemPopup.overlayContainer, true); //Enable the overlays

			if(playerSystemInfoScreen.activeInHierarchy == true) //If there are any menu components active
			{
				NGUITools.SetActive(playerSystemInfoScreen, false); //Disable them
				selectedPlanet = -1; //Reset the selected planet
				systemScrollviews.selectedPlanet = -1;
			}
		}
	}

	private string ReturnSystemSize(int size)
	{
		string temp = null;

		switch(size)
		{
		case 1:
			temp = "Tiny";
			break;
		case 2:
			temp = "Small";
			break;
		case 3:
			temp = "Medium";
			break;
		case 4:
			temp = "Large";
			break;
		case 5:
			temp = "Very Large";
			break;
		case 6:
			temp = "Massive";
			break;
		default:
			break;
		}

		int tempInt = 0;
		
		for(int i = 0; i < size; ++i)
		{
			if(systemListConstructor.systemList[selectedSystem].planetsInSystem[i].planetColonised == true)
			{
				++tempInt;
			}
		}

		temp = temp + " (" + tempInt + "/" + size + ")";

		return temp;
	}

	private void UpdateOverview()
	{
		if(cameraFunctionsScript.selectedSystem != null)
		{
			selectedSystem = cameraFunctionsScript.selectedSystemNumber;

			systemName.text = systemListConstructor.systemList[selectedSystem].systemName.ToUpper();

			systemSize.text = ReturnSystemSize(systemListConstructor.systemList[selectedSystem].systemSize).ToUpper();

			systemSIMData = systemListConstructor.systemList[selectedSystem].systemObject.GetComponent<SystemSIMData>();
			
			if(selectedPlanet != -1)
			{
				systemSIMData.CheckPlanetValues(selectedPlanet, "None");

				string headerTemp = systemListConstructor.systemList[selectedSystem].systemName + " " + (selectedPlanet + 1);

				planetHeaderName.text = headerTemp.ToUpper();
				planetHeaderClass.text = systemListConstructor.systemList[selectedSystem].planetsInSystem[selectedPlanet].planetCategory.ToUpper();
				planetHeaderOwner.text = systemListConstructor.systemList[selectedSystem].systemOwnedBy;
			}
			
			if(cameraFunctionsScript.openMenu == true)
			{
				systemPower.text = Math.Round (systemSIMData.totalSystemPower, 1).ToString();
				systemKnowledge.text = Math.Round (systemSIMData.totalSystemKnowledge, 1).ToString();  
				systemWealth.text = Math.Round (systemSIMData.totalSystemWealth, 1).ToString();
			}
		}
	}

	public void PositionGrid(GameObject grid, int size)
	{
		float gridWidth = (size * grid.GetComponent<UIGrid>().cellWidth) / 2 - (grid.GetComponent<UIGrid>().cellWidth/2);
		
		grid.transform.localPosition = new Vector3(playerSystemInfoScreen.transform.localPosition.x - gridWidth, 
		                                                     grid.transform.localPosition.y, grid.transform.localPosition.z);
		
		grid.GetComponent<UIGrid>().repositionNow = true;
	}

	public void HireHero()
	{
		NGUITools.SetActive (heroChooseScreen, true);
	}

	public void ChooseHeroSpecialisation()
	{
		string heroToHire = UIButton.current.transform.parent.gameObject.name;
		turnInfoScript.CheckIfCanHire(playerTurnScript, heroToHire);
		NGUITools.SetActive (heroChooseScreen, false);
	}

	public void PlanetInterfaceClick()
	{
		for(int i = 0; i < 6; ++i)
		{
			if(planetObjects[i] == UIButton.current.gameObject)
			{
				if(systemListConstructor.systemList[selectedSystem].planetsInSystem[i].planetColonised == true)
				{
					NGUITools.SetActive(systemScrollviews.improvementsWindow, false);
					planetObjects[i].GetComponent<UIButton>().enabled = false;
					planetObjects[i].GetComponent<UISprite>().spriteName = "Button Hover (Orange)";
				}

				selectedPlanet = i;
				continue;
			}
			else
			{
				planetObjects[i].GetComponent<UIButton>().enabled = true;
				planetObjects[i].GetComponent<UISprite>().spriteName = "Button Click";
			}
		}

		if(systemListConstructor.systemList[selectedSystem].systemOwnedBy == playerTurnScript.playerRace)
		{
			if(systemListConstructor.systemList[selectedSystem].planetsInSystem[selectedPlanet].planetColonised == true)
			{
				systemScrollviews.selectedPlanet = selectedPlanet;
				systemScrollviews.selectedSlot = -1;
			}

			if(playerTurnScript.wealth >= systemListConstructor.systemList[selectedSystem].planetsInSystem[selectedPlanet].wealthValue)
			{
				if(systemListConstructor.systemList[selectedSystem].planetsInSystem[selectedPlanet].planetColonised == false)
				{
					systemListConstructor.systemList[selectedSystem].planetsInSystem[selectedPlanet].planetColonised = true;
					systemListConstructor.systemList [selectedSystem].planetsInSystem [selectedPlanet].expansionPenaltyTimer = Time.time;
					++playerTurnScript.planetsColonisedThisTurn;
					systemSIMData.CheckPlanetValues(selectedPlanet, "None");
					playerTurnScript.wealth -= systemListConstructor.systemList[selectedSystem].planetsInSystem[selectedPlanet].wealthValue;
				}
			}
		}
	}
	
	public void ImprovePlanet()
	{
		systemSIMData.improvementNumber = systemListConstructor.systemList[selectedSystem].planetsInSystem[selectedPlanet].planetImprovementLevel;

		if(systemSIMData.improvementNumber < 3)
		{
			systemFunctions.CheckImprovement(selectedSystem, selectedPlanet);

			float powerImprovementCost = systemFunctions.PowerCost(systemSIMData.improvementNumber, selectedSystem, selectedPlanet);

			if(playerTurnScript.power >= powerImprovementCost && playerTurnScript.wealth >= systemSIMData.improvementCost)
			{
				playerTurnScript.power -= powerImprovementCost;
				playerTurnScript.wealth -= systemSIMData.improvementCost;
				++systemListConstructor.systemList[selectedSystem].planetsInSystem[selectedPlanet].planetImprovementLevel;
			}
		}
	}

	private void UpdateColonisedPlanetDetails(int i, int system)
	{
		planetElementList [i].name.text = systemListConstructor.systemList [selectedSystem].planetsInSystem [i].planetType.ToUpper();
		
		planetElementList [i].quality.text = systemSIMData.allPlanetsInfo [i].generalInfo.ToUpper();
		planetElementList [i].powerOP.text = systemSIMData.allPlanetsInfo [i].powerOutput;
		planetElementList [i].knowledgeOP.text = systemSIMData.allPlanetsInfo [i].knowledgeOutput;
		planetElementList [i].population.text = systemSIMData.allPlanetsInfo [i].population;
		
		if(i == selectedPlanet)
		{
			NGUITools.SetActive (planetElementList [i].knowledge.gameObject, true);
			NGUITools.SetActive (planetElementList [i].power.gameObject, true);
			NGUITools.SetActive (planetElementList [i].population.gameObject, true);
			NGUITools.SetActive (planetElementList [i].quality.gameObject, true);
			NGUITools.SetActive (planetElementList [i].name.gameObject, true);
			NGUITools.SetActive (planetElementList [i].uncolonised.gameObject, false);

			float maxPopPlanet = systemListConstructor.systemList[system].sysMaxPopulationModifier + systemListConstructor.systemList[system].planetsInSystem[i].maxPopulationModifier
				+ systemListConstructor.systemList[system].planetsInSystem[i].maxPopulation;

			population.text = Math.Round (systemListConstructor.systemList [system].planetsInSystem [i].planetPopulation, 1).ToString () + "%/" + 
				Math.Round (maxPopPlanet, 1).ToString () + "% COLONISED";
			growth.text = Math.Round (systemSIMData.populationToAdd, 1).ToString() + "% GROWTH";

			defence.text = Math.Round (systemListConstructor.systemList [system].planetsInSystem [i].planetCurrentDefence, 0).ToString () + "/" +
							Math.Round (systemListConstructor.systemList [system].planetsInSystem [i].planetMaxDefence, 0).ToString () + " DEFENCE (+" + 
							Math.Round (systemListConstructor.systemList [system].planetsInSystem [i].defenceRegeneration, 1).ToString () + "/S)";

			string tempRes = systemListConstructor.systemList[system].planetsInSystem[i].rareResourceType;

			if(tempRes == null)
			{
				tempRes = "NO RESOURCE";
			}

			rareResourceLabel.text = tempRes;
							
			if(systemSIMData.improvementNumber < 3)
			{
				if(improveButton.GetComponent<UIButton>().isEnabled == false)
				{
					improveButton.GetComponent<UIButton>().isEnabled = true;
				}

				float temp = systemFunctions.PowerCost(systemSIMData.improvementNumber, selectedSystem, i);
				improveButtonPower.text = Math.Round (temp, 1).ToString();
				improveButtonWealth.text = systemSIMData.improvementCost.ToString();
			}
			
			if(systemSIMData.improvementNumber == 3 && improveButton.GetComponent<UIButton>().isEnabled == true)
			{
				improveButtonPower.text = "-";
				improveButtonWealth.text = "-";
				improveButton.GetComponent<UIButton>().isEnabled = false;
			}
		}
	}

	private void UpdateUncolonisedPlanetDetails(int i)
	{
		NGUITools.SetActive (planetElementList [i].knowledge.gameObject, false);
		NGUITools.SetActive (planetElementList [i].power.gameObject, false);
		NGUITools.SetActive (planetElementList [i].population.gameObject, false);
		NGUITools.SetActive (planetElementList [i].quality.gameObject, false);
		NGUITools.SetActive (planetElementList [i].name.gameObject, false);

		NGUITools.SetActive (planetElementList [i].uncolonised.gameObject, true);

		planetElementList[i].uncolonised.text = systemListConstructor.systemList[selectedSystem].planetsInSystem[i].planetType.ToUpper() + " (UNCOLONISED)";
		planetElementList [i].knowledgeOP.text = "";
		planetElementList [i].powerOP.text = "";
		planetElementList [i].population.text = "";
	}

	private void SabotageButton()
	{
		for(int i = 0; i < planetElementList.Count; ++i)
		{
			//TODO
		}
	}

	private void SetUpPlanets()
	{
		for(int i = 0; i < planetObjects.Length; ++i)
		{
			PlanetElementDetails temp = new PlanetElementDetails();

			temp.knowledge = planetObjects[i].transform.Find ("Knowledge").gameObject.GetComponent<UISprite>();
			temp.power = planetObjects[i].transform.Find ("Power").gameObject.GetComponent<UISprite>();
			temp.knowledgeOP = planetObjects[i].transform.Find ("Knowledge").transform.Find ("Knowledge Output").gameObject.GetComponent<UILabel>();
			temp.powerOP = planetObjects[i].transform.Find ("Power").transform.Find ("Power Output").gameObject.GetComponent<UILabel>();
			temp.quality = planetObjects[i].transform.Find ("Quality").gameObject.GetComponent<UILabel>();
			temp.name = planetObjects[i].transform.Find ("Name").gameObject.GetComponent<UILabel>();
			temp.population = planetObjects[i].transform.Find ("Population").gameObject.GetComponent<UILabel>(); 
			temp.uncolonised = planetObjects[i].transform.Find ("Uncolonised Label").gameObject.GetComponent<UILabel>();

			planetElementList.Add (temp);
		}
	}

	private class PlanetElementDetails
	{
		public UILabel knowledgeOP, powerOP, quality, name, population, uncolonised;
		public UISprite power, knowledge;
	}
}