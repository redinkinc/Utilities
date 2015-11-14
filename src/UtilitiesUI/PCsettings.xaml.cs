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
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Utilities
{
    /// <summary>
    /// Interaction logic for PCsettings.xaml
    /// </summary>
    public partial class PCsettings
    {
        private ParallelCoordinatesControl ParallelCoordinatesControlModel { get; set; }
        public PCsettings(ParallelCoordinatesControl pcControl)
        {
            InitializeComponent();
            ParallelCoordinatesControlModel = pcControl;
            StartColorBox.ItemsSource = typeof(Colors).GetProperties();
            StartColorBox.SelectedItem = typeof(Colors).GetProperty(pcControl.StartColorName);
            StopColorBox.ItemsSource = typeof(Colors).GetProperties();
            StopColorBox.SelectedItem = typeof(Colors).GetProperty(pcControl.StopColorName);
        }

        private void StartColorBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var propertyInfo = StartColorBox.SelectedItem as PropertyInfo;
            if (propertyInfo == null) return;
            var myColor = (Color)propertyInfo.GetValue(null, null);
            ParallelCoordinatesControlModel.StartColor = myColor;
            ParallelCoordinatesControlModel.StartColorName = GetColorName(myColor);
        }

        private void StopColorBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var propertyInfo = StopColorBox.SelectedItem as PropertyInfo;
            if (propertyInfo == null) return;
            var myColor = (Color)propertyInfo.GetValue(null, null);
            ParallelCoordinatesControlModel.StopColor = myColor;
            ParallelCoordinatesControlModel.StopColorName = GetColorName(myColor);
        }

        public static string GetColorName(Color color)
        {
            var colors = typeof(Colors);
            for (int index = 0; index < colors.GetProperties().Length; index++)
            {
                var prop = colors.GetProperties()[index];
                if (((Color) prop.GetValue(null, null)) == color)
                    return prop.Name;
            }

            throw new Exception("The provided Color is not named.");
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            ParallelCoordinatesControlModel.AddChart();
            var parentWindow = Window.GetWindow(this);
            parentWindow?.Close();
        }

        private void headerThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var parentWindow = Window.GetWindow(this);
            if (parentWindow == null) return;
            parentWindow.Left += e.HorizontalChange;
            parentWindow.Top += e.VerticalChange;
        }

        
    }
}
