using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Composition;



namespace BarGraphUtility
{
    class BarGraph
    {
        private float[] data = new float[] { }; //TODO remove later

        private Compositor _compositor;
        private float width, height;
        private float _shapeGraphContainerHeight, _shapeGraphContainerWidth, _shapeGraphOffsetY, _shapeGraphOffsetX;

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
         * As of 12/6 to insert graph, call the constructor then use barGraph.Root to get the container to parent
         */
        public BarGraph(Compositor compositor, string title, string xAxisLabel, string yAxisLabel, float width, float height,//required parameters
            bool AnimationsOn = true, GraphOrientation orientation = GraphOrientation.Vertical) //optional parameters
        {
            _compositor = compositor;
            this.width = width;
            this.height = height;

            Title = title;
            XAxisLabel = xAxisLabel;
            YAxisLabel = yAxisLabel;

            Orientation = orientation;

            //TODO use dwrite to render text
            //TODO data parameter -> call method to create bars?
            //TODO color configurations; preset options or custom
            //TODO legend
            //TODO trend line on top of bars
            //TODO image on top of bars
            //TODO meaningful orientation
            //TODO on orientation changed reset graph 

            // Generate graph structure
            var graphRoot = GenerateGraphStructure();
            Root = graphRoot;

            //TODO if data has been provided, init bars and animations
            AddBars(data);
            //if (data.Length>0)
            //{
            //    AddBars(data);
            //}
        }

        private void AddBars(float[] data)
        {
            //TODO Based on data set size, compute the X offset and size of each bar
            var barSpacing = 10f;
            var barWidth = 20f;

            //TODO iterate through data. For each piece of data do the following:
            var dataValue = 30;
            var computedHeight = 50;

            var bar1 = new BarGraphUtility.Bar(_compositor, computedHeight, barWidth, "something", dataValue);  //TODO replace with actual data

            //TODO compute x offset appropriately
            bar1.Root.Offset = new System.Numerics.Vector3(_shapeGraphOffsetX + barSpacing * 1, _shapeGraphContainerHeight + _shapeGraphOffsetY, 0);

            Root.Children.InsertAtTop(bar1.Root);

            bar1.AnimateIn();

            var bar2 = new Bar(_compositor, computedHeight * 2, barWidth, "something", dataValue, _compositor.CreateColorBrush(Colors.Green));  //TODO replace with actual data
            //TODO compute x offset appropriately
            bar2.Root.Offset = new System.Numerics.Vector3(_shapeGraphOffsetX + barSpacing * 2 + barWidth, _shapeGraphContainerHeight + _shapeGraphOffsetY, 0);
            Root.Children.InsertAtTop(bar2.Root);

            bar2.AnimateIn();
        }

        private ContainerVisual GenerateGraphStructure()
        {
            ContainerVisual mainContainer = _compositor.CreateContainerVisual();
            mainContainer.Offset = new System.Numerics.Vector3(_shapeGraphOffsetX, _shapeGraphOffsetY, 0);

            //TODO use dwrite to render title & x/y labels
            _shapeGraphOffsetY = height * 2 / 8;
            _shapeGraphOffsetX = width * 2 / 8;
            _shapeGraphContainerHeight = height - _shapeGraphOffsetY;
            _shapeGraphContainerWidth = width - _shapeGraphOffsetX;

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





    }
}
