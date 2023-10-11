using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace EmojiEngine;

public class PlayerMuzzleLineEmoji : Emoji
{
	public float Lifetime { get; set; }
	public Vector2 StartPos { get; set; }
	public Vector2 EndPos { get; set; }
	public Vector2 Direction { get; set; }

	public PlayerMuzzleLineEmoji()
	{
		Text = "☄️";
		IsInteractable = false;
		ZIndex = Globals.DEPTH_PLAYER_MUZZLE_LINE;

		Opacity = 0f;

		//Brightness = 0f;

		SetFontSize(60f);
	}

	public override void Update(float dt)
	{
		base.Update(dt);

		Opacity = Utils.Map(TimeSinceSpawn, 0f, Lifetime, 1f, 0f, EasingType.Linear);

		if(TimeSinceSpawn > Lifetime)
			Hud.Instance.RemoveEmoji(this);
	}
}
