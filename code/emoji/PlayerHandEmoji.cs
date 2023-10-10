using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace EmojiEngine;

public class PlayerHandEmoji : Emoji
{
	public CrosshairEmoji CrosshairEmoji { get; set; }

	private TimeSince _timeSinceSpawn;
	private TimeSince _timeSinceShoot;

	private float _currKickbackAmount;
	private float _kickbackDistance;

	private List<string> _handEmojis = new() { "✋", "🤘", "🤟", "🖖", "☝️", "✊", "🖕", };

	public PlayerHandEmoji()
	{
		Text = "✋";

		IsInteractable = false;

		//ScaleX = Game.Random.Float(1.2f, 1.4f);
		//ScaleY = Game.Random.Float(0.7f, 0.8f);
		SetFontSize(350f);

		ZIndex = 9999;
		Opacity = 0f;

		//HasDropShadow = true;
		//DropShadowX = 0f;
		//DropShadowY = 0f;
		//DropShadowBlur = 10f;
		//DropShadowColor = Color.Black;
		_timeSinceSpawn = 0f;
		_timeSinceShoot = 999f;

		FlipX = true;
	}

	public override void Update(float dt)
	{
		base.Update(dt);

		if(CrosshairEmoji == null)
			return;

		float screenWidth = Hud.Instance.ScreenWidth;
		float screenHeight = Hud.Instance.ScreenHeight;

		var crosshairCenterPos = CrosshairEmoji.CenterPos;
		var targetPos = 
			new Vector2(260f, 70f)
			+ (Position - crosshairCenterPos).Normal * _currKickbackAmount
			+ new Vector2(0f, crosshairCenterPos.x < screenWidth * 0.5f && crosshairCenterPos.y < screenHeight * 0.4f ? Utils.Map(crosshairCenterPos.y, 0f, screenHeight * 0.4f, -500f, 0f, EasingType.SineOut) * Utils.Map(crosshairCenterPos.x, 0f, screenWidth * 0.5f, 1f, 0f, EasingType.Linear) : 0f)
			+ new Vector2(Utils.FastSin(_timeSinceSpawn * 2.75f) * 15f, Utils.FastSin(8f + _timeSinceSpawn * 2.1f) * 12f);

		Position = Utils.DynamicEaseTo(Position, targetPos, 0.3f, dt);

		if(crosshairCenterPos.y > Position.y)
		{
			float targetDegrees = Utils.VectorToDegrees(crosshairCenterPos - Position) - 90f;
			if(targetDegrees < -40f)
				targetDegrees = Utils.Map(targetDegrees, -40f, -100f, -40f, -50f, EasingType.QuadIn);

			Degrees = Utils.DynamicEaseTo(Degrees, targetDegrees, 0.05f, dt);
		}
			
		_currKickbackAmount = Utils.DynamicEaseTo(_currKickbackAmount, Utils.Map(_timeSinceShoot, 0f, 0.5f, _kickbackDistance, 0f, EasingType.QuadOut), 0.6f, dt);

		Blur = Utils.Map(_timeSinceShoot, 0f, 0.3f, 10f, 2f, EasingType.QuadOut);
		Opacity = Utils.Map(_timeSinceShoot, 0f, 0.7f, 0.5f, 1f, EasingType.QuadOut);
	}

	public void Shoot(Vector2 hitPos)
	{
		_timeSinceShoot = 0f;
		_kickbackDistance = Game.Random.Float(-30f, -80f);

		Text = _handEmojis[Game.Random.Int(0, _handEmojis.Count - 1)];
	}
}
