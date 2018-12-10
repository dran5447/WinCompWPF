using Windows.UI;
using Windows.UI.Composition;



namespace BarGraphUtility
{
    class BarGraph
    {
        private float[] _graphData;

        private Compositor _compositor;
        private float _graphWidth, _graphHeight;
        private float _shapeGraphContainerHeight, _shapeGraphContainerWidth, _shapeGraphOffsetY, _shapeGraphOffsetX;
        private float _barWidth, _barSpacing;

        #region public setters
        public string Title { get; set; }
        public string XAxisLabel { get; set; }
        public string YAxisLabel { get; set; }
        public GraphOrientation Orientation { get; set; }
        public ContainerVisual Root { get; }
        #endregion

        public enum GraphOrientation
        {
            Vertical = 0,
            Horizontal = 1
        }

        /*
         * Constructor for bar graph. 
         * For now only does single bars, no grouping
         * As of 12/6 to insert graph, call the constructor then use barGraph.Root to get the container to parent
         */
        public BarGraph(Compositor compositor, string title, string xAxisLabel, string yAxisLabel, float width, float height, float[] data,//required parameters
            bool AnimationsOn = true, GraphOrientation orientation = GraphOrientation.Vertical) //optional parameters
        {
            _compositor = compositor;
            this._graphWidth = width;
            this._graphHeight = height;
            this._graphData = data;

            Title = title;
            XAxisLabel = xAxisLabel;
            YAxisLabel = yAxisLabel;

            Orientation = orientation;

            //TODO legend
            //TODO trend line on top of bars
            //TODO surface or value on top of bars?
            //TODO meaningful orientation options
            //TODO option for data sets (more than one bar in a set, multiple sets per graph)

            // Generate graph structure
            var graphRoot = GenerateGraphStructure();
            Root = graphRoot;

            //If data has been provided init bars and animations, else leave graph empty
            if (_graphData.Length > 0)
            {
                AddBars();
            }
        }


        private ContainerVisual GenerateGraphStructure()
        {
            ContainerVisual mainContainer = _compositor.CreateContainerVisual();
            mainContainer.Offset = new System.Numerics.Vector3(_shapeGraphOffsetX, _shapeGraphOffsetY, 0);

            //TODO use dwrite to render title & x/y labels

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
            xAxisLine.End = new System.Numerics.Vector2(_shapeGraphContainerHeight, _shapeGraphContainerWidth);

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

            // Return root node for graph
            return mainContainer;
        }

        private void AddBars()
        {
            var maxValue = GetMaxBarValue();
            var barBrushHelper = new BarGraphUtility.BarBrushHelper(_compositor);

            for(int i=0; i<_graphData.Length; i++)
            {
                var xOffset = _shapeGraphOffsetX + _barSpacing + (_barWidth + _barSpacing) * i;
                var height = GetAdjustedBarHeight(maxValue, _graphData[i]);

                var bar = new BarGraphUtility.Bar(_compositor, height, _barWidth, "something", _graphData[i]);
                bar.Root.Offset = new System.Numerics.Vector3(xOffset, _shapeGraphContainerHeight + _shapeGraphOffsetY, 0);
                Root.Children.InsertAtTop(bar.Root);
                bar.AnimateIn();
            }
        }


        private float GetMaxBarValue()
        {
            float max = _graphData[0];
            for (int i = 0; i < _graphData.Length; i++)
            {
                if (_graphData[i] > max)
                {
                    max = _graphData[i];
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
