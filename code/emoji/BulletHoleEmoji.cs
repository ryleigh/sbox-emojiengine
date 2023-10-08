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

	public BulletHoleEmoji()
	{
		float rand = Game.Random.Float(0f, 1f);
		if(rand < 0.8f)
			Text = "💥";
		else
			Text = "⭐️";

		IsInteractable = false;
		ZIndex = -50;
		Saturation = Game.Random.Float(1f, 2f);

		ScaleX = Game.Random.Float(1.15f, 1.25f);
		ScaleY = Game.Random.Float(0.75f, 0.85f);
		_timeSinceSpawn = 0f;
		Lifetime = Game.Random.Float(1.1f, 1.5f);
		PanelSizeFactor = 2f;
		SetFontSize(Game.Random.Float(20f, 24f));
		Degrees = Game.Random.Float(-45f, 45f);
		_brightness = Game.Random.Float(1f, 4f);
	}

	public override void Update(float dt)
	{
		base.Update(dt);

		Opacity = Utils.Map(_timeSinceSpawn, 0f, Lifetime, 1f, 0f);
		Brightness = Utils.Map(_timeSinceSpawn, 0f, Lifetime * 0.25f, _brightness, 0f, EasingType.QuadOut);
		ZIndex = (int)Utils.Map(_timeSinceSpawn, 0f, Lifetime * 0.25f, -50f, -99f, EasingType.QuadOut);
		Blur = Utils.Map(_timeSinceSpawn, 0f, Lifetime * 0.25f, 12f, 3f, EasingType.QuadOut);
		Scale = Utils.Map(_timeSinceSpawn, 0f, Lifetime * 0.15f, 1.25f, 1f, EasingType.QuadOut) * (Utils.Map(Position.y, 0f, Hud.Instance.ScreenHeight, 1.4f, 0.8f));

		if(_timeSinceSpawn > Lifetime)
			Hud.Instance.RemoveEmoji(this);
	}
}
