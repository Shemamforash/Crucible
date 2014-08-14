using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VoronoiGeneratorAndDelaunay : MasterScript 
{
	public Transform boundaryContainer;
	public Material humansMat, selkiesMat, nereidesMat;
	private List<VoronoiCellVertices> voronoiVertices = new List<VoronoiCellVertices> ();
	private int flips = 0;
	public List<GameObject> voronoiCells = new List<GameObject>();
	public GameObject voronoiCellContainer;

	private class VoronoiCellVertices
	{
		public List<Vector3> vertices = new List<Vector3>();
		public List<float> vertexAngles = new List<float> ();
	}

	public bool CheckIsDelaunay(Triangle triOne, Triangle triTwo)
	{
		List<GameObject> sharedSides = triangulation.CheckIfSharesSide(triOne, triTwo); //Find if triangles share a side (this actually returns the 2 shared and 2 unshared vertices)

		if(sharedSides.Count == 4) //If 4 vertices are shared
		{
			GameObject sharedPointA = sharedSides[0]; //Assign the shared points
			GameObject sharedPointB = sharedSides[1];
			GameObject unsharedPointA = sharedSides[2]; //Assign the unshared points
			GameObject unsharedPointB = sharedSides[3];
			float angleAlpha = 0f, angleBeta = 0f; //Set some angles to 0

			angleAlpha = MathsFunctions.AngleBetweenLineSegments (unsharedPointA.transform.position, sharedPointA.transform.position, sharedPointB.transform.position); //First angle is at one unshared point

			angleBeta = MathsFunctions.AngleBetweenLineSegments (unsharedPointB.transform.position, sharedPointA.transform.position, sharedPointB.transform.position); //Second angle is at the other unshared point

			Vector3 sharedPointLine = MathsFunctions.ABCLineEquation (sharedPointA.transform.position, sharedPointB.transform.position); //Get the line between the shared points
			Vector3 unsharedPointLine = MathsFunctions.ABCLineEquation (unsharedPointA.transform.position, unsharedPointB.transform.position); //Get the line between the unshared points
			Vector2 intersection = MathsFunctions.IntersectionOfTwoLines (sharedPointLine, unsharedPointLine); //Find the intersection of the two lines
			
			if(MathsFunctions.PointLiesOnLine(sharedPointA.transform.position, sharedPointB.transform.position, intersection) == false) //If the intersection does not lie between the shared points then this is a non convex hull
			{
				return true; //So it cannot be flipped, continue to the next triangle
			}

			if(angleAlpha + angleBeta > 180f) //If the polygon is convex, and the two angles combine to be greater than 180 degrees, the triangles are non delaunay, so we flip them
			{				      
				int triPosOne = triangulation.triangles.IndexOf(triOne); //Find the position of the two triangles in the triangles list
				int triPosTwo = triangulation.triangles.IndexOf(triTwo);

				triOne.points[0] = unsharedPointA; //Reassign the vertices of tri one to make the previously unshared vertices the shared vertices. One of the previously shared vertices is now the unshared vertex.
				triOne.points[1] = unsharedPointB;
				triOne.points[2] = sharedPointA;
				triOne.lines[0] = MathsFunctions.ABCLineEquation (triOne.points[0].transform.position, triOne.points[1].transform.position); //Get the line equations for all the sides
				triOne.lines[1] = MathsFunctions.ABCLineEquation (triOne.points[1].transform.position, triOne.points[2].transform.position);
				triOne.lines[2] = MathsFunctions.ABCLineEquation (triOne.points[2].transform.position, triOne.points[0].transform.position);
				triangulation.triangles[triPosOne] = triOne; //Replace the original triangle with this new, flipped triangle

				triTwo.points[0] = unsharedPointA; //Do the same for tri two
				triTwo.points[1] = unsharedPointB;
				triTwo.points[2] = sharedPointB;
				triTwo.lines[0] = MathsFunctions.ABCLineEquation (triTwo.points[0].transform.position, triTwo.points[1].transform.position);
				triTwo.lines[1] = MathsFunctions.ABCLineEquation (triTwo.points[1].transform.position, triTwo.points[2].transform.position);
				triTwo.lines[2] = MathsFunctions.ABCLineEquation (triTwo.points[2].transform.position, triTwo.points[0].transform.position);
				triangulation.triangles[triPosTwo] = triTwo;

				++flips; //Increase the number of flips that have been made this pass.

				return false; //Return false (a flip has been made)
			}
		}

		return true; //Otherwise continue to the next triangle
	}

	public float AngleBetweenLinesOfTri(Triangle tri, int anglePoint) //Anglepoint is the point at which the angle needs to be found (this works)
	{
		float lengthAB = Mathf.Sqrt(Mathf.Pow(tri.points[0].transform.position.x - tri.points[1].transform.position.x, 2f) + Mathf.Pow(tri.points[0].transform.position.y - tri.points[1].transform.position.y, 2f)); //Get the lengths of all 3 sides of the tri
		float lengthBC = Mathf.Sqrt(Mathf.Pow(tri.points[1].transform.position.x - tri.points[2].transform.position.x, 2f) + Mathf.Pow(tri.points[1].transform.position.y - tri.points[2].transform.position.y, 2f));
		float lengthCA = Mathf.Sqrt(Mathf.Pow(tri.points[0].transform.position.x - tri.points[2].transform.position.x, 2f) + Mathf.Pow(tri.points[0].transform.position.y - tri.points[2].transform.position.y, 2f));
		
		float angle = 0f;
		
		if(anglePoint == 0) //The vertex whose angle we are finding dictates the order in which the sides are used in the cos law
		{
			angle = CosLawAngle(lengthBC, lengthCA, lengthAB);
		}
		if(anglePoint == 1)
		{
			angle = CosLawAngle(lengthCA, lengthAB, lengthBC);
		}
		if(anglePoint == 2)
		{
			angle = CosLawAngle(lengthAB, lengthBC, lengthCA);
		}
		
		return angle; //Return the angle at the point required
	}

	private float CosLawAngle(float a, float b, float c) //This uses the cos law aCos(b^2 + c^2 - a^2 / 2bc) = angle at A in radians
	{
		float numerator = (b * b) + (c * c) - (a * a);
		float denominator = 2 * b * c;
		float angleRad = Mathf.Acos (numerator / denominator);
		
		return angleRad * Mathf.Rad2Deg;
	}

	public bool TriangulationToDelaunay() //This takes the triangle list and converts it to a delaunay triangulation
	{
		flips = 0; //Say no flips have been made

		for(int i = 0; i < triangulation.triangles.Count; ++i) //For all triangles
		{
			for(int j = 0; j < triangulation.triangles.Count; ++j) //For all other triangles
			{
				if(i == j)
				{
					continue;
				}
		
				if(CheckIsDelaunay(triangulation.triangles[i], triangulation.triangles[j]) == false) //Check that the triangle is delaunay (must obviously be connected but this is tested in the CheckIsDelaunay method)
				{
					return false; //If it wasn't delaunay then we return false
				}
			}
		}

		if(flips > 0) //If there were any flips we return false
		{
			return false;
		}

		return true; //Otherwise we can verify that the triangulation is Delaunay
	}
	
	public void CreateVoronoiCells() //This appears to be fine
	{
		triangulation.SimpleTriangulation ();

		for(int j = 0; j < systemListConstructor.systemList.Count; ++j)
		{
			VoronoiCellVertices newCell = new VoronoiCellVertices(); //Create new voronoi cell

			Vector3 voronoiCentre = systemListConstructor.systemList[j].systemObject.transform.position;

			for(int i = 0; i < triangulation.triangles.Count; ++i) //For all systems
			{
				if(triangulation.triangles[i].points.Contains(systemListConstructor.systemList[j].systemObject))
				{
					bool looped = false;

					Vector3 systemA = triangulation.triangles[i].points[0].transform.position; //System A equals this system

					Vector3 systemB = triangulation.triangles[i].points[1].transform.position;

					Vector3 systemC = triangulation.triangles[i].points[2].transform.position;

					Vector3 lineAB = MathsFunctions.PerpendicularLineEquation(systemA, systemB);
					Vector3 lineBC = MathsFunctions.PerpendicularLineEquation(systemB, systemC);

					Vector3 voronoiVertex = MathsFunctions.IntersectionOfTwoLines(lineAB, lineBC);

					float angle = MathsFunctions.RotationOfLine(voronoiVertex, voronoiCentre);

					newCell.vertexAngles.Add(angle);
					newCell.vertices.Add (voronoiVertex);

					if(looped)
					{
						break;
					}
				}
			}

			voronoiVertices.Add (newCell);
		}

		for(int i = 0; i < voronoiVertices.Count; ++i)
		{
			for(int j = voronoiVertices[i].vertexAngles.Count; j > 0; --j) //For all unvisited stars
			{
				bool swapsMade = false;
				
				for(int k = 1; k < j; ++k) //While k is less than j (anything above current j value is sorted)
				{
					if(voronoiVertices[i].vertexAngles[k] < voronoiVertices[i].vertexAngles[k - 1]) //Sort smallest to largest
					{
						float tempAng = voronoiVertices[i].vertexAngles[k];
						Vector3 tempPos = voronoiVertices[i].vertices[k];
						voronoiVertices[i].vertexAngles[k] = voronoiVertices[i].vertexAngles[k - 1];
						voronoiVertices[i].vertices[k] = voronoiVertices[i].vertices[k - 1];
						voronoiVertices[i].vertexAngles[k - 1] = tempAng;
						voronoiVertices[i].vertices[k - 1] = tempPos;
						swapsMade = true;
					}
				}
				
				if(swapsMade == false) //If no swaps made, list must have been sorted
				{
					break; //So break
				}
			}

			for(int j = 1; j < voronoiVertices[i].vertices.Count; ++j)
			{
				if(voronoiVertices[i].vertices[j] == voronoiVertices[i].vertices[j - 1])
				{
					voronoiVertices[i].vertices.RemoveAt(j);
					--j;
				}
			}
		}

		for(int i = 0; i < voronoiVertices.Count; ++i)
		{
			GameObject newCell = new GameObject("Voronoi Cell");
			newCell.AddComponent("MeshRenderer");
			newCell.AddComponent("MeshFilter");
			newCell.AddComponent("MeshCollider");
		
			MeshFilter newMesh = (MeshFilter)newCell.GetComponent("MeshFilter");

			List<Vector3> vertices = new List<Vector3>();

			for(int j = 0; j < voronoiVertices[i].vertices.Count; ++j)
			{
				int nextVertex = j + 1;

				if(nextVertex == voronoiVertices[i].vertices.Count)
				{
					nextVertex = 0;
				}

				int prevVertex = j - 1;

				if(prevVertex == -1)
				{
					prevVertex = voronoiVertices[i].vertices.Count - 1;
				}

				Vector3 lineA = voronoiVertices[i].vertices[prevVertex] - voronoiVertices[i].vertices[j]; //Line from vertex to next vertex
				Vector3 lineB = voronoiVertices[i].vertices[nextVertex] - voronoiVertices[i].vertices[j]; //Line from vertex to previous vertex

				Vector3 bisector = (lineA.normalized + lineB.normalized) / 2f; //Bisector line
				float cIntersect = voronoiVertices[i].vertices[j].y - (bisector.y / bisector.x) * voronoiVertices[i].vertices[j].x; //Cintersect = y - mx
				Vector3 bisectorIntersect = new Vector3(0f, cIntersect, 0f);

				float vertAngle = MathsFunctions.AngleBetweenLineSegments(voronoiVertices[i].vertices[j], voronoiVertices[i].vertices[prevVertex], voronoiVertices[i].vertices[nextVertex]) / 2f;

				float hypotenuseLength = 0.25f / Mathf.Sin(vertAngle * Mathf.Deg2Rad);

				Vector3 dir = bisectorIntersect - voronoiVertices[i].vertices[j]; //Intersect
				dir = bisector.normalized;
				vertices.Add (voronoiVertices[i].vertices[j] + (dir * hypotenuseLength));
			}

			for(int j = 0; j < vertices.Count; ++j)
			{
				float x = vertices[j].x - systemListConstructor.systemList[i].systemObject.transform.position.x;
				float y = vertices[j].y - systemListConstructor.systemList[i].systemObject.transform.position.y;
				Vector3 newPos = new Vector3(x, y, 0f);
				vertices[j] = newPos;
			}

			newMesh.mesh.vertices = vertices.ToArray();

			List<int> tris = new List<int>();

			for(int j = 2; j < voronoiVertices[i].vertices.Count; ++j)
			{
				tris.Add (0);
				tris.Add (j);
				tris.Add (j-1);
			}

			List<Vector2> uvs =new List<Vector2>();

			for(int j = 0; j < newMesh.mesh.vertices.Length; ++j) 
			{
				Vector2 uvCoordinate = new Vector2(newMesh.mesh.vertices[j].x, newMesh.mesh.vertices[j].z);
				uvs.Add(uvCoordinate);
			}

			newMesh.mesh.triangles = tris.ToArray();
			newMesh.mesh.uv = uvs.ToArray();

			newMesh.mesh.RecalculateNormals();
			newMesh.mesh.RecalculateBounds();
			newMesh.mesh.Optimize();

			newCell.renderer.material = turnInfoScript.emptyMaterial;
			newCell.renderer.material.shader = Shader.Find("Transparent/Diffuse");
			Color newColour = newCell.renderer.material.color;
			newColour = new Color(newColour.r, newColour.g, newColour.b, 0f);
			newCell.renderer.material.color = newColour;
			newCell.AddComponent<SystemRotate>();
			newCell.name = "Voronoi Cell" + i.ToString();
			newCell.tag = "VoronoiCell";
			newCell.transform.position = new Vector3(systemListConstructor.systemList[i].systemObject.transform.position.x, systemListConstructor.systemList[i].systemObject.transform.position.y, 0f);
			newCell.transform.parent = voronoiCellContainer.transform;

			voronoiCells.Add (newCell);
		}
	}

	public GameObject DrawDebugLine(Vector3 start, Vector3 end, Material mat)
	{
		float distance = Vector3.Distance (start, end);

		Vector3 vectRotation = new Vector3(0.0f, 0.0f, MathsFunctions.RotationOfLine(start, end) - 90f);

		Quaternion rotation = new Quaternion();
		
		rotation.eulerAngles = vectRotation;
		
		Vector3 midPoint = (start + end) / 2; //Get midpoint between target and current system
		
		midPoint = new Vector3 (midPoint.x, midPoint.y, -2.0f); //Create vector from midpoint
		
		lineRenderScript = systemListConstructor.systemList[0].systemObject.GetComponent<LineRenderScript>();
		
		GameObject line = (GameObject)Instantiate(lineRenderScript.line, midPoint, rotation);

		line.renderer.material = mat;

		float width = 0.20f;

		if(mat == turnInfoScript.humansMaterial || mat == turnInfoScript.selkiesMaterial)
		{
			width = 0.1f;
		}
		
		line.transform.localScale = new Vector3(width, distance, 0f);

		return line;
	}
}