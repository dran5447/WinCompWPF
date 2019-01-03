using System;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using Windows.UI.Composition;

using BarGraphUtility;
using System.Collections.Generic;
using Windows.UI;
using System.Windows;
using System.Windows.Controls;
using WinCompWPF;
using Windows.UI.Xaml.Hosting;

namespace WinCompWPFWinForms
{
    /// <summary>
    /// Interaction logic for HostControl.xaml
    /// </summary>
    public class HostControl
    {
        private HwndHostControl listControl;
        private Compositor c;
        private BarGraph _currentGraph;

        public HostControl()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            listControl = new HwndHostControl(ControlHostElement.Width, ControlHostElement.Height);
            ControlHostElement.Child = listControl;
        }

        /*
         * Handle Composition tree creation and updates
         */
        //TODO move all graph logic out of this control
        //public void UpdateGraph(Customer customer)
        //{
        //    var graphTitle = customer.FirstName + " Investment History";
        //    var xAxisTitle = "Investment #";
        //    var yAxisTitle = "# Shares of Stock";

        //    // If graph already exists update values. Else create new graph.
        //    if (mainContainer.Children.Count > 0 && _currentGraph != null)
        //    {
        //        _currentGraph.UpdateGraphData(graphTitle, xAxisTitle, yAxisTitle, customer.Data);
        //    }
        //    else
        //    {
        //        BarGraph graph = new BarGraph(c, hwndHost, graphTitle, xAxisTitle, yAxisTitle,
        //            hostWidth, hostHeight, customer.Data,
        //            true, BarGraph.GraphBarStyle.AmbientAnimatingPerBarLinearGradient,
        //            new List<Color> { Colors.DarkBlue, Colors.BlueViolet, Colors.LightSkyBlue, Colors.White });

        //        _currentGraph = graph;
        //        mainContainer.Children.InsertAtTop(graph.GraphRoot);
        //    }
        //}

        #region DependencyProperty Data

        /// <summary>
        /// Registers a dependency property as backing store for the Content property
        /// </summary>
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(object), typeof(HostControl),
            new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets or sets the Data.
        /// </summary>
        /// <value>The Data.</value>
        public object Data
        {
            get { return (object)GetValue(DataProperty); }
            set {
                //TODO check if graph is null. if so, create and parent graph else call to update

                SetValue(DataProperty, value);
                //TODO update graph
            }
        }

        #endregion
    }
}
