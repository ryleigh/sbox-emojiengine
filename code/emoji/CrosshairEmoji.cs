using Sandbox;
using Sandbox.UI.Construct;
using System;
using System.Drawing;
using System.IO;
using System.Linq;

namespace EmojiEngine;

public partial class CrosshairEmoji : Emoji
{
	private float _length;
	private float _width;

	private const float MIN_GAP = 30f;
	private TimeSince _timeSinceShoot;
	private float _recoilAmount;

	private float _mouseDeltaGap;
	private float _recoilGap;

	public CrosshairEmoji()
	{
		Text = "";
		FontSize = 64f;
		ZIndex = 9999;
		IsInteractable = false;
		IsVisible = false;
		_timeSinceShoot = 99f;
		_length = 50f;
	}

	public override void Update(float dt)
	{
		base.Update(dt);

		float dX = Hud.Instance.MouseDelta.x;
		float dY = Hud.Instance.MouseDelta.y;

		var mousePos = Hud.Instance.MousePos;
		_length = Utils.DynamicEaseTo(_length, Utils.Map(_recoilAmount, 0f, 300f, 50f, 100f, EasingType.Linear), 0.5f, dt);
		_width = Utils.DynamicEaseTo(_width, Utils.Map(_timeSinceShoot, 0f, 0.3f, 20f, 14f), 0.6f, dt);

		_mouseDeltaGap = Utils.DynamicEaseTo(_mouseDeltaGap, Utils.Map(Hud.Instance.MouseDelta.Length, 0f, 20f, 0f, 60f, EasingType.QuadIn), 0.25f, dt);
		_recoilGap = Utils.DynamicEaseTo(_recoilGap, Utils.Map(_timeSinceShoot, 0f, 0.5f, _recoilAmount, 0f, EasingType.SineOut), 0.3f, dt);
		_recoilAmount = Utils.DynamicEaseTo(_recoilAmount, 0f, 0.05f, dt);

		float gap = MIN_GAP + _mouseDeltaGap + _recoilGap;
		Color color = Color.Lerp(Color.White, Color.Black, 0.5f + Utils.FastSin(Time.Now * 24f) * 0.5f).WithAlpha(0.5f);
		float invert = 1f;
		float saturation = Utils.Map(_timeSinceShoot, 0f, 1f, 10f, 1f);
		float blur = Utils.Map(_timeSinceShoot, 0f, 0.5f, 3f, 1f);

		Hud.Instance.DrawLine(mousePos - new Vector2(gap, 0f), mousePos - new Vector2(gap + _length, 0f), _width, color, 0f, ZIndex, invert, saturation, blur);
		Hud.Instance.DrawLine(mousePos + new Vector2(gap, 0f), mousePos + new Vector2(gap + _length, 0f), _width, color, 0f, ZIndex, invert, saturation, blur);
		Hud.Instance.DrawLine(mousePos - new Vector2(0f, gap), mousePos - new Vector2(0f, gap + _length), _width, color, 0f, ZIndex, invert, saturation, blur);
		Hud.Instance.DrawLine(mousePos + new Vector2(0f, gap), mousePos + new Vector2(0f, gap + _length), _width, color, 0f, ZIndex, invert, saturation, blur);
	}

	public override void OnMouseDown(bool rightClick)
	{
		base.OnMouseDown(rightClick);

		_timeSinceShoot = 0f;
		_recoilAmount += 120f;
	}
}
