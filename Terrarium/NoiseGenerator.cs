
using Microsoft.Xna.Framework;
using System;

namespace Terrarium
{
	public static class NoiseGenerator
	{
		static OpenSimplexNoise _simplexNoise = new OpenSimplexNoise();

		public static float[] GenerateNoiseMap(int length, float scale, int octaves, float persistance, float lacunarity)
		{
			float[] noiseMap = new float[length];

			float minNoiseValue = float.MaxValue;
			float maxNoiseValue = float.MinValue;

			for (int x = 0; x < length; x++)
			{
				float amplitude = 1f;
				float frequency = 1f;
				float noiseValue = 0f;

				for (int i = 0; i < octaves; i++)
				{
					float sampleX = x / scale * frequency;

					noiseValue += (float)_simplexNoise.Evaluate(sampleX, 0f) * amplitude;

					amplitude *= persistance;
					frequency *= lacunarity;
				}

				if (noiseValue > maxNoiseValue)
					maxNoiseValue = noiseValue;
				else if (noiseValue < minNoiseValue)
					minNoiseValue = noiseValue;

				noiseMap[x] = noiseValue;
			}

			// Normalize noise map values
			for (int x = 0; x < length; x++)
				noiseMap[x] = Util.InverseLerp(minNoiseValue, maxNoiseValue, noiseMap[x]);

			return noiseMap;
		}
	}
}
