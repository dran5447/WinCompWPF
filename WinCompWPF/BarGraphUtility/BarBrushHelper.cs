using System;
using Windows.UI;
using Windows.UI.Composition;

namespace BarGraphUtility
{
    /*
     * Create brushes to fill the bars
     */ 
    public class BarBrushHelper
    {
        private Compositor _compositor;
        private Random rand;

        public BarBrushHelper(Compositor c)
        {
            _compositor = c;
            rand = new Random();
        }

        internal CompositionBrush[] GenerateSingleRandomColorBrush(int numBrushes)
        {            
            int max = byte.MaxValue + 1; // 256
            byte r = Convert.ToByte(rand.Next(max));
            byte g = Convert.ToByte(rand.Next(max));
            byte b = Convert.ToByte(rand.Next(max));
            Color c = Color.FromArgb(Convert.ToByte(255), r, g, b);

            CompositionBrush[] brushes = new CompositionBrush[numBrushes];
            for(int i=0; i<numBrushes; i++)
            {
                brushes[i] = _compositor.CreateColorBrush(c);
            }

            return brushes;
        }

        internal CompositionBrush[] GenerateRandomColorBrushes(int numBrushes)
        {
            int max = byte.MaxValue + 1; // 256
            CompositionBrush[] brushes = new CompositionBrush[numBrushes];
            for (int i = 0; i < numBrushes; i++)
            {
                byte r = Convert.ToByte(rand.Next(max));
                byte g = Convert.ToByte(rand.Next(max));
                byte b = Convert.ToByte(rand.Next(max));
                Color c = Color.FromArgb(Convert.ToByte(255), r, g, b);
                brushes[i] = _compositor.CreateColorBrush(c);
            }

            return brushes;
        }

        internal CompositionBrush[] GeneratePerBarLinearGradient(int numBrushes)
        {
            var lgb = _compositor.CreateLinearGradientBrush();
            lgb.RotationAngleInDegrees = 45;

            var colorStop1 = _compositor.CreateColorGradientStop(0.0f, Colors.Red);
            var colorStop2 = _compositor.CreateColorGradientStop(0.5f, Colors.Orange);
            var colorStop3 = _compositor.CreateColorGradientStop(1.0f, Colors.Yellow);

            lgb.ColorStops.Add(colorStop1);
            lgb.ColorStops.Add(colorStop2);
            lgb.ColorStops.Add(colorStop3);

            CompositionBrush[] brushes = new CompositionBrush[numBrushes];
            for (int i = 0; i < numBrushes; i++)
            {
                brushes[i] = lgb;
            }
            return brushes;
        }

        internal CompositionBrush[] GenerateSharedLinearGradient(int length)
        {
            throw new NotImplementedException();
        }

        internal CompositionBrush[] GenerateTintedBlur(int length)
        {
            throw new NotImplementedException();
        }
    }
}