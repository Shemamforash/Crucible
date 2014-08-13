using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class Triangulation : MasterScript 
{
	public List<Triangle> triangles = new List<Triangle> ();
	private List<Triangle> tempTri = new List<Triangle>();
	private List<GameObject> unvisitedStars = new List<GameObject> ();
	private List<GameObject> externalPoints = new List<GameObject> ();

	public void SimpleTriangulation()
	{
		CacheNearestStars ();
		
		Triangle newTri = new Triangle();
		newTri.points.Add (unvisitedStars [0]);
		newTri.points.Add (unvisitedStars [1]);
		for(int i = 2; i < unvisitedStars.Count; ++i)
		{
			if(MathsFunctions.PointsAreColinear(unvisitedStars[0].transform.position, unvisitedStars[1].transform.position, unvisitedStars[i].transform.position) == false)
			{
				newTri.points.Add (unvisitedStars[i]);
				newTri.lines.Add (MathsFunctions.ABCLineEquation (newTri.points[0].transform.position, newTri.points[1].transform.position));
				newTri.lines.Add (MathsFunctions.ABCLineEquation (newTri.points[1].transform.position, newTri.points[2].transform.position));
				newTri.lines.Add (MathsFunctions.ABCLineEquation (newTri.points[2].transform.position, newTri.points[0].transform.position));
				externalPoints.Add (unvisitedStars [0]);
				externalPoints.Add (unvisitedStars [1]);
				externalPoints.Add (unvisitedStars [i]);
				triangles.Add (newTri);
				unvisitedStars.RemoveAt(i);
				unvisitedStars.RemoveRange(0, 2);
				break;
			}
		}

		for(int i = 0; i < unvisitedStars.Count; ++i) //For all unchecked points
		{
			LinkPointToTris(i);
			CacheTempTris(i);
		}
			
		bool isDelaunay = false;

		while(isDelaunay == false)
		{
			isDelaunay = voronoiGenerator.TriangulationToDelaunay ();
		}

		CheckForNonDelaunayTriangles ();
	}
	
	private void CacheNearestStars()
	{
		for(int i = 0; i < systemListConstructor.systemList.Count; ++i) //Add all systems to a list of unvisited nodes
		{
			unvisitedStars.Add (systemListConstructor.systemList[i].systemObject);
		}

		float theta = 0f;

		while(theta < 360f)
		{
			float xPos = 50f + 60f * Mathf.Cos (theta * Mathf.Deg2Rad);
			float yPos = 50f + 60f * Mathf.Sin (theta * Mathf.Deg2Rad);

			Vector3 newPos = new Vector3 (xPos, yPos, 0f);
			
			GameObject edgePoint = new GameObject();
			edgePoint.name = ((int)(theta / 15f)).ToString();
			edgePoint.transform.position = newPos;
			unvisitedStars.Add (edgePoint); //This adds bounds to the voronoi diagram (forces it to be a circle);
			theta += 15f;
		}
		
		Vector3 centre = new Vector3 (50f, 50f, 0f); //Create centre point at middle of map
		
		for(int j = unvisitedStars.Count; j > 0; --j) //For all unvisited stars
		{
			bool swapsMade = false;
			
			for(int k = 1; k < j; ++k) //While k is less than j (anything above current j value is sorted)
			{
				float distanceA = Vector3.Distance (unvisitedStars[k].transform.position, centre); //Check distance to centre
				float distanceB = Vector3.Distance (unvisitedStars[k - 1].transform.position, centre); //Check distance to centre
				
				if(distanceA < distanceB) //Sort smallest to largest
				{
					GameObject temp = unvisitedStars[k];
					unvisitedStars[k] = unvisitedStars[k - 1];
					unvisitedStars[k - 1] = temp;
					swapsMade = true;
				}
			}
			
			if(swapsMade == false) //If no swaps made, list must have been sorted
			{
				break; //So break
			}
		}
	}

	private void LinkPointToTris(int curPoint) //Send the current unvisited star as the seed point SOMETHING WRONG
	{
		for(int i = 0; i < externalPoints.Count; ++i) //For all the external points
		{
			int nextPoint = i + 1; //Assign the next point

			if(nextPoint == externalPoints.Count) //If the next point
			{
				nextPoint = 0;
			}

			Vector3 lineCurToExternal = MathsFunctions.ABCLineEquation(unvisitedStars[curPoint].transform.position, externalPoints[i].transform.position); //Create a line between the external point and the unvisited star

			if(IsIllegalIntersection(i, curPoint, lineCurToExternal)) //This is where the issue lies
			{
				continue;
			}

			Vector3 lineCurToNextExternal = MathsFunctions.ABCLineEquation(unvisitedStars[curPoint].transform.position, externalPoints[nextPoint].transform.position);

			if(IsIllegalIntersection(nextPoint, curPoint, lineCurToNextExternal))
			{
				continue;
			}

			bool illegal = false;

			for(int j = 0; j < externalPoints.Count; ++j)
			{
				if(j == i || j == nextPoint)
				{
					continue;
				}

				if(MathsFunctions.IsInTriangle(externalPoints[i].transform.position, externalPoints[nextPoint].transform.position, unvisitedStars[curPoint].transform.position, externalPoints[j].transform.position) == true)
				{
					if(MathsFunctions.PointsAreColinear(externalPoints[i].transform.position, externalPoints[nextPoint].transform.position, externalPoints[j].transform.position) == false)
					{
						if(MathsFunctions.PointsAreColinear(externalPoints[i].transform.position, unvisitedStars[curPoint].transform.position, externalPoints[j].transform.position) == false)
						{
							if(MathsFunctions.PointsAreColinear(externalPoints[nextPoint].transform.position, unvisitedStars[curPoint].transform.position, externalPoints[j].transform.position) == false)
							{
								illegal = true;
								break;
							}
						}
					}
				}
			}

			if(illegal)
			{
				continue;
			}

			GameObject pointA = externalPoints[i];
			GameObject pointB = externalPoints[nextPoint];
			GameObject pointC = unvisitedStars[curPoint];

			Triangle newTri = new Triangle();
			newTri.points.Add (pointA);
			newTri.points.Add (pointB);
			newTri.points.Add (pointC);
			newTri.lines.Add (MathsFunctions.ABCLineEquation (pointA.transform.position, pointB.transform.position));
			newTri.lines.Add (MathsFunctions.ABCLineEquation (pointB.transform.position, pointC.transform.position));
			newTri.lines.Add (MathsFunctions.ABCLineEquation (pointC.transform.position, pointA.transform.position));
			tempTri.Add (newTri);
		}
	}

	public List<GameObject> CheckIfSharesSide(Triangle triOne, Triangle triTwo) //Just return the points
	{
		List<GameObject> pointList = new List<GameObject> ();

		int counter = 0;

		for(int i = 0; i < 3; ++i) //For all points in tri one
		{
			for(int j = 0; j < 3; ++j) //For all points in tri two
			{
				if(triOne.points[i].name == triTwo.points[j].name) //If tri one shares a point with tri two
				{
					pointList.Add(triOne.points[i]);
					break;
				}
			}
		}

		if(pointList.Count == 2)
		{
			for(int i = 0; i < 3; ++i)
			{
				if(triOne.points[i].name != pointList[0].name && triOne.points[i].name != pointList[1].name)
				{
					pointList.Add(triOne.points[i]);
				}
				if(triTwo.points[i].name != pointList[0].name && triTwo.points[i].name != pointList[1].name)
				{
					pointList.Add (triTwo.points[i]);
				}
			}
		}
	
		return pointList;
	}

	private void CheckForNonDelaunayTriangles()
	{
		List<Triangle> numberofnondelaunay = new List<Triangle>();

		for(int i = 0; i < triangles.Count; ++i)
		{
			for(int j = 0; j < triangles.Count; ++j)
			{
				if(i == j)
				{
					continue;
				}

				List<GameObject> sharedSides = CheckIfSharesSide(triangles[i], triangles[j]);

				if(sharedSides.Count == 4)
				{
					GameObject sharedPointA = sharedSides[0];
					GameObject sharedPointB = sharedSides[1];
					GameObject unsharedPointA = sharedSides[2];
					GameObject unsharedPointB = sharedSides[3];
					float angleAlpha = 0f, angleBeta = 0f;
					
					angleAlpha = MathsFunctions.AngleBetweenLineSegments (unsharedPointA.transform.position, sharedPointA.transform.position, sharedPointB.transform.position);

					angleBeta = MathsFunctions.AngleBetweenLineSegments (unsharedPointB.transform.position, sharedPointA.transform.position, sharedPointB.transform.position);
					
					Vector3 sharedPointLine = MathsFunctions.ABCLineEquation (sharedPointA.transform.position, sharedPointB.transform.position);
					Vector3 unsharedPointLine = MathsFunctions.ABCLineEquation (unsharedPointA.transform.position, unsharedPointB.transform.position);
					Vector2 intersection = MathsFunctions.IntersectionOfTwoLines (sharedPointLine, unsharedPointLine);
					
					if(MathsFunctions.PointLiesOnLine(sharedPointA.transform.position, sharedPointB.transform.position, intersection) == false) //Is non-convex
					{
						continue;
					}
				
					if(angleBeta + angleAlpha > 180)
					{
						numberofnondelaunay.Add (triangles[i]);
					}
				}
			}
		}

		for(int i = 0; i < numberofnondelaunay.Count; ++i)
		{
			for(int j = 0; j < 3; ++j)
			{
				int a = j;
				int b = j + 1;

				if(j + 1 == 3)
				{
					b = 0;
				}
			}
		}

		Debug.Log(numberofnondelaunay.Count + " | " + triangles.Count);
	}

	private void CacheTempTris(int curPoint)
	{
		for(int i = 0; i < tempTri.Count; ++i)
		{
			triangles.Add (tempTri[i]);
		}

		if(tempTri.Count == 0)
		{
			Debug.Log ("wut" + unvisitedStars[curPoint]);
			Instantiate (systemInvasion.invasionQuad, unvisitedStars[curPoint].transform.position, Quaternion.identity);
		}

		externalPoints.Add (unvisitedStars [curPoint]);
		CheckInteriorPoints ();
		SortExternalPoints ();
		tempTri.Clear ();
	}

	private void SortExternalPoints()
	{
		for(int j = externalPoints.Count; j > 0; --j) //For all unvisited stars
		{
			bool swapsMade = false;
			
			for(int k = 1; k < j; ++k) //While k is less than j (anything above current j value is sorted)
			{
				float angleK = MathsFunctions.RotationOfLine(new Vector3(50f, 50f, 0f), externalPoints[k].transform.position);
				float angleKMinus1 = MathsFunctions.RotationOfLine(new Vector3(50f, 50f, 0f), externalPoints[k - 1].transform.position);

				if(angleK < angleKMinus1) //Sort smallest to largest
				{
					GameObject tempExternal = externalPoints[k];
					externalPoints[k] = externalPoints[k - 1];
					externalPoints[k - 1] = tempExternal;
					swapsMade = true;
				}
			}
			
			if(swapsMade == false) //If no swaps made, list must have been sorted
			{
				break; //So break
			}
		}
	}

	private bool IsIllegalIntersection(int external, int point, Vector3 curToExt)
	{
		for(int j = 0; j < triangles.Count; ++j) //For every triangle
		{
			if(triangles[j].isInternal == true)
			{
				continue;
			}

			for(int k = 0; k < 3; ++k) //For each line in that triangle
			{
				GameObject pointA = null; //Get the points at each end of the line
				GameObject pointB = null;
				
				if(k == 0)
				{
					pointA = triangles[j].points[0];
					pointB = triangles[j].points[1];
				}
				if(k == 1)
				{
					pointA = triangles[j].points[1];
					pointB = triangles[j].points[2];
				}
				if(k == 2)
				{
					pointA = triangles[j].points[2];
					pointB = triangles[j].points[0];
				}

				if(pointA.name == externalPoints[external].name || pointB.name == externalPoints[external].name) //If the line contains the external point we can ignore it
				{
					continue;
				}

				Vector2 intersection = MathsFunctions.IntersectionOfTwoLines(curToExt, triangles[j].lines[k]); //Get the intersection of the line with the current star to external point line
				
				if(MathsFunctions.PointLiesOnLine(externalPoints[external].transform.position, unvisitedStars[point].transform.position, intersection)) //If the point lies elsewhere on the line
				{
					if(MathsFunctions.PointLiesOnLine(pointA.transform.position, pointB.transform.position, intersection))
					{
						return true; //This IS an illegal intersection so return that, otherwise keep checking
					}
				}
			}
		}
		
		return false;
	}

	private void CheckInteriorPoints() //Checks through external point list to see if any points have become internal ones (are within the polygon formed by linking triangles
	{
		List<int> pointsToRemove = new List<int> ();
		
		for(int i = 0; i < externalPoints.Count; ++i)
		{
			float tempAngle = 0;
			
			for(int j = 0; j < triangles.Count; ++j)
			{
				if(triangles[j].isInternal)
				{
					continue;
				}

				for(int k = 0; k < 3; ++k)
				{
					if(triangles[j].points[k] == externalPoints[i])
					{
						int a = 0, b = 0;

						if(k == 0)
						{
							a = 1; b = 2;
						}
						if(k == 1)
						{
							a = 0; b = 2;
						}
						if(k == 2)
						{
							a = 0; b = 1;
						}

						tempAngle += MathsFunctions.AngleBetweenLineSegments(triangles[j].points[k].transform.position, triangles[j].points[a].transform.position, triangles[j].points[b].transform.position);

						break;
					}
				}
			}

			if(tempAngle > 359f)
			{
				pointsToRemove.Add (i);
			}
		}

		for(int j = 0; j < triangles.Count; ++j) //For all triangles
		{
			bool isInternalTri = true; //Say its internal
			
			for(int k = 0; k < 3; ++k) //For all points in that triangle
			{
				if(externalPoints.Contains (triangles[j].points[k])) //If one of them is external
				{
					isInternalTri = false; //The tri isn't internal
				}
			}
			
			if(isInternalTri)
			{
				triangles[j].isInternal = true;
			}
		}

		int counter = 0;

		for(int i = 0; i < pointsToRemove.Count; ++i)
		{
			externalPoints.RemoveAt(pointsToRemove[i] - counter);
			++counter;
		}
		
		for(int i = 0; i < externalPoints.Count; ++i)
		{
			if(externalPoints[i] == null)
			{
				Debug.Log ("whu");
			}
		}
	}
}

public class Triangle
{
	public List<GameObject> points = new List<GameObject>(); //A, B, C points
	public List<Vector3> lines = new List<Vector3>(); //AB, BC, CA line equations
	public bool isInternal = false; //This allows me to ignore any triangles with no external points
}
