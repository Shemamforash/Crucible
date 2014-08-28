using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TechTreeGUI : MonoBehaviour
{
	public GameObject techTree;
	public List<TechLabels> techLabels = new List<TechLabels>();
	public UILabel openCloseTree;
	
	public void Start()
	{
		NGUITools.SetActive(techTree, true);

		GameObject[] tempArray = GameObject.FindGameObjectsWithTag ("TechLabel");

		for(int i = 0; i < tempArray.Length; ++i)
		{
			TechLabels techLabel = new TechLabels();

			techLabel.label = tempArray[i].GetComponent<UILabel>();
			techLabel.button = tempArray[i].GetComponent<UIButton>();

			techLabels.Add (techLabel);
		}

		for(int i = 0; i < techLabels.Count; ++i)
		{
			techLabels[i].label.text = techLabels[i].label.gameObject.name;
			techLabels[i].button.enabled = false;
		}

		NGUITools.SetActive(techTree, false);
	}

	public void ShowTechTree ()
	{
		if(techTree.activeInHierarchy == false)
		{
			CheckActiveTech ();

			NGUITools.SetActive(techTree, true);

			openCloseTree.text = "Close Window";
		}

		else if (techTree.activeInHierarchy == true) 
		{
			NGUITools.SetActive(techTree, false);

			openCloseTree.text = "Tech Tree";
		}	
	}

	private static int FindTechInTree(string tech)
	{
		for(int i = 0; i < HeroTechTree.heroTechList.Count; ++i)
		{
			if(HeroTechTree.heroTechList[i].techName == tech)
			{
				return i;
			}
		}

		return -1;
	}

	private void CheckActiveTech()
	{
		for(int j = 0; j < techLabels.Count; ++j)
		{
			int techNo = FindTechInTree(techLabels[j].label.gameObject.name);

			if(techNo != -1)
			{
				if(HeroTechTree.heroTechList[techNo].isActive == true)
				{
					techLabels[j].label.text = HeroTechTree.heroTechList[techNo].techName;
					techLabels[j].label.gameObject.GetComponent<UISprite>().spriteName = "Blank Text Box";
					techLabels[j].button.enabled = false;
				}

				if(HeroTechTree.heroTechList[techNo].prerequisite == "Basic Components" && HeroTechTree.heroTechList[techNo].isActive == false)
				{
					techLabels[j].button.enabled = true;
					continue;
				}

				int preTech = FindTechInTree(HeroTechTree.heroTechList[techNo].prerequisite);

				if(preTech != -1)
				{
					if(HeroTechTree.heroTechList[preTech].isActive == false)
					{
						techLabels[j].label.text = "Requires Previous Upgrades";
						techLabels[j].label.gameObject.GetComponent<UISprite>().spriteName = "Label";
						continue;
					}

					if(HeroTechTree.heroTechList[preTech].isActive == true && HeroTechTree.heroTechList[techNo].isActive == false)
					{
						techLabels[j].button.enabled = true;
						techLabels[j].label.text = HeroTechTree.heroTechList[techNo].techName + " " + (HeroTechTree.heroTechList[techNo].knowledgeCost - MasterScript.playerTurnScript.researchCostModifier);
						techLabels[j].label.gameObject.GetComponent<UISprite>().spriteName = "Blank Text Box";
						continue;
					}
				}
			}
		}
	}

	public void ActivateTech()
	{
		PlayerTurn playerTurnScript = GameObject.FindGameObjectWithTag("ScriptContainer").GetComponent<PlayerTurn>();

		for(int i = 0; i < HeroTechTree.heroTechList.Count; ++i)
		{
			if(HeroTechTree.heroTechList[i].techName == UIButton.current.gameObject.name && playerTurnScript.knowledge >= (HeroTechTree.heroTechList[i].knowledgeCost - playerTurnScript.researchCostModifier))
			{
				playerTurnScript.knowledge -= (HeroTechTree.heroTechList[i].knowledgeCost - playerTurnScript.researchCostModifier);
				HeroTechTree.heroTechList[i].isActive = true;
				ShipFunctions.UpdateShips();
				CheckActiveTech();
			}
		}
	}
}

public class TechLabels
{
	public UILabel label;
	public UIButton button;
}
