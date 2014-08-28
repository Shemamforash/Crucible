using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour 
{
	public Texture selkies, nereides, humans;
	private List<string> raceList = new List<string>();
	private List<MainMenuScrollView> objectList = new List<MainMenuScrollView> ();
	private List<string> chosenRaces = new List<string>();

	void Awake () 
	{
		string[] strArr = new string[3] {"Player 1", "Player 2", "Player 3"};

		raceList.Add ("None");
		raceList.Add ("Humans");
		raceList.Add ("Selkies");
		raceList.Add ("Nereides");
	
		for(int i = 0; i < 3; ++i)
		{
			MainMenuScrollView scrollObject = new MainMenuScrollView();

			scrollObject.raceChooseList = GameObject.Find (strArr[i]).GetComponent<UIPopupList>();
			scrollObject.raceIcon = scrollObject.raceChooseList.transform.FindChild("Race Icon").GetComponent<UISprite>();
			scrollObject.currentRace = scrollObject.raceChooseList.transform.FindChild("Selected Race Label").GetComponent<UILabel>();
			scrollObject.currentRace.text = raceList[0];

			objectList.Add (scrollObject);
		}

		ChangeOptions ();
	}

	public void FindCurrentObject()
	{
		int selectedScroll = -1;

		chosenRaces.Clear ();

		for(int i = 0; i < 3; ++i)
		{
			if(objectList[i].currentRace.text != null && objectList[i].currentRace.text != "None")
			{
				chosenRaces.Add (objectList[i].currentRace.text);
			}
			if(UIPopupList.current.gameObject == objectList[i].raceChooseList.gameObject)
			{
				selectedScroll = i;
			}
		}
		
		ChangeOptions();
		ShowSymbol(selectedScroll);
	}

	void ChangeOptions()
	{
		for(int i = 0; i < 3; ++i)
		{
			objectList[i].raceChooseList.items.Clear ();

			for(int j = 0; j < raceList.Count; ++j)
			{
				if(chosenRaces.Contains (raceList[j]) == false)
				{
					objectList[i].raceChooseList.items.Add(raceList[j]);
				}
			}
		}
	}

	void ShowSymbol(int i)
	{
		if(objectList[i].currentRace.text != null)
		{
			switch(objectList[i].currentRace.text)
			{
			case "Humans":
				//TODO
				break;
			case "Selkies":
				objectList[i].raceIcon.spriteName = "Selkies Race Symbol";
				break;
			case "Nereides":
				objectList[i].raceIcon.spriteName = "Nereides Racial Symbol (Flat)";
				break;
			default:
				break;
			}
		}
	}

	public void SetGameInfo()
	{
		UILabel size = GameObject.Find ("Size Label").GetComponent<UILabel> ();

		if(size.text != "-" && objectList[0].currentRace.text != "None" && objectList[1].currentRace.text != "None")
		{
			PlayerPrefs.DeleteAll ();

			switch(size.text)
			{
			case "Very Small (15 Systems)":
				PlayerPrefs.SetInt ("Map Size", 15);
				break;
			case "Small (30 Systems)":
				PlayerPrefs.SetInt ("Map Size", 30);
				break;
			case "Medium (45 Systems)":
				PlayerPrefs.SetInt ("Map Size", 45);
				break;
			case "Large (60 Systems)":
				PlayerPrefs.SetInt ("Map Size", 60);
				break;
			case "Very Large (75 Systems)":
				PlayerPrefs.SetInt ("Map Size", 75);
				break;
			case "Massive (90 Systems)":
				PlayerPrefs.SetInt ("Map Size", 90);
				break;
			case "Max Star Debug (180 Systems)":
				PlayerPrefs.SetInt ("Map Size", 180);
				break;
			default:
				break;
			}

			PlayerPrefs.SetString ("Player Race", objectList [0].currentRace.text);
			PlayerPrefs.SetString ("AI One", objectList [1].currentRace.text);
			PlayerPrefs.SetString ("AI Two", objectList [2].currentRace.text);

			for(int i = 0; i < objectList.Count; ++i)
			{
				switch(objectList[i].currentRace.text)
				{
				case "Humans":
					objectList[i].currentPlanet = "Midgard";
					break;
				case "Selkies":
					objectList[i].currentPlanet = "Samael";
					break;
				case "Nereides":
					objectList[i].currentPlanet = "Nephthys";
					break;
				default:
					objectList[i].currentPlanet = "None";
					break;
				}
			}

			PlayerPrefs.SetString("Planet One", objectList[0].currentPlanet);
			PlayerPrefs.SetString("Planet Two", objectList[1].currentPlanet);
			PlayerPrefs.SetString("Planet Three", objectList[2].currentPlanet);


			Application.LoadLevel("Crucible");
		}
	}
}

public class MainMenuScrollView
{
	public UIPopupList raceChooseList;
	public UISprite raceIcon;
	public UILabel currentRace;
	public string currentPlanet;
}
