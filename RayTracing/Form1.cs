using System.Windows.Forms;

namespace RayTracing
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            Width = 720;
            Height = 480;

            var rayTracer = new RayTracer(box);
            KeyDown += rayTracer.KeyDown;

            rayTracer.Run();
        }
    }
}
