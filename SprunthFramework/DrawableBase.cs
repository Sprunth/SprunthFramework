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
    public class DrawableBase
    {
        protected RenderTexture Target { get; private set; }
        private readonly Sprite _targetSpr;

        public Vector2f Position
        {
            get { return _targetSpr.Position; }
            set { _targetSpr.Position = value; }
        }

        protected Color BackgroundColor;

        public DrawableBase(Vector2u displaySize)
        {
            toUpdate = new List<IUpdatable>();
            toDraw = new Dictionary<uint, List<Drawable>>();

            _targetSpr = new Sprite();

            BackgroundColor = Color.Transparent;

            Target = new RenderTexture(displaySize.X, displaySize.Y)
            {
                Smooth = true
            };
        }

        public virtual void Update()
        {
            foreach (var updatable in toUpdate)
            {
                updatable.Update();
            }
        }

        public void Draw(RenderTarget sourceTarget)
        {
            Target.Clear(BackgroundColor);
            foreach (var tup in toDraw)
            {
                tup.Value.ForEach(drawable => Target.Draw(drawable));
            }

            DrawExtra(Target);
            
            Target.Display();
            _targetSpr.Texture = Target.Texture;
            
            sourceTarget.Draw(_targetSpr);
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            Draw(target);
        }

        /// <summary>
        /// Is called right before finishing the drawing for this DrawableBase
        /// Overriding this allows for inherited classes to draw their own things that
        ///     do not fit in the Drawable class, such as the Gwen GUI for Displays
        /// </summary>
        /// <param name="target"></param>
        protected virtual void DrawExtra(RenderTarget target)
        {
            
        }

        /// <summary>
        /// Called when resumes
        /// </summary>
        protected virtual void OnResume()
        {

        }

        /// <summary>
        /// Called right before closed
        /// </summary>
        protected virtual void OnPause()
        {

        }

        public void AddItemToDraw(Drawable drawable, uint zlevel)
        {
            var exists = false;
            foreach (var tup in toDraw)
            {
                if (tup.Value.Contains(drawable))
                    exists = true;
            }
            if (exists)
                return;
            if (!toDraw.ContainsKey(zlevel))
                toDraw.Add(zlevel, new List<Drawable>());
            toDraw[zlevel].Add(drawable);
        }

        public void RemoveItemToDraw(Drawable drawable, uint zlevel)
        {
            // Could be sped up with binary search, or keep an index.
            toDraw[zlevel].Remove(drawable);
        }

        #region events
        /// <summary>
        /// Subscribes and unsubscribes to window events
        /// </summary>
        /// <param name="on"></param>
        /// <param name="window"></param>
        public void EventSubscribe(bool on, RenderWindow window)
        {
            if (on)
            {
                OnResume();
                window.LostFocus += LostFocus;
                window.KeyPressed += KeyPressed;
                window.KeyReleased += KeyReleased;
                window.MouseMoved += MouseMoved;
                window.MouseWheelMoved += MouseWheelMoved;
                window.MouseButtonPressed += MousePressed;
                window.MouseButtonReleased += MouseReleased;
            }
            else
            {
                OnPause();
                window.LostFocus -= LostFocus;
                window.KeyPressed -= KeyPressed;
                window.KeyReleased -= KeyReleased;
                window.MouseMoved -= MouseMoved;
                window.MouseWheelMoved -= MouseWheelMoved;
                window.MouseButtonPressed -= MousePressed;
                window.MouseButtonReleased -= MouseReleased;
            }
        }

        private readonly List<IUpdatable> toUpdate;
        private Dictionary<uint, List<Drawable>> toDraw;

        public delegate void LostFocusHandler(object sender, EventArgs e);
        public event LostFocusHandler OnLostFocus;

        public delegate void KeyPressHandler(object sender, KeyEventArgs e);
        public event KeyPressHandler OnKeyPress;

        public delegate void KeyReleaseHandler(object sender, KeyEventArgs e);
        public event KeyReleaseHandler OnKeyRelease;

        public delegate void MouseWheelMoveHandler(object sender, MouseWheelEventArgs e, Vector2f displayCoords);
        public event MouseWheelMoveHandler OnMouseWheelMove;

        public delegate void MouseMoveHandler(object sender, MouseMoveEventArgs e, Vector2f displayCoords);
        public event MouseMoveHandler OnMouseMove;

        public delegate void MousePressHandler(object sender, MouseButtonEventArgs e, Vector2f displayCoords);
        public event MousePressHandler OnMousePress;

        public delegate void MouseReleaseHandler(object sender, MouseButtonEventArgs e, Vector2f displayCoords);
        public event MouseReleaseHandler OnMouseRelease;

        private void LostFocus(object sender, EventArgs e)
        {
            if (OnLostFocus != null)
            {
                OnLostFocus(sender, e);
            }
        }

        private void KeyPressed(object sender, KeyEventArgs e)
        {
            if (OnKeyPress != null)
            {
                OnKeyPress(sender, e);
            }
        }

        private void KeyReleased(object sender, KeyEventArgs e)
        {
            if (OnKeyRelease != null)
            {
                OnKeyRelease(sender, e);
            }
        }

        private void MouseMoved(object sender, MouseMoveEventArgs e)
        {
            if (OnMouseMove != null && ContainsPoint(new Vector2i(e.X, e.Y)))
            {
                OnMouseMove(sender, e, MouseCoordToDisplayCoord(new Vector2i(e.X, e.Y)));
            }
        }

        private void MouseWheelMoved(object sender, MouseWheelEventArgs e)
        {
            if (OnMouseWheelMove != null && ContainsPoint(new Vector2i(e.X, e.Y)))
            {
                OnMouseWheelMove(sender, e, MouseCoordToDisplayCoord(new Vector2i(e.X, e.Y)));
            }
        }

        private void MousePressed(object sender, MouseButtonEventArgs e)
        {
            if (OnMousePress != null)
            {
                OnMousePress(sender, e, MouseCoordToDisplayCoord(new Vector2i(e.X, e.Y)));
            }
        }

        private void MouseReleased(object sender, MouseButtonEventArgs e)
        {
            if (OnMouseRelease != null)
            {
                OnMouseRelease(sender, e, MouseCoordToDisplayCoord(new Vector2i(e.X, e.Y)));
            }
        }

        private Vector2f MouseCoordToDisplayCoord(Vector2i e)
        {
            var rawDisplayCoord = e - new Vector2i((int)Math.Round(Position.X), (int)Math.Round(Position.Y));
            rawDisplayCoord.X = (int)Math.Round(rawDisplayCoord.X * 1 / _targetSpr.Scale.X);
            rawDisplayCoord.Y = (int)Math.Round(rawDisplayCoord.Y * 1 / _targetSpr.Scale.Y);
            return Target.MapPixelToCoords(rawDisplayCoord);
        }

        /// <summary>
        /// Returns true if <paramref name="v"/> is inside this drawable object
        /// By default this does a AABB box test with the given display size
        /// Can be overriden for objects with transparency/particular shapes
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public virtual bool ContainsPoint(Vector2i v)
        {
            return (v.X >= Position.X && v.X <= Position.X + Target.Size.X &&
                    v.Y >= Position.Y && v.Y <= Position.Y + Target.Size.Y);
        }
        #endregion
    }
}
