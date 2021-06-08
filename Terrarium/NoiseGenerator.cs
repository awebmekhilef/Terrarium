
namespace Terrarium
{
	public static class NoiseGenerator
	{
		static OpenSimplexNoise _simplexNoise = new OpenSimplexNoise();

		public static float[,] GenerateNoiseMap(int width, int height, float scale, int octaves, float persistance, float lacunarity)
		{
			float[,] noiseMap = new float[width, height];

			float minNoiseValue = float.MaxValue;
			float maxNoiseValue = float.MinValue;

			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					float amplitude = 1f;
					float frequency = 1f;
					float noiseValue = 0f;

					for (int i = 0; i < octaves; i++)
					{
						float sampleX = x / scale * frequency;
						float sampleY = y / scale * frequency;

						noiseValue += (float)_simplexNoise.Evaluate(sampleX, sampleY) * amplitude;

						amplitude *= persistance;
						frequency *= lacunarity;
					}

					if (noiseValue > maxNoiseValue)
						maxNoiseValue = noiseValue;
					else if (noiseValue < minNoiseValue)
						minNoiseValue = noiseValue;

					noiseMap[x, y] = noiseValue;
				}

			}

			// Normalize noise map values
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					noiseMap[x, y] = Util.InverseLerp(minNoiseValue, maxNoiseValue, noiseMap[x, y]);
				}
			}

			return noiseMap;
		}
	}
}
