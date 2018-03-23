using Alien_World.App;

namespace Alien_World
{
    class Program
    {
        static int Main(string[] args)
        {
            Application app = Application.Instance;
            app.Init(title: "Alien World", width: 1280, height: 780, vsync: false, fullscreen: false);
            app.Layers.Add(new Editor());
            return app.Run();
        }
    }
}
