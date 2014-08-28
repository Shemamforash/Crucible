using UnityEngine;
using System.Collections;

public class BoundaryFadeScript : MonoBehaviour 
{
	private Color reference, start, end;
	private bool fade, initialised, lerp;
	private float timer, t;

	void Start()
	{
		reference = gameObject.renderer.material.color;
		Debug.Log (gameObject.name);
	}

	void Update () 
	{
		if(lerp == true)
		{
			gameObject.renderer.material.color = Color.Lerp (start, end, t);
			t += Time.deltaTime / 0.25f;
		}

		if(MasterScript.systemPopup.mainCamera.transform.position.z > -65f && fade == false)
		{
			if(lerp == false)
			{
				start = new Color (reference.r, reference.g, reference.b, 1.0f);
				end = new Color (reference.r, reference.g, reference.b, 0.0f);
				lerp = true;
			}

			if(timer == 0f)
			{
				timer = Time.time;
				t = 0f;
			}

			if(timer + 0.25f < Time.time)
			{
				gameObject.renderer.material.color = end;
				fade = true;
				lerp = false;
				timer = 0f;
			}
		}

		if(MasterScript.systemPopup.mainCamera.transform.position.z <= -65f && fade == true)
		{
			if(lerp == false)
			{
				start = new Color (reference.r, reference.g, reference.b, 0.0f);
				end = new Color (reference.r, reference.g, reference.b, 1.0f);
				lerp = true;
			}

			if(timer == 0f)
			{
				timer = Time.time;
				t = 0f;
			}
			
			if(timer + 0.25f < Time.time)
			{
				gameObject.renderer.material.color = end;
				fade = false;
				lerp = false;
				timer = 0f;
			}
		}
	}
}
