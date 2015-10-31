using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gwen.Control;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace SprunthFramework
{
    public abstract class SprunthGame : IDisposable
    {
        private GameManager GM;

        public Vector2u ScreenSize { get; private set; }
        public RenderWindow GameWindow { get; private set; }
        private Gui _gui;
        public Canvas RootCanvas { get { return _gui.GuiCanvas; } }

        public SprunthGame(Vector2u screenSize)
        {
            ScreenSize = screenSize;
        }

        public void Initialize(string windowTitle = "", GameManager defaultManager = null, uint fpsLimit = 60)
        {
            GameWindow = new RenderWindow(new VideoMode(ScreenSize.X, ScreenSize.Y), windowTitle, Styles.Close | Styles.Titlebar, new ContextSettings(32,32,4,1,0));

            GM = defaultManager ?? new GameManager(GameWindow);

            GameWindow.SetFramerateLimit(fpsLimit);

            GameWindow.Closed += GameWindow_Closed;
            GameWindow.Resized += GameWindow_Resized;

            _gui = new Gui(GameWindow);

            PostInitialize();
        }

        protected virtual void PostInitialize()
        {
            
        }

        public void Run()
        {
            var spr = new Sprite();
            while (GameWindow.IsOpen)
            {
                GameWindow.DispatchEvents();
                
                GM.Update();

                spr.Texture = GM.Draw();

                GameWindow.Clear();
                GameWindow.Draw(spr);
                _gui.Draw();
                GameWindow.Display();
            }
        }

        public virtual void GameWindow_Closed(object sender, EventArgs e)
        {
            GameWindow.Close();
        }

        public virtual void GameWindow_Resized(object sender, SizeEventArgs e)
        {
            GameWindow.SetView(new View(new FloatRect(0f, 0f, e.Width, e.Height)));
        }

        public void Dispose()
        {
            GameWindow.Dispose();
        }
    }
}
