using UnityEngine;
using System.Collections;

public class ColorChanger : MonoBehaviour 
{
	[SerializeField] Color[] colors;
	[SerializeField] float transitionTime = 0.5f;
	bool animate = true;
	Light light;

	void Start () 
	{
		light = GetComponent<Light>();
		StartCoroutine("ChangeColor", 1);
	}


	IEnumerator ChangeColor(int colorNumber)
	{
		float elapsedTime = 0.0f;
		Color oldColor = light.color;
		Color targetColor = colors[colorNumber];
//		Debug.Log(targetColor.ToString());
		do
		{
			light.color = Color.Lerp(oldColor, targetColor, elapsedTime);
			yield return null;
			elapsedTime += Time.deltaTime / transitionTime;
//			Debug.Log(light.color.ToString());
//			Debug.Log(elapsedTime.ToString());
		} while (elapsedTime < 1.0f);

		StartCoroutine("ChangeColor", GetNextColor(colorNumber));
	}

	int GetNextColor(int colorNumber)
	{
		int nextColor = colorNumber + 1;
		if (nextColor >= colors.Length)
		{
			nextColor = 0;
		}
		return nextColor;
	}
}
