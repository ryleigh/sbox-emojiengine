using Sandbox;
using Sandbox.Internal;
using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;

namespace EmojiEngine;

public partial class Hud : RootPanel, Sandbox.Menu.IGameMenuPanel
{
	public static Hud Instance { get; private set; }
	public Stage CurrentStage { get; private set; }
	public Dictionary<string, Stage> Stages = new();

	public DebugDisplay DebugDisplay { get; private set; }

	public EEGame EEGame { get; set; }
	public EmojiDisplay EmojiDisplay { get; private set; }
	public OverlayDisplay OverlayDisplay { get; private set; }
	public Vector2 CameraOffset { get; set; }
	public float CameraScale { get; set; }

	public float ScreenWidth => Screen.Width * ScaleFromScreen;
	public float ScreenHeight => Screen.Height * ScaleFromScreen;
	public Vector2 CenterPos => new Vector2(ScreenWidth / 2f, ScreenHeight / 2f);
	public Vector2 MousePos => new Vector2(MousePosition.x, Screen.Height - MousePosition.y) * ScaleFromScreen;
	public Vector2 LastMousePos { get; private set; }
	public Vector2 MouseDelta => MousePos - LastMousePos;

	public bool MouseDownLeft { get; private set; }
	public bool MouseDownRight { get; private set; }

	public const float BOUNDS_BUFFER = 40f;

	public Hud()
	{
		Instance = this;
		StyleSheet.Load("/ui/Hud.scss");

		EmojiDisplay = AddChild<EmojiDisplay>();
		DebugDisplay = AddChild<DebugDisplay>();
		OverlayDisplay = AddChild<OverlayDisplay>();

		Stage stage = new Stage();
		Stages.Add("stage0", stage);
		SetStage(stage);

		Restart();
	}

	public void SetStage(Stage stage)
	{
		CurrentStage = stage;
	}

	public void Restart()
	{
		CameraOffset = Vector2.Zero;
		CameraScale = 1f;

		DebugDisplay.Text = "";

		CurrentStage.Restart();
	}

	public override void Tick()
	{
		base.Tick();

		CurrentStage.Update(Time.Delta);

		//DebugDisplay.Text = $"{_faceEmojis.Count}";

		//Log.Info($"{Input.Pressed("Restart")}, {Input.Pressed("attack1")}");

		//Log.Info($"{ScreenWidth} / {ScreenHeight}");
		//Log.Info($"{MathF.Round(MousePos.x)}, {MathF.Round(MousePos.y)}");

		//if((MousePos - LastMousePos).LengthSquared > 0f && (MousePos - LastMousePos).LengthSquared < MathF.Pow(300f, 2f))
		//	DrawLine(LastMousePos, MousePos, 1f, new Color(0.75f, 0.75f, 1f, 0.2f), 0.1f);

		//DrawLine(new Vector2(20f, 20f), new Vector2(200f, 200f), 3f, Color.Red, 0.5f);

		LastMousePos = MousePos;

		//Style.BackgroundColor = BgColor;
	}

	

	protected override void OnMouseDown(MousePanelEvent e)
	{
		base.OnClick(e);
		bool rightClick = e.Button == "mouseright";

		if(rightClick)
			MouseDownRight = true;
		else
			MouseDownLeft = true;

		CurrentStage.OnMouseDown(rightClick);
	}

	protected override void OnMouseUp(MousePanelEvent e)
	{
		base.OnMouseUp(e);
		bool rightClick = e.Button == "mouseright";

		if(rightClick)
			MouseDownRight = false;
		else
			MouseDownLeft = false;

		CurrentStage.OnMouseUp(rightClick);
	}
}
