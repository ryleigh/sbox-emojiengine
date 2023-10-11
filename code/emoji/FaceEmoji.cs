using Sandbox;
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

	private static List<string> _faces = new() { "🙂", "🙄", "😱", "😐", "😔", "😋", "😇", "🤔", "😩", "😳", "😌", "🤗", "🤤", "😰", "😁", "🤨", "😡", "🥴", "🤓", "😫", "😒", "😜", "😬", "🙃", "🥱", "🧐", "😨",
			"😥", "😥", "😲", "😖", "😶", "🤧", "😤", "😑", "🥶", "😕", "😆", "🥳", "😞", "😮", "😓", "😀", "😃", "😄", "😵", "😛", "😢", "🤫", "👿", "😟", "😣", "😧", "☹️", "🤮", "🌝", "🐸", "😠", "😪", "😝", "🤐",
			"🌚", "😦", "😙", "😴", "🙁", "🤬", "🤯", "😗", "😯", "🤒", "😘", "😎", "🤡", "🥺", "🤕", "😏", "🤪", "💀", "🤣", "🥵", "🥰", "😈", "😭", "😍", "🤩", "😊", "😉", "😂", "🤭", "😚", "🤢", "😅", "☺️",
			"👹", "😷", "🤑", "🌞", "👽", "🤖", "👨‍🦲", "🎃", "🟡", "😺", "😸", "👸", "🎅", "👻", "👶", "👲", "👴", "🌎️", "🐞", };

	public FaceEmoji()
	{
		Text = GetFaceText();
		TransformOriginY = BaseTransformOriginY = 0.75f;

		DetermineRotVars();

		SetFontSize(Game.Random.Float(FONT_SIZE_MIN, FONT_SIZE_MAX));
		Radius = FontSize * RADIUS_SIZE_FACTOR;

		ShadowEmoji = Hud.Instance.AddEmoji(new ShadowEmoji(), new Vector2(-999f, -999f));
		ShadowEmoji.SetFontSize(FontSize * Utils.Map(FontSize, FONT_SIZE_MIN, FONT_SIZE_MAX, 0.75f, 0.8f));

		_pokedScale = 1f;
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

		Scale = Utils.DynamicEaseTo(Scale, depthScale * _pokedScale, 0.3f, dt);

		Degrees += Velocity.x * 0.1f * dt;

		Degrees = Utils.DynamicEaseTo(Degrees, Utils.FastSin(_rotTimeOffset + Hud.Instance.CurrentTime * _rotSpeed) * _rotAmount, 0.2f, dt);
		ScaleX = Utils.DynamicEaseTo(ScaleX, 1f, 0.1f, dt);
		ScaleY = Utils.DynamicEaseTo(ScaleY, 1f, 0.1f, dt);

		Velocity += Utils.GetRandomVector() * 1200f * dt;

		Position += Velocity * dt;
		float deceleration = Utils.Map(FontSize, FONT_SIZE_MIN, FONT_SIZE_MAX, 3f, 5.5f);
		Velocity *= (1f - deceleration * dt);

		CheckBounds();
		
		ZIndex = (int)(height - y);

		//Blur = Utils.Map(y, centerY, y < centerY ? 0f : height, 0f, 10f, EasingType.QuadIn);

		_shadowHeight = Utils.DynamicEaseTo(_shadowHeight, -40f, 0.2f, dt);
		ShadowEmoji.Text = Text;
		ShadowEmoji.Position = Position + new Vector2(Degrees * 0.4f, _shadowHeight) * Utils.Map(FontSize, FONT_SIZE_MIN, FONT_SIZE_MAX, 0.8f, 1.3f);
		ShadowEmoji.ScaleX = ScaleX * 1.25f;
		ShadowEmoji.ScaleY = ScaleY * 0.8f;
		ShadowEmoji.Blur = Blur * 0.1f + Utils.DynamicEaseTo(ShadowEmoji.Blur, 6f, 0.2f, dt);
		ShadowEmoji.Scale = Utils.DynamicEaseTo(ShadowEmoji.Scale, Scale, 0.2f, dt);
		ShadowEmoji.Degrees = Degrees * 0.3f;

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
		_isPoked = true;
		_pokeTime = Game.Random.Float(POKE_TIME_MIN, POKE_TIME_MAX);
		_pokedScaleX = Game.Random.Float(1.15f, 1.3f);
		_pokedScaleY = Game.Random.Float(0.7f, 0.85f);
		LastPokedTime = Hud.Instance.CurrentTime;

		//Text = "😲";
		//Text = GetFaceText();

		//var color = new Color(Game.Random.Float(0.5f, 1f), Game.Random.Float(0.5f, 1f), Game.Random.Float(0.5f, 1f));

		//int numSegments = Game.Random.Int(14, 20);
		//if(numSegments % 2 != 0)
		//	numSegments++;

		//Hud.Instance.AddRing(Position, color, Game.Random.Float(0.2f, 0.4f), Radius, Radius * Game.Random.Float(1.6f, 2f), 9f, 1f, numSegments, ZIndex - 1);

		Hud.Instance.CursorEmoji?.BounceScale(1.2f, 0.15f);

		Degrees = 0f;
		DetermineRotVars();

		Velocity += new Vector2(0f, 1f) * Game.Random.Float(50f, 140f);

		Hud.Instance.TimeScale = Game.Random.Float(0.1f, 0.5f);
	}

	void DetermineRotVars()
	{
		_rotTimeOffset = Game.Random.Float(0f, 99f);
		_rotSpeed = Game.Random.Float(1.5f, 7f);
		_rotAmount = Game.Random.Float(5f, 30f);
	}

	public string GetFaceText()
	{
		return _faces[Game.Random.Int(0, _faces.Count - 1)];
	}
}
