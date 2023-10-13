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
	//public Emoji Parent { get; set; }

	public override void Init()
	{
		base.Init();

		Text = "⚫️";
		ZIndex = -9999;
		Contrast = 2f;
		Brightness = 0f;
	}

	public override void Update(float dt)
	{
		base.Update(dt);

	}
}
