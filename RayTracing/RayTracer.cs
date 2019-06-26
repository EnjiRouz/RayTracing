using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RayTracing
{
    internal class RayTracer
    {
        // число, близкое к нулю для сравнения вещественных чисел
        private const double Zero = 1e-4;

        // глубина трассировки
        private const int Depth = 4;

        // цвет фона
        private readonly Vector _backgroundColor = new Vector(1.0, 1.0, 1.0);

        // различные материалы
        private readonly Material _glass = new Material() { Refractive = 1.4, Albedo = new[] { 0, 0.5, 0.1, 0.8 },
                                                        Diffuse = new Vector(0.6, 0.7, 0.8), Specular = 125 };
        private readonly Material _mirror = new Material() { Refractive = 1.0, Albedo = new[] { 0, 10, 0.8, 0 }, 
                                                            Diffuse = new Vector(1, 1, 1), Specular = 1425 };
        private readonly Material _red = new Material() { Refractive = 1.0, Albedo = new[] { 1, 0.5, 0.2, 0 }, 
                                                            Diffuse = new Vector(1, 0, 0), Specular = 500 };
        private readonly Material _blue = new Material() { Refractive = 1.0, Albedo = new[] { 1, 0.5, 0.3, 0 }, 
                                                            Diffuse = new Vector(0, 0, 1), Specular = 500 };
        private readonly Material _green = new Material() { Refractive = 1.0, Albedo = new[] { 1, 0.5, 0.5, 0 }, 
                                                                Diffuse = new Vector(0, 1, 0), Specular = 10 };
        private readonly Material _yellow = new Material() { Refractive = 1.0, Albedo = new[] { 0.8, 10, 0.5, 0 },
                                                             Diffuse = new Vector(1, 1, 0), Specular = 1000 };

        // ширина и высота окна (картинки)
        private readonly int _width;
        private readonly int _height;

        // поле для отрисовки картинки
        private readonly PictureBox _box;

        // картинка для отрисовки
        private readonly Bitmap _bitmap;

        // камера
        private Camera _camera; 

        // список объектов на сфене
        private List<Shape> _shapes;

        // список источников освещения
        private List<Light> _lights;

        // координаты X, Y
        private readonly double[] _x;
        private readonly double[] _y;

        // буфер для запоминания цвета лучей
        private readonly Color[] _buffer;

        /// <summary>
        /// Инициализация трассировщика лучей
        /// </summary>
        /// <param name="box"> поле для отрисовки картинки </param>
        public RayTracer(PictureBox box)
        {
            _box = box;
            _width = box.Width;
            _height = box.Height;

            // создание картинки, буффера цветов и массива координат
            _bitmap = new Bitmap(_width, _height);
            _buffer = new Color[_width * _height];
            _x = new double[_width * _height];
            _y = new double[_width * _height];

            var windowSize = _width * _height;
            var size = Math.Max(_width, _height);

            for (var i = 0; i < windowSize; i++)
            {
                _x[i] = (double)(i % _width) / size - 0.5;
                _y[i] = 0.5 - ((double)i / _width) / size;
            }

            InitScene();
        }

        /// <summary>
        /// Инициализация сцены (создание камеры, фигур, источников света)
        /// </summary>
        private void InitScene()
        {
            _camera = new Camera(new Vector(-0.25, 0, -1.5), new Vector(6.1, 0, 0));

            _shapes = new List<Shape>
            {
                new Sphere(-1.0, 0, 3, 0.5, _red),
                new Sphere(0, 0.5, 3, 0.3, _green),
                new Sphere(1.0, 0, 3, 0.6, _blue),
                new Cylinder(0,-0.3,0.5,0.25,0.5, _glass),
                new Cylinder(0,0,3.5,0.5,0.75, _mirror),
                new ChessGeometryPlane(new Vector(0, 0, 12), 0, 1, 0, 0.5, 16, 
                    100, _yellow, new Vector(0, 0, 0), new Vector(1, 1, 1)),
            };

            _lights = new List<Light>
            {
                new Light(LightType.Point, 0.3, new Vector(2, 1, 0)),
                new Light(LightType.Point, 0.3, new Vector(0, 1, 0)),
                new Light(LightType.Point, 0.3, new Vector(-2, 1, 0)),
                new Light(LightType.Directional, 0.5, new Vector(0, 1, 1))
            };
        }

        /// <summary>
        /// Преломление по закону Снеллиуса
        /// </summary>
        /// <param name="I"> угол падения </param>
        /// <param name="n"> коэффициент преломления </param>
        /// <param name="cos"> косинус угла </param>
        /// <param name="thetaT"> угол преломления света </param>
        /// <param name="thetaI"> угол падения света </param>
        /// <returns> лучевой вектор падающего и преломленного световых лучей </returns>
        private static Vector RefractRay(Vector I, Vector n, double cos, double thetaT, double thetaI = 1)
        {
            if (cos < 0) return RefractRay(I, -n, -cos, thetaI, thetaT);
            var eta = thetaI / thetaT;
            var k = 1 - eta * eta * (1 - cos * cos);
            return k < 0 ? new Vector(1, 0, 0) : I * eta + n * (eta * cos - Math.Sqrt(k));
        }

        /// <summary>
        /// Нахождение ближайшего пересечения с объектом
        /// </summary>
        /// <param name="origin"> начальная точка </param>
        /// <param name="direction"> направление </param>
        /// <param name="minDistance"> минимальное расстояние </param>
        /// <param name="maxDistance"> максимальное расстояние </param>
        /// <param name="closestShape"> ближайшая фигура </param>
        /// <param name="closestDistance"> ближайшее расстояние </param>
        private void ClosestIntersection(Vector origin, Vector direction, double minDistance, double maxDistance, 
                                        out Shape closestShape, out double closestDistance)
        {
            // предполагает, что нет пересечения
            closestDistance = double.PositiveInfinity; 
            closestShape = null;

            // проходит по всем фигурам
            foreach (var shape in _shapes)
            {
                // если нет пересечения - пееходит к следующей фигуре
                if (!shape.DoesRayIntersect(origin, direction, out var t)) 
                    continue;

                // если удовлетворяет параметрам луча (tmin, tmax) и меньше ближайшего
                // расстония - обновляет ближайшее расстоние и запоминает ближайшую фигуру
                if (!(t >= minDistance) || !(t <= maxDistance) || !(t < closestDistance)) continue;
                closestDistance = t;
                closestShape = shape;
            }
        }

        /// <summary>
        /// Проверка наличия пересечения луча с каким-либо объектом сцены
        /// </summary>
        /// <param name="origin"> начальная точка </param>
        /// <param name="direction"> направление </param>
        /// <param name="minDistance"> минимальное расстояние </param>
        /// <param name="maxDistance"> максимальное расстояние </param>
        /// <returns> true, если нашли хоть ождно пересечение, удовлетворяющее границам интервала </returns>
        private bool HaveIntersection(Vector origin, Vector direction, double minDistance, double maxDistance)
        {
            foreach (var shape in _shapes)
                if (shape.DoesRayIntersect(origin, direction, out var t) && t >= minDistance && t <= maxDistance)
                    return true;

            return false;
        }

        /// <summary>
        /// Луч
        /// </summary>
        /// <param name="origin"> начальная точка </param>
        /// <param name="direction"> направление </param>
        /// <param name="minDistance"> минимальное расстояние </param>
        /// <param name="maxDistance"> максимальное расстояние </param>
        /// <param name="depth"> глубина трассировки </param>
        /// <returns></returns>
        private Vector Ray(Vector origin, Vector direction, double minDistance, double maxDistance, int depth)
        {
            // находит ближайший объект, с которым пересекается луч
            ClosestIntersection(origin, direction, minDistance, maxDistance, 
                                out var closestShape, out var closestDistance);

            // если луч ни с чем не пересекается - возвращает цвет фона
            if (closestShape == null)
                return _backgroundColor;

            // находит точку перемесения луча с объектом, нормаль в этой точке, материал ближайшего объекта
            var point = origin + direction * closestDistance; 
            var normal = closestShape.Normal(point);
            var material = closestShape.Material;

            // вычисляет освещение источников света и итоговый цвет пикселя
            ComputeLighting(point, normal, direction, closestShape.Material, out var diffuse, out var specular); 
            var diffuseColor = closestShape.GetColor(point) * diffuse;
            var specularColor = new Vector(specular, specular, specular);
            var localColor = diffuseColor + specularColor;

            // если достигнута минимальная глубина - возвращает цвет 
            if (depth < 0) 
                return localColor;

            // вычисляет косинус угла между направлением луча и нормалью
            var directionCos = direction * normal;

            // находит направление луча отражения, добавляет показатель отражаемости
            if (Math.Abs(material.Albedo[2]) >Zero)
            {
                var reflectDirection = direction - normal * (2 * directionCos); 
                var reflectColor = Ray(point, reflectDirection, Zero, 
                                        double.PositiveInfinity, depth - 1);
                localColor += reflectColor * material.Albedo[2];
            }

            // находит направление луча преломления, добавляет показатель преломления
            if (!(Math.Abs(material.Albedo[3]) > Zero)) return localColor;
            {
                var refractDirection = RefractRay(direction, normal, 
                                                -directionCos, material.Refractive); 
                var refractColor = Ray(point, refractDirection, Zero, 
                                        double.PositiveInfinity, depth - 1);
                localColor += refractColor * material.Albedo[3];
            }

            // смешивает цвет с соответствующими коэффициентами
            return localColor; 
        }

        /// <summary>
        /// Вычисление освещенности
        /// </summary>
        /// <param name="point"> точка, в которой вычисляется свет </param>
        /// <param name="normal"> нормаль </param>
        /// <param name="direction"> направление луча света </param>
        /// <param name="material"> материал поверхности </param>
        /// <param name="diffuse"> показатель рассеянности </param>
        /// <param name="specular"> показатель отражаемости </param>
        private void ComputeLighting(Vector point, Vector normal, Vector direction, Material material, 
                                    out double diffuse, out double specular)
        {
            diffuse = 0;
            specular = 0;

            // проходит по всем источникам освещения
            foreach (var light in _lights)
            {
                // максимальное расстояние
                double maxDistance;

                // вычисляет вектор направления луча от источника света
                Vector lightDirection;

                if (light.Type == LightType.Point)
                {
                    lightDirection = light.Position - point;
                    maxDistance = lightDirection.GetLength() - Zero;
                    lightDirection = lightDirection.Normalize();
                }
                else {
                    lightDirection = light.Position;
                    maxDistance = double.PositiveInfinity;
                }

                // проверяет на нахождение в тени и переходит к следующему источнику
                if (HaveIntersection(point, lightDirection, Zero, maxDistance))
                    continue;

                // вычисляет косинус угла между источником света и нормалью
                var lightCos = lightDirection * normal;

                // вычисляет косинус угла между отражённым лучём и направлением луча
                var specularCos = (lightDirection - normal * (2 * lightCos)) * direction;

                // увеличение показателя рассеянности
                if (lightCos > 0)
                    diffuse += lightCos * light.Intensity;

                // увеличение показателя отражаемости
                if (specularCos > 0)
                    specular += Math.Pow(specularCos, material.Specular) * light.Intensity;
            }

            diffuse *= material.Albedo[0];
            specular *= material.Albedo[1];
        }

        /// <summary>
        /// Обратная трассировка лучей и отображение результата на картинке
        /// </summary>
        public void Run()
        {
            // находит общее число лучей для трассировки (количество пикселей, которые надо просчитать)
            var rayCount = _width * _height; 

            var origin = _camera.GetPosition();

            // параллельно испускает лучи через каждый пиксель
            Parallel.For(0, rayCount, i => 
            {
                // получает вектор направления луча и испускает его
                var direction = _camera.GetDirection(_x[i], _y[i]); 
                var color = Ray(origin, direction, 0, double.PositiveInfinity, Depth);
                
                // преобразует вектор в цвет
                _buffer[i] = color.ToColor(); 
            });

            // отрисовывает содержимое буфера на картинке
            var index = 0;

            for (var y = 0; y < _height; y++)
                for (var x = 0; x < _width; x++)
                    _bitmap.SetPixel(x, y, _buffer[index++]);

            _box.Image = _bitmap;
            _box.Update();
        }

        /// <summary>
        /// Обработка нажатия клавиш на клавиатуре
        /// </summary>
        /// <param name="sender"> отправитель оповещения о нажатии клавиши </param>
        /// <param name="e"> клавиша </param>
        public void KeyDown(object sender, KeyEventArgs e)
        {
            MoveCamera(e);
            RotateCamera(e);
            Run();
        }

        /// <summary>
        /// Поворот камеры
        /// </summary>
        /// <param name="e"> клавиша </param>
        private void RotateCamera(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Left:
                    _camera.Rotate(0, -0.15);
                    break;
                case Keys.Right:
                    _camera.Rotate(0, 0.15);
                    break;
                case Keys.Up:
                    _camera.Rotate(0.15, 0);
                    break;
                case Keys.Down:
                    _camera.Rotate(-0.15, 0);
                    break;
            }
        }

        /// <summary>
        /// Передвижение камеры
        /// </summary>
        /// <param name="e"> клавиша </param>
        private void MoveCamera(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.A:
                    _camera.MoveLeft();
                    break;
                case Keys.D:
                    _camera.MoveRight();
                    break;
                case Keys.W:
                    _camera.MoveForward();
                    break;
                case Keys.S:
                    _camera.MoveBackward();
                    break;
                case Keys.Q:
                    _camera.MoveDown();
                    break;
                case Keys.E:
                    _camera.MoveUp();
                    break;
            }
        }
    }
}