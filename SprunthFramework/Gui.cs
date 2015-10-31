using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gwen.Control;
using Gwen.Skin;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace SprunthFramework
{
    class Gui : IDisposable
    {
        private readonly Gwen.Input.SFML _input;
        public Canvas GuiCanvas { get; private set; }
        private readonly Gwen.Renderer.SFML _renderer;
        private readonly TexturedBase skin;

        /// <summary>
        /// Contains the shared constructor code
        /// </summary>
        /// <param name="target"></param>
        private Gui(RenderTarget target)
        {
            _renderer = new Gwen.Renderer.SFML(target);

            skin = new TexturedBase(_renderer, "DefaultSkin.png");

            // load font. TODO: remove hardcoding
            using (var font = new Gwen.Font(_renderer, "OpenSans.ttf", 14))
            {
                if (_renderer.LoadFont(font))
                    _renderer.FreeFont(font);
                else
                {
                    font.FaceName = "Arial Unicode MS";
                    if (_renderer.LoadFont(font))
                        _renderer.FreeFont(font);
                    else
                    {
                        font.FaceName = "Arial";
                        _renderer.LoadFont(font);
                    }
                }

                skin.SetDefaultFont(font.FaceName, 14);
            }

            GuiCanvas = new Canvas(skin);
            GuiCanvas.SetSize((int)target.Size.X, (int)target.Size.Y);
            GuiCanvas.ShouldDrawBackground = false;
            GuiCanvas.BackgroundColor = System.Drawing.Color.Black;
            GuiCanvas.KeyboardInputEnabled = true;

            _input = new Gwen.Input.SFML();
            _input.Initialize(GuiCanvas, target);
        }

        /// <summary>
        /// Constructor for GUIs that attach to a specific display
        /// </summary>
        /// <param name="target"></param>
        /// <param name="display"></param>
        public Gui(RenderTarget target, Display display) : this(target)
        {
            display.OnKeyPress += window_KeyPressed;
            display.OnKeyRelease += window_KeyReleased;
            display.OnMousePress += window_MouseButtonPressed;
            display.OnMouseRelease += window_MouseButtonReleased;
            display.OnMouseMove += window_MouseMoved;
            display.OnMouseWheelMove += window_MouseWheelMoved;
            // No text entered or resized events for display yet.
        }

        /// <summary>
        /// Constructor for base GUI that attached directly to the window
        /// </summary>
        /// <param name="window"></param>
        public Gui(RenderWindow window) : this((RenderTarget)window)
        {
            window.KeyPressed += window_KeyPressed;
            window.Resized += OnResized;
            window.KeyReleased += window_KeyReleased;
            window.MouseButtonPressed += window_MouseButtonPressed;
            window.MouseButtonReleased += window_MouseButtonReleased;
            window.MouseWheelMoved += window_MouseWheelMoved;
            window.MouseMoved += window_MouseMoved;
            window.TextEntered += window_TextEntered;
        }

        public void Dispose()
        {
            GuiCanvas.Dispose();
            skin.Dispose();
            _renderer.Dispose();
        }

        public void Draw()
        {
            GuiCanvas.RenderCanvas();
        }

        void window_TextEntered(object sender, TextEventArgs e)
        {
            _input.ProcessMessage(e);
        }

        void window_MouseMoved(object sender, MouseMoveEventArgs e)
        {
            _input.ProcessMessage(e);
        }
        void window_MouseMoved(object sender, MouseMoveEventArgs e, Vector2f displayCoords)
        {
            window_MouseMoved(sender, e);
        }

        void window_MouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
            _input.ProcessMessage(new Gwen.Input.SFMLMouseButtonEventArgs(e, true));
        }
        void window_MouseButtonPressed(object sender, MouseButtonEventArgs e, Vector2f displayCoords)
        {
            window_MouseButtonPressed(sender, e);
        }

        void window_MouseButtonReleased(object sender, MouseButtonEventArgs e)
        {
            _input.ProcessMessage(new Gwen.Input.SFMLMouseButtonEventArgs(e, false));
        }
        void window_MouseButtonReleased(object sender, MouseButtonEventArgs e, Vector2f displayCoords)
        {
            window_MouseButtonReleased(sender, e);
        }

        void window_MouseWheelMoved(object sender, MouseWheelEventArgs e)
        {
            _input.ProcessMessage(e);
        }
        void window_MouseWheelMoved(object sender, MouseWheelEventArgs e, Vector2f displayCoords)
        {
            window_MouseWheelMoved(sender, e);
        }

        void window_KeyReleased(object sender, KeyEventArgs e)
        {
            _input.ProcessMessage(new Gwen.Input.SFMLKeyEventArgs(e, false));
        }
        
        /// <summary>
        /// Function called when a key is pressed
        /// </summary>
        void window_KeyPressed(object sender, KeyEventArgs e)
        {
             _input.ProcessMessage(new Gwen.Input.SFMLKeyEventArgs(e, true));
        }

        /// <summary>
        /// Function called when the window is resized
        /// </summary>
        void OnResized(object sender, SizeEventArgs e)
        {
            GuiCanvas.SetSize((int)e.Width, (int)e.Height);
        }

    }
}
