using Godot;
using System;

public partial class GameScene : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var httpRequest = new HttpRequest();
		AddChild(httpRequest);
		httpRequest.RequestCompleted += HttpRequestCompleted;
		
		httpRequest.Request("https://jobfair.nordeus.com/jf24-fullstack-challenge/test");
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}
	
	private void HttpRequestCompleted(long result, long responseCode, string[] headers, byte[] body){
		if (responseCode == 200){
			GD.Print("Request successful!");
			GD.Print("Response Body: " + System.Text.Encoding.UTF8.GetString(body));
		}
		
		else{
			GD.Print("Request failed. Response code: " + responseCode);
		}
	}
}
