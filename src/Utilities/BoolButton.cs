using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    [NodeName("BoolButton")]
    [NodeCategory("Utilities")]
    [NodeDescription("BoolButtonDescription", typeof(Resources))]
    [IsDesignScriptCompatible]
    public class BoolButton : NodeModel
    {
        #region private members

        private bool _value;

        public bool Value
        {
            get
            {
                return _value;
            }

            set
            {
                _value = value;
                RaisePropertyChanged("Bool");

                OnNodeModified();
            }
        }

        #endregion

        #region constructor

        /// <summary>
        /// The constructor for a NodeModel is used to create
        /// the input and output ports and specify the argument
        /// lacing.
        /// </summary>
        public BoolButton()
        {
            // Nodes can have an arbitrary number of inputs and outputs.
            // If you want more ports, just create more PortData objects.
            OutPortData.Add(new PortData("", Resources.BoolButtonPortDataOutputToolTip));

            // This call is required to ensure that your ports are
            // properly created.
            RegisterAllPorts();

            // The arugment lacing is the way in which Dynamo handles
            // inputs of lists. If you don't want your node to
            // support argument lacing, you can set this to LacingStrategy.Disabled.
            ArgumentLacing = LacingStrategy.Disabled;

            this.PropertyChanged += BoolButton_PropertyChanged;

            Value = false;
        }

        public event EventHandler RequestChangeBoolButton;
        protected virtual void OnRequestChangeBoolButton(object sender, EventArgs e)
        {
            RequestChangeBoolButton?.Invoke(sender, e);
        }

        private void BoolButton_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "IsUpdated")
                return;

            OnRequestChangeBoolButton(this, EventArgs.Empty);
        }

        #endregion

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
                    AstFactory.BuildBooleanNode(Value))
            };
        }

        #endregion

    }

    /// <summary>
    ///     View customizer for Utilities Node Model.
    /// </summary>
    public class BoolButtonViewCustomization : INodeViewCustomization<BoolButton>
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
        public void CustomizeView(BoolButton model, NodeView nodeView)
        {
            var boolButtonControl = new BoolButtonControl(model);
            nodeView.inputGrid.Children.Add(boolButtonControl);

            boolButtonControl.DataContext = model;
        }

        /// <summary>
        /// Here you can do any cleanup you require if you've assigned callbacks for particular 
        /// UI events on your node.
        /// </summary>
        public void Dispose() { }
    }

}
