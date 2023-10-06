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
		Interactable = false;
		_timeSinceSpawn = 0f;
		Lifetime = Game.Random.Float(0.1f, 0.2f);
		Degrees = Game.Random.Float(0f, 360f);
		ScaleX = Game.Random.Float(0.5f, 1.5f);
		ScaleY = Game.Random.Float(0.5f, 1.5f);
		//Brightness = 10f;
		Invert = 1f;
	}

	public override void Update(float dt)
	{
		base.Update(dt);

		if(FaceEmoji != null)
		{
			ZIndex = FaceEmoji.ZIndex - 2;
			Position = FaceEmoji.Position;
		}

		float progress = Utils.Map(_timeSinceSpawn, 0f, Lifetime, 0f, 1f, EasingType.QuadOut);
		Scale = 1f + progress * 0.7f;
		Opacity = (1f - progress);
		Blur = Utils.Map(progress, 0f, 1f, 0f, 10f);

		if(_timeSinceSpawn > Lifetime)
			Hud.Instance.RemoveEmoji(this);
	}
}
