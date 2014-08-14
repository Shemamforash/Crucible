using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

public class SystemListConstructor : MasterScript 
{
	private AmbientStarRandomiser ambientStars;

	[HideInInspector]
	public List<StarSystem> systemList = new List<StarSystem>();
	[HideInInspector]
	public List<PlanetInfo> planetList = new List<PlanetInfo>();
	[HideInInspector]
	private List<string> uncheckedSystems = new List<string> ();
	[HideInInspector]
	private List<string> firmSystems = new List<string> ();
	public List<BasicImprovement> basicImprovementsList = new List<BasicImprovement> ();

	private int connections;
	public int mapSize;
	public GameObject systemClone, kType, gType, rGiant, wDwarf, binary;
	private float xPos, yPos, distanceXY;
	public float systemScale = 0.0f, sysDistMin;
	public Transform systemContainer;
	public bool loaded = false;

	private void Start()
	{
		mapSize = PlayerPrefs.GetInt ("Map Size");
		PlanetRead ();
		SystemRead ();
		HeroTechTree.ReadTechFile ();
		SelectSystemsForMap ();
		CheckSystem ();
		CreateObjects ();
		mapConstructor.DrawMinimumSpanningTree ();

		voronoiGenerator.CreateVoronoiCells ();

		ambientStars = GameObject.Find ("ScriptsContainer").GetComponent<AmbientStarRandomiser> ();
		ambientStars.GenerateStars ();

		LoadBasicTechTree ();

		for(int i = 0; i < systemList.Count; ++i)
		{
			lineRenderScript = systemList[i].systemObject.GetComponent<LineRenderScript>();

			lineRenderScript.StartUp();
		}

		systemPopup.LoadOverlays ();

		galaxyGUI.SelectRace(PlayerPrefs.GetString ("Player Race"));
		loaded = true;
	}

	public int RefreshCurrentSystemA(GameObject thisSystem)
	{
		for(int i = 0; i < systemList.Count; ++i)
		{
			if(systemList[i].systemObject == thisSystem)
			{
				return i;
			}
		}
		
		return 0;
	}

	private void SelectSystemsForMap()
	{
		int randomInt = -1;
		mapConstructor.distanceMax = (mapSize - 260) / -4f;

		systemScale = (mapSize - 300.0f) / -320.0f;

		int difference = systemList.Count - firmSystems.Count;

		for(int j = 0; j < difference; ++j)
		{
			if(firmSystems.Count == mapSize)
			{
				break;
			}
			
			randomInt = Random.Range (0, uncheckedSystems.Count - 1);
			
			if(CheckWithinMinMaxDistance(uncheckedSystems[randomInt]) == true)
			{
				firmSystems.Add(uncheckedSystems[randomInt]);
				uncheckedSystems.RemoveAt(randomInt);
			}
		}

		if(firmSystems.Count < mapSize)
		{
			difference = mapSize - firmSystems.Count;

			for(int i = 0; i < difference; ++i)
			{
				randomInt = Random.Range (0, uncheckedSystems.Count - 1);

				firmSystems.Add(uncheckedSystems[randomInt]);
				
				uncheckedSystems.RemoveAt(randomInt);
			}
		}
	}

	private bool CheckWithinMinMaxDistance(string system)
	{
		Vector3 sysOne = systemList [CheckSystemName (system)].systemPosition;

		for(int i = 0; i < firmSystems.Count; ++i)
		{
			Vector3 sysTwo = systemList[CheckSystemName(firmSystems[i])].systemPosition;

			float distance = Vector3.Distance(sysOne, sysTwo);

			if(distance < sysDistMin)
			{
				return false;
			}
		}

		return true;
	}

	private int CheckSystemName(string name)
	{
		for(int i = 0; i < systemList.Count; ++i)
		{
			if(systemList[i].systemName == name)
			{
				return i;
			}
		}

		return -1;
	}
	
	private void CheckSystem()
	{
		for(int i = 0; i < systemList.Count; ++i)
		{
			if(firmSystems.Contains (systemList[i].systemName) == false)
			{
				systemList.RemoveAt(i);
				--i;
			}
		}
	}

	private GameObject FindStarType(int system)
	{
		switch(systemList[system].starType)
		{
		case "G-Type":
				return gType;
		case "K-Type":
				return kType;
		case "Red Giant":
				return rGiant;
		case "White Dwarf":
				return wDwarf;
		case "Binary":
				return binary;
		default:
				break;
		}

		return null;
	}

	private float StarScale(int system)
	{
		switch(systemList[system].starType)
		{
		case "G-Type":
			return systemScale / 2f;
		case "K-Type":
			return systemScale / 2f;
		case "Red Giant":
			return systemScale / 1.5f;
		case "White Dwarf":
			return systemScale / 3f;
		case "Binary":
			return systemScale / 3.5f;
		default:
			break;
		}

		return 0f;
	}

	private void CreateObjects()
	{
		for(int i = 0; i < systemList.Count; ++i)
		{
			Quaternion rot = new Quaternion();

			float randomRot = Random.Range(-180f, 180f);

			rot.eulerAngles = new Vector3(0f, -180f, randomRot);

			systemClone = (GameObject)Instantiate(FindStarType(i), systemList[i].systemPosition, rot);

			systemClone.transform.parent = systemContainer;

			float scale = StarScale(i);

			float tempScale = Random.Range (scale - scale / 5f, scale + scale / 5f);
		
			systemClone.transform.localScale = new Vector3(tempScale, tempScale, tempScale);

			systemClone.name = systemList[i].systemName;
		
			systemList[i].systemObject = systemClone;
		}
	}

	private string RandomPlanet()
	{
		int rnd = Random.Range (0, 100);

		if(rnd < 8)
		{
			return "Waste";
		}
		if(rnd >= 8 && rnd < 16)
		{
			return "Desolate";
		}
		if(rnd >= 16 && rnd < 21)
		{
			return "Gas Giant";
		}
		if(rnd >= 21 && rnd < 34)
		{
			return "Chasm";
		}
		if(rnd >= 34 && rnd < 43)
		{
			return "Prairie";
		}
		if(rnd >= 43 && rnd < 56)
		{
			return "Tundra";
		}
		if(rnd >= 56 && rnd < 71)
		{
			return "Cold Giant";
		}
		if(rnd >= 71 && rnd < 79)
		{
			return "Molten";
		}
		if(rnd >= 79 && rnd < 85)
		{
			return "Ocean";
		}
		if(rnd >= 85 && rnd < 93)
		{
			return "Boreal";
		}
		if(rnd >= 93 && rnd < 97)
		{
			return "Forest";
		}
		if(rnd >= 97)
		{
			return "Chthonic";
		}

		return "Null";
	}

	public void SystemRead()
	{
		string[] planetLocations = new string[6]{"C","D","E","F","G","H"};
		string planetName;

		using(XmlReader reader = XmlReader.Create ("System Data New.xml"))
		{
			while(reader.Read ())
			{
				if(reader.Name == "Row")
				{
					StarSystem system = new StarSystem();
					
					system.systemName = reader.GetAttribute("A");

					uncheckedSystems.Add(system.systemName);
					
					system.systemSize = int.Parse (reader.GetAttribute("B"));
					
					system.systemOwnedBy = null;
					
					system.numberOfConnections = 0;
					
					for(int j = 0; j < system.systemSize; ++j)
					{
						planetName = system.systemName + " " + j.ToString();
						
						Planet newPlanet = new Planet();
						
						newPlanet.planetName = planetName;

						if(reader.GetAttribute(planetLocations[j]) != null)
						{
							newPlanet.planetType = reader.GetAttribute(planetLocations[j]);
						}
						else
						{
							newPlanet.planetType = RandomPlanet();
						}

						newPlanet.planetCategory = FindPlanetCategory(newPlanet.planetType);
						newPlanet.planetImprovementLevel = 0;
						newPlanet.planetColonised = false;
						newPlanet.planetPopulation = 0;
						newPlanet.planetKnowledge = FindPlanetSIM(newPlanet.planetType, "Knowledge");
						newPlanet.planetPower = FindPlanetSIM(newPlanet.planetType, "Power");
						newPlanet.wealthValue = (int)FindPlanetSIM(newPlanet.planetType, "Wealth");
						newPlanet.maxPopulation = 0;
						newPlanet.baseImprovementSlots = (int)FindPlanetSIM(newPlanet.planetType, "Improvement Slots");
						newPlanet.underEnemyControl = false;
						newPlanet.expansionPenaltyTimer = 0.0f;

						int hasResources = Random.Range (0, 3);

						if(hasResources == 0)
						{
							switch(newPlanet.planetCategory)
							{
							case "Hot":
								newPlanet.rareResourceType = "RADIOISOTOPES";
								break;
							case "Cold":
								newPlanet.rareResourceType = "LIQUID HYDROGEN";
								break;
							case "Terran":
								newPlanet.rareResourceType = "BLUE CARBON";
								break;
							case "Gas Giant":
								newPlanet.rareResourceType = "ANTIMATTER";
								break;
							default:
								newPlanet.rareResourceType = null;
								break;
							}
						}
						
						for(int k = 0; k < (int)FindPlanetSIM(newPlanet.planetType, "Improvement Slots"); ++k)
						{
							newPlanet.improvementsBuilt.Add (null);
						}
						
						system.planetsInSystem.Add (newPlanet);
					}
					
					xPos = float.Parse (reader.GetAttribute("I"));
					yPos = float.Parse (reader.GetAttribute("J"));

					system.starType = reader.GetAttribute("K");
					
					system.systemPosition = new Vector3(xPos, yPos, 0.0f);
					
					systemList.Add (system);

					if(system.systemName == "Nephthys" || system.systemName == "Midgard" || system.systemName == "Samael")
					{
						if(PlayerPrefs.GetString("Planet One") == system.systemName || PlayerPrefs.GetString("Planet Two") == system.systemName || PlayerPrefs.GetString("Planet Three") == system.systemName)
						{
							firmSystems.Add (system.systemName);
							uncheckedSystems.Remove(system.systemName);
						}
					}
				}
			}
		}
	}

	public void PlanetRead()
	{		
		using(XmlReader reader =  XmlReader.Create("PlanetSICData.xml"))
		{
			while(reader.Read ())
			{
				if(reader.Name == "Row")
				{
					PlanetInfo planet = new PlanetInfo();
					
					planet.planetType = reader.GetAttribute ("A");
					planet.planetCategory = reader.GetAttribute ("B");
					planet.knowledge = float.Parse (reader.GetAttribute("C"));
					planet.power = float.Parse (reader.GetAttribute("D"));
					planet.improvementSlots = int.Parse (reader.GetAttribute("E"));
					planet.wealthCost = int.Parse (reader.GetAttribute("F"));
					
					planetList.Add (planet);
				}
			}
		}
	}

	private string FindPlanetCategory(string planetType)
	{
		for(int i = 0; i < 12; ++i)
		{
			if(planetList[i].planetType == planetType)
			{
				return planetList[i].planetCategory;
			}
		}

		return null;
	}

	public float FindPlanetSIM(string planetType, string resourceType)
	{
		for(int i = 0; i < 12; ++i)
		{
			if(planetList[i].planetType == planetType)
			{
				switch(resourceType)
				{
				case "Improvement Slots":
					return planetList[i].improvementSlots;
				case "Knowledge":
					return planetList[i].knowledge;
				case "Power":
					return planetList[i].power;
				case "Wealth":
					return (float)planetList[i].wealthCost;
				default:
					break;
				}
			}
		}
		
		return 0.0f;
	}

	public void LoadBasicTechTree()
	{
		using(XmlReader reader = XmlReader.Create ("ImprovementList.xml"))
		{
			while(reader.Read ())
			{
				if(reader.Name == "Row")
				{
					BasicImprovement improvement = new BasicImprovement();
					
					improvement.name = reader.GetAttribute("A");
					improvement.level = int.Parse (reader.GetAttribute("B"));
					improvement.category = reader.GetAttribute("C");
					improvement.cost = float.Parse(reader.GetAttribute("D"));
					improvement.wealthUpkeep = float.Parse (reader.GetAttribute("E"));
					improvement.powerUpkeep = float.Parse (reader.GetAttribute("F"));
					improvement.influence = reader.GetAttribute("G");
					improvement.details = reader.GetAttribute("H") + "\n" + reader.GetAttribute("I");
					
					basicImprovementsList.Add (improvement);
				}
			}
		}
	}
}

public class PlanetInfo
{
	public string planetType, planetCategory;
	public float knowledge, power;
	public int improvementSlots, wealthCost;
}

public class StarSystem
{
	public string systemName, systemOwnedBy, starType;
	public Vector3 systemPosition;
	public GameObject systemObject, allyHero, enemyHero;
	public GameObject tradeRoute;
	public int systemSize, numberOfConnections;
	public float systemDefence, systemOffence;
	public List<Planet> planetsInSystem = new List<Planet> ();
	public List<Node> tempConnections = new List<Node>();
	public List<GameObject> permanentConnections = new List<GameObject>();
	public float sysKnowledgeModifier, sysPowerModifier, sysGrowthModifier, sysMaxPopulationModifier, sysResourceModifier, sysAmberPenalty, sysAmberModifier;
}

public class Planet
{
	public string planetName, planetType, planetCategory, rareResourceType;
	public List<string> improvementsBuilt = new List<string> ();
	public float planetKnowledge, planetPower, planetPopulation, planetCurrentDefence, planetMaxDefence, defenceRegeneration, planetOffence, virusTimer, chillTimer, poisonTimer, chillLength, maxPopulation, expansionPenaltyTimer;
	public bool planetColonised, underEnemyControl, virusActive, chillActive, poisonActive;
	public int planetImprovementLevel, currentImprovementSlots, baseImprovementSlots, wealthValue;
	public float knowledgeModifier, powerModifier, growthModifier, maxPopulationModifier, resourceModifier, amberPenalty, amberModifier;
}

public class BasicImprovement
{
	public string name, category, details, influence;
	public float cost, wealthUpkeep, powerUpkeep;
	public int level;
}

