using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace EmojiEngine;

public class PlayerMuzzleLineEmoji : Emoji
{
	public float Lifetime { get; set; }
	private TimeSince _timeSinceSpawn;

	public Vector2 StartPos { get; set; }
	public Vector2 EndPos { get; set; }
	public Vector2 Direction { get; set; }
	private float _speed;

	public PlayerMuzzleLineEmoji()
	{
		Text = "☄️";
		IsInteractable = false;
		_timeSinceSpawn = 0f;
		//Lifetime = Game.Random.Float(0.1f, 0.1f);
		ZIndex = 9996;

		Opacity = 0f;
		//ScaleX = 10f;
		//ScaleY = 0.3f;

		SetFontSize(60f);

		_speed = Game.Random.Float(0f, 1000f);
	}

	public override void Update(float dt)
	{
		base.Update(dt);

		Opacity = Utils.Map(_timeSinceSpawn, 0f, Lifetime, 1f, 0f, EasingType.QuadOut);

		Position += Direction * _speed * dt;

		if(_timeSinceSpawn > Lifetime)
			Hud.Instance.RemoveEmoji(this);
	}
}
