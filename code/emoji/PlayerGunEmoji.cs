﻿using Sandbox;
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

	private TimeSince _timeSinceSpawn;
	private TimeSince _timeSinceShoot;

	private float _currKickbackAmount;
	private float _kickbackDistance;

	public PlayerHandRightEmoji PlayerHandRightEmoji { get; set; }

	public PlayerGunEmoji()
	{
		BackgroundImage = "textures/gun4.png";

		IsInteractable = false;
		//Saturation = 10f;

		//ScaleX = Game.Random.Float(1.2f, 1.4f);
		//ScaleY = Game.Random.Float(0.7f, 0.8f);
		PanelSize = 600f;

		ZIndex = Globals.DEPTH_PLAYER_GUN;
		Opacity = 0f;

		//HasDropShadow = true;
		//DropShadowX = 0f;
		//DropShadowY = 0f;
		//DropShadowBlur = 10f;
		//DropShadowColor = Color.Black;
		_timeSinceSpawn = 0f;
		_timeSinceShoot = 999f;

		PlayerHandRightEmoji = Hud.Instance.AddEmoji(new PlayerHandRightEmoji(), new Vector2(0f, -999f)) as PlayerHandRightEmoji;
	}

	public override void Update(float dt)
	{
		base.Update(dt);

		if(CrosshairEmoji == null)
			return;

		var aimPos = CrosshairEmoji.CenterPos;
		var width = Hud.Instance.ScreenWidth;
		float height = Hud.Instance.ScreenHeight;

		var targetPos = new Vector2(width, 0f)
			+ new Vector2(Utils.Map(aimPos.x, 0f, width, -700f, -260f, EasingType.QuadOut), Utils.Map(aimPos.y, 0f, height, 50f, 300f, EasingType.QuadIn))
			+ (Position - aimPos).Normal * _currKickbackAmount
			+ new Vector2(aimPos.x > width * 0.6f ? Utils.Map(aimPos.x, width * 0.6f, width, 0f, 250f, EasingType.SineIn) : 0f, aimPos.y < height * 0.5f ? Utils.Map(aimPos.y, 0f, height * 0.5f, -500f, 0f, EasingType.SineOut) : 0f)
			+ new Vector2(Utils.FastSin(_timeSinceSpawn * 2.5f) * 10f, Utils.FastSin(10f + _timeSinceSpawn * 2.8f) * 15f);

		Position = Utils.DynamicEaseTo(Position, targetPos, 0.3f, dt);

		if(aimPos.y > Position.y)
			Degrees = Utils.DynamicEaseTo(Degrees, 172f - Utils.VectorToDegrees(aimPos - Position), 0.4f, dt);

		_currKickbackAmount = Utils.DynamicEaseTo(_currKickbackAmount, Utils.Map(_timeSinceShoot, 0f, 0.5f, _kickbackDistance, 0f, EasingType.QuadOut), 0.6f, dt);

		Blur = Utils.Map(_timeSinceShoot, 0f, 0.3f, 10f, 2f, EasingType.QuadOut);
		Opacity = Utils.Map(_timeSinceShoot, 0f, 0.7f, 0.5f, 1f, EasingType.QuadOut);
		ScaleX = Utils.Map(_timeSinceShoot, 0f, 0.25f, 0.75f, 1f, EasingType.QuadOut);
		ScaleY = Utils.Map(_timeSinceShoot, 0f, 0.3f, 1.25f, 1f, EasingType.QuadOut);

		PlayerHandRightEmoji.Position = Utils.DynamicEaseTo(PlayerHandRightEmoji.Position, Position + new Vector2(20f, -140f), 0.25f, dt);
		PlayerHandRightEmoji.Degrees = Utils.DynamicEaseTo(PlayerHandRightEmoji.Degrees, Degrees - 90f, 0.15f, dt);
		PlayerHandRightEmoji.Opacity = Opacity;
		PlayerHandRightEmoji.Blur = Blur;
	}

	public void Shoot(Vector2 hitPos)
	{
		_timeSinceShoot = 0f;
		_kickbackDistance = Game.Random.Float(120f, 300f);

		var toCrosshair = (CrosshairEmoji.CenterPos - Position).Normal;
		var muzzleFlashPos = Position + toCrosshair * Game.Random.Float(150f, 400f) + Utils.GetPerpendicularVector(toCrosshair) * -100f;
		PlayerMuzzleFlashEmoji flash = Hud.Instance.AddEmoji(new PlayerMuzzleFlashEmoji(), muzzleFlashPos) as PlayerMuzzleFlashEmoji;

		flash.Degrees = 172f - Utils.VectorToDegrees(toCrosshair);// + Game.Random.Float(-40f, 40f);
		flash.PlayerGunEmoji = this;
		flash.LastPlayerGunPos = Position;
		//flash.ScaleX = Utils.Map(Math.Abs(toCrosshair.x), 0f, 1f, 0.8f, 1.3f);
		//flash.ScaleY = Utils.Map(Math.Abs(toCrosshair.y), 0f, 1f, 0.8f, 1.3f);
		//flash.Velocity = toCrosshair * 1000f;

		PlayerMuzzleSmokeEmoji smoke = Hud.Instance.AddEmoji(new PlayerMuzzleSmokeEmoji(), muzzleFlashPos) as PlayerMuzzleSmokeEmoji;
		smoke.Degrees = -Utils.VectorToDegrees(hitPos - muzzleFlashPos);
		smoke.Velocity = (hitPos - muzzleFlashPos).Normal * Game.Random.Float(3000f, 4500f);

		float distPercent = Game.Random.Float(0.1f, 0.2f);
		while(distPercent < 0.95f)
		{
			SpawnMuzzleLine(muzzleFlashPos, hitPos, distPercent);
			distPercent += Game.Random.Float(0.1f, 0.3f);
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
