using UnityEngine;
using System.Collections;

public class SystemDefence : MasterScript 
{
	public int system, regenerateTimer;
	public float maxSystemDefence, defenceRegenerator;
	public bool underInvasion, canEnter, regenerated = true;

	void Start () 
	{
		systemSIMData = gameObject.GetComponent<SystemSIMData> ();
		CalculateSystemDefence ();
		system = RefreshCurrentSystem (gameObject);
	}

	public void TakeDamage(float assaultDamage, float auxiliaryDamage, int planet)
	{
		if(planet == -1)
		{
			systemListConstructor.systemList[system].systemDefence -= assaultDamage;
		}
		else
		{
			systemListConstructor.systemList[system].planetsInSystem[planet].planetCurrentDefence -= assaultDamage;
			systemListConstructor.systemList[system].planetsInSystem[planet].planetPopulation -= auxiliaryDamage;
		}
	}

	public void CalculateSystemDefence() 
	{
		maxSystemDefence = 0f;

		for(int i = 0; i < systemListConstructor.systemList[system].systemSize; ++i)
		{
			maxSystemDefence += systemListConstructor.systemList[system].planetsInSystem[i].planetImprovementLevel + 1 * 50f;
		}

		maxSystemDefence = (maxSystemDefence / systemListConstructor.systemList [system].systemSize) * 20f;
		systemListConstructor.systemList [system].systemOffence = (int)(maxSystemDefence / 2f);

		defenceRegenerator = maxSystemDefence / 5f;

		if(underInvasion == false)
		{
			if(regenerateTimer > 0)
			{
				--regenerateTimer;
			}

			for(int i = 0; i < systemListConstructor.systemList[system].systemSize; ++i)
			{
				CalculatePlanetDefence(i);
			}

			if(regenerateTimer <= 0 && systemListConstructor.systemList [system].systemDefence != maxSystemDefence)
			{
				regenerateTimer = 0;

				systemListConstructor.systemList [system].systemDefence += (int)defenceRegenerator;

				if(systemListConstructor.systemList [system].systemDefence >= maxSystemDefence)
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
				systemListConstructor.systemList [system].systemDefence = (int)maxSystemDefence;
			}
		}
	}

	public void CalculatePlanetDefence(int planet)
	{
		float maxPlanetDefence = ((systemListConstructor.systemList[system].planetsInSystem[planet].planetImprovementLevel) * (systemListConstructor.systemList[system].planetsInSystem[planet].planetImprovementLevel) * 20) + 50f;
		systemListConstructor.systemList [system].planetsInSystem[planet].planetOffence = maxPlanetDefence;

		systemListConstructor.systemList[system].planetsInSystem[planet].defenceRegeneration = defenceRegenerator/10f;
		systemListConstructor.systemList [system].planetsInSystem [planet].planetMaxDefence = maxPlanetDefence;

		if(systemListConstructor.systemList[system].planetsInSystem[planet].planetColonised == true && systemListConstructor.systemList[system].planetsInSystem[planet].planetCurrentDefence != maxPlanetDefence)
		{
			systemListConstructor.systemList[system].planetsInSystem[planet].planetCurrentDefence += defenceRegenerator/10f;

			if(systemListConstructor.systemList[system].planetsInSystem[planet].planetCurrentDefence > maxPlanetDefence)
			{
				systemListConstructor.systemList[system].planetsInSystem[planet].planetCurrentDefence = maxPlanetDefence;
			}
		}
	}

	public void CheckStatusEffects(int planet)
	{
		systemSIMData.knowledgeBuffModifier = 1.0f;
		systemSIMData.powerBuffModifier = 1.0f;

		if(systemListConstructor.systemList[system].planetsInSystem[planet].chillActive == true)
		{
			Chill (0, planet);
		}
		if(systemListConstructor.systemList[system].planetsInSystem[planet].virusActive == true)
		{
			Virus (planet);
		}
		if(systemListConstructor.systemList[system].planetsInSystem[planet].poisonActive == true)
		{
			Poison(planet);
		}
	}

	private void Virus(int planet)
	{
		if(systemListConstructor.systemList[system].planetsInSystem[planet].virusTimer == 0.0f)
		{
			systemListConstructor.systemList[system].planetsInSystem[planet].virusTimer = Time.time;
		}

		float timeDifference = Time.time - systemListConstructor.systemList[system].planetsInSystem[planet].virusTimer;

		if(timeDifference > 600f)
		{
			timeDifference = 600f;
		}

		timeDifference = (timeDifference - 300f) / 191f;

		float sinDifference = Mathf.Sin(timeDifference) + 1;

		systemSIMData.knowledgeBuffModifier = systemSIMData.knowledgeBuffModifier * sinDifference; // y = sin((x-300)/191) + 1
		systemSIMData.powerBuffModifier = systemSIMData.powerBuffModifier * sinDifference;

		for(int i = 0; i < systemListConstructor.systemList[system].permanentConnections.Count; ++i)
		{
			int j = RefreshCurrentSystem(systemListConstructor.systemList[system].permanentConnections[i]);

			bool skipSystem = false;

			for(int k = 0; k < systemListConstructor.systemList[j].systemSize; ++k)
			{
				if(systemListConstructor.systemList[system].planetsInSystem[k].virusActive == true)
				{
					skipSystem = true;
					break;
				}
			}

			if(skipSystem == false)
			{
				for(int k = 0; k < systemListConstructor.systemList[j].systemSize; ++k)
				{
					if(systemListConstructor.systemList[system].planetsInSystem[k].virusActive == false)
					{
						float ratio = Mathf.Max(systemListConstructor.systemList[system].planetsInSystem[k].planetCurrentDefence, maxSystemDefence) / 
										Mathf.Min (systemListConstructor.systemList[system].planetsInSystem[k].planetCurrentDefence, maxSystemDefence);

						if(ratio * 2 < sinDifference)
						{
							systemListConstructor.systemList[system].planetsInSystem[k].virusActive = true;
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
			systemListConstructor.systemList[system].planetsInSystem[planet].chillLength = systemListConstructor.systemList[system].planetsInSystem[planet].chillLength + newLength;
		}

		if(systemListConstructor.systemList[system].planetsInSystem[planet].chillTimer == 0.0f && systemListConstructor.systemList[system].planetsInSystem[planet].chillLength != 0.0f)
		{
			systemListConstructor.systemList[system].planetsInSystem[planet].chillTimer = Time.time;
		}

		if(systemListConstructor.systemList[system].planetsInSystem[planet].chillTimer + systemListConstructor.systemList[system].planetsInSystem[planet].chillLength >= Time.time)
		{
			systemSIMData.knowledgeBuffModifier = systemSIMData.knowledgeBuffModifier * 0.5f;
			systemSIMData.powerBuffModifier = systemSIMData.powerBuffModifier * 0.5f;
		}

		if(systemListConstructor.systemList[system].planetsInSystem[planet].chillTimer + systemListConstructor.systemList[system].planetsInSystem[planet].chillLength < Time.time)
		{
			systemListConstructor.systemList[system].planetsInSystem[planet].chillLength = 0.0f;
			systemListConstructor.systemList[system].planetsInSystem[planet].chillTimer = 0.0f;
			systemListConstructor.systemList[system].planetsInSystem[planet].chillActive = false;
		}
	}

	private void Poison(int planet)
	{
		if(systemListConstructor.systemList[system].planetsInSystem[planet].poisonTimer == 0.0f)
		{
			systemListConstructor.systemList[system].planetsInSystem[planet].poisonTimer = Time.time;
		}

		if(systemListConstructor.systemList[system].planetsInSystem[planet].poisonTimer + 2.0f < Time.time)
		{
			systemListConstructor.systemList[system].planetsInSystem[planet].poisonTimer = systemListConstructor.systemList[system].planetsInSystem[planet].poisonTimer + 2.0f;

			for(int i = 0; i < systemListConstructor.systemList[system].systemSize; ++i)
			{
				float population = systemListConstructor.systemList[system].planetsInSystem[i].planetPopulation;

				systemListConstructor.systemList[system].planetsInSystem[i].planetPopulation -= (population / 40f) + 0.5f;
			}
		}
	}
}
