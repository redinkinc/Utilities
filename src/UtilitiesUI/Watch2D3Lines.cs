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
using System.ComponentModel;
using System.Linq;
using System.Xml;
using Autodesk.DesignScript.Runtime;
using Dynamo.Controls;
using Dynamo.Models;
using Dynamo.Utilities;
using Dynamo.Wpf;
using ProtoCore.AST.AssociativeAST;
using Utilities.Properties;


namespace Utilities
{
    [NodeName("Watch2D")]
    [NodeCategory("Utilities")]
    [NodeDescription("Watch2dDescription", typeof(Resources))]
    [IsDesignScriptCompatible]
    public class Watch2D3Lines : NodeModel
    {
        #region private members

        private double _xmin;
        private double _ymin;
        private double _xmax;
        private double _ymax;

        private int _plotType;
        private int _plotColor;

        #endregion

        #region properties

        public event EventHandler RequestChangeWatch2D;
        protected virtual void OnRequestChangeWatch2D(object sender, EventArgs e)
        {
            RequestChangeWatch2D?.Invoke(sender, e);
        }

        public double Xmin
        {
            get { return _xmin; }
            set 
            {
                _xmin = value;
                RaisePropertyChanged("Xmin");
            }
        }

        public double Ymin
        {
            get { return _ymin; }
            set
            {
                _ymin = value;
                RaisePropertyChanged("Ymin");
            }
        }

        public double Xmax
        {
            get { return _xmax; }
            set
            {
                _xmin = value;
                RaisePropertyChanged("Xmax");
            }
        }

        public double Ymax
        {
            get { return _ymax; }
            set
            {
                _ymin = value;
                RaisePropertyChanged("Ymax");
            }
        }

        #endregion

        #region constructor

        /// <summary>
        /// The constructor for a NodeModel is used to create
        /// the input and output ports and specify the argument
        /// lacing.
        /// </summary>
        public Watch2D3Lines()
        {
            // When you create a UI node, you need to do the
            // work of setting up the ports yourself. To do this,
            // you can populate the InPortData and the OutPortData
            // collections with PortData objects describing your ports.
            InPortData.Add(new PortData("", Resources.Watch2dPortDataInputToolTip));
            InPortData.Add(new PortData("", Resources.Watch2dPortDataInputToolTip));
            InPortData.Add(new PortData("", Resources.Watch2dPortDataInputToolTip));
            //InPortData.Add(new PortData("2", Resources.UtilitiesPortDataInputToolTip));

            // Nodes can have an arbitrary number of inputs and outputs.
            // If you want more ports, just create more PortData objects.
            OutPortData.Add(new PortData("", Resources.Watch2dPortDataOutputToolTip));

            // This call is required to ensure that your ports are
            // properly created.
            RegisterAllPorts();

            // The arugment lacing is the way in which Dynamo handles
            // inputs of lists. If you don't want your node to
            // support argument lacing, you can set this to LacingStrategy.Disabled.
            ArgumentLacing = LacingStrategy.Disabled;

            this.PropertyChanged += Watch2D_PropertyChanged;

            // never used in current build!!
            Xmin = 0;
            Xmax = 6.5;
        
            Ymin = -1.1;
            Ymax = 1.1;
        }

        private void Watch2D_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "IsUpdated")
                return;

            if (InPorts.Any(x => x.Connectors.Count == 0))
                return;

            OnRequestChangeWatch2D(this, EventArgs.Empty);
        }

        #endregion

        protected override void SerializeCore(XmlElement element, SaveContext context)
        {
            base.SerializeCore(element, context);

            var helper = new XmlElementHelper(element);
            helper.SetAttribute("color", _plotColor);
        }

        protected override void DeserializeCore(XmlElement element, SaveContext context)
        {
            base.DeserializeCore(element, context);

            var helper = new XmlElementHelper(element);
            _plotColor = helper.ReadInteger("color", 0);
        }

        #region public methods

        public new void Updated()
        {
            OnNodeModified();
        }

        /// <summary>
        /// If this method is not overriden, Dynamo will, by default
        /// pass data through this node. But we wouldn't be here if
        /// we just wanted to pass data through the node, so let's 
        /// try using the data.
        /// </summary>
        /// <param name="inputAstNodes"></param>
        /// <returns></returns>
        [IsVisibleInDynamoLibrary(false)]
        public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
        {
            return new[]
            {
                AstFactory.BuildAssignment(
                    GetAstIdentifierForOutputIndex(0),
                    AstFactory.BuildExprList(inputAstNodes))
            };
        }

        #endregion

    }

    /// <summary>
    ///     View customizer for Utilities Node Model.
    /// </summary>
    public class Watch2D3LinesNodeViewCustomization : INodeViewCustomization<Watch2D3Lines>
    {
        /// <summary>
        /// At run-time, this method is called during the node 
        /// creation. Here you can create custom UI elements and
        /// add them to the node view, but we recommend designing
        /// your UI declaratively using xaml, and binding it to
        /// properties on this node as the DataContext.
        /// </summary>
        /// <param name="model">The NodeModel representing the node's core logic.</param>
        /// <param name="nodeView">The NodeView representing the node in the graph.</param>
        public void CustomizeView(Watch2D3Lines model, NodeView nodeView)
        {
            var dm = nodeView.ViewModel.DynamoViewModel.Model;
            var watch2D3LinesControl = new Watch2D3LinesControl(model);
            
            nodeView.inputGrid.Children.Add(watch2D3LinesControl);

            watch2D3LinesControl.DataContext = model;

            model.RequestChangeWatch2D += delegate
            {
                model.DispatchOnUIThread(delegate
                {
                    var x1ValueNode = model.InPorts[0].Connectors[0].Start.Owner;
                    var x1ValueIndex = model.InPorts[0].Connectors[0].Start.Index;
                    var x2ValueNode = model.InPorts[1].Connectors[0].Start.Owner;
                    var x2ValueIndex = model.InPorts[1].Connectors[0].Start.Index;
                    var x3ValueNode = model.InPorts[2].Connectors[0].Start.Owner;
                    var x3ValueIndex = model.InPorts[2].Connectors[0].Start.Index;

                    var x1ValueId = x1ValueNode.GetAstIdentifierForOutputIndex(x1ValueIndex).Name;
                    var x2ValueId = x2ValueNode.GetAstIdentifierForOutputIndex(x2ValueIndex).Name;
                    var x3ValueId = x3ValueNode.GetAstIdentifierForOutputIndex(x3ValueIndex).Name;

                    var startMirror = dm.EngineController.GetMirror(x1ValueId);
                    var midMirror = dm.EngineController.GetMirror(x2ValueId);
                    var endMirror = dm.EngineController.GetMirror(x3ValueId);

                    var start = new List<double>();
                    var mid = new List<double>();
                    var end = new List<double>();

                    if (startMirror == null)
                    {
                        start.Add(1.1);
                    }
                    else
                    {
                        if (startMirror.GetData().IsCollection)
                        {
                            start.AddRange(startMirror.GetData().GetElements().Select(data => (double) data.Data));
                        }
                        else
                        {
                            var x = (double) startMirror.GetData().Data;
                            start.Add(x); 
                        }
                    }

                    if (midMirror == null)
                    {
                        mid.Add(1.1);
                    }
                    else
                    {
                        if (midMirror.GetData().IsCollection)
                        {
                            mid.AddRange(midMirror.GetData().GetElements().Select(data => (double)data.Data));
                        }
                        else
                        {
                            var x = (double)midMirror.GetData().Data;
                            mid.Add(x);
                        }
                    }

                    if (endMirror == null)
                    {
                        end.Add(1.1);
                    }
                    else
                    {
                        if (endMirror.GetData().IsCollection)
                        {
                            end.AddRange(endMirror.GetData().GetElements().Select(data => (double)data.Data));
                        }
                        else
                        {
                            var x = (double)endMirror.GetData().Data;
                            end.Add(x);
                        }
                    }

                    var values = new List<List<double>>() {start, mid, end};

                    watch2D3LinesControl.Values = values;
                    watch2D3LinesControl.AddChart();
                });
            };

        }

        /// <summary>
        /// Here you can do any cleanup you require if you've assigned callbacks for particular 
        /// UI events on your node.
        /// </summary>
        public void Dispose() { }
    }

}
