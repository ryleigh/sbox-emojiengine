using Sandbox;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EmojiEngine;

public class Stage
{
	public List<Emoji> Emojis { get; private set; }
	public List<Emoji> InteractableEmojis { get; private set; }
	public List<Emoji> AllHoveredEmojis { get; private set; }
	public Emoji HoveredEmoji { get; private set; }

	public Color BgColorBottom { get; set; }
	public Color BgColorTop { get; set; }

	public List<RingData> Rings { get; private set; }
	public List<LineData> Lines { get; private set; }

	public CursorEmoji CursorEmoji { get; private set; }
	public CrosshairEmoji CrosshairEmoji { get; private set; }

	public float CurrentTime { get; private set; }
	public float TimeScale { get; set; }

	public Stage()
	{
		Emojis = new();
		InteractableEmojis = new();
		AllHoveredEmojis = new();
		Lines = new();
		Rings = new();
	}

	public void Restart()
	{
		Emojis.Clear();
		InteractableEmojis.Clear();
		AllHoveredEmojis.Clear();
		Lines.Clear();
		Rings.Clear();

		CurrentTime = 0f;
		TimeScale = 1f;

		//CursorEmoji = AddEmoji(new CursorEmoji(), new Vector2(-999f, -999f)) as CursorEmoji;
		CrosshairEmoji = AddEmoji(new CrosshairEmoji(), Hud.Instance.MousePos) as CrosshairEmoji;

		FaceEmoji face1 = AddEmoji(new FaceEmoji(), new Vector2(200f, 700f)) as FaceEmoji;
		face1.Scale = 0.25f;

		FaceEmoji face1b = AddEmoji(new FaceEmoji(), new Vector2(200f, 300f)) as FaceEmoji;
		face1b.Scale = 0.1f;

		FaceEmoji face1c = AddEmoji(new FaceEmoji(), new Vector2(400f, 300f)) as FaceEmoji;
		face1c.Scale = 0.33f;

		FaceEmoji face2 = AddEmoji(new FaceEmoji(), new Vector2(400f, 700f)) as FaceEmoji;
		face2.Scale = 0.5f;

		FaceEmoji face3 = AddEmoji(new FaceEmoji(), new Vector2(700f, 700f)) as FaceEmoji;
		face3.Scale = 1f;

		FaceEmoji face4 = AddEmoji(new FaceEmoji(), new Vector2(1100f, 700f)) as FaceEmoji;
		face4.Scale = 1.333333f;

		FaceEmoji face5 = AddEmoji(new FaceEmoji(), new Vector2(1600f, 700f)) as FaceEmoji;
		face5.Scale = 2.5f;


		for(int i = 0; i < 1; i++)
		{
			//FaceEmoji faceEmoji = AddEmoji(new FaceEmoji(), new Vector2(Hud.Instance.ScreenWidth / 2f, Hud.Instance.ScreenHeight / 2f)) as FaceEmoji;
			//FaceEmoji faceEmoji = AddEmoji(new FaceEmoji(), new Vector2(Game.Random.Float(0f, Hud.Instance.ScreenWidth), Game.Random.Float(0f, Hud.Instance.ScreenHeight))) as FaceEmoji;

			//if(Game.Random.Float(0f, 1f) < 0.7f)
			//{
			//	var knife = AddEmoji(new KnifeEmoji());
			//	faceEmoji.AddChild(knife);
			//	faceEmoji.HeldItem = knife;
			//}
		}

		//AddEmoji(new KnifeEmoji(), new Vector2(800f, 800f));
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

		TimeScale = Utils.DynamicEaseTo(TimeScale, 1f, Utils.Map(TimeScale, 0f, 1f, 0.01f, 0.1f), dtRaw);

		BgColorBottom = new Color(0.05f + Utils.FastSin(CurrentTime * 0.15f) * 0.05f, 0.05f + Utils.FastSin(20f + CurrentTime * 0.13f) * 0.05f, 0.05f + Utils.FastSin(10f + CurrentTime * 0.17f) * 0.05f);
		BgColorTop = new Color(0.35f + Utils.FastSin(30f + CurrentTime * 0.12f) * 0.15f, 0.35f + Utils.FastSin(CurrentTime * 0.11f) * 0.1f, 0.35f + Utils.FastSin(5f + CurrentTime * 0.09f) * 0.2f);
	}

	void HandleEmoji(float dt)
	{
		// ALL EMOJI (need to update child emoji after their parents, so the zindex/position/etc can be set correctly)
		for(int i = Emojis.Count - 1; i >= 0; i--)
		{
			var emoji = Emojis[i];

			if(emoji.HasParent)
				continue;

			emoji.Update(dt);

			if(emoji.HasChildren)
				UpdateEmojisChildren(emoji, dt);
		}

		// INTERACTABLE EMOJI ONLY
		var mousePos = Hud.Instance.MousePos;
		AllHoveredEmojis.Clear();

		float RADIUS_REPEL_SCALE = 0.8f;
		float REPEL_STRENGTH = 220f;

		for(int i = InteractableEmojis.Count - 1; i >= 0; i--)
		{
			var emoji = InteractableEmojis[i];

			// mouse hover
			emoji.IsHovered = false;
			var radius = emoji.Radius * RADIUS_REPEL_SCALE;

			if(emoji.Radius > 0f)
			{
				var distSqr = (mousePos - emoji.GetRotatedPos()).LengthSquared;
				if(distSqr < MathF.Pow(emoji.Radius, 2f))
					AllHoveredEmojis.Add(emoji);
			}

			// repel
			if(emoji.Parent != null)
				continue;

			for(int j = InteractableEmojis.Count - 1; j >= 0; j--)
			{
				var other = InteractableEmojis[j];

				if(other == emoji || other.Parent != null)
					continue;

				var pos = emoji.GetRotatedPos();
				var otherPos = other.GetRotatedPos();
				float otherRadius = other.Radius * RADIUS_REPEL_SCALE;

				float distSqr = (pos - otherPos).LengthSquared;
				float reqDistSqr = MathF.Pow(radius + otherRadius, 2f);
				if(distSqr < reqDistSqr)
				{
					float percent = Utils.Map(distSqr, reqDistSqr, 0f, 0f, 1f);
					float repelStrength = REPEL_STRENGTH * other.Weight;
					emoji.Velocity += (pos == otherPos)
						? Utils.GetRandomVector() * repelStrength * dt
						: (pos - otherPos).Normal * percent * repelStrength * dt;
				}
			}
		}

		HoveredEmoji = AllHoveredEmojis.Count > 0 ? AllHoveredEmojis.OrderByDescending(x => x.ZIndex).First() : null;
		if(HoveredEmoji != null)
			HoveredEmoji.IsHovered = true;
	}

	void UpdateEmojisChildren(Emoji emoji, float dt)
	{
		if(emoji.Children == null)
			return;

		for(int i = emoji.Children.Count - 1; i >= 0; i--)
		{
			var child = emoji.Children[i];
			child.Update(dt);

			if(child.HasChildren)
				UpdateEmojisChildren(child, dt);
		}
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

		if(emoji.IsInteractable)
			InteractableEmojis.Add(emoji);

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

		if(emoji.IsInteractable)
			InteractableEmojis.Remove(emoji);

		//if(HoveredEmoji == emoji)
		//	HoveredEmoji = null;

		//Log.Info($"RemoveEmoji: {emoji}, emoji.HasParent: {emoji.HasParent}");

		if(emoji.HasParent)
			emoji.Parent.RemoveChild(emoji);
	}

	public void DrawLine(Vector2 posA, Vector2 posB, float thickness, Color color, float lifetime = 0f, int zIndex = 99999, float invert = 0f, float saturation = 1f, float blur = 0f)
	{
		Lines.Add(new LineData(posA, posB, thickness, color, CurrentTime, lifetime, zIndex, invert, saturation, blur));
	}

	public void DrawLineTo(Vector2 posB, float thickness, Color color, float lifetime = 0f, int zIndex = 99999, float invert = 0f, float saturation = 1f, float blur = 0f)
	{
		Lines.Add(new LineData(new Vector2(5f, 5f), posB, thickness, color, CurrentTime, lifetime, zIndex, invert, saturation, blur));
	}

	public void AddRing(Vector2 pos, Color color, float lifetime, float startRadius, float endRadius, float startWidth, float endWidth, int numSegments, int zIndex = 0)
	{
		Rings.Add(new RingData(pos, color, CurrentTime, lifetime, zIndex, startRadius, endRadius, startWidth, endWidth, numSegments));
	}
}
