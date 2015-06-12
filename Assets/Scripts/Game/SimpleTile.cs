using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class SimpleTile : MonoBehaviour 
{
	[SerializeField] bool fitToGrid = true;
	public float tileSize = 1;
	[SerializeField] Material backgroundMaterial;
	[SerializeField] Sprite backgroundSprite;

	protected Vector3 checkPosition;
	protected Mineral mineral;


	void Awake() 
	{
		checkPosition = transform.position;
	}

	void Update() 
	{
		if (!Application.isPlaying)
		{
			Vector3 displacement = transform.position - checkPosition;
			if (fitToGrid && (displacement.x != 0 || displacement.y != 0)) 
			{
				transform.position = new Vector3(Mathf.Round(transform.position.x / tileSize) * tileSize, 
				                                 Mathf.Round(transform.position.y / tileSize) * tileSize, 
				                                 transform.position.z);
			}
		}
	}

	public Mineral AddMineral(Mineral newMineral)
	{
		GameObject go = GameObject.Instantiate(newMineral.gameObject) as GameObject;
		go.transform.parent = this.transform;
		go.transform.localPosition = Vector3.zero;

		mineral = go.GetComponent<Mineral>();

		return mineral;
	}

	public bool DigMe(float invokeTime)
	{
		bool haveMineral = false;
		if (mineral != null)
		{
			haveMineral = true;
			mineral.TakeMineral();
		}

		StartCoroutine("DisableTile", invokeTime);

		return haveMineral;
	}

	IEnumerator DisableTile(float invokeTime)
	{
		gameObject.layer = 0;

		yield return new WaitForSeconds(invokeTime);

		BoxCollider collider = this.GetComponent<BoxCollider>();
		collider.enabled = false;
		
		SpriteRenderer render = this.GetComponent<SpriteRenderer>();
		render.material = backgroundMaterial;
		render.sprite = backgroundSprite;
	}

}
