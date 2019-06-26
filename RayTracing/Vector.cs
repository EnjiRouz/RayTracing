using System;
using System.Drawing;

namespace RayTracing
{
    internal class Vector
    {
        public double X, Y, Z;

        /// <summary>
        /// Создание нулевого вектора
        /// </summary>
        public Vector()
        {
            X = 0.0;
            Y = 0.0;
            Z = 0.0;
        }

        /// <summary>
        /// Инициализация ненулевого вектора
        /// </summary>
        /// <param name="vector"> вектор 3d, имеющий x,y,z координаты </param>
		public Vector(Vector vector)
        {
            X = vector.X;
            Y = vector.Y;
            Z = vector.Z;
        }

        /// <summary>
        /// Инициализация ненулевого вектора
        /// </summary>
        /// <param name="x"> x-коррдината вектора </param>
        /// <param name="y"> y-коррдината вектора </param>
        /// <param name="z"> z-коррдината вектора </param>
	    public Vector(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Сложение векторов
        /// </summary>
        /// <param name="vector1"> вектор 1, который требуется сложить с вектором 2 </param>
        /// <param name="vector2"> вектор 2, который требуется сложить с вектором 1 </param>
        /// <returns> новый вектор, представляющий собой сумму двух векторов </returns>
        public static Vector operator +(Vector vector1, Vector vector2)
        {
            return new Vector(vector1.X + vector2.X, vector1.Y + vector2.Y, vector1.Z + vector2.Z);
        }

        /// <summary>
        /// Вычитание векторов
        /// </summary>
        /// <param name="vector1"> вектор 1, из которого требуется вычесть вектор 2 </param>
        /// <param name="vector2"> вектор 2, который требуется вычесть из вектора 1 </param>
        /// <returns> новый вектор, представляющий собой разность двух векторов </returns>
        public static Vector operator -(Vector vector1, Vector vector2)
        {
            return new Vector(vector1.X - vector2.X, vector1.Y - vector2.Y, vector1.Z - vector2.Z);
        }

        /// <summary>
        /// Вычисление скалярного произведения двух векторов
        /// </summary>
        /// <param name="vector1"> вектор 1, который требуется умножить на вектор 2 </param>
        /// <param name="vector2"> вектор 2, который требуется умножить на вектор 1 </param>
        /// <returns> скалярное произведение двух векторов </returns>
        public static double operator *(Vector vector1, Vector vector2)
        {
            return vector1.X * vector2.X + vector1.Y * vector2.Y + vector1.Z * vector2.Z;
        }

        /// <summary>
        /// Умножение вектора на число
        /// </summary>
        /// <param name="vector"> вектор, который требуется умножить на число </param>
        /// <param name="multiplier"> множитель </param>
        /// <returns> вектор, умноженный на число </returns>
        public static Vector operator *(Vector vector, double multiplier)
        {
            return new Vector(vector.X * multiplier, vector.Y * multiplier, vector.Z * multiplier);
        }

        /// <summary>
        /// Инверсия вектора (умножение на -1)
        /// </summary>
        /// <param name="vector"> исходный вектор, который требуется инверсировать </param>
        /// <returns> инверсированный вектор </returns>
        public static Vector operator -(Vector vector)
        {
            return new Vector(-vector.X, -vector.Y, -vector.Z);
        }

        /// <summary>
        /// Вращение по оси X
        /// </summary>
        /// <param name="angle"> угол вращения </param>
        /// <returns> вектор, полученный в результате вращения </returns>
        public Vector RotateX(double angle)
        {
            var vector = new Vector
            {
                X = X,
                Y = Math.Cos(angle) * Y - Math.Sin(angle) * Z,
                Z = Math.Sin(angle) * Y + Math.Cos(angle) * Z
            };
            return vector;
        }

        /// <summary>
        /// Вращение по оси Y
        /// </summary>
        /// <param name="angle"> угол вращения </param>
        /// <returns> вектор, полученный в результате вращения </returns>
        public Vector RotateY(double angle)
        {
            var vector = new Vector
            {
                X = Math.Cos(angle) * X - Math.Sin(angle) * Z,
                Y = Y,
                Z = Math.Sin(angle) * X + Math.Cos(angle) * Z
            };
            return vector;
        }

        /// <summary>
        /// Вращение по оси Z
        /// </summary>
        /// <param name="angle"> угол вращения </param>
        /// <returns> вектор, полученный в результате вращения </returns>
        public Vector RotateZ(double angle)
        {
            var vector = new Vector
            {
                X = Math.Cos(angle) * X - Math.Sin(angle) * Y,
                Y = Math.Sin(angle) * X + Math.Cos(angle) * Y,
                Z = Z
            };
            return vector;
        }
        
        /// <summary>
        /// Получение длины вектора
        /// </summary>
        /// <returns> длина вектора </returns>
        public double GetLength()
        {
            return Math.Sqrt(X * X + Y * Y + Z * Z);
        }

        /// <summary>
        /// Нормализация вектора (деление каждого компонента вектора на его длину)
        /// </summary>
        /// <returns> нормализованный вектор </returns>
        public Vector Normalize()
        {
            var vectorLength = GetLength();

            if (Math.Abs(vectorLength) <= 0)
                vectorLength = 1;

            return new Vector(X / vectorLength, Y / vectorLength, Z / vectorLength);
        }

        /// <summary>
        /// Преобразование вектора в цвет
        /// </summary>
        /// <returns> цвет пикселя </returns>
        public Color ToColor()
        {
            var red = (int)(255 * Math.Min(1, Math.Max(0, X)));
            var green = (int)(255 * Math.Min(1, Math.Max(0, Y)));
            var blue = (int)(255 * Math.Min(1, Math.Max(0, Z)));

            return Color.FromArgb(red, green, blue);
        }
    }
}