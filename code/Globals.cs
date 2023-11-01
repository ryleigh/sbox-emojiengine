using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Sandbox;
using System.Text.Json;

namespace EmojiEngine;

public static class Globals
{
	public const int DEPTH_SHADOW = -9999;
	public const int DEPTH_BULLET_HOLE_MIN = -4500;
	public const int DEPTH_BULLET_HOLE_MAX = -4000;
	public const int DEPTH_BLOOD_PUDDLE_MIN = -1000;

	public const int DEPTH_INCREASE_WOUND_IMPACT = 1;
	public const int DEPTH_INCREASE_BLOOD_DRIP = 1;
	public const int DEPTH_INCREASE_BLOOD_SPRAY = 2;

	public const int DEPTH_INCREASE_DECORATION = 1;
	public const int DEPTH_INCREASE_WOUND = 2;
	public const int DEPTH_INCREASE_HELD = 3;
	public const int DEPTH_INCREASE_BUBBLE = 4;

	public const int THROWN_AT_PLAYER_MIN = 20000;
	public const int THROWN_AT_PLAYER_MAX = 21000;
	public const int THROWN_AT_PLAYER_SHADOW = 19000;

	public const int DEPTH_PLAYER_HAND_LEFT = 99999;
	public const int DEPTH_PLAYER_GUN = 99999;
	public const int DEPTH_PLAYER_HAND_RIGHT = 99998;
	public const int DEPTH_PLAYER_MUZZLE_FLASH = 99997;
	public const int DEPTH_PLAYER_MUZZLE_SMOKE = 99996;
	public const int DEPTH_CROSSHAIR = 99995;
	public const int DEPTH_PLAYER_MUZZLE_LINE = 99989;

	public const int DEPTH_DEBUG = 999999999;



	public const float GRAVITY_ACCEL = -1500f;

	public const float NEAR_SCALE = 1.5f;
	public const float FAR_SCALE = 0.4f;
}
