using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace SprunthFramework
{
    public abstract class SprunthGame
    {
        private GameManager GM;

        public Vector2u ScreenSize { get; private set; }
        public RenderWindow GameWindow { get; private set; }

        public SprunthGame(Vector2u screenSize)
        {
            ScreenSize = screenSize;
        }

        public void Initialize(string windowTitle = "", GameManager GM = null, uint fpsLimit = 60)
        {
            GameWindow = new RenderWindow(new VideoMode(ScreenSize.X, ScreenSize.Y), windowTitle, Styles.Close | Styles.Titlebar, new ContextSettings(32,32,4,1,0));

            this.GM = GM ?? new GameManager(GameWindow);

            GameWindow.SetFramerateLimit(fpsLimit);

            GameWindow.Closed += GameWindow_Closed;

            PostInitialize();
        }

        public virtual void PostInitialize()
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
                GameWindow.Display();
            }
        }

        public virtual void GameWindow_Closed(object sender, EventArgs e)
        {
            GameWindow.Close();
        }


    }
}
