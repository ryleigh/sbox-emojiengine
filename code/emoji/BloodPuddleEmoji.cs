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
	public float Lifetime { get; set; }

	private float _brightness;
	private float _brightnessTime;

	private float _startScale;
	private float _endScale;
	private float _scaleMidTime;

	public override void Init()
	{
		base.Init();

		Text = "🔴";

		//Saturation = Game.Random.Float(1f, 2f);

		ScaleX = Game.Random.Float(1.2f, 1.4f);
		ScaleY = Game.Random.Float(0.7f, 0.8f);
		//Lifetime = Game.Random.Float(3f, 4f);
		PanelSizeFactor = 3f;
		Opacity = 0f;
		//SetFontSize(Game.Random.Float(16f, 32f));
		//_brightness = Game.Random.Float(1f, 4f);
		//_brightnessTime = Game.Random.Float(0.1f, 0.2f);
		//Brightness = 10f;
		//Saturation = 10f;

		_startScale = Game.Random.Float(1.3f, 1.5f);
		_endScale = Game.Random.Float(1.15f, 1.35f);
		_scaleMidTime = Game.Random.Float(0.05f, 0.15f);

		TextStrokeColor = Color.Black;

		TextShadowColor = new Color(0f, 0f, 0f);
		TextShadowY = -5f;
		TextShadowBlur = 3f;
	}

	public override void Update(float dt)
	{
		base.Update(dt);

		Opacity = Utils.Map(TimeSinceSpawn, 0f, Lifetime, 1f, 0f, EasingType.QuadIn);
		//Brightness = Utils.Map(TimeSinceSpawn, 0f, Lifetime * _brightnessTime, _brightness, 1f, EasingType.QuadOut);
		//Brightness = Utils.Map(TimeSinceSpawn, 0f, Lifetime, 1f, 5f, EasingType.Linear);
		Blur = Utils.Map(TimeSinceSpawn, 0f, Lifetime, 0f, 12f, EasingType.QuadIn);
		TextStroke = Utils.Map(TimeSinceSpawn, 0f, Lifetime, 7f, 0f, EasingType.Linear);
		Scale = Utils.Map(TimeSinceSpawn, 0f, Lifetime * _scaleMidTime, _startScale, 1f, EasingType.QuadOut) * Utils.Map(TimeSinceSpawn, Lifetime * _scaleMidTime, Lifetime, 1f, _endScale, EasingType.Linear)
			* Utils.Map(Position.y, 0f, Hud.Instance.ScreenHeight, Globals.NEAR_SCALE, Globals.FAR_SCALE); ;

		ZIndex = -(int)Position.y - (int)Utils.Map(TimeSinceSpawn, 0f, Lifetime, 0f, 1000f);

		if(TimeSinceSpawn > Lifetime)
			Stage.RemoveEmoji(this);
	}
}
