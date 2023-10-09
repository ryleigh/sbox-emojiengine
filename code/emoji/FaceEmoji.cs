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
	private TimeSince _timeSincePoked;
	private float _pokeTime;
	private const float POKE_TIME_MIN = 0.15f;
	private const float POKE_TIME_MAX = 0.2f;
	private const float POKE_SCALE = 1.2f;

	private float _rotTimeOffset;
	private float _rotSpeed;
	private float _rotAmount;

	public bool IsBeingDragged { get; set; }
	public bool IsBeingPressed { get; set; }

	public Emoji ShadowEmoji { get; set; }
	private float _shadowHeight;

	public Vector2 Velocity { get; set; }
	public const float FONT_SIZE_MIN = 80f;
	public const float FONT_SIZE_MAX = 150f;

	public const float RADIUS_SIZE_FACTOR = 0.56f;

	float _pokedScale;

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
			if(_timeSincePoked > _pokeTime)
			{
				_pokedScale = 1f;
				_isPoked = false;
			}
			else
			{
				_pokedScale = Utils.Map(_timeSincePoked, 0f, _pokeTime, POKE_SCALE, 1f, EasingType.BounceInOut);
			}
		}

		//Position = new Vector2(900f + Utils.FastSin(Time.Now * 3f) * 10f, 600f + Utils.FastSin(Time.Now * 2f) * 10f);
		//Opacity = 0.5f + Utils.FastSin(Time.Now * 1.37f) * 0.5f;

		if(IsBeingPressed && !IsHovered)
			IsBeingPressed = false;

		float height = Hud.Instance.ScreenHeight;
		float y = Position.y;
		//float centerY = height / 2f;

		//float depthScale = Utils.Map(MathX.CeilToInt(Position.y / 100f) * 100f, 0f, Hud.Instance.ScreenHeight, 2f, 0.2f);
		//float depthScale = Utils.Map(Position.y, 0f, Hud.Instance.ScreenHeight, 2f, 0.2f);
		float depthScale = 1f;

		Scale = Utils.DynamicEaseTo(Scale, depthScale * _pokedScale * (IsHovered ? 1.1f : 1f) * (IsBeingPressed ? 0.9f : 1f), 0.3f, dt);

		//FontSize = _fontSize * _scale * (IsHovered ? 1.05f : 1f) * (IsBeingPressed ? 0.9f : 1f);
		//Radius = _fontSize * RADIUS_SIZE_FACTOR * _scale;

		if(!IsBeingDragged)
		{
			Degrees += Velocity.x * 0.1f * dt;

			Degrees = Utils.DynamicEaseTo(Degrees, Utils.FastSin(_rotTimeOffset + Time.Now * _rotSpeed) * _rotAmount, 0.2f, dt);
			ScaleX = Utils.DynamicEaseTo(ScaleX, 1f, 0.1f, dt);
			ScaleY = Utils.DynamicEaseTo(ScaleY, 1f, 0.1f, dt);

			Velocity += Utils.GetRandomVector() * 1200f * dt;

			Position += Velocity * dt;
			float deceleration = Utils.Map(FontSize, FONT_SIZE_MIN, FONT_SIZE_MAX, 3f, 5.5f);
			Velocity *= (1f - deceleration * dt);

			CheckBounds();
		}
		
		ZIndex = (int)(height - y);

		//Blur = Utils.Map(y, centerY, y < centerY ? 0f : height, 0f, 10f, EasingType.QuadIn);

		_shadowHeight = Utils.DynamicEaseTo(_shadowHeight, IsBeingDragged ? -65f : -40f, 0.2f, dt);
		ShadowEmoji.Text = Text;
		ShadowEmoji.Position = Position + new Vector2(Degrees * 0.4f * (IsBeingDragged ? -4f : 1f), _shadowHeight) * Utils.Map(FontSize, FONT_SIZE_MIN, FONT_SIZE_MAX, 0.8f, 1.3f);
		ShadowEmoji.ScaleX = ScaleX * 1.25f;
		ShadowEmoji.ScaleY = ScaleY * 0.8f;
		ShadowEmoji.Blur = Blur * 0.1f + Utils.DynamicEaseTo(ShadowEmoji.Blur, IsBeingDragged ? 8f : 6f, 0.2f, dt);
		ShadowEmoji.Scale = Utils.DynamicEaseTo(ShadowEmoji.Scale, Scale * (IsBeingDragged ? 0.8f : 1f), 0.2f, dt);
		ShadowEmoji.Degrees = Degrees * 0.3f;

		//Hud.Instance.DrawLine(Position, AnchorPos, 4f, Color.White, 0f, 999);

		//Hud.Instance.DebugDisplay.Text = $"Screen.Width: {Hud.Instance.ScreenWidth}, Position.x: {Position.x}, Position.x * Hud.Instance.ScaleToScreen: {Position.x * Hud.Instance.ScaleToScreen}";

		//Text = IsHovered ? "😲" : "😘";

		//opacity: @(0.5f + Utils.FastSin(Time.Now * 1.37f) * 0.5f);
		//text - shadow: 0 0 @(8f + Utils.FastSin(Time.Now * 2.55f) * 8f)px #000000;
		//text-stroke: @(8f + Utils.FastSin(Time.Now * 3.6f) * 8f)px @((Color.Lerp(new Color(1f, 0f, 0f), new Color(0f, 0f, 1f), 0.5f + Utils.FastSin(Time.Now * 3.33f) * 0.5f)).Rgba);
		//font - size: @(64f + Utils.FastSin(Time.Now * 1.5f) * 12f)px;
		//left: @(600f + Utils.FastSin(Time.Now * 3f) * 10f)px;
		//bottom: @(300f + Utils.FastSin(Time.Now * 2f) * 10f)px;
		//transform: scaleX(@((0.9f + Utils.FastSin(Time.Now * 4.5f) * 0.1f) * (Utils.FastSin(Time.Now * 1.1f) < 0f ? -1f : 1f)))
		//scaleY(@(0.95f + Utils.FastSin(Time.Now * 4.25f) * 0.05f));
		//transform: rotate(@(Utils.FastSin(Time.Now * 4.25f) * 10f)deg);

		//if(Radius > 0f)
		//	Utils.DrawCircle(Position, Radius, 12, Time.Now, Color.White, width: 1f, lifetime: 0f, zIndex: ZIndex + 1);
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

	//public override void OnClickedDown(bool rightClick)
	//{
	//	base.OnClickedDown(rightClick);

	//	if(!rightClick)
	//	{
	//		IsBeingPressed = true;
	//	}
	//}

	//public override void OnClickedUp(bool rightClick)
	//{
	//	base.OnClickedUp(rightClick);

	//	if(IsBeingPressed && !rightClick)
	//	{
	//		Hit();
	//	}
	//}

	public void Hit(Vector2 hitPos)
	{
		_isPoked = true;
		_pokeTime = Game.Random.Float(POKE_TIME_MIN, POKE_TIME_MAX);
		_timeSincePoked = 0f;

		//Text = "😲";
		//Text = GetFaceText();

		//var color = new Color(Game.Random.Float(0.5f, 1f), Game.Random.Float(0.5f, 1f), Game.Random.Float(0.5f, 1f));

		//int numSegments = Game.Random.Int(14, 20);
		//if(numSegments % 2 != 0)
		//	numSegments++;

		//Hud.Instance.AddRing(Position, color, Game.Random.Float(0.2f, 0.4f), Radius, Radius * Game.Random.Float(1.6f, 2f), 9f, 1f, numSegments, ZIndex - 1);

		Hud.Instance.CursorEmoji?.BounceScale(1.2f, 0.15f);

		IsBeingPressed = false;

		Degrees = 0f;
		DetermineRotVars();

		ExplosionEmoji explosion = Hud.Instance.AddEmoji(new ExplosionEmoji(), hitPos) as ExplosionEmoji;
		explosion.SetFontSize(FontSize * Game.Random.Float(0.6f, 0.8f));
		explosion.ZIndex = ZIndex + 1;
		explosion.FaceEmoji = this;

		Velocity += new Vector2(0f, 1f) * Game.Random.Float(30f, 80f);
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
