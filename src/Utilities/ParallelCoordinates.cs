using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml;
using Autodesk.DesignScript.Runtime;
using Dynamo.Models;
using Dynamo.Controls;
using Dynamo.Wpf;
using Utilities.Properties;
using Dynamo.Utilities;
using ProtoCore.AST.AssociativeAST;

namespace Utilities
{
    [NodeName("ParallelCoordinates")]
    [NodeCategory("Utilities")]
    [NodeDescription("ParallelCoordinatesDescription", typeof (Resources))]
    [IsDesignScriptCompatible]
    public class ParallelCoordinates : NodeModel
    {
        private int _plotColor;

        private List<string> _parameterNames;
        private List<List<double>> _values;

        public List<string> ParameterNames
        {
        get { return _parameterNames; }
            set
            {
                _parameterNames = value;
                RaisePropertyChanged("ParameterNames");

                OnNodeModified();
            }    
        } 

        public List<List<double>> Values
        {
            get { return _values; }
            set
            {
                _values = value;
                RaisePropertyChanged("Values");

                OnNodeModified();
            }
        }

        public event EventHandler RequestChangeParallelCoordinates;

        protected virtual void OnRequestChangeParallelCoordinates(object sender, EventArgs e)
        {
            RequestChangeParallelCoordinates?.Invoke(sender, e);
        }

        public ParallelCoordinates()
        {
            // When you create a UI node, you need to do the
            // work of setting up the ports yourself. To do this,
            // you can populate the InPortData and the OutPortData
            // collections with PortData objects describing your ports.
            InPortData.Add(new PortData("", Resources.ParallelCoordinatesPortDataInputToolTip));
            //InPortData.Add(new PortData("", Resources.ParallelCoordinatesPortDataInputToolTip));

            // Nodes can have an arbitrary number of inputs and outputs.
            // If you want more ports, just create more PortData objects.
            OutPortData.Add(new PortData("", Resources.ParallelCoordinatesPortDataOutputToolTip));

            // This call is required to ensure that your ports are
            // properly created.
            RegisterAllPorts();

            // The arugment lacing is the way in which Dynamo handles
            // inputs of lists. If you don't want your node to
            // support argument lacing, you can set this to LacingStrategy.Disabled.
            ArgumentLacing = LacingStrategy.Disabled;

            this.PropertyChanged += ParallelCoordinates_PropertyChanged;

            _parameterNames = new List<string>();
            _values = new List<List<double>>();
            _plotColor = new int();
        }

        private void ParallelCoordinates_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "IsUpdated")
                return;

            if (InPorts.Any(x => x.Connectors.Count == 0))
                return;

            OnRequestChangeParallelCoordinates(this, EventArgs.Empty);
        }

        protected override void SerializeCore(XmlElement element, SaveContext context)
        {
            base.SerializeCore(element, context);

            var helper = new XmlElementHelper(element);
            helper.SetAttribute("color", _plotColor);
            helper.SetAttribute("values", _values.ToString());
        }

        protected override void DeserializeCore(XmlElement element, SaveContext context)
        {
            base.DeserializeCore(element, context);

            var helper = new XmlElementHelper(element);
            _plotColor = helper.ReadInteger("color", 0);
            //_values = helper.ReadString("values").Split(","); //read in string to List<List<double>>
        }

        public new void Updated()
        {
            OnNodeModified();
        }

        public void Reset()
        {
            _values.Clear();
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

    }

    /// <summary>
    ///     View customizer for Utilities Node Model.
    /// </summary>
    public class ParallelCoordinatesNodeViewCustomization : INodeViewCustomization<ParallelCoordinates>
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
        public void CustomizeView(ParallelCoordinates model, NodeView nodeView)
        {
            var dm = nodeView.ViewModel.DynamoViewModel.Model;
            var parallelCoordinatesControl = new ParallelCoordinatesControl(model);

            nodeView.inputGrid.Children.Add(parallelCoordinatesControl);

            parallelCoordinatesControl.DataContext = model;

            model.RequestChangeParallelCoordinates += delegate
            {
                model.DispatchOnUIThread(delegate
                {
                    var paramValueNode = model.InPorts[0].Connectors[0].Start.Owner;
                    var paramValueIndex = model.InPorts[0].Connectors[0].Start.Index;
                    //var xValueNode = model.InPorts[1].Connectors[0].Start.Owner;
                    //var xValueIndex = model.InPorts[1].Connectors[0].Start.Index;

                    var paramValueId = paramValueNode.GetAstIdentifierForOutputIndex(paramValueIndex).Name;
                    //var xValueId = xValueNode.GetAstIdentifierForOutputIndex(xValueIndex).Name;

                    var paramMirror = dm.EngineController.GetMirror(paramValueId);
                    //var startMirror = dm.EngineController.GetMirror(xValueId);

                    ParallelCoordinateData pcData;

                    //if (paramMirror.GetData().IsCollection)
                    //{
                    //    pcData = paramMirror.GetData().GetElements().Select(data => data.Data);
                    //}
                    //else
                    //{
                        pcData = (ParallelCoordinateData) paramMirror.GetData().Data;
                    //}

                    //if (startMirror.GetData().IsCollection)
                    //{
                    //    start.AddRange(startMirror.GetData().GetElements().Select(data => (double)data.Data));
                    //}
                    //else
                    //{
                    //    //var test = paramMirror.GetData().Data;
                    //    start.Add( (double) startMirror.GetData().Data );
                    //}
                    
                    //model.ParameterNames = parameters;
                    //model.Values.Add(start);
                    parallelCoordinatesControl.AddChart(pcData);
                });
            };
        }

        /// <summary>
        /// Here you can do any cleanup you require if you've assigned callbacks for particular 
        /// UI events on your node.
        /// </summary>
        public void Dispose()
        {
        }
    }
}
