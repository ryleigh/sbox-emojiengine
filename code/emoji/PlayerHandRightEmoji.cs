﻿using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace EmojiEngine;

public class PlayerHandRightEmoji : Emoji
{
	public CrosshairEmoji CrosshairEmoji { get; set; }

	public float LastShootTime { get; private set; }
	public float TimeSinceShoot => Stage.CurrentTime - LastShootTime;

	public override void Init()
	{
		base.Init();

		Text = "☝️";

		//ScaleX = Game.Random.Float(1.2f, 1.4f);
		//ScaleY = Game.Random.Float(0.7f, 0.8f);
		SetFontSize(350f);

		ZIndex = Globals.DEPTH_PLAYER_HAND_RIGHT;
		//Opacity = 1f;

		TextShadowColor = new Color(0f, 0f, 0f, 0.6f);
		TextShadowY = 4f;
		TextShadowBlur = 15f;
	}

	public override void Update(float dt)
	{
		base.Update(dt);

		ScaleX = Utils.Map(TimeSinceShoot, 0f, 0.3f, 1.2f, 1f, EasingType.QuadOut);
		ScaleY = Utils.Map(TimeSinceShoot, 0f, 0.5f, 1.2f, 1f, EasingType.QuadOut);
	}

	public void Shoot()
	{
		LastShootTime = Stage.CurrentTime;
	}
}
