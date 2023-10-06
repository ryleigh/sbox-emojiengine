using Sandbox;
using Sandbox.Internal;
using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace EmojiEngine;

public partial class Hud : RootPanel, Sandbox.Menu.IGameMenuPanel
{
	public static Hud Instance { get; private set; }
	public DebugDisplay DebugDisplay { get; private set; }

	public List<Emoji> Emojis { get; private set; }
	public EEGame EEGame { get; set; }
	public EmojiDisplay EmojiDisplay { get; private set; }

	public List<LineData> Lines { get; private set; }

	public float ScreenWidth => Screen.Width * ScaleFromScreen;
	public float ScreenHeight => Screen.Height * ScaleFromScreen;
	public Vector2 MousePos => new Vector2(MousePosition.x, Screen.Height - MousePosition.y) * ScaleFromScreen;
	public Vector2 LastMousePos { get; private set; }
	public Vector2 MouseDelta => MousePos - LastMousePos;

	public bool MouseDownLeft { get; private set; }
	public bool MouseDownRight { get; private set; }

	public List<Emoji> AllHoveredEmojis = new();
	public Emoji HoveredEmoji { get; private set; }

	public Color BgColorBottom { get; set; }
	public Color BgColorTop { get; set; }

	public List<RingData> Rings { get; private set; }

	private List<FaceEmoji> _faceEmojis = new();
	public CursorEmoji CursorEmoji { get; private set; }

	public const float BOUNDS_BUFFER = 40f;

	public Hud()
	{
		Instance = this;
		StyleSheet.Load("/ui/Hud.scss");

		Emojis = new();
		Lines = new();
		Rings = new();

		EmojiDisplay = AddChild<EmojiDisplay>();
		DebugDisplay = AddChild<DebugDisplay>();

		Restart();
	}

	public void Restart()
	{
		Emojis.Clear();
		CursorEmoji = AddEmoji(new CursorEmoji(), new Vector2(-999f, -999f)) as CursorEmoji;

		Lines.Clear();
		Rings.Clear();
		AllHoveredEmojis.Clear();
		_faceEmojis.Clear();

		DebugDisplay.Text = "";

		for(int i = 0; i < 30; i++)
		{
			//var emoji = AddEmoji(new FaceEmoji(), new Vector2(Game.Random.Float(BOUNDS_BUFFER, ScreenWidth - BOUNDS_BUFFER), Game.Random.Float(BOUNDS_BUFFER, ScreenHeight - BOUNDS_BUFFER)));
			var emoji = AddEmoji(new FaceEmoji(), new Vector2(ScreenWidth / 2f, ScreenHeight / 2f));
		}

		//AddEmoji(new ShadowEmoji(), new Vector2(600f, 600f));
	}

	public override void Tick()
	{
		base.Tick();
		float dt = Time.Delta;
		
		HandleEmoji(dt);
		HandleLines(dt);
		HandleRings(dt);

		if(Input.Pressed("Restart"))
			Restart();

		float HITBOX_RADIUS = 0.7f;
		float REPEL_STRENGTH = 125f;
		for(int i = _faceEmojis.Count - 1; i >= 0; i--)
		{
			var face = _faceEmojis[i];
			if(face.IsBeingDragged)
				continue;

			float faceRadius = face.Radius * HITBOX_RADIUS;

			for(int j = _faceEmojis.Count - 1; j >= 0; j--)
			{
				var other = _faceEmojis[j];

				if(other == face)
					continue;

				float otherRadius = other.Radius * HITBOX_RADIUS;

				float distSqr = (face.Position - other.Position).LengthSquared;
				float reqDistSqr = MathF.Pow(faceRadius + otherRadius, 2f);
				if(distSqr < reqDistSqr)
				{
					float percent = Utils.Map(distSqr, reqDistSqr, 0f, 0f, 1f);
					float repelStrength = REPEL_STRENGTH * Utils.Map(other.FontSize, FaceEmoji.FONT_SIZE_MIN, FaceEmoji.FONT_SIZE_MAX, 0.3f, 1f);
					face.Velocity += (face.Position == other.Position)
						? Utils.GetRandomVector() * repelStrength
						: (face.Position - other.Position).Normal * percent * repelStrength;
				}
			}
		}

		//DebugDisplay.Text = $"{_faceEmojis.Count}";

		//Log.Info($"{Input.Pressed("Restart")}, {Input.Pressed("attack1")}");

		//Log.Info($"{ScreenWidth} / {ScreenHeight}");
		//Log.Info($"{MathF.Round(MousePos.x)}, {MathF.Round(MousePos.y)}");

		//if((MousePos - LastMousePos).LengthSquared > 0f && (MousePos - LastMousePos).LengthSquared < MathF.Pow(300f, 2f))
		//	DrawLine(LastMousePos, MousePos, 1f, new Color(0.75f, 0.75f, 1f, 0.2f), 0.1f);

		//DrawLine(new Vector2(20f, 20f), new Vector2(200f, 200f), 3f, Color.Red, 0.5f);

		LastMousePos = MousePos;

		BgColorBottom = new Color(0.05f + Utils.FastSin(Time.Now * 0.15f) * 0.05f, 0.05f + Utils.FastSin(20f + Time.Now * 0.13f) * 0.05f, 0.05f + Utils.FastSin(10f + Time.Now * 0.17f) * 0.05f);
		BgColorTop = new Color(0.35f + Utils.FastSin(30f + Time.Now * 0.12f) * 0.15f, 0.35f + Utils.FastSin(Time.Now * 0.11f) * 0.1f, 0.35f + Utils.FastSin(5f + Time.Now * 0.09f) * 0.2f);
		//Style.BackgroundColor = BgColor;
	}

	void HandleEmoji(float dt)
	{
		var mousePos = MousePos;
		AllHoveredEmojis.Clear();
		for(int i = Emojis.Count - 1; i >= 0; i--)
		{
			var emoji = Emojis[i];
			emoji.Update(dt);

			emoji.IsHovered = false;

			if(emoji.Interactable && emoji.Radius > 0f)
			{
				var distSqr = (mousePos - emoji.Position).LengthSquared;
				if(distSqr < MathF.Pow(emoji.Radius, 2f))
					AllHoveredEmojis.Add(emoji);
			}
		}

		HoveredEmoji = AllHoveredEmojis.Count > 0 ? AllHoveredEmojis.OrderByDescending(x => x.ZIndex).First() : null;
		if(HoveredEmoji != null)
			HoveredEmoji.IsHovered = true;
	}

	void HandleLines(float dt)
	{
		for(int i = Lines.Count - 1; i >= 0; i--)
		{
			var line = Lines[i];

			if(Time.Now > line.spawnTime + line.lifetime)
				Lines.RemoveAt(i);
		}
	}

	void HandleRings(float dt)
	{
		for(int i = Rings.Count - 1; i >= 0; i--)
		{
			var ring = Rings[i];

			float progress = Utils.Map(Time.Now, ring.spawnTime, ring.spawnTime + ring.lifetime, 0f, 1f);
			float radius = Utils.Map(progress, 0f, 1f, ring.startRadius, ring.endRadius, EasingType.QuadOut);
			float width = Utils.Map(progress, 0f, 1f, ring.startWidth, ring.endWidth, EasingType.QuadOut);
			Color baseColor = Color.Lerp(Color.Black, ring.color, Utils.Map(progress, 0f, 0.5f, 0f, 1f, EasingType.QuadOut));
			Color color = baseColor.WithAlpha(Utils.Map(progress, 0.5f, 1f, ring.color.a, 0f, EasingType.QuadOut));
			var circleProgress = Utils.Map(progress, 0f, 1f, 0f, 1f, EasingType.SineIn);
			float startingAngle = Utils.FastSin(Time.Now * 2f);

			Utils.DrawCircle(ring.pos, radius, ring.numSegments, startingAngle, color, width, 0f, ring.zIndex, circleProgress);

			if(Time.Now > ring.spawnTime + ring.lifetime)
				Rings.RemoveAt(i);
		}
	}

	protected override void OnMouseDown(MousePanelEvent e)
	{
		base.OnClick(e);
		bool rightClick = e.Button == "mouseright";

		if(rightClick)
			MouseDownRight = true;
		else
			MouseDownLeft = true;

		ProcessMouseEvent(rightClick, down: true);

		if(rightClick && HoveredEmoji != null && HoveredEmoji is FaceEmoji faceEmoji)
			CursorEmoji.StartDragging(faceEmoji);
	}

	protected override void OnMouseUp(MousePanelEvent e)
	{
		base.OnMouseUp(e);
		bool rightClick = e.Button == "mouseright";

		if(rightClick)
			MouseDownRight = false;
		else
			MouseDownLeft = false;

		CursorEmoji.BounceScale(1.1f, 0.1f);

		ProcessMouseEvent(rightClick, down: false);

		if(rightClick && CursorEmoji.IsDragging)
			CursorEmoji.StopDragging();
	}

	void ProcessMouseEvent(bool rightClick, bool down)
	{
		var emojis = Emojis
			.Where(x => x.Interactable)
			.Where(x => (x.Position - MousePos).LengthSquared < MathF.Pow(x.Radius, 2f))
			.OrderBy(x => x.ZIndex)
			.ToList();

		for(int i = emojis.Count - 1; i >= 0; i--)
		{
			var emoji = emojis[i];

			if(down)
				emoji.OnMouseDown(rightClick);
			else
				emoji.OnMouseUp(rightClick);

			if(emoji.SwallowClicks)
				break;
		}
	}

	//public Emoji AddEmoji(string text, Vector2 pos)
	//{
	//	Emoji emoji = new Emoji();
	//	emoji.Text = text;
	//	emoji.Position = pos;
	//	Emojis.Add(emoji);

	//	return emoji;
	//}

	public Emoji AddEmoji(Emoji emoji, Vector2 pos)
	{
		Emojis.Add(emoji);

		if(emoji is FaceEmoji faceEmoji)
			_faceEmojis.Add(faceEmoji);

		emoji.Position = pos;
		return emoji;
	}

	public void RemoveEmoji(Emoji emoji)
	{
		Emojis.Remove(emoji);

		if(emoji is FaceEmoji faceEmoji)
			_faceEmojis.Remove(faceEmoji);

		if(HoveredEmoji == emoji)
			HoveredEmoji = null;
	}

	public void DrawLine(Vector2 posA, Vector2 posB, float thickness, Color color, float lifetime = 0f, int zIndex = 0)
	{
		Lines.Add(new LineData(posA, posB, thickness, color, Time.Now, lifetime, zIndex));
	}

	public void AddRing(Vector2 pos, Color color, float lifetime, float startRadius, float endRadius, float startWidth, float endWidth, int numSegments, int zIndex = 0)
	{
		Rings.Add(new RingData(pos, color, Time.Now, lifetime, zIndex, startRadius, endRadius, startWidth, endWidth, numSegments));
	}
}
