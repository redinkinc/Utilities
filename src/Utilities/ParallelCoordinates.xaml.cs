using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Utilities
{
    /// <summary>
    /// Interaction logic for ParallelCoordinates.xaml
    /// </summary>
    public partial class ParallelCoordinatesControl
    {
        public double CanvasWidth { get; private set; }

        private Polyline _pl;

        private static readonly SolidColorBrush BgrColorBrush = new SolidColorBrush() { Color = Color.FromArgb(255, 229, 227, 223) };

        public ParallelCoordinates Model { get; set; }
        public SolidColorBrush PlotColor { get; set; }
  
        public int NumOfCoordinates { get; set; }

        public ParallelCoordinatesControl(ParallelCoordinates model)
        {
            InitializeComponent();
            Model = model;
            PlotColor = new SolidColorBrush() { Color = Color.FromArgb(255, 0, 0, 0) };
            PlotColorBox.ItemsSource = typeof(Colors).GetProperties();
            PlotColorBox.SelectedItem = typeof(Colors).GetProperty("Black");
        }

        public void AddChart(ParallelCoordinateData pcData)
        {
            PlotCanvas.Children.Clear();
            DrawPlot(pcData);
        }

        private void DrawPlot(ParallelCoordinateData pcData)
        {
            
            if ( pcData.ParameterList.Count == 0 )
            {
                for (var i = 0; i < Model.Values[1].Count; i++)
                {
                    Model.ParameterNames.Add("default" + i);
                }
            }

            NumOfCoordinates = Model.ParameterNames.Count;
            CanvasWidth = 30*NumOfCoordinates;
            
            for (var i = 0; i < NumOfCoordinates; i++)
            {
                var pl = new Polyline() {Stroke = Brushes.Black};
                pl.Points.Add(new Point(i * 30, 300));
                pl.Points.Add(new Point(i * 30, 0));
                PlotCanvas.Children.Add(pl);
            }

            if (Model.Values.Count < 2)
            {
                _pl = new Polyline { Stroke = PlotColor };

                for (var i = 0; i < NumOfCoordinates; i++)
                {
                    _pl.Points.Add(new Point(i * 30, 0.5 * PlotCanvas.ActualHeight));
                }
            }
            else
            {
                foreach (var value in Model.Values)
                {
                    _pl = new Polyline {Stroke = PlotColor};

                    var maxValue = value.Max();
                    var minValue = value.Min();

                    for (var i = 0; i < NumOfCoordinates; i++)
                    {
                        double x = i*30;
                        var y = value[i];
                        _pl.Points.Add(ScaledCurvePoint(x, y, minValue, maxValue));
                    }

                    PlotCanvas.Children.Add(_pl);
                }
            }
        }

        private Point ScaledCurvePoint(double x, double y, double min, double max)
        {
            var scaledPoint = new Point
            {
                X = x,
                Y = PlotCanvas.ActualHeight - (y - min) * PlotCanvas.ActualHeight / (max - min)
            };
            return scaledPoint;
        }

        private void PlotColorBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var propertyInfo = PlotColorBox.SelectedItem as PropertyInfo;
            if (propertyInfo == null) return;
            var myColor = (Color)propertyInfo.GetValue(null, null);
            PlotColor = new SolidColorBrush(myColor);
        }
    }
}
