﻿using Sandbox;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime;

namespace EmojiEngine;

public class Emoji
{
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
	private float _radius;
	public float Radius { 
		get { return _radius * Scale; } 
		set { _radius = value; } 
	}
	public bool IsVisible { get; set; }
	public bool IsInteractable { get; set; }
	public bool SwallowClicks { get; set; }
	public bool IsHovered { get; set; }
	public float TransformOriginX { get; set; }
	public float TransformOriginY { get; set; }
	public float BaseTransformOriginX { get; set; }
	public float BaseTransformOriginY { get; set; }
	public float Scale { get; set; }
	public float ScaleX { get; set; }
	public float ScaleY { get; set; }
	public float Blur { get; set; }
	public float Contrast { get; set; }
	public float Brightness { get; set; }
	public float Invert { get; set; }
	public float Saturation { get; set; }
	public float Sepia { get; set; }
	public float HueRotateDegrees { get; set; }

	public List<Emoji> Children { get; private set; }

	public Emoji()
	{
		Text = "";
		FontSize = 32f;
		PanelSizeFactor = 1.5f;
		PanelSize = FontSize * PanelSizeFactor;
		Opacity = 1f;
		IsVisible = true;
		IsInteractable = true;
		SwallowClicks = true;
		TransformOriginX = BaseTransformOriginX = 0.5f;
		TransformOriginY = BaseTransformOriginY = 0.5f;
		Scale = 1f;
		ScaleX = 1f;
		ScaleY = 1f;
		Contrast = 1f;
		Brightness = 1f;
		Saturation = 1f;
	}

	public virtual void Update(float dt)
	{
		//if(Radius > 0f)
		//	Utils.DrawCircle(Position, Radius, 12, Time.Now, Color.White, width: 1f, lifetime: 0f, zIndex: ZIndex + 1);
	}

	public virtual void OnMouseDown(bool rightClick) { }
	public virtual void OnMouseUp(bool rightClick) { }
	public virtual void OnClickedDown(bool rightClick) { }
	public virtual void OnClickedUp(bool rightClick) { }

	public void AddChild(Emoji emoji)
	{
		if(Children == null)
			Children = new List<Emoji>();

		Children.Add(emoji);
	}

	public void SetFontSize(float fontSize)
	{
		FontSize = fontSize;
		PanelSize = FontSize * PanelSizeFactor;
	}
}
