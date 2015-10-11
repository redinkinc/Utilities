using System.Windows.Input;

namespace Utilities
{
    public partial class BoolButtonControl
    {
        private readonly BoolButton _boolButton;

        public BoolButtonControl(BoolButton model)
        {
            InitializeComponent();
            _boolButton = model;
        }

        public void Display(bool state)
        {
            BoolButton.Content = state;
        }

        private void BoolButton_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            _boolButton.Value = true;
        }

        private void BoolButton_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            _boolButton.Value = false;
        }
    }

}
