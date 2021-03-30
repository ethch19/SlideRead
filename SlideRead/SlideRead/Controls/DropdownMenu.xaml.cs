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
    public partial class DropdownMenu : ContentView
    {
        public Dictionary<string, DropdownSelectionButton> selectionControlsDictionary = new Dictionary<string, DropdownSelectionButton>();
        public Dictionary<string, Guid> selectionIDControlsDictionary = new Dictionary<string, Guid>();
        public DropdownMenu()
        {
            InitializeComponent();
        }
        public DropdownMenu(int selectionNumber)
        {
            InitializeComponent();
            if (selectionNumber <= 2)
            {
                return;
            }
            selectionNumber -= 2;
            Console.WriteLine(defineRow.Height.Value);
            int tempHeight = Int32.Parse(defineRow.Height.Value.ToString());
            for (int i=1; i<=selectionNumber; i++)
            {
                tempHeight += 30;
                baseImage.ScaleY += 0.35;
                var addingSelectionButton = new DropdownSelectionButton();
                defineRow.Height = new GridLength(tempHeight);
                stackSelectionGrid.Children.Add(addingSelectionButton);
                selectionControlsDictionary.Add(("ExtendedSelectionButton" + i.ToString()), addingSelectionButton);
                selectionIDControlsDictionary.Add(("ExtendedSelectionButton" + i.ToString()), addingSelectionButton.Id);
            }
        }
    }
}