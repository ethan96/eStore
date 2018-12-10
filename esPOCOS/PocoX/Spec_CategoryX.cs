using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;

namespace eStore.POCOS
{
    public partial class Spec_Category
    {

        public enum DisType { List, CheckBoxList, DropdownList, RadioButtonList, TextBox, DoubleList, DoubleRow }
        public enum CateType { BUNode, Node, Root, Value }
        public enum CateContion { And, Or}

        public bool isChange = false;        


        private List<Spec_Category> _children;
        public List<Spec_Category> children
        {
            get 
            {
                if (isChange || _children == null)
                {
                    spGetSpecByCategoryHelper helper = new spGetSpecByCategoryHelper();
                    var idLs = helper.getIdList("Spec_Category", "CATEGORY_NODE", "CATEGORY_ID", this.CATEGORY_ID, 1);
                    _children = (new Spec_CategoryHelper()).getCategoryListByIds(idLs);
                }
                return _children; 
            }
            set { _children = value; }
        }

        private List<Spec_Category> _grandchildren;
        public List<Spec_Category> grandchildren
        {
            get
            {
                if (isChange || _grandchildren == null)
                {
                    spGetSpecByCategoryHelper helper = new spGetSpecByCategoryHelper();
                    var idLs = helper.getIdList("Spec_Category", "CATEGORY_NODE", "CATEGORY_ID", this.CATEGORY_ID, 2);
                    _grandchildren = (new Spec_CategoryHelper()).getCategoryListByIds(idLs);
                }
                return _grandchildren;
            }
            set { _grandchildren = value; }
        }

        private Spec_Category _parentCategory;
        public Spec_Category ParentCategory
        {
            get 
            {
                if (_parentCategory == null)
                {
                    spGetSpecByCategoryHelper helper = new spGetSpecByCategoryHelper();
                    var idLs = helper.getIdList("Spec_Category", "CATEGORY_NODE", "CATEGORY_ID", this.CATEGORY_ID, -1);
                    if (idLs.Any())
                        _parentCategory = (new Spec_CategoryHelper()).getSpecCategoryById(idLs.FirstOrDefault());
                    if (_parentCategory == null)
                        _parentCategory = new Spec_Category() { CATEGORY_ID = 0, CONDITIONS = "And" };
                }
                return _parentCategory; 
            }
        }


        public Spec_Category Parent
        {
            get;
            set;
        }

        /// <summary>
        /// get category all products
        /// </summary>
        /// <param name="storeid"></param>
        /// <returns></returns>
        public List<Product> getProducts(string storeid)
        {
            return (new Spec_CategoryHelper()).getProductByCategory(this, storeid);
        }


        /// <summary>
        /// categroy display type
        /// </summary>
        public DisType displayType
        {
            get
            {
                switch (this.DisplayType)
                { 
                    case "List":
                        return DisType.List;
                    case "CheckBoxList":
                        return DisType.CheckBoxList;                    
                    case "RadioButtonList":
                        return DisType.RadioButtonList;
                    case "TextBox":
                        return DisType.TextBox;
                    case "DoubleList":
                        return DisType.DoubleList;
                    case "DoubleRow":
                        return DisType.DoubleRow;
                    case "DropdownList":
                    default:
                        return DisType.DropdownList;
                }
            }
        }

        public CateType categoryType
        {
            get
            {
                switch (this.CATEGORY_TYPE)
                {
                    case "BUNode":
                        return CateType.BUNode;
                    case "Node":
                        return CateType.Node;
                    case "Root":
                        return CateType.Root;
                    case "Value":
                    default:
                        return CateType.Value;
                }
            }
        }

        public CateContion categoryContion
        {
            get
            {
                switch (this.CONDITIONS)
                {
                    case "Or":
                        return CateContion.Or;
                    case "And":
                    default:
                        return CateContion.And;
                }
            }
        }

        private string storeid;
        public string StoreID
        {
            get
            {
                return this.storeid;
            }
            set
            {
                this.storeid = value;
            }
        }

        private string translation_name;
        public string Translation_Name
        {
            get
            {
                if (string.IsNullOrEmpty(this.translation_name))
                {
                    var lang = this.Spec_Category_Lang.FirstOrDefault(p => p.StoreID == this.StoreID);
                    if (lang != null)
                        this.translation_name = lang.Local_Displayname;
                    else
                        this.translation_name = this.CATEGORY_DISPLAYNAME;
                }
                return this.translation_name;
            }
        }

        private List<Part_Spec_V3> _allSpecParts = null;
        public List<Part_Spec_V3> allSpecParts
        {
            get 
            {
                if (_allSpecParts == null)
                    _allSpecParts = getSpecParts(this);

                return _allSpecParts.OrderBy(x => x.SEQUENCE).ToList(); 
            }
            set { _allSpecParts = value; }
        }

        public void prefetchPartList(string storeid)
        {
            PartHelper parthelper = new PartHelper();
            List<Part> prefetched = parthelper.prefetchPartList(storeid, allSpecParts.Select(x => x.PART_NO).ToList());
            foreach (var s in allSpecParts)
                s.part = prefetched.FirstOrDefault(c => c.SProductID.Equals(s.PART_NO, StringComparison.OrdinalIgnoreCase));
        }


        private List<Part_Spec_V3> getSpecParts(Spec_Category categroy)
        {
            List<Part_Spec_V3> ls = new List<Part_Spec_V3>();
            ls.AddRange(categroy.Part_Spec_V3);
            if (categroy.children.Any())
            {
                foreach (var c in categroy.children)
                {
                    c.StoreID = categroy.StoreID;
                    var sublist = getSpecParts(c);
                    foreach (var p in sublist)
                    {
                        if (string.IsNullOrEmpty(p.StoreID) || p.StoreID == c.StoreID)
                        {
                            if (ls.FirstOrDefault(t => t.PART_NO.Equals(p.PART_NO, StringComparison.OrdinalIgnoreCase)) == null)
                                ls.Add(p);
                        }
                    }
                }
            }
            return ls;
        }

    }
}
