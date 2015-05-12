using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class MineralVariables
{
	public Mineral[] mineral;

	public Mineral GetRandomMineral()
	{
		return mineral[Random.Range(0, mineral.Length)];
	}
}

public class GoldKeeper : MonoBehaviour 
{
	public float goldDensity = 0.5f;
	
	[SerializeField] AnimationCurve goldPerLevel;
	[SerializeField] MineralVariables[] mineralPrefabs;
	[SerializeField] InfiniteMap infiniteMap;

	List<Mineral> allMinerals;

	void Awake()
	{
		InfiniteMap.OnInit += OnInitInfiniteMap;
		InfiniteMap.OnAddLine += OnAddLine;
	}

	void OnDestroy()
	{
		InfiniteMap.OnInit -= OnInitInfiniteMap;
		InfiniteMap.OnAddLine -= OnAddLine;
	}

	void OnInitInfiniteMap()
	{
		ClearMapFromMinerals();
		AddMinerals(infiniteMap.GetAllTiles(), infiniteMap.GetCentralColumn());
	}

	void OnAddLine()
	{
		AddMinerals(infiniteMap.GetLastLine(), infiniteMap.GetLastCentralColumn());
	}

	void AddMinerals(List<SimpleTile> allTiles, List<SimpleTile> centralTiles)
	{
		float randomShift = Random.Range(0.0f, 100.0f);
		float goldDensityBarier = 1.0f - goldDensity;

		foreach (SimpleTile tile in allTiles)
		{
//			if (!centralTiles.Contains(tile))
			{
				float randomValue = Random.Range(0.0f, 1.0f);
				if (randomValue > goldDensityBarier)
				{
					float perlinValue = Mathf.PerlinNoise(tile.transform.localPosition.x + randomShift, tile.transform.localPosition.y + randomShift);
					perlinValue *= goldPerLevel.Evaluate(-tile.transform.localPosition.y);
					CreateMineral(tile, perlinValue);
				}
			}
		}

//		foreach (SimpleTile tile in centralTiles)
//		{
//			float randomValue = Random.Range(0.0f, 1.0f);
//			if (randomValue > goldDensityBarier)
//			{
//				float perlinValue = Mathf.PerlinNoise(tile.transform.localPosition.x + randomShift, tile.transform.localPosition.y + randomShift);
//
//				CreateMineral(tile, perlinValue);
//			}
//		}
	}

	void CreateMineral(SimpleTile tile, float value)
	{
		int mineralNumber = 0;
		float step = 1.0f / mineralPrefabs.Length;

		for (int i = 0; i < mineralPrefabs.Length; i++)
		{
			if (value >= step * (i)/* && value <= step * (i + 1)*/)
			{
				mineralNumber = i;
//				break;
			}
		}

		Mineral mineral = tile.AddMineral(mineralPrefabs[mineralNumber].GetRandomMineral());
		allMinerals.Add(mineral);
	}

	void ClearMapFromMinerals()
	{
		if (allMinerals != null)
		{
			foreach (Mineral mineral in allMinerals)
			{
				Destroy(mineral.gameObject);
			}
			allMinerals.Clear();
		}

		allMinerals = new List<Mineral>();
	}

}
