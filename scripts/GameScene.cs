using Godot;
using System;

public partial class GameScene : Node2D
{
	
	private TileMapLayer tileMapLayer;
	
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
			generateTileMap(createMap(System.Text.Encoding.UTF8.GetString(body), 30, 30));
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
	
	private void generateTileMap(int [,] mapArray){
		int mapWidth = mapArray.GetLength(0);
		int mapHeight = mapArray.GetLength(1);
		
		for (int x = 0; x < mapWidth; x++)
		{
			for (int y = 0; y < mapHeight; y++)
			{
				tileMapLayer.SetCell(new Vector2I(x, y), 2, getTileType(mapArray[x,y]));
			}
		}
	}
	
	
	private Vector2I getTileType(int height){
		switch (height){
			case 0:
				return new Vector2I(0,0);
				break;
			case <200:
				return new Vector2I(1,0);
				break;
			case <400:
				return new Vector2I(2,0);
				break;
			case <600:
				return new Vector2I(3,0);
				break;
			case <800:
				return new Vector2I(4,0);
				break;
			case <= 1000:
				return new Vector2I(5,0);
				break;
			default:
				return new Vector2I(1,0);
		}
	}
}
