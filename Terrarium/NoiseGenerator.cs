
namespace Terrarium
{
	public static class NoiseGenerator
	{
		static OpenSimplexNoise _simplexNoise = new OpenSimplexNoise();

		public static float[] GenerateNoiseMap(int length, float scale)
		{
			float[] noiseMap = new float[length];

			for (int i = 0; i < length; i++)
				noiseMap[i] = (float)_simplexNoise.Evaluate(i / scale, 0f);

			return noiseMap;
		}
	}
}
