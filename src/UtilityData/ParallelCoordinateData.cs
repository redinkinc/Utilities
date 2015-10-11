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
