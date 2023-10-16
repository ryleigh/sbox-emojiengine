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
	public float Lifetime { get; set; }

	//private float _brightness;
	private Vector2 _gravityVelocity;

	private float _scale;

	public float GravityModifier { get; set; }

	public float GroundYPos { get; set; }
	public float RotateSpeed { get; set; }

	public override void Init()
	{
		base.Init();

		Text = "💦";

		Saturation = 4f;
		Lifetime = Game.Random.Float(0.25f, 0.5f);
		PanelSizeFactor = 2f;
		SetFontSize(Game.Random.Float(45f, 52f));
		Brightness = Game.Random.Float(0.7f, 1.2f);
		Opacity = 0f;
		_scale = Game.Random.Float(1.1f, 1.4f);

		HueRotateDegrees = 170f;

		TextShadowColor = new Color(0f, 0f, 0f, 0.7f);
		TextShadowY = -2f;
		TextShadowBlur = 3f;
	}

	public override void Update(float dt)
	{
		base.Update(dt);

		Opacity = Utils.Map(TimeSinceSpawn, 0f, Lifetime, 1f, 0f, EasingType.ExpoIn);
		//Brightness = Utils.Map(TimeSinceSpawn, 0f, Lifetime * 0.25f, _brightness, 1f, EasingType.QuadOut);
		Blur = Utils.Map(TimeSinceSpawn, 0f, Lifetime, 1f, 5f, EasingType.QuadIn);
		Scale = Utils.Map(TimeSinceSpawn, 0f, Lifetime, 0.5f, _scale, EasingType.QuadOut) * (Utils.Map(Position.y, 0f, Hud.Instance.ScreenHeight, 1.2f, 0.9f));

		Position += (Velocity + new Vector2(0f, Gravity)) * dt;
		Velocity *= (1f - 7f * dt);

		Degrees += RotateSpeed * dt;

		if(GroundYPos > 0f && Position.y < GroundYPos && TimeSinceSpawn > 0.25f)
		{
			int numPuddles = Game.Random.Int(2, 3);
			for(int i = 0; i < numPuddles; i++)
			{
				var puddlePos = Position + Utils.GetRandomVector() * (Game.Random.Float(0f, 5f) + Game.Random.Float(5f, 20f) * i);
				BloodPuddleEmoji puddle = Stage.AddEmoji(new BloodPuddleEmoji(), puddlePos) as BloodPuddleEmoji;
				puddle.SetFontSize(Game.Random.Float(10f, 15f));
				puddle.Lifetime = Game.Random.Float(0.7f, 0.9f);
			}

			Stage.RemoveEmoji(this);
			return;
		}

		Gravity += Globals.GRAVITY_ACCEL * GravityModifier * dt;

		if(TimeSinceSpawn > Lifetime)
			Stage.RemoveEmoji(this);
	}
}
