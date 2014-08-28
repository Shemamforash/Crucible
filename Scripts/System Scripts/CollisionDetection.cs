using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CollisionDetection : MonoBehaviour 
{
	private int connectionIterator;
	private bool reconnected = false;

	private List<GameObject> unvisitedSystems = new List<GameObject> ();
	private List<GameObject> firmSystems = new List<GameObject> ();

	void Update () //FIXED PLS DONT CHANGE THIS FUTURE SAM
	{
		UpdateConnections ();

		if(reconnected)
		{
			//CheckForGraphComplete ();
			reconnected =false;
		}
	}

	private void CheckVisitedSystems()
	{
		bool foundSystem = false;
		
		for(int i = 0; i < firmSystems.Count; ++i) //For all current systems
		{
			int sys = MasterScript.RefreshCurrentSystem (firmSystems [i]);
			
			for(int j = 0; j < MasterScript.systemListConstructor.systemList[sys].permanentConnections.Count; ++j) //Check the systems permanent connections
			{
				if(firmSystems.Contains(MasterScript.systemListConstructor.systemList[sys].permanentConnections[j])) //If the system is already in firm systems, ignore it
				{
					continue;
				}
				
				firmSystems.Add (MasterScript.systemListConstructor.systemList[sys].permanentConnections[j]); //Add systems to firm systems
				unvisitedSystems.Remove (MasterScript.systemListConstructor.systemList[sys].permanentConnections[j]); //Remove the system from unvisited systems
				
				foundSystem = true; //Found a system
			}
		}

		if(foundSystem == true) //If system is found
		{
			CheckVisitedSystems(); //Repeat
		}
	}

	private void CheckForGraphComplete()
	{
		unvisitedSystems.Clear (); //Clear lists
		firmSystems.Clear ();

		for(int i = 1; i < MasterScript.systemListConstructor.systemList.Count; ++i) //For all systems
		{
			unvisitedSystems.Add (MasterScript.systemListConstructor.systemList[i].systemObject); //Add them to the unvisited systems
		}

		firmSystems.Add (MasterScript.systemListConstructor.systemList[0].systemObject); //Add the first system to firm systems

		CheckVisitedSystems (); //Start the cycle of checking for systems

		if(firmSystems.Count != MasterScript.systemListConstructor.systemList.Count) //If the size of firm systems is not equal to the systemlist, this means that not all systems are connected
		{
			float distance = 10000f;
			int sysToJoinA = -1, sysToJoinB = -1;

			for(int i = 0; i < firmSystems.Count; ++i)
			{
				for(int j = 0; j < unvisitedSystems.Count; ++j)
				{
					float tempDistance = Vector3.Distance (firmSystems[i].transform.position, unvisitedSystems[j].transform.position);

					if(tempDistance < distance)
					{
						if(MasterScript.mapConstructor.IsValidConnection(firmSystems[i], unvisitedSystems[j]))
						{
							distance = tempDistance;
							sysToJoinA = i;
							sysToJoinB = j;
						}
					}
				}
			}

			if(sysToJoinA != -1 && sysToJoinB != -1)
			{
				sysToJoinA = MasterScript.RefreshCurrentSystem(firmSystems[sysToJoinA]);
				sysToJoinB = MasterScript.RefreshCurrentSystem(unvisitedSystems[sysToJoinB]);

				MasterScript.systemListConstructor.systemList[sysToJoinA].permanentConnections.Add (MasterScript.systemListConstructor.systemList[sysToJoinB].systemObject);
				MasterScript.systemListConstructor.systemList[sysToJoinA].numberOfConnections = MasterScript.systemListConstructor.systemList[sysToJoinA].permanentConnections.Count;
				MasterScript.systemListConstructor.systemList[sysToJoinB].permanentConnections.Add (MasterScript.systemListConstructor.systemList[sysToJoinA].systemObject);
				MasterScript.systemListConstructor.systemList[sysToJoinB].numberOfConnections = MasterScript.systemListConstructor.systemList[sysToJoinB].permanentConnections.Count;
			}
		}
	}

	private void UpdateConnections()
	{
		for(int i = 0; i < MasterScript.systemListConstructor.systemList[connectionIterator].permanentConnections.Count; ++i) //For all connections
		{
			bool validConnection = true;

			if(MasterScript.mapConstructor.IsValidConnection(MasterScript.systemListConstructor.systemList[connectionIterator].systemObject, MasterScript.systemListConstructor.systemList[connectionIterator].permanentConnections[i]) == false)
			{
				validConnection = false;
			}

			if(validConnection == false)
			{
				GameObject target = MasterScript.systemListConstructor.systemList[connectionIterator].permanentConnections[i];

				if(ReconnectSystems(MasterScript.systemListConstructor.systemList[connectionIterator].systemObject, target))
				{
					ReconnectSystems(target, MasterScript.systemListConstructor.systemList[connectionIterator].systemObject);
					RemoveConnection(connectionIterator, MasterScript.RefreshCurrentSystem(target));
				}
			}
		}

		connectionIterator++;

		if(connectionIterator == MasterScript.systemListConstructor.systemList.Count)
		{
			connectionIterator = 0;
		}
	}

	private void RemoveConnection(int current, int target)
	{
		GameObject systemA = MasterScript.systemListConstructor.systemList[current].systemObject;
		GameObject systemB = MasterScript.systemListConstructor.systemList[target].systemObject;

		for(int i = 0; i < MasterScript.mapConstructor.coordinateList.Count; ++i)
		{
			if((systemA == MasterScript.mapConstructor.coordinateList[i].systemOne && systemB == MasterScript.mapConstructor.coordinateList[i].systemTwo) ||
			   (systemA == MasterScript.mapConstructor.coordinateList[i].systemTwo && systemB == MasterScript.mapConstructor.coordinateList[i].systemOne))
			{
				MasterScript.mapConstructor.coordinateList.RemoveAt(i);
				break;
			}
		}

		for(int i = 0; i < MasterScript.systemListConstructor.systemList[current].permanentConnections.Count; ++i)
		{
			if(MasterScript.systemListConstructor.systemList[current].permanentConnections[i] == MasterScript.systemListConstructor.systemList[target].systemObject)
			{
				MasterScript.systemListConstructor.systemList[current].permanentConnections.RemoveAt(i);
				break;
			}
		}

		for(int i = 0; i < MasterScript.systemListConstructor.systemList[target].permanentConnections.Count; ++i)
		{
			if(MasterScript.systemListConstructor.systemList[target].permanentConnections[i] == MasterScript.systemListConstructor.systemList[current].systemObject)
			{
				MasterScript.systemListConstructor.systemList[target].permanentConnections.RemoveAt(i);
				break;
			}
		}
	}

	public bool ReconnectSystems(GameObject systemA, GameObject systemB)
	{
		int thisSystem = MasterScript.RefreshCurrentSystem (systemA); //Get this system
		reconnected = true;
		List<GameObject> potentialConnections = new List<GameObject>();
		float distance = 100f; //Set max distance
		int newConnection = -1;
		
		for(int i = 0; i < MasterScript.systemListConstructor.systemList[thisSystem].permanentConnections.Count; ++i) //For all permanent connections in this system
		{
			if(MasterScript.systemListConstructor.systemList[thisSystem].permanentConnections[i] == systemB) //If connection equals target
			{
				for(int j = 0; j < MasterScript.systemListConstructor.systemList.Count; ++j) //For all other systems
				{
					if(MasterScript.systemListConstructor.systemList[j].systemObject == systemA || MasterScript.systemListConstructor.systemList[j].systemObject == systemB) //If system equals either of original systems, continue
					{
						continue;
					}

					float tempDistance = Vector3.Distance (systemA.transform.position, MasterScript.systemListConstructor.systemList[j].systemObject.transform.position);
					
					if(tempDistance < distance) //Test if distance is less than current max distance
					{
						potentialConnections.Add (MasterScript.systemListConstructor.systemList[j].systemObject);
					}
				}

				GameObject tempObject = new GameObject();
	
				for(int k = potentialConnections.Count - 1; k >= 0; --k)
				{
					bool swaps = false;
					
					for(int l = 1; l <= k; ++l) //Sort smallest to largest
					{
						float lMinus1Dist = Vector3.Distance(systemA.transform.position, potentialConnections[l-1].transform.position);
						float lDist = Vector3.Distance(systemA.transform.position, potentialConnections[l].transform.position);

						if(lMinus1Dist > lDist)
						{
							tempObject = potentialConnections[l-1];
							potentialConnections[l-1] = potentialConnections[l];
							potentialConnections[l] = tempObject;
							swaps = true;
						}
					}
					
					if(swaps == false)
					{
						break;
					}
				}

				for(int j = 0; j < potentialConnections.Count; ++j)
				{
					if(MasterScript.mapConstructor.IsValidAngle(systemA, potentialConnections[j])) //If the connection would be within the valid angle parameters
					{
						if(MasterScript.mapConstructor.IsValidConnection(systemA, potentialConnections[j])) //If there is no intersection between this system and other system
						{
							newConnection = MasterScript.RefreshCurrentSystem(potentialConnections[j]);
							break; 
						}
					}
				}

				if(newConnection != -1) //If the new connection is not -1
				{
					MasterScript.systemListConstructor.systemList[thisSystem].permanentConnections[i] = MasterScript.systemListConstructor.systemList[newConnection].systemObject; //The connection that was system b is now the new connection
					MasterScript.systemListConstructor.systemList[newConnection].permanentConnections.Add(MasterScript.systemListConstructor.systemList[thisSystem].systemObject); //The new connection is now connected to this system

					ConnectionCoordinates temp = new ConnectionCoordinates();
					
					temp.systemOne = MasterScript.systemListConstructor.systemList[thisSystem].systemObject;
					temp.systemTwo = MasterScript.systemListConstructor.systemList[newConnection].systemObject;

					MasterScript.mapConstructor.coordinateList.Add (temp);
					
					MasterScript.systemListConstructor.systemList[thisSystem].numberOfConnections = MasterScript.systemListConstructor.systemList[thisSystem].permanentConnections.Count;
					MasterScript.systemListConstructor.systemList[newConnection].numberOfConnections = MasterScript.systemListConstructor.systemList[newConnection].permanentConnections.Count;

					return true;
				}
				
				break;
			}
		}
		return false;
	}
}
