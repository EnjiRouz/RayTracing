using System;

namespace RayTracing
{
    internal class GeometryPlane : Shape
    {
        private readonly Vector _normalVector;
        private readonly double _offset;
        protected readonly double Size;
        protected readonly Vector Center;

        /// <summary>
        /// Инициализация плоскости
        /// </summary>
        /// <param name="center"> координаты центра плоскости </param>
        /// <param name="x"> x-координата нормали </param>
        /// <param name="y"> y-координата нормали </param>
        /// <param name="z"> z-координата нормали </param>
        /// <param name="offset"> смещение плоскости </param>
        /// <param name="size"> размер плоскости </param>
        /// <param name="material"> материал плоскости </param>
        protected GeometryPlane(Vector center, double x, double y, double z, 
                                double offset, double size, Material material)
        {
            _normalVector = new Vector(x, y, z).Normalize();
            Center = center;
            _offset = offset / _normalVector.GetLength();
            Size = size;
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
            // находит косинус между нарпавлением луча и нормалью
            t = double.PositiveInfinity;
            var cos = direction * _normalVector; 

            // если косинус меньше нуля, то луч параллелен плоскости, т.е. пересечения нет
            if (Math.Abs(cos) < 0)
                return false;

            // находит расстояние от начала луча до плоскости
            t = (-_offset - origin * _normalVector) / (_normalVector * direction); 

            // находит точку пересечения луча с плоскостью
            var intersectionPoint = new Vector
            {
                X = origin.X + direction.X * t - Center.X,
                Y = origin.Y + direction.Y * t - Center.Y,
                Z = origin.Z + direction.Z * t - Center.Z
            };

            // если точка дальше размера плосокости - пересечения нет
            return !(Math.Abs(intersectionPoint.X) > Size) && !(Math.Abs(intersectionPoint.Y) > Size) &&
                   !(Math.Abs(intersectionPoint.Z) > Size);
        }

        /// <summary>
        /// Получение нормали в точке
        /// </summary>
        /// <param name="point"> точка, в которой требуется получить нормаль </param>
        /// <returns> нормаль </returns>
        public override Vector Normal(Vector point)
        {
            return _normalVector;
        }
    }    
}
