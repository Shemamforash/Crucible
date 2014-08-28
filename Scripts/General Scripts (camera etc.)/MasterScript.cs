using UnityEngine;
using System.Collections;

public static class MasterScript
{
	[HideInInspector]
	public static SystemListConstructor systemListConstructor;
	[HideInInspector]
	public static MapConstructor mapConstructor;
	[HideInInspector]
	public static AmbientStarRandomiser ambientStarRandomiser;

	[HideInInspector]
	public static SystemSIMData systemSIMData;
	[HideInInspector]
	public static CameraFunctions cameraFunctionsScript;
	[HideInInspector]
	public static LineRenderScript lineRenderScript;
	[HideInInspector]
	public static SystemDefence systemDefence;

	[HideInInspector]
	public static TurnInfo turnInfoScript;
	[HideInInspector]
	public static PlayerTurn playerTurnScript;
	[HideInInspector]
	public static AIBasicParent baseAIScript;
	[HideInInspector]
	public static WinConditions winConditions;
	[HideInInspector]
	public static SystemFunctions systemFunctions;
	[HideInInspector]
	public static SystemInvasions systemInvasion;
	
	[HideInInspector]
	public static ImprovementsBasic improvementsBasic;
	[HideInInspector]
	public static HeroScriptParent heroScript;
	[HideInInspector]
	public static HeroMovement heroMovement;
	[HideInInspector]
	public static HeroShip heroShip;
	[HideInInspector]
	public static DiplomacyControlScript diplomacyScript;
	[HideInInspector]
	public static RacialTraits racialTraitScript;
	[HideInInspector]
	public static UIObjects uiObjects;
	[HideInInspector]
	public static SystemInfoPopup systemPopup;

	[HideInInspector]
	public static SystemGUI systemGUI;
	[HideInInspector]
	public static HeroGUI heroGUI;
	[HideInInspector]
	public static GalaxyGUI galaxyGUI;
	[HideInInspector]
	public static InvasionGUI invasionGUI;
	[HideInInspector]
	public static VoronoiGeneratorAndDelaunay voronoiGenerator;
	[HideInInspector]
	public static Triangulation triangulation;
	[HideInInspector]
	public static HeroResourceImprovement heroResource;

	public static void ScriptReferences() //Assigns script references
	{
		systemListConstructor = GameObject.FindGameObjectWithTag("ScriptContainer").GetComponent<SystemListConstructor>();
		cameraFunctionsScript = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFunctions>();
		turnInfoScript = GameObject.FindGameObjectWithTag("ScriptContainer").GetComponent<TurnInfo>();
		playerTurnScript = GameObject.FindGameObjectWithTag("ScriptContainer").GetComponent<PlayerTurn>();
		diplomacyScript = GameObject.FindGameObjectWithTag("ScriptContainer").GetComponent<DiplomacyControlScript>();
		systemGUI = GameObject.FindGameObjectWithTag("GUIContainer").GetComponent<SystemGUI>();
		heroGUI = GameObject.FindGameObjectWithTag("GUIContainer").GetComponent<HeroGUI>();
		racialTraitScript = GameObject.FindGameObjectWithTag ("ScriptContainer").GetComponent<RacialTraits> ();
		galaxyGUI = GameObject.FindGameObjectWithTag("GUIContainer").GetComponent<GalaxyGUI>();
		invasionGUI = GameObject.FindGameObjectWithTag ("GUIContainer").GetComponent<InvasionGUI> ();
		mapConstructor = GameObject.FindGameObjectWithTag ("ScriptContainer").GetComponent<MapConstructor> ();
		winConditions = GameObject.FindGameObjectWithTag ("ScriptContainer").GetComponent<WinConditions> ();
		systemFunctions = GameObject.FindGameObjectWithTag ("ScriptContainer").GetComponent<SystemFunctions> ();
		systemInvasion = GameObject.FindGameObjectWithTag ("ScriptContainer").GetComponent<SystemInvasions> ();
		uiObjects = GameObject.FindGameObjectWithTag ("GUIContainer").GetComponent<UIObjects> ();
		ambientStarRandomiser = GameObject.FindGameObjectWithTag ("ScriptContainer").GetComponent<AmbientStarRandomiser> ();
		voronoiGenerator = GameObject.FindGameObjectWithTag ("GUIContainer").GetComponent<VoronoiGeneratorAndDelaunay> ();
		heroResource = GameObject.FindGameObjectWithTag ("GUIContainer").GetComponent<HeroResourceImprovement> ();
		systemPopup = GameObject.FindGameObjectWithTag ("GUIContainer").GetComponent<SystemInfoPopup> ();
		triangulation = GameObject.FindGameObjectWithTag ("GUIContainer").GetComponent<Triangulation> ();
	}

	public static int RefreshCurrentSystem(GameObject thisSystem) //Returns the systemList enumerator of a system gameobject
	{
		for(int i = 0; i < systemListConstructor.systemList.Count; ++i)
		{
			if(systemListConstructor.systemList[i].systemObject == thisSystem)
			{
				return i;
			}
		}
		
		return 0;
	}

	public static void WipePlanetInfo(int system, int planet) //Used to reset planets to default
	{
		systemListConstructor.systemList [system].planetsInSystem [planet].planetColonised = false;
		systemListConstructor.systemList [system].planetsInSystem [planet].expansionPenaltyTimer = 0f;
		systemListConstructor.systemList [system].planetsInSystem [planet].planetImprovementLevel = 0;

		for(int i = 0; i < systemListConstructor.systemList [system].planetsInSystem [planet].currentImprovementSlots; ++i)
		{
			improvementsBasic = systemListConstructor.systemList[system].systemObject.GetComponent<ImprovementsBasic>();

			for(int j = 0; j < improvementsBasic.listOfImprovements.Count; ++j)
			{
				if(improvementsBasic.listOfImprovements[j].improvementName == systemListConstructor.systemList [system].planetsInSystem [planet].improvementsBuilt [i])
				{
					improvementsBasic.listOfImprovements[j].hasBeenBuilt = false;
					systemListConstructor.systemList [system].planetsInSystem [planet].improvementsBuilt [i] = null;
				}
			}
		}
	}
}


