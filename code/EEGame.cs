using Sandbox;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

//
// You don't need to put things in a namespace, but it doesn't hurt.
//
namespace EmojiEngine;

/// <summary>
/// This is your game class. This is an entity that is created serverside when
/// the game starts, and is replicated to the client. 
/// 
/// You can use this to create things like HUDs and declare which player class
/// to use for spawned players.
/// </summary>
public partial class EEGame : GameManager
{
	public new static EEGame Current => GameManager.Current as EEGame;
	public EEPlayer LocalPlayer => Game.LocalClient.Pawn as EEPlayer; // Client-only
	public Hud Hud { get; private set; }

	public EEGame()
	{
		if(Game.IsClient)
		{
			Hud = new Hud();
			Game.RootPanel = Hud;
		}
	}

	/// <summary>
	/// A client has joined the server. Make them a pawn to play with
	/// </summary>
	public override void ClientJoined(IClient client)
	{
		base.ClientJoined(client);

		var player = new EEPlayer();
		client.Pawn = player;
	}

	[GameEvent.Tick.Server]
	public void ServerTick()
	{
		
	}

	[GameEvent.Tick.Client]
	public void ClientTick()
	{
		
	}

	public IEnumerable<EEPlayer> Players => Game.Clients
		.Select(x => x.Pawn)
		.OfType<EEPlayer>();
}
