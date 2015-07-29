using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Sign
{
	Plus,
	Minus,
	Multiply,
	Divided
}

public struct Formula
{
	public int x;
	public int y;
	public Sign sign;
	public int result;

	public Formula(int X, int Y, Sign sign, int Result)
	{
		x = X;
		y = Y;
		this.sign = sign;
		result = Result;
	}

	public string GetSignString()
	{
		return Mathematician.GetSignString(sign);
	}

	public override string ToString()
	{
		string signChar = "";
		switch (sign)
		{
		case Sign.Plus:
			signChar = "+";
			break;
		case Sign.Minus:
			signChar = "-";
			break;
		case Sign.Multiply:
			signChar = "*";
			break;
		case Sign.Divided:
			signChar = "/";
			break;
		}

		
		return string.Format("{0} {1} {2} = {3}", x, signChar, y, result);
	}
}

public static class Mathematician
{
	static float angle = 0.35f;
	static float offset = 5.0f;
	static float stretch = 0.7f;

    //static float[] difficultLevels = {7, 11, 13};
    static float[] difficultLevels = { 12, 16, 19 };
	public static Sign[] signs = {Sign.Plus, Sign.Minus, Sign.Multiply, Sign.Divided};

	public static string GetSignString(Sign sign)
	{
		switch (sign)
		{
		case Sign.Plus:
			return "+";
		case Sign.Minus:
			return "-";
		case Sign.Multiply:
			return "*";
		case Sign.Divided:
			return "/";
		}
		
		return "";
	}

	public static float GetDifficult(int level)
	{
		//inclined sinusoid
		return (angle * level + offset) + Mathf.Sin(stretch * level);
	}

	public static int GetSector(float difficult)
	{
		int sector = 1;
		for (int i = 0; i < difficultLevels.Length; i++)
		{
			if (difficult > difficultLevels[i])
			{
				sector = i + 2;
			}
		}

		return sector;
	}

	public static Formula GetFormula(int level)
	{
		float difficult = GetDifficult(level);
		int sector = GetSector(difficult);

		Sign sign = signs[Random.Range(0, sector)];
		int x = 0;
		int y = 0;
		int result = 0;

		if (sign != Sign.Divided)
		{
			x = GetValue(difficult);
			y = GetValue(difficult);
			switch (sign)
			{
			case Sign.Plus:
				result = x + y;
				break;
			case Sign.Minus:
				result = x - y;
				break;
			case Sign.Multiply:
				result = x * y;
				break;
			}
		}
		else
		{
			result = GetValue(difficult);
			do 
			{
				y = GetValue(difficult);
			} while (y == 0);
			x = result * y;
		}
		Formula formula = new Formula(x, y, sign, result);

		return formula;
	}

	static int GetValue(float difficult)
	{
        Debug.Log(string.Format("difficult = {0}; min {1}, max {2}", difficult, (-(int)difficult + (int)offset), ((int)difficult + (int)offset)));
		int value = 0;
		do
		{
            value = Random.Range(-(int)difficult + (int)offset, (int)difficult + (int)offset);
            //value = Random.Range(-(int)difficult + (int)offset, (int)difficult);
		} while (value == 0);

		return value;
	}
}
