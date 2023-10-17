using Sandbox;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Runtime.InteropServices;

namespace EmojiEngine;

public class Emoji
{
	public Stage Stage { get; set; }
	public Vector2 Position { get; set; }
	public Vector2 SpriteOffset { get; set; }
	public string Text { get; set; }
	public float FontSize { get; private set; } // warning - changing this is slow!
	public float PanelSize { get; set; }
	public float PanelSizeFactor { get; set; }
	public float Opacity { get; set; }
	public float Degrees { get; set; }
	public float StrokeWidth { get; set; }
	public Color StrokeColor { get; set; }
	public Vector2 ShadowOffset { get; set; }
	public float ShadowBlur { get; set; }
	public Color ShadowColor { get; set; }
	public int ZIndex { get; set; }
	public bool IsVisible { get; set; }
	public float TransformOriginX { get; set; }
	public float TransformOriginY { get; set; }
	public Vector2 AnchorPos => Position + new Vector2((TransformOriginX - 0.5f) * PanelSize, -(TransformOriginY - 0.5f) * PanelSize) * Scale;
	public Vector2 GetRotatedPos()
	{
		if(TransformOriginX == 0.5f && TransformOriginY == 0.5f)
			return Position;

		return AnchorPos + Utils.RotateVector(Position - AnchorPos, -Degrees);
	}
	public Vector2 GetRotatedPos(Vector2 pos)
	{
		if(TransformOriginX == 0.5f && TransformOriginY == 0.5f)
			return pos;

		return AnchorPos + Utils.RotateVector(pos - AnchorPos, -Degrees);
	}
	public float Scale { get; set; }
	public float ScaleX { get; set; }
	public float ScaleY { get; set; }
	public bool FlipX { get; set; }
	public float Blur { get; set; }
	public float Contrast { get; set; }
	public float Brightness { get; set; }
	public float Invert { get; set; }
	public float Saturation { get; set; }
	public float Sepia { get; set; }
	public float HueRotateDegrees { get; set; }
	public float TextStroke { get; set; }
	public Color TextStrokeColor { get; set; }
	public float TextShadowX { get; set; }
	public float TextShadowY { get; set; }
	public float TextShadowBlur { get; set; }
	public Color TextShadowColor { get; set; }
	public string BackgroundImage { get; set; }

	public float SpawnTime { get; set; }
	public float TimeSinceSpawn => Stage.CurrentTime - SpawnTime;
	public bool IsFirstUpdate { get; set; }
	private bool _firstUpdateFinished;
	public string DebugText { get; set; }

	public bool IsInteractable { get; set; }
	public bool SwallowClicks { get; set; }
	public bool IsHovered { get; set; }
	private float _radius;
	public float Radius
	{
		get { return _radius * Scale; }
		set { _radius = value; }
	}
	public float Weight { get; set; }
	public Vector2 Velocity { get; set; }

	public Emoji Parent { get; set; }
	public bool HasParent => Parent != null;
	public List<Emoji> Children { get; private set; }
	public bool HasChildren => Children != null && Children.Count > 0;

	public float Altitude { get; set; }
	public float Gravity { get; set; }

	internal Vector2 PanelPos => GetRotatedPos() - PanelSize * 0.5f + SpriteOffset * Scale - Hud.Instance.CameraOffset + new Vector2( 0f, Altitude );
	internal Vector2 PanelScale => new Vector2( ScaleX * (FlipX ? -1f : 1f), ScaleY ) * Scale;

	public Emoji()
	{
		Text = "";
		FontSize = 32f;
		PanelSizeFactor = 1.5f;
		PanelSize = FontSize * PanelSizeFactor;
		Opacity = 1f;
		IsVisible = true;
		SwallowClicks = true;
		TransformOriginX = 0.5f;
		TransformOriginY = 0.5f;
		Scale = 1f;
		ScaleX = 1f;
		ScaleY = 1f;
		Contrast = 1f;
		Brightness = 1f;
		Saturation = 1f;
		SpawnTime = Hud.Instance.CurrentStage.CurrentTime;
		IsFirstUpdate = true;
	}

	public virtual void Init()
	{
		
	}

	public virtual void Update(float dt)
	{
		//DrawDebug();

		if(IsFirstUpdate)
		{
			if(_firstUpdateFinished)
				IsFirstUpdate = false;
			else
				_firstUpdateFinished = true;
		}
	}

	public void DrawDebug()
	{
		if(IsInteractable && Radius > 0f)
		{
			Utils.DrawCircle(GetRotatedPos(), Radius, 10, Stage.CurrentTime * 4f, new Color(0.6f, 1f, 0.6f, 0.6f), width: 2f, lifetime: 0f, zIndex: ZIndex + 1);
			Utils.DrawCircle(Position, Radius, 10, Stage.CurrentTime, Color.White.WithAlpha(0.1f), width: 1f, lifetime: 0f, zIndex: ZIndex + 1);

			Stage.DrawLine(Position, AnchorPos, 2f, new Color(1f, 0.3f, 0.2f, 0.6f));

			float size = PanelSize;// * Scale;
			Stage.DrawLine(Position + new Vector2(-size * 0.5f, -size * 0.5f), Position + new Vector2(-size * 0.5f, -size * 0.5f) + new Vector2(size, 0f), 4f, new Color(0f, 0f, 0f, 0.5f));
			Stage.DrawLine(Position + new Vector2(-size * 0.5f, -size * 0.5f), Position + new Vector2(-size * 0.5f, -size * 0.5f) + new Vector2(0f, size), 4f, new Color(0f, 0f, 0f, 0.5f));
			Stage.DrawLine(Position + new Vector2(-size * 0.5f, size * 0.5f), Position + new Vector2(-size * 0.5f, size * 0.5f) + new Vector2(size, 0f), 4f, new Color(0f, 0f, 0f, 0.5f));
			Stage.DrawLine(Position + new Vector2(size * 0.5f, -size * 0.5f), Position + new Vector2(size * 0.5f, -size * 0.5f) + new Vector2(0f, size), 4f, new Color(0f, 0f, 0f, 0.5f));
		}
	}

	public virtual void OnMouseDown(bool rightClick) { }
	public virtual void OnMouseUp(bool rightClick) { }
	//public virtual void OnClickedDown(bool rightClick) { }
	//public virtual void OnClickedUp(bool rightClick) { }

	public void SetFontSize(float fontSize)
	{
		FontSize = fontSize;
		PanelSize = FontSize * PanelSizeFactor;
	}

	protected void CheckBounds()
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

	public virtual void Hit(Vector2 hitPos)
	{

	}

	public void AddChild(Emoji emoji)
	{
		if(Children == null)
			Children = new();

		Children.Add(emoji);
		emoji.Parent = this;
	}

	public void RemoveChild(Emoji emoji)
	{
		if(Children == null || !Children.Contains(emoji))
			return;

		Children.Remove(emoji);
		emoji.Parent = null;
	}
}
