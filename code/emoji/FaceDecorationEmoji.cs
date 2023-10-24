using Sandbox;
using System;

namespace EmojiEngine;

public class FaceDecorationEmoji : Emoji
{
	public float ParentOffsetDistance { get; set; }
	public float ParentOffsetDegrees { get; set; }
	public float ParentStartDegrees { get; set; }

	public float StartDegrees { get; set; }


	public ImpactEmoji ImpactEmoji { get; set; }

	public override void Init()
	{
		base.Init();
		
	}

	public override void Update(float dt)
	{
		base.Update(dt);

		if(Parent == null)
		{
			Stage.RemoveEmoji(this);
			return;
		}

		ZIndex = Parent.ZIndex + Globals.DEPTH_INCREASE_DECORATION;
		float parentDegreesDiff = (Parent.Degrees - ParentStartDegrees);
		Position = Parent.AnchorPos + Utils.DegreesToVector(ParentOffsetDegrees - parentDegreesDiff) * ParentOffsetDistance * Parent.Scale;
		Degrees = StartDegrees + parentDegreesDiff;
		Scale = Parent.Scale;
	}
}
