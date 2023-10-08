using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace EmojiEngine;

public class ExplosionEmoji : Emoji
{
	public FaceEmoji FaceEmoji { get; set; }

	public float Lifetime { get; set; }
	private TimeSince _timeSinceSpawn;

	public ExplosionEmoji()
	{
		Text = "💥";
		IsInteractable = false;
		_timeSinceSpawn = 0f;
		Lifetime = Game.Random.Float(0.1f, 0.2f);
		Degrees = Game.Random.Float(0f, 360f);
		ScaleX = Game.Random.Float(0.66f, 1.33f);
		ScaleY = Game.Random.Float(0.66f, 1.33f);
		//Brightness = 10f;
		//Invert = 1f;

		//HasDropShadow = true;
		//DropShadowX = 0f;
		//DropShadowY = 0f;
		//DropShadowBlur = 8f;
		//DropShadowColor = Color.Red;
	}

	public override void Update(float dt)
	{
		base.Update(dt);

		if(FaceEmoji != null)
		{
			ZIndex = FaceEmoji.ZIndex + 2;
			//Position = FaceEmoji.Position;
		}

		Scale = Utils.Map(_timeSinceSpawn, 0f, Lifetime, 0.5f, 1.7f, EasingType.QuadOut);
		Opacity = Utils.Map(_timeSinceSpawn, 0f, Lifetime, 1f, 0f, EasingType.Linear);
		Blur = Utils.Map(_timeSinceSpawn, 0f, Lifetime, 6f, 4f, EasingType.QuadOut);
		Invert = Utils.Map(_timeSinceSpawn, 0f, Lifetime, 0f, 1f, EasingType.QuadIn);

		if(_timeSinceSpawn > Lifetime)
			Hud.Instance.RemoveEmoji(this);
	}
}
