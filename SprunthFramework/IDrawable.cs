﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;

namespace SprunthFramework
{
    public interface IDrawable : Drawable
    {
        void Draw();
    }
}
