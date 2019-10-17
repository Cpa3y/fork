using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Fork.Gui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CollectionViewSource Source => (CollectionViewSource)this.FindResource("uiSource");

        private bool doUpdateState;




        public MainWindow()
        {
            InitializeComponent();
        }





        protected async override void OnStateChanged(EventArgs e)
        {
            base.OnStateChanged(e);
            switch (this.WindowState)
            {
                case WindowState.Normal:
                    if (!doUpdateState)
                    {
                        doUpdateState = true;
                        await UpdateState();
                    }
                    break;

                case WindowState.Minimized:
                    doUpdateState = false;
                    break;

            }

        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            doUpdateState = false;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            doUpdateState = true;

            await UpdateState();
        }

        private async Task UpdateState()
        {
            Source.Source = ForkHelper.GetStatus();
            while (doUpdateState)
            {
                await Task.Delay(1000);
                ForkHelper.UpdateState();
                Debug.WriteLine("Updated");
            }
        }
    }
}
