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
	public float Lifetime { get; set; }

	private float _brightness;
	private float _brightnessTime;

	public override void Init()
	{
		base.Init();

		Text = "💥";

		Saturation = Game.Random.Float(1f, 2f);
		Lifetime = Game.Random.Float(2f, 2.3f);
		PanelSizeFactor = 2.5f;
		SetFontSize(Game.Random.Float(19f, 25f));
		ScaleX = Game.Random.Float(1.2f, 1.4f);
		ScaleY = Game.Random.Float(0.6f, 0.8f);
		_brightness = Game.Random.Float(1f, 4f);
		_brightnessTime = Game.Random.Float(0.1f, 0.2f);

		TextShadowColor = Color.Black;
		TextShadowY = Game.Random.Float(-2f, -5f);
		TextShadowBlur = Game.Random.Float(3f, 5f);
	}

	public override void Update(float dt)
	{
		base.Update(dt);

		Opacity = Utils.Map(TimeSinceSpawn, 0f, Lifetime, 1f, 0f, EasingType.QuadIn);
		Brightness = Utils.Map(TimeSinceSpawn, 0f, Lifetime * _brightnessTime, _brightness, 0f, EasingType.QuadOut);
		ZIndex = (int)Utils.Map(TimeSinceSpawn, 0f, Lifetime * _brightnessTime, Globals.DEPTH_BULLET_HOLE_MAX, Globals.DEPTH_BULLET_HOLE_MIN, EasingType.QuadOut);
		Blur = Utils.Map(TimeSinceSpawn, 0f, Lifetime * 0.25f, 7f, 3f, EasingType.QuadOut);
		Scale = Utils.Map(TimeSinceSpawn, 0f, Lifetime * 0.15f, 1.25f, 1f, EasingType.QuadOut) * Utils.Map(Position.y, 0f, Hud.Instance.ScreenHeight, Globals.NEAR_SCALE, Globals.FAR_SCALE); ;

		TextStroke = Utils.Map(TimeSinceSpawn, 0f, Lifetime * 0.1f, 8f, 0f, EasingType.ExpoOut);
		TextStrokeColor = new Color(1f, Game.Random.Float(0f, 1f), 0f);

		if(TimeSinceSpawn > Lifetime)
			Stage.RemoveEmoji(this);
	}
}
