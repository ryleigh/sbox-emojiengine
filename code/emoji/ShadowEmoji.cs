using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace EmojiEngine;

public class ShadowEmoji : Emoji
{
	public ShadowEmoji()
	{
		Text = "⚫️";
		IsInteractable = false;
		ZIndex = -999;
		Contrast = 2f;
		Brightness = 0f;
		//ScaleX = 1.2f;
		//ScaleY = 0.6f;
		//Blur = 6f;
	}

	public override void Update(float dt)
	{
		base.Update(dt);

	}
}
