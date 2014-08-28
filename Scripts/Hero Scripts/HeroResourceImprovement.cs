using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeroResourceImprovement : MonoBehaviour 
{
	public GameObject improvementScreen;
	private List<Stat> statObject = new List<Stat>();
	private string[] infiltratorStats = new string[4]{"Cloak Effectiveness", "Primary Offence", "Movement Speed", "Bomb Cooldown"};
	private float[] infiltratorStatVal = new float[4]{5.0f, 1.0f, 5.0f, -2.0f};
	private string[] soldierStats = new string[4]{"Armour", "Primary Offence", "Artillery Power", "Collateral Damage"};
	private float[] soldierStatVal = new float[4]{3.0f, 2.5f, 2.5f, -1.0f};
	private string[] diplomatStats = new string[4]{"Armour", "Resource Bonus", "Movement Speed", "Collateral Damage"};
	private float[] diplomatStatVal = new float[4]{1.0f, 0.5f, 2.5f, -2.0f};
	private bool initialised = false;
	private HeroScriptParent heroScript;

	public void LoadObjects()
	{
		NGUITools.SetActive (improvementScreen, true);

		string[] tempArr = new string[4] {"Stat One", "Stat Two", "Stat Three", "Stat Four"};
		string[] resources = new string[4]{"Blue Carbon", "Radioisotopes", "Antimatter", "Liquid Hydrogen"};

		for(int i = 0; i < 4; ++i)
		{
			Stat newStat = new Stat ();

			newStat.statName = improvementScreen.transform.Find (tempArr[i]).GetComponent<UILabel>();
			newStat.statBonus = newStat.statName.transform.Find ("Level").GetComponent<UILabel>();
			newStat.button = newStat.statName.transform.Find("Button").GetComponent<UIButton>();
			newStat.resourceRq = resources[i];

			statObject.Add (newStat);
		}

		initialised = true;

		NGUITools.SetActive (improvementScreen, false);
	}

	void Update()
	{
		if(Input.GetKeyDown("escape"))
		{
			NGUITools.SetActive (improvementScreen, false);
		}

		if(improvementScreen.activeInHierarchy == true)
		{
			heroScript = MasterScript.heroGUI.currentHero.GetComponent<HeroScriptParent> ();

			if(MasterScript.playerTurnScript.blueCarbon > 0)
			{
				statObject[0].button.enabled = true;
			}
			if(MasterScript.playerTurnScript.blueCarbon == 0)
			{
				statObject[0].button.enabled = false;
			}

			if(MasterScript.playerTurnScript.radioisotopes > 0)
			{
				statObject[1].button.enabled = true;
			}
			if(MasterScript.playerTurnScript.radioisotopes == 0)
			{
				statObject[1].button.enabled = false;
			}

			if(MasterScript.playerTurnScript.antimatter > 0)
			{
				statObject[2].button.enabled = true;
			}
			if(MasterScript.playerTurnScript.antimatter == 0)
			{
				statObject[2].button.enabled = false;
			}

			if(MasterScript.playerTurnScript.liquidH2 > 0)
			{
				statObject[3].button.enabled = true;
			}
			if(MasterScript.playerTurnScript.liquidH2 == 0)
			{
				statObject[3].button.enabled = false;
			}
		}
	}

	private void SetLabels(string[] arr, float[] floatArr)
	{
		heroScript = MasterScript.heroGUI.currentHero.GetComponent<HeroScriptParent> ();

		for(int i = 0; i < 4; ++i)
		{
			string tempStr = null;

			statObject[i].statName.text = arr[i];

			switch(statObject[i].resourceRq)
			{
			case "Liquid Hydrogen":
				tempStr = heroScript.lH2Spent + "(+" + heroScript.lH2Spent * floatArr[i] / 100 + "%)";
				statObject[i].statBonus.text = tempStr;
				break;
			case "Antimatter":
				tempStr = heroScript.antiSpent + "(+" + heroScript.antiSpent * floatArr[i] / 100 + "%)";
				statObject[i].statBonus.text = tempStr;
				break;
			case "Blue Carbon":
				tempStr = heroScript.blueCSpent + "(+" + heroScript.blueCSpent * floatArr[i] / 100 + "%)";
				statObject[i].statBonus.text = tempStr;
				break;
			case "Radioisotopes":
				tempStr = heroScript.radioSpent + "(+" + heroScript.radioSpent * floatArr[i] / 100 + "%)";
				statObject[i].statBonus.text = tempStr;
				break;
			default:
				break;
			}
		}

		NGUITools.SetActive (improvementScreen, true);
	}

	public void SpendResources()
	{
		heroScript = MasterScript.heroGUI.currentHero.GetComponent<HeroScriptParent> ();

		for(int i = 0; i < 4; ++i)
		{
			if(statObject[i].button == UIButton.current)
			{
				switch(i)
				{
				case 0:
					--MasterScript.playerTurnScript.blueCarbon;
					++heroScript.blueCSpent;

					if(heroScript.heroType == "Infiltrator")
					{
						heroScript.cloakMod += 0.05f;
					}
					if(heroScript.heroType == "Soldier")
					{
						heroScript.healthMod += 0.03f;
					}
					if(heroScript.heroType == "Diplomat")
					{
						heroScript.healthMod += 0.01f;
					}

					break;
				case 1:
					--MasterScript.playerTurnScript.radioisotopes;
					++heroScript.radioSpent;

					if(heroScript.heroType == "Infiltrator")
					{
						heroScript.assaultMod += 0.01f;
					}
					if(heroScript.heroType == "Soldier")
					{
						heroScript.assaultMod += 0.025f;
					}
					if(heroScript.heroType == "Diplomat")
					{
						heroScript.resourceMod += 0.005f;
					}

					break;
				case 2:
					--MasterScript.playerTurnScript.antimatter;
					++heroScript.antiSpent;

					if(heroScript.heroType == "Infiltrator")
					{
						heroScript.movementMod += 0.05f;
					}
					if(heroScript.heroType == "Soldier")
					{
						heroScript.auxiliaryMod += 0.025f;
					}
					if(heroScript.heroType == "Diplomat")
					{
						heroScript.movementMod += 0.025f;
					}

					break;
				case 3:
					--MasterScript.playerTurnScript.liquidH2;
					++heroScript.lH2Spent;

					if(heroScript.heroType == "Infiltrator")
					{
						heroScript.cooldownMod -= 0.02f;
					}
					if(heroScript.heroType == "Soldier")
					{
						heroScript.auxiliaryMod -= 0.01f;
					}
					if(heroScript.heroType == "Diplomat")
					{
						heroScript.auxiliaryMod += 0.02f;
					}

					break;
				default:
					break;
				}

				OpenMenu();
			}
		}
	}

	public void OpenMenu()
	{
		if(initialised == false)
		{
			LoadObjects ();
		}

		heroScript = MasterScript.heroGUI.currentHero.GetComponent<HeroScriptParent> ();

		switch(heroScript.heroType)
		{
		case "Infiltrator":
			SetLabels(infiltratorStats, infiltratorStatVal);
			break;
		case "Soldier":
			SetLabels(soldierStats, soldierStatVal);
			break;
		case "Diplomat":
			SetLabels(diplomatStats, diplomatStatVal);
			break;
		default:
			break;
		}
	}
}

public class Stat
{
	public UIButton button;
	public UILabel statName, statBonus;
	public string resourceRq;
}
