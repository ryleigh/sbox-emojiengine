using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace EmojiEngine;

public class PlayerGunEmoji : Emoji
{
	public CrosshairEmoji CrosshairEmoji { get; set; }

	private TimeSince _timeSinceShoot;

	private float _currKickbackAmount;
	private float _kickbackDistance;

	public PlayerGunEmoji()
	{
		BackgroundImage = "textures/gun.png";

		IsInteractable = false;
		Saturation = Game.Random.Float(1f, 8f);

		//ScaleX = Game.Random.Float(1.2f, 1.4f);
		//ScaleY = Game.Random.Float(0.7f, 0.8f);
		PanelSize = 500f;

		ZIndex = 9999;
		Opacity = 0f;

		//HasDropShadow = true;
		//DropShadowX = 0f;
		//DropShadowY = 0f;
		//DropShadowBlur = 10f;
		//DropShadowColor = Color.Black;

	}

	public override void Update(float dt)
	{
		base.Update(dt);

		if(CrosshairEmoji == null)
			return;

		var crosshairCenterPos = CrosshairEmoji.CenterPos;
		var targetPos = new Vector2(Hud.Instance.ScreenWidth, 0f)
			+ new Vector2(-260f, 50f)
			+ (Position - crosshairCenterPos).Normal * _currKickbackAmount
			+ new Vector2(crosshairCenterPos.x > Hud.Instance.ScreenWidth - 350f ? Utils.Map(crosshairCenterPos.x, Hud.Instance.ScreenWidth - 350f, Hud.Instance.ScreenWidth, 0f, 250f, EasingType.SineIn) : 0f, crosshairCenterPos.y < 200f ? Utils.Map(crosshairCenterPos.y, 0f, 200f, -300f, 0f, EasingType.SineOut) : 0f);

		Position = Utils.DynamicEaseTo(Position, targetPos, 0.3f, dt);

		if(crosshairCenterPos.y > Position.y)
			Degrees = Utils.DynamicEaseTo(Degrees, 172f - Utils.VectorToDegrees(crosshairCenterPos - Position), 0.4f, dt);

		_currKickbackAmount = Utils.DynamicEaseTo(_currKickbackAmount, Utils.Map(_timeSinceShoot, 0f, 0.5f, _kickbackDistance, 0f, EasingType.QuadOut), 0.6f, dt);

		Blur = Utils.Map(_timeSinceShoot, 0f, 0.5f, 10f, 2f, EasingType.QuadOut);
		Opacity = Utils.Map(_timeSinceShoot, 0f, 0.5f, 0.8f, 1f, EasingType.QuadOut);
	}

	public void Shoot()
	{
		_timeSinceShoot = 0f;
		_kickbackDistance = Game.Random.Float(120f, 300f);

		var toCrosshair = (CrosshairEmoji.CenterPos - Position).Normal;
		var muzzleFlashPos = Position + toCrosshair * Game.Random.Float(150f, 240f) + Utils.GetPerpendicularVector(toCrosshair) * Game.Random.Float(-60f, -80f);
		PlayerMuzzleFlashEmoji muzzleFlashEmoji = Hud.Instance.AddEmoji(new PlayerMuzzleFlashEmoji(), muzzleFlashPos) as PlayerMuzzleFlashEmoji;

		muzzleFlashEmoji.Degrees = 172f - Utils.VectorToDegrees(toCrosshair) + 45f + Game.Random.Float(-20f, 20f);
		muzzleFlashEmoji.ScaleX = Utils.Map(Math.Abs(toCrosshair.x), 0f, 1f, 0.8f, 1.3f);
		muzzleFlashEmoji.ScaleY = Utils.Map(Math.Abs(toCrosshair.y), 0f, 1f, 0.8f, 1.3f);
		//muzzleFlashEmoji.Velocity = -toCrosshair * 200f;
	}
}
