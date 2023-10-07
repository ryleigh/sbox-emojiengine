using Sandbox;
using Sandbox.UI.Construct;
using System;
using System.IO;
using System.Linq;

namespace EmojiEngine;

public struct LineData
{
	public Vector2 posA;
	public Vector2 posB;
	public float thickness;
	public Color color;
	public float spawnTime;
	public float lifetime;
	public int zIndex;
	public float invert;
	public float saturation;
	public float blur;

	public LineData(Vector2 _posA, Vector2 _posB, float _thickness, Color _color, float _spawnTime, float _lifetime, int _zIndex, float _invert, float _saturation, float _blur)
	{
		posA = _posA;
		posB = _posB;
		thickness = _thickness;
		color = _color;
		spawnTime = _spawnTime;
		lifetime = _lifetime;
		zIndex = _zIndex;
		invert = _invert;
		saturation = _saturation;
		blur = _blur;
	}
}
