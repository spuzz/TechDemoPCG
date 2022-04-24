Interactive Agents and Procedural Generation Tech Demo ReadMe

PCG level generation using multi layered Fractal Brownian Motion (perlin) noise, domain warping and combined with Billow/Ridge noise 
Interactive agents include 3 different birds interacting with the player and mountain regions of the map

Video Documentation Link:

Third Party Libraries:
Sebastian Lague youtube series https://www.youtube.com/watch?v=wbpMiKiSKm8&list=PLFt_AvWsXl0eBW2EiBtl_sxmDtSgZBxB3
NPBehave
Unity Starter Assets
Movement AI https://github.com/sturdyspoon/unity-movement-ai


This unity project creates a small map that is auto updated using the MapGenerator editor. These settings will then be used
when the game is played.

Playing the project will create a full terrain using "Map Chunks" that combine to create a smooth terrain. These chunks use LOD's
so that further away chunks use less detail.

These map chunks are created using the Sebastian Lague InfiniteTerrain class. The MapGenerator class will fill out the height
and colour maps of the map chunks by calling the NoiseGeneration class with the appropriate coordinates and settings. This is done
using threads taken from Sebesatian Lagues tutorials.

The MapGenerator contains overall map settings a list of noise layer settings that will determine each map chunks heightmap. 
The Following settings can be used:

SETTINGS

The player can navigate the terrain using the starter unity assets player controller using the mouse to look around and WASD to move.

There are 3 different coloured Agents that will interact with the player and the map using the flockingbird class which uses 
combined weighted Seek/Flocking/Flee steering behaviour based on the Movement AI library and NPBehave Behaviour Trees to alternate 
their behaviour

Blue: Seek and floak with the player - Flee from Red 
Yellow: Seek tallest mountain peaks and follow player if none nearby - Flee from Red
Red: Randomly chase other agents. Will change target after a few seconds


