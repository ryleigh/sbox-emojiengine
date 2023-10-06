using Sandbox;
using Sandbox.UI.Construct;
using System;
using System.Drawing;
using System.IO;
using System.Linq;

namespace EmojiEngine;

public partial class CursorEmoji : Emoji
{
	public bool IsDragging { get; private set; }
	public FaceEmoji DraggedEmoji { get; private set; }
	public Vector2 CurrSpriteOffset { get; private set; }

	private bool _isScaling;
	private TimeSince _timeSinceScale;
	private float _scaleTime;
	private float _scaleAmount;

	private float _bounceScale;

	public bool IsHoveringSomething => Hud.Instance.AllHoveredEmojis.Count() > 0;

	public CursorEmoji()
	{
		Text = "☝️";
		FontSize = 64f;
		ZIndex = 9999;
		Interactable = false;
		_bounceScale = 1f;
	}

	public override void Update(float dt)
	{
		base.Update(dt);

		float dX = Hud.Instance.MouseDelta.x;
		float dY = Hud.Instance.MouseDelta.y;

		if(_isScaling)
		{
			if(_timeSinceScale > _scaleTime)
			{
				_isScaling = false;
				_bounceScale = 1f;
			}
			else
			{
				_bounceScale = Utils.Map(_timeSinceScale, 0f, _scaleTime, _scaleAmount, 1f, EasingType.QuadOut);
			}
		}

		Position = Hud.Instance.MousePos + new Vector2(-13f, -28f) + (IsDragging ? new Vector2(8f, 0f) : Vector2.Zero);
		SpriteOffset = CurrSpriteOffset + (Hud.Instance.MouseDownLeft ? new Vector2(0f, 4f) : new Vector2(0f, 0f));

		var hoveringScale = IsHoveringSomething ? 1.1f : 1f;
		Scale = Utils.DynamicEaseTo(Scale, _bounceScale * hoveringScale, 0.8f, dt);

		Degrees = Utils.DynamicEaseTo(Degrees, Math.Clamp(dX * 2.2f, -40f, 40f), 0.65f, dt);
		ScaleY = Utils.DynamicEaseTo(ScaleY, Utils.Map(MathF.Abs(dY), 0f, 10f, 1f, dY < 0f ? 0.7f : 1.3f), 0.3f, dt);
		ScaleX = 1f + (1f - ScaleY);

		Blur = Hud.Instance.MouseDelta.Length * 0.2f;

		if(IsDragging && DraggedEmoji != null)
		{
			DraggedEmoji.Position += Hud.Instance.MouseDelta;

			Vector2 holdPos = Hud.Instance.MousePos + new Vector2(0f, -DraggedEmoji.Radius * 0.7f);
			if((DraggedEmoji.Position - holdPos).LengthSquared > MathF.Pow(3f, 2f))
				DraggedEmoji.Position = Utils.DynamicEaseTo(DraggedEmoji.Position, holdPos, 0.15f, dt);

			DraggedEmoji.Degrees = Utils.DynamicEaseTo(DraggedEmoji.Degrees, dX * 1.1f, 0.2f, dt);

			DraggedEmoji.ScaleY = Utils.DynamicEaseTo(DraggedEmoji.ScaleY, Utils.Map(MathF.Abs(dY), 0f, 10f, 1f, dY < 0f ? 0.8f : 1.2f), 0.3f, dt);
			DraggedEmoji.ScaleX = 1f + (1f - DraggedEmoji.ScaleY);

			//DraggedEmoji.ScaleX = Utils.DynamicEaseTo(DraggedEmoji.ScaleX, Utils.Map(Hud.Instance.MouseDelta.Length, 0f, 10f, 1f, 0.8f), 0.3f, dt);
			//DraggedEmoji.ScaleY = Utils.DynamicEaseTo(DraggedEmoji.ScaleY, Utils.Map(Hud.Instance.MouseDelta.Length, 0f, 10f, 1f, 1.2f), 0.3f, dt);
		}

		if(!IsDragging)
		{
			Text = IsHoveringSomething ? "👆" : "☝️";
			CurrSpriteOffset = IsHoveringSomething ? new Vector2(22f, 0f) : new Vector2(0f, 0f);
		}
	}

	public void StartDragging(FaceEmoji faceEmoji)
	{
		Text = "🤏";
		CurrSpriteOffset = new Vector2(34f, 16f);
		IsDragging = true;
		DraggedEmoji = faceEmoji;
		DraggedEmoji.TransformOriginY = 0.1f;
		DraggedEmoji.IsBeingDragged = true;
		DraggedEmoji.Velocity = 0f;

		Hud.Instance.CursorEmoji.BounceScale(1.2f, 0.1f);
	}

	public void StopDragging()
	{
		Text = IsHoveringSomething ? "👆" : "☝️";
		CurrSpriteOffset = IsHoveringSomething ? new Vector2(22f, 0f) : new Vector2(0f, 0f);

		IsDragging = false;
		DraggedEmoji.TransformOriginY = DraggedEmoji.BaseTransformOriginY;
		DraggedEmoji.Position += new Vector2(-DraggedEmoji.Degrees, 0f);
		DraggedEmoji.IsBeingDragged = false;
		DraggedEmoji.Velocity = Hud.Instance.MouseDelta * 100f;

		DraggedEmoji = null;

		Hud.Instance.CursorEmoji.BounceScale(0.9f, 0.15f);
	}

	public void BounceScale(float scale, float time)
	{
		_scaleAmount = scale;
		_timeSinceScale = 0f;
		_isScaling = true;
		_scaleTime = time;
	}
}
