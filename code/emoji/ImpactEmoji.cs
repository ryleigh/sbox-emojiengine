using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace EmojiEngine;

public class ImpactEmoji : Emoji
{
	public float Lifetime { get; set; }

	private float _startScale;
	private float _endScale;

	public ImpactEmoji()
	{
		Text = "💥";
		IsInteractable = false;
		Lifetime = Game.Random.Float(0.15f, 0.2f);
		Degrees = Game.Random.Float(0f, 360f);
		ScaleX = Game.Random.Float(0.7f, 1.3f);
		ScaleY = Game.Random.Float(0.7f, 1.3f);
		//Brightness = 10f;
		//Invert = 1f;

		Opacity = 0f;

		_startScale = Game.Random.Float(0.4f, 0.65f);
		_endScale = Game.Random.Float(1.3f, 1.8f);

		HueRotateDegrees = Game.Random.Float(0f, 40f);
	}

	public override void Update(float dt)
	{
		base.Update(dt);

		Scale = Utils.Map(TimeSinceSpawn, 0f, Lifetime, _startScale, _endScale, EasingType.QuadOut);
		Opacity = Utils.Map(TimeSinceSpawn, 0f, Lifetime, 1f, 0f, EasingType.QuadIn) * Utils.Map(TimeSinceSpawn, 0f, 0.1f, 0f, 1f, EasingType.Linear);
		Blur = Utils.Map(TimeSinceSpawn, 0f, Lifetime, 6f, 12f, EasingType.QuadOut);
		//Invert = Utils.Map(TimeSinceSpawn, 0f, Lifetime, 0f, 1f, EasingType.Linear);

		if(TimeSinceSpawn > Lifetime)
			Hud.Instance.RemoveEmoji(this);
	}
}
