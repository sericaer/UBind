﻿using System;

namespace Aya.DataBinding
{
    public abstract class DataBinder<TTarget, TData> : DataBinder<TData>
    {
        public TTarget Target;

        public virtual Type TargetType { get; internal set; } = typeof(TTarget);

        public override void UpdateSource()
        {
            if(Target == null)
            {
                return;
            }

            base.UpdateSource();
        }
    }
}