using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GalaxyGUI : MonoBehaviour 
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
		if(MasterScript.playerTurnScript.playerRace != null && MasterScript.cameraFunctionsScript.selectedSystem != null)
		{
			knowledgeString = ((int)MasterScript.playerTurnScript.knowledge).ToString();
			powerString = ((int)MasterScript.playerTurnScript.power).ToString ();
			wealthString = ((int)MasterScript.playerTurnScript.wealth).ToString ();
			turnNumber = "Year: " + (2200 + (int)(MasterScript.turnInfoScript.turn / 2f)).ToString();
			selectedSystem = MasterScript.RefreshCurrentSystem(MasterScript.cameraFunctionsScript.selectedSystem);

			if(MasterScript.systemListConstructor.systemList[selectedSystem].systemOwnedBy == null)
			{
				NGUITools.SetActive(coloniseButton, true);

				if(MasterScript.playerTurnScript.playerRace == "Nereides")
				{
					float totalPower = 20;
					float totalWealth = 10;
					
					for(int i = 0; i < MasterScript.systemListConstructor.systemList[selectedSystem].systemSize; ++i)
					{
						totalWealth += MasterScript.systemListConstructor.systemList[selectedSystem].planetsInSystem[i].wealthValue;
						totalPower += ((float)MasterScript.systemListConstructor.systemList[selectedSystem].planetsInSystem[i].wealthValue / 3f) * 20f;
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
		MasterScript.playerTurnScript.playerRace = thisRace;
		MasterScript.playerTurnScript.StartTurn();
		MasterScript.turnInfoScript.CreateEnemyAI ();

		for(int i = 0; i < MasterScript.turnInfoScript.allPlayers.Count; ++i)
		{
			MasterScript.turnInfoScript.allPlayers[i].RaceStart(MasterScript.turnInfoScript.allPlayers[i].playerRace);
		}

		raceLabel.text = MasterScript.playerTurnScript.playerRace;

		if(MasterScript.playerTurnScript.playerRace == "Nereides")
		{
			NGUITools.SetActive(purgeButton, true);
		}

		MasterScript.turnInfoScript.StartGame ();
	}

	private void UpdateLabels()
	{
		knowledgeLabel.text = knowledgeString;
		powerLabel.text = powerString;
		wealthLabel.text = wealthString;
		turnLabel.text = turnNumber;

		string resources = null;

		if(MasterScript.playerTurnScript.antimatter > 0)
		{
			resources = "ANTIMATTER: " + MasterScript.playerTurnScript.antimatter + "  ";
		}
		if(MasterScript.playerTurnScript.blueCarbon > 0)
		{
			resources = resources + "BLUE CARBON: " + MasterScript.playerTurnScript.blueCarbon + "  ";
		}
		if(MasterScript.playerTurnScript.radioisotopes > 0)
		{
			resources = resources + "RADIOISOTOPES: " + MasterScript.playerTurnScript.radioisotopes + "  ";
		}
		if(MasterScript.playerTurnScript.liquidH2 > 0)
		{
			resources = resources + "LIQUID HYDROGEN: " + MasterScript.playerTurnScript.liquidH2 + "  ";
		}

		rareResources.text = resources;

		string tempString = null;

		for(int i = 0; i < MasterScript.diplomacyScript.relationsList.Count; ++i)
		{
			if(MasterScript.diplomacyScript.relationsList[i].playerOne.playerRace == MasterScript.playerTurnScript.playerRace 
			   || MasterScript.diplomacyScript.relationsList[i].playerTwo.playerRace == MasterScript.playerTurnScript.playerRace)
			{
				tempString = tempString + MasterScript.diplomacyScript.relationsList[i].diplomaticState + " | " + MasterScript.diplomacyScript.relationsList[i].stateCounter;

				if(i != MasterScript.diplomacyScript.relationsList.Count - 1)
				{
					tempString = tempString + "\n";
				}
			}
		}

		diplomacyLabelOne.text = tempString;
	}

	public void CheckToColoniseSystem()
	{
		if(MasterScript.playerTurnScript.wealth >= 10)
		{
			MasterScript.playerTurnScript.FindSystem (selectedSystem);
			SelectFirstPlanet();
			NGUITools.SetActive(coloniseButton, false);
			NGUITools.SetActive(snapColoniseButton, false);
		}
	}

	public void SnapColonise()
	{
		float totalPower = 20;
		float totalWealth = 10;

		for(int i = 0; i < MasterScript.systemListConstructor.systemList[selectedSystem].systemSize; ++i)
		{
			totalWealth += MasterScript.systemListConstructor.systemList[selectedSystem].planetsInSystem[i].wealthValue;
			totalPower += ((float)MasterScript.systemListConstructor.systemList[selectedSystem].planetsInSystem[i].wealthValue / 3f) * 20f;
		}

		if(MasterScript.playerTurnScript.wealth >= totalWealth && MasterScript.playerTurnScript.power > totalPower)
		{
			MasterScript.playerTurnScript.FindSystem (selectedSystem);
			MasterScript.playerTurnScript.wealth -= totalWealth;
			MasterScript.playerTurnScript.power -= totalPower;

			for(int i = 0; i < MasterScript.systemListConstructor.systemList[selectedSystem].systemSize; ++i)
			{
				MasterScript.systemListConstructor.systemList[selectedSystem].planetsInSystem[i].planetColonised = true;
				MasterScript.systemListConstructor.systemList [selectedSystem].planetsInSystem [i].expansionPenaltyTimer = Time.time;
			}

			NGUITools.SetActive(coloniseButton, false);
			NGUITools.SetActive(snapColoniseButton, false);
		}
	}

	public void ColonisePlanet()
	{
		int planet = planetSelectionList.IndexOf (UIButton.current.gameObject);

		MasterScript.systemListConstructor.systemList[selectedSystem].planetsInSystem[planet].planetColonised = true;

		MasterScript.systemListConstructor.systemList [selectedSystem].planetsInSystem [planet].expansionPenaltyTimer = Time.time;
		
		++MasterScript.playerTurnScript.planetsColonisedThisTurn;
		
		++MasterScript.playerTurnScript.systemsColonisedThisTurn;
		
		MasterScript.playerTurnScript.systemHasBeenColonised = false;

		NGUITools.SetActive (planetSelectionWindow, false);
	}

	private void SelectFirstPlanet()
	{
		NGUITools.SetActive (planetSelectionWindow, true);

		for(int i = 0; i < planetSelectionList.Count; ++i)
		{
			if(i < MasterScript.systemListConstructor.systemList[selectedSystem].systemSize)
			{
				float planetSIM = MasterScript.systemListConstructor.systemList[selectedSystem].planetsInSystem[i].planetKnowledge + MasterScript.systemListConstructor.systemList[selectedSystem].planetsInSystem[i].planetPower;
				
				string planetInfo = MasterScript.systemListConstructor.systemList[selectedSystem].planetsInSystem[i].planetType + " " + planetSIM.ToString() + " SIM";

				planetSelectionList[i].transform.Find ("Label").gameObject.GetComponent<UILabel>().text = planetInfo.ToUpper();

				NGUITools.SetActive(planetSelectionList[i], true);
			}

			if(i >= MasterScript.systemListConstructor.systemList[selectedSystem].systemSize)
			{
				NGUITools.SetActive(planetSelectionList[i], false);
			}
		}

		planetSelectionWindow.GetComponent<UIScrollView> ().ResetPosition ();
		planetSelectionWindow.GetComponent<UIGrid> ().repositionNow = true;
	}
}
