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
	public Vector2 Velocity { get; set; }
	private Vector2 _gravityVelocity;

	public float TargetDegrees { get; set; }
	private float _scale;

	public float Gravity { get; set; }

	public WoundEmoji WoundEmoji { get; set; }
	public Vector2 WoundPosLast { get; set; }

	public float GroundYPos { get; set; }

	public override void Init()
	{
		base.Init();

		Text = "🩸";

		IsInteractable = false;
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

		if(WoundEmoji != null)
		{
			ZIndex = WoundEmoji.ZIndex + 1;
			
			if(TimeSinceSpawn < Lifetime * 0.6f)
			{
				Vector2 woundPosDelta = WoundEmoji.Position - WoundPosLast;
				Position += woundPosDelta * Utils.Map(TimeSinceSpawn, 0f, Lifetime * 0.6f, 1f, 0f);

				WoundPosLast = WoundEmoji.Position;
			}
		}

		Opacity = Utils.Map(TimeSinceSpawn, 0f, Lifetime, 1f, 0f, EasingType.ExpoIn);
		//Brightness = Utils.Map(TimeSinceSpawn, 0f, Lifetime * 0.25f, _brightness, 1f, EasingType.QuadOut);
		Blur = Utils.Map(TimeSinceSpawn, 0f, Lifetime, 8f, 0f, EasingType.QuadOut);
		Scale = Utils.Map(TimeSinceSpawn, 0f, Lifetime, 0.2f, _scale, EasingType.QuadOut);
		ScaleY = Utils.Map(TimeSinceSpawn, 0f, Lifetime * 0.5f, 1f, 1.2f, EasingType.QuadIn);

		Position += (Velocity + _gravityVelocity) * dt;
		Velocity *= (1f - 7f * dt);

		if(GroundYPos > 0f && Position.y < GroundYPos)
		{
			BloodPuddleEmoji puddle = Stage.AddEmoji(new BloodPuddleEmoji(), Position) as BloodPuddleEmoji;
			puddle.SetFontSize(Utils.Map(FontSize, 15f, 45f, 16f, 35f) * Game.Random.Float(0.95f, 1.05f));
			puddle.Lifetime = Utils.Map(FontSize, 15f, 45f, 1.5f, 2f) * Game.Random.Float(0.9f, 1.1f);

			Stage.RemoveEmoji(this);
			return;
		}

		_gravityVelocity += new Vector2(0f, -1f) * Gravity * dt;

		if(TimeSinceSpawn > Lifetime)
			Stage.RemoveEmoji(this);
	}
}
