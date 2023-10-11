using Sandbox;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace EmojiEngine;

public partial class CrosshairEmoji : Emoji
{
	public Vector2 CenterPos { get; set; }

	private float _length;
	private float _width;

	private const float MIN_GAP = 30f;
	public float LastShootTime { get; private set; }
	private float TimeSinceShoot => Hud.Instance.CurrentTime - LastShootTime;
	private float _recoilAmount;

	private float _mouseDeltaGap;
	private float _recoilGap;

	private float _gap;

	private Vector2 _recoilOffset;
	private Vector2 _targetRecoilOffset;

	private Vector2 _velocity;
	private float _shakeAmount;
	private float _shakeTime;
	private float _blurAmount;
	private float _blurTime;
	private float _brightnessAmount;

	private float _decelerationFactor;

	public PlayerGunEmoji PlayerGunEmoji { get; private set; }
	public PlayerHandLeftEmoji PlayerHandEmoji { get; private set; }

	public CrosshairEmoji()
	{
		Text = "";
		ZIndex = Globals.DEPTH_CROSSHAIR;
		IsInteractable = false;
		IsVisible = false;
		_length = 50f;
		//_decelerationFactor = 4f;

		PlayerGunEmoji = Hud.Instance.AddEmoji(new PlayerGunEmoji(), new Vector2(0f, -999f)) as PlayerGunEmoji;
		PlayerGunEmoji.CrosshairEmoji = this;

		PlayerHandEmoji = Hud.Instance.AddEmoji(new PlayerHandLeftEmoji(), new Vector2(0f, -999f)) as PlayerHandLeftEmoji;
		PlayerHandEmoji.CrosshairEmoji = this;
	}

	public override void Update(float dt)
	{
		base.Update(dt);

		float mouseDeltaLength = Hud.Instance.MouseDelta.Length;
		//_decelerationFactor = Utils.DynamicEaseTo(_decelerationFactor, Utils.Map(mouseDeltaLength, 0f, 30f, 4f, 1f) * Utils.Map(TimeSinceShoot, 0f, 1f, 0.1f, 1f, EasingType.QuadOut), 0.2f, dt);
		_decelerationFactor = 1.2f;

		_velocity += (Hud.Instance.MousePos - Position).Normal * Utils.Map((Hud.Instance.MousePos - Position).Length, 0f, 400f, 0f, 40000f * _decelerationFactor, EasingType.Linear) * dt;
		Position += _velocity * dt;

		_velocity *= (1f - 10f * _decelerationFactor * dt);

		Position = Hud.Instance.MousePos;

		_length = Utils.DynamicEaseTo(_length, Utils.Map(_recoilAmount, 0f, 300f, 50f, 160f, EasingType.Linear), 0.5f, dt);
		_width = Utils.DynamicEaseTo(_width, Utils.Map(TimeSinceShoot, 0f, 0.3f, 8f, 15f), 0.6f, dt);
		CenterPos = Position + new Vector2(0f, _width * 0.5f) + _recoilOffset;

		_recoilOffset = Utils.DynamicEaseTo(_recoilOffset, _targetRecoilOffset, 0.1f, dt);
		_targetRecoilOffset = Utils.DynamicEaseTo(_targetRecoilOffset, Vector2.Zero, 0.05f, dt);

		_mouseDeltaGap = Utils.DynamicEaseTo(_mouseDeltaGap, Utils.Map(mouseDeltaLength, 0f, 20f, 0f, 100f, EasingType.SineIn), 0.28f, dt);
		_recoilGap = Utils.DynamicEaseTo(_recoilGap, Utils.Map(TimeSinceShoot, 0f, 0.5f, _recoilAmount, 0f, EasingType.SineOut), 0.3f, dt);
		_recoilAmount = Utils.DynamicEaseTo(_recoilAmount, 0f, 0.05f, dt);

		_gap = Utils.DynamicEaseTo(_gap, MIN_GAP + _mouseDeltaGap + _recoilGap, 0.25f, dt);
		Color color = Color.Lerp(Color.White, Color.Black, 0.5f + Utils.FastSin(Hud.Instance.CurrentTime * 24f) * 0.5f).WithAlpha(0.5f);
		float invert = 1f;
		float saturation = Utils.Map(TimeSinceShoot, 0f, 1f, 10f, 1f);
		float blur = Utils.Map(TimeSinceShoot, 0f, 0.5f, 3f, 1f);

		Hud.Instance.DrawLine(CenterPos - new Vector2(_gap, 0f), CenterPos - new Vector2(_gap + _length, 0f), _width, color, 0f, ZIndex, invert, saturation, blur);
		Hud.Instance.DrawLine(CenterPos + new Vector2(_gap, 0f), CenterPos + new Vector2(_gap + _length, 0f), _width, color, 0f, ZIndex, invert, saturation, blur);
		Hud.Instance.DrawLine(CenterPos - new Vector2(0f, _gap), CenterPos - new Vector2(0f, _gap + _length), _width, color, 0f, ZIndex, invert, saturation, blur);
		Hud.Instance.DrawLine(CenterPos + new Vector2(0f, _gap), CenterPos + new Vector2(0f, _gap + _length), _width, color, 0f, ZIndex, invert, saturation, blur);

		if(Hud.Instance.MouseDownLeft && TimeSinceShoot > 0.25f)
			Shoot();

		Hud.Instance.OverlayDisplay.Brightness = Utils.Map(TimeSinceShoot, 0f, 0.125f, _brightnessAmount, 1f, EasingType.QuadOut);
		Hud.Instance.OverlayDisplay.Blur = Utils.Map(TimeSinceShoot, 0f, _blurTime, _blurAmount, 0f, EasingType.Linear);

		Hud.Instance.CameraOffset = Utils.GetRandomVector() * Utils.Map(TimeSinceShoot, 0f, _shakeTime, _shakeAmount, 0f, EasingType.QuadOut);
	}

	public override void OnMouseDown(bool rightClick)
	{
		base.OnMouseDown(rightClick);

		if(!rightClick)
			Shoot();
	}

	void Shoot()
	{
		var aimPos = Position + _recoilOffset;
		float gap = _gap - 6f;
		var hitPos = aimPos + Game.Random.Float(0f, gap) * Utils.GetRandomVector();

		if(Hud.Instance.Raycast(hitPos, out List<Emoji> hitEmojis))
		{
			var emoji = hitEmojis[0];
			if(emoji is FaceEmoji faceEmoji)
			{
				float HOLE_SIZE = 20f * faceEmoji.Scale;
				if((hitPos - faceEmoji.Position).LengthSquared > MathF.Pow(faceEmoji.Radius - HOLE_SIZE, 2f))
					hitPos = faceEmoji.Position + (hitPos - faceEmoji.Position).Normal * (faceEmoji.Radius - HOLE_SIZE);

				faceEmoji.Hit(hitPos);

				WoundEmoji wound = Hud.Instance.AddEmoji(new WoundEmoji(), hitPos) as WoundEmoji;
				wound.ZIndex = faceEmoji.ZIndex + 1;
				wound.Parent = faceEmoji;
				Vector2 faceAnchorPos = faceEmoji.AnchorPos;

				wound.ParentOffsetDistance = (hitPos - faceAnchorPos).Length / faceEmoji.Scale;
				wound.ParentOffsetDegrees = Utils.VectorToDegrees(hitPos - faceAnchorPos);
				wound.ParentStartDegrees = faceEmoji.Degrees;
			}
		}
		else
		{
			Hud.Instance.AddEmoji(new BulletHoleEmoji(), hitPos);

			DustEmoji dust = Hud.Instance.AddEmoji(new DustEmoji(), hitPos) as DustEmoji;
			dust.ZIndex = (int)(Hud.Instance.ScreenHeight - hitPos.y);
			float offsetX = hitPos.x - aimPos.x;
			dust.Degrees = Utils.Map(offsetX, -200f, 200f, -120f, -60f);
			dust.Velocity = Utils.DegreesToVector(-dust.Degrees) * Game.Random.Float(1000f, 3000f);
		}

		LastShootTime = Hud.Instance.CurrentTime;
		_recoilAmount += 160f;
		_targetRecoilOffset += new Vector2(Game.Random.Float(-70f, 70f), Game.Random.Float(0f, 1f) < 0.8f ? Game.Random.Float(50f, 350f) : Game.Random.Float(-20f, -70f));
		//_velocity += Utils.GetRandomVector() * Game.Random.Float(50f, 500f);
		_shakeTime = Game.Random.Float(0.05f, 0.15f);
		_shakeAmount = Game.Random.Float(10f, 30f);
		_shakeTime = Game.Random.Float(0.05f, 0.2f);
		_blurAmount = Game.Random.Float(0f, 20f);
		_blurTime = Game.Random.Float(0f, 0.1f);
		_brightnessAmount = Game.Random.Float(2f, 5f);

		PlayerGunEmoji.Shoot(hitPos);
		PlayerHandEmoji.Shoot(hitPos);
	}
}
