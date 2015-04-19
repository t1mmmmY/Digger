using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(SpriteRenderer))]
public class AutoTileSetSprite : AutoTile {
	[Header("Tileset sprites")]
	public Sprite[] tilesetCorner;
	public Sprite[] tilesetSlopes;
	int currentTileIndex=0;
	SpriteRenderer spriteRenderer {get {return spriteRenderer_==null?GetComponent<SpriteRenderer>():spriteRenderer_;} set {spriteRenderer_=value;}}
	SpriteRenderer spriteRenderer_;

	new protected void Awake() {
		base.Awake();
		spriteRenderer=GetComponent<SpriteRenderer>();
	}
	
	override protected void UpdateDisplay() {
		currentTileIndex=(5-(int)sy)*8+(int)sx;
		
		if (autoTileMode==AutoTileMode.Corner) {
			spriteRenderer.sprite=tilesetCorner[currentTileIndex];
		} else {
			if (tilesetSlopes!=null) {
				spriteRenderer.sprite=tilesetSlopes[currentTileIndex];
			}
		}
	}
	
		 
}
