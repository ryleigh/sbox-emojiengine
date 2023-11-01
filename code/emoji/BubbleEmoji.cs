using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace EmojiEngine;

public class BubbleEmoji : Emoji
{
	private Emoji _emoji;

	public override void Init()
	{
		base.Init();

		Text = "💭";
		SetFontSize(180f);

		_emoji = new Emoji();
		_emoji.SetFontSize(80f);
		_emoji.Text = "📡";
		Stage.AddEmoji(_emoji);
	}

	public override void Update(float dt)
	{
		base.Update(dt);

		if(Parent == null || Parent is not FaceEmoji face || Opacity < 0.01f)
		{
			Stage.RemoveEmoji(this);
			return;
		}

		ZIndex = Parent.ZIndex + Globals.DEPTH_INCREASE_BUBBLE;
		Opacity = Utils.DynamicEaseTo(Opacity, face.IsDead ? 0f : 0.8f, 0.1f, dt);

		float scaleMod = Utils.Map(Parent.GetRotatedPos().y, 0f, Hud.Instance.ScreenHeight, Globals.NEAR_SCALE, Globals.FAR_SCALE);
		Scale = scaleMod;
		Position = Utils.DynamicEaseTo(Position, Parent.GetRotatedPos() + new Vector2(150f, 150f) * scaleMod, IsFirstUpdate ? 1f : 0.2f, dt);

		_emoji.Position = Position;
		_emoji.ZIndex = ZIndex + 1;
		_emoji.Scale = Scale;
		_emoji.Opacity = Utils.DynamicEaseTo(_emoji.Opacity, face.IsDead ? 0f : 1f, 0.15f, dt);
	}
}
