using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace EmojiEngine;

public class KnifeEmoji : Emoji
{
	public const float RADIUS_SIZE_FACTOR = 0.56f;

	public ShadowEmoji ShadowEmoji { get; set; }

	public KnifeEmoji()
	{
		IsInteractable = true;
	}

	public override void Init()
	{
		base.Init();

		Text = "🔪";
		SetFontSize(120f);
		Radius = FontSize * RADIUS_SIZE_FACTOR;

		ShadowEmoji = Stage.AddEmoji(new ShadowEmoji(), new Vector2(-999f, -999f)) as ShadowEmoji;
		ShadowEmoji.SetFontSize(FontSize * 0.8f);

		Weight = 2f;
	}

	public override void Update(float dt)
	{
		base.Update(dt);

		ShadowEmoji.Text = Text;
		ShadowEmoji.ScaleX = ScaleX * 1.25f;
		ShadowEmoji.ScaleY = ScaleY * 0.8f;
		ShadowEmoji.Degrees = Degrees * 0.2f;
		ShadowEmoji.Position = Position + new Vector2(Degrees * 0.4f, -25f);
		ShadowEmoji.Blur = Blur * 0.1f + Utils.DynamicEaseTo(ShadowEmoji.Blur, 6f, 0.2f, dt);
		ShadowEmoji.Scale = Utils.DynamicEaseTo(ShadowEmoji.Scale, Scale, 0.2f, dt);

		if(Parent == null)
		{
			Position += Velocity * dt;
			Velocity *= (1f - 3f * dt);

			CheckBounds();

			ZIndex = Hud.Instance.GetZIndex(Position.y);
		}
	}

	public override void Hit(Vector2 hitPos)
	{
		base.Hit(hitPos);

		Velocity += new Vector2(0f, 1f) * Game.Random.Float(50f, 140f);
	}
}
