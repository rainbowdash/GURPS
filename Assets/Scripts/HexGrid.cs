﻿ using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class HexGrid : MonoBehaviour {
	public int mapSizeX= 21;
	public int mapSizeY = 21;

	string parentName = "";
	float xOffset = 0.882f;
	float zOffset = 0.764f;

	int[,] tiles;
	Node[,] graph;
	public TileType[] tileTypes;

	public QuillUnit selectedUnit;

	void Start(){

		//selectedUnit = (GameObject)Instantiate (player, new Vector3 (0, 0, 0), Quaternion.identity);
		selectedUnit.GetComponent<QuillUnit>().tileX = (int)selectedUnit.transform.position.x;
		selectedUnit.GetComponent<QuillUnit>().tileY = (int)selectedUnit.transform.position.y;
		selectedUnit.GetComponent<QuillUnit>().map = this;

		GenerateMapData ();
		GeneratePathfindingGraph ();
		generateHexGrid ();
	}


	private void generateHexGrid (){

			for (int x = 0; x < mapSizeX; x++) {
				for (int y = 0; y < mapSizeY; y++) {

				float xPos = x * xOffset;

					// Are we on an odd row?
					if (y % 2 == 1) {
						xPos += xOffset / 2f;
					}
					TileType tt = tileTypes[ tiles[x,y] ];

				    GameObject go = (GameObject)Instantiate( tt.tileVisualPrefab, new Vector3(xPos, 0, y * zOffset), Quaternion.identity );


					// Name the gameobject something sensible.
					go.name = "Hex_" + x + "_" + y;

					// Make sure the hex is aware of its place on the map
			

					ClickableTile ct = go.GetComponent<ClickableTile>();
					ct.tileX = x;
					ct.tileY = y;
					ct.map = this;
				}
			}
	}

	void GenerateMapData() {
		// Allocate our map tiles
		tiles = new int[mapSizeX,mapSizeY];

		int x,y;

		// Initialize our map tiles to be grass
		for(x=0; x < mapSizeX; x++) {
			for(y=0; y < mapSizeX; y++) {
				tiles[x,y] = 0;
			}
		}



		// Let's make a u-shaped mountain range
		tiles[4, 4] = 1;
		tiles[5, 4] = 1;
		tiles[6, 4] = 1;
		tiles[7, 4] = 1;
		tiles[8, 4] = 1;

		tiles[4, 5] = 1;
		tiles[4, 6] = 1;
		tiles[8, 5] = 1;
		tiles[8, 6] = 1;

	}

	public float CostToEnterTile(int sourceX, int sourceY, int targetX, int targetY) {

		TileType tt = tileTypes[ tiles[targetX,targetY] ];

		if(UnitCanEnterTile(targetX, targetY) == false)
			return Mathf.Infinity;

		float cost = tt.movementCost;

		if( sourceX!=targetX && sourceY!=targetY) {
			// We are moving diagonally!  Fudge the cost for tie-breaking
			// Purely a cosmetic thing!
			cost += 0.001f;
		}

		return cost;

	}

	void GeneratePathfindingGraph() {
		// Initialize the array
		graph = new Node[mapSizeX,mapSizeY];

		// Initialize a Node for each spot in the array
		for(int x=0; x < mapSizeX; x++) {
			for(int y=0; y < mapSizeX; y++) {
				graph[x,y] = new Node();
				graph[x,y].x = x;
				graph[x,y].y = y;
			}
		}

		// Now that all the nodes exist, calculate their neighbours
		for(int x=0; x < mapSizeX; x++) {
			for(int y=0; y < mapSizeX; y++) {

				// This is the 4-way connection version:
				/*				if(x > 0)
					graph[x,y].neighbours.Add( graph[x-1, y] );
				if(x < mapSizeX-1)
					graph[x,y].neighbours.Add( graph[x+1, y] );
				if(y > 0)
					graph[x,y].neighbours.Add( graph[x, y-1] );
				if(y < mapSizeY-1)
					graph[x,y].neighbours.Add( graph[x, y+1] );
*/

				// This is the 8-way connection version (allows diagonal movement)
				// Try left
				/*if(x > 0) {
					graph[x,y].neighbours.Add( graph[x-1, y] );
					if(y > 0)
						graph[x,y].neighbours.Add( graph[x-1, y-1] );
					if(y < mapSizeY-1)
						graph[x,y].neighbours.Add( graph[x-1, y+1] );
				}

				// Try Right
				if(x < mapSizeX-1) {
					graph[x,y].neighbours.Add( graph[x+1, y] );
					if(y > 0)
						graph[x,y].neighbours.Add( graph[x+1, y-1] );
					if(y < mapSizeY-1)
						graph[x,y].neighbours.Add( graph[x+1, y+1] );
				}

				// Try straight up and down
				if(y > 0)
					graph[x,y].neighbours.Add( graph[x, y-1] );
				if(y < mapSizeY-1)
					graph[x,y].neighbours.Add( graph[x, y+1] ); **/

				// This also works with 6-way hexes and n-way variable areas (like EU4)
				// try left
				if (x > 0) {
					graph [x, y].neighbours.Add ( graph[x - 1, y]);
					//try left up
					if(y > 0)
						graph [x, y].neighbours.Add (graph[x , y + 1]);
					//try left down
					if(y< mapSizeY - 1)
						graph [x, y].neighbours.Add (graph[x , y - 1]);
				}
				//try right
				if (x < mapSizeX - 1) {
					graph [x, y].neighbours.Add (graph[x + 1, y]);
					//try left up
					if (y > 0)
						graph [x, y].neighbours.Add (graph[x + 1, y + 1]);
					//try left down
					if (y < mapSizeY - 1)
						graph [x, y].neighbours.Add (graph[x + 1, y - 1]);
				}
			}
		}
	}

	void GenerateMapVisual() {
		for(int x=0; x < mapSizeX; x++) {
			for(int y=0; y < mapSizeX; y++) {
				TileType tt = tileTypes[ tiles[x,y] ];
				GameObject go = (GameObject)Instantiate( tt.tileVisualPrefab, new Vector3(x, y, 0), Quaternion.identity );

				ClickableTile ct = go.GetComponent<ClickableTile>();
				ct.tileX = x;
				ct.tileY = y;
				ct.map = this;
			}
		}
	}

	public Vector3 TileCoordToWorldCoord(int x, int y) {
		return new Vector3(x, y, 0);
	}

	public bool UnitCanEnterTile(int x, int y) {

		// We could test the unit's walk/hover/fly type against various
		// terrain flags here to see if they are allowed to enter the tile.

		return tileTypes[ tiles[x,y] ].isWalkable;
	}

	public void GeneratePathTo(int x, int y) {
		// Clear out our unit's old path.
		selectedUnit.GetComponent<QuillUnit>().currentPath = null;

		if( UnitCanEnterTile(x,y) == false ) {
			// We probably clicked on a mountain or something, so just quit out.
			return;
		}

		Dictionary<Node, float> dist = new Dictionary<Node, float>();
		Dictionary<Node, Node> prev = new Dictionary<Node, Node>();

		// Setup the "Q" -- the list of nodes we haven't checked yet.
		List<Node> unvisited = new List<Node>();

		Node source = graph[
			selectedUnit.GetComponent<QuillUnit>().tileX, 
			selectedUnit.GetComponent<QuillUnit>().tileY
		];

		Node target = graph[
			x, 
			y
		];

		dist[source] = 0;
		prev[source] = null;

		// Initialize everything to have INFINITY distance, since
		// we don't know any better right now. Also, it's possible
		// that some nodes CAN'T be reached from the source,
		// which would make INFINITY a reasonable value
		foreach(Node v in graph) {
			if(v != source) {
				dist[v] = Mathf.Infinity;
				prev[v] = null;
			}

			unvisited.Add(v);
		}

		while(unvisited.Count > 0) {
			// "u" is going to be the unvisited node with the smallest distance.
			Node u = null;

			foreach(Node possibleU in unvisited) {
				if(u == null || dist[possibleU] < dist[u]) {
					u = possibleU;
				}
			}

			if(u == target) {
				break;	// Exit the while loop!
			}

			unvisited.Remove(u);

			foreach(Node v in u.neighbours) {
				//float alt = dist[u] + u.DistanceTo(v);
				float alt = dist[u] + CostToEnterTile(u.x, u.y, v.x, v.y);
				if( alt < dist[v] ) {
					dist[v] = alt;
					prev[v] = u;
				}
			}
		}

		// If we get there, the either we found the shortest route
		// to our target, or there is no route at ALL to our target.

		if(prev[target] == null) {
			// No route between our target and the source
			return;
		}

		List<Node> currentPath = new List<Node>();

		Node curr = target;

		// Step through the "prev" chain and add it to our path
		while(curr != null) {
			currentPath.Add(curr);
			curr = prev[curr];
		}

		// Right now, currentPath describes a route from out target to our source
		// So we need to invert it!

		currentPath.Reverse();

		selectedUnit.GetComponent<QuillUnit>().currentPath = currentPath;
	}
}
