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
    public partial class Watch2Dsettings : UserControl
    {
        private Watch2DControl Watch2DControlModel { get; set; }

        public Watch2Dsettings(Watch2DControl w2DControl)
        {
            InitializeComponent();
            Watch2DControlModel = w2DControl;
            PlotColorBox.ItemsSource = typeof(Colors).GetProperties();
            PlotColorBox.SelectedItem = typeof(Colors).GetProperty(w2DControl.PlotColorName);
        }

        //private void CanvasType_OnSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        //{
        //    Watch2DControlModel.SelectedType = CanvasType.SelectedIndex;
        //}

        private void PlotColorBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var propertyInfo = PlotColorBox.SelectedItem as PropertyInfo;
            if (propertyInfo == null) return;
            Color myColor = (Color)propertyInfo.GetValue(null, null);
            Watch2DControlModel.PlotColor = new SolidColorBrush(myColor);
            Watch2DControlModel.PlotColorName = GetColorName(myColor);
        }

        public static string GetColorName(Color color)
        {
            Type colors = typeof(Colors);
            foreach (var prop in colors.GetProperties())
            {
                if (((Color)prop.GetValue(null, null)) == color)
                    return prop.Name;
            }

            throw new Exception("The provided Color is not named.");
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            Watch2DControlModel.AddChart();
            Window parentWindow = Window.GetWindow(this);
            parentWindow.Close();
        }

        private void YMax_OnSelectionChanged(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
