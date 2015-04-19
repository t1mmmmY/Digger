using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer))]
public class AutoTileSetQuad : AutoTile {
	[Header("Squared mode tilesets textures")]
	public Texture2D[] tilesetVariations_Squared;
	public Texture2D tilesetSquaredNormalMap;
	[Header("Sloped mode tileset textures")]
	public Texture2D[] tilesetVariations_Sloped;
	public Texture2D tilesetSlopesNormalMap;
	Material tempMaterial;
	
	override protected void UpdateDisplay() {
		if (tempMaterial==null) {
			tempMaterial = new Material(GetComponent<Renderer>().sharedMaterial);
		}
		tempMaterial.mainTexture=GetComponent<Renderer>().sharedMaterial.mainTexture;
		tempMaterial.mainTextureScale=new Vector2(1f/8f,1f/6f);
		tempMaterial.mainTextureOffset=new Vector2(1f/8f*sx,1f/6f*sy);
		tempMaterial.shader=GetComponent<Renderer>().sharedMaterial.shader;
		if (autoTileMode==AutoTileMode.Corner) {
			if (tilesetVariations_Squared.Length>0) {
				tempMaterial.mainTexture=tilesetVariations_Squared.PickElement();
			}
		} else {
			if (tilesetVariations_Sloped.Length>0) {
				tempMaterial.mainTexture=tilesetVariations_Sloped.PickElement();
			}
		}
		tempMaterial.mainTexture=GetComponent<Renderer>().sharedMaterial.mainTexture;
		
		if (autoTileMode==AutoTileMode.Corner) {
			if (tilesetSquaredNormalMap!=null) {
				tempMaterial.SetTexture("_BumpMap", tilesetSquaredNormalMap);
			}
		} else {
			if (tilesetSlopesNormalMap!=null) {
				tempMaterial.SetTexture("_BumpMap", tilesetSlopesNormalMap);
			}
		}
		tempMaterial.SetTextureScale ("_BumpMap", new Vector2(1f/8f,1f/6f));
		tempMaterial.SetTextureOffset("_BumpMap", new Vector2(1f/8f*sx,1f/6f*sy));
		
		tempMaterial.shader=GetComponent<Renderer>().sharedMaterial.shader;
		GetComponent<Renderer>().sharedMaterial = tempMaterial;
	}
	 
}
