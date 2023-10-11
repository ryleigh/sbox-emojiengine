using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace EmojiEngine;

public class BulletHoleEmoji : Emoji
{
	private TimeSince _timeSinceSpawn;
	public float Lifetime { get; set; }

	private float _brightness;
	private float _brightnessTime;

	public BulletHoleEmoji()
	{
		Text = "💥";

		IsInteractable = false;
		Saturation = Game.Random.Float(1f, 2f);

		_timeSinceSpawn = 0f;
		Lifetime = Game.Random.Float(2f, 2.3f);
		PanelSizeFactor = 2f;
		SetFontSize(Game.Random.Float(19f, 25f));
		ScaleX = Game.Random.Float(1.2f, 1.4f);
		ScaleY = Game.Random.Float(0.6f, 0.8f);
		Degrees = Game.Random.Float(-6f, 6f);
		_brightness = Game.Random.Float(1f, 4f);
		_brightnessTime = Game.Random.Float(0.1f, 0.2f);

		//HasDropShadow = true;
		//DropShadowX = 0f;
		//DropShadowY = 5f;
		//DropShadowBlur = 12f;
		//DropShadowColor = Color.Black;
	}

	public override void Update(float dt)
	{
		base.Update(dt);

		Opacity = Utils.Map(_timeSinceSpawn, 0f, Lifetime, 1f, 0f, EasingType.QuadIn);
		Brightness = Utils.Map(_timeSinceSpawn, 0f, Lifetime * _brightnessTime, _brightness, 0f, EasingType.QuadOut);
		ZIndex = (int)Utils.Map(_timeSinceSpawn, 0f, Lifetime * _brightnessTime, -4000f, -4500f, EasingType.QuadOut);
		Blur = Utils.Map(_timeSinceSpawn, 0f, Lifetime * 0.25f, 7f, 3f, EasingType.QuadOut);
		Scale = Utils.Map(_timeSinceSpawn, 0f, Lifetime * 0.15f, 1.25f, 1f, EasingType.QuadOut) * (Utils.Map(Position.y, 0f, Hud.Instance.ScreenHeight, 1.25f, 0.75f));

		TextStroke = Utils.Map(_timeSinceSpawn, 0f, Lifetime * 0.1f, 8f, 0f, EasingType.ExpoOut);
		TextStrokeColor = new Color(1f, Game.Random.Float(0f, 1f), 0f);

		if(_timeSinceSpawn > Lifetime)
			Hud.Instance.RemoveEmoji(this);
	}
}
