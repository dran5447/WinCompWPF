using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using BarGraphUtility;
using NativeHelpers;
using Windows.UI;
using Windows.UI.Composition;

namespace WinCompWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : PerMonitorDPIWindow
    {
        Application app;
        Window myWindow;

        private Random random = new Random();
        private string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        private string[] customerFirstNames = new string[] { "Angel", "Josephine", "Wade", "Christie", "Whitney", "Ismael", "Alexandra", "Rhonda", "Dawn", "Roman", "Emanuel", "Evan", "Aaron", "Bill", "Margaret", "Mandy", "Carlton", "Cornelius", "Cora", "Alejandro", "Annette", "Bertha", "John", "Billy", "Randall" };
        private string[] customerLastNames = new string[] { "Murphy", "Swanson", "Sandoval", "Moore", "Adkins", "Tucker", "Cook", "Fernandez", "Schwartz", "Sharp", "Bryant", "Gross", "Spencer", "Powers", "Hunter", "Moreno", "Baldwin", "Stewart", "Rice", "Watkins", "Hawkins", "Dean", "Howard", "Bailey", "Gill" };

        private HwndHostControl hostControl;
        private BarGraph currentGraph;
        private ContainerVisual graphContainer;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void PerMonitorDPIWindow_DPIChanged(object sender, EventArgs e)
        {
            if(this.ActualWidth > 0)
            {
                currentGraph.UpdateDPI(this.CurrentDPI, ControlHostElement.Width, ControlHostElement.Height);
            }
        }

        /*
         * Generate customers, pass data to grid, and create host control
         */
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            app = System.Windows.Application.Current;
            myWindow = app.MainWindow;

            List<Customer> customers = new List<Customer>();
            for (int i = 0; i < customerFirstNames.Length; i++)
            {
                string id = new string(Enumerable.Repeat(chars, 12).Select(s => s[random.Next(s.Length)]).ToArray());
                customers.Add(new Customer(id, customerFirstNames[i], customerLastNames[random.Next(0, customerLastNames.Length - 1)], GenerateRandomDay(), random.NextDouble() >= 0.5, GenerateRandomData()));
            }

            CustomerGrid.ItemsSource = customers;

            hostControl = new HwndHostControl(ControlHostElement.Width, ControlHostElement.Height, this.CurrentDPI);

            ControlHostElement.Child = hostControl;
            graphContainer = hostControl.Compositor.CreateContainerVisual();
            hostControl.Child = graphContainer;
        }

        /*
         * Handle Composition tree creation and updates
         */
        public void UpdateGraph(Customer customer)
        {
            var graphTitle = customer.FirstName + " Investment History";
            var xAxisTitle = "Investment #";
            var yAxisTitle = "# Shares of Stock";

            // If graph already exists update values. Else create new graph.
            if (graphContainer.Children.Count > 0 && currentGraph != null)
            {
                currentGraph.UpdateGraphData(graphTitle, xAxisTitle, yAxisTitle, customer.Data);
            }
            else
            {
                BarGraph graph = new BarGraph(hostControl.Compositor, hostControl.hwndHost, graphTitle, xAxisTitle, yAxisTitle,
                    (float)ControlHostElement.Width, (float)ControlHostElement.Height, this.CurrentDPI, customer.Data,
                    true, BarGraph.GraphBarStyle.AmbientAnimatingPerBarLinearGradient,
                    new List<Color> { Colors.DarkBlue, Colors.BlueViolet, Colors.LightSkyBlue, Colors.White });

                currentGraph = graph;
                graphContainer.Children.InsertAtTop(graph.GraphRoot);
            }
        }

        /*
         * Send customer info to the control on row select
         */
        private void CustomerGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            UpdateGraph((Customer)CustomerGrid.SelectedItem);
        }

        /*
         * Generate random customer data
         */
        private float[] GenerateRandomData()
        {
            var numDataPoints = 6;//random.Next(2, 8);
            float[] data = new float[numDataPoints];

            for (int j = 0; j < numDataPoints; j++)
            {
                data[j] = random.Next(50, 300);
            }

            return data;
        }

        /*
         * Generate random date to use for the customer info
         */
        private DateTime GenerateRandomDay()
        {
            DateTime start = new DateTime(1995, 1, 1);
            int range = (DateTime.Today - start).Days;
            return start.AddDays(random.Next(range));
        }

    }
}
