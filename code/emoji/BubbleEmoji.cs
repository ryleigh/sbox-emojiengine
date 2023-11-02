using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace EmojiEngine;

public enum BubbleMode { Thought, Yell, Talk };

public class BubbleEmoji : Emoji
{
	private Vector2 _posOffset;

	private Emoji _emoji;
	private Vector2 _emojiOffset;

	private Emoji _emoji2;
	private Vector2 _emojiOffset1;
	private Vector2 _emojiOffset2;

	private float _emojiScale;
	private float _emojiScale2;

	private bool _show2Emojis;

	private float _opacityMod;

	public override void Init()
	{
		base.Init();

		_emoji = new Emoji();
		_emoji.Text = "❓️";
		_emoji.Opacity = 0f;
		_emoji.SetFontSize(70f);
		Stage.AddEmoji(_emoji);

		Opacity = 0f;
		_opacityMod = 1f;
	}

	public void SetBubbleMode(BubbleMode mode)
	{
		switch (mode)
		{
			case BubbleMode.Thought:
				Text = "💭";
				SetFontSize(180f);
				_posOffset = new Vector2(150f, 150f);
				_emojiOffset = new Vector2(0f, -1f);
				_emojiOffset1 = new Vector2(-32f, -3f);
				_emojiOffset2 = new Vector2(28f, -3f);
				_emojiScale = 1f;
				_emojiScale2 = 0.8f;
				break;
			case BubbleMode.Yell:
				Text = "🗯️";
				SetFontSize(180f);
				_posOffset = new Vector2(150f, 150f);
				_emojiOffset = new Vector2(0f, -5f);
				_emojiOffset1 = new Vector2(-32f, -5f);
				_emojiOffset2 = new Vector2(28f, -5f);
				_emojiScale = 1f;
				_emojiScale2 = 0.8f;
				break;
			case BubbleMode.Talk:
				Text = "🗨";
				SetFontSize(170f);
				_posOffset = new Vector2(-140f, 140f);
				_emojiOffset = new Vector2(0f, 8f);
				_emojiOffset1 = new Vector2(-40f, 7f);
				_emojiOffset2 = new Vector2(40f, 7f);
				_emojiScale = 0.95f;
				_emojiScale2 = 0.9f;
				break;
		}
	}

	public void SetThoughtEmoji(string text)
	{
		_emoji.Text = text;

		if(_emoji2 != null)
		{
			Stage.RemoveEmoji(_emoji2);
			_emoji2 = null;
		}

		_show2Emojis = false;
	}

	public void SetThoughtEmoji(string text1, string text2)
	{
		_emoji.Text = text1;

		if(_emoji2 == null)
		{
			_emoji2 = new Emoji();
			_emoji2.Opacity = 0f;
			_emoji2.SetFontSize(70f);
			Stage.AddEmoji(_emoji2);
		}

		_emoji2.Text = text2;
		_show2Emojis = true;
	}

	public override void Update(float dt)
	{
		base.Update(dt);

		if(Parent == null || Parent is not FaceEmoji face || (face.IsDead && Opacity < 0.01f))
		{
			Stage.RemoveEmoji(this);
			return;
		}

		float scaleMod = Utils.Map(TimeSinceSpawn, 0f, 0.2f, 1.3f, 1f, EasingType.QuadOut) * Utils.Map(Parent.GetRotatedPos().y, 0f, Hud.Instance.ScreenHeight, Globals.NEAR_SCALE, Globals.FAR_SCALE);
		Scale = scaleMod;
		float emojiScale = (_show2Emojis ? _emojiScale2 : _emojiScale);

		Vector2 posOffset = new Vector2(_posOffset.x * (FlipX ? -1f : 1f), _posOffset.y);
		Position = Utils.DynamicEaseTo(Position, Parent.GetRotatedPos() + posOffset * scaleMod, IsFirstUpdate ? 1f : 0.2f, dt);

		float mouseDistSqr = (Position - Hud.Instance.MousePos).LengthSquared;
		_opacityMod = Utils.DynamicEaseTo(_opacityMod, Utils.Map(mouseDistSqr, 0f, MathF.Pow(140f * scaleMod, 2f), 0.75f, 1f, EasingType.QuadOut), 0.1f, dt);

		ZIndex = Parent.ZIndex + Globals.DEPTH_INCREASE_BUBBLE;
		Opacity = Utils.DynamicEaseTo(Opacity, face.IsDead ? 0f : 1f, 0.15f, dt) * _opacityMod;

		_emoji.Position = Position + (_show2Emojis ? _emojiOffset1 : _emojiOffset) * scaleMod;
		_emoji.ZIndex = ZIndex + 1;
		_emoji.Scale = Scale * emojiScale;
		_emoji.Opacity = Utils.DynamicEaseTo(_emoji.Opacity, face.IsDead ? 0f : 1f, 0.2f, dt) * _opacityMod;

		if(_show2Emojis)
		{
			_emoji2.Position = Position + _emojiOffset2 * scaleMod;
			_emoji2.ZIndex = ZIndex + 2;
			_emoji2.Scale = Scale * emojiScale;
			_emoji2.Opacity = Utils.DynamicEaseTo(_emoji2.Opacity, face.IsDead ? 0f : 1f, 0.2f, dt) * _opacityMod;
		}
	}

	public override void OnRemove()
	{
		base.OnRemove();

		Stage.RemoveEmoji(_emoji);
	}
}
