using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace EmojiEngine;

public class WoundEmoji : Emoji
{
	public Emoji Parent { get; set; }
	public float ParentOffsetDistance { get; set; }
	public float ParentOffsetDegrees { get; set; }
	public float ParentStartDegrees { get; set; }

	private TimeSince _timeSinceSpawn;
	public float Lifetime { get; set; }

	private float _brightness;
	private float _brightnessTime;

	private float _startDegrees;

	private bool _shouldSpawnBlood;

	public WoundEmoji()
	{
		Text = "🍁";

		IsInteractable = false;
		ZIndex = -50;
		Saturation = Game.Random.Float(1f, 2f);

		ScaleX = Game.Random.Float(1f, 1.1f);
		ScaleY = Game.Random.Float(0.9f, 1f);
		_timeSinceSpawn = 0f;
		Lifetime = Game.Random.Float(3f, 3.5f);
		PanelSizeFactor = 2f;
		SetFontSize(Game.Random.Float(20f, 22f));
		Degrees = _startDegrees = Game.Random.Float(0, 360f);
		_brightness = Game.Random.Float(4f, 5f);
		_brightnessTime = Game.Random.Float(0.2f, 0.35f);

		//HasDropShadow = true;
		//DropShadowX = 0f;
		//DropShadowY = 3f;
		//DropShadowBlur = 12f;
		//DropShadowColor = Color.Red;

		TextStrokeColor = Color.Red;

		_shouldSpawnBlood = true;
	}

	public override void Update(float dt)
	{
		base.Update(dt);

		if(Parent != null)
		{
			ZIndex = Parent.ZIndex + (int)Utils.Map(_timeSinceSpawn, 0f, Lifetime, 5f, 2f, EasingType.Linear);
			float parentDegreesDiff = (Parent.Degrees - ParentStartDegrees);
			Position = Parent.AnchorPos + Utils.DegreesToVector(ParentOffsetDegrees - parentDegreesDiff) * ParentOffsetDistance * Parent.Scale;
			Degrees = _startDegrees + parentDegreesDiff;
		}

		Opacity = Utils.Map(_timeSinceSpawn, 0f, Lifetime, 1f, 0f, EasingType.QuadIn);
		Brightness = Utils.Map(_timeSinceSpawn, 0f, _brightnessTime, _brightness, 0f, EasingType.QuadOut);
		Blur = Utils.Map(_timeSinceSpawn, 0f, Lifetime * 0.25f, 7f, 4f, EasingType.QuadOut);
		Scale = (Parent?.Scale ?? 1f) * Utils.Map(_timeSinceSpawn, 0f, Lifetime * 0.15f, 1.25f, 1f, EasingType.QuadOut) * (Utils.Map(Position.y, 0f, Hud.Instance.ScreenHeight, 1.4f, 0.8f));

		//TextStroke = Utils.Map(_timeSinceSpawn, 0f, Lifetime * 0.25f, 6f, 0f, EasingType.Linear);

		if(_shouldSpawnBlood)
		{
			BloodSprayEmoji blood = Hud.Instance.AddEmoji(new BloodSprayEmoji(), Position) as BloodSprayEmoji;
			blood.ZIndex = ZIndex + 2;
			blood.Velocity = (Position - Parent.Position).Normal * Game.Random.Float(600f, 1200f);

			blood.FlipX = true;
			var angle = Utils.VectorToDegrees(Position - Parent.Position);
			//Log.Info($"{angle}");

			if(angle > 0f && angle < 90f) // top right
			{
				blood.FlipY = true;
				blood.Degrees = angle + Utils.Map(angle, 0f, 90f, 180f, 0f);
				blood.TargetDegrees = blood.Degrees + Utils.Map(angle, 0f, 90f, 90f, 0f);
				blood.Velocity *= Utils.Map(angle, 0f, 90f, 1f, 1.5f);
			}
			else if(angle < 0f && angle > -90f) // bottom right
			{
				blood.FlipY = true;
				blood.Degrees = angle + Utils.Map(angle, 0f, -90f, -180f, 0f);
				blood.TargetDegrees = blood.Degrees + Utils.Map(angle, 0f, -90f, 90f, 0f);
			}
			else if(angle > 90f && angle < 180f) // top left
			{
				blood.Degrees = angle + 180f;
				blood.TargetDegrees = blood.Degrees + Utils.Map(angle, 90f, 180f, 0f, 90f);
				blood.Velocity *= Utils.Map(angle, 90f, 180f, 1.5f, 1f);
			}
			else // bottom left
			{
				blood.Degrees = angle + 180f;
				blood.TargetDegrees = blood.Degrees + Utils.Map(angle, -90f, -180f, 0f, 90f);
			}

			_shouldSpawnBlood = false;
		}

		if(_timeSinceSpawn > Lifetime)
			Hud.Instance.RemoveEmoji(this);
	}
}
