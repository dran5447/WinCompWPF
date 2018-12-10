using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Composition;

namespace BarGraphUtility 
{
    class Bar
    {
        private Compositor _compositor;

        public CompositionBrush Brush { get; set; }
        public float Height { get; set; }
        public float Width { get; set; }
        public float Value { get; set; }
        public string Label { get; set; }

        public ShapeVisual Root { get; private set; }


        public Bar(Compositor compositor, float height, float width, string label, float value, CompositionBrush brush = null)
        {
            _compositor = compositor;

            Height = height;
            Width = width;
            Value = value;
            Label = label;

            if (brush == null)
            {
                brush = compositor.CreateColorBrush(Colors.Blue);
            }
            Brush = brush;

            CreateBar();
        }

        public void CreateBar()
        {
            var shapeVisual = _compositor.CreateShapeVisual();
            shapeVisual.Size = new System.Numerics.Vector2(Height, Width); //reverse width and height since rect will be at a 90* angle
            shapeVisual.RotationAngleInDegrees = -90f;

            var rectGeometry = _compositor.CreateRectangleGeometry();
            rectGeometry.Size = new System.Numerics.Vector2(Height, Width);

            var barVisual = _compositor.CreateSpriteShape(rectGeometry);
            barVisual.FillBrush = Brush;

            shapeVisual.Shapes.Add(barVisual);

            Root = shapeVisual;
        }

        public void AnimateIn()
        {
            ScalarKeyFrameAnimation heightAnimation = _compositor.CreateScalarKeyFrameAnimation();
            heightAnimation.Duration = new TimeSpan(0, 0, 4);
            heightAnimation.InsertKeyFrame(0.0f, 0f);
            heightAnimation.InsertKeyFrame(1.0f, Height);
            Root.StartAnimation("Size.X", heightAnimation);

            //TODO optionally easing functions to add to the insertkeyframe overload
        }

        //TODO changed events
    }
}
