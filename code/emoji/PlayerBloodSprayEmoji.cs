using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace EmojiEngine;

public class PlayerBloodSprayEmoji : Emoji
{
	public float Lifetime { get; set; }

	public float RotateSpeed { get; set; }

	//public float GroundYPos { get; set; }

	private float _scaleStart;
	private float _scaleEnd;

	private float _deceleration;

	public override void Init()
	{
		base.Init();

		Text = "💦";

		Saturation = 4f;
		Lifetime = Game.Random.Float(0.15f, 0.4f);
		PanelSizeFactor = 2f;
		SetFontSize(Game.Random.Float(150f, 230f));
		Brightness = Game.Random.Float(0.7f, 1.2f);
		Opacity = 0f;

		HueRotateDegrees = 170f;

		TextShadowColor = new Color(0f, 0f, 0f, 0.7f);
		TextShadowY = -2f;
		TextShadowBlur = 3f;

		_scaleStart = Game.Random.Float(1f, 1.7f);
		_scaleEnd = Game.Random.Float(0.7f, 2f);

		_deceleration = Game.Random.Float(6f, 10f);
	}

	public override void Update(float dt)
	{
		base.Update(dt);

		Opacity = Utils.Map(TimeSinceSpawn, 0f, Lifetime, 1f, 0f, EasingType.Linear);
		Blur = Utils.Map(TimeSinceSpawn, 0f, Lifetime, 3f, 8f, EasingType.Linear);
		Scale = Utils.Map(TimeSinceSpawn, 0f, Lifetime, _scaleStart, _scaleEnd, EasingType.QuadOut);

		Position += (Velocity + new Vector2(0f, Gravity)) * dt;
		Velocity *= (1f - _deceleration * dt);

		Degrees += RotateSpeed * dt;

		Gravity += Globals.GRAVITY_ACCEL * dt;

		if(TimeSinceSpawn > Lifetime)
			Stage.RemoveEmoji(this);
	}
}
