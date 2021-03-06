﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class InvasionGUI : MonoBehaviour
{
	public GameObject invasionScreen, background, summary, tokenContainer, includeHero2, includeHero3, hero2Object, hero3Object;
	public GameObject[] planetList = new GameObject[6];
	public GameObject[] heroInterfaces = new GameObject[3];
	public bool openInvasionMenu = false;
	public List<HeroInvasionLabel> heroInvasionLabels = new List<HeroInvasionLabel>();
	public int system;
	public List<PlanetInvasionLabels> planetInvasionLabels = new List<PlanetInvasionLabels>();

	private TokenManagement management;
	private LoadInvasionScreen loadInvasion;
	private bool createdTokens;
	private HeroScriptParent heroScript;
	private SystemDefence systemDefence;
	
	void Start () 
	{
		management = tokenContainer.GetComponent<TokenManagement> ();
		loadInvasion = tokenContainer.GetComponent<LoadInvasionScreen> ();
		
		NGUITools.SetActive (invasionScreen, true);

		SetUpHeroesAndPlanets ();
	}

	private void SetUpHeroesAndPlanets() //Used to assign the components for the planet and hero interfaces
	{
		for(int i = 0; i < 3; ++i) //Hero interface
		{
			NGUITools.SetActive(heroInterfaces[i], true);

			HeroInvasionLabel temp = new HeroInvasionLabel();
			
			temp.assaultTokenContainer = heroInterfaces[i].transform.Find("Assault Tokens").gameObject;
			temp.auxiliaryTokenContainer = heroInterfaces[i].transform.Find("Auxiliary Tokens").gameObject;
			temp.defenceTokenContainer = heroInterfaces[i].transform.Find("Defence Tokens").gameObject;

			for(int j = 0; j < 6; ++j)
			{
				string newStr = "Token " + (j + 1);
				temp.assaultTokenPositions.Add (temp.assaultTokenContainer.transform.FindChild(newStr).gameObject);
				temp.auxiliaryTokenPositions.Add (temp.auxiliaryTokenContainer.transform.FindChild(newStr).gameObject);
				temp.defenceTokenPositions.Add (temp.defenceTokenContainer.transform.FindChild(newStr).gameObject);
			}

			Transform summary = heroInterfaces[i].transform.Find("Summary");

			temp.reset = summary.transform.Find ("Buttons").transform.Find("Reset").gameObject;
			temp.assaultDamage = summary.Find ("Assault Damage").GetComponent<UILabel>();
			temp.assaultDamagePerToken = temp.assaultDamage.transform.Find ("Per Token").GetComponent<UILabel>();
			temp.auxiliaryDamage = summary.Find ("Auxiliary Damage").GetComponent<UILabel>();
			temp.auxiliaryDamagePerToken = temp.auxiliaryDamage.transform.Find ("Per Token").GetComponent<UILabel>();
			temp.defence = summary.Find ("Defence").GetComponent<UILabel>();
			temp.defencePerToken = temp.defence.transform.Find ("Per Token").GetComponent<UILabel>();
			temp.health = summary.Find ("Health").GetComponent<UILabel>();
			temp.name = summary.Find ("Name").GetComponent<UILabel>();
			temp.type = summary.Find ("Type").GetComponent<UILabel>();
			
			heroInvasionLabels.Add (temp); //Assigning to list

			NGUITools.SetActive(heroInterfaces[i], false);
		}
		
		for(int i = 0; i < 6; ++i) //Planet interface
		{
			PlanetInvasionLabels temp = new PlanetInvasionLabels();
			
			Transform info = planetList[i].transform.Find ("Info");
			
			temp.name = info.Find("Name").GetComponent<UILabel>();
			temp.type = info.Find("Type").GetComponent<UILabel>();
			temp.offence = info.Find("Offence").GetComponent<UILabel>();
			temp.defence = info.Find("Defence").GetComponent<UILabel>();
			temp.population = info.Find("Population").GetComponent<UILabel>();
			temp.tokenContainer = planetList[i].transform.Find ("Token Container").gameObject;

			planetInvasionLabels.Add (temp);
		}
	}

	private void LayoutPlanets(int system) //Used to position the list of planets to be invaded
	{
		int size = MasterScript.systemListConstructor.systemList[system].systemSize;
		float maxHeight = ((size - 1) * 10f) + (65f * size); //Height of all planet sprites
		maxHeight = maxHeight / 2f; //Over 2 to get difference from y = 0

		for(int i = 0; i < 6; ++i) //For all planet sprites
		{
			if(i < size) //If it is less than the system size
			{
				NGUITools.SetActive(planetList[i], true); //Activate it
				float y = (i * 80f) + 32.5f; //Its y height is it's iterator value multiplied by 75, +32.5
				y = maxHeight - y; //Get the actual y value by subtracting it from maxheight

				float xDifference = -50f;

				if(i % 2 == 0)
				{
					xDifference = 50f;
				}

				Vector3 temp = new Vector3(-85f + xDifference, y, 0f); //Create a new vector with a -85 x offset to centre the sprite
				planetList[i].transform.localPosition = temp; //Set the local position

				if(MasterScript.systemListConstructor.systemList[system].planetsInSystem[i].planetColonised == false)
				{
					NGUITools.SetActive (planetInvasionLabels[i].tokenContainer, false);
				}
			}
			else //If it's greater than the system size
			{
				NGUITools.SetActive(planetList[i], false); //Deactivate it
			}
		}
	}

	private void UpdateOpenMenuItems()
	{
		NGUITools.SetActive(MasterScript.heroGUI.heroDetailsContainer, false); //Close the hero details window
		NGUITools.SetActive(summary, true);
	
		CheckForOtherHeroes();
			
		if(background.activeInHierarchy == false) //Set the background to active
		{
			NGUITools.SetActive(background, true);
		}
			
		UpdatePlanetInvasionValues(system);
		UpdateHeroInterfaces();
			
		if(createdTokens == false)
		{
			management.CreateTokens("Assault", MasterScript.heroGUI.currentHero.GetComponent<HeroScriptParent>(), 0);
			management.CreateTokens("Auxiliary", MasterScript.heroGUI.currentHero.GetComponent<HeroScriptParent>(), 0);
			management.CreateTokens("Defence", MasterScript.heroGUI.currentHero.GetComponent<HeroScriptParent>(), 0);
				
			createdTokens = true;
		}
	}

	private void UpdateClosedMenuItems()
	{
		hero2Object = null;
		hero3Object = null;
		NGUITools.SetActive(MasterScript.heroGUI.heroDetailsContainer, true);
		NGUITools.SetActive(includeHero2, false);
		NGUITools.SetActive(includeHero3, false);
		NGUITools.SetActive(summary, false);
		
		for(int i = 0; i < 3; ++i)
		{
			management.ResetTokenPositions(heroInvasionLabels[i].assaultTokensList, true); //Call the reset function with the gameobject lists containing the tokens
			management.ResetTokenPositions(heroInvasionLabels[i].auxiliaryTokensList, true);
			management.ResetTokenPositions(heroInvasionLabels[i].defenceTokensList, true);
			NGUITools.SetActive(heroInterfaces[i], false);
			createdTokens = false;
		}
		
		for(int i = 0; i < planetList.Length; ++i)
		{
			NGUITools.SetActive (planetList[i], false);
		}
		
		if(background.activeInHierarchy == true)
		{
			NGUITools.SetActive(background, false);
		}
	}

	void Update()
	{
		if(MasterScript.heroGUI.currentHero != null) //If a hero is selected
		{
			heroScript = MasterScript.heroGUI.currentHero.GetComponent<HeroScriptParent> (); //Get references to hero scripts
			systemDefence = heroScript.heroLocation.GetComponent<SystemDefence> (); //And the defence script of that system
			
			if(systemDefence.underInvasion == true && openInvasionMenu == true) //If system is under invasion and the invasion menu is open
			{
				UpdateOpenMenuItems();
			}
		}
		
		if(openInvasionMenu == false)
		{
			UpdateClosedMenuItems();
			management.allTokens.Clear ();
		}
	}

	private void CheckForOtherHeroes()
	{
		for(int i = 0; i < MasterScript.playerTurnScript.playerOwnedHeroes.Count; ++i)
		{
			if(MasterScript.playerTurnScript.playerOwnedHeroes[i] == MasterScript.heroGUI.currentHero)
			{
				continue;
			}
			
			heroScript = MasterScript.playerTurnScript.playerOwnedHeroes[i].GetComponent<HeroScriptParent>();
			
			if(heroScript.heroLocation == MasterScript.systemListConstructor.systemList[system].systemObject)
			{
				if(includeHero2.activeInHierarchy == false && hero2Object == null)
				{
					hero2Object = MasterScript.playerTurnScript.playerOwnedHeroes[i];
					NGUITools.SetActive(includeHero2, true);
					includeHero2.transform.Find ("Label").GetComponent<UILabel>().text = "INCLUDE " + heroScript.heroType;
					continue;
				}
				if(includeHero2.activeInHierarchy == true && hero3Object == null && includeHero3.activeInHierarchy == false)
				{
					if(MasterScript.playerTurnScript.playerOwnedHeroes[i] != hero2Object)
					{
						hero3Object = MasterScript.playerTurnScript.playerOwnedHeroes[i];
						NGUITools.SetActive(includeHero3, true);
						includeHero3.transform.Find ("Label").GetComponent<UILabel>().text = "INCLUDE" + heroScript.heroType;
					}
				}
			}
		}
	}

	private void UpdateHeroInterfaces()
	{
		for(int i = 0; i < 3; ++i)
		{
			if(hero2Object != null && i == 1 && includeHero2.activeInHierarchy == false || hero3Object != null && i == 2 && includeHero3.activeInHierarchy == false || i == 0)
			{
				NGUITools.SetActive (heroInterfaces [i], true);

				switch(i)
				{
				case 0:
					heroScript = MasterScript.heroGUI.currentHero.GetComponent<HeroScriptParent>();
					break;
				case 1:
					heroScript = hero2Object.GetComponent<HeroScriptParent>();
					break;
				case 2:
					heroScript = hero3Object.GetComponent<HeroScriptParent>();
					break;
				default:
					break;
				}

				heroInvasionLabels [i].health.text = heroScript.currentHealth + "/" + heroScript.maxHealth + " HEALTH";
				heroInvasionLabels [i].name.text = "A HERO";
				heroInvasionLabels [i].type.text = heroScript.heroType;
				
				heroInvasionLabels [i].assaultDamage.text = Math.Round (heroScript.assaultDamage, 0) + " ASSAULT DAMAGE";
				heroInvasionLabels [i].assaultDamagePerToken.text = Math.Round (heroScript.assaultDamage / (float)heroScript.assaultTokens, 0) + " PER";
				heroInvasionLabels [i].auxiliaryDamage.text = Math.Round (heroScript.auxiliaryDamage, 0) + " AUXILIARY DAMAGE";
				heroInvasionLabels [i].auxiliaryDamagePerToken.text = Math.Round (heroScript.auxiliaryDamage / (float)heroScript.auxiliaryTokens, 0) + " PER";
				heroInvasionLabels [i].defence.text = Math.Round (heroScript.defence, 0) + " DEFENCE";
				heroInvasionLabels [i].defencePerToken.text = Math.Round (heroScript.defence / (float)heroScript.defenceTokens, 0) + " PER";
			}
		}
	}

	private void UpdatePlanetInvasionValues(int thisSystem) //Used to update the labels of the planets
	{
		for(int i = 0; i < 6; i++) //For all labels
		{	
			if(i < MasterScript.systemListConstructor.systemList[thisSystem].systemSize) //If this planet is active in the system
			{
				NGUITools.SetActive(planetList[i], true); //Set the label container to be active

				planetInvasionLabels[i].name.text = MasterScript.systemListConstructor.systemList[thisSystem].systemName.ToUpper() + " " + i; //Set it's name
				planetInvasionLabels[i].type.text = MasterScript.systemListConstructor.systemList[thisSystem].planetsInSystem[i].planetType.ToUpper(); //And it's type

				if(MasterScript.systemListConstructor.systemList[thisSystem].planetsInSystem[i].planetColonised == false) //If it's not colonised
				{
					planetInvasionLabels[i].defence.text = "NA"; //Set appropriate values for defence, offence, and population
					planetInvasionLabels[i].offence.text = "NA";
					planetInvasionLabels[i].population.text = "UNCOLONISED";
					continue;
				}

				planetInvasionLabels[i].defence.text = MasterScript.systemListConstructor.systemList[thisSystem].planetsInSystem[i].planetCurrentDefence.ToString() + "/"
					+ MasterScript.systemListConstructor.systemList[thisSystem].planetsInSystem[i].planetMaxDefence.ToString () + " DEFENCE"; //Else display the current values of defence, offence, and population
				planetInvasionLabels[i].offence.text = MasterScript.systemListConstructor.systemList[thisSystem].planetsInSystem[i].planetOffence.ToString ();
				planetInvasionLabels[i].population.text = MasterScript.systemListConstructor.systemList[thisSystem].planetsInSystem[i].planetPopulation + "%/" 
					+ MasterScript.systemListConstructor.systemList[thisSystem].planetsInSystem[i].maxPopulation + "% POPULATION";
			}
			
			if(i >= MasterScript.systemListConstructor.systemList[thisSystem].systemSize) //If the planet is not active i.e the label number is greater than the system size
			{
				NGUITools.SetActive(planetList[i], false); //Set the label container to inactive
			}
		}
	}

	public void OpenPlanetInvasionScreen()
	{
		NGUITools.SetActive (invasionScreen, true);
		
		heroScript = MasterScript.heroGUI.currentHero.GetComponent<HeroScriptParent> ();
		
		system = MasterScript.RefreshCurrentSystem(heroScript.heroLocation);

		int loadSystem = loadInvasion.CheckForExistingInvasion (system);

		if(loadSystem != -1)
		{
			loadInvasion.ReloadInvasionScreen(loadSystem);
			createdTokens = true;
		}
		
		systemDefence = MasterScript.systemListConstructor.systemList [system].systemObject.GetComponent<SystemDefence> ();

		systemDefence.underInvasion = true;
		
		LayoutPlanets(system);
	}

	public class PlanetInvasionLabels
	{
		public UILabel offence, defence, name, population, type;
		public GameObject tokenContainer; 
	}

	public class HeroInvasionLabel
	{
		public GameObject defenceTokenContainer, assaultTokenContainer, auxiliaryTokenContainer, reset;
		public List<GameObject> defenceTokensList = new List<GameObject>();
		public List<GameObject> assaultTokensList = new List<GameObject>();
		public List<GameObject> auxiliaryTokensList = new List<GameObject>();
		public List<GameObject> assaultTokenPositions = new List<GameObject>();
		public List<GameObject> auxiliaryTokenPositions = new List<GameObject>();
		public List<GameObject> defenceTokenPositions = new List<GameObject>();
		public UILabel defence, defencePerToken, assaultDamage, assaultDamagePerToken, auxiliaryDamage, auxiliaryDamagePerToken, health, type, name;
	}
}
