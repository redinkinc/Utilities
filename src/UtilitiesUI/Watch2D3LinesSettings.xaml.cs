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
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Utilities
{
    /// <summary>
    /// Interaction logic for Watch2Dsettings.xaml
    /// </summary>
    public partial class Watch2D3LinesSettings : UserControl
    {
        private Watch2D3LinesControl Watch2D3LinesControlModel { get; set; }

        public Watch2D3LinesSettings(Watch2D3LinesControl w2D3LControl)
        {
            InitializeComponent();
            Watch2D3LinesControlModel = w2D3LControl;
            PlotColorBox1.ItemsSource = typeof(Colors).GetProperties();
            PlotColorBox1.SelectedItem = typeof(Colors).GetProperty(w2D3LControl.PlotColorName[0]);
            PlotColorBox2.ItemsSource = typeof(Colors).GetProperties();
            PlotColorBox2.SelectedItem = typeof(Colors).GetProperty(w2D3LControl.PlotColorName[1]);
            PlotColorBox3.ItemsSource = typeof(Colors).GetProperties();
            PlotColorBox3.SelectedItem = typeof(Colors).GetProperty(w2D3LControl.PlotColorName[2]);
        }

        //private void CanvasType_OnSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        //{
        //    Watch2DControlModel.SelectedType = CanvasType.SelectedIndex;
        //}

        private void PlotColorBox1_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var propertyInfo = PlotColorBox1.SelectedItem as PropertyInfo;
            if (propertyInfo == null) return;
            var myColor = (Color)propertyInfo.GetValue(null, null);
            Watch2D3LinesControlModel.PlotColorBrushes[0] = new SolidColorBrush(myColor);
            Watch2D3LinesControlModel.PlotColorName[0] = GetColorName(myColor);
        }

        private void PlotColorBox2_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var propertyInfo = PlotColorBox2.SelectedItem as PropertyInfo;
            if (propertyInfo == null) return;
            var myColor = (Color)propertyInfo.GetValue(null, null);
            Watch2D3LinesControlModel.PlotColorBrushes[1] = new SolidColorBrush(myColor);
            Watch2D3LinesControlModel.PlotColorName[1] = GetColorName(myColor);
        }

        private void PlotColorBox3_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var propertyInfo = PlotColorBox3.SelectedItem as PropertyInfo;
            if (propertyInfo == null) return;
            var myColor = (Color)propertyInfo.GetValue(null, null);
            Watch2D3LinesControlModel.PlotColorBrushes[2] = new SolidColorBrush(myColor);
            Watch2D3LinesControlModel.PlotColorName[2] = GetColorName(myColor);
        }

        public static string GetColorName(Color color)
        {
            var colors = typeof(Colors);
            foreach (var prop in colors.GetProperties())
            {
                if ((Color)prop.GetValue(null, null) == color)
                    return prop.Name;
            }

            throw new Exception("The provided Color is not named.");
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            Watch2D3LinesControlModel.AddChart();
            var parentWindow = Window.GetWindow(this);
            parentWindow.Close();
        }

        private void YMax_OnSelectionChanged(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
