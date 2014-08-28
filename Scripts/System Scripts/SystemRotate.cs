using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class SystemRotate : MonoBehaviour
{
	public Vector3 galacticCentre = new Vector3(60f, 60f, 0f);
	public float radius, xPos, yPos, speed, rndSpd1, rndSpd2, rndSpd3;
	public GameObject corona1, corona2, corona3, thisObject;

	public void Start()
	{
		thisObject = gameObject;
		
		if(gameObject.tag == "VoronoiCell")
		{
			string aStr = gameObject.name;
			aStr = aStr.Remove(0, 12);
			thisObject = MasterScript.systemListConstructor.systemList[Convert.ToInt32 (aStr)].systemObject;
		}

		radius = Vector3.Distance (thisObject.transform.position, galacticCentre);
		speed = 0.001f;

		if(gameObject.tag == "StarSystem")
		{
			corona1 = gameObject.transform.Find ("Rotating Objects").transform.Find("Point01").transform.Find ("corona1").gameObject;
			rndSpd1 = UnityEngine.Random.Range (8f, 12f);
			corona2 = gameObject.transform.Find ("Rotating Objects").transform.Find("Point01").transform.Find ("corona2").gameObject;
			rndSpd2 = UnityEngine.Random.Range (8f, 12f);
			corona3 = gameObject.transform.Find ("Rotating Objects").transform.Find("Point01").transform.Find ("corona03").gameObject;
			rndSpd3 = UnityEngine.Random.Range (8f, 12f);
			gameObject.transform.Find ("BorderCloudObject").renderer.sharedMaterial = MasterScript.systemListConstructor.sharedBorderMaterial;
		}
	}

	void Update () //FIXED PLS DONT CHANGE THIS FUTURE SAM
	{
		if(corona1 != null)
		{
			corona1.transform.Rotate (Vector3.forward, Time.deltaTime * rndSpd1);
			corona2.transform.Rotate (Vector3.forward, Time.deltaTime * -rndSpd2);
			corona2.transform.Rotate (Vector3.forward, Time.deltaTime * rndSpd3);
			gameObject.transform.Rotate (Vector3.forward, Time.deltaTime * 5f);
		}

		if(MasterScript.systemListConstructor.loaded == true)
		{
			if(gameObject.tag == "Galaxy")
			{
				Vector3 newRot = new Vector3 (0f, 0f, gameObject.transform.rotation.eulerAngles.z - speed);
				Quaternion rot = new Quaternion();
				rot.eulerAngles = newRot;
				gameObject.transform.rotation = rot;
			}
			else
			{
				UpdateRotation ();
			}
		}
	}

	public void UpdateRotation()
	{
		float angle = -speed * Mathf.Deg2Rad;

		xPos = (float)(Math.Cos(angle) * (thisObject.transform.position.x - galacticCentre.x) - Math.Sin(angle) * (thisObject.transform.position.y - galacticCentre.y) + galacticCentre.x);
		yPos = (float)(Math.Sin(angle) * (thisObject.transform.position.x - galacticCentre.x) + Math.Cos(angle) * (thisObject.transform.position.y - galacticCentre.y) + galacticCentre.y);

		Vector3 newPos = new Vector3 (xPos, yPos, gameObject.transform.position.z);

		gameObject.transform.position = newPos;

		if(gameObject.tag == "VoronoiCell")
		{
			Vector3 newRot = new Vector3 (0f, 0f, gameObject.transform.rotation.eulerAngles.z - speed);
			Quaternion rot = new Quaternion();
			rot.eulerAngles = newRot;
			gameObject.transform.rotation = rot;
		}
	}
}
