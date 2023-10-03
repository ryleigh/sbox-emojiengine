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

	public LineData(Vector2 _posA, Vector2 _posB, float _thickness, Color _color, float _spawnTime, float _lifetime, int _zIndex)
	{
		posA = _posA;
		posB = _posB;
		thickness = _thickness;
		color = _color;
		spawnTime = _spawnTime;
		lifetime = _lifetime;
		zIndex = _zIndex;
	}
}
