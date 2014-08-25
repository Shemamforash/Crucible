using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class MapConstructor : MasterScript
{
	public List<ConnectionCoordinates> coordinateList = new List<ConnectionCoordinates>();
	public float distanceMax;
	public bool connected = false;

	public bool IsValidConnection(Vector3 thisSystem, Vector3 targetSystem) //Returns true if no intersection
	{
		Vector3 lineA = MathsFunctions.ABCLineEquation(thisSystem, targetSystem); //Line equation from current system to target system

		for (int i = 0; i < coordinateList.Count; ++i) //For all existing connections
		{
			if(coordinateList[i].systemA == thisSystem && coordinateList[i].systemB == targetSystem) //If a connection already exists between the current system and the target system, continue to the next connection
			{
				continue;
			}
			if(coordinateList[i].systemB == thisSystem && coordinateList[i].systemA == targetSystem) //Continuation of above
			{
				continue;
			}
			if(coordinateList[i].systemA == thisSystem || coordinateList[i].systemB == thisSystem) //If the connection contains this system it will not make intersections with the temporary connection
			{
				continue;
			}
			if(coordinateList[i].systemA == targetSystem || coordinateList[i].systemB == targetSystem) //If the connection contains the target system it will not make intersections with the temporary connection
			{
				continue;
			}

			Vector3 lineB = coordinateList[i].lineEquation; //Get the line equation between of the connection

			Vector2 intersection = MathsFunctions.IntersectionOfTwoLines(lineA, lineB); //Find the intersection of the two lines
		
			if(intersection == Vector2.zero) //If the lines are parallel the method returns a zero vector, so we can return false (they do not cross)
			{
				return true;
			}

			if(MathsFunctions.PointLiesOnLine(thisSystem, targetSystem, intersection)) //If the intersection lies on the temporary connection
			{
				if(MathsFunctions.PointLiesOnLine(coordinateList[i].systemA, coordinateList[i].systemB, intersection)) //And it lies on the current permanent connection
				{
					return false; //Return true, an intersection does exist
				}
			}

			if(intersection.x < 55f && intersection.x > 45f) //If the intersection exists in the galactic core, ignore it
			{
				if(intersection.y < 55f && intersection.y > 45f)
				{
					return false;
				}
			}
		}
			
		return true;
	}

	public bool IsValidAngle(GameObject thisSystem, GameObject targetSystem) //Returns true if angle within limits
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
		List<GameObject> unlinkedSystems = new List<GameObject> (); //Create empty list of unlinked systems
		
		linkedSystems.Add (systemListConstructor.systemList [0].systemObject); //Add initial system to list

		for (int i = 1; i < systemListConstructor.systemList.Count; ++i)  //For all other systems
		{
			unlinkedSystems.Add (systemListConstructor.systemList[i].systemObject); //Add them to the unlinked system list
		}
		
		for(int i = 0; i < systemListConstructor.systemList.Count; ++i) //For all systems in the game
		{
			float tempDistance = 400.0f; //Reset variables
			GameObject nearestSystem = null;
			GameObject thisSystem = null;

			for(int j = 0; j < linkedSystems.Count; ++j) //For all linked systems
			{
				for(int k = 0; k < unlinkedSystems.Count; ++k) //For all unlinked systems
				{
					if(IsValidConnection(linkedSystems[j].transform.position, unlinkedSystems[k].transform.position) == false || IsValidAngle(linkedSystems[j], unlinkedSystems[k]) == false) //If no valid connection can be formed
					{
						continue; //Continue to the next unlinked system
					}
					
					float distance = Vector3.Distance(linkedSystems[j].transform.position, unlinkedSystems[k].transform.position); //Get the distance between the unlinked and linked system
					
					if(distance < tempDistance) //If it is less than the previously cached system
					{
						tempDistance = distance; //Cache the new system as the nearest valid system
						nearestSystem = unlinkedSystems[k];
						thisSystem = linkedSystems[j];
					}
				}
			}
			
			if(thisSystem != null) //If a nearest system has been found/the graph is not complete
			{
				AddPermanentSystem(thisSystem, nearestSystem); //Add the system to the graph

				linkedSystems.Add (nearestSystem); //Add nearest unlinked system to linkedsystems list
				unlinkedSystems.Remove (nearestSystem); //Remove the system from the unlinked system list
			}
		}

		AssignMaximumConnections (); //Assign the maximum connections

		for(int i = 0; i < systemListConstructor.systemList.Count; ++i) //For all systems
		{
			SortConnectionsByAngle(i); //Sort the connections by angle
		}
	}

	private void SortConnectionsByAngle (int i) //Sorts the permanent connections of a system into clockwise order from first added connection
	{
		Vector3 zeroVector = systemListConstructor.systemList [i].systemObject.transform.position; //Centre which other points are relevant to

		for(int j = systemListConstructor.systemList[i].permanentConnections.Count; j > 0; --j) //For all connections
		{
			bool swapsMade = false; //Say no swaps have been made

			for(int k = 1; k < j; ++k) //Sort smallest to largest
			{
				float angleK = MathsFunctions.RotationOfLine(systemListConstructor.systemList[i].permanentConnections[k].transform.position, zeroVector); //Get angle between centre and this point
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

	private void AddPermanentSystem(GameObject current, GameObject target) //Connects the two systems
	{
		int thisSystem = systemListConstructor.RefreshCurrentSystemA(current); //Get integer location of system in systemList
		int nearestSystem = systemListConstructor.RefreshCurrentSystemA(target); //Same for target

		systemListConstructor.systemList[thisSystem].permanentConnections.Add (systemListConstructor.systemList[nearestSystem].systemObject); //Add target system to current systems' permanent connections
		systemListConstructor.systemList [nearestSystem].permanentConnections.Add (systemListConstructor.systemList [thisSystem].systemObject); //Add current system to target systems' permanent connections
		
		ConnectionCoordinates connection = new ConnectionCoordinates (); //Create new connection object

		connection.systemOne = systemListConstructor.systemList [thisSystem].systemObject; //Assign the relevant data
		connection.systemTwo = systemListConstructor.systemList [nearestSystem].systemObject;
		connection.systemA = systemListConstructor.systemList [thisSystem].systemObject.transform.position;
		connection.systemB = systemListConstructor.systemList [nearestSystem].systemObject.transform.position;
		connection.lineEquation = MathsFunctions.ABCLineEquation(connection.systemA, connection.systemB);
		
		coordinateList.Add (connection); //Add the object to the list of connections

		for(int i = 0; i < systemListConstructor.systemList[thisSystem].tempConnections.Count; ++i) //For all temporary connections in the current system
		{
			if(systemListConstructor.systemList[thisSystem].tempConnections[i].targetSystem == systemListConstructor.systemList[nearestSystem].systemObject) //If the target system is found in the current systems temporary connections
			{
				systemListConstructor.systemList[thisSystem].tempConnections.RemoveAt(i); //Remove it
				break; //And break the loop
			}
		}

		for(int i = 0; i < systemListConstructor.systemList[nearestSystem].tempConnections.Count; ++i) //Same as above for the target system
		{
			if(systemListConstructor.systemList[nearestSystem].tempConnections[i].targetSystem == systemListConstructor.systemList[thisSystem].systemObject)
			{
				systemListConstructor.systemList[nearestSystem].tempConnections.RemoveAt (i);
				break;
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
			int lowerBound = 0;

			if(systemListConstructor.systemList[i].systemName == "Samael" || systemListConstructor.systemList[i].systemName == "Midgard" || systemListConstructor.systemList[i].systemName == "Nephthys")
			{
				lowerBound = 49;
			}

			int randomInt = WeightedConnectionFinder(Random.Range (lowerBound, 100)); //Generate number

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
					if(systemListConstructor.systemList[i].tempConnections[k-1].targetDistance > systemListConstructor.systemList[i].tempConnections[k].targetDistance)
					{
						tempObject = systemListConstructor.systemList[i].tempConnections[k-1].targetSystem;
						tempFloat = systemListConstructor.systemList[i].tempConnections[k-1].targetDistance;

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
			if(systemListConstructor.systemList[j].numberOfConnections == systemListConstructor.systemList[j].permanentConnections.Count) //If the permanent connections count equals the maximum number of connections ignore it
			{
				continue;
			}

			for(int l = 0; l < systemListConstructor.systemList[j].tempConnections.Count; ++l) //For all temporary connections of that system
			{
				int targetSystem = systemListConstructor.RefreshCurrentSystemA(systemListConstructor.systemList[j].tempConnections[l].targetSystem); //Find systemList iterator value of target system

				if(systemListConstructor.systemList[targetSystem].numberOfConnections <= systemListConstructor.systemList[targetSystem].permanentConnections.Count) //If that systems permanent connections are full
				{
					continue; //Continue to the next system
				}

				if(IsValidAngle(systemListConstructor.systemList[j].systemObject, systemListConstructor.systemList[targetSystem].systemObject)) //If the connection would be within the valid angle range
				{
					if(IsValidConnection(systemListConstructor.systemList[j].systemObject.transform.position, systemListConstructor.systemList[targetSystem].systemObject.transform.position)) //And has a valid connection
					{		
						AddPermanentSystem(systemListConstructor.systemList[j].systemObject, systemListConstructor.systemList[targetSystem].systemObject); //Add it to the permanent system list of the target systems
						--l;
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
	public Vector3 systemA, systemB, lineEquation;
}

public class Node
{
	public GameObject targetSystem;
	public float targetDistance;
}
