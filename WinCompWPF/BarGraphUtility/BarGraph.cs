using Windows.UI;
using Windows.UI.Composition;

using SharpDX;
using SharpDX.DirectWrite;
using SharpDX.Direct2D1;
using TextAntialiasMode = SharpDX.Direct2D1.TextAntialiasMode;
using SharpDX.DXGI;
using System;
using System.Collections;
using System.Diagnostics;

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

        //int key = position#; Bar value = Bar
        private Hashtable barValueMap;   

        private WindowRenderTarget _titleRenderTarget;
        private WindowRenderTarget _xAxisRenderTarget;
        private WindowRenderTarget _yAxisRenderTarget;

        #region public setters
        public string Title { get; set; }
        public string XAxisLabel { get; set; }
        public string YAxisLabel { get; set; }
        public GraphOrientation Orientation { get; set; }
        public GraphBarColorOptions GraphColor { get; set; }
        public ContainerVisual BarRoot { get; }
        public ContainerVisual GraphRoot { get; }
        #endregion

        //TODO make meaningful
        public enum GraphOrientation
        {
            Vertical = 0,
            Horizontal = 1
        }

        //TODO make meaningful
        public enum GraphBarColorOptions
        {
            SingleRandom = 0,
            Random = 1,
            PerBarLinearGradient = 3,
            SharedLinearGradient = 4,
            TintedBlur = 5
        }

        /*
         * Constructor for bar graph. 
         * For now only does single bars, no grouping
         * As of 12/6 to insert graph, call the constructor then use barGraph.Root to get the container to parent
         */
        public BarGraph(Compositor compositor, IntPtr hwnd, string title, string xAxisLabel, string yAxisLabel, float width, float height, float[] data,//required parameters
            bool AnimationsOn = true, GraphOrientation orientation = GraphOrientation.Vertical, GraphBarColorOptions colorOptions = GraphBarColorOptions.SingleRandom) //optional parameters
        {
            _compositor = compositor;
            _hwnd = hwnd;
            this._graphWidth = width;
            this._graphHeight = height;
            this._graphData = data;

            Title = title;
            XAxisLabel = xAxisLabel;
            YAxisLabel = yAxisLabel;

            Orientation = orientation;
            GraphColor = colorOptions;

            // Configure options for text
            var Factory2D = new SharpDX.Direct2D1.Factory();

            HwndRenderTargetProperties properties = new HwndRenderTargetProperties();
            properties.Hwnd = _hwnd;
            properties.PixelSize = new SharpDX.Size2((int)_graphWidth, (int)_graphHeight);
            properties.PresentOptions = PresentOptions.None;

            _titleRenderTarget = new WindowRenderTarget(Factory2D, new RenderTargetProperties(new PixelFormat(Format.Unknown, SharpDX.Direct2D1.AlphaMode.Premultiplied)), properties);
            _xAxisRenderTarget = new WindowRenderTarget(Factory2D, new RenderTargetProperties(new PixelFormat(Format.Unknown, SharpDX.Direct2D1.AlphaMode.Premultiplied)), properties);
            _yAxisRenderTarget = new WindowRenderTarget(Factory2D, new RenderTargetProperties(new PixelFormat(Format.Unknown, SharpDX.Direct2D1.AlphaMode.Premultiplied)), properties);


            //TODO legend
            //TODO trend line on top of bars
            //TODO surface or value on top of bars?
            //TODO meaningful orientation options
            //TODO option for data sets (more than one bar in a set, multiple sets per graph)

            // Generate graph structure
            var graphRoot = GenerateGraphStructure();
            GraphRoot = graphRoot;

            BarRoot = _compositor.CreateContainerVisual();
            GraphRoot.Children.InsertAtTop(BarRoot);

            //If data has been provided init bars and animations, else leave graph empty
            //TODO add ability to add data later on (move out of constructor)
            //TODO add ability to either create individual bars or barsets for the graph
            if (_graphData.Length > 0)
            {
                barValueMap = new Hashtable();
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
            DrawText(_titleRenderTarget, Title, 20, 0);
   //         DrawText(_xAxisRenderTarget, XAxisLabel, 20, 270);  //TODO add back in later
   //         DrawText(_yAxisRenderTarget, YAxisLabel, 20, 0);

            // Return root node for graph
            return mainContainer;
        }

        public void DrawText(WindowRenderTarget renderTarget, string text, int textSize, int rotationInDegrees)
        {
            //TODO add rotation bit
            
            var FactoryDWrite = new SharpDX.DirectWrite.Factory();
            var TextFormat = new TextFormat(FactoryDWrite, "Segoe", textSize) { TextAlignment = TextAlignment.Center, ParagraphAlignment = ParagraphAlignment.Center };

            renderTarget.AntialiasMode = AntialiasMode.PerPrimitive;
            renderTarget.TextAntialiasMode = TextAntialiasMode.Cleartype;

            SharpDX.Mathematics.Interop.RawColor4 black = new SharpDX.Mathematics.Interop.RawColor4(0, 0, 0, 255);
            SharpDX.Mathematics.Interop.RawColor4 white = new SharpDX.Mathematics.Interop.RawColor4(255, 255, 255, 255);

            var SceneColorBrush = new SolidColorBrush(renderTarget, black);

            RectangleF ClientRectangle = new RectangleF(0, 0, _graphWidth, _graphHeight);

            SceneColorBrush.Color = black;

            renderTarget.BeginDraw();

            renderTarget.Clear(white);
            renderTarget.DrawText(text, TextFormat, ClientRectangle, SceneColorBrush);
            renderTarget.EndDraw();
        }

        private Bar[] CreateBars(float[] data)
        {
            //Clear hashmap 
            barValueMap.Clear();


            // TODO break out into separate UpdateColors method?
            var barBrushHelper = new BarGraphUtility.BarBrushHelper(_compositor);
            CompositionBrush[] brushes = new CompositionBrush[data.Length];
            switch (GraphColor)
            {
                case GraphBarColorOptions.SingleRandom:
                    brushes = barBrushHelper.GenerateSingleRandomColorBrush(data.Length);
                    break;
                case GraphBarColorOptions.Random:
                    brushes = barBrushHelper.GenerateRandomColorBrushes(data.Length);
                    break;
                case GraphBarColorOptions.PerBarLinearGradient:
                    brushes = barBrushHelper.GeneratePerBarLinearGradient(data.Length);
                    break;
                case GraphBarColorOptions.SharedLinearGradient:
                    brushes = barBrushHelper.GenerateSharedLinearGradient(data.Length);
                    break;
                case GraphBarColorOptions.TintedBlur:
                    brushes = barBrushHelper.GenerateTintedBlur(data.Length);
                    break;
                default:
                    brushes = barBrushHelper.GenerateSingleRandomColorBrush(data.Length);
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

                barValueMap.Add(i, bar);

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
            //TODO add property changed notifiers which will automatically trigger redraw instead of calling manually
            Title = title;
            XAxisLabel = xAxisTitle;
            YAxisLabel = yAxisTitle;


            // Update text
            DrawText(_titleRenderTarget, Title, 20, 0);
            //TODO update other axes

            // Generate bars 
            // If the same number of data points, update bars with new data. Else wipe and create new.
            if (_graphData.Length == newData.Length)
            {
                var maxValue = GetMaxBarValue(newData);
                for (int i=0; i< _graphData.Length; i++)
                {
                    // Animate bar height
                    var oldBar = (Bar)(barValueMap[i]);
                    var newBarHeight = GetAdjustedBarHeight(maxValue, newData[i]);

                    // Update Bar
                    oldBar.Height = newBarHeight; //Trigger height animation
                    oldBar.Label = "something2"; //TODO update
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
         * TODO add option for min width or min spacing
         */
        private float ComputeBarWidth()
        {
            var spacingUnits = (_graphData.Length + 1) / 2;
            
            return (_shapeGraphContainerWidth / (_graphData.Length + spacingUnits));
        }

    }
}
