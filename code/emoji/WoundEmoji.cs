using Sandbox;
using System;

namespace EmojiEngine;

public class WoundEmoji : Emoji
{
	public float ParentOffsetDistance { get; set; }
	public float ParentOffsetDegrees { get; set; }
	public float ParentStartDegrees { get; set; }

	public float Lifetime { get; set; }

	private float _brightness;
	private float _brightnessTime;

	private float _startDegrees;

	private float _countdownToDrip;

	public ImpactEmoji ImpactEmoji { get; set; }

	public override void Init()
	{
		base.Init();

		Saturation = Game.Random.Float(1f, 2f);
		ScaleX = Game.Random.Float(1f, 1.1f);
		ScaleY = Game.Random.Float(0.9f, 1f);
		Lifetime = Game.Random.Float(6f, 6.5f);
		PanelSizeFactor = 2f;
		Degrees = _startDegrees = Game.Random.Float(0, 360f);

		float rand = Game.Random.Float(0f, 1f);
		if(rand < 0.5f)
		{
			float rand2 = Game.Random.Float(0f, 1f);
			if(rand2 < 0.7f)		Text = "🍁";
			else					Text = "🥮";

			SetFontSize(Game.Random.Float(20f, 22f));
			_brightness = Game.Random.Float(4f, 5f);
		}
		else
		{
			float rand2 = Game.Random.Float(0f, 1f);
			if(rand2 < 0.1f)		Text = "🍀";
			else if(rand2 < 0.2f)	Text = "🍅";
			else if(rand2 < 0.3f)	Text = "💱";
			else if(rand2 < 0.5f)	Text = "🎃";
			else if(rand2 < 0.6f)	Text = "🧼";
			else if(rand2 < 0.7f)	Text = "🧅";
			else if(rand2 < 0.8f)	Text = "🌰";
			else if(rand2 < 0.9f)	Text = "🧆";
			else                    Text = "🧩";

			SetFontSize(Game.Random.Float(16f, 18f));
			_brightness = 0f;
		}

		_brightnessTime = Game.Random.Float(0.2f, 0.35f);

		_countdownToDrip = Game.Random.Float(0.075f, 1.1f);

		TextStrokeColor = Color.Red;

		ImpactEmoji = Stage.AddEmoji(new ImpactEmoji(), Position) as ImpactEmoji;
		ImpactEmoji.SetFontSize(Game.Random.Float(60f, 105f));
		ImpactEmoji.ZIndex = ZIndex + 1;
	}

	public override void Update(float dt)
	{
		base.Update(dt);

		if(Parent != null)
		{
			//ZIndex = ParentFace.ZIndex + (int)Utils.Map(TimeSinceSpawn, 0f, Lifetime, 5f, 2f, EasingType.Linear);
			ZIndex = Parent.ZIndex + Globals.DEPTH_INCREASE_WOUND;
			float parentDegreesDiff = (Parent.Degrees - ParentStartDegrees);
			Position = Parent.AnchorPos + Utils.DegreesToVector(ParentOffsetDegrees - parentDegreesDiff) * ParentOffsetDistance * Parent.Scale;
			Degrees = _startDegrees + parentDegreesDiff;
		}

		//Log.Info($"parent: {ParentFace.ZIndex}, wound: {ZIndex}");

		Opacity = Utils.Map(TimeSinceSpawn, 0f, Lifetime, 1f, 0f, EasingType.ExpoIn);
		Brightness = Utils.Map(TimeSinceSpawn, 0f, _brightnessTime, _brightness, 0f, EasingType.QuadOut);
		Blur = Utils.Map(TimeSinceSpawn, 0f, Lifetime * 0.25f, 7f, 5f, EasingType.QuadOut);
		Scale = (Parent?.Scale ?? 1f) * Utils.Map(TimeSinceSpawn, 0f, Lifetime * 0.15f, 1.25f, 1f, EasingType.QuadOut);

		TextStroke = Utils.Map(TimeSinceSpawn, 0f, Lifetime * 0.2f, 6f, 0f, EasingType.Linear);

		if(IsFirstUpdate)
			SpawnBloodSpray();

		if(TimeSinceSpawn < Lifetime * 0.8f)
		{
			_countdownToDrip -= dt;
			if(_countdownToDrip < 0f)
			{
				SpawnBloodDrip();
				_countdownToDrip = Game.Random.Float(0.5f, 2f) * Utils.Map(TimeSinceSpawn, 0f, Lifetime * 0.8f, 0.8f, 3f, EasingType.QuadIn);
			}
		}

		if(ImpactEmoji != null)
		{
			ImpactEmoji.Position = Position;
			ImpactEmoji.ZIndex = ZIndex + 1;
		}

		if(TimeSinceSpawn > Lifetime)
			Stage.RemoveEmoji(this);
	}

	void SpawnBloodSpray()
	{
		if(Parent is not FaceEmoji face)
			return;

		if(face.BloodAmountLeft <= 0)
			return;

		face.BloodAmountLeft -= 1;

		var parentPos = face.GetRotatedPos();

		BloodSprayEmoji spray = Stage.AddEmoji(new BloodSprayEmoji(), Position) as BloodSprayEmoji;
		spray.ZIndex = ZIndex + 2;
		spray.Velocity = (Position - parentPos).Normal * Game.Random.Float(500f, 1200f);

		var dir = (Position - parentPos).Normal;
		spray.FlipX = dir.x < 0f;
		spray.Degrees = -Utils.VectorToDegrees(Position - parentPos) + (spray.FlipX ? 180f : 0f);
		spray.RotateSpeed = Game.Random.Float(140f, 350f) * Utils.Map(MathF.Abs(dir.y), 0f, 1f, 1f, 0f) * (spray.FlipX ? -1f : 1f);
		spray.GravityModifier = Utils.Map(MathF.Abs(dir.y), 0f, 1f, 1.5f, 0.5f) * Game.Random.Float(0.9f, 1.1f);
		spray.GroundYPos = face != null ? parentPos.y - face.Radius * 1.25f : -999f;
	}

	void SpawnBloodDrip()
	{
		if(Parent is not FaceEmoji face)
			return;

		if(face.BloodAmountLeft <= 0)
			return;

		face.BloodAmountLeft -= 2;

		BloodDripEmoji drip = Stage.AddEmoji(new BloodDripEmoji(), Position + new Vector2(0f, -5f)) as BloodDripEmoji;
		AddChild(drip);
		drip.ZIndex = ZIndex + 1;
		drip.WoundPosLast = Position;
		drip.GroundYPos = face != null ? face.GetRotatedPos().y - face.Radius * 1.25f : -999f;
	}
}
