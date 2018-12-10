using System;
using Windows.UI;
using Windows.UI.Composition;

namespace BarGraphUtility
{
    public class BarBrushHelper
    {
        private Compositor _compositor;

        public BarBrushHelper(Compositor c)
        {
            _compositor = c;
        }

        public CompositionBrush GenerateRandomColorBrush()
        {
            Random rand = new Random();
            int max = byte.MaxValue + 1; // 256
            byte r = Convert.ToByte(rand.Next(max));
            byte g = Convert.ToByte(rand.Next(max));
            byte b = Convert.ToByte(rand.Next(max));
            Color c = Color.FromArgb(Convert.ToByte(256), r, g, b);

            return _compositor.CreateColorBrush(c);
        }

        //public CompositionBrush GenerateRandomLinearGradientBrush()
        //{

        //}


    }
}