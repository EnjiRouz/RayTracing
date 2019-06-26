namespace RayTracing
{
    internal struct Material
    {
        // показатель преломления
        public double Refractive;

        // показатель рассеянности
        public Vector Diffuse;

        // показатель отражаемости
        public double Specular;

        // коэффициент смешивания (характеристика отражательных свойств поверхности)
        public double[] Albedo; 
    }
}