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

namespace Utilities
{
    /// <summary>
    /// 
    /// </summary>
    public class ParallelCoordinateData
    {
        #region private members

        #endregion

        #region public members

        /// <summary>
        /// Parameter names
        /// </summary>
        public List<string> ParameterList { get; set; }

        /// <summary>
        /// Values of the parameters.
        /// </summary>
        public List<List<double>> ParamValues { get; set; }

        #endregion

        #region constructor

        private ParallelCoordinateData(List<string> parameterList)
        {
            ParameterList = parameterList;
            ParamValues = new List<List<double>>();
        }

        #endregion

        #region public member functions

        /// <summary>
        /// Initilaize the Data for Utilities.ParallelCoordinates.
        /// </summary>
        /// <param name="parameterList">List of the names of the parameter.</param>
        /// <returns></returns>
        public static ParallelCoordinateData InitParallelCoordinateData(List<string> parameterList)
        {
            return new ParallelCoordinateData(parameterList);
        }

        /// <summary>
        /// Adds values to the parallelCoordinateData
        /// </summary>
        /// <param name="paramValues">List of Values to be added.</param>
        /// <returns></returns>
        public ParallelCoordinateData AddValues(List<double> paramValues)
        {
            ParamValues.Add(paramValues);
            return this;
        }

        public override string ToString()
        {
            return ParameterList.ToString();
        }

        #endregion
    }
}
