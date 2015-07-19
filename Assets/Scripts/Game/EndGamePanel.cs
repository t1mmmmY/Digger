using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EndGamePanel : MonoBehaviour 
{
	[SerializeField] Animator animator;
	[SerializeField] Text[] titleTexts;
	[SerializeField] Text cheaterText;
	[SerializeField] int[] scoreLevels;
//	[SerializeField] Text niceWorkText;
//	[SerializeField] Text newRecordText;
	[SerializeField] Text coinsCountText;
	[SerializeField] Text levelText;

	int showPanelAnimationHash = Animator.StringToHash("ShowPanel");

	public void ShowEndGamePanel(bool isRecord, int coinsCount, int level)
	{
		if (isRecord)
		{
			cheaterText.enabled = false;
			for (int i = 0; i < titleTexts.Length - 1; i++)
			{
				titleTexts[i].enabled = false;
			}
			titleTexts[titleTexts.Length-1].enabled = true;
		}
		else
		{
			titleTexts[titleTexts.Length-1].enabled = false;
			bool findTitle = false;
			for (int i = 0; i < titleTexts.Length - 1; i++)
			{
				if (level >= scoreLevels[i] && level < scoreLevels[i + 1])
				{
					titleTexts[i].enabled = true;
					findTitle = true;
				}
				else
				{
					titleTexts[i].enabled = false;
				}
			}
			cheaterText.enabled = !findTitle;
		}
//		niceWorkText.enabled = !isRecord;
//		newRecordText.enabled = isRecord;

		coinsCountText.text = coinsCount.ToString();
		levelText.text = level.ToString();

		animator.SetTrigger(showPanelAnimationHash);
	}

}
