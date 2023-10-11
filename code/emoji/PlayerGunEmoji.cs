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

	public float LastShootTime { get; private set; }
	public float TimeSinceShoot => Hud.Instance.CurrentTime - LastShootTime;

	private float _currKickbackAmount;
	private float _kickbackDistance;

	public PlayerHandRightEmoji PlayerHandRightEmoji { get; set; }

	public PlayerGunEmoji()
	{
		BackgroundImage = "textures/gun4.png";
		IsInteractable = false;
		PanelSize = 600f;
		ZIndex = Globals.DEPTH_PLAYER_GUN;
		Opacity = 0f;

		PlayerHandRightEmoji = Hud.Instance.AddEmoji(new PlayerHandRightEmoji(), new Vector2(0f, -999f)) as PlayerHandRightEmoji;

		//TextShadowColor = new Color(0f, 0f, 0f, 0.6f);
		//TextShadowY = 10f;
		//TextShadowBlur = 20f;
	}

	public override void Update(float dt)
	{
		base.Update(dt);

		if(CrosshairEmoji == null)
			return;

		var aimPos = CrosshairEmoji.CenterPos;
		var aimDir = (aimPos - Position).Normal;
		var width = Hud.Instance.ScreenWidth;
		float height = Hud.Instance.ScreenHeight;

		var targetPos = new Vector2(width, 0f)
			+ new Vector2(Utils.Map(aimPos.x, 0f, width, -700f, -260f, EasingType.QuadOut), Utils.Map(aimPos.y, 0f, height, 50f, 300f, EasingType.QuadIn))
			+ (Position - aimPos).Normal * _currKickbackAmount
			+ new Vector2(aimPos.x > width * 0.6f ? Utils.Map(aimPos.x, width * 0.6f, width, 0f, 250f, EasingType.SineIn) : 0f, aimPos.y < height * 0.5f ? Utils.Map(aimPos.y, 0f, height * 0.5f, -500f, 0f, EasingType.SineOut) : 0f)
			+ new Vector2(Utils.FastSin(TimeSinceSpawn * 2.5f) * 10f, Utils.FastSin(10f + TimeSinceSpawn * 2.8f) * 15f);

		Position = Utils.DynamicEaseTo(Position, targetPos, 0.3f, dt);

		if(aimPos.y > Position.y)
			Degrees = Utils.DynamicEaseTo(Degrees, 172f - Utils.VectorToDegrees(aimPos - Position), 0.4f, dt);

		_currKickbackAmount = Utils.DynamicEaseTo(_currKickbackAmount, Utils.Map(TimeSinceShoot, 0f, 0.5f, _kickbackDistance, 0f, EasingType.QuadOut), 0.6f, dt);

		Blur = Utils.Map(TimeSinceShoot, 0f, 0.3f, 10f, 2f, EasingType.QuadOut);
		Opacity = Utils.Map(TimeSinceShoot, 0f, 0.7f, 0.5f, 1f, EasingType.QuadOut);
		ScaleX = Utils.Map(TimeSinceShoot, 0f, 0.25f, 0.75f, 1f, EasingType.QuadOut);
		ScaleY = Utils.Map(TimeSinceShoot, 0f, 0.3f, 1.25f, 1f, EasingType.QuadOut);

		//PlayerHandRightEmoji.Position = Utils.DynamicEaseTo(PlayerHandRightEmoji.Position, Position + new Vector2(20f, -140f), 0.25f, dt);
		PlayerHandRightEmoji.Position = Utils.DynamicEaseTo(PlayerHandRightEmoji.Position, Position + aimDir * Utils.Map(TimeSinceShoot, 0f, 0.3f, -250f, -100f) + Utils.GetPerpendicularVector(aimDir) * 100f, 0.25f, dt);
		PlayerHandRightEmoji.Degrees = Utils.DynamicEaseTo(PlayerHandRightEmoji.Degrees, Degrees - 90f, 0.25f, dt);
		PlayerHandRightEmoji.Opacity = Opacity;
		PlayerHandRightEmoji.Blur = Blur;
	}

	public void Shoot(Vector2 hitPos)
	{
		LastShootTime = Hud.Instance.CurrentTime;
		_kickbackDistance = Game.Random.Float(120f, 300f);

		var toCrosshair = (CrosshairEmoji.CenterPos - Position).Normal;
		var muzzleFlashPos = Position + toCrosshair * Game.Random.Float(150f, 400f) + Utils.GetPerpendicularVector(toCrosshair) * -100f;
		PlayerMuzzleFlashEmoji flash = Hud.Instance.AddEmoji(new PlayerMuzzleFlashEmoji(), muzzleFlashPos) as PlayerMuzzleFlashEmoji;

		flash.Degrees = 172f - Utils.VectorToDegrees(toCrosshair);// + Game.Random.Float(-40f, 40f);
		flash.PlayerGunEmoji = this;
		flash.LastPlayerGunPos = Position;

		PlayerMuzzleSmokeEmoji smoke = Hud.Instance.AddEmoji(new PlayerMuzzleSmokeEmoji(), muzzleFlashPos) as PlayerMuzzleSmokeEmoji;
		smoke.Degrees = -Utils.VectorToDegrees(hitPos - muzzleFlashPos);
		smoke.Velocity = (hitPos - muzzleFlashPos).Normal * Game.Random.Float(3000f, 4500f);

		int numLines = MathX.FloorToInt(Utils.Map((hitPos - muzzleFlashPos).Length, 0f, 1000f, 0f, 5f)) + Game.Random.Int(0, 1);
		float increment = 1f / (numLines + 1f);
		float distPercent = increment;
		while(distPercent < 0.95f)
		{
			SpawnMuzzleLine(muzzleFlashPos, hitPos, distPercent + Game.Random.Float(-0.05f, 0.05f));
			distPercent += increment;
		}

		PlayerHandRightEmoji.Shoot();
	}

	void SpawnMuzzleLine(Vector2 startPos, Vector2 endPos, float distPercent)
	{
		var pos = startPos + (endPos - startPos) * distPercent;
		var degrees = 228f - Utils.VectorToDegrees(endPos - startPos);
		PlayerMuzzleLineEmoji muzzleLineEmoji = Hud.Instance.AddEmoji(new PlayerMuzzleLineEmoji(), pos) as PlayerMuzzleLineEmoji;
		muzzleLineEmoji.Scale = Utils.Map(distPercent, 0f, 1f, 2f, 0.5f);
		muzzleLineEmoji.Lifetime = Utils.Map(distPercent, 0f, 1f, 0.02f, 0.1f);
		muzzleLineEmoji.Degrees = degrees;
		muzzleLineEmoji.Blur = Utils.Map(distPercent, 0f, 1f, 12f, 3f);
		muzzleLineEmoji.HueRotateDegrees = Utils.Map(distPercent, 0f, 1f, 200f, 140f);
		muzzleLineEmoji.Direction = (endPos - startPos).Normal;
	}
}
