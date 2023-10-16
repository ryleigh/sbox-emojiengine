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
		Radius = FontSize * 0.3f;

		ShadowEmoji = Stage.AddEmoji(new ShadowEmoji(), new Vector2(-999f, -999f)) as ShadowEmoji;
		ShadowEmoji.SetFontSize(FontSize * 0.8f);

		Weight = 2f;
	}

	public override void Update(float dt)
	{
		base.Update(dt);

		//Stage.DrawLineTo(ShadowEmoji.Position, 2f, Color.White);

		if(Parent == null)
		{
			Position += Velocity * dt;
			Velocity *= (1f - 3f * dt);

			Altitude += Gravity * dt;
			
			if(Altitude > 0f)
			{
				Gravity += Globals.GRAVITY_ACCEL * dt;

				Degrees += 1500f * dt;

				while(Degrees > 180f)
					Degrees -= 180f;

				while(Degrees < -180f)
					Degrees += 180f;
			}
			else
			{
				if(Altitude < 0f)
					Altitude = 0f;

				Degrees = Utils.DynamicEaseTo(Degrees, 0f, 0.3f, dt);
			}

			//DebugText = $"{(int)Degrees}";

			//Height = 32f + Utils.FastSin(TimeSinceSpawn) * 32f;

			CheckBounds();

			ZIndex = Hud.Instance.GetZIndex(Position.y);

			

			ShadowEmoji.Position = Position + new Vector2(0f, -25f);
		}
		else
		{
			ShadowEmoji.Position = Position + new Vector2(0f, -45f);
		}

		ShadowEmoji.Text = Text;
		ShadowEmoji.ScaleX = ScaleX * 1.25f * Utils.Map(Altitude, 0f, 600f, 1f, 1.2f);
		ShadowEmoji.ScaleY = ScaleY * 0.8f * Utils.Map(Altitude, 0f, 600f, 1f, 0.8f);
		ShadowEmoji.Degrees = Degrees;
		ShadowEmoji.Blur = Blur * 0.1f + Utils.DynamicEaseTo(ShadowEmoji.Blur, 6f, 0.2f, dt);
		ShadowEmoji.Scale = Utils.DynamicEaseTo(ShadowEmoji.Scale, Scale, 0.2f, dt);
	}

	public override void Hit(Vector2 hitPos)
	{
		base.Hit(hitPos);

		if(Parent != null && Parent is FaceEmoji face)
		{
			if(face.HeldItem != null)
			{
				face.RemoveChild(face.HeldItem);
				face.HeldItem = null;
			}
		}

		Velocity += new Vector2(0f, 1f) * Game.Random.Float(50f, 140f);

		Gravity = 600f;
	}
}
