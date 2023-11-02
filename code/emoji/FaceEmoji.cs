using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Numerics;
using static Sandbox.Package;

namespace EmojiEngine;

public class FaceEmoji : Emoji
{
	private bool _isPoked;
	public float LastPokedTime { get; private set; }
	public float TimeSincePoked => Stage.CurrentTime - LastPokedTime;
	private float _pokeTime;
	private const float POKE_TIME_MIN = 0.15f;
	private const float POKE_TIME_MAX = 0.2f;
	private const float POKE_SCALE = 1.2f;

	private float _rotTimeOffset;
	private float _rotSpeed;
	private float _rotAmount;

	public ShadowEmoji ShadowEmoji { get; set; }

	public const float RADIUS_SIZE_FACTOR = 0.56f;

	private float _pokedScale;
	private float _pokedScaleX;
	private float _pokedScaleY;

	public bool IsDead { get; private set; }
	private float _targetDeathDegrees;
	private float _deathRotateSpeed;
	private float _deathRotateSpeedEaseTime;
	private EasingType _deathRotateSpeedEasingType;
	private float _deathBrightness;
	private float _deathSepia;
	public float DeathTime { get; private set; }
	public float TimeSinceDeath => IsDead ? Stage.CurrentTime - DeathTime : 0f;

	public int BloodAmountLeft { get; set; }
	
	private static List<string> _faces = new() { "🙂", "😀", "😄", "🙁", "😕", "😐", "🙁", "🙄", "🤨", "😌", "🤤", "😴", "😛", "😗", "😊", "😉", };
	private static List<string> _deadFaces = new() { "😲", "😑", "😖", "😣", "😫", "😩", "😯", "😵", "😔", "😞", };

	public bool IsCivilian { get; private set; }

	public Emoji HeldItem { get; set; }

	private float _throwTimer;

	public BubbleEmoji Bubble { get; set; }

	public FaceEmoji()
	{
		IsInteractable = true;
	}

	public override void Init()
	{
		base.Init();

		IsCivilian = Game.Random.Float(0f, 1f) < 0.05f;

		if(IsCivilian)
			Text = "👸";
		else
			Text = _faces[Game.Random.Int(0, _faces.Count - 1)];

		TransformOriginY = 0.75f;
		DetermineRotVars();

		SetFontSize(200f);
		Radius = FontSize * RADIUS_SIZE_FACTOR;

		ShadowEmoji = Stage.AddEmoji(new ShadowEmoji(), new Vector2(-999f, -999f)) as ShadowEmoji;
		ShadowEmoji.SetFontSize(FontSize * 0.75f);

		_pokedScale = 1f;

		BloodAmountLeft = Game.Random.Int(14, 16);

		Weight = 10f;

		if(Game.Random.Float(0f, 1f) < 0.4f)
			AddDecoration("🩹", new Vector2(-35f, 55f), 64f, 20f);
		else if(Game.Random.Float(0f, 1f) < 0.2f)
			AddDecoration("🎀", new Vector2(65f, 65f), 80f, -45f);
		else if(Game.Random.Float(0f, 1f) < 0.05f)
			AddDecoration("👁️", new Vector2(0f, 50f), 60f, 0f);
		else if(Game.Random.Float(0f, 1f) < 0.1f)
			AddDecoration("👃", new Vector2(0f, 0f), 50f, 0f);

		_throwTimer = Game.Random.Float(2f, 5f);
	}

	public override void Update(float dt)
	{
		base.Update(dt);

		if(_isPoked)
		{
			if(TimeSincePoked > _pokeTime)
			{
				_pokedScale = 1f;
				ScaleX = 1f;
				ScaleY = 1f;
				_isPoked = false;
			}
			else
			{
				_pokedScale = Utils.Map(TimeSincePoked, 0f, _pokeTime, POKE_SCALE, 1f, EasingType.BounceInOut);
				ScaleX = Utils.Map(TimeSincePoked, 0f, _pokeTime, _pokedScaleX, 1f, EasingType.BounceInOut);
				ScaleY = Utils.Map(TimeSincePoked, 0f, _pokeTime, _pokedScaleY, 1f, EasingType.BounceInOut);
			}
		}

		//float depthScale = Utils.Map(MathX.CeilToInt(Position.y / 100f) * 100f, 0f, Hud.Instance.ScreenHeight, 2f, 0.2f);
		float depthScale = Utils.Map(Position.y, 0f, Hud.Instance.ScreenHeight, Globals.NEAR_SCALE, Globals.FAR_SCALE);
		//float depthScale = 1f;

		float deathScale = (IsDead ? Utils.Map(TimeSinceDeath, 0f, 10f, 1f, 0.9f) : 1f);
		Scale = Utils.DynamicEaseTo(Scale, depthScale * _pokedScale * deathScale, 0.3f, dt);

		Degrees += Velocity.x * 0.1f * dt;

		ShadowEmoji.Text = Text;
		//ShadowEmoji.ScaleX = ScaleX * (1f + 0.25f * (IsDead ? -1f : 1f));
		//ShadowEmoji.ScaleY = ScaleY * (1f - 0.2f * (IsDead ? -1f : 1f));
		ShadowEmoji.Blur = Blur * 0.1f + Utils.DynamicEaseTo(ShadowEmoji.Blur, 6f, 0.2f, dt);
		ShadowEmoji.Scale = Utils.DynamicEaseTo(ShadowEmoji.Scale, Scale, 0.2f, dt);

		if(IsDead)
		{
			ScaleX = Utils.DynamicEaseTo(ScaleX, Utils.Map(TimeSinceDeath, 0f, 10f, 1f, 0.925f), 0.1f, dt);
			ScaleY = Utils.DynamicEaseTo(ScaleY, Utils.Map(TimeSinceDeath, 0f, 8f, 1f, 1.075f), 0.1f, dt);

			Degrees = Utils.DynamicEaseTo(Degrees, _targetDeathDegrees, _deathRotateSpeed * Utils.Map(TimeSinceDeath, 0f, _deathRotateSpeedEaseTime, 0f, 1f, _deathRotateSpeedEasingType), dt);
			ShadowEmoji.ScaleX = Utils.Map(TimeSinceDeath, 0f, 0.2f, ScaleX * 0.6f, ScaleX * 0.75f, EasingType.QuadOut);
			ShadowEmoji.ScaleY = Utils.Map(TimeSinceDeath, 0f, 0.2f, ScaleY * 1.5f, ScaleY * 1.2f, EasingType.QuadOut);
			//ShadowEmoji.ScaleX = ScaleX * 0.75f;
			//ShadowEmoji.ScaleY = ScaleY * 1.2f;
			ShadowEmoji.Degrees = _targetDeathDegrees;
			ShadowEmoji.Position = GetRotatedPos() + new Vector2(0f, -64f * Scale);

			Brightness = Utils.Map(TimeSinceDeath, 0f, 9f, 1f, _deathBrightness);
			Sepia = Utils.Map(TimeSinceDeath, 0f, 15f, 0f, _deathSepia);
		}
		else
		{
			ScaleX = Utils.DynamicEaseTo(ScaleX, 1f, 0.15f, dt);
			ScaleY = Utils.DynamicEaseTo(ScaleY, 1f, 0.15f, dt);

			Degrees = Utils.DynamicEaseTo(Degrees, Utils.FastSin(_rotTimeOffset + Stage.CurrentTime * _rotSpeed) * _rotAmount, 0.2f, dt);
			Velocity += Utils.GetRandomVector() * 1200f * dt;
			ShadowEmoji.ScaleX = ScaleX * 1.25f;
			ShadowEmoji.ScaleY = ScaleY * 0.8f;
			ShadowEmoji.Degrees = Degrees * 0.1f;
			ShadowEmoji.Position = GetRotatedPos() + new Vector2(0f, -64f * Scale);
		}

		Position += Velocity * dt;
		//Position = new Vector2(Position.x, Utils.Map(Utils.FastSin(TimeSinceSpawn * 0.6f), -1f, 1f, 300f, Hud.Instance.ScreenHeight - 50f));

		float deceleration = 3f;
		Velocity *= (1f - deceleration * dt);

		CheckBounds();

		ZIndex = Hud.Instance.GetZIndex(GetRotatedPos().y);

		//TransformOriginY = 0.5f + Utils.FastSin(TimeSinceSpawn) * 0.5f;
		//Blur = Utils.Map(y, centerY, y < centerY ? 0f : height, 0f, 10f, EasingType.QuadIn);

		if(HeldItem != null)
		{
			HeldItem.Position = GetRotatedPos(Position + new Vector2(-Radius * 0.9f, -Radius * 0.5f));
			HeldItem.ZIndex = ZIndex + Globals.DEPTH_INCREASE_HELD;
			HeldItem.Degrees = Utils.DynamicEaseTo(HeldItem.Degrees, 180f + 25f + Degrees, 0.2f, dt);
			HeldItem.Altitude = 0f;

			_throwTimer -= dt;
			if(_throwTimer < 0f && HeldItem is KnifeEmoji knife)
			{
				RemoveChild(knife);
				HeldItem = null;
				knife.ThrowAtPlayer();
			}
		}

		//DrawDebug();

		//DebugText = $"{Bubble == null}";

		//Hud.Instance.DrawLine(Position, AnchorPos, 4f, Color.White, 0f, 999);
		//Hud.Instance.DebugDisplay.Text = $"Screen.Width: {Hud.Instance.ScreenWidth}, Position.x: {Position.x}, Position.x * Hud.Instance.ScaleToScreen: {Position.x * Hud.Instance.ScaleToScreen}";
	}

	public override void Hit(Vector2 hitPos)
	{
		base.Hit(hitPos);

		//Hud.Instance.DrawLine(new Vector2(5f, 5f), hitPos, 4f, Color.Red, 0.5f);

		WoundEmoji wound = Stage.AddEmoji(new WoundEmoji(), hitPos) as WoundEmoji;
		AddChild(wound);
		wound.ZIndex = ZIndex + Globals.DEPTH_INCREASE_WOUND;

		wound.ParentOffsetDistance = (hitPos - AnchorPos).Length / Scale;
		wound.ParentOffsetDegrees = Utils.VectorToDegrees(hitPos - AnchorPos);
		wound.ParentStartDegrees = Degrees;

		_isPoked = true;
		_pokeTime = Game.Random.Float(POKE_TIME_MIN, POKE_TIME_MAX);
		_pokedScaleX = 1f + Game.Random.Float(0.1f, 0.2f) * (IsDead && MathF.Abs(Degrees) > 25f ? -1f : 1f);
		_pokedScaleY = 1f + Game.Random.Float(-0.2f, -0.1f) * (IsDead && MathF.Abs(Degrees) > 25f ? -1f : 1f);
		LastPokedTime = Stage.CurrentTime;

		Velocity += new Vector2(0f, 1f) * Game.Random.Float(50f, 140f) * Utils.Map(TimeSinceDeath, 0f, 1f, 1f, 0.3f);

		if(!IsDead)
		{
			Degrees = 0f;
			DetermineRotVars();
			Stage.TimeScale = MathF.Min(Game.Random.Float(0.1f, 0.5f), Stage.TimeScale);
			Die();
		}

		AddBubble(BubbleMode.Yell, "❗️", 2f, leftSide: Bubble?.LeftSide ?? false);
	}

	public void Die()
	{
		if(IsDead)
			return;

		IsDead = true;
		DeathTime = Stage.CurrentTime;
		//TransformOriginY = 0.5f;
		_targetDeathDegrees = 90f * (Game.Random.Float(0f, 1f) < 0.5f ? -1f : 1f);
		_deathRotateSpeed = Game.Random.Float(0.01f, 0.25f);
		_deathRotateSpeedEaseTime = Game.Random.Float(0.1f, 1.5f);
		_deathRotateSpeedEasingType = GetRandomEasingType();
		_deathBrightness = Game.Random.Float(0.35f, 0.55f);
		_deathSepia = Game.Random.Float(0.1f, 0.25f);

		if(!IsCivilian)
			Text = _deadFaces[Game.Random.Int(0, _deadFaces.Count - 1)];

		if(HeldItem != null)
		{
			RemoveChild(HeldItem);
			HeldItem = null;
		}
	}

	void DetermineRotVars()
	{
		_rotTimeOffset = Game.Random.Float(0f, 99f);
		_rotSpeed = Game.Random.Float(1.5f, 7f);
		_rotAmount = Game.Random.Float(5f, 30f);
	}

	EasingType GetRandomEasingType()
	{
		int rand = Game.Random.Int(0, 5);
		switch(rand)
		{
			case 0: default: return EasingType.Linear;
			case 1: return EasingType.QuadOut;
			case 2: return EasingType.QuadIn;
			case 3: return EasingType.ExpoOut;
			case 4: return EasingType.ExpoIn;
			case 5: return EasingType.QuintIn;
		}
	}

	public void AddDecoration(string text, Vector2 offset, float fontSize, float degrees)
	{
		var decorationPos = Position + offset;
		FaceDecorationEmoji decoration = Stage.AddEmoji(new FaceDecorationEmoji(), decorationPos) as FaceDecorationEmoji;
		AddChild(decoration);
		decoration.Text = text;
		decoration.SetFontSize(fontSize);
		decoration.ZIndex = ZIndex + Globals.DEPTH_INCREASE_DECORATION;
		decoration.ParentOffsetDistance = (decorationPos - AnchorPos).Length / Scale;
		decoration.ParentOffsetDegrees = Utils.VectorToDegrees(decorationPos - AnchorPos);
		decoration.ParentStartDegrees = Degrees;
		decoration.StartDegrees = decoration.Degrees = degrees;
	}

	public void AddBubble(BubbleMode mode, string text, float lifetime, bool leftSide = false)
	{
		if(Bubble == null)
		{
			Bubble = Stage.AddEmoji(new BubbleEmoji(), Position) as BubbleEmoji;
			AddChild(Bubble);
		}

		Bubble.SetBubbleMode(mode, leftSide);
		Bubble.SetInnerEmoji(text);
		Bubble.Lifetime = lifetime;
	}

	public void AddBubble(BubbleMode mode, string text1, string text2, float lifetime, bool leftSide = false)
	{
		if(Bubble == null)
		{
			Bubble = Stage.AddEmoji(new BubbleEmoji(), Position) as BubbleEmoji;
			AddChild(Bubble);
		}

		Bubble.SetBubbleMode(mode, leftSide);
		Bubble.SetInnerEmoji(text1, text2);
		Bubble.Lifetime = lifetime;
	}
}
