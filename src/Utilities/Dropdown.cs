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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Autodesk.DesignScript.Runtime;
using Dynamo.Controls;
using Dynamo.Models;
using Dynamo.Utilities;
using Dynamo.Wpf;
using DSCoreNodesUI;
using ProtoCore.AST.AssociativeAST;
using Utilities.Properties;
using System.Xml;

namespace Utilities
{
    [NodeName("DropDown")]
    [NodeCategory("Utilities")]
    [NodeDescription("DropdownDescription", typeof(Resources))]
    [IsDesignScriptCompatible]
    public class Dropdown : NodeModel
    {
        #region members

        private int _index;
        private ObservableCollection<DynamoDropDownItem> _items = new ObservableCollection<DynamoDropDownItem>();
        public event EventHandler RequestChangeDropdown;
        public int tempIndex;

        #endregion

        #region properties

        /// <summary>
        /// The selected Index
        /// </summary>

        public int Index
        {
            get { return _index; }
            set
            {
                _index = value;
                RaisePropertyChanged("Index");

                OnNodeModified();
            }
        }

        public ObservableCollection<DynamoDropDownItem> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                RaisePropertyChanged("Items");

                OnNodeModified();
            }
        }

        protected virtual void OnRequestChangeDropdown(object sender, EventArgs e)
        {
            if (RequestChangeDropdown != null)
                RequestChangeDropdown(sender, e);
        }

        private void Dropdown_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "IsUpdated")
                return;

            if (InPorts.Any(x => x.Connectors.Count == 0))
                return;

            OnRequestChangeDropdown(this, EventArgs.Empty);
        }
        
        #endregion

        #region constructor

        /// <summary>
        /// The constructor for a NodeModel is used to create
        /// the input and output ports and specify the argument
        /// lacing.
        /// </summary>
        public Dropdown()
        {

            InPortData.Add(new PortData("list", Resources.DropdownPortDataInputToolTip));

            OutPortData.Add(new PortData("Item", Resources.DropdownPortDataOutputValueToolTip));
            OutPortData.Add(new PortData("Index", Resources.DropdownPortDataOutputIndexToolTip));

            RegisterAllPorts();

            // The arugment lacing is the way in which Dynamo handles
            // inputs of lists. If you don't want your node to
            // support argument lacing, you can set this to LacingStrategy.Disabled.
            ArgumentLacing = LacingStrategy.Disabled;

            // removes preview button in the lower right corner of the node.
            ShouldDisplayPreviewCore = false;
            
            this.PropertyChanged += Dropdown_PropertyChanged;

            Items.Add(new DynamoDropDownItem("nothing selected", null));
            Index = 0;

        }

        #endregion

        protected override void SerializeCore(XmlElement element, SaveContext context)
        {
            base.SerializeCore(element, context);

            var helper = new XmlElementHelper(element);
            helper.SetAttribute("selectedIndex", Index);
        }

        protected override void DeserializeCore(XmlElement element, SaveContext context)
        {
            base.DeserializeCore(element, context);

            var helper = new XmlElementHelper(element);
            tempIndex = helper.ReadInteger("selectedIndex", 0);
        }

        #region public methods


        [IsVisibleInDynamoLibrary(false)]
        public override IEnumerable<AssociativeNode> BuildOutputAst(
            List<AssociativeNode> inputAstNodes)
        {
            var item = Items[Index].Item;
            
            return new[]
            {

                // In these assignments, GetAstIdentifierForOutputIndex finds 
                // the unique identifier which represents an output on this node
                // and 'assigns' that variable the expression that you create.
                
                // For the first node, return an index of the input
                
                AstFactory.BuildAssignment(
                GetAstIdentifierForOutputIndex(0),
                item is IEnumerable
                   ? CreateASTForCollection((IEnumerable) item)
                   : AstFactory.BuildPrimitiveNodeFromObject(item)),

                //AstFactory.BuildAssignment(
                //    GetAstIdentifierForOutputIndex(0),
                //    AstFactory.BuildPrimitiveNodeFromObject(Items[Index].Item)),

                AstFactory.BuildAssignment(
                    GetAstIdentifierForOutputIndex(1),
                    AstFactory.BuildIntNode(Index))//,

                //AstFactory.BuildAssignment(
                //GetAstIdentifierForOutputIndex(2),
                //AstFactory.BuildStringNode(inputAstNodes[0].ToString()))

            };
        }

        AssociativeNode CreateASTForCollection(IEnumerable items)
        {
            var itemAsts = new List<AssociativeNode>();
            foreach (var item in items)
            {
                if (item is IEnumerable)
                    itemAsts.Add(CreateASTForCollection((IEnumerable) item));
                else 
                    // suppose the type of item is in {int, double, boolean, string}
                    itemAsts.Add(AstFactory.BuildPrimitiveNodeFromObject(item));
            }
            return AstFactory.BuildExprList(itemAsts);
        }

        #endregion

    }

    /// <summary>
    ///     View customizer for Utilities Node Model.
    /// </summary>
    public class UtilitiesNodeViewCustomization : INodeViewCustomization<Dropdown>
    {
        bool isFirstSet = true;

        public void CustomizeView(Dropdown model, NodeView nodeView)
        {
            var dm = nodeView.ViewModel.DynamoViewModel.Model;
            // Create an instance of our custom UI class (defined in xaml),
            // and put it into the input grid.
            var dropdownControl = new DropdownControl(model);
            nodeView.inputGrid.Children.Add(dropdownControl);

            // Set the data context for our control to be this class.
            // Properties in this class which are data bound will raise 
            // property change notifications which will update the UI.
            dropdownControl.DataContext = model;

            model.RequestChangeDropdown += delegate
            {
                model.DispatchOnUIThread(delegate
                {
                    var listNode = model.InPorts[0].Connectors[0].Start.Owner;
                    var listIndex = model.InPorts[0].Connectors[0].Start.Index;

                    var listId = listNode.GetAstIdentifierForOutputIndex(listIndex).Name;

                    var listMirror = dm.EngineController.GetMirror(listId);

                    model.Items.Clear();
                    
                    if (listMirror == null)
                    {
                        model.Items.Add(new DynamoDropDownItem("nothing selected", null));
                    }
                    else
                    {   
                        var index = 0;
                        if (listMirror.GetData().IsCollection)
                        {
                            if (listMirror.GetData().GetElements().First().IsCollection)
                            {
                                foreach (var data in listMirror.GetData().GetElements())
                                {
                                    var dataString = index.ToString() + ": " +
                                                     data.GetElements()[0].Data.ToString() +
                                                     " (" + data.GetElements().ToString() +")";
                                    var classtype = data.GetElements().ToList().GetType().ToString();
                                    var obj = data.GetElements().ToList();
                                    model.Items.Add(new DynamoDropDownItem(dataString, obj));
                                    index++;
                                }
                            }
                            else
                            {
                                foreach (var data in listMirror.GetData().GetElements())
                                {
                                    var dataString = index.ToString() + ": " + data.Data.ToString() + " (" +
                                                     data.Data.GetType().ToString().Remove(0, 7) + ")";
                                    model.Items.Add(new DynamoDropDownItem(dataString, data.Data));
                                    index++;
                                }
                            }
                        }
                        else
                        {
                            var data = listMirror.GetData().Data;
                            var dataString = index.ToString() + ':' + data.ToString();
                            model.Items.Add(new DynamoDropDownItem(dataString, data));
                        }
                    }

                    var itemStrings = model.Items.Select(x => x.ToString());
                    dropdownControl.AddItems(itemStrings.ToObservableCollection());

                    if (isFirstSet)
                    {
                        if (dropdownControl.Box.Items.Count > model.tempIndex)
                            dropdownControl.Box.SelectedIndex = model.tempIndex;
                        isFirstSet = false;
                    }

                    //model.Value = dropdownControl.Selection;

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
