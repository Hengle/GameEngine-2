﻿using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Windowing.Common;

namespace Toys
{
    [Flags]
    public enum InputContext
    { 
        Main = 1,
        Menu = 2,
        Action1 = 4,
        Action2 = 8
    }

    public class InputSystem
    {
        public InputContext CurrentContext { get; private set; }
        InputScheme currentScheme = null;

        public InputSystem()
        {
            CurrentContext = InputContext.Main;
        }

        public void SetInputScheme(InputScheme scheme)
        {
            if (currentScheme != null)
                currentScheme.ResetTriggers();

            CurrentContext = scheme.ContextType;
            currentScheme = scheme;
        }

        public void Update()
        {
            currentScheme?.UpdateTriggers();
        }
    }
}
