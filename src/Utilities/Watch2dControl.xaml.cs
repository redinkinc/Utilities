using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Utilities
{
    public partial class Watch2DControl
    {
        private double xmin = 0;
        private double _xmax;
        private double ymin = 0;
        private double _ymax;

        private Watch2D _watch2D;

        private Polyline _pl;

        private static SolidColorBrush _bgrColorBrush = new SolidColorBrush() { Color = Color.FromArgb(255,229,227,223) };

        public SolidColorBrush PlotColor { get; set; }
        public List<double> Values { get; set; }
        public int SelectedType { get; set; }

        public Watch2DControl(Watch2D model)
        {
            InitializeComponent();
            _watch2D = model;
            Values = new List<double>();
            PlotColor = new SolidColorBrush() { Color = Color.FromArgb(255, 0, 0, 0) };
            PlotColorBox.ItemsSource = typeof (Colors).GetProperties();
            PlotColorBox.SelectedItem = typeof (Colors).GetProperty("Black");
        }

        public void AddChart()
        {
            PlotCanvas.Children.Clear();

            if (SelectedType == 0)
            {
                DrawPlot();
            }
            else
            {
                DrawHisto();
            }
            
        }

        private void DrawHisto()
        {
            var recWidth = PlotCanvas.Width/Values.Count;

            // Create a SolidColorBrush with a red color to fill the 
            // Ellipse with.

            var max = Values.Max();
            var scale = PlotCanvas.Height/max; 

            // middle points
            for (int i = 0; i < Values.Count; i++)
            {
                var rectangle = new Rectangle { Fill = PlotColor, StrokeThickness = 5, Stroke = _bgrColorBrush };
                rectangle.MouseLeftButtonUp += rectangle_MouseLeftButtonUp;
                rectangle.Width = recWidth;
                rectangle.Height = Values[i] * scale;
                rectangle.Name = "rec" + i;

                PlotCanvas.Children.Add(rectangle);

                Canvas.SetLeft(rectangle, recWidth*i);
                Canvas.SetTop(rectangle, (max - Values[i]) *scale);
            }
        }

        private void rectangle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var id = sender.ToString();
        }

        private void DrawPlot()
        {
            _pl = new Polyline {Stroke = PlotColor};

            _xmax = Values.Count - 1;
            _ymax = Values.Max();

            for (int i = 0; i <= _xmax; i++)
            {
                double x = i;
                var y = Values[i];
                _pl.Points.Add(CurvePoint(
                    new Point(x, y)));
            }

            PlotCanvas.Children.Add(_pl);
        }

        private Point CurvePoint(Point pt)
        {
            var result = new Point
            {
                X = (pt.X - xmin) * PlotCanvas.Width / (_xmax - xmin),
                Y = PlotCanvas.Height - (pt.Y - ymin) * PlotCanvas.Height
                    / (_ymax - ymin)
            };
            return result;
        }

        private void CanvasType_OnSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            SelectedType = CanvasType.SelectedIndex;
            //_watch2d.Updated();
            //update Chart();
        }

        private void PlotColorBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var propertyInfo = PlotColorBox.SelectedItem as PropertyInfo;
            if (propertyInfo != null)
            {
                Color myColor = (Color)propertyInfo.GetValue(null, null);
                PlotColor = new SolidColorBrush(myColor);
            }
            //_watch2d.Updated();
            //update Chart
        }
    }

}
