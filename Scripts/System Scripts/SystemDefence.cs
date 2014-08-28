using UnityEngine;
using System.Collections;

public class SystemDefence : MonoBehaviour 
{
	public int system, regenerateTimer;
	public float maxSystemDefence, defenceRegenerator;
	public bool underInvasion, regenerated = true;
	private SystemSIMData systemSIMData;

	void Start () 
	{
		systemSIMData = gameObject.GetComponent<SystemSIMData> ();
		CalculateSystemDefence ();
		system = MasterScript.RefreshCurrentSystem (gameObject);
	}

	public void TakeDamage(float assaultDamage, float auxiliaryDamage, int planet)
	{
		if(planet == -1)
		{
			MasterScript.systemListConstructor.systemList[system].systemDefence -= assaultDamage;
		}
		else
		{
			MasterScript.systemListConstructor.systemList[system].planetsInSystem[planet].planetCurrentDefence -= assaultDamage;
			MasterScript.systemListConstructor.systemList[system].planetsInSystem[planet].planetPopulation -= auxiliaryDamage;
		}
	}

	public void CalculateSystemDefence() 
	{
		maxSystemDefence = 0f;

		for(int i = 0; i < MasterScript.systemListConstructor.systemList[system].systemSize; ++i)
		{
			maxSystemDefence += MasterScript.systemListConstructor.systemList[system].planetsInSystem[i].planetImprovementLevel + 1 * 50f;
		}

		maxSystemDefence = (maxSystemDefence / MasterScript.systemListConstructor.systemList [system].systemSize) * 20f;
		MasterScript.systemListConstructor.systemList [system].systemOffence = (int)(maxSystemDefence / 2f);

		defenceRegenerator = maxSystemDefence / 5f;

		if(underInvasion == false)
		{
			if(regenerateTimer > 0)
			{
				--regenerateTimer;
			}

			for(int i = 0; i < MasterScript.systemListConstructor.systemList[system].systemSize; ++i)
			{
				CalculatePlanetDefence(i);
			}

			if(regenerateTimer <= 0 && MasterScript.systemListConstructor.systemList [system].systemDefence != maxSystemDefence)
			{
				regenerateTimer = 0;

				MasterScript.systemListConstructor.systemList [system].systemDefence += (int)defenceRegenerator;

				if(MasterScript.systemListConstructor.systemList [system].systemDefence >= maxSystemDefence)
				{
					regenerated = true;
				}
				else
				{
					regenerated = false;
				}
			}
			
			if(regenerated == true)
			{
				MasterScript.systemListConstructor.systemList [system].systemDefence = (int)maxSystemDefence;
			}
		}
	}

	public void CalculatePlanetDefence(int planet)
	{
		float maxPlanetDefence = ((MasterScript.systemListConstructor.systemList[system].planetsInSystem[planet].planetImprovementLevel) * (MasterScript.systemListConstructor.systemList[system].planetsInSystem[planet].planetImprovementLevel) * 20) + 50f;
		MasterScript.systemListConstructor.systemList [system].planetsInSystem[planet].planetOffence = maxPlanetDefence;

		MasterScript.systemListConstructor.systemList[system].planetsInSystem[planet].defenceRegeneration = defenceRegenerator/10f;
		MasterScript.systemListConstructor.systemList [system].planetsInSystem [planet].planetMaxDefence = maxPlanetDefence;

		if(MasterScript.systemListConstructor.systemList[system].planetsInSystem[planet].planetColonised == true && MasterScript.systemListConstructor.systemList[system].planetsInSystem[planet].planetCurrentDefence != maxPlanetDefence)
		{
			MasterScript.systemListConstructor.systemList[system].planetsInSystem[planet].planetCurrentDefence += defenceRegenerator/10f;

			if(MasterScript.systemListConstructor.systemList[system].planetsInSystem[planet].planetCurrentDefence > maxPlanetDefence)
			{
				MasterScript.systemListConstructor.systemList[system].planetsInSystem[planet].planetCurrentDefence = maxPlanetDefence;
			}
		}
	}

	public void CheckStatusEffects(int planet)
	{
		systemSIMData.knowledgeBuffModifier = 1.0f;
		systemSIMData.powerBuffModifier = 1.0f;

		if(MasterScript.systemListConstructor.systemList[system].planetsInSystem[planet].chillActive == true)
		{
			Chill (0, planet);
		}
		if(MasterScript.systemListConstructor.systemList[system].planetsInSystem[planet].virusActive == true)
		{
			Virus (planet);
		}
		if(MasterScript.systemListConstructor.systemList[system].planetsInSystem[planet].poisonActive == true)
		{
			Poison(planet);
		}
	}

	private void Virus(int planet)
	{
		if(MasterScript.systemListConstructor.systemList[system].planetsInSystem[planet].virusTimer == 0.0f)
		{
			MasterScript.systemListConstructor.systemList[system].planetsInSystem[planet].virusTimer = Time.time;
		}

		float timeDifference = Time.time - MasterScript.systemListConstructor.systemList[system].planetsInSystem[planet].virusTimer;

		if(timeDifference > 600f)
		{
			timeDifference = 600f;
		}

		timeDifference = (timeDifference - 300f) / 191f;

		float sinDifference = Mathf.Sin(timeDifference) + 1;

		systemSIMData.knowledgeBuffModifier = systemSIMData.knowledgeBuffModifier * sinDifference; // y = sin((x-300)/191) + 1
		systemSIMData.powerBuffModifier = systemSIMData.powerBuffModifier * sinDifference;

		for(int i = 0; i < MasterScript.systemListConstructor.systemList[system].permanentConnections.Count; ++i)
		{
			int j = MasterScript.RefreshCurrentSystem(MasterScript.systemListConstructor.systemList[system].permanentConnections[i]);

			bool skipSystem = false;

			for(int k = 0; k < MasterScript.systemListConstructor.systemList[j].systemSize; ++k)
			{
				if(MasterScript.systemListConstructor.systemList[system].planetsInSystem[k].virusActive == true)
				{
					skipSystem = true;
					break;
				}
			}

			if(skipSystem == false)
			{
				for(int k = 0; k < MasterScript.systemListConstructor.systemList[j].systemSize; ++k)
				{
					if(MasterScript.systemListConstructor.systemList[system].planetsInSystem[k].virusActive == false)
					{
						float ratio = Mathf.Max(MasterScript.systemListConstructor.systemList[system].planetsInSystem[k].planetCurrentDefence, maxSystemDefence) / 
										Mathf.Min (MasterScript.systemListConstructor.systemList[system].planetsInSystem[k].planetCurrentDefence, maxSystemDefence);

						if(ratio * 2 < sinDifference)
						{
							MasterScript.systemListConstructor.systemList[system].planetsInSystem[k].virusActive = true;
							continue;
						}
					}
				}
			}
		}
	}

	private void Chill(int newLength, int planet)
	{
		if(newLength != 0.0f)
		{
			MasterScript.systemListConstructor.systemList[system].planetsInSystem[planet].chillLength = MasterScript.systemListConstructor.systemList[system].planetsInSystem[planet].chillLength + newLength;
		}

		if(MasterScript.systemListConstructor.systemList[system].planetsInSystem[planet].chillTimer == 0.0f && MasterScript.systemListConstructor.systemList[system].planetsInSystem[planet].chillLength != 0.0f)
		{
			MasterScript.systemListConstructor.systemList[system].planetsInSystem[planet].chillTimer = Time.time;
		}

		if(MasterScript.systemListConstructor.systemList[system].planetsInSystem[planet].chillTimer + MasterScript.systemListConstructor.systemList[system].planetsInSystem[planet].chillLength >= Time.time)
		{
			systemSIMData.knowledgeBuffModifier = systemSIMData.knowledgeBuffModifier * 0.5f;
			systemSIMData.powerBuffModifier = systemSIMData.powerBuffModifier * 0.5f;
		}

		if(MasterScript.systemListConstructor.systemList[system].planetsInSystem[planet].chillTimer + MasterScript.systemListConstructor.systemList[system].planetsInSystem[planet].chillLength < Time.time)
		{
			MasterScript.systemListConstructor.systemList[system].planetsInSystem[planet].chillLength = 0.0f;
			MasterScript.systemListConstructor.systemList[system].planetsInSystem[planet].chillTimer = 0.0f;
			MasterScript.systemListConstructor.systemList[system].planetsInSystem[planet].chillActive = false;
		}
	}

	private void Poison(int planet)
	{
		if(MasterScript.systemListConstructor.systemList[system].planetsInSystem[planet].poisonTimer == 0.0f)
		{
			MasterScript.systemListConstructor.systemList[system].planetsInSystem[planet].poisonTimer = Time.time;
		}

		if(MasterScript.systemListConstructor.systemList[system].planetsInSystem[planet].poisonTimer + 2.0f < Time.time)
		{
			MasterScript.systemListConstructor.systemList[system].planetsInSystem[planet].poisonTimer = MasterScript.systemListConstructor.systemList[system].planetsInSystem[planet].poisonTimer + 2.0f;

			for(int i = 0; i < MasterScript.systemListConstructor.systemList[system].systemSize; ++i)
			{
				float population = MasterScript.systemListConstructor.systemList[system].planetsInSystem[i].planetPopulation;

				MasterScript.systemListConstructor.systemList[system].planetsInSystem[i].planetPopulation -= (population / 40f) + 0.5f;
			}
		}
	}
}
