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

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using ProtoCore.AST.AssociativeAST;

namespace Utilities
{
    /// <summary>
    /// Interaction logic for ParallelCoordinates.xaml
    /// </summary>
    public partial class ParallelCoordinatesControl
    {
        public double CanvasWidth { get; private set; }

        private static readonly SolidColorBrush BgrColorBrush = new SolidColorBrush() { Color = Color.FromArgb(255, 229, 227, 223) };

        public ParallelCoordinates Model { get; set; }
        public Color StartColor { get; set; }
        public string StartColorName { get; set; }
        public Color StopColor { get; set; }
        public string StopColorName { get; set; }
        public int CoordinateDistance { get; set; }

        public ParallelCoordinatesControl(ParallelCoordinates model)
        {
            InitializeComponent();
            Model = model;
            StartColor = Colors.Blue;
            StartColorName = "Blue";
            StopColor = Colors.Green;
            StopColorName = "Green";
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
            HeaderPanel.Children.Clear();
            foreach (var maxValue in Model.MaxValues)
            {
                HeaderPanel.Children.Add( new TextBox()
                {
                    Text = Math.Round(maxValue).ToString(CultureInfo.InvariantCulture),
                    Width = CoordinateDistance,
                    Margin = new Thickness(5, 0, 5, 0),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Padding = new Thickness(2)
                 });
            }

            FooterPanel.Children.Clear();
            foreach (var minValue in Model.MinValues)
            {
                FooterPanel.Children.Add(new TextBox()
                {
                    Text = Math.Round(minValue).ToString(CultureInfo.InvariantCulture),
                    Width = CoordinateDistance,
                    Margin = new Thickness(5, 0, 5, 0),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Padding = new Thickness(2)

                });

            }

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

            var numOfValues = Model.Values.Count;

            for (var index = 0; index < numOfValues; index++)
            {
                var pl = new Polyline
                {
                    Stroke = new SolidColorBrush(GetRelativeColor(index, numOfValues))
                };

                for (var i = 0; i < numOfCoordinates; i++)
                {
                    double x = i*CoordinateDistance;
                    var y = Model.Values[index][i];
                    pl.Points.Add(ScaledCurvePoint(x, y, Model.MinValues[i], Model.MaxValues[i]));
                }

                PlotCanvas.Children.Add(pl);
            }
        }

        private Point ScaledCurvePoint(double x, double y, double min, double max)
        {
            var scaledPoint = new Point();
            if ( Equals(min, max) )
            {
                scaledPoint.X = x;
                scaledPoint.Y = PlotCanvas.ActualHeight/2;
            }
            else
            {
                scaledPoint.X = x;
                scaledPoint.Y = PlotCanvas.ActualHeight - (y - min)*PlotCanvas.ActualHeight/(max - min);
            }

            return scaledPoint;
        }

        private void ResetButton_OnClick_OnClick(object sender, RoutedEventArgs e)
        {
            this.Reset();
        }

        private void ParallelCoordinateSettings_OnClick(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new Window { Content = new PCsettings(this) };
            settingsWindow.Height = 250;
            settingsWindow.Width = 200;
            settingsWindow.WindowStyle = WindowStyle.None;
            settingsWindow.Show();
        }

        public Color GetRelativeColor(int i, int n)
        {
            if (n > 1) n -= 1;
            var div = (double) i / (double) n;
            var a = Convert.ToByte(StartColor.A - (StartColor.A - StopColor.A) * div);
            var r = Convert.ToByte(StartColor.R - (StartColor.R - StopColor.R) * div);
            var g = Convert.ToByte(StartColor.G - (StartColor.G - StopColor.G) * div);
            var b = Convert.ToByte(StartColor.B - (StartColor.B - StopColor.B) * div);

            return Color.FromArgb( a, r, g, b);
        }
    }
}
