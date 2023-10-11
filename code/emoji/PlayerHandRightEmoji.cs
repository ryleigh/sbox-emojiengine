using Sandbox;
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
	public float TimeSinceShoot => Hud.Instance.CurrentTime - LastShootTime;

	public PlayerHandRightEmoji()
	{
		Text = "☝️";

		IsInteractable = false;

		//ScaleX = Game.Random.Float(1.2f, 1.4f);
		//ScaleY = Game.Random.Float(0.7f, 0.8f);
		SetFontSize(350f);

		ZIndex = Globals.DEPTH_PLAYER_HAND_RIGHT;
		//Opacity = 1f;
	}

	public override void Update(float dt)
	{
		base.Update(dt);
	}

	public void Shoot()
	{
		LastShootTime = Hud.Instance.CurrentTime;
	}
}
