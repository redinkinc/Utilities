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

using System.Windows;
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
        public string PlotColorName { get; set; }
        public int CoordinateDistance { get; set; }

        public ParallelCoordinatesControl(ParallelCoordinates model)
        {
            InitializeComponent();
            Model = model;
            PlotColor = new SolidColorBrush() { Color = Color.FromArgb(255, 0, 0, 255) };
            PlotColorName = "Blue";
            CoordinateDistance = 30;
        }

        public void AddChart()
        {
            PlotCanvas.Children.Clear();
            DrawPlot();
        }

        public void Reset()
        {
            PlotCanvas.Children.Clear();
            Model.Reset();
        }

        private void DrawPlot()
        {
            
            var numOfCoordinates = Model.ParameterNames.Count;
            CanvasWidth = CoordinateDistance*(numOfCoordinates-1);
            PlotCanvas.Width = CanvasWidth;
            
            for (var i = 0; i < numOfCoordinates; i++)
            {
                var pl = new Polyline() {Stroke = Brushes.Black};
                pl.Points.Add(new Point(i * CoordinateDistance, 300));
                pl.Points.Add(new Point(i * CoordinateDistance, 0));
                PlotCanvas.Children.Add(pl);
            }

            if (Model.Values.Count < 2)
            {
                _pl = new Polyline { Stroke = PlotColor };

                for (var i = 0; i < numOfCoordinates; i++)
                {
                    _pl.Points.Add(new Point(i * CoordinateDistance, 0.5 * PlotCanvas.Height));
                }
            }
            else
            {

                foreach (var value in Model.Values)
                {
                    _pl = new Polyline {Stroke = PlotColor};

                    for (var i = 0; i < numOfCoordinates; i++)
                    {
                        double x = i*CoordinateDistance;
                        var y = value[i];
                        _pl.Points.Add(ScaledCurvePoint(x, y, Model.MinValues[i], Model.MaxValues[i]));
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

        private void ResetButton_OnClick_OnClick(object sender, RoutedEventArgs e)
        {
            this.Reset();
        }

        private void ParallelCoordinateSettings_OnClick(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new Window { Content = new PCsettings(this) };
            settingsWindow.Height = 200;
            settingsWindow.Width = 200;
            settingsWindow.WindowStyle = WindowStyle.None;
            settingsWindow.Show();
        }
    }
}
