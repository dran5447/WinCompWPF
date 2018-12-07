using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WinCompWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Application app;
        Window myWindow;

        private ControlHost listControl;

        private Random random = new Random();
        private string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        private string[] customerFirstNames = new string[] { "Angel", "Josephine", "Wade", "Christie", "Whitney", "Ismael", "Alexandra", "Rhonda", "Dawn", "Roman", "Emanuel", "Evan", "Aaron", "Bill", "Margaret", "Mandy", "Carlton", "Cornelius", "Cora", "Alejandro", "Annette", "Bertha", "John", "Billy", "Randall" };
        private string[] customerLastNames = new string[] { "Murphy", "Swanson", "Sandoval", "Moore", "Adkins", "Tucker", "Cook", "Fernandez", "Schwartz", "Sharp", "Bryant", "Gross", "Spencer", "Powers", "Hunter", "Moreno", "Baldwin", "Stewart", "Rice", "Watkins", "Hawkins", "Dean", "Howard", "Bailey", "Gill" };

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            app = System.Windows.Application.Current;
            myWindow = app.MainWindow;
            myWindow.SizeToContent = SizeToContent.WidthAndHeight;

            List<Customer> customers = new List<Customer>();
            for (int i = 0; i < customerFirstNames.Length; i++)
            {
                string id = new string(Enumerable.Repeat(chars, 12).Select(s => s[random.Next(s.Length)]).ToArray());
                customers.Add(new Customer(id, customerFirstNames[i], customerLastNames[random.Next(0, customerLastNames.Length - 1)], RandomDay(), random.NextDouble() >= 0.5));
            }

            CustomerGrid.ItemsSource = customers;


            //TODO Update with actual cohesive control
            listControl = new ControlHost(ControlHostElement.ActualHeight, ControlHostElement.ActualWidth);
            ControlHostElement.Child = listControl;
        }
        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //TODO send info

            listControl.UpdateGraph();


        }

        private DateTime RandomDay()
        {
            DateTime start = new DateTime(1995, 1, 1);
            int range = (DateTime.Today - start).Days;
            return start.AddDays(random.Next(range));
        }

        //private IntPtr ControlMsgFilter(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        //{
        //    //int textLength;

        //    //handled = false;
        //    //if (msg == WM_COMMAND)
        //    //{
        //    //    switch ((uint)wParam.ToInt32() >> 16 & 0xFFFF) //extract the HIWORD
        //    //    {
        //    //        case LBN_SELCHANGE: //Get the item text and display it
        //    //            selectedItem = SendMessage(listControl.hwndListBox, LB_GETCURSEL, IntPtr.Zero, IntPtr.Zero);
        //    //            textLength = SendMessage(listControl.hwndListBox, LB_GETTEXTLEN, IntPtr.Zero, IntPtr.Zero);
        //    //            StringBuilder itemText = new StringBuilder();
        //    //            SendMessage(hwndListBox, LB_GETTEXT, selectedItem, itemText);
        //    //            selectedText.Text = itemText.ToString();
        //    //            handled = true;
        //    //            break;
        //    //    }
        //    //}
        //    return IntPtr.Zero;
        //}

        //internal const int
        //  LBN_SELCHANGE = 0x00000001,
        //  WM_COMMAND = 0x00000111,
        //  LB_GETCURSEL = 0x00000188,
        //  LB_GETTEXTLEN = 0x0000018A,
        //  LB_ADDSTRING = 0x00000180,
        //  LB_GETTEXT = 0x00000189,
        //  LB_DELETESTRING = 0x00000182,
        //  LB_GETCOUNT = 0x0000018B;

        //private void AppendText(object sender, EventArgs args)
        //{
        //    //if (txtAppend.Text != string.Empty)
        //    //{
        //    //    SendMessage(hwndListBox, LB_ADDSTRING, IntPtr.Zero, txtAppend.Text);
        //    //}
        //    //itemCount = SendMessage(hwndListBox, LB_GETCOUNT, IntPtr.Zero, IntPtr.Zero);
        //    //numItems.Text = "" + itemCount.ToString();
        //}
        //private void DeleteText(object sender, EventArgs args)
        //{
        //    //selectedItem = SendMessage(listControl.hwndListBox, LB_GETCURSEL, IntPtr.Zero, IntPtr.Zero);
        //    //if (selectedItem != -1) //check for selected item
        //    //{
        //    //    SendMessage(hwndListBox, LB_DELETESTRING, (IntPtr)selectedItem, IntPtr.Zero);
        //    //}
        //    //itemCount = SendMessage(hwndListBox, LB_GETCOUNT, IntPtr.Zero, IntPtr.Zero);
        //    //numItems.Text = "" + itemCount.ToString();
        //}



        //[DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = CharSet.Unicode)]
        //internal static extern int SendMessage(IntPtr hwnd,
        //                                       int msg,
        //                                       IntPtr wParam,
        //                                       IntPtr lParam);

        //[DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = CharSet.Unicode)]
        //internal static extern int SendMessage(IntPtr hwnd,
        //                                       int msg,
        //                                       int wParam,
        //                                       [MarshalAs(UnmanagedType.LPWStr)] StringBuilder lParam);

        //[DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = CharSet.Unicode)]
        //internal static extern IntPtr SendMessage(IntPtr hwnd,
        //                                          int msg,
        //                                          IntPtr wParam,
        //                                          String lParam);
    }
}
