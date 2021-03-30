using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SlideRead.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ScrollList : ContentView
    {
        public ScrollList()
        {
            InitializeComponent();
        }
        private void KeyChangeClicked(object sender, EventArgs args)
        {
            Button btn = (Button)sender;
            if (btn.IsEnabled == false) return;
            btn.IsEnabled = false;
            CurrentItemLabel.Text = btn.Text;
            btn.IsEnabled = true;
        }
    }
}