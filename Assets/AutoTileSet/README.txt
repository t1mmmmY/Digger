AutoTileSet - Automatic Tile Setter

AutoTileSet makes 2D level editing faster and easier than ever while giving your tiles a wonderful look through the autotiling features.
You just need to worry about "solid or not" and let AutoTileSet pick the correct image in any case for you.
It also includes support for normal maps, slopes, 8 tileset examples and many other features.

===============BASICS=================
SETUP:
1. Drag the "AutoTileSet" prefab into the hierarchy window or open the included demo scene.

QUICK START GUIDE:
1. Open the demo scene (../AutoTileSet/Demo/DemoScene)
2. Select the "AutoTileSet" gameobject in the inspector
	NOTE: Enable 2D mode if it's not enabled
3. Click+drag on the grid to add tiles, click+drag on a tile to remove them
4. Right click to change corner type (slope/corner)
5. Drop an "AutoTile" prebaf in the "current tile" slot of the AutoTileSet component to use it
6. Press ESC or select another object in the hierarchy when finished

IMPORTANTE NOTE: If Unity asks to "fix" the included normal maps, click "ignore". Otherwise they will not work correctly.

===============FEATURES===============
-Select a tile and check/uncheck the "Slope corners" option to enable slope tiles in the corners
-Select a tile and change the Tile Size field to change the "fit to grid" size
-Select a tile and check/uncheck the "Only At Start" to do the autotiling once and improve performance
-You can specify which collision layers the autotile reacts to by adjusting the "AutoTileLayer" property and the gameobject layer (in the top of the inspector)
-The normal maps/dynamic lighting features require quad tiles (sprite shaders do not support this)
-Select the AutoTileSet gameobject and adjust the displayed grid options in the inspector
-Select the AutoTileSet gameobject and assign any prefab to the "Current Tile" field to set what tile will be draw when the scene is clicked.
	NOTE: Right click over a tile also picks it as the current, but it has to be an instanced prefab

=============CUSTOMIZATION============
TO CREATE CUSTOM TILES:
1. Copy the QuadTileExample folder (or SpriteTileExample folder, if you plan to work with sprites) and change the tileset images with your own*
	NOTE: You can also use the tilesets included in the "Tileset gallery" folder
2. In the same folder, assign the new tilesets to the corresponding prefab

*TO CREATE CUSTOM TILESETS:
Method A) Use AutoTileGen (www.autotilegen.com) saving as tileset (for quad tiles) or separate tiles (for sprites)
Method B) Grab the "@TILESET_TEMPLATE.png" file (in the "../AutoTileSet/Tileset gallery" folder) and draw the tileset in your preferred drawing program
	NOTE: For a slope tileset reference you can also use the "Rock Tiles" example included in the demo scene

===============SUPPORT================
If you have any issues you can contact the developer at info@pixelatto.com

AutoTileSet - © Pixelatto Games 2014