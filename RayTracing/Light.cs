namespace RayTracing
{
    /// <summary>
    /// Тип источника света (направленный, точечный)
    /// </summary>
    internal enum LightType
    {
        Directional,
        Point
    }

    internal class Light
    {
        public readonly LightType Type;
        public readonly double Intensity;
        public readonly Vector Position;

        /// <summary>
        /// Инициализация источника света
        /// </summary>
        /// <param name="type"> тип источника света </param>
        /// <param name="intensity"> интенсивность света </param>
        /// <param name="position"> позиция (или направление у направленного источника света) источника света </param>
        public Light(LightType type, double intensity, Vector position)
        {
            Type = type;
            Intensity = intensity;

            // нормализует вектор для направленного источника света
            Position = type == LightType.Directional
                ? position.Normalize()
                : new Vector(position.X, position.Y, position.Z);
        }
    }
}