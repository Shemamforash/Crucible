using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class HeroGUI : MasterScript 
{
	public bool openHeroLevellingScreen;
	public GameObject heroObject, merchantQuad, turnInfoBar, heroDetailsContainer, currentHero;
	public GameObject[] heroUIParents = new GameObject[3];
	private List<HeroUI> heroUIInterfaces = new List<HeroUI>();
	private RaycastHit hit;

	void Start()
	{
		string[] heroNames = new string[3] {"Hero Info 1", "Hero Info 2", "Hero Info 3"};

		for(int i = 0; i < 3; ++i)
		{
			HeroUI newUI = new HeroUI();
			newUI.uiParent = heroDetailsContainer.transform.FindChild(heroNames[i]).gameObject;
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
			NGUITools.SetActive (newUI.claim.transform.parent.gameObject, false);
			NGUITools.SetActive (newUI.embargo.transform.parent.gameObject, false);
			NGUITools.SetActive (newUI.strike.transform.parent.gameObject, false);
			NGUITools.SetActiveSelf (newUI.uiParent, false);

		}
	}

	void Update()
	{
		hit = new RaycastHit();
		
		if(Input.GetMouseButtonDown(0)) //Used to start double click events and to identify systems when clicked on. Throws up error if click on a connector object.
		{
			if(Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit))
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
		}

		if(Input.GetMouseButtonDown (1) && currentHero != null)
		{
			if(Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit))
			{
				if(hit.collider.gameObject.tag == "StarSystem")
				{
					heroMovement = currentHero.GetComponent<HeroMovement> ();
					heroMovement.FindPath(heroScript.heroLocation, hit.collider.gameObject);
					if(heroScript.invasionObject != null)
					{
						Destroy (heroScript.invasionObject);
					}
				}
			}
		}

		ShowHeroDetails();
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

	public void ShowHeroDetails()
	{
		for(int i = 0; i < 3; ++i)
		{
			NGUITools.SetActive(heroUIInterfaces[i].uiParent, false);

			if(i < playerTurnScript.playerOwnedHeroes.Count)
			{
				//NGUITools.SetActive(heroUIInterfaces[i].uiParent, true); 
				heroUIInterfaces[i].nameInterface.enabled = true;
				heroScript = playerTurnScript.playerOwnedHeroes[i].GetComponent<HeroScriptParent>();
				heroUIInterfaces[i].armour.text = Math.Round(heroScript.currentHealth, 1) + "/" + Math.Round(heroScript.maxHealth, 1);
				heroUIInterfaces[i].name.text = heroScript.heroType + " Dude/Ette";

				bool enemyOwned = false;
				bool unowned = false;

				if(systemListConstructor.systemList[heroScript.system].systemOwnedBy != heroScript.heroOwnedBy && systemListConstructor.systemList[heroScript.system].systemOwnedBy != null)
				{
					enemyOwned = true;
				}
				if(systemListConstructor.systemList[heroScript.system].systemOwnedBy == null)
				{
					unowned = true;
				}

				switch(heroScript.heroType)
				{
				case "Diplomat":
					//NGUITools.SetActive(heroUIInterfaces[i].enterSystem.gameObject, true);
					//NGUITools.SetActive(heroUIInterfaces[i].promote.gameObject, true);
					//NGUITools.SetActive(heroUIInterfaces[i].embargo.gameObject, true);
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
					//DeactivateSoldierButtons(i);
					//DeactivateInfiltratorButtons(i);
					break;
				case "Soldier":
					//NGUITools.SetActive(heroUIInterfaces[i].invade.gameObject, true);
					//NGUITools.SetActive(heroUIInterfaces[i].guard.gameObject, true);
					//NGUITools.SetActive(heroUIInterfaces[i].claim.gameObject, true);
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
					///DeactivateInfiltratorButtons(i);
					//DeactivateDiplomatButtons(i);
					break;
				case "Infiltrator":
					//NGUITools.SetActive(heroUIInterfaces[i].spy.gameObject, true);
					//NGUITools.SetActive(heroUIInterfaces[i].strike.gameObject, true);
					//NGUITools.SetActive(heroUIInterfaces[i].otherSkill.gameObject, true);
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
					//DeactivateSoldierButtons(i);
					//DeactivateDiplomatButtons(i);
					break;
				default:
					Debug.Log ("Invalid Hero");
					break;
				}
			}
			else if(i >= playerTurnScript.playerOwnedHeroes.Count);
			{
				Debug.Log (playerTurnScript.playerOwnedHeroes.Count);
				heroUIInterfaces[i].name.text = "";
				heroUIInterfaces[i].armour.text = "";
				heroUIInterfaces[i].nameInterface.enabled = false;
				NGUITools.SetActive(heroUIInterfaces[i].uiParent, false); 
			}
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
		public GameObject uiParent;
		public UILabel name, armour;
		public UIButton nameInterface, embargo, promote, enterSystem, otherSkill, spy, strike, claim, guard, invade;
	}
}
