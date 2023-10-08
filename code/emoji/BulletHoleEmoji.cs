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
	public BulletHoleEmoji()
	{
		Text = "🔴";
		IsInteractable = false;
		ZIndex = -50;
		//Contrast = 10f;

		ScaleX = 1.2f;
		ScaleY = 0.8f;
		_timeSinceSpawn = 0f;
		Lifetime = Game.Random.Float(1.1f, 1.5f);
		PanelSizeFactor = 2f;
		SetFontSize(Game.Random.Float(20f, 24f));
	}

	public override void Update(float dt)
	{
		base.Update(dt);

		Opacity = Utils.Map(_timeSinceSpawn, 0f, Lifetime, 1f, 0f);
		Brightness = Utils.Map(_timeSinceSpawn, 0f, Lifetime * 0.25f, 2f, 0f, EasingType.QuadOut);
		ZIndex = (int)Utils.Map(_timeSinceSpawn, 0f, Lifetime * 0.25f, -50f, -99f, EasingType.QuadOut);
		Blur = Utils.Map(_timeSinceSpawn, 0f, Lifetime * 0.25f, 12f, 3f, EasingType.QuadOut);
		Scale = Utils.Map(_timeSinceSpawn, 0f, Lifetime * 0.15f, 1.25f, 1f, EasingType.QuadOut);

		if(_timeSinceSpawn > Lifetime)
			Hud.Instance.RemoveEmoji(this);
	}
}
