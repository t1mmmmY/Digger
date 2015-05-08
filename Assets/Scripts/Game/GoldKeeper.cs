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

	[SerializeField] MineralVariables[] mineralPrefabs;
	[SerializeField] InfiniteMap infiniteMap;

	List<Mineral> allMinerals;

	void OnEnable()
	{
		InfiniteMap.OnInit += OnInitInfiniteMap;
		InfiniteMap.OnAddLine += OnAddLine;
	}

	void OnDisable()
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
			if (!centralTiles.Contains(tile))
			{
				float randomValue = Random.Range(0.0f, 1.0f);
				if (randomValue > goldDensityBarier)
				{
					float perlinValue = Mathf.PerlinNoise(tile.transform.localPosition.x + randomShift, tile.transform.localPosition.y + randomShift);

					CreateMineral(tile, perlinValue);
				}
			}
		}

		foreach (SimpleTile tile in centralTiles)
		{
			float randomValue = Random.Range(0.0f, 1.0f);
			if (randomValue > goldDensityBarier)
			{
				float perlinValue = Mathf.PerlinNoise(tile.transform.localPosition.x + randomShift, tile.transform.localPosition.y + randomShift);

				CreateMineral(tile, perlinValue);
			}
		}
	}

	void CreateMineral(SimpleTile tile, float value)
	{
		int mineralNumber = Mathf.RoundToInt((mineralPrefabs.Length * value) / mineralPrefabs.Length);

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
