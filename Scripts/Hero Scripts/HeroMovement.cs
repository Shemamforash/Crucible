using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeroMovement : MasterScript 
{
	public bool heroIsMoving;
	private int currentVertex;

	private List<AStarNode> openList = new List<AStarNode> ();
	private List<AStarNode> closedList = new List<AStarNode> ();
	private List<GameObject> finalPath = new List<GameObject> ();
	private GameObject start, target;

	private Vector3 targetPosition = Vector3.zero, currentPosition;

	void Start () 
	{
		heroScript = gameObject.GetComponent<HeroScriptParent> ();
	}

	void Update()
	{
		if(target != null)
		{
			heroIsMoving = true;
			RefreshHeroLocation();
		}

		if(target == null)
		{
			heroIsMoving = false;
		}
	}

	public void FindPath(GameObject begin, GameObject end)
	{
		openList.Clear ();
		closedList.Clear ();
		finalPath.Clear ();

		start = begin;
		target = end;
		
		AStarNode node = new AStarNode ();
		
		node.system = start;
		node.parent = null;
		node.g = 0f; 
		node.h = Vector3.Distance(start.transform.position, target.transform.position);
		node.f = node.g;
		
		closedList.Add (node);
		finalPath.Add (start);
		
		int i = 0;
		
		while(finalPath.Contains(target) == false)
		{
			GetNearestNode(i);
			++i;
		}

		finalPath.Clear ();

		node = closedList[closedList.Count - 1];

		while(finalPath.Contains(start) == false)
		{
			finalPath.Add (node.system);
			node = node.parent;
		}

		finalPath.Reverse ();

		currentVertex = 0;
	}

	private void GetNearestNode(int curNode)
	{
		int sys = RefreshCurrentSystem (closedList [curNode].system); //Current system is the last node on the closed list
		
		for(int i = 0; i < systemListConstructor.systemList[sys].permanentConnections.Count; ++i) //For all permanent connections
		{
			bool skip = false;
			
			for(int j = 0; j < closedList.Count; ++j) //If system is already on the closed list, continue
			{
				if(systemListConstructor.systemList[sys].permanentConnections[i] == closedList[j].system)
				{
					skip = true;
				}
			}
			
			if(skip == false)
			{
				AStarNode node = new AStarNode (); //New node
				
				node.system = systemListConstructor.systemList[sys].permanentConnections[i]; //Node system is connected to this system

				float tempG = 0f, tempH = 0f;
				int replaceNode = -1;
				bool containsNode = false;

				for(int j = 0; j < openList.Count; ++j) //For all other nodes in the open list
				{
					if(openList[j].system == node.system && node.parent != null) //If the selected node is already on the open list
					{
						float gToParent = openList[j].g; //Get it's g distance to parent
						float gThroughCurrent = closedList[curNode].g + Vector3.Distance(node.system.transform.position, closedList[curNode].system.transform.position); //And it's g through the current node
						
						if(gThroughCurrent < gToParent) //If the route through the current node is less than its current g
						{
							tempG = gThroughCurrent; 
							tempH = Vector3.Distance(node.system.transform.position, target.transform.position); 
							replaceNode = j;
						}

						containsNode = true;
					}
				}

				if(replaceNode != -1)
				{
					node.g = tempG; //Change it's g value
					node.h = tempH; //And it's h value
					node.f = node.g + node.h; //And f value
					node.parent = closedList[curNode];  //Set it's parent to the current node
					openList[replaceNode] = node; //Replace it in the open list
					continue;
				}

				if(containsNode == false)
				{
					node.parent = closedList[curNode]; //Set the parent to the current node
					node.g = closedList[curNode].g + Vector3.Distance(node.system.transform.position, closedList[curNode].system.transform.position); //Calculate g
					node.h = Vector3.Distance(node.system.transform.position, target.transform.position); //Calculate h
					node.f = node.g + node.h; //Calculate f
					openList.Add (node); //Add it to the open list
				}
			}
		}
		
		float temp = 10000f;
		int nodeToPick = -1;
		
		for(int i = 0; i < openList.Count; ++i) //For all the nodes in the open list
		{
			if(openList[i].f < temp) //If the node's f value is less than the temp f value
			{
				temp = openList[i].f; //Set the temp f to this node's f
				nodeToPick = i; //Set this node to be the next to join the closed list
			}
		}

		if(nodeToPick != -1)
		{
			closedList.Add (openList [nodeToPick]); //Add node to closed list
			finalPath.Add (openList [nodeToPick].system); //You can also add it to the final path as the closed list does not change
			openList.RemoveAt (nodeToPick); //Remove it from the open list- we will not be backtracking to already assigned nodes
		}
	}

	public void RefreshHeroLocation()
	{
		if(finalPath[currentVertex] != target) //If current system does not equal the destination system
		{
			currentPosition = gameObject.transform.position; //Current hero position is updated

			targetPosition = HeroPositionAroundStar (finalPath[currentVertex + 1]); //Target position is set

			if(TestForProximity(currentPosition, targetPosition) == true) //If current hero position is equal to the next system on route
			{	
				systemSIMData = finalPath[currentVertex].GetComponent<SystemSIMData>();
				systemDefence = finalPath[currentVertex].GetComponent<SystemDefence>();

				systemDefence.underInvasion = false;
				systemDefence.regenerateTimer = 3;
				heroScript.isInvading = false;
				systemListConstructor.systemList[currentVertex].enemyHero = null;

				++currentVertex; //Update current system
				
				heroScript.heroLocation = finalPath[currentVertex]; //Set herolocation to current system

				heroShip = gameObject.GetComponent<HeroShip>();

				targetPosition = Vector3.zero;
			}

			else
			{
				gameObject.transform.position = Vector3.MoveTowards (currentPosition, targetPosition, (10 * heroScript.movementSpeed) * Time.deltaTime);
			}
		}

		if(TestForProximity(currentPosition, HeroPositionAroundStar(target)) == true)
		{
			heroScript.heroLocation = target;
			target = null;
		}
	}

	public bool TestForProximity(Vector3 current, Vector3 target)
	{
		if(current.x <= target.x + 0.1f && current.x >= target.x - 0.1f)
		{
			if(current.y <= target.y + 0.1f && current.y >= target.y - 0.1f)
			{
				return true;
			}
		}

		return false;
	}

	public Vector3 HeroPositionAroundStar(GameObject location)
	{
		Vector3 position = new Vector3();

		heroScript = gameObject.GetComponent<HeroScriptParent> ();

		switch(heroScript.heroOwnedBy)
		{
		case "Humans":
			position.x = location.transform.position.x;
			position.y = location.transform.position.y + 2.5f;
			break;
		case "Selkies":
			position.x = location.transform.position.x + 2.5f;
			position.y = location.transform.position.y - 1.66f;
			break;
		case "Nereides":
			position.x = location.transform.position.x - 2.5f;
			position.y = location.transform.position.y - 1.66f;
			break;
		default:
			break;
		}

		position.z = location.transform.position.z;
		
		return position;
	}

	private class AStarNode
	{
		public GameObject system;
		public AStarNode parent;
		public float f, g, h;
	}
}


