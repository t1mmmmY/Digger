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
    public float goldDensityBonus = 0.5f;
	
	[SerializeField] AnimationCurve goldPerLevel;
    [SerializeField] AnimationCurve goldPerLevelBonus;
	[SerializeField] MineralVariables[] mineralPrefabs;
	[SerializeField] InfiniteMap infiniteMap;
    [SerializeField] int minBonusAmount = 30;
    [SerializeField] int maxBonusAmount = 80;
    [SerializeField] int bonusAmount = 50;

	List<Mineral> allMinerals;
    bool isBonus = false;
    int startCoinsCount = 0;
    int coinsCount = 0;

	void Awake()
	{
		InfiniteMap.OnInit += OnInitInfiniteMap;
		InfiniteMap.OnAddLine += OnAddLine;
        BankController.OnChangeCoins += OnChangeCoins;
	}

	void OnDestroy()
	{
		InfiniteMap.OnInit -= OnInitInfiniteMap;
		InfiniteMap.OnAddLine -= OnAddLine;
        BankController.OnChangeCoins -= OnChangeCoins;
	}

	void OnInitInfiniteMap()
	{
		float goldMultiplier = GeneralGameController.Instance.goldMultiplier;
//		if (bonusCharacter != null)
//		{
			goldDensity *= goldMultiplier;
			goldDensityBonus *= goldMultiplier;
//		}

		if (BonusController.Instance != null)
		{
        	isBonus = BonusController.Instance.IsBonusReady();
		}
        if (isBonus)
        {
            startCoinsCount = BankController.coins;
            bonusAmount = Random.Range(minBonusAmount, maxBonusAmount);
            Debug.Log("Bonus time!");
            BonusController.Instance.GrabBonus();
        }

		ClearMapFromMinerals();
		AddMinerals(infiniteMap.GetAllTiles(), infiniteMap.GetCentralColumn());
	}

	void OnAddLine()
	{
		AddMinerals(infiniteMap.GetLastLine(), infiniteMap.GetLastCentralColumn());
	}

    void OnChangeCoins(int newCoinsCount)
    {
        coinsCount = newCoinsCount - startCoinsCount;
        if (isBonus && coinsCount > bonusAmount)
        {
            isBonus = false;
            
            Debug.Log("GrabBonus!");
        }
    }

	void AddMinerals(List<SimpleTile> allTiles, List<SimpleTile> centralTiles)
	{
		float randomShift = Random.Range(0.0f, 100.0f);

        float density = 0;
        if (isBonus)
        {
            density = goldDensityBonus;
        }
        else
        {
            density = goldDensity;
        }

        float goldDensityBarier = 1.0f - density;

		foreach (SimpleTile tile in allTiles)
		{
//			if (!centralTiles.Contains(tile))
			{
				float randomValue = Random.Range(0.0f, 1.0f);
				if (randomValue > goldDensityBarier)
				{
					float perlinValue = Mathf.PerlinNoise(tile.transform.localPosition.x + randomShift, tile.transform.localPosition.y + randomShift);
                    if (isBonus)
                    {
                        perlinValue *= goldPerLevelBonus.Evaluate(-tile.transform.localPosition.y);
                    }
                    else
                    {
                        perlinValue *= goldPerLevel.Evaluate(-tile.transform.localPosition.y);
                    }
					
					CreateMineral(tile, perlinValue);
				}
			}
		}

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
