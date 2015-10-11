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

