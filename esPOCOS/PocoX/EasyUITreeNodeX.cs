using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eStore.POCOS.PocoX
{
    public partial class EasyUITreeNode
    {
        public EasyUITreeNode()
        {
            this.children = new List<EasyUITreeNode>();
            this.products = new List<string>();
        }

        public EasyUITreeNode(getHierarchyTree_Result category)
        {
            this.children = new List<EasyUITreeNode>();
            this.products = new List<string>();
            this.id = category.CATEGORY_ID;
            this.text = category.CATEGORY_DISPLAYNAME;
            this.nodeState = TreeStates.closed;
            this.categoryType = category.CATEGORY_TYPE;
            this.sequence = category.SEQUENCE;
            this.condition = category.CONDITIONS;
            this.createdBy = category.CREATED_BY;
            this.createdDate = category.CREATED_DATE;
            this.lastUpdateBy = category.LAST_UPDATED_BY;
            this.lastUpdateDate = category.LAST_UPDATED_DATE;
            this.displayType = category.DisplayType;
            this.nodeTree = category.NodeTree;
            this.hierarchyID = category.HierarchyID;
            this.parentHierarchyID = category.ParentHierarchyID;
            this.transtext = string.Empty;
        }

        public int id { get; set; }
        public string text { get; set; }
        public string state
        {
            get
            {
                return this.nodeState.ToString();
            }
        }
        public TreeStates nodeState { get; set; }
        public NodeType nodeType
        {
            get
            {
                switch(this.categoryType)
                {
                    case "BUNode":
                        return NodeType.BUNode;
                    case "Node":
                        return NodeType.Node;
                    case "Value":
                        return NodeType.Value;
                    case "Root":
                    default:
                        return NodeType.Root;
                }
            }
        }
        public string categoryType { get; set; }
        public int? sequence { get; set; }
        public string condition { get; set; }
        public string createdBy { get; set; }
        public DateTime? createdDate { get; set; }
        public string lastUpdateBy { get; set; }
        public DateTime? lastUpdateDate { get; set; }
        public string displayType { get; set; }
        public string nodeTree { get; set; }
        public string hierarchyID { get; set; }
        public string parentHierarchyID { get; set; }
        public List<EasyUITreeNode> children { get; set; }
        public List<string> products { get; set; }
        public string iconCls
        {
            get
            {
                return this.nodeType.ToString();
            }
        }
        public string transtext { get; set; }
    }

    public enum TreeStates
    {
        open,
        closed,
    }
    public enum NodeType
    {
        BUNode,
        Node,
        Root,
        Value
    }
}
