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

	public WoundEmoji()
	{
		Text = "🍁";

		IsInteractable = false;
		ZIndex = -50;
		Saturation = Game.Random.Float(1f, 2f);

		ScaleX = Game.Random.Float(1f, 1.1f);
		ScaleY = Game.Random.Float(0.9f, 1f);
		_timeSinceSpawn = 0f;
		Lifetime = Game.Random.Float(0.8f, 1.2f) + 5f;
		PanelSizeFactor = 2f;
		SetFontSize(Game.Random.Float(20f, 22f));
		Degrees = _startDegrees = Game.Random.Float(0, 360f);
		_brightness = Game.Random.Float(4f, 5f);
		_brightnessTime = Game.Random.Float(0.1f, 0.3f);

		//HasDropShadow = true;
		//DropShadowX = 0f;
		//DropShadowY = 3f;
		//DropShadowBlur = 12f;
		//DropShadowColor = Color.Red;

		TextStrokeColor = Color.Red;
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

		if(_timeSinceSpawn > Lifetime)
			Hud.Instance.RemoveEmoji(this);
	}
}
