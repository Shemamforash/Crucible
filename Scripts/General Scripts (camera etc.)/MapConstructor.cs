using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class MapConstructor : MasterScript
{
	public List<ConnectionCoordinates> coordinateList = new List<ConnectionCoordinates>();
	public float distanceMax;
	public bool connected = false;
	public List<int> allIntersections = new List<int>();

	public bool TestForIntersection(Vector3 thisSystem, Vector3 targetSystem, bool includeIntersections)
	{
		allIntersections.Clear ();
		bool intersects = false;

		Vector3 lineA = MathsFunctions.ABCLineEquation(thisSystem, targetSystem);

		for (int i = 0; i < coordinateList.Count; ++i) 
		{
			if(coordinateList[i].systemA == thisSystem && coordinateList[i].systemB == targetSystem)
			{
				continue;
			}
			if(coordinateList[i].systemB == thisSystem && coordinateList[i].systemA == targetSystem)
			{
				continue;
			}
			if(coordinateList[i].systemA == thisSystem || coordinateList[i].systemB == thisSystem)
			{
				continue;
			}

			Vector3 lineB = MathsFunctions.ABCLineEquation(coordinateList[i].systemA, coordinateList[i].systemB);

			Vector2 intersection = MathsFunctions.IntersectionOfTwoLines(lineA, lineB);

			if(intersection == Vector2.zero)
			{
				return false;
			}

			if(MathsFunctions.PointLiesOnLine(thisSystem, targetSystem, intersection))
			{
				if(MathsFunctions.PointLiesOnLine(coordinateList[i].systemA, coordinateList[i].systemB, intersection))
				{
					if(includeIntersections == true)
					{
						allIntersections.Add (i);
					}

					intersects = true;
				}
			}

			if(intersection.x < 65f && intersection.x > 50f)
			{
				if(intersection.y < 65f && intersection.y > 50f)
				{
					intersects = true;
				}
			}
		}

		if(intersects == true)
		{
			return true;
		}

		return false;
	}

	public bool TestForAngle(GameObject thisSystem, GameObject targetSystem)
	{
		int current = RefreshCurrentSystem (thisSystem);
		int target = RefreshCurrentSystem (targetSystem);

		Vector3 directionVector1 = targetSystem.transform.position - thisSystem.transform.position;

		for(int i = 0; i < systemListConstructor.systemList[current].permanentConnections.Count; ++i)
		{
			Vector3 directionVector2 = systemListConstructor.systemList[current].permanentConnections[i].transform.position - thisSystem.transform.position;

			float angleA = MathsFunctions.AngleBetweenLineSegments(thisSystem.transform.position, targetSystem.transform.position, systemListConstructor.systemList[current].permanentConnections[i].transform.position);
			float angleB = MathsFunctions.AngleBetweenLineSegments(targetSystem.transform.position, thisSystem.transform.position, systemListConstructor.systemList[current].permanentConnections[i].transform.position);
			
			if(angleA <= 10.0f || angleB <= 10.0f)
			{
				return false;
			}
		}

		return true;
	}
	
	public void DrawMinimumSpanningTree() //Working
	{
		List<GameObject> linkedSystems = new List<GameObject> (); //Create empty list of linkedsystems
		List<GameObject> unlinkedSystems = new List<GameObject> ();
		
		linkedSystems.Add (systemListConstructor.systemList [0].systemObject); //Add initial system to list

		for (int i = 1; i < systemListConstructor.systemList.Count; ++i) 
		{
			unlinkedSystems.Add (systemListConstructor.systemList[i].systemObject);
		}
		
		for(int i = 0; i < systemListConstructor.systemList.Count; ++i) //For all systems in the game
		{
			float tempDistance = 400.0f; //Reset variables
			int nearestSystem = -1;
			int thisSystem = -1;
			bool toLink = false;
			
			for(int j = 0; j < linkedSystems.Count; ++j) //For all linked systems
			{
				int system = systemListConstructor.RefreshCurrentSystemA(linkedSystems[j]);
				
				for(int k = 0; k < unlinkedSystems.Count; ++k) //For all unlinked systems
				{
					if(TestForIntersection(linkedSystems[j].transform.position, unlinkedSystems[k].transform.position, false) == true && TestForAngle(linkedSystems[j], unlinkedSystems[k]) == false)
					{
						continue;
					}
					
					float distance = Vector3.Distance(linkedSystems[j].transform.position, unlinkedSystems[k].transform.position);
					
					if(distance < tempDistance) //Find the nearest unlinked system
					{
						tempDistance = distance;
						nearestSystem = systemListConstructor.RefreshCurrentSystemA(unlinkedSystems[k]);
						thisSystem = system;
						toLink = true;
					}
				}
			}
			
			if(toLink == true)
			{
				AddPermanentSystem(thisSystem, nearestSystem);

				linkedSystems.Add (systemListConstructor.systemList[nearestSystem].systemObject); //Add nearest unlinked system to linkedsystems list
				unlinkedSystems.Remove (systemListConstructor.systemList[nearestSystem].systemObject);
			}
		}
		
		for(int i = 0; i < systemListConstructor.systemList.Count; ++i)
		{
			systemListConstructor.systemList[i].numberOfConnections = systemListConstructor.systemList[i].permanentConnections.Count;

			SortConnectionsByAngle(i);
		}

		AssignMaximumConnections ();
	}

	private void SortConnectionsByAngle (int i) //Sorts the permanent connections of a system into clockwise order from first added connection
	{
		Vector3 zeroVector = systemListConstructor.systemList [i].permanentConnections [0].transform.position;

		for(int j = systemListConstructor.systemList[i].permanentConnections.Count; j > 0; --j)
		{
			bool swapsMade = false;

			for(int k = 1; k < j; ++k)
			{
				float angleK = MathsFunctions.RotationOfLine(systemListConstructor.systemList[i].permanentConnections[k].transform.position, zeroVector);
				float angleKMinusOne = MathsFunctions.RotationOfLine(systemListConstructor.systemList[i].permanentConnections[k - 1].transform.position, zeroVector);

				if(angleK < angleKMinusOne)
				{
					GameObject temp = systemListConstructor.systemList[i].permanentConnections[k];
					systemListConstructor.systemList[i].permanentConnections[k] = systemListConstructor.systemList[i].permanentConnections[k - 1];
					systemListConstructor.systemList[i].permanentConnections[k - 1] = temp;
					swapsMade = true;
				}
			}

			if(swapsMade == false)
			{
				break;
			}
		}
	}

	private void AddPermanentSystem(int thisSystem, int nearestSystem)
	{
		systemListConstructor.systemList[thisSystem].permanentConnections.Add (systemListConstructor.systemList[nearestSystem].systemObject); //Add target system to current systems permanent connections
		systemListConstructor.systemList [nearestSystem].permanentConnections.Add (systemListConstructor.systemList [thisSystem].systemObject);
		
		ConnectionCoordinates connection = new ConnectionCoordinates ();

		connection.systemOne = systemListConstructor.systemList [thisSystem].systemObject;
		connection.systemTwo = systemListConstructor.systemList [nearestSystem].systemObject;
		connection.systemA = systemListConstructor.systemList [thisSystem].systemObject.transform.position;
		connection.systemB = systemListConstructor.systemList [nearestSystem].systemObject.transform.position;
		
		coordinateList.Add (connection);

		bool thisSystemRemove = false;
		bool targetSystemRemove = false;

		for(int i = 0; i < systemListConstructor.systemList[thisSystem].tempConnections.Count; ++i)
		{
			if(systemListConstructor.systemList[thisSystem].tempConnections[i].targetSystem == systemListConstructor.systemList[nearestSystem].systemObject && thisSystemRemove == false)
			{
				systemListConstructor.systemList[thisSystem].tempConnections.RemoveAt(i);
				thisSystemRemove = true;
				continue;
			}
		}

		for(int i = 0; i < systemListConstructor.systemList[nearestSystem].tempConnections.Count; ++i)
		{
			if(systemListConstructor.systemList[nearestSystem].tempConnections[i].targetSystem == systemListConstructor.systemList[thisSystem].systemObject && targetSystemRemove == false)
			{
				systemListConstructor.systemList[nearestSystem].tempConnections.RemoveAt (i);
				targetSystemRemove = true;
				continue;
			}
		}
	}
	
	private int WeightedConnectionFinder(int randomInt)
	{
		if(randomInt < 10)
		{
			return 1;
		}
		if(randomInt >= 10 && randomInt < 20)
		{
			return 2;
		}
		if(randomInt >= 20 && randomInt < 40)
		{
			return 3;
		}
		if(randomInt >= 40 && randomInt < 60)
		{
			return 4;
		}
		if(randomInt >= 60 && randomInt < 80)
		{
			return 5;
		}
		if(randomInt >= 80 && randomInt < 90)
		{
			return 6;
		}
		if(randomInt >= 90)
		{
			return 7;
		}
		
		return 0;
	}
	
	private void AssignMaximumConnections()
	{
		for(int i = 0; i < systemListConstructor.systemList.Count; ++i) //For all systems
		{
			int randomInt = WeightedConnectionFinder(Random.Range (0,100)); //Generate number
			
			if(systemListConstructor.systemList[i].systemName == "Samael" || systemListConstructor.systemList[i].systemName == "Midgard" || systemListConstructor.systemList[i].systemName == "Nephthys")
			{
				randomInt = WeightedConnectionFinder(Random.Range (49, 99));
			}

			if(systemListConstructor.systemList[i].numberOfConnections < randomInt) //If number of connections is lower than number
			{
				systemListConstructor.systemList[i].numberOfConnections = randomInt; //Increase number of connections
			}
			
			for(int j = 0; j < systemListConstructor.systemList.Count; ++j) //For all systems
			{
				if(i == j)
				{
					continue;
				}
				
				bool skipSystem = false;
				
				float distance = Vector3.Distance (systemListConstructor.systemList[i].systemObject.transform.position, systemListConstructor.systemList[j].systemObject.transform.position); //Assign distance
				
				for(int k = 0; k < systemListConstructor.systemList[i].permanentConnections.Count; ++k) //For all of this systems permanent connections
				{
					if(systemListConstructor.systemList[i].permanentConnections[k] == systemListConstructor.systemList[j].systemObject) //If target systems is already in permanent connections, continue;
					{
						skipSystem = true;
						break;
					}
				}
				
				if(skipSystem == true)
				{
					continue;
				}

				if(distance < distanceMax)
				{
					Node nearbySystem = new Node();
						
					nearbySystem.targetSystem = systemListConstructor.systemList[j].systemObject;
					nearbySystem.targetDistance = distance;
						
					systemListConstructor.systemList[i].tempConnections.Add (nearbySystem);
				}

			}
		}

		SortNearestConnections();
		ConnectSystems();
	}
	
	private void SortNearestConnections()
	{
		GameObject tempObject;
		float tempFloat;
		
		for(int i = 0; i < systemListConstructor.systemList.Count; ++i)
		{
			for(int j = systemListConstructor.systemList[i].tempConnections.Count - 1; j >= 0; --j)
			{
				bool swaps = false;
				
				for(int k = 1; k <= j; ++k)
				{
					tempObject = systemListConstructor.systemList[i].tempConnections[k-1].targetSystem;
					tempFloat = systemListConstructor.systemList[i].tempConnections[k-1].targetDistance;
					
					if(systemListConstructor.systemList[i].tempConnections[k-1].targetDistance > systemListConstructor.systemList[i].tempConnections[k].targetDistance)
					{
						systemListConstructor.systemList[i].tempConnections[k-1].targetSystem = systemListConstructor.systemList[i].tempConnections[k].targetSystem;
						systemListConstructor.systemList[i].tempConnections[k-1].targetDistance = systemListConstructor.systemList[i].tempConnections[k].targetDistance;
						
						systemListConstructor.systemList[i].tempConnections[k].targetSystem = tempObject;
						systemListConstructor.systemList[i].tempConnections[k].targetDistance = tempFloat;
						
						swaps = true;
					}
				}
				
				if(swaps == false)
				{
					break;
				}
			}
		}
	}
	
	private void ConnectSystems()
	{
		for(int j = 0; j < systemListConstructor.systemList.Count; ++j) //For all systems
		{
			for(int l = 0; l < systemListConstructor.systemList[j].tempConnections.Count; ++l) //For all tempconnections
			{
				if(systemListConstructor.systemList[j].numberOfConnections == systemListConstructor.systemList[j].permanentConnections.Count)
				{
					break;
				}

				int targetSystem = systemListConstructor.RefreshCurrentSystemA(systemListConstructor.systemList[j].tempConnections[l].targetSystem); //Get target system

				if(systemListConstructor.systemList[targetSystem].numberOfConnections == systemListConstructor.systemList[targetSystem].permanentConnections.Count)
				{
					continue;
				}

				if(TestForAngle(systemListConstructor.systemList[j].systemObject, systemListConstructor.systemList[targetSystem].systemObject))
				{
					if(TestForIntersection(systemListConstructor.systemList[j].systemObject.transform.position, systemListConstructor.systemList[targetSystem].systemObject.transform.position, false) == false)
					{		
						AddPermanentSystem(j, targetSystem);
					}
				}
			}
		}
		
		for(int i = 0; i < systemListConstructor.systemList.Count; ++i)
		{
			if(systemListConstructor.systemList[i].permanentConnections.Count != systemListConstructor.systemList[i].numberOfConnections)
			{
				systemListConstructor.systemList[i].numberOfConnections = systemListConstructor.systemList[i].permanentConnections.Count;
			}
		}

		connected = true;
	}
}

public class ConnectionCoordinates
{
	public GameObject systemOne, systemTwo;
	public Vector3 systemA, systemB;
}

public class Node
{
	public GameObject targetSystem;
	public float targetDistance;
}
