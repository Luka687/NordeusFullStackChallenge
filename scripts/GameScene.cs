using Godot;
using System;
using System.Collections.Generic;

public partial class GameScene : Node2D
{
	
	private TileMapLayer tileMapLayer;
	private int [,] mapArray;
	private Dictionary<int, List<Vector2>> islands = new Dictionary<int, List<Vector2>>();
	private Dictionary<int, double> islandHeights = new Dictionary<int, double>();
	private int islandID = 0;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var httpRequest = new HttpRequest();
		AddChild(httpRequest);
		httpRequest.RequestCompleted += HttpRequestCompleted;
		httpRequest.Request("https://jobfair.nordeus.com/jf24-fullstack-challenge/test");	
		tileMapLayer = GetNode<TileMapLayer>("TileMapLayer");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}
	
	private void HttpRequestCompleted(long result, long responseCode, string[] headers, byte[] body){
		if (responseCode == 200){
			//GD.Print("Request successful!");
			//GD.Print("Response Body: " + System.Text.Encoding.UTF8.GetString(body));
			mapArray = createMap(System.Text.Encoding.UTF8.GetString(body), 30, 30);
			findIslands();
			generateTileMap();
		}
		
		else{
			//GD.Print("Request failed. Response code: " + responseCode);
		}
	}
	
	private int[,] createMap(string stringMap, int rows, int columns){
		int [,] map = new int[rows, columns];
		string [] rowValues = stringMap.Split('\n');
		
		(int, int) index = (0,0);
		foreach (string s in rowValues){
			index.Item2 = 0;
			foreach(string c in s.Split(' ')){
				//GD.Print(index);
				map[index.Item1,index.Item2] = int.Parse(c);
				index.Item2+=1;
			}
			index.Item1+= 1;
		}
		return map;
	}

	private void generateTileMap(){		
		for (int x = 0; x < mapArray.GetLength(0); x++)
		{
			for (int y = 0; y < mapArray.GetLength(1); y++)
			{
				tileMapLayer.SetCell(new Vector2I(x, y), 2, getTileType(mapArray[x,y]));			
			}
		}
	}
	
	private Vector2I getTileType(int height){
		switch (height){
			case 0:
				return new Vector2I(0,0);
			case <200:
				return new Vector2I(1,0);
			case <400:
				return new Vector2I(2,0);
			case <600:
				return new Vector2I(3,0);
			case <800:
				return new Vector2I(4,0);
			case <= 1000:
				return new Vector2I(5,0);
			default:
				return new Vector2I(1,0);
		}
	}	
	
	private void findIslands(){
		bool[,] visited = new bool[mapArray.GetLength(0), mapArray.GetLength(1)];
		
		for (int x = 0; x < mapArray.GetLength(0); x++)
		{
			for (int y = 0; y < mapArray.GetLength(1); y++)
			{
				if (mapArray[x, y] > 0 && !visited[x, y])
				{
					islandID++;
					List<Vector2> currentIsland = new List<Vector2>();
					double currentIslandHeight = 0;		
					int islandTileCount = 0;			
					BFS(ref visited, x, y, ref currentIsland, ref currentIslandHeight, ref islandTileCount);
					islands[islandID] = currentIsland;
					islandHeights[islandID] = currentIslandHeight / islandTileCount;
					GD.Print($"Island {islandID} found with {islandTileCount} tiles, average height: {currentIslandHeight / islandTileCount:F2}");
				}
			}
		}
	}
	
	private void BFS(ref bool[,] visited, int startX, int startY, ref List<Vector2> islandCoordinates,
					ref double currentIslandHeight, ref int islandTileCount){	
			
		Vector2[] Directions = {
			new Vector2(0, 1),
			new Vector2(1, 0),
			new Vector2(0, -1),
			new Vector2(-1, 0)
		};
		
		Queue<Vector2> queue = new Queue<Vector2>();
		queue.Enqueue(new Vector2(startX, startY));
		visited[startX, startY] = true;
		islandCoordinates.Add(new Vector2(startX, startY));
		currentIslandHeight += mapArray[startX, startY];
		islandTileCount++;
		
		while (queue.Count > 0)
		{
			Vector2 current = queue.Dequeue();
			int x = (int)current.X;
			int y = (int)current.Y;

			// Explore all 4 adjacent directions
			foreach (var dir in Directions)
			{
				int newX = x + (int)dir.X;
				int newY = y + (int)dir.Y;

				// Check bounds and whether the new cell is land and unvisited
				if (newX >= 0 && newX < mapArray.GetLength(0) && newY >= 0 && newY < mapArray.GetLength(1)
					&& mapArray[newX, newY] > 0 && !visited[newX, newY])
				{
					visited[newX, newY] = true;
					queue.Enqueue(new Vector2(newX, newY));
					islandCoordinates.Add(new Vector2(newX, newY));		
					currentIslandHeight += mapArray[newX, newY];
					islandTileCount++;		
				}
			}
		}	
	}
}
