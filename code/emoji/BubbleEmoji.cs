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

	public float Lifetime { get; set; }

	public float ChangeTime { get; set; }
	public float TimeSinceChange => Stage.CurrentTime - ChangeTime;

	public bool LeftSide { get; set; }

	public override void Init()
	{
		base.Init();

		_emoji = new Emoji();
		_emoji.Text = "❓️";
		_emoji.Opacity = 0f;
		_emoji.SetFontSize(70f);
		Stage.AddEmoji(_emoji);

		Opacity = 0f;

		ChangeTime = Stage.CurrentTime;
	}

	public void SetBubbleMode(BubbleMode mode, bool left = false)
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
				FlipX = left;
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
				FlipX = left;
				break;
			case BubbleMode.Talk:
				Text = "🗨";
				SetFontSize(170f);
				_posOffset = new Vector2(140f, 140f);
				_emojiOffset = new Vector2(0f, 8f);
				_emojiOffset1 = new Vector2(-40f, 7f);
				_emojiOffset2 = new Vector2(40f, 7f);
				_emojiScale = 0.95f;
				_emojiScale2 = 0.9f;
				FlipX = !left;
				break;
		}

		LeftSide = left;
		ChangeTime = Stage.CurrentTime;
	}

	public void SetInnerEmoji(string text)
	{
		_emoji.Text = text;

		if(_emoji2 != null)
		{
			Stage.RemoveEmoji(_emoji2);
			_emoji2 = null;
		}

		_show2Emojis = false;

		ChangeTime = Stage.CurrentTime;
	}

	public void SetInnerEmoji(string text1, string text2)
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

		ChangeTime = Stage.CurrentTime;
	}

	public override void Update(float dt)
	{
		base.Update(dt);

		if(Parent == null || Parent is not FaceEmoji face || (face.IsDead && face.TimeSinceDeath > 1f && Opacity < 0.01f))
		{
			Stage.RemoveEmoji(this);
			return;
		}

		var parentRotatedPos = Parent.GetRotatedPos();

		float scaleMod = Utils.Map(TimeSinceChange, 0f, 0.2f, 1.3f, 1f, EasingType.QuadOut) * Utils.Map(parentRotatedPos.y, 0f, Hud.Instance.ScreenHeight, Globals.NEAR_SCALE, Globals.FAR_SCALE);
		Scale = scaleMod;
		float emojiScale = (_show2Emojis ? _emojiScale2 : _emojiScale);

		Vector2 posOffset = new Vector2(_posOffset.x * (LeftSide ? -1f : 1f), _posOffset.y);
		Position = Utils.DynamicEaseTo(Position, parentRotatedPos + posOffset * scaleMod, IsFirstUpdate ? 1f : 0.2f, dt);

		ZIndex = Parent.ZIndex + Globals.DEPTH_INCREASE_BUBBLE;

		float targetOpacity = ((face.IsDead && face.TimeSinceDeath > 1f) || (Lifetime > 0f && TimeSinceSpawn > Lifetime)) ? 0f : 1f;
		targetOpacity *= Utils.Map((Position - Hud.Instance.MousePos).LengthSquared, 0f, MathF.Pow(140f * scaleMod, 2f), 0.7f, 1f, EasingType.QuadOut);

		Opacity = Utils.DynamicEaseTo(Opacity, targetOpacity, 0.2f, dt);

		_emoji.Position = Position + (_show2Emojis ? _emojiOffset1 : _emojiOffset) * scaleMod;
		_emoji.ZIndex = ZIndex + 1;
		_emoji.Scale = Scale * emojiScale;
		_emoji.Opacity = Opacity;

		if(_show2Emojis)
		{
			_emoji2.Position = Position + _emojiOffset2 * scaleMod;
			_emoji2.ZIndex = ZIndex + 2;
			_emoji2.Scale = Scale * emojiScale;
			_emoji2.Opacity = Opacity;
		}
	}

	public override void OnRemove()
	{
		base.OnRemove();

		Stage.RemoveEmoji(_emoji);
	}
}
