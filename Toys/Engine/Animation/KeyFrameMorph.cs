﻿using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Mathematics;

namespace Toys
{
    public class KeyFrameMorph
    {
        public float FrameId;
        public float Value;
        public string MorphName;
        public BezierCurveCubic bezierCurve;
        public bool InterpolateCurve = false;
    }
}
