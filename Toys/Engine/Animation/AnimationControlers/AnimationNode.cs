﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toys
{
    public class AnimationNode
    {
        public Animation MainAnimation;

        public AnimationNode NextAnimation;

        public bool Repeat;
        public float Speed;

        public List<AnimationTransition> Transitions;
    }
}