using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;

namespace SprunthFramework
{
    public class GameManager
    {
        private RenderTexture tex;
        private RenderWindow _window;

        private Display activeDisplay;
        public Display ActiveDisplay
        {
            get { return activeDisplay; }
            set
            {
                if (ActiveDisplay != null)
                    ActiveDisplay.EventSubscribe(false, _window);

                value.EventSubscribe(true, _window);

                activeDisplay = value;
            }
        }

        public GameManager(RenderWindow window)
        {
            _window = window;

            // blank display
            ActiveDisplay = new Display(new Vector2u(window.Size.X, window.Size.Y));

            tex = new RenderTexture(window.Size.X, window.Size.Y);
        }

        public void Update()
        {
            ActiveDisplay.Update();
        }

        public Texture Draw()
        {
            tex.Clear(Color.Black);
            ActiveDisplay.Draw(tex);
            tex.Display();
            return tex.Texture;
        }
    }
}
