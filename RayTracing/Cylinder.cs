using System;

namespace RayTracing
{
    internal class Cylinder : Shape
    {
        // число, близкое к нулю для сравнения вещественных чисел
        private const double Zero = 1e-4;

        private readonly double _radius;
        private readonly double _height;
        private readonly Vector _center;

        /// <summary>
        /// Инициализация цилиндра
        /// </summary>
        /// <param name="x"> x-координата центра цилиндра </param>
        /// <param name="y"> y-координата центра цилиндра </param>
        /// <param name="z"> z-координата центра цилиндра </param>
        /// <param name="radius"> радиус цилиндра </param>
        /// <param name="height"> высота цилиндра </param>
        /// <param name="material"> материал цилиндра </param>
        public Cylinder(double x, double y, double z, double radius, double height, Material material)
        {
            _center = new Vector(x, y, z);
            _radius = radius;
            _height = height / 2;
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
            // определяет предполагаемое положение пересечения (начало луча - центр цилиндра)
            var intersectionPoint = origin - _center;

            // определяет принадлежность пересечения основаниям
            var isBelongToCylinderBase = false;
            var ts1 = (_height - origin.Y + _center.Y) / direction.Y;
            var point = intersectionPoint + direction * ts1;

            if (point.X * point.X + point.Z * point.Z - _radius * _radius < Zero)
                isBelongToCylinderBase = true;

            var ts2 = (-_height - origin.Y + _center.Y) / direction.Y;
            point = intersectionPoint + direction * ts2;

            if (point.X * point.X + point.Z * point.Z - _radius * _radius < Zero)
                isBelongToCylinderBase = true;

            // определяет принадлежность пересечения цилиндру путем вычисления корня уравнения
            var a = direction.X * direction.X + direction.Z * direction.Z;
            var b =
                (origin.X * direction.X - direction.X * _center.X + origin.Z * direction.Z - direction.Z * _center.Z);
            var c = origin.X * origin.X + _center.X * _center.X + origin.Z * origin.Z + _center.Z * _center.Z -
                    2 * (origin.X * _center.X + origin.Z * _center.Z) - _radius * _radius;

            var discriminant = b * b - a * c;

            // если дискриминант меньше нуля, то пересечения с лучом нет и  t - бесконечность 
            if (discriminant < Zero)
            {
                if (isBelongToCylinderBase)
                {
                    t = Math.Min(ts1, ts2);
                    return true;
                }
                t = double.PositiveInfinity;
                return false;
            }

            // вычисляет корни уравнения      
            var t1 = (-b - Math.Sqrt(discriminant)) / a;
            var t2 = (-b + Math.Sqrt(discriminant)) / a;

            // если корень меньше нуля, то пересечения с лучом нет 
            t = t1;
            if (t1 < Zero)
                t = t2;

            if (!(Math.Abs(origin.Y + t * direction.Y - _center.Y) > _height)) return t > Zero;
            if (!isBelongToCylinderBase) return false;
            t = Math.Min(ts1, ts2);
            return true;
        }

        /// <summary>
        /// Получение нормали в точке
        /// </summary>
        /// <param name="point"> точка, в которой требуется получить нормаль </param>
        /// <returns> нормаль </returns>
        public override Vector Normal(Vector point)
        {
            var normalIntersectionPoint = point - _center;

            return Math.Abs(normalIntersectionPoint.Y) < _height
                ? (new Vector(normalIntersectionPoint.X, 0, normalIntersectionPoint.Z)).Normalize()
                : normalIntersectionPoint.Normalize();
        }
    }
}