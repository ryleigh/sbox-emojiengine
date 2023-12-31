﻿using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace EmojiEngine;

public class BloodDripEmoji : Emoji
{
	public float Lifetime { get; set; }

	//private float _brightness;
	private Vector2 _gravityVelocity;

	public float TargetDegrees { get; set; }
	private float _scale;

	public Vector2 WoundPosLast { get; set; }

	public float GroundYPos { get; set; }

	public override void Init()
	{
		base.Init();

		Text = "🩸";

		Saturation = Game.Random.Float(1f, 3f);

		Lifetime = Game.Random.Float(0.55f, 0.65f);
		SetFontSize(Game.Random.Float(15f, 45f));
		Brightness = Game.Random.Float(0.7f, 1.3f);
		Opacity = 0f;
		_scale = Game.Random.Float(1.1f, 1.3f);

		TextShadowColor = new Color(0f, 0f, 0f, 0.9f);
		TextShadowY = -1f;
		TextShadowBlur = 3f;
	}

	public override void Update(float dt)
	{
		base.Update(dt);

		if(Parent != null)
		{
			ZIndex = Parent.ZIndex + Globals.DEPTH_INCREASE_BLOOD_DRIP;

			if(TimeSinceSpawn < Lifetime * 0.6f)
			{
				Vector2 woundPosDelta = Parent.Position - WoundPosLast;
				//Position += woundPosDelta * Utils.Map(TimeSinceSpawn, 0f, Lifetime * 0.6f, 1f, 0f);

				WoundPosLast = Parent.Position;
			}
		}

		Opacity = Utils.Map(TimeSinceSpawn, 0f, Lifetime, 1f, 0f, EasingType.ExpoIn);
		//Brightness = Utils.Map(TimeSinceSpawn, 0f, Lifetime * 0.25f, _brightness, 1f, EasingType.QuadOut);
		Blur = Utils.Map(TimeSinceSpawn, 0f, Lifetime, 8f, 0f, EasingType.QuadOut);
		Scale = Utils.Map(TimeSinceSpawn, 0f, Lifetime, 0.2f, _scale, EasingType.QuadOut) * Utils.Map(Position.y, 0f, Hud.Instance.ScreenHeight, Globals.NEAR_SCALE, Globals.FAR_SCALE);
		ScaleY = Utils.Map(TimeSinceSpawn, 0f, Lifetime * 0.5f, 1f, 1.2f, EasingType.QuadIn);

		Position += (Velocity + new Vector2(0f, Gravity)) * dt;
		Velocity *= (1f - 7f * dt);

		if(GroundYPos > 0f && Position.y < GroundYPos)
		{
			var puddlePos = Position + new Vector2(Game.Random.Float(-7f, 7f), Game.Random.Float(-12f, 0f));
			BloodPuddleEmoji puddle = Stage.AddEmoji(new BloodPuddleEmoji(), puddlePos) as BloodPuddleEmoji;
			puddle.SetFontSize(Utils.Map(FontSize, 15f, 45f, 16f, 35f) * Game.Random.Float(0.95f, 1.05f));
			puddle.Lifetime = Utils.Map(FontSize, 15f, 45f, 1.5f, 2f) * Game.Random.Float(0.9f, 1.1f);

			Stage.RemoveEmoji(this);
			return;
		}

		Gravity += Globals.GRAVITY_ACCEL * dt;

		if(TimeSinceSpawn > Lifetime)
			Stage.RemoveEmoji(this);
	}
}
