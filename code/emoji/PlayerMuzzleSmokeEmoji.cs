using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace EmojiEngine;

public class PlayerMuzzleSmokeEmoji : Emoji
{
	public float Lifetime { get; set; }
	private TimeSince _timeSinceSpawn;

	public Vector2 Velocity { get; set; }

	private float _opacity;
	private float _scaleStart;
	private float _scaleEnd;
	private float _deceleration;

	public PlayerMuzzleSmokeEmoji()
	{
		Text = "💨";
		IsInteractable = false;
		_timeSinceSpawn = 0f;
		Lifetime = Game.Random.Float(0.125f, 0.15f);
		ZIndex = Globals.DEPTH_PLAYER_MUZZLE_SMOKE;
		Opacity = 0f;

		_opacity = Game.Random.Float(0.4f, 0.9f);
		_scaleStart = Game.Random.Float(0.4f, 0.8f);
		_scaleEnd = Game.Random.Float(0.9f, 1.3f);
		_deceleration = Game.Random.Float(12f, 18f);

		SetFontSize(Game.Random.Float(120f, 160f));
	}

	public override void Update(float dt)
	{
		base.Update(dt);

		Scale = Utils.Map(_timeSinceSpawn, 0f, Lifetime, _scaleStart, _scaleEnd, EasingType.QuadOut);
		Opacity = Utils.Map(_timeSinceSpawn, 0f, Lifetime, _opacity, 0f, EasingType.Linear) * Utils.Map(_timeSinceSpawn, 0f, 0.05f, 0f, 1f, EasingType.QuadOut);
		Blur = Utils.Map(_timeSinceSpawn, 0f, Lifetime, 4f, 10f, EasingType.Linear);

		Position += Velocity * dt;
		Velocity *= (1f - _deceleration * dt);

		if(_timeSinceSpawn > Lifetime)
			Hud.Instance.RemoveEmoji(this);
	}
}
