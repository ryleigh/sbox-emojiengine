using Sandbox;

namespace EmojiEngine;

public class WoundEmoji : Emoji
{
	public Emoji Parent { get; set; }
	public float ParentOffsetDistance { get; set; }
	public float ParentOffsetDegrees { get; set; }
	public float ParentStartDegrees { get; set; }

	private TimeSince _timeSinceSpawn;
	public float Lifetime { get; set; }

	private float _brightness;
	private float _brightnessTime;

	private float _startDegrees;

	private bool _shouldSpawnBlood;
	private float _countdownToDrip;

	public WoundEmoji()
	{
		IsInteractable = false;
		Saturation = Game.Random.Float(1f, 2f);

		ScaleX = Game.Random.Float(1f, 1.1f);
		ScaleY = Game.Random.Float(0.9f, 1f);
		_timeSinceSpawn = 0f;
		Lifetime = Game.Random.Float(6f, 6.5f);
		PanelSizeFactor = 2f;
		Degrees = _startDegrees = Game.Random.Float(0, 360f);

		float rand = Game.Random.Float(0f, 1f);
		if(rand < 0.5f)
		{
			float rand2 = Game.Random.Float(0f, 1f);
			if(rand2 < 0.7f)		Text = "🍁";
			else					Text = "🥮";

			SetFontSize(Game.Random.Float(20f, 22f));
			_brightness = Game.Random.Float(4f, 5f);
		}
		else
		{
			float rand2 = Game.Random.Float(0f, 1f);
			if(rand2 < 0.1f)		Text = "🍀";
			else if(rand2 < 0.2f)	Text = "🍅";
			else if(rand2 < 0.3f)	Text = "💱";
			else if(rand2 < 0.5f)	Text = "🎃";
			else if(rand2 < 0.6f)	Text = "🧼";
			else if(rand2 < 0.7f)	Text = "🧅";
			else if(rand2 < 0.8f)	Text = "🌰";
			else if(rand2 < 0.9f)	Text = "🧆";
			else                    Text = "🧩";

			SetFontSize(Game.Random.Float(16f, 18f));
			_brightness = 0f;
		}

		_brightnessTime = Game.Random.Float(0.2f, 0.35f);

		HasDropShadow = true;
		DropShadowX = Game.Random.Float(0f, 2f);
		DropShadowY = Game.Random.Float(0f, 2f);
		DropShadowBlur = 8f;
		DropShadowColor = Color.Red;

		_countdownToDrip = Game.Random.Float(0.3f, 1f);

		TextStrokeColor = Color.Red;

		_shouldSpawnBlood = true;
	}

	public override void Update(float dt)
	{
		base.Update(dt);

		if(Parent != null)
		{
			ZIndex = Parent.ZIndex + (int)Utils.Map(_timeSinceSpawn, 0f, Lifetime, 5f, 2f, EasingType.Linear);
			float parentDegreesDiff = (Parent.Degrees - ParentStartDegrees);
			Position = Parent.AnchorPos + Utils.DegreesToVector(ParentOffsetDegrees - parentDegreesDiff) * ParentOffsetDistance * Parent.Scale;
			Degrees = _startDegrees + parentDegreesDiff;
		}

		Opacity = Utils.Map(_timeSinceSpawn, 0f, Lifetime, 1f, 0f, EasingType.ExpoIn);
		Brightness = Utils.Map(_timeSinceSpawn, 0f, _brightnessTime, _brightness, 0f, EasingType.QuadOut);
		Blur = Utils.Map(_timeSinceSpawn, 0f, Lifetime * 0.25f, 7f, 5f, EasingType.QuadOut);
		Scale = (Parent?.Scale ?? 1f) * Utils.Map(_timeSinceSpawn, 0f, Lifetime * 0.15f, 1.25f, 1f, EasingType.QuadOut) * (Utils.Map(Position.y, 0f, Hud.Instance.ScreenHeight, 1.4f, 0.8f));

		TextStroke = Utils.Map(_timeSinceSpawn, 0f, Lifetime * 0.2f, 6f, 0f, EasingType.Linear);

		if(_shouldSpawnBlood)
		{
			SpawnBloodSpray();
			_shouldSpawnBlood = false;
		}

		if(_timeSinceSpawn < Lifetime * 0.6f)
		{
			_countdownToDrip -= dt;
			if(_countdownToDrip < 0f)
			{
				SpawnBloodDrip();
				_countdownToDrip = Game.Random.Float(0.5f, 2.5f);
			}
		}
		
		if(_timeSinceSpawn > Lifetime)
			Hud.Instance.RemoveEmoji(this);
	}

	void SpawnBloodSpray()
	{
		BloodSprayEmoji blood = Hud.Instance.AddEmoji(new BloodSprayEmoji(), Position) as BloodSprayEmoji;
		blood.ZIndex = ZIndex + 2;
		blood.Velocity = (Position - Parent.Position).Normal * Game.Random.Float(500f, 1200f);
		blood.Gravity = Game.Random.Float(900f, 1400f);

		blood.FlipX = true;
		var angle = Utils.VectorToDegrees(Position - Parent.Position);
		//Log.Info($"{angle}");

		if(angle > 0f && angle < 90f) // top right
		{
			blood.FlipY = true;
			blood.Degrees = angle + Utils.Map(angle, 0f, 90f, 180f, 0f);
			blood.TargetDegrees = blood.Degrees + Utils.Map(angle, 0f, 90f, 90f, 0f);
			blood.Velocity *= Utils.Map(angle, 0f, 90f, 1f, 1.5f);
		}
		else if(angle < 0f && angle > -90f) // bottom right
		{
			blood.FlipY = true;
			blood.Degrees = angle + Utils.Map(angle, 0f, -90f, -180f, 0f);
			blood.TargetDegrees = blood.Degrees + Utils.Map(angle, 0f, -90f, 90f, 0f);
		}
		else if(angle > 90f && angle < 180f) // top left
		{
			blood.Degrees = angle + 180f;
			blood.TargetDegrees = blood.Degrees + Utils.Map(angle, 90f, 180f, 0f, 90f);
			blood.Velocity *= Utils.Map(angle, 90f, 180f, 1.5f, 1f);
		}
		else // bottom left
		{
			blood.Degrees = angle + 180f;
			blood.TargetDegrees = blood.Degrees + Utils.Map(angle, -90f, -180f, 0f, 90f);
		}
	}

	void SpawnBloodDrip()
	{
		BloodDripEmoji blood = Hud.Instance.AddEmoji(new BloodDripEmoji(), Position + new Vector2(0f, -5f)) as BloodDripEmoji;
		blood.ZIndex = ZIndex + 1;
		blood.Gravity = Game.Random.Float(900f, 1400f);
		blood.WoundEmoji = this;
		blood.WoundPosLast = Position;
		blood.GroundYPos = Parent != null ? Parent.Position.y - Parent.Radius * 1.25f : -999f;
	}
}
