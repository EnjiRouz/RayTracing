using System;

namespace RayTracing
{
    // шахматная доска
    internal class ChessGeometryPlane : GeometryPlane
    {
        private readonly int _cellCount;
        private readonly Vector _color1;
        private readonly Vector _color2;

        /// <summary>
        /// Инициализация шахматной доски
        /// </summary>
        /// <param name="center"> координаты центра плоскости </param>
        /// <param name="x"> x-координата нормали </param>
        /// <param name="y"> y-координата нормали </param>
        /// <param name="z"> z-координата нормали </param>
        /// <param name="offset"> смещение плоскости </param>
        /// <param name="size"> размер плоскости </param>
        /// <param name="cellCount"> количество клеток на плоскости </param>
        /// <param name="material"> материал плоскости </param>
        /// <param name="color1"> цвет квадратов №1 </param>
        /// <param name="color2"> цвет квадратов №2 </param>
        public ChessGeometryPlane(Vector center, double x, double y, double z,
                                  double offset, double size, int cellCount, Material material,
                                  Vector color1, Vector color2) : base(center, x, y, z, offset, size, material)
        {
            _cellCount = cellCount - 1;
            _color1 = color1;
            _color2 = color2;
        }

        /// <summary>
        /// Получение цвета пикселей квадратов доски
        /// </summary>
        /// <param name="point"> пиксель (точка), в котором вычисляется цвет </param>
        /// <returns></returns>
        public override Vector GetColor(Vector point)
        {
            var dx = point.X - Center.X + Size;
            var dy = point.Y - Center.Y + Size;
            var dz = point.Z - Center.Z + Size;

            var v1 = (int)(Math.Round(dx / (2 * Size) * _cellCount)) % 2;
            var v2 = (int)(Math.Round(dy / (2 * Size) * _cellCount)) % 2;
            var v3 = (int)(Math.Round(dz / (2 * Size) * _cellCount)) % 2;

            return (v1 ^ v2 ^ v3) == 1 ? _color1 : _color2;
        }
    }
}