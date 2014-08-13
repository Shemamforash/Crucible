using UnityEngine;
using System.Collections;

public class MasterScript : MonoBehaviour
{
	[HideInInspector]
	public MasterScript masterScript;
	[HideInInspector]
	public SystemListConstructor systemListConstructor;
	[HideInInspector]
	public MapConstructor mapConstructor;
	[HideInInspector]
	public AmbientStarRandomiser ambientStarRandomiser;

	[HideInInspector]
	public SystemSIMData systemSIMData;
	[HideInInspector]
	public CameraFunctions cameraFunctionsScript;
	[HideInInspector]
	public LineRenderScript lineRenderScript;
	[HideInInspector]
	public SystemDefence systemDefence;

	[HideInInspector]
	public TurnInfo turnInfoScript;
	[HideInInspector]
	public PlayerTurn playerTurnScript;
	[HideInInspector]
	public AIBasicParent baseAIScript;
	[HideInInspector]
	public WinConditions winConditions;
	[HideInInspector]
	public SystemFunctions systemFunctions;
	[HideInInspector]
	public SystemInvasions systemInvasion;
	
	[HideInInspector]
	public ImprovementsBasic improvementsBasic;
	[HideInInspector]
	public HeroScriptParent heroScript;
	[HideInInspector]
	public HeroMovement heroMovement;
	[HideInInspector]
	public HeroShip heroShip;
	[HideInInspector]
	public DiplomacyControlScript diplomacyScript;
	[HideInInspector]
	public RacialTraits racialTraitScript;
	[HideInInspector]
	public UIObjects uiObjects;
	[HideInInspector]
	public SystemInfoPopup systemPopup;

	[HideInInspector]
	public SystemGUI systemGUI;
	[HideInInspector]
	public HeroGUI heroGUI;
	[HideInInspector]
	public GalaxyGUI galaxyGUI;
	[HideInInspector]
	public InvasionGUI invasionGUI;
	[HideInInspector]
	public VoronoiGeneratorAndDelaunay voronoiGenerator;
	[HideInInspector]
	public Triangulation triangulation;
	[HideInInspector]
	public HeroResourceImprovement heroResource;

	private void Awake() //Assigns script references
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

	public int RefreshCurrentSystem(GameObject thisSystem) //Returns the systemList enumerator of a system gameobject
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

	public void WipePlanetInfo(int system, int planet) //Used to reset planets to default
	{
		systemListConstructor.systemList [system].planetsInSystem [planet].planetColonised = false;
		systemListConstructor.systemList [system].planetsInSystem [planet].expansionPenaltyTimer = 0f;
		systemListConstructor.systemList [system].planetsInSystem [planet].planetImprovementLevel = 0;

		for(int i = 0; i < systemListConstructor.systemList [system].planetsInSystem [planet].improvementSlots; ++i)
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


