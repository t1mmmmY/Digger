using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Messages
{
	public class AllFormulas
	{
		public List<Formula> formulas;

		public AllFormulas(List<Formula> allFormulas)
		{
			formulas = allFormulas;
		}

		public void Copy(AllFormulas other)
		{
			formulas = other.formulas;
		}
	}

	public class OneTurn
	{
		public bool isRightTurn;
		public int turnNumber;
		
		public OneTurn(bool isRightTurn, int turnNumber)
		{
			this.isRightTurn = isRightTurn;
			this.turnNumber = turnNumber;
		}
	}
}