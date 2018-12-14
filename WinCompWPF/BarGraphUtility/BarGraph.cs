using Windows.UI;
using Windows.UI.Composition;

using SharpDX;
using SharpDX.DirectWrite;
using SharpDX.Direct2D1;
using TextAntialiasMode = SharpDX.Direct2D1.TextAntialiasMode;
using SharpDX.DXGI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace BarGraphUtility
{
    class BarGraph
    {
        private float[] _graphData;

        private Compositor _compositor;
        private IntPtr _hwnd;

        private float _graphWidth, _graphHeight;
        private float _shapeGraphContainerHeight, _shapeGraphContainerWidth, _shapeGraphOffsetY, _shapeGraphOffsetX;
        private float _barWidth, _barSpacing;

        private GraphBarStyle _graphBarStyle;
        private List<Windows.UI.Color> _graphBarColors;

        //int key = position#; Bar value = Bar
        private Hashtable _barValueMap;   

        private WindowRenderTarget _textRenderTarget;
        private SolidColorBrush _textSceneColorBrush;
        private TextFormat _textFormatTitle;
        private TextFormat _textFormatHorizontal;
        private TextFormat _textFormatVertical;

        private static int _textRectWidth = 500;
        private static int _textRectHeight = 50;
        private static SharpDX.Mathematics.Interop.RawColor4 black = new SharpDX.Mathematics.Interop.RawColor4(0, 0, 0, 255);
        private static SharpDX.Mathematics.Interop.RawColor4 white = new SharpDX.Mathematics.Interop.RawColor4(255, 255, 255, 255);

        #region public setters
        public string Title { get; set; }
        public string XAxisLabel { get; set; }
        public string YAxisLabel { get; set; }
        public ContainerVisual BarRoot { get; }
        public ContainerVisual GraphRoot { get; }
        #endregion

        public enum GraphBarStyle
        {
            Single = 0,
            Random = 1,
            PerBarLinearGradient = 3,
            AmbientAnimatingPerBarLinearGradient = 4
        }

        /*
         * Constructor for bar graph. 
         * For now only does single bars, no grouping
         * As of 12/6 to insert graph, call the constructor then use barGraph.Root to get the container to parent
         */
        public BarGraph(Compositor compositor, IntPtr hwnd, string title, string xAxisLabel, string yAxisLabel, float width, float height, float[] data,//required parameters
            bool AnimationsOn = true, GraphBarStyle graphBarStyle = GraphBarStyle.Single, //optional parameters
            List<Windows.UI.Color> barColors = null)
        {
            _compositor = compositor;
            _hwnd = hwnd;
            this._graphWidth = width;
            this._graphHeight = height;
            this._graphData = data;

            Title = title;
            XAxisLabel = xAxisLabel;
            YAxisLabel = yAxisLabel;
            
            _graphBarStyle = graphBarStyle;

            if(barColors != null)
            {
                _graphBarColors = barColors;
            }
            else
            {
                _graphBarColors = new List<Windows.UI.Color>() { Colors.Blue };
            }

            // Configure options for text
            var Factory2D = new SharpDX.Direct2D1.Factory();

            HwndRenderTargetProperties properties = new HwndRenderTargetProperties();
            properties.Hwnd = _hwnd;
            properties.PixelSize = new SharpDX.Size2((int)_graphWidth, (int)_graphHeight);
            properties.PresentOptions = PresentOptions.None;

            _textRenderTarget = new WindowRenderTarget(Factory2D, new RenderTargetProperties(new PixelFormat(Format.Unknown, SharpDX.Direct2D1.AlphaMode.Premultiplied)), properties);

            // Generate graph structure
            var graphRoot = GenerateGraphStructure();
            GraphRoot = graphRoot;

            BarRoot = _compositor.CreateContainerVisual();
            GraphRoot.Children.InsertAtTop(BarRoot);

            //If data has been provided init bars and animations, else leave graph empty
            if (_graphData.Length > 0)
            {
                _barValueMap = new Hashtable();
                var bars = CreateBars(_graphData);
                AddBarsToTree(bars);
            }
        }

        private ContainerVisual GenerateGraphStructure()
        {
            ContainerVisual mainContainer = _compositor.CreateContainerVisual();
            mainContainer.Offset = new System.Numerics.Vector3(_shapeGraphOffsetX, _shapeGraphOffsetY, 0);

            _shapeGraphOffsetY = _graphHeight * 2 / 8;
            _shapeGraphOffsetX = _graphWidth * 2 / 8;
            _shapeGraphContainerHeight = _graphHeight - _shapeGraphOffsetY;
            _shapeGraphContainerWidth = _graphWidth - _shapeGraphOffsetX;

            _barWidth = ComputeBarWidth();
            _barSpacing = (float)(0.5 * _barWidth);


            // Create shape tree to hold 
            var shapeContainer = _compositor.CreateShapeVisual();
            shapeContainer.Offset = new System.Numerics.Vector3(_shapeGraphOffsetX, _shapeGraphOffsetY, 0);
            shapeContainer.Size = new System.Numerics.Vector2(_shapeGraphContainerWidth, _shapeGraphContainerHeight);  //leave some room for labels and title

            var xAxisLine = _compositor.CreateLineGeometry();
            xAxisLine.Start = new System.Numerics.Vector2(0, _shapeGraphContainerHeight);
            xAxisLine.End = new System.Numerics.Vector2(_shapeGraphContainerWidth, _shapeGraphContainerHeight);

            var xAxisShape = _compositor.CreateSpriteShape(xAxisLine);
            xAxisShape.StrokeBrush = _compositor.CreateColorBrush(Colors.Black);
            xAxisShape.FillBrush = _compositor.CreateColorBrush(Colors.Black);

            var yAxisLine = _compositor.CreateLineGeometry();
            yAxisLine.Start = new System.Numerics.Vector2(0, _shapeGraphContainerHeight);
            yAxisLine.End = new System.Numerics.Vector2(0, 0);

            var yAxisShape = _compositor.CreateSpriteShape(yAxisLine);
            yAxisShape.StrokeBrush = _compositor.CreateColorBrush(Colors.Blue);

            shapeContainer.Shapes.Add(xAxisShape);
            shapeContainer.Shapes.Add(yAxisShape);

            mainContainer.Children.InsertAtTop(shapeContainer);

            // Draw text
            DrawText(_textRenderTarget, Title, XAxisLabel, YAxisLabel, 20);

            // Return root node for graph
            return mainContainer;
        }

        public void DrawText(WindowRenderTarget renderTarget, string titleText, string xAxisText, string yAxisText, int baseTextSize)
        {
            var FactoryDWrite = new SharpDX.DirectWrite.Factory();

            _textFormatTitle = new TextFormat(FactoryDWrite, "Segoe", baseTextSize*5/4) {   TextAlignment = TextAlignment.Center, ParagraphAlignment = ParagraphAlignment.Center };
            _textFormatHorizontal = new TextFormat(FactoryDWrite, "Segoe", baseTextSize) { TextAlignment = TextAlignment.Center, ParagraphAlignment = ParagraphAlignment.Far };
            _textFormatVertical = new TextFormat(FactoryDWrite, "Segoe", baseTextSize) { ReadingDirection = ReadingDirection.TopToBottom, FlowDirection=FlowDirection.LeftToRight,
                TextAlignment = TextAlignment.Center, ParagraphAlignment = ParagraphAlignment.Near };

            renderTarget.AntialiasMode = AntialiasMode.PerPrimitive;
            renderTarget.TextAntialiasMode = TextAntialiasMode.Cleartype;

            _textSceneColorBrush = new SolidColorBrush(renderTarget, black);

            RectangleF ClientRectangleTitle = new RectangleF(0, 0, _textRectWidth, _textRectHeight);
            RectangleF ClientRectangleXAxis = new RectangleF(0, _shapeGraphContainerHeight- _textRectHeight, _textRectWidth, _textRectHeight);
            RectangleF ClientRectangleYAxis = new RectangleF(_shapeGraphOffsetX, _shapeGraphContainerHeight/2 + _textRectWidth/4, _textRectHeight, _textRectWidth);

            _textSceneColorBrush.Color = black;

            //Draw title and x axis text

            renderTarget.BeginDraw();

            renderTarget.Clear(white);
            renderTarget.DrawText(titleText, _textFormatTitle, ClientRectangleTitle, _textSceneColorBrush);
            renderTarget.DrawText(xAxisText, _textFormatHorizontal, ClientRectangleXAxis, _textSceneColorBrush);

            renderTarget.EndDraw();

            // Rotate render target to draw y axis text
            renderTarget.Transform = Matrix3x2.Rotation(-3.141f, new Vector2(100, _shapeGraphContainerHeight));

            renderTarget.BeginDraw();

            renderTarget.DrawText(yAxisText, _textFormatVertical, ClientRectangleYAxis, _textSceneColorBrush);

            renderTarget.EndDraw();

            // Rotate the RenderTarget back
            renderTarget.Transform = Matrix3x2.Identity;
        }

        /*
         * Dispose of resources
         */ 
        public void Dispose()
        {
            _textSceneColorBrush.Dispose();
            _textFormatTitle.Dispose();
            _textFormatHorizontal.Dispose();
            _textFormatVertical.Dispose();
        }

        private Bar[] CreateBars(float[] data)
        {
            //Clear hashmap 
            _barValueMap.Clear();


            // TODO break out into separate UpdateColors method?
            var barBrushHelper = new BarGraphUtility.BarBrushHelper(_compositor);
            CompositionBrush[] brushes = new CompositionBrush[data.Length];
            switch (_graphBarStyle)
            {
                case GraphBarStyle.Single:
                    brushes = barBrushHelper.GenerateSingleColorBrush(data.Length, _graphBarColors[0]);
                    break;
                case GraphBarStyle.Random:
                    brushes = barBrushHelper.GenerateRandomColorBrushes(data.Length);
                    break;
                case GraphBarStyle.PerBarLinearGradient:
                    brushes = barBrushHelper.GeneratePerBarLinearGradient(data.Length, _graphBarColors);
                    break;
                case GraphBarStyle.AmbientAnimatingPerBarLinearGradient:
                    brushes = barBrushHelper.GenerateAmbientAnimatingPerBarLinearGradient(data.Length, _graphBarColors);
                    break;
                default:
                    brushes = barBrushHelper.GenerateSingleColorBrush(data.Length, _graphBarColors[0]);
                    break;
            }
           
            var maxValue = GetMaxBarValue(data);
            var bars = new Bar[data.Length];
            for(int i=0; i<data.Length; i++)
            {
                var xOffset = _shapeGraphOffsetX + _barSpacing + (_barWidth + _barSpacing) * i;
                var height = GetAdjustedBarHeight(maxValue, _graphData[i]);

                var bar = new BarGraphUtility.Bar(_compositor, _shapeGraphContainerHeight, height, _barWidth, "something", _graphData[i], brushes[i]);
                bar.Root.Offset = new System.Numerics.Vector3(xOffset, _shapeGraphContainerHeight + _shapeGraphOffsetY , 0);

                _barValueMap.Add(i, bar);

                bars[i] = bar;
            }
            return bars;
        }

        private void AddBarsToTree(Bar[] bars)
        {
            BarRoot.Children.RemoveAll();
            for (int i = 0; i < bars.Length; i++)
            {
                BarRoot.Children.InsertAtTop(bars[i].Root);
            }
        }


        public void UpdateGraphData(string title, string xAxisTitle, string yAxisTitle, float[] newData)
        {
            // Update properties
            Title = title;
            XAxisLabel = xAxisTitle;
            YAxisLabel = yAxisTitle;

            // Update text
            DrawText(_textRenderTarget, Title, XAxisLabel, YAxisLabel, 20);

            // Generate bars 
            // If the same number of data points, update bars with new data. Else wipe and create new.
            if (_graphData.Length == newData.Length)
            {
                var maxValue = GetMaxBarValue(newData);
                for (int i=0; i< _graphData.Length; i++)
                {
                    // Animate bar height
                    var oldBar = (Bar)(_barValueMap[i]);
                    var newBarHeight = GetAdjustedBarHeight(maxValue, newData[i]);

                    // Update Bar
                    oldBar.Height = newBarHeight; //Trigger height animation
                    oldBar.Label = "something2";
                    oldBar.Value = newData[i];
                }
            }
            else
            {
                var bars = CreateBars(newData);
                AddBarsToTree(bars);
            }

            // Reset to new data
            _graphData = newData;
        }

        private float GetMaxBarValue(float[] data)
        {
            float max = data[0];
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] > max)
                {
                    max = data[i];
                }
            }
            return max;
        }

        /*
         * Adjust bar height relative to the max bar value
         */ 
        private float GetAdjustedBarHeight(float maxValue, float originalValue)
        {
            return _shapeGraphContainerHeight * (originalValue / maxValue);
        }

        /*
         * Return computed bar width for graph. Default spacing is 1/2 bar width.
         */
        private float ComputeBarWidth()
        {
            var spacingUnits = (_graphData.Length + 1) / 2;
            
            return (_shapeGraphContainerWidth / (_graphData.Length + spacingUnits));
        }

    }
}
