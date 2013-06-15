﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Candyland
{
    /// <summary>
    /// Basic Class from which all GameObjects, UI-Elements and the Camera are derived
    /// </summary>
    public abstract class GameElement
    {
        public abstract void initialize();
        public abstract void update();
    }
}