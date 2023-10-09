using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace EmojiEngine;

public class BloodPuddleEmoji : Emoji
{
	private TimeSince _timeSinceSpawn;
	public float Lifetime { get; set; }

	private float _brightness;
	private float _brightnessTime;

	public BloodPuddleEmoji()
	{
		Text = "🔴";

		IsInteractable = false;
		Saturation = Game.Random.Float(1f, 2f);

		ScaleX = Game.Random.Float(1.2f, 1.4f);
		ScaleY = Game.Random.Float(0.7f, 0.8f);
		_timeSinceSpawn = 0f;
		Lifetime = Game.Random.Float(1.6f, 3.8f);
		PanelSizeFactor = 1.5f;
		SetFontSize(Game.Random.Float(16f, 32f));
		_brightness = Game.Random.Float(1f, 4f);
		_brightnessTime = Game.Random.Float(0.1f, 0.2f);

		HasDropShadow = true;
		DropShadowX = 0f;
		DropShadowY = 4f;
		DropShadowBlur = 16f;
		DropShadowColor = Color.Black;
	}

	public override void Update(float dt)
	{
		base.Update(dt);

		Opacity = Utils.Map(_timeSinceSpawn, 0f, Lifetime, 1f, 0f, EasingType.QuadIn);
		Brightness = Utils.Map(_timeSinceSpawn, 0f, Lifetime * _brightnessTime, _brightness, 1f, EasingType.QuadOut);
		Blur = Utils.Map(_timeSinceSpawn, 0f, Lifetime * 0.25f, 7f, 3f, EasingType.QuadOut);
		Scale = Utils.Map(_timeSinceSpawn, 0f, Lifetime * 0.15f, 1.5f, 1f, EasingType.QuadOut) * Utils.Map(_timeSinceSpawn, Lifetime * 0.15f, Lifetime, 1f, 1.2f, EasingType.Linear);
		//DropShadowColor = Color.Lerp(Color.Red, Color.Black, Utils.Map(_timeSinceSpawn, 0f, Lifetime, 0f, 1f, EasingType.QuadOut));

		if(_timeSinceSpawn > Lifetime)
			Hud.Instance.RemoveEmoji(this);
	}
}
