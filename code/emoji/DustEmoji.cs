using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace EmojiEngine;

public class DustEmoji : Emoji
{
	public float Lifetime { get; set; }

	private float _brightness;
	public Vector2 Velocity { get; set; }

	private float _opacity;
	private float _blurStart;
	private float _blurEnd;
	private float _scaleStart;
	private float _scaleEnd;
	private float _scaleXStart;
	private float _scaleXEnd;
	private float _deceleration;

	public DustEmoji()
	{
		Text = "💨";

		IsInteractable = false;
		Saturation = Game.Random.Float(1f, 2f);

		//ScaleX = Game.Random.Float(1.15f, 1.25f);
		//ScaleY = Game.Random.Float(0.75f, 0.85f);
		Lifetime = Game.Random.Float(0.075f, 0.55f);
		PanelSizeFactor = 2f;
		SetFontSize(Game.Random.Float(25f, 32f));
		_brightness = Game.Random.Float(1f, 4f);
		Opacity = 0f;
		_opacity = Game.Random.Float(0.5f, 1f);
		_blurStart = Game.Random.Float(3f, 14f);
		_blurEnd = Game.Random.Float(3f, 8f);
		_scaleStart = Game.Random.Float(0.6f, 1.2f);
		_scaleEnd = Game.Random.Float(0.7f, 1.3f);
		_scaleXStart = Game.Random.Float(0.6f, 1.5f);
		_scaleXEnd = Game.Random.Float(0.7f, 1.3f);
		_deceleration = Game.Random.Float(20f, 30f);
	}

	public override void Update(float dt)
	{
		base.Update(dt);

		Opacity = Utils.Map(TimeSinceSpawn, 0f, Lifetime, _opacity, 0f, EasingType.QuadOut);
		Brightness = Utils.Map(TimeSinceSpawn, 0f, Lifetime * 0.25f, _brightness, 1f, EasingType.QuadOut);
		Blur = Utils.Map(TimeSinceSpawn, 0f, Lifetime * 0.25f, _blurStart, _blurEnd, EasingType.QuadOut);
		Scale = Utils.Map(TimeSinceSpawn, 0f, Lifetime * 0.15f, _scaleStart, _scaleEnd, EasingType.QuadOut);
		ScaleX = Utils.Map(TimeSinceSpawn, 0f, Lifetime * 0.4f, _scaleXStart, _scaleXEnd, EasingType.Linear);

		Position += Velocity * dt;
		Velocity *= (1f - _deceleration * dt);

		if(TimeSinceSpawn > Lifetime)
			Hud.Instance.RemoveEmoji(this);
	}
}
