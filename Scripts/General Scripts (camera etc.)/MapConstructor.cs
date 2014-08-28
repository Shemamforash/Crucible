using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class MapConstructor : MonoBehaviour
{
	public List<ConnectionCoordinates> coordinateList = new List<ConnectionCoordinates>();
	public float distanceMax;
	public bool connected = false;

	public bool IsValidConnection(GameObject thisSystem, GameObject targetSystem) //Returns true if no intersection
	{
		Vector3 lineA = MathsFunctions.ABCLineEquation(thisSystem.transform.position, targetSystem.transform.position); //Line equation from current system to target system

		Vector3 roughCentre = (thisSystem.transform.position + targetSystem.transform.position) / 2f;

		if(roughCentre.x < 65f && roughCentre.x > 55f) //If the intersection exists in the galactic core, ignore it
		{
			if(roughCentre.y < 65f && roughCentre.y > 55f)
			{
				return false;
			}
		}

		for (int i = 0; i < coordinateList.Count; ++i) //For all existing connections
		{
			if(coordinateList[i].systemOne == thisSystem && coordinateList[i].systemTwo == targetSystem) //If a connection already exists between the current system and the target system, continue to the next connection
			{
				continue;
			}
			if(coordinateList[i].systemTwo == thisSystem && coordinateList[i].systemOne == targetSystem) //Continuation of above
			{
				continue;
			}
			if(coordinateList[i].systemOne == thisSystem || coordinateList[i].systemTwo == thisSystem) //If the connection contains this system it will not make intersections with the temporary connection
			{
				continue;
			}
			if(coordinateList[i].systemOne == targetSystem || coordinateList[i].systemTwo == targetSystem) //If the connection contains the target system it will not make intersections with the temporary connection
			{
				continue;
			}

			//if(CheckIfIntersectionCouldOccur(thisSystem, targetSystem, coordinateList[i].systemOne, coordinateList[i].systemTwo) == false)
			//{
			//	continue;
			//

			Vector3 lineB = MathsFunctions.ABCLineEquation(coordinateList[i].systemOne.transform.position, coordinateList[i].systemTwo.transform.position); //Get the line equation between of the connection

			Vector2 intersection = MathsFunctions.IntersectionOfTwoLines(lineA, lineB); //Find the intersection of the two lines
		
			if(intersection == Vector2.zero) //If the lines are parallel the method returns a zero vector continue to the next connection
			{
				continue;
			}

			if(MathsFunctions.PointLiesOnLine(thisSystem.transform.position, targetSystem.transform.position, intersection)) //If the intersection lies on the temporary connection
			{
				if(MathsFunctions.PointLiesOnLine(coordinateList[i].systemOne.transform.position, coordinateList[i].systemTwo.transform.position, intersection)) //And it lies on the current permanent connection
				{
					return false; //Return true, an intersection does exist
				}
			}
		}
			
		return true;
	}

	private bool CheckIfIntersectionCouldOccur(GameObject systemA1, GameObject systemB1, GameObject systemA2, GameObject systemB2) //Checks to see if the bounding boxes could intersect- optimisation method
	{
		Vector2 boxACorner1 = new Vector2(systemA1.transform.position.x, systemA1.transform.position.y); //Top left
		Vector2 boxACorner2 = new Vector2(systemB1.transform.position.x, systemA1.transform.position.y); //Top right
		Vector2 boxACorner3 = new Vector2(systemB1.transform.position.x, systemB1.transform.position.y); //Bottom right
		Vector2 boxACorner4 = new Vector2(systemA1.transform.position.x, systemB1.transform.position.y); //Bottom left

		Vector2[] boxACorners = new Vector2[4] {boxACorner1, boxACorner2, boxACorner3, boxACorner4}; //Create array containing all corners

		for(int i = 0; i < 4; ++i) //For all corners
		{
			if(boxACorners[i].x > Mathf.Min(systemA2.transform.position.x, systemB2.transform.position.x) && boxACorners[i].x < Mathf.Max (systemA2.transform.position.x, systemB2.transform.position.x)) //If it lies within the other bounding box x values
			{
				if(boxACorners[i].y > Mathf.Min(systemA2.transform.position.y, systemB2.transform.position.y) && boxACorners[i].y < Mathf.Max (systemA2.transform.position.y, systemB2.transform.position.y)) //If it also lies within the other bounding box's y values
				{
					return true; //An intersection could occur
				}
			}
		}

		return false; //Otherwise an intersection could not occur
	}

	public bool IsValidAngle(GameObject thisSystem, GameObject targetSystem) //Returns true if angle within limits
	{
		int current = MasterScript.RefreshCurrentSystem (thisSystem);

		for(int i = 0; i < MasterScript.systemListConstructor.systemList[current].permanentConnections.Count; ++i)
		{
			float angleA = MathsFunctions.AngleBetweenLineSegments(thisSystem.transform.position, targetSystem.transform.position, MasterScript.systemListConstructor.systemList[current].permanentConnections[i].transform.position);
			float angleB = MathsFunctions.AngleBetweenLineSegments(targetSystem.transform.position, thisSystem.transform.position, MasterScript.systemListConstructor.systemList[current].permanentConnections[i].transform.position);
			
			if(angleA <= 10.0f || angleB <= 10.0f)
			{
				return false;
			}
		}

		return true;
	}

	private void AddInitialConnections() //This creates connections for the galactic centre so that nothing intersects with it
	{
		Vector3[] centreCorners = new Vector3[4] {new Vector3(55f, 55f, 0f), new Vector3(55f, 65f, 0f), new Vector3(65f, 65f, 0f), new Vector3(65f, 55f, 0f)};  

		for(int i = 0; i < 4; ++i)
		{
			int prev = i - 1;

			if(prev < 0)
			{
				prev = 3;
			}

			ConnectionCoordinates connection = new ConnectionCoordinates (); //Create new connection object

			GameObject sysOne = new GameObject();
			sysOne.transform.position = centreCorners[i];
			GameObject sysTwo = new GameObject();
			sysTwo.transform.position = centreCorners[prev];

			connection.systemOne = sysOne; //Assign the relevant data
			connection.systemTwo = sysTwo;

			coordinateList.Add (connection); //Add the object to the list of connections
		}
	}

	public void DrawMinimumSpanningTree() //Working
	{
		AddInitialConnections();

		List<GameObject> linkedSystems = new List<GameObject> (); //Create empty list of linkedsystems
		List<GameObject> unlinkedSystems = new List<GameObject> (); //Create empty list of unlinked systems
		
		linkedSystems.Add (MasterScript.systemListConstructor.systemList [0].systemObject); //Add initial system to list

		for (int i = 1; i < MasterScript.systemListConstructor.systemList.Count; ++i)  //For all other systems
		{
			unlinkedSystems.Add (MasterScript.systemListConstructor.systemList[i].systemObject); //Add them to the unlinked system list
		}
		
		for(int i = 0; i < MasterScript.systemListConstructor.systemList.Count; ++i) //For all systems in the game
		{
			float tempDistance = 400.0f; //Reset variables
			GameObject nearestSystem = null;
			GameObject thisSystem = null;

			for(int j = 0; j < linkedSystems.Count; ++j) //For all linked systems
			{
				for(int k = 0; k < unlinkedSystems.Count; ++k) //For all unlinked systems
				{
					if(IsValidConnection(linkedSystems[j], unlinkedSystems[k]) == false || IsValidAngle(linkedSystems[j], unlinkedSystems[k]) == false) //If no valid connection can be formed
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

		for(int i = 0; i < MasterScript.systemListConstructor.systemList.Count; ++i)
		{
			if(MasterScript.systemListConstructor.systemList[i].numberOfConnections < MasterScript.systemListConstructor.systemList[i].permanentConnections.Count)
			{
				MasterScript.systemListConstructor.systemList[i].numberOfConnections = MasterScript.systemListConstructor.systemList[i].permanentConnections.Count;
			}
		}

		AssignMaximumConnections (); //Assign the maximum connections

		for(int i = 0; i < MasterScript.systemListConstructor.systemList.Count; ++i) //For all systems
		{
			SortConnectionsByAngle(i); //Sort the connections by angle
		}

		for(int i = 0; i < MasterScript.systemListConstructor.systemList.Count; ++i)
		{
			for(int j = 0; j < MasterScript.systemListConstructor.systemList[i].permanentConnections.Count; ++j)
			{
				if(IsValidConnection(MasterScript.systemListConstructor.systemList[i].systemObject, MasterScript.systemListConstructor.systemList[i].permanentConnections[j]) == false)
				{
					Debug.Log ("bacon");
				}
			}
		}
	}

	private void SortConnectionsByAngle (int i) //Sorts the permanent connections of a system into clockwise order from first added connection
	{
		Vector3 zeroVector = MasterScript.systemListConstructor.systemList [i].systemObject.transform.position; //Centre which other points are relevant to

		for(int j = MasterScript.systemListConstructor.systemList[i].permanentConnections.Count; j > 0; --j) //For all connections
		{
			bool swapsMade = false; //Say no swaps have been made

			for(int k = 1; k < j; ++k) //Sort smallest to largest
			{
				float angleK = MathsFunctions.RotationOfLine(MasterScript.systemListConstructor.systemList[i].permanentConnections[k].transform.position, zeroVector); //Get angle between centre and this point
				float angleKMinusOne = MathsFunctions.RotationOfLine(MasterScript.systemListConstructor.systemList[i].permanentConnections[k - 1].transform.position, zeroVector);

				if(angleK < angleKMinusOne)
				{
					GameObject temp = MasterScript.systemListConstructor.systemList[i].permanentConnections[k];
					MasterScript.systemListConstructor.systemList[i].permanentConnections[k] = MasterScript.systemListConstructor.systemList[i].permanentConnections[k - 1];
					MasterScript.systemListConstructor.systemList[i].permanentConnections[k - 1] = temp;
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
		int thisSystem = MasterScript.systemListConstructor.RefreshCurrentSystemA(current); //Get integer location of system in systemList
		int nearestSystem = MasterScript.systemListConstructor.RefreshCurrentSystemA(target); //Same for target

		MasterScript.systemListConstructor.systemList[thisSystem].permanentConnections.Add (MasterScript.systemListConstructor.systemList[nearestSystem].systemObject); //Add target system to current systems' permanent connections
		MasterScript.systemListConstructor.systemList [nearestSystem].permanentConnections.Add (MasterScript.systemListConstructor.systemList [thisSystem].systemObject); //Add current system to target systems' permanent connections
		
		ConnectionCoordinates connection = new ConnectionCoordinates (); //Create new connection object

		connection.systemOne = MasterScript.systemListConstructor.systemList [thisSystem].systemObject; //Assign the relevant data
		connection.systemTwo = MasterScript.systemListConstructor.systemList [nearestSystem].systemObject;

		coordinateList.Add (connection); //Add the object to the list of connections

		for(int i = 0; i < MasterScript.systemListConstructor.systemList[thisSystem].tempConnections.Count; ++i) //For all temporary connections in the current system
		{
			if(MasterScript.systemListConstructor.systemList[thisSystem].tempConnections[i].targetSystem == MasterScript.systemListConstructor.systemList[nearestSystem].systemObject) //If the target system is found in the current systems temporary connections
			{
				MasterScript.systemListConstructor.systemList[thisSystem].tempConnections.RemoveAt(i); //Remove it
				break; //And break the loop
			}
		}

		for(int i = 0; i < MasterScript.systemListConstructor.systemList[nearestSystem].tempConnections.Count; ++i) //Same as above for the target system
		{
			if(MasterScript.systemListConstructor.systemList[nearestSystem].tempConnections[i].targetSystem == MasterScript.systemListConstructor.systemList[thisSystem].systemObject)
			{
				MasterScript.systemListConstructor.systemList[nearestSystem].tempConnections.RemoveAt (i);
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
		for(int i = 0; i < MasterScript.systemListConstructor.systemList.Count; ++i) //For all systems
		{
			int lowerBound = 0;

			if(MasterScript.systemListConstructor.systemList[i].systemName == "Samael" || MasterScript.systemListConstructor.systemList[i].systemName == "Midgard" || MasterScript.systemListConstructor.systemList[i].systemName == "Nephthys")
			{
				lowerBound = 49;
			}

			int randomInt = WeightedConnectionFinder(Random.Range (lowerBound, 100)); //Generate number

			if(MasterScript.systemListConstructor.systemList[i].numberOfConnections < randomInt) //If number of connections is lower than number
			{
				MasterScript.systemListConstructor.systemList[i].numberOfConnections = randomInt; //Increase number of connections
			}

			for(int j = 0; j < MasterScript.systemListConstructor.systemList.Count; ++j) //For all other systems
			{
				if(i == j || MasterScript.systemListConstructor.systemList[i].permanentConnections.Contains(MasterScript.systemListConstructor.systemList[j].systemObject)) //If the j iterator equals the i iterator or the current system already contains the j iterator system, continue
				{
					continue;
				}

				float distance = Vector3.Distance (MasterScript.systemListConstructor.systemList[i].systemObject.transform.position, MasterScript.systemListConstructor.systemList[j].systemObject.transform.position); //Assign distance

				if(distance < distanceMax) //If the distance between these two systems is less than the maximum distance
				{
					Node nearbySystem = new Node(); //Create a new node for the system
						
					nearbySystem.targetSystem = MasterScript.systemListConstructor.systemList[j].systemObject; //Assign the object
					nearbySystem.targetDistance = distance; //And the distance
						
					MasterScript.systemListConstructor.systemList[i].tempConnections.Add (nearbySystem); //And add it to the temporary connections
				}
			}
		}

		SortNearestConnections();
		ConnectSystems();
	}
	
	private void SortNearestConnections()
	{
		GameObject tempObject = null;
		float tempFloat = 0;
		
		for(int i = 0; i < MasterScript.systemListConstructor.systemList.Count; ++i) //For all systems
		{
			for(int j = MasterScript.systemListConstructor.systemList[i].tempConnections.Count - 1; j >= 0; --j)
			{
				bool swaps = false;
				
				for(int k = 1; k <= j; ++k) //Sort smallest to largest
				{
					if(MasterScript.systemListConstructor.systemList[i].tempConnections[k-1].targetDistance > MasterScript.systemListConstructor.systemList[i].tempConnections[k].targetDistance)
					{
						tempObject = MasterScript.systemListConstructor.systemList[i].tempConnections[k-1].targetSystem;
						tempFloat = MasterScript.systemListConstructor.systemList[i].tempConnections[k-1].targetDistance;

						MasterScript.systemListConstructor.systemList[i].tempConnections[k-1].targetSystem = MasterScript.systemListConstructor.systemList[i].tempConnections[k].targetSystem;
						MasterScript.systemListConstructor.systemList[i].tempConnections[k-1].targetDistance = MasterScript.systemListConstructor.systemList[i].tempConnections[k].targetDistance;
						
						MasterScript.systemListConstructor.systemList[i].tempConnections[k].targetSystem = tempObject;
						MasterScript.systemListConstructor.systemList[i].tempConnections[k].targetDistance = tempFloat;
						
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
		for(int j = 0; j < MasterScript.systemListConstructor.systemList.Count; ++j) //For all systems
		{
			for(int l = 0; l < MasterScript.systemListConstructor.systemList[j].tempConnections.Count; ++l) //For all temporary connections of that system
			{
				if(MasterScript.systemListConstructor.systemList[j].numberOfConnections <= MasterScript.systemListConstructor.systemList[j].permanentConnections.Count) //If the permanent connections count equals the maximum number of connections ignore it
				{
					break;
				}

				int targetSystem = MasterScript.systemListConstructor.RefreshCurrentSystemA(MasterScript.systemListConstructor.systemList[j].tempConnections[l].targetSystem); //Find systemList iterator value of target system

				if(MasterScript.systemListConstructor.systemList[targetSystem].numberOfConnections <= MasterScript.systemListConstructor.systemList[targetSystem].permanentConnections.Count) //If that systems permanent connections are full
				{
					continue; //Continue to the next system
				}

				if(IsValidAngle(MasterScript.systemListConstructor.systemList[j].systemObject, MasterScript.systemListConstructor.systemList[targetSystem].systemObject)) //If the connection would be within the valid angle range
				{
					if(IsValidConnection(MasterScript.systemListConstructor.systemList[j].systemObject, MasterScript.systemListConstructor.systemList[targetSystem].systemObject)) //And has a valid connection
					{		
						AddPermanentSystem(MasterScript.systemListConstructor.systemList[j].systemObject, MasterScript.systemListConstructor.systemList[targetSystem].systemObject); //Add it to the permanent system list of the target systems
					}
				}
			}
		}
		
		for(int i = 0; i < MasterScript.systemListConstructor.systemList.Count; ++i)
		{
			if(MasterScript.systemListConstructor.systemList[i].permanentConnections.Count != MasterScript.systemListConstructor.systemList[i].numberOfConnections)
			{
				MasterScript.systemListConstructor.systemList[i].numberOfConnections = MasterScript.systemListConstructor.systemList[i].permanentConnections.Count;
			}
		}

		connected = true;
	}
}

public class ConnectionCoordinates
{
	public GameObject systemOne, systemTwo;
}

public class Node
{
	public GameObject targetSystem;
	public float targetDistance;
}
