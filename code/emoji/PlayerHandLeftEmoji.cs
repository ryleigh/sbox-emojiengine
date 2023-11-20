using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace EmojiEngine;

public class PlayerHandLeftEmoji : Emoji
{
	public CrosshairEmoji CrosshairEmoji { get; set; }

	public float LastShootTime { get; private set; }
	public float TimeSinceShoot => Stage.CurrentTime - LastShootTime;

	private float _currKickbackAmount;
	private float _kickbackDistance;

	private List<string> _handEmojis = new() { "✋", "✋", "🤘", "🤘", "🤟", "🖖", "☝️", "☝️", "☝️", "✊", "✊", };

	public override void Init()
	{
		base.Init();

		Text = "✋";

		SetFontSize(350f);

		ZIndex = Globals.DEPTH_PLAYER_HAND_LEFT;
		Opacity = 0f;

		FlipX = true;
		Degrees = 45f;

		TextShadowColor = new Color(0f, 0f, 0f, 0.6f);
		TextShadowY = 4f;
		TextShadowBlur = 15f;
	}

	public override void Update(float dt)
	{
		base.Update(dt);

		if(CrosshairEmoji == null)
			return;

		var aimPos = CrosshairEmoji.CenterPos;
		float width = Hud.Instance.ScreenWidth;
		float height = Hud.Instance.ScreenHeight;
		
		var targetPos = 
			new Vector2(Utils.Map(aimPos.x, 0f, width, 120f, 700f, EasingType.QuadIn), Utils.Map(aimPos.y, 0f, height, 70f, 220f, EasingType.QuadIn))
			+ (Position - aimPos).Normal * _currKickbackAmount
			+ new Vector2(0f, aimPos.x < width * 0.5f && aimPos.y < height * 0.4f ? Utils.Map(aimPos.y, 0f, height * 0.4f, -500f, 0f, EasingType.SineOut) * Utils.Map(aimPos.x, 0f, width * 0.5f, 1f, 0f, EasingType.Linear) : 0f)
			+ new Vector2(Utils.FastSin(TimeSinceSpawn * 2.75f) * 15f, Utils.FastSin(8f + TimeSinceSpawn * 2.1f) * 12f);

		Position = Utils.DynamicEaseTo(Position, targetPos, 0.3f, dt);

		if(aimPos.y > Position.y)
		{
			float targetDegrees = 90f - Utils.VectorToDegrees(aimPos - Position);
			if(targetDegrees < -40f)
				targetDegrees = Utils.Map(targetDegrees, -40f, -100f, -40f, -50f, EasingType.QuadIn);

			Degrees = Utils.DynamicEaseTo(Degrees, targetDegrees, 0.05f, dt);
		}

		_currKickbackAmount = Utils.DynamicEaseTo(_currKickbackAmount, Utils.Map(TimeSinceShoot, 0f, 0.5f, _kickbackDistance, 0f, EasingType.QuadOut), 0.6f, dt);

		Blur = Utils.Map(TimeSinceShoot, 0f, 0.3f, 15f, 3f, EasingType.QuadOut);
		Opacity = Utils.Map(TimeSinceShoot, 0f, 0.7f, 0.4f, 0.75f, EasingType.QuadOut);
		ScaleX = Utils.Map(TimeSinceShoot, 0f, 0.3f, 0.8f, 1f, EasingType.QuadOut);
		ScaleY = Utils.Map(TimeSinceShoot, 0f, 0.5f, 1.2f, 1f, EasingType.QuadOut);
	}

	public void Shoot(Vector2 hitPos)
	{
		LastShootTime = Stage.CurrentTime;
		_kickbackDistance = Game.Random.Float(-30f, -80f);

		Text = _handEmojis[Game.Random.Int(0, _handEmojis.Count - 1)];
	}
}
