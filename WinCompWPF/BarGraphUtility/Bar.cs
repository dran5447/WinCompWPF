using System;
using System.Numerics;
using Windows.UI;
using Windows.UI.Composition;

namespace BarGraphUtility
{
    class Bar
    {
        private Compositor _compositor;
        private float _height;
        private CompositionRectangleGeometry rectGeometry;

        public CompositionBrush Brush { get; set; }
        public float Height {
            get { return _height; }
            set {
                _height = value;
                if (Root != null)
                {
                    Root.Size = new Vector2(value, Width);
                }
            }
        }
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

            rectGeometry = _compositor.CreateRectangleGeometry();
            rectGeometry.Size = new System.Numerics.Vector2(Height, Width);

            var barVisual = _compositor.CreateSpriteShape(rectGeometry);
            barVisual.FillBrush = Brush;

            shapeVisual.Shapes.Add(barVisual);

            Root = shapeVisual;

            // Add implict animation to bar
            var implicitAnimations = _compositor.CreateImplicitAnimationCollection();
            // Trigger animation when the size property changes. 
            implicitAnimations["Size"] = CreateAnimation();
            Root.ImplicitAnimations = implicitAnimations;
        }

        Vector2KeyFrameAnimation CreateAnimation()
        {
            Vector2KeyFrameAnimation animation = _compositor.CreateVector2KeyFrameAnimation();
            animation.InsertExpressionKeyFrame(0f, "this.StartingValue");
            animation.InsertExpressionKeyFrame(1f, "this.FinalValue");
            animation.Target = "Size";
            animation.Duration = TimeSpan.FromSeconds(3);
            return animation;
        }
    }
}
