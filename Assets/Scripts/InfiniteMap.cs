using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InfiniteMap : MonoBehaviour 
{
	public int maxLines = 10;
	public int linesCount = 0;

	List<AutoTileSetQuad> allTiles;
	List<AutoTileSetQuad> lastLine;
	List<AutoTileSetQuad> topLine;


	void OnEnable()
	{
		FindAllTiles();
		FindLastLine();
		FindTopLine();
		linesCount = GetLinesCount();

		Digger.onDig += OnDig;
	}

	void OnDisable()
	{
		Digger.onDig -= OnDig;
	}

	void FindAllTiles()
	{
		allTiles = new List<AutoTileSetQuad>();

		AutoTileSetQuad[] tempAllTiles = FindObjectsOfType<AutoTileSetQuad>();
		foreach (AutoTileSetQuad tile in tempAllTiles)
		{
			allTiles.Add(tile);
		}
	}

	void FindLastLine()
	{
		lastLine = new List<AutoTileSetQuad>();
		float lastLineNumber = float.MaxValue;
		
		//Find last line number
		foreach (AutoTileSetQuad tile in allTiles)
		{
			if (tile.transform.position.y < lastLineNumber)
			{
				lastLineNumber = tile.transform.position.y;
			}
		}
		
		//Find all last line tiles
		foreach (AutoTileSetQuad tile in allTiles)
		{
			if (tile.transform.position.y == lastLineNumber)
			{
				lastLine.Add(tile);
			}
		}
	}

	void FindTopLine()
	{
		topLine = new List<AutoTileSetQuad>();
		float topLineNumber = float.MinValue;
		
		//Find top line number
		foreach (AutoTileSetQuad tile in allTiles)
		{
			if (tile != null)
			{
				if (tile.transform.position.y > topLineNumber)
				{
					topLineNumber = tile.transform.position.y;
				}
			}
		}
		
		//Find all top line tiles
		foreach (AutoTileSetQuad tile in allTiles)
		{
			if (tile != null)
			{
				if (tile.transform.position.y == topLineNumber)
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
			return (int)(topLine[0].transform.position.y - lastLine[0].transform.position.y);
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
			GameObject newTile = AutoTileSetManager.instance.CreateTile(lastLine[i].transform.position - Vector3.up * lastLine[i].tileSize);
			AutoTileSetQuad newTileQuad = newTile.GetComponent<AutoTileSetQuad>();
			if (newTileQuad != null)
			{
				lastLine[i] = newTileQuad;
				allTiles.Add(lastLine[i]);
			}
		}
		linesCount++;
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
