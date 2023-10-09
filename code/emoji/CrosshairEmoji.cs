using Sandbox;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
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

	private float _gap;

	public CrosshairEmoji()
	{
		Text = "";
		ZIndex = 9999;
		IsInteractable = false;
		IsVisible = false;
		_timeSinceShoot = 99f;
		_length = 50f;
	}

	public override void Update(float dt)
	{
		base.Update(dt);

		_length = Utils.DynamicEaseTo(_length, Utils.Map(_recoilAmount, 0f, 300f, 50f, 160f, EasingType.Linear), 0.5f, dt);
		_width = Utils.DynamicEaseTo(_width, Utils.Map(_timeSinceShoot, 0f, 0.3f, 8f, 15f), 0.6f, dt);
		var centerPos = Hud.Instance.MousePos + new Vector2(0f, _width * 0.5f);

		_mouseDeltaGap = Utils.DynamicEaseTo(_mouseDeltaGap, Utils.Map(Hud.Instance.MouseDelta.Length, 0f, 20f, 0f, 100f, EasingType.SineIn), 0.28f, dt);
		_recoilGap = Utils.DynamicEaseTo(_recoilGap, Utils.Map(_timeSinceShoot, 0f, 0.5f, _recoilAmount, 0f, EasingType.SineOut), 0.3f, dt);
		_recoilAmount = Utils.DynamicEaseTo(_recoilAmount, 0f, 0.05f, dt);

		_gap = Utils.DynamicEaseTo(_gap, MIN_GAP + _mouseDeltaGap + _recoilGap, 0.25f, dt);
		Color color = Color.Lerp(Color.White, Color.Black, 0.5f + Utils.FastSin(Time.Now * 24f) * 0.5f).WithAlpha(0.5f);
		float invert = 1f;
		float saturation = Utils.Map(_timeSinceShoot, 0f, 1f, 10f, 1f);
		float blur = Utils.Map(_timeSinceShoot, 0f, 0.5f, 3f, 1f);

		Hud.Instance.DrawLine(centerPos - new Vector2(_gap, 0f), centerPos - new Vector2(_gap + _length, 0f), _width, color, 0f, ZIndex, invert, saturation, blur);
		Hud.Instance.DrawLine(centerPos + new Vector2(_gap, 0f), centerPos + new Vector2(_gap + _length, 0f), _width, color, 0f, ZIndex, invert, saturation, blur);
		Hud.Instance.DrawLine(centerPos - new Vector2(0f, _gap), centerPos - new Vector2(0f, _gap + _length), _width, color, 0f, ZIndex, invert, saturation, blur);
		Hud.Instance.DrawLine(centerPos + new Vector2(0f, _gap), centerPos + new Vector2(0f, _gap + _length), _width, color, 0f, ZIndex, invert, saturation, blur);

		if(Hud.Instance.MouseDownLeft && _timeSinceShoot > 0.2f)
			Shoot();

		Hud.Instance.OverlayDisplay.Brightness = Utils.Map(_timeSinceShoot, 0f, 0.125f, 3f, 1f, EasingType.QuadOut);
		//Hud.Instance.EmojiDisplay.Brightness = Utils.Map(_timeSinceShoot, 0f, 0.15f, 10f, 1f, EasingType.QuadOut);

		Hud.Instance.CameraOffset = Utils.GetRandomVector() * Utils.Map(_timeSinceShoot, 0f, 0.1f, 15f, 0f, EasingType.QuadOut);
	}

	public override void OnMouseDown(bool rightClick)
	{
		base.OnMouseDown(rightClick);

		if(!rightClick)
			Shoot();
	}

	void Shoot()
	{
		float gap = _gap - 6f;
		var mousePos = Hud.Instance.MousePos;
		var pos = mousePos + Game.Random.Float(0f, gap) * Utils.GetRandomVector();

		if(Hud.Instance.Raycast(pos, out List<Emoji> hitEmojis))
		{
			var emoji = hitEmojis[0];
			if(emoji is FaceEmoji faceEmoji)
			{
				float HOLE_SIZE = 20f * faceEmoji.Scale;
				if((pos - faceEmoji.Position).LengthSquared > MathF.Pow(faceEmoji.Radius - HOLE_SIZE, 2f))
					pos = faceEmoji.Position + (pos - faceEmoji.Position).Normal * (faceEmoji.Radius - HOLE_SIZE);

				faceEmoji.Hit(pos);

				WoundEmoji wound = Hud.Instance.AddEmoji(new WoundEmoji(), pos) as WoundEmoji;
				wound.ZIndex = faceEmoji.ZIndex + 1;
				wound.Parent = faceEmoji;
				Vector2 faceAnchorPos = faceEmoji.AnchorPos;

				wound.ParentOffsetDistance = (pos - faceAnchorPos).Length / faceEmoji.Scale;
				wound.ParentOffsetDegrees = Utils.VectorToDegrees(pos - faceAnchorPos);
				wound.ParentStartDegrees = faceEmoji.Degrees;
			}
		}
		else
		{
			Hud.Instance.AddEmoji(new BulletHoleEmoji(), pos);
			DustEmoji dust = Hud.Instance.AddEmoji(new DustEmoji(), pos) as DustEmoji;
			dust.ZIndex = (int)(Hud.Instance.ScreenHeight - pos.y);

			float offsetX = pos.x - mousePos.x;
			dust.Degrees = Utils.Map(offsetX, -200f, 200f, -120f, -60f);
			dust.Velocity = Utils.DegreesToVector(-dust.Degrees) * Game.Random.Float(1500f, 3000f);
		}

		_timeSinceShoot = 0f;
		_recoilAmount += 160f;
	}
}
