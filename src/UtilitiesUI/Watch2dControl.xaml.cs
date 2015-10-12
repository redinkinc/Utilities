/*
 *  This file is part of Utilities for Dynamo.
 *  
 *  Copyright (c) 2014-2015 Technische Universitaet Muenchen, 
 *  Chair of Computational Modeling and Simulation (https://www.cms.bgu.tum.de/)
 *  LEONHARD OBERMEYER CENTER (https://www.loc.tum.de)
 *  
 *  Utilities by Fabian Ritter (mailto:mail@redinkinc.de)
 * 
 *  Utilities is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *  
 *  Utilities is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *  
 *  You should have received a copy of the GNU General Public License
 *  along with Utilities. If not, see <http://www.gnu.org/licenses/>.
 */
 
using System.Collections.Generic;
using System.Linq;
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
        public string PlotColorName { get; set; }
        public List<double> Values { get; set; }
        public int SelectedType { get; set; }

        public Watch2DControl(Watch2D model)
        {
            InitializeComponent();
            _watch2D = model;
            Values = new List<double>();
            PlotColor = new SolidColorBrush() { Color = Color.FromArgb(255, 0, 0, 0) };
            PlotColorName = "Black";
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
                X = (pt.X - xmin)*PlotCanvas.Width/(_xmax - xmin),
                Y = PlotCanvas.Height - (pt.Y - ymin)*PlotCanvas.Height
                    /(_ymax - ymin)
            };
            return result;
        }
        private void CanvasType_OnSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            SelectedType = CanvasType.SelectedIndex;
        }

        private void Watch2Dsettings_OnClick(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new Window
            {
                Content = new Watch2Dsettings(this),
                Width = 200,
                Height = 200,
                WindowStyle = WindowStyle.None
            };
            settingsWindow.Show();
        }

    }

}
