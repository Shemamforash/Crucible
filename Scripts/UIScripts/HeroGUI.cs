using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class HeroGUI : MasterScript 
{
	public bool openHeroLevellingScreen;
	public GameObject heroObject, merchantQuad, turnInfoBar, heroDetailsContainer, currentHero, currentTarget;
	public GameObject[] heroUIParents = new GameObject[3];
	private List<HeroUI> heroUIInterfaces = new List<HeroUI>();
	private List<GameObject> pathToSystem = new List<GameObject>();
	private RaycastHit hit;

	void Start()
	{
		string[] heroNames = new string[3] {"Hero Info 1", "Hero Info 2", "Hero Info 3"};

		for(int i = 0; i < 3; ++i)
		{
			HeroUI newUI = new HeroUI();
			newUI.uiParent = heroDetailsContainer.transform.FindChild(heroNames[i]).gameObject;
			newUI.background = newUI.uiParent.transform.FindChild("Background").gameObject;
			newUI.armour = newUI.uiParent.transform.FindChild ("Armour").GetComponent<UILabel>();
			newUI.name = newUI.uiParent.transform.FindChild ("Name").GetComponent<UILabel>();
			newUI.nameInterface = newUI.name.transform.FindChild ("Button").GetComponent<UIButton>();
			Transform temp = newUI.uiParent.transform.FindChild ("Diplomat Buttons");
			newUI.embargo = temp.FindChild ("Embargo Button").GetComponent<UIButton>();
			newUI.enterSystem = temp.FindChild ("Enter System Button").GetComponent<UIButton>();
			newUI.promote = temp.FindChild ("Promote Button").GetComponent<UIButton>();
			temp = newUI.uiParent.transform.FindChild ("Infiltrator Buttons");
			newUI.otherSkill = temp.FindChild ("Other Skill Button").GetComponent<UIButton>();
			newUI.spy = temp.FindChild ("Spy Button").GetComponent<UIButton>();
			newUI.strike = temp.FindChild ("Strike Button").GetComponent<UIButton>();
			temp = newUI.uiParent.transform.FindChild ("Soldier Buttons");
			newUI.claim = temp.FindChild ("Claim Button").GetComponent<UIButton>();
			newUI.guard = temp.FindChild ("Guard Button").GetComponent<UIButton>();
			newUI.invade = temp.FindChild ("Invasion Button").GetComponent<UIButton>();
			heroUIInterfaces.Add (newUI);
			ActivateHeroUI(i, false);
		}
	}

	void Update()
	{
		hit = new RaycastHit();
		currentTarget = null;
		
		if(Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit))
		{
			if(Input.GetMouseButtonDown(0)) //Used to start double click events and to identify systems when clicked on. Throws up error if click on a connector object.
			{
				if(hit.collider.gameObject.tag == "Hero")
				{
					currentHero = hit.collider.gameObject;
					heroShip = currentHero.GetComponent<HeroShip>();
					heroScript = currentHero.GetComponent<HeroScriptParent>();

					if(heroScript.heroOwnedBy != playerTurnScript.playerRace)
					{
						currentHero = null;
					}
				}
			}

			if(hit.collider.gameObject.tag == "StarSystem")
			{
				if(currentHero != null)
				{
					currentTarget = hit.collider.gameObject;
					heroMovement = currentHero.GetComponent<HeroMovement> ();
					heroMovement.FindPath(heroScript.heroLocation, hit.collider.gameObject, false);
					
					if(Input.GetMouseButtonDown (1))
					{
						heroMovement.allowMovement = true;
						if(heroScript.invasionObject != null)
						{
							Destroy (heroScript.invasionObject);
						}
					}

					DestroyUIPath();

					for(int i = 0; i < heroMovement.finalPath.Count - 1; ++i)
					{
						pathToSystem.Add (uiObjects.CreateConnectionLine(heroMovement.finalPath[i], heroMovement.finalPath[i + 1]));
					}
				}
			}

			if(hit.collider.gameObject.tag != "StarSystem")
			{
				DestroyUIPath();
			}
		}

		if(currentTarget == null)
		{
			DestroyUIPath();
		}

		ShowHeroDetails();
	}

	private void DestroyUIPath()
	{
		if(pathToSystem.Count != 0)
		{
			for(int i = 0; i < pathToSystem.Count; ++i)
			{
				GameObject.Destroy(pathToSystem[i]);
			}
			
			pathToSystem.Clear ();
		}
	}
	
	private void DeactivateSoldierButtons(int i)
	{
		NGUITools.SetActive(heroUIInterfaces[i].invade.gameObject, false);
		NGUITools.SetActive(heroUIInterfaces[i].guard.gameObject, false);
		NGUITools.SetActive(heroUIInterfaces[i].claim.gameObject, false);
		
		heroUIInterfaces[i].invade.enabled = false;
		heroUIInterfaces[i].guard.enabled = false;
		heroUIInterfaces[i].claim.enabled = false;
	}

	private void DeactivateInfiltratorButtons(int i)
	{
		NGUITools.SetActive(heroUIInterfaces[i].spy.gameObject, false);
		NGUITools.SetActive(heroUIInterfaces[i].strike.gameObject, false);
		NGUITools.SetActive(heroUIInterfaces[i].otherSkill.gameObject, false);
		
		heroUIInterfaces[i].spy.enabled = false;
		heroUIInterfaces[i].strike.enabled = false;
		heroUIInterfaces[i].otherSkill.enabled = false;
	}

	private void DeactivateDiplomatButtons(int i)
	{
		NGUITools.SetActive(heroUIInterfaces[i].enterSystem.gameObject, false);
		NGUITools.SetActive(heroUIInterfaces[i].promote.gameObject, false);
		NGUITools.SetActive(heroUIInterfaces[i].embargo.gameObject, false);
		
		heroUIInterfaces[i].enterSystem.enabled = false;
		heroUIInterfaces[i].promote.enabled = false;
		heroUIInterfaces[i].embargo.enabled = false;
	}

	private void ActivateHeroUI(int i, bool stateOfHero)
	{
		Debug.Log (stateOfHero);
		NGUITools.SetActive (heroUIInterfaces[i].claim.transform.parent.gameObject, stateOfHero);
		NGUITools.SetActive (heroUIInterfaces[i].embargo.transform.parent.gameObject, stateOfHero);
		NGUITools.SetActive (heroUIInterfaces[i].strike.transform.parent.gameObject, stateOfHero);
		NGUITools.SetActive (heroUIInterfaces[i].name.gameObject, stateOfHero);
		NGUITools.SetActive (heroUIInterfaces[i].background, stateOfHero);
	}
	
	private void DiplomatSwitchFunction(int i, bool enemyOwned)
	{
		NGUITools.SetActive(heroUIInterfaces[i].enterSystem.gameObject, true);
		NGUITools.SetActive(heroUIInterfaces[i].promote.gameObject, true);
		NGUITools.SetActive(heroUIInterfaces[i].embargo.gameObject, true);
		if(enemyOwned == true)
		{
			heroUIInterfaces[i].enterSystem.enabled = true;
			heroUIInterfaces[i].promote.enabled = true;
			heroUIInterfaces[i].embargo.enabled = true;
		}
		else
		{
			heroUIInterfaces[i].enterSystem.enabled = false;
			heroUIInterfaces[i].promote.enabled = false;
			heroUIInterfaces[i].embargo.enabled = false;
		}
		DeactivateSoldierButtons(i);
		DeactivateInfiltratorButtons(i);
	}

	private void SoldierSwitchFunction(int i, bool enemyOwned, bool unowned)
	{
		NGUITools.SetActive(heroUIInterfaces[i].invade.gameObject, true);
		NGUITools.SetActive(heroUIInterfaces[i].guard.gameObject, true);
		NGUITools.SetActive(heroUIInterfaces[i].claim.gameObject, true);
		if(enemyOwned == true)
		{
			heroUIInterfaces[i].invade.enabled = true;
		}
		else
		{
			heroUIInterfaces[i].invade.enabled = false;
		}
		if(unowned != true)
		{
			heroUIInterfaces[i].guard.enabled = true;
		}
		else
		{
			heroUIInterfaces[i].guard.enabled = false;
		}
		if(unowned == true)
		{
			heroUIInterfaces[i].claim.enabled = true;
		}
		else
		{
			heroUIInterfaces[i].claim.enabled = false;
		}
		DeactivateInfiltratorButtons(i);
		DeactivateDiplomatButtons(i);
	}

	private void InfiltratorSwitchFunction(int i, bool enemyOwned)
	{
		NGUITools.SetActive(heroUIInterfaces[i].spy.gameObject, true);
		NGUITools.SetActive(heroUIInterfaces[i].strike.gameObject, true);
		NGUITools.SetActive(heroUIInterfaces[i].otherSkill.gameObject, true);
		if(enemyOwned == true)
		{
			heroUIInterfaces[i].spy.enabled = true;
		}
		else
		{
			heroUIInterfaces[i].spy.enabled = false;
		}
		heroUIInterfaces[i].strike.enabled = true;
		heroUIInterfaces[i].otherSkill.enabled = true;
		DeactivateSoldierButtons(i);
		DeactivateDiplomatButtons(i);
	}
	
	public void ShowHeroDetails()
	{
		for(int i = 0; i < 3; ++i) //For all possible heroes
		{
			if(i < playerTurnScript.playerOwnedHeroes.Count) //If the possible hero corresponds to an actual hero
			{
				ActivateHeroUI(i, true); //Activate the heroUI
				heroScript = playerTurnScript.playerOwnedHeroes[i].GetComponent<HeroScriptParent>(); //Get a reference to the hero script
				heroUIInterfaces[i].armour.text = Math.Round(heroScript.currentHealth, 1) + "/" + Math.Round(heroScript.maxHealth, 1); //Write the current armour level to a text box
				heroUIInterfaces[i].name.text = heroScript.heroType + " Dude/Ette"; //Write the name of the hero to a text box

				bool enemyOwned = false; //New bool to determine whether the system the hero is orbiting is currently owned by an enemy player
				bool unowned = false; //New bool to determinte whether the system the hero is orbiting is unowned

				if(systemListConstructor.systemList[heroScript.system].systemOwnedBy != heroScript.heroOwnedBy && systemListConstructor.systemList[heroScript.system].systemOwnedBy != null) //If the system is owned by someone that is not this player
				{
					enemyOwned = true; //It is enemy owned
				}
				if(systemListConstructor.systemList[heroScript.system].systemOwnedBy == null) //If it has no owner
				{
					unowned = true; //It is unowned
				}

				switch(heroScript.heroType) //Switch to activate the class specific UI components
				{
				case "Diplomat":
					DiplomatSwitchFunction(i, enemyOwned); //Diplomat takes the hero number and the enemy owned value
					break;
				case "Soldier":
					SoldierSwitchFunction(i, enemyOwned, unowned); //Soldier takes hero number, enemy owned value and unowned value
					break;
				case "Infiltrator":
					InfiltratorSwitchFunction(i, enemyOwned); //Infiltrator takes hero number and the enemy owned value
					break;
				default:
					Debug.Log ("Invalid Hero"); //If for some reason there is no hero write something to the console so I know there is an error
					break;
				}
				continue; //Continue to the next possible hero
			}

			ActivateHeroUI(i, false); //If the hero doesn't exist, disable the UI
		}
	}

	public void InvasionButtonClick()
	{
		for(int i = 0; i < 3; ++i)
		{
			if(heroUIInterfaces[i].nameInterface == UIButton.current.transform.parent)
			{
				heroScript = playerTurnScript.playerOwnedHeroes[i].GetComponent<HeroScriptParent>();
			}
		}

		systemInvasion.hero = heroScript;
		systemInvasion.StartSystemInvasion(heroScript.system);
	}

	public void Embargo()
	{
		heroScript = currentHero.GetComponent<HeroScriptParent> ();
		systemSIMData = heroScript.heroLocation.GetComponent<SystemSIMData> ();
		systemSIMData.promotedBy = null;
		systemSIMData.embargoedBy = heroScript.heroOwnedBy;
		systemSIMData.embargoTimer = Time.time;
	}

	public void Promote()
	{
		heroScript = currentHero.GetComponent<HeroScriptParent> ();
		systemSIMData = heroScript.heroLocation.GetComponent<SystemSIMData> ();
		systemSIMData.embargoedBy = null;
		systemSIMData.promotedBy = heroScript.heroOwnedBy;
		systemSIMData.promotionTimer = Time.time;
	}

	public void Guard()
	{
		systemSIMData = heroScript.heroLocation.GetComponent<SystemSIMData> ();
		systemSIMData.guardedBy = heroScript.heroOwnedBy;
	}

	public void Protect()
	{
		systemSIMData = heroScript.heroLocation.GetComponent<SystemSIMData> ();
		systemSIMData.protectedBy = heroScript.gameObject;
	}

	private class HeroUI
	{
		public GameObject uiParent, background;
		public UILabel name, armour;
		public UIButton nameInterface, embargo, promote, enterSystem, otherSkill, spy, strike, claim, guard, invade;
	}
}
