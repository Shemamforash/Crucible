using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GalaxyGUI : MasterScript 
{
	public GameObject coloniseButton, snapColoniseButton, planetSelectionWindow, purgeButton;
	private List<GameObject> planetSelectionList = new List<GameObject>();
	private int selectedSystem;
	private string tempRace, knowledgeString, powerString, wealthString, turnNumber;
	public UILabel knowledgeLabel, powerLabel, wealthLabel, raceLabel, turnLabel, diplomacyLabelOne, diplomacyLabelTwo, rareResources;

	void Start()
	{
		foreach(Transform child in planetSelectionWindow.transform)
		{
			planetSelectionList.Add (child.gameObject);
		}
	}

	void Update()
	{
		UpdateVariables();
		UpdateLabels ();
	}

	private void UpdateVariables()
	{
		if(playerTurnScript.playerRace != null && cameraFunctionsScript.selectedSystem != null)
		{
			knowledgeString = ((int)playerTurnScript.knowledge).ToString();
			powerString = ((int)playerTurnScript.power).ToString ();
			wealthString = ((int)playerTurnScript.wealth).ToString ();
			turnNumber = "Year: " + (2200 + (int)(turnInfoScript.turn / 2f)).ToString();
			selectedSystem = RefreshCurrentSystem(cameraFunctionsScript.selectedSystem);

			if(systemListConstructor.systemList[selectedSystem].systemOwnedBy == null)
			{
				NGUITools.SetActive(coloniseButton, true);

				if(playerTurnScript.playerRace == "Nereides")
				{
					float totalPower = 20;
					float totalWealth = 10;
					
					for(int i = 0; i < systemListConstructor.systemList[selectedSystem].systemSize; ++i)
					{
						totalWealth += systemListConstructor.systemList[selectedSystem].planetsInSystem[i].wealthValue;
						totalPower += ((float)systemListConstructor.systemList[selectedSystem].planetsInSystem[i].wealthValue / 3f) * 20f;
					}

					string cost = "Power: " + totalPower + "\nWealth: " + totalWealth;

					snapColoniseButton.GetComponent<UILabel>().text = cost;

					NGUITools.SetActive(snapColoniseButton, true);
				}
			}
		}
	}

	public void SelectRace(string thisRace)
	{
		playerTurnScript.playerRace = thisRace;
		playerTurnScript.StartTurn();
		turnInfoScript.CreateEnemyAI ();

		for(int i = 0; i < turnInfoScript.allPlayers.Count; ++i)
		{
			turnInfoScript.allPlayers[i].RaceStart(turnInfoScript.allPlayers[i].playerRace);
		}

		raceLabel.text = playerTurnScript.playerRace;

		if(playerTurnScript.playerRace == "Nereides")
		{
			NGUITools.SetActive(purgeButton, true);
		}

		turnInfoScript.StartGame ();
	}

	private void UpdateLabels()
	{
		knowledgeLabel.text = knowledgeString;
		powerLabel.text = powerString;
		wealthLabel.text = wealthString;
		turnLabel.text = turnNumber;

		string resources = null;

		if(playerTurnScript.antimatter > 0)
		{
			resources = "ANTIMATTER: " + playerTurnScript.antimatter + "  ";
		}
		if(playerTurnScript.blueCarbon > 0)
		{
			resources = resources + "BLUE CARBON: " + playerTurnScript.blueCarbon + "  ";
		}
		if(playerTurnScript.radioisotopes > 0)
		{
			resources = resources + "RADIOISOTOPES: " + playerTurnScript.radioisotopes + "  ";
		}
		if(playerTurnScript.liquidH2 > 0)
		{
			resources = resources + "LIQUID HYDROGEN: " + playerTurnScript.liquidH2 + "  ";
		}

		rareResources.text = resources;

		string tempString = null;

		for(int i = 0; i < diplomacyScript.relationsList.Count; ++i)
		{
			if(diplomacyScript.relationsList[i].playerOne.playerRace == playerTurnScript.playerRace || diplomacyScript.relationsList[i].playerTwo.playerRace == playerTurnScript.playerRace)
			{
				tempString = tempString + diplomacyScript.relationsList[i].diplomaticState + " | " + diplomacyScript.relationsList[i].stateCounter;

				if(i != diplomacyScript.relationsList.Count - 1)
				{
					tempString = tempString + "\n";
				}
			}
		}

		diplomacyLabelOne.text = tempString;
	}

	public void CheckToColoniseSystem()
	{
		if(playerTurnScript.wealth >= 10)
		{
			playerTurnScript.FindSystem (selectedSystem);
			SelectFirstPlanet();
			NGUITools.SetActive(coloniseButton, false);
			NGUITools.SetActive(snapColoniseButton, false);
		}
	}

	public void SnapColonise()
	{
		float totalPower = 20;
		float totalWealth = 10;

		for(int i = 0; i < systemListConstructor.systemList[selectedSystem].systemSize; ++i)
		{
			totalWealth += systemListConstructor.systemList[selectedSystem].planetsInSystem[i].wealthValue;
			totalPower += ((float)systemListConstructor.systemList[selectedSystem].planetsInSystem[i].wealthValue / 3f) * 20f;
		}

		if(playerTurnScript.wealth >= totalWealth && playerTurnScript.power > totalPower)
		{
			playerTurnScript.FindSystem (selectedSystem);
			playerTurnScript.wealth -= totalWealth;
			playerTurnScript.power -= totalPower;

			for(int i = 0; i < systemListConstructor.systemList[selectedSystem].systemSize; ++i)
			{
				systemListConstructor.systemList[selectedSystem].planetsInSystem[i].planetColonised = true;
				systemListConstructor.systemList [selectedSystem].planetsInSystem [i].expansionPenaltyTimer = Time.time;
			}

			NGUITools.SetActive(coloniseButton, false);
			NGUITools.SetActive(snapColoniseButton, false);
		}
	}

	public void ColonisePlanet()
	{
		int planet = planetSelectionList.IndexOf (UIButton.current.gameObject);

		systemListConstructor.systemList[selectedSystem].planetsInSystem[planet].planetColonised = true;

		systemListConstructor.systemList [selectedSystem].planetsInSystem [planet].expansionPenaltyTimer = Time.time;
		
		++playerTurnScript.planetsColonisedThisTurn;
		
		++playerTurnScript.systemsColonisedThisTurn;
		
		playerTurnScript.systemHasBeenColonised = false;

		NGUITools.SetActive (planetSelectionWindow, false);
	}

	private void SelectFirstPlanet()
	{
		NGUITools.SetActive (planetSelectionWindow, true);

		for(int i = 0; i < planetSelectionList.Count; ++i)
		{
			if(i < systemListConstructor.systemList[selectedSystem].systemSize)
			{
				float planetSIM = systemListConstructor.systemList[selectedSystem].planetsInSystem[i].planetKnowledge + systemListConstructor.systemList[selectedSystem].planetsInSystem[i].planetPower;
				
				string planetInfo = systemListConstructor.systemList[selectedSystem].planetsInSystem[i].planetType + " " + planetSIM.ToString() + " SIM";

				planetSelectionList[i].transform.Find ("Label").gameObject.GetComponent<UILabel>().text = planetInfo.ToUpper();

				NGUITools.SetActive(planetSelectionList[i], true);
			}

			if(i >= systemListConstructor.systemList[selectedSystem].systemSize)
			{
				NGUITools.SetActive(planetSelectionList[i], false);
			}
		}

		planetSelectionWindow.GetComponent<UIScrollView> ().ResetPosition ();
		planetSelectionWindow.GetComponent<UIGrid> ().repositionNow = true;
	}
}
