using System;

namespace RayTracing
{
    internal class Camera
    {
        private const double Pi2 = Math.PI * 2;
        private const double Movement = 0.5;

        private readonly Vector _position;
        private readonly Vector _angle;
        private readonly Vector _sin=new Vector();
        private readonly Vector _cos = new Vector();

        /// <summary>
        /// Инициализация камеры (наблюдателя)
        /// </summary>
        /// <param name="position"> позиция камеры (наблюдателя)</param>
        /// <param name="angle"> угол просмотра </param>
        public Camera(Vector position, Vector angle)
        {
            _position = position;
            _angle = angle;

            UpdateAngle();
        }

        /// <summary>
        /// Обновление угла просмотра при изменении его с клавиатуры
        /// </summary>
        private void UpdateAngle()
        {
            _sin.X = Math.Sin(_angle.X);
            _sin.Y = Math.Sin(_angle.Y);

            _cos.X = Math.Cos(_angle.X);
            _cos.Y = Math.Cos(_angle.Y);
        }

        /// <summary>
        /// Получение направления луча из камеры по двум осям (x, y)
        /// </summary>
        /// <param name="x"> координата на оси x </param>
        /// <param name="y"> координата на оси y </param>
        /// <returns> вектор (направление) луча </returns>
        public Vector GetDirection(double x, double y)
        {
            var newY = _cos.X * y + _sin.X;
            var z = -_sin.X * y + _cos.X;
            var newX = _cos.Y * x + _sin.Y * z;
            var newZ = -_sin.Y * x + _cos.Y * z;
            var length = Math.Sqrt(newX * newX + newY * newY + newZ * newZ);
            return new Vector(newX / length, newY / length, newZ / length);
        }

        /// <summary>
        /// Получение позиции камеры (наблюдателя)
        /// </summary>
        /// <returns> позиция камеры (наблюдателя) </returns>
        public Vector GetPosition()
        {
            return _position;
        }

        /// <summary>
        /// Получение угла просмотра камеры
        /// </summary>
        /// <returns> угол просмотра камеры </returns>
        public Vector GetAngle()
        {
            return _angle;
        }

        /// <summary>
        /// Поворот камеры по осям x, y
        /// </summary>
        /// <param name="x"> координата на оси x </param>
        /// <param name="y"> координата на оси y </param>
        public void Rotate(double x, double y)
        {
            _angle.X = (_angle.X + x + Pi2) % Pi2;
            _angle.Y = (_angle.Y + y + Pi2) % Pi2;

            UpdateAngle();
        }

        /// <summary>
        /// Движение камеры вперёд
        /// </summary>
        public void MoveForward()
        {
            _position.Y += Math.Sin(_angle.X) * Movement;
            _position.X += Math.Cos(_angle.Y + Math.PI / 2) * Math.Cos(_angle.X) * Movement;
            _position.Z += Math.Sin(_angle.Y + Math.PI / 2) * Math.Cos(_angle.X) * Movement;
        }

        /// <summary>
        /// Движение камеры назад
        /// </summary>
        public void MoveBackward()
        {
            _position.Y -= Math.Sin(_angle.X) * Movement;
            _position.X += Math.Cos(_angle.Y - Math.PI / 2) * Math.Cos(_angle.X) * Movement;
            _position.Z += Math.Sin(_angle.Y - Math.PI / 2) * Math.Cos(_angle.X) * Movement;
        }

        /// <summary>
        /// Движение камеры влево
        /// </summary>
        public void MoveLeft()
        {
            _position.X -= Math.Cos(_angle.Y) * Movement;
            _position.Z -= Math.Sin(_angle.Y) * Movement;
        }

        /// <summary>
        /// Движение камеры вправо
        /// </summary>
        public void MoveRight()
        {
            _position.X += Math.Cos(_angle.Y) * Movement;
            _position.Z += Math.Sin(_angle.Y) * Movement;
        }

        /// <summary>
        /// Движение камеры вверх
        /// </summary>
        public void MoveUp()
        {
            _position.Y += Movement;
        }

        /// <summary>
        /// Движение камеры вниз
        /// </summary>
        public void MoveDown()
        {
            _position.Y -= Movement;
        }
    }
}
