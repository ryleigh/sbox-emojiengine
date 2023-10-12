﻿using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;

namespace EmojiEngine;

public class FaceEmoji : Emoji
{
	private bool _isPoked;
	public float LastPokedTime { get; private set; }
	public float TimeSincePoked => Hud.Instance.CurrentTime - LastPokedTime;
	private float _pokeTime;
	private const float POKE_TIME_MIN = 0.15f;
	private const float POKE_TIME_MAX = 0.2f;
	private const float POKE_SCALE = 1.2f;

	private float _rotTimeOffset;
	private float _rotSpeed;
	private float _rotAmount;

	public Emoji ShadowEmoji { get; set; }
	private float _shadowHeight;

	public Vector2 Velocity { get; set; }
	public const float FONT_SIZE_MIN = 80f;
	public const float FONT_SIZE_MAX = 150f;

	public const float RADIUS_SIZE_FACTOR = 0.56f;

	private float _pokedScale;
	private float _pokedScaleX;
	private float _pokedScaleY;

	public bool IsDead { get; private set; }
	private float _targetDeathDegrees;
	private float _deathRotateSpeed;
	private float _deathBrightness;
	private float _deathSepia;
	public float DeathTime { get; private set; }
	public float TimeSinceDeath => Hud.Instance.CurrentTime - DeathTime;

	public int BloodAmountLeft { get; set; }
	
	private static List<string> _faces = new() { "🙂", "😀", "😄", "🙁", "😕", "😐", "🙁", "🙄", "🤨", "😌", "🤤", "😴", "😛", "😗", "😊", "😉", };
	private static List<string> _deadFaces = new() { "😲", "😑", "😖", "😣", "😫", "😩", "😯", "😦", "😧", "😵", "😔", "😞", "😟", "☹️", };


	//private static List<string> _faces = new() { "🙂", "🙄", "😱", "😐", "😔", "😋", "😇", "🤔", "😩", "😳", "😌", "🤗", "🤤", "😰", "😁", "🤨", "😡", "🥴", "🤓", "😫", "😒", "😜", "😬", "🙃", "🥱", "🧐", "😨",
	//		"😥", "😥", "😲", "😖", "😶", "🤧", "😤", "😑", "🥶", "😕", "😆", "🥳", "😞", "😮", "😓", "😀", "😃", "😄", "😵", "😛", "😢", "🤫", "👿", "😟", "😣", "😧", "☹️", "🤮", "🌝", "🐸", "😠", "😪", "😝", "🤐",
	//		"🌚", "😦", "😙", "😴", "🙁", "🤬", "🤯", "😗", "😯", "🤒", "😘", "😎", "🤡", "🥺", "🤕", "😏", "🤪", "💀", "🤣", "🥵", "🥰", "😈", "😭", "😍", "🤩", "😊", "😉", "😂", "🤭", "😚", "🤢", "😅", "☺️",
	//		"👹", "😷", "🤑", "🌞", "👽", "🤖", "👨‍🦲", "🎃", "🟡", "😺", "😸", "👸", "🎅", "👻", "👶", "👲", "👴", "🌎️", "🐞", };

	public FaceEmoji()
	{
		Text = _faces[Game.Random.Int(0, _faces.Count - 1)];
		TransformOriginY = 0.75f;

		DetermineRotVars();

		SetFontSize(Game.Random.Float(FONT_SIZE_MIN, FONT_SIZE_MAX) + 60f);
		Radius = FontSize * RADIUS_SIZE_FACTOR;

		ShadowEmoji = Hud.Instance.AddEmoji(new ShadowEmoji(), new Vector2(-999f, -999f));
		ShadowEmoji.SetFontSize(FontSize * Utils.Map(FontSize, FONT_SIZE_MIN, FONT_SIZE_MAX, 0.75f, 0.8f));

		_pokedScale = 1f;

		BloodAmountLeft = Game.Random.Int(14, 16);
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

		float height = Hud.Instance.ScreenHeight;
		float y = Position.y;
		//float centerY = height / 2f;

		//float depthScale = Utils.Map(MathX.CeilToInt(Position.y / 100f) * 100f, 0f, Hud.Instance.ScreenHeight, 2f, 0.2f);
		//float depthScale = Utils.Map(Position.y, 0f, Hud.Instance.ScreenHeight, 2f, 0.2f);
		float depthScale = 1f;

		float deathScale = (IsDead ? Utils.Map(TimeSinceDeath, 0f, 10f, 1f, 0.9f) : 1f);
		Scale = Utils.DynamicEaseTo(Scale, depthScale * _pokedScale * deathScale, 0.3f, dt);

		Degrees += Velocity.x * 0.1f * dt;

		_shadowHeight = Utils.DynamicEaseTo(_shadowHeight, -40f, 0.2f, dt);
		ShadowEmoji.Text = Text;
		//ShadowEmoji.ScaleX = ScaleX * (1f + 0.25f * (IsDead ? -1f : 1f));
		//ShadowEmoji.ScaleY = ScaleY * (1f - 0.2f * (IsDead ? -1f : 1f));
		ShadowEmoji.Blur = Blur * 0.1f + Utils.DynamicEaseTo(ShadowEmoji.Blur, 6f, 0.2f, dt);
		ShadowEmoji.Scale = Utils.DynamicEaseTo(ShadowEmoji.Scale, Scale, 0.2f, dt);

		if(IsDead)
		{
			ScaleX = Utils.DynamicEaseTo(ScaleX, Utils.Map(TimeSinceDeath, 0f, 10f, 1f, 0.925f), 0.1f, dt);
			ScaleY = Utils.DynamicEaseTo(ScaleY, Utils.Map(TimeSinceDeath, 0f, 8f, 1f, 1.075f), 0.1f, dt);

			Degrees = Utils.DynamicEaseTo(Degrees, _targetDeathDegrees, _deathRotateSpeed, dt);
			ShadowEmoji.ScaleX = ScaleX * 0.75f;
			ShadowEmoji.ScaleY = ScaleY * 1.2f;
			//ShadowEmoji.ScaleX = 1f - ScaleX;
			//ShadowEmoji.ScaleY = 1f - ScaleY;
			ShadowEmoji.Degrees = _targetDeathDegrees;
			ShadowEmoji.Position = GetRotatedPos() + new Vector2(0f, _shadowHeight) * Utils.Map(FontSize, FONT_SIZE_MIN, FONT_SIZE_MAX, 0.8f, 1.3f);

			Brightness = Utils.Map(TimeSinceDeath, 0f, 9f, 1f, _deathBrightness);
			Sepia = Utils.Map(TimeSinceDeath, 0f, 15f, 0f, _deathSepia);
		}
		else
		{
			ScaleX = Utils.DynamicEaseTo(ScaleX, 1f, 0.15f, dt);
			ScaleY = Utils.DynamicEaseTo(ScaleY, 1f, 0.15f, dt);

			Degrees = Utils.DynamicEaseTo(Degrees, Utils.FastSin(_rotTimeOffset + Hud.Instance.CurrentTime * _rotSpeed) * _rotAmount, 0.2f, dt);
			Velocity += Utils.GetRandomVector() * 1200f * dt;
			ShadowEmoji.ScaleX = ScaleX * 1.25f;
			ShadowEmoji.ScaleY = ScaleY * 0.8f;
			ShadowEmoji.Degrees = Degrees * 0.2f;
			ShadowEmoji.Position = Position + new Vector2(Degrees * 0.4f, _shadowHeight) * Utils.Map(FontSize, FONT_SIZE_MIN, FONT_SIZE_MAX, 0.8f, 1.3f);
		}

		Position += Velocity * dt;
		float deceleration = Utils.Map(FontSize, FONT_SIZE_MIN, FONT_SIZE_MAX, 3f, 5.5f);
		Velocity *= (1f - deceleration * dt);

		CheckBounds();
		
		ZIndex = (int)(height - y);

		//Blur = Utils.Map(y, centerY, y < centerY ? 0f : height, 0f, 10f, EasingType.QuadIn);

		//Hud.Instance.DrawLine(Position, AnchorPos, 4f, Color.White, 0f, 999);
		//Hud.Instance.DebugDisplay.Text = $"Screen.Width: {Hud.Instance.ScreenWidth}, Position.x: {Position.x}, Position.x * Hud.Instance.ScaleToScreen: {Position.x * Hud.Instance.ScaleToScreen}";
	}

	void CheckBounds()
	{
		if(Position.x < Hud.BOUNDS_BUFFER)
		{
			Position = new Vector2(Hud.BOUNDS_BUFFER, Position.y);
			Velocity = new Vector2(MathF.Abs(Velocity.x), Velocity.y);
		}
		else if(Position.x > Hud.Instance.ScreenWidth - Hud.BOUNDS_BUFFER)
		{
			Position = new Vector2(Hud.Instance.ScreenWidth - Hud.BOUNDS_BUFFER, Position.y);
			Velocity = new Vector2(-MathF.Abs(Velocity.x), Velocity.y);
		}

		if(Position.y < Hud.BOUNDS_BUFFER)
		{
			Position = new Vector2(Position.x, Hud.BOUNDS_BUFFER);
			Velocity = new Vector2(Velocity.x, MathF.Abs(Velocity.y));
		}
		else if(Position.y > Hud.Instance.ScreenHeight - Hud.BOUNDS_BUFFER)
		{
			Position = new Vector2(Position.x, Hud.Instance.ScreenHeight - Hud.BOUNDS_BUFFER);
			Velocity = new Vector2(Velocity.x, -MathF.Abs(Velocity.y));
		}
	}

	public void Hit(Vector2 hitPos)
	{
		//Hud.Instance.DrawLine(new Vector2(5f, 5f), hitPos, 4f, Color.Red, 0.5f);

		WoundEmoji wound = Hud.Instance.AddEmoji(new WoundEmoji(), hitPos) as WoundEmoji;
		wound.ZIndex = ZIndex + 1;
		wound.ParentFace = this;

		wound.ParentOffsetDistance = (hitPos - AnchorPos).Length / Scale;
		wound.ParentOffsetDegrees = Utils.VectorToDegrees(hitPos - AnchorPos);
		wound.ParentStartDegrees = Degrees;

		_isPoked = true;
		_pokeTime = Game.Random.Float(POKE_TIME_MIN, POKE_TIME_MAX);
		_pokedScaleX = 1f + Game.Random.Float(0.1f, 0.2f) * (IsDead ? -1f : 1f);
		_pokedScaleY = 1f + Game.Random.Float(-0.2f, -0.1f) * (IsDead ? -1f : 1f);
		LastPokedTime = Hud.Instance.CurrentTime;

		Velocity += new Vector2(0f, 1f) * Game.Random.Float(50f, 140f);

		if(!IsDead)
		{
			Degrees = 0f;
			DetermineRotVars();
			Hud.Instance.TimeScale = Game.Random.Float(0.1f, 0.5f);
			Die();
		}
	}

	public void Die()
	{
		if(IsDead)
			return;

		IsDead = true;
		DeathTime = Hud.Instance.CurrentTime;
		//TransformOriginY = 0.5f;
		_targetDeathDegrees = 90f * (Game.Random.Float(0f, 1f) < 0.5f ? -1f : 1f);
		_deathRotateSpeed = Game.Random.Float(0.01f, 0.1f);
		_deathBrightness = Game.Random.Float(0.45f, 0.55f);
		_deathSepia = Game.Random.Float(0.1f, 0.25f);
		Text = _deadFaces[Game.Random.Int(0, _deadFaces.Count - 1)];
	}

	void DetermineRotVars()
	{
		_rotTimeOffset = Game.Random.Float(0f, 99f);
		_rotSpeed = Game.Random.Float(1.5f, 7f);
		_rotAmount = Game.Random.Float(5f, 30f);
	}
}
