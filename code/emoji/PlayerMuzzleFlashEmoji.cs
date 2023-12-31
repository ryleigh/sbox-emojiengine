﻿using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace EmojiEngine;

public class PlayerMuzzleFlashEmoji : Emoji
{
	public float Lifetime { get; set; }
	public PlayerGunEmoji PlayerGunEmoji { get; set; }
	public Vector2 LastPlayerGunPos { get; set; }

	public override void Init()
	{
		base.Init();

		Text = "💥";
		Lifetime = Game.Random.Float(0.1f, 0.15f);
		ZIndex = Globals.DEPTH_PLAYER_MUZZLE_FLASH;

		ScaleX = Game.Random.Float(1.6f, 2.5f);
		ScaleY = Game.Random.Float(0.4f, 0.7f);

		SetFontSize(Game.Random.Float(140f, 260f));
	}

	public override void Update(float dt)
	{
		base.Update(dt);

		Scale = Utils.Map(TimeSinceSpawn, 0f, Lifetime, 0.7f, 1.5f, EasingType.QuadOut);
		Opacity = Utils.Map(TimeSinceSpawn, 0f, Lifetime, 1f, 0f, EasingType.Linear) * Utils.Map(TimeSinceSpawn, 0f, 0.05f, 0f, 1f, EasingType.QuadOut);
		Blur = Utils.Map(TimeSinceSpawn, 0f, Lifetime, 3f, 10f, EasingType.Linear);

		if(PlayerGunEmoji != null)
		{
			Vector2 gunPosDelta = PlayerGunEmoji.Position - LastPlayerGunPos;
			LastPlayerGunPos = PlayerGunEmoji.Position;

			Position += gunPosDelta;
		}

		if(TimeSinceSpawn > Lifetime)
			Stage.RemoveEmoji(this);
	}
}
