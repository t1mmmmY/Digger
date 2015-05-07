using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InfiniteMap : MonoBehaviour 
{
	public int maxLines = 10;
	public int linesCount = 0;

	List<SimpleTile> allTiles;
	List<SimpleTile> lastLine;
	List<SimpleTile> topLine;

	public static System.Action OnInit;
	public static System.Action OnAddLine;

	public List<SimpleTile> GetCentralColumn()
	{
		List<SimpleTile> centralColumn = new List<SimpleTile>();

		foreach (SimpleTile tile in allTiles)
		{
			if (tile.transform.localPosition.x == 0)
			{
				centralColumn.Add(tile);
			}
		}

		return centralColumn;
	}

	public List<SimpleTile> GetLastCentralColumn()
	{
		List<SimpleTile> centralColumn = new List<SimpleTile>();
		
		foreach (SimpleTile tile in lastLine)
		{
			if (tile.transform.localPosition.x == 0)
			{
				centralColumn.Add(tile);
			}
		}
		
		return centralColumn;
	}

	public List<SimpleTile> GetAllTiles()
	{
		return allTiles;
	}

	public List<SimpleTile> GetLastLine()
	{
		return lastLine;
	}

//	void Awake()
//	{
//		FindAllTiles();
//		FindLastLine();
//		FindTopLine();
//		linesCount = GetLinesCount();
//	}

	void OnEnable()
	{
		FindAllTiles();
		FindLastLine();
		FindTopLine();
		linesCount = GetLinesCount();

		Digger.onDig += OnDig;

		if (OnInit != null)
		{
			OnInit();
		}
	}

	void OnDisable()
	{
		Digger.onDig -= OnDig;
	}

	void FindAllTiles()
	{
		allTiles = new List<SimpleTile>();

		SimpleTile[] tempAllTiles = FindObjectsOfType<SimpleTile>();
		foreach (SimpleTile tile in tempAllTiles)
		{
			allTiles.Add(tile);
		}
	}

	void FindLastLine()
	{
		lastLine = new List<SimpleTile>();
		float lastLineNumber = float.MaxValue;
		
		//Find last line number
		foreach (SimpleTile tile in allTiles)
		{
			if (tile.transform.localPosition.y < lastLineNumber)
			{
				lastLineNumber = tile.transform.localPosition.y;
			}
		}
		
		//Find all last line tiles
		foreach (SimpleTile tile in allTiles)
		{
			if (tile.transform.localPosition.y == lastLineNumber)
			{
				lastLine.Add(tile);
			}
		}
	}

	void FindTopLine()
	{
		topLine = new List<SimpleTile>();
		float topLineNumber = float.MinValue;
		
		//Find top line number
		foreach (SimpleTile tile in allTiles)
		{
			if (tile != null)
			{
				if (tile.transform.localPosition.y > topLineNumber)
				{
					topLineNumber = tile.transform.localPosition.y;
				}
			}
		}
		
		//Find all top line tiles
		foreach (SimpleTile tile in allTiles)
		{
			if (tile != null)
			{
				if (tile.transform.localPosition.y == topLineNumber)
				{
					topLine.Add(tile);
				}
			}
		}
	}

	int GetLinesCount()
	{
		if (topLine != null && lastLine != null)
		{
			return (int)(topLine[0].transform.localPosition.y - lastLine[0].transform.localPosition.y);
		}
		else
		{
			return 0;
		}
	}

	void OnDig()
	{
		CreateLine();
		if (linesCount > maxLines)
		{
			RemoveTopLine();
		}
	}

	void CreateLine()
	{
		for (int i = 0; i < lastLine.Count; i++)
		{
			GameObject newTile = AutoTileSetManager.instance.CreateTile(lastLine[i].transform.localPosition - Vector3.up * lastLine[i].tileSize);
			SimpleTile newTileQuad = newTile.GetComponent<SimpleTile>();
			if (newTileQuad != null)
			{
				lastLine[i] = newTileQuad;
				allTiles.Add(lastLine[i]);
			}
		}
		linesCount++;

		if (OnAddLine != null)
		{
			OnAddLine();
		}
	}

	void RemoveTopLine()
	{
		for (int i = 0; i < topLine.Count; i++)
		{
			if (topLine[i] != null)
			{
				allTiles.Remove(topLine[i]);
				Destroy(topLine[i].gameObject);
			}
		}
		linesCount--;

		FindTopLine();
	}

}
