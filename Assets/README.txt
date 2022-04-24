Interactive Agents and Procedural Generation Tech Demo ReadMe

PCG level generation using multi layered Fractal Brownian Motion (perlin) noise, domain warping and combined with Billow/Ridge noise 
Interactive agents include 3 different birds interacting with the player and mountain regions of the map

Video Documentation Link: https://youtu.be/07pmfLQRyUo

Third Party Libraries:
Sebastian Lague youtube series https://www.youtube.com/watch?v=wbpMiKiSKm8&list=PLFt_AvWsXl0eBW2EiBtl_sxmDtSgZBxB3
NPBehave
Unity Starter Assets
Movement AI https://github.com/sturdyspoon/unity-movement-ai


This unity project creates a small map that is auto updated using the MapGenerator editor. These settings will then be used
when the game is played.

Playing the project will create a full terrain using "Map Chunks" that combine to create a smooth transition. These chunks use LOD's
so that further away chunks have lower detail.

These map chunks are created using the Sebastian Lague InfiniteTerrain class. The MapGenerator class will fill out the height
and colour maps of the map chunks by calling the NoiseGeneration class with the appropriate coordinates and settings for each chunk. 
This uses threads taken from Sebesatian Lagues tutorials.

The MapGenerator contains overall map settings a list of NoiseSettings class objects that determine the layers of noise
that will determine each map chunks heightmap. 

The Following settings can be used:
LOD: Height values will lower the level of detail (level triangles in mesh)
Seed: Random starting point in perlin noise sample. Changing this will produce a whole new map
Strength: Determines the level of impact this layer will have on the height of the map I.E hight values will produce higher hills/mountains
Minimum Value: Reduces all heights for this layer by this amount to a minimum of zero. 
Scale: Height scale means a low range of perlin noise giving smaller changes in height distribution
Offset: Changes position of perlin noise sample giving different heights 
Octaves: Determines number loops of Perlin noise with increasing levels of detail (frequency) and lower impact (amplitude)
Persistance: Determines the amplitude of each octave in FBM (impact of each octave)
Lacunarity: Determines the frequency of each octave in FBM (level of detail in each octave)
Sharpness: Combined billow/ridge noise which produce terrain with smoother hills or ridges respectively
Warp: feed in additional layers of FBM into coords to produce more realistic terrain

The player can navigate the terrain using the starter unity assets player controller using the mouse to look around and WASD to move.

There are 3 different coloured Agents that will interact with the player and the map using the flockingbird class which uses 
combined weighted Seek/Flocking/Flee steering behaviour based on the Movement AI library and NPBehave Behaviour Trees to alternate 
their behaviour

Blue: Seek and floak with the player - Flee from Red 
Yellow: Seek tallest mountain peaks and follow player if none nearby - Flee from Red
Red: Randomly chase other agents. Will change target after a few seconds.


