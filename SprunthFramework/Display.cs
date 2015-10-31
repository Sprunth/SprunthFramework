using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gwen.Control;
using SFML.Graphics;
using SFML.System;

namespace SprunthFramework
{
    public class Display : DrawableBase
    {
        private readonly Gui _gui;
        protected Canvas GuiCanvas { get { return _gui.GuiCanvas; } }

        public Display(Vector2u displaySize) : base(displaySize)
        {
            _gui = new Gui(Target, this);
        }

        protected override void DrawExtra(RenderTarget target)
        {
            _gui.Draw();
        }
    }
}
