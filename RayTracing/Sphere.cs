using System;

namespace RayTracing
{
    internal class Sphere : Shape
    {
        // число, близкое к нулю для сравнения вещественных чисел
        private const double Zero = 1e-4;

        private readonly double _radius;
        private readonly Vector _center;

        /// <summary>
        ///  Инициализация сферы
        /// </summary>
        /// <param name="x"> x-координата центра сферы </param>
        /// <param name="y"> y-координата центра сферы </param>
        /// <param name="z"> z-координата центра сферы </param>
        /// <param name="radius"> радиус сферы </param>
        /// <param name="material"> материал сферы </param>
        public Sphere(double x, double y, double z, double radius, Material material)
        {
            _center = new Vector(x, y, z);
            _radius = radius;
            Material = material;
        }

        /// <summary>
        /// Проверка на пересечение с лучом
        /// </summary>
        /// <param name="origin"> откуда идет луч </param>
        /// <param name="direction"> с чем пересекается луч </param>
        /// <param name="t"> корень квадратного уравнения </param>
        /// <returns> true, если существует пересечение вектора с лучом </returns>
        public override bool DoesRayIntersect(Vector origin, Vector direction, out double t)
        {
            // определяет предполагаемое положение пересечения (начало луча - центр сферы)
            var intersectionPoint = origin - _center;

            // вычисляет дискриминант
            var b = intersectionPoint * direction;
            var c = intersectionPoint * intersectionPoint - _radius * _radius;
            var discriminant = b * b - c;

            // если дискриминант меньше нуля, то пересечения с лучом нет и  t - бесконечность 
            if (discriminant < Zero)
            {
                t = double.PositiveInfinity;
                return false;
            }

            // вычисляет корень уравнения            
            t = -b - Math.Sqrt(discriminant);

            // если корень меньше нуля, то пересечения с лучом нет 
            if (t < Zero)
                t = -b + Math.Sqrt(discriminant);

            return t> Zero;
        }

        /// <summary>
        /// Получение нормали в точке (нормаль к сфере - вектор между точкой и центром сферы)
        /// </summary>
        /// <param name="point"> точка, в которой требуется получить нормаль </param>
        /// <returns> нормаль </returns>
        public override Vector Normal(Vector point)
        {
            return (point - _center).Normalize();
        }
    }
}