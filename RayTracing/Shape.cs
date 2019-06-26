namespace RayTracing
{
    internal abstract class Shape
    {
        // задавание материала
        public Material Material; 

        // проверка на пересечение с лучом
        public abstract bool DoesRayIntersect(Vector origin, Vector direction, out double t); 

        // получение нормали
        public abstract Vector Normal(Vector point); 

        // получение цвета в точке
        public virtual Vector GetColor(Vector point)
        {
            return Material.Diffuse;
        }
    }
}