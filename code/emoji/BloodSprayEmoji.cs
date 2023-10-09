using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace EmojiEngine;

public class BloodSprayEmoji : Emoji
{
	private TimeSince _timeSinceSpawn;
	public float Lifetime { get; set; }

	//private float _brightness;
	public Vector2 Velocity { get; set; }
	private Vector2 _gravityVelocity;

	public float TargetDegrees { get; set; }
	private float _scale;

	public float Gravity { get; set; }

	public BloodSprayEmoji()
	{
		Text = "💦";

		IsInteractable = false;
		Saturation = Game.Random.Float(1f, 3f);

		_timeSinceSpawn = 0f;
		Lifetime = Game.Random.Float(0.25f, 0.35f);
		PanelSizeFactor = 2f;
		SetFontSize(Game.Random.Float(45f, 52f));
		Brightness = Game.Random.Float(0.7f, 1.3f);
		Opacity = 0f;
		_scale = Game.Random.Float(1.3f, 1.8f);

		HueRotateDegrees = 170f;
	}

	public override void Update(float dt)
	{
		base.Update(dt);

		Opacity = Utils.Map(_timeSinceSpawn, 0f, Lifetime, 1f, 0f, EasingType.QuadIn);
		//Brightness = Utils.Map(_timeSinceSpawn, 0f, Lifetime * 0.25f, _brightness, 1f, EasingType.QuadOut);
		Blur = Utils.Map(_timeSinceSpawn, 0f, Lifetime, 3f, 10f, EasingType.Linear);
		Scale = Utils.Map(_timeSinceSpawn, 0f, Lifetime, 0.5f, _scale, EasingType.QuadOut) * (Utils.Map(Position.y, 0f, Hud.Instance.ScreenHeight, 1.4f, 0.8f));

		Position += (Velocity + _gravityVelocity) * dt;
		Velocity *= (1f - 7f * dt);

		Degrees = Utils.DynamicEaseTo(Degrees, TargetDegrees, 0.075f, dt);

		_gravityVelocity += new Vector2(0f, -1f) * Gravity * dt;

		if(_timeSinceSpawn > Lifetime)
			Hud.Instance.RemoveEmoji(this);
	}
}
