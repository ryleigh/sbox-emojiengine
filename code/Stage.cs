﻿using Sandbox;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EmojiEngine;

public class Stage
{
	public List<Emoji> Emojis { get; private set; }
	public List<Emoji> AllHoveredEmojis = new();
	public Emoji HoveredEmoji { get; private set; }

	public Color BgColorBottom { get; set; }
	public Color BgColorTop { get; set; }

	public List<RingData> Rings { get; private set; }
	public List<LineData> Lines { get; private set; }

	private List<FaceEmoji> _faceEmojis = new();
	public CursorEmoji CursorEmoji { get; private set; }
	public CrosshairEmoji CrosshairEmoji { get; private set; }

	public float CurrentTime { get; private set; }
	public float TimeScale { get; set; }

	public Stage()
	{
		Emojis = new();
		Lines = new();
		Rings = new();
	}

	public void Restart()
	{
		Emojis.Clear();

		Lines.Clear();
		Rings.Clear();
		AllHoveredEmojis.Clear();
		_faceEmojis.Clear();

		CurrentTime = 0f;
		TimeScale = 1f;

		//CursorEmoji = AddEmoji(new CursorEmoji(), new Vector2(-999f, -999f)) as CursorEmoji;
		CrosshairEmoji = AddEmoji(new CrosshairEmoji(), Hud.Instance.MousePos) as CrosshairEmoji;

		for(int i = 0; i < 25; i++)
		{
			//var emoji = AddEmoji(new FaceEmoji(), new Vector2(Game.Random.Float(BOUNDS_BUFFER, ScreenWidth - BOUNDS_BUFFER), Game.Random.Float(BOUNDS_BUFFER, ScreenHeight - BOUNDS_BUFFER)));
			var emoji = AddEmoji(new FaceEmoji(), new Vector2(Hud.Instance.ScreenWidth / 2f, Hud.Instance.ScreenHeight / 2f));
		}
	}

	public void Update(float dt)
	{
		float dtRaw = dt;
		dt *= TimeScale;
		
		CurrentTime += dt;

		HandleEmoji(dt);
		HandleLines(dt);
		HandleRings(dt);

		if(Input.Pressed("Restart"))
		{
			Restart();
			return;
		}

		float HITBOX_RADIUS = 0.7f;
		float REPEL_STRENGTH = 125f;
		for(int i = _faceEmojis.Count - 1; i >= 0; i--)
		{
			var face = _faceEmojis[i];

			float faceRadius = face.Radius * HITBOX_RADIUS;

			for(int j = _faceEmojis.Count - 1; j >= 0; j--)
			{
				var other = _faceEmojis[j];

				if(other == face)
					continue;

				var facePos = face.GetRotatedPos();
				var otherPos = other.GetRotatedPos();
				float otherRadius = other.Radius * HITBOX_RADIUS;

				float distSqr = (facePos - otherPos).LengthSquared;
				float reqDistSqr = MathF.Pow(faceRadius + otherRadius, 2f);
				if(distSqr < reqDistSqr)
				{
					float percent = Utils.Map(distSqr, reqDistSqr, 0f, 0f, 1f);
					float repelStrength = REPEL_STRENGTH * Utils.Map(other.FontSize, FaceEmoji.FONT_SIZE_MIN, FaceEmoji.FONT_SIZE_MAX, 10f, 50f);
					face.Velocity += (facePos == otherPos)
						? Utils.GetRandomVector() * repelStrength * dt
						: (facePos - otherPos).Normal * percent * repelStrength * dt;
				}
			}
		}

		TimeScale = Utils.DynamicEaseTo(TimeScale, 1f, Utils.Map(TimeScale, 0f, 1f, 0.01f, 0.1f), dtRaw);

		BgColorBottom = new Color(0.05f + Utils.FastSin(CurrentTime * 0.15f) * 0.05f, 0.05f + Utils.FastSin(20f + CurrentTime * 0.13f) * 0.05f, 0.05f + Utils.FastSin(10f + CurrentTime * 0.17f) * 0.05f);
		BgColorTop = new Color(0.35f + Utils.FastSin(30f + CurrentTime * 0.12f) * 0.15f, 0.35f + Utils.FastSin(CurrentTime * 0.11f) * 0.1f, 0.35f + Utils.FastSin(5f + CurrentTime * 0.09f) * 0.2f);
	}

	void HandleEmoji(float dt)
	{
		var mousePos = Hud.Instance.MousePos;
		AllHoveredEmojis.Clear();
		for(int i = Emojis.Count - 1; i >= 0; i--)
		{
			var emoji = Emojis[i];
			emoji.Update(dt);

			emoji.IsHovered = false;

			if(emoji.IsInteractable && emoji.Radius > 0f)
			{
				var distSqr = (mousePos - emoji.GetRotatedPos()).LengthSquared;
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

			if(CurrentTime > line.spawnTime + line.lifetime)
				Lines.RemoveAt(i);
		}
	}

	void HandleRings(float dt)
	{
		for(int i = Rings.Count - 1; i >= 0; i--)
		{
			var ring = Rings[i];

			float progress = Utils.Map(CurrentTime, ring.spawnTime, ring.spawnTime + ring.lifetime, 0f, 1f);
			float radius = Utils.Map(progress, 0f, 1f, ring.startRadius, ring.endRadius, EasingType.QuadOut);
			float width = Utils.Map(progress, 0f, 1f, ring.startWidth, ring.endWidth, EasingType.QuadOut);
			Color baseColor = Color.Lerp(Color.Black, ring.color, Utils.Map(progress, 0f, 0.5f, 0f, 1f, EasingType.QuadOut));
			Color color = baseColor.WithAlpha(Utils.Map(progress, 0.5f, 1f, ring.color.a, 0f, EasingType.QuadOut));
			var circleProgress = Utils.Map(progress, 0f, 1f, 0f, 1f, EasingType.SineIn);
			float startingAngle = Utils.FastSin(CurrentTime * 2f);

			Utils.DrawCircle(ring.pos, radius, ring.numSegments, startingAngle, color, width, 0f, ring.zIndex, circleProgress);

			if(CurrentTime > ring.spawnTime + ring.lifetime)
				Rings.RemoveAt(i);
		}
	}

	public void OnMouseDown(bool rightClick)
	{
		for(int i = Emojis.Count - 1; i >= 0; i--)
			Emojis[i].OnMouseDown(rightClick);
	}

	public void OnMouseUp(bool rightClick)
	{
		for(int i = Emojis.Count - 1; i >= 0; i--)
			Emojis[i].OnMouseUp(rightClick);
	}

	public bool Raycast(Vector2 pos, out List<Emoji> hitEmojis)
	{
		hitEmojis = Emojis
			.Where(x => x.IsInteractable)
			.Where(x => (x.GetRotatedPos() - pos).LengthSquared < MathF.Pow(x.Radius, 2f))
			.OrderByDescending(x => x.ZIndex)
			.ToList();

		return hitEmojis.Count > 0;
	}

	public Emoji AddEmoji(Emoji emoji, Vector2 pos)
	{
		Emojis.Add(emoji);

		if(emoji is FaceEmoji faceEmoji)
			_faceEmojis.Add(faceEmoji);

		emoji.Position = pos;
		emoji.Stage = this;
		emoji.Init();

		return emoji;
	}

	public Emoji AddEmoji(Emoji emoji)
	{
		return AddEmoji(emoji, Vector2.Zero);
	}

	public void RemoveEmoji(Emoji emoji)
	{
		if(!Emojis.Contains(emoji))
			return;

		Emojis.Remove(emoji);

		if(emoji is FaceEmoji faceEmoji)
			_faceEmojis.Remove(faceEmoji);

		if(HoveredEmoji == emoji)
			HoveredEmoji = null;
	}

	public void DrawLine(Vector2 posA, Vector2 posB, float thickness, Color color, float lifetime = 0f, int zIndex = 99999, float invert = 0f, float saturation = 1f, float blur = 0f)
	{
		Lines.Add(new LineData(posA, posB, thickness, color, CurrentTime, lifetime, zIndex, invert, saturation, blur));
	}

	public void AddRing(Vector2 pos, Color color, float lifetime, float startRadius, float endRadius, float startWidth, float endWidth, int numSegments, int zIndex = 0)
	{
		Rings.Add(new RingData(pos, color, CurrentTime, lifetime, zIndex, startRadius, endRadius, startWidth, endWidth, numSegments));
	}
}
