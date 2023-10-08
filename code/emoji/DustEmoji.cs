using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace EmojiEngine;

public class DustEmoji : Emoji
{
	private TimeSince _timeSinceSpawn;
	public float Lifetime { get; set; }

	private float _brightness;
	public Vector2 Velocity { get; set; }

	public DustEmoji()
	{
		Text = "💨";

		IsInteractable = false;
		ZIndex = -50;
		Saturation = Game.Random.Float(1f, 2f);

		//ScaleX = Game.Random.Float(1.15f, 1.25f);
		//ScaleY = Game.Random.Float(0.75f, 0.85f);
		_timeSinceSpawn = 0f;
		Lifetime = Game.Random.Float(0.075f, 0.35f);
		PanelSizeFactor = 2f;
		SetFontSize(Game.Random.Float(25f, 32f));
		_brightness = Game.Random.Float(1f, 4f);
		Opacity = 0f;
	}

	public override void Update(float dt)
	{
		base.Update(dt);

		Opacity = Utils.Map(_timeSinceSpawn, 0f, Lifetime, 0.5f, 0f, EasingType.QuadOut);
		Brightness = Utils.Map(_timeSinceSpawn, 0f, Lifetime * 0.25f, _brightness, 1f, EasingType.QuadOut);
		Blur = Utils.Map(_timeSinceSpawn, 0f, Lifetime * 0.25f, 12f, 5f, EasingType.QuadOut);
		Scale = Utils.Map(_timeSinceSpawn, 0f, Lifetime * 0.15f, 1.1f, 1f, EasingType.QuadOut) * (Utils.Map(Position.y, 0f, Hud.Instance.ScreenHeight, 1.4f, 0.8f));
		ScaleX = Utils.Map(_timeSinceSpawn, 0f, Lifetime * 0.4f, 1.25f, 1f, EasingType.Linear);

		Position += Velocity * dt;
		Velocity *= (1f - 25f * dt);

		if(_timeSinceSpawn > Lifetime)
			Hud.Instance.RemoveEmoji(this);
	}
}
