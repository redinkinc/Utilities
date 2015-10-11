using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace Utilities
{
    public partial class DropdownControl
    {
        private Dropdown _dropdown;

        public DropdownControl(Dropdown dropdown)
        {
            _dropdown = dropdown;
            InitializeComponent();
        }

        public void AddItems(ICollection collection)
        {
            Box.ItemsSource = collection;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _dropdown.Index = Box.SelectedIndex;
        }

        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            if (Box.SelectedIndex > 0)
                Box.SelectedIndex = Box.SelectedIndex - 1;
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if (Box.SelectedIndex < Box.Items.Count - 1)
                Box.SelectedIndex = Box.SelectedIndex + 1;
        }
    }
}

