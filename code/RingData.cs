using Sandbox;
using Sandbox.UI.Construct;
using System;
using System.IO;
using System.Linq;

namespace EmojiEngine;

public struct RingData
{
	public Vector2 pos;
	public Color color;
	public float spawnTime;
	public float lifetime;
	public int zIndex;
	public float startRadius;
	public float endRadius;
	public float startWidth;
	public float endWidth;
	public int numSegments;

	public RingData(Vector2 _pos, Color _color, float _spawnTime, float _lifetime, int _zIndex, float _startRadius, float _endRadius, float _startWidth, float _endWidth, int _numSegments)
	{
		pos = _pos;
		color = _color;
		spawnTime = _spawnTime;
		lifetime = _lifetime;
		zIndex = _zIndex;
		startRadius = _startRadius;
		endRadius = _endRadius;
		startWidth = _startWidth;
		endWidth = _endWidth;
		numSegments = _numSegments;
	}
}
