using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using static Sandbox.Package;

namespace EmojiEngine;

public class KnifeEmoji : Emoji
{
	public ShadowEmoji ShadowEmoji { get; set; }

	public bool IsThrownAtPlayer { get; set; }

	public float ThrowTime { get; set; }
	public float TimeSinceThrown => Stage.CurrentTime - ThrowTime;
	private Vector2 _throwStartPos;
	private Vector2 _throwTargetPos;
	private float _throwTotalTime;
	private float _throwStartScale;

	private float _blinkTimer;
	private bool _blinked;
	private const float BLINK_TIME = 0.075f;

	public bool DidHitPlayer { get; set; }
	public float HitPlayerTime { get; set; }
	public float TimeSinceHitPlayer => Stage.CurrentTime - HitPlayerTime;
	private float FADE_OUT_TIME = 1.2f;
	private float _bloodSprayTimer;
	private float _bloodSprayDelay;

	public KnifeEmoji()
	{
		IsInteractable = true;
	}

	public override void Init()
	{
		base.Init();

		Text = "🔪";
		SetFontSize(120f);
		Radius = FontSize * 0.3f;
		HitRectSize = new Vector2(60f, 160f);
		HitRectDegrees = 45f;

		ShadowEmoji = Stage.AddEmoji(new ShadowEmoji(), new Vector2(-999f, -999f)) as ShadowEmoji;
		ShadowEmoji.SetFontSize(FontSize * 0.8f);

		Weight = 2f;
	}

	public override void Update(float dt)
	{
		base.Update(dt);

		//Stage.DrawLineTo(ShadowEmoji.Position, 2f, Color.White);

		if(DidHitPlayer)
		{
			float hitProgress = Utils.Map(TimeSinceHitPlayer, 0f, FADE_OUT_TIME, 0f, 1f);

			_bloodSprayTimer += dt;
			if(_bloodSprayTimer > _bloodSprayDelay)
			{
				var bloodPos = Position + (_throwTargetPos - _throwStartPos).Normal * Game.Random.Float(10f, 25f);

				PlayerBloodSprayEmoji spray = Stage.AddEmoji(new PlayerBloodSprayEmoji(), bloodPos) as PlayerBloodSprayEmoji;
				spray.ZIndex = ZIndex + 1;

				var dir = Utils.RotateVector((Position - bloodPos).Normal, Game.Random.Float(-30f, 30f));
				spray.Velocity = dir * Game.Random.Float(1000f, 6000f);
				spray.FlipX = dir.x < 0f;
				spray.Degrees = -Utils.VectorToDegrees(Position - bloodPos) + (spray.FlipX ? 180f : 0f);
				spray.RotateSpeed = Game.Random.Float(140f, 450f) * Utils.Map(MathF.Abs(dir.y), 0f, 1f, 1f, 0f) * (spray.FlipX ? -1f : 1f);

				_bloodSprayTimer = 0f;
				_bloodSprayDelay = Game.Random.Float(0.05f, 0.2f) * Utils.Map(hitProgress, 0f, 1f, 0.5f, 1.5f, EasingType.QuadOut);
			}

			Opacity = Utils.Map(hitProgress, 0f, 1f, 1f, 0f, EasingType.ExpoIn);

			if(TimeSinceHitPlayer > FADE_OUT_TIME)
				Stage.RemoveEmoji(this);
		}
		else if(IsThrownAtPlayer)
		{
			float throwProgress = Utils.Map(TimeSinceThrown, 0f, _throwTotalTime, 0f, 1f);

			//targetPos = Hud.Instance.MousePos;

			Position = Vector2.Lerp(_throwStartPos, _throwTargetPos, Utils.Map(throwProgress, 0f, 1f, 0f, 1f, EasingType.QuadIn));
			//Altitude += 100f * dt;

			Scale = Utils.Map(throwProgress, 0f, 1f, _throwStartScale, 3f, EasingType.SineIn);

			ZIndex = (int)MathF.Round(Utils.Map(throwProgress, 0f, 1f, Globals.THROWN_AT_PLAYER_MIN, Globals.THROWN_AT_PLAYER_MAX)) + (int)Position.x;

			ShadowEmoji.Position = Position + new Vector2(0f, Utils.Map(throwProgress, 0f, 1f, -45f, -300f, EasingType.SineIn));
			ShadowEmoji.Opacity = Utils.Map(throwProgress, 0f, 1f, 1f, 0f, EasingType.QuadIn);
			ShadowEmoji.ScaleX = ScaleX;
			ShadowEmoji.ScaleY = ScaleY;
			ShadowEmoji.ZIndex = Globals.THROWN_AT_PLAYER_SHADOW;

			_blinkTimer += dt;
			if(_blinkTimer > BLINK_TIME)
			{
				_blinkTimer -= BLINK_TIME;
				_blinked = !_blinked;
			}

			Brightness = _blinked ? 6f : 1f;

			if(TimeSinceThrown > _throwTotalTime)
				HitPlayer();
		}
		else
		{
			if(Parent == null)
			{
				Position += Velocity * dt;
				Velocity *= (1f - 3f * dt);

				Altitude += Gravity * dt;

				if(Altitude > 0f)
				{
					Gravity += Globals.GRAVITY_ACCEL * dt;

					Degrees += 1500f * dt;

					while(Degrees > 360f)
						Degrees -= 360f;

					while(Degrees < -360f)
						Degrees += 360f;
				}
				else
				{
					if(Altitude < 0f)
						Altitude = 0f;

					Degrees = Utils.DynamicEaseTo(Degrees, 0f, 0.3f, dt);
				}

				//DebugText = $"{(int)Degrees}";

				//Height = 32f + Utils.FastSin(TimeSinceSpawn) * 32f;

				CheckBounds();

				ZIndex = Hud.Instance.GetZIndex(Position.y);
			}

			Brightness = 1f;
			Scale = Utils.DynamicEaseTo(Scale, Utils.Map(Position.y, 0f, Hud.Instance.ScreenHeight, Globals.NEAR_SCALE, Globals.FAR_SCALE), 0.2f, dt);
			ShadowEmoji.Position = Position + new Vector2(0f, (Parent == null ? -25f : -45f) * Scale);
			ShadowEmoji.Opacity = Utils.DynamicEaseTo(ShadowEmoji.Opacity, 1f, 0.3f, dt);
			ShadowEmoji.ZIndex = Globals.DEPTH_SHADOW;
			ShadowEmoji.ScaleX = ScaleX * 1.25f * Utils.Map(Altitude, 0f, 600f, 1f, 1.2f);
			ShadowEmoji.ScaleY = ScaleY * 0.8f * Utils.Map(Altitude, 0f, 600f, 1f, 0.8f);
		}

		ShadowEmoji.Text = Text;
		ShadowEmoji.Degrees = Degrees;
		ShadowEmoji.Blur = Blur * 0.1f + Utils.DynamicEaseTo(ShadowEmoji.Blur, 6f, 0.2f, dt);
		ShadowEmoji.Scale = Utils.DynamicEaseTo(ShadowEmoji.Scale, Scale, 0.2f, dt);

		//DrawDebug();
	}

	public override void Hit(Vector2 hitPos)
	{
		base.Hit(hitPos);

		if(DidHitPlayer)
			return;
		
		if(Parent != null && Parent is FaceEmoji face)
		{
			if(face.HeldItem != null)
			{
				face.RemoveChild(face.HeldItem);
				face.HeldItem = null;
			}
		}

		var velAdd = (Position == hitPos ? Utils.GetRandomVector() : (Position - hitPos).Normal) * Game.Random.Float(80f, 400f);
		velAdd = new Vector2(velAdd.x, MathF.Abs(velAdd.y));
		Velocity += velAdd;

		ImpactEmoji impact = Stage.AddEmoji(new ImpactEmoji(), hitPos) as ImpactEmoji;

		Stage.TimeScale = MathF.Min(Game.Random.Float(0.5f, 0.6f), Stage.TimeScale);

		Gravity = Game.Random.Float(400f, 700f);
		IsThrownAtPlayer = false;
		ShouldRepel = true;
	}

	public void ThrowAtPlayer()
	{
		IsThrownAtPlayer = true;
		ThrowTime = Stage.CurrentTime;
		_throwStartPos = Position;
		_throwTargetPos = Vector2.Lerp(new Vector2(Position.x, 0f), new Vector2(Hud.Instance.ScreenWidth * 0.5f, 0f), Game.Random.Float(0.2f, 0.7f));
		Degrees = -45f - Utils.VectorToDegrees(_throwTargetPos - Position);
		ShouldRepel = false;
		_throwStartScale = Scale;
		_throwTotalTime = Utils.Map((_throwTargetPos - Position).Length, 0f, 1000f, 0.25f, 0.8f, EasingType.SineOut);
	}

	public void HitPlayer()
	{
		DidHitPlayer = true;
		HitPlayerTime = Stage.CurrentTime;
		Brightness = 1f;
		_bloodSprayDelay = Game.Random.Float(0.05f, 0.15f);

		Stage.CrosshairEmoji.Hurt();
	}
}
