// <copyright file="MeasureSpecFactory.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo.Droid
{
    using Android.Views;

    internal static class MeasureSpecFactory
    {
        public static int GetSize(int measureSpec)
        {
            const int modeMask = 0x3 << 30;
            return measureSpec & ~modeMask;
        }

        public static int MakeMeasureSpec(int size, MeasureSpecMode mode)
        {
            return size + (int)mode;
        }
    }
}