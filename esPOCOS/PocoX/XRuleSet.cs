
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using eStore.POCOS.PocoX;
using System.Linq;

namespace eStore.POCOS
{
    public partial class XRuleSet
    {
        public enum XRuleSetOperator { Equal, StartWith, EndWith, Contains }
        public enum XRuleSetType { Group, Product, ProductCateogory, ProductType }
        public enum XRuleSetCheckLevel { Cart, CartItem }

        public enum XRuleSetConjoinBy { And, Or }
        public enum XRuleSetExpectResult { True, False }
        public enum XRuleSetProductType { All, StandardProduct, System, Bundle }

        private XRuleSetCheckLevel _checkLevel = XRuleSetCheckLevel.CartItem;
        public XRuleSetCheckLevel checkLevel
        {
            get
            {
                if(!string.IsNullOrEmpty(CheckLevel))
                    Enum.TryParse<XRuleSetCheckLevel>(CheckLevel, out _checkLevel);
                return _checkLevel;
            }
            set
            {
                CheckLevel = value.ToString();
                _checkLevel = value;
            }
        }
        private XRuleSetOperator _operatorX = XRuleSetOperator.Equal;
        public XRuleSetOperator operatorX
        {
            get
            {

                if (!string.IsNullOrEmpty(Operator))
                {
                    Enum.TryParse<XRuleSetOperator>(Operator, out _operatorX);
                }
                return _operatorX;

            }
            set
            {
                Operator = value.ToString();
                _operatorX = value;
            }
        }

        private XRuleSetExpectResult _exceptResultX = XRuleSetExpectResult.True;
        public XRuleSetExpectResult exceptResultX
        {
            get
            {
                return _exceptResultX;
            }
            set
            {
                _exceptResultX = value;
            }
        }

        private XRuleSetType _typeX = XRuleSetType.Product;
        public XRuleSetType typeX
        {
            get
            {
                if (!string.IsNullOrEmpty(this.RuleSetType))
                {
                    Enum.TryParse<XRuleSetType>(RuleSetType, out _typeX);
                }
                return _typeX;
            }
            set
            {
                RuleSetType = value.ToString();
                _typeX = value;
            }
        }

        private XRuleSetConjoinBy _conjoinBy = XRuleSetConjoinBy.And;
        public XRuleSetConjoinBy conjoinBy
        {
            get
            {
                if (!string.IsNullOrEmpty(this.JoinSubRulesetBy))
                {
                    Enum.TryParse<XRuleSetConjoinBy>(JoinSubRulesetBy, out _conjoinBy);
                }
                return _conjoinBy;
            }
            set
            {
                JoinSubRulesetBy = value.ToString();
                _conjoinBy = value;
            }
        }




        public bool isMatched(Part part)
        {
            bool isMatchTerm = false;
            switch (typeX)
            {
                case XRuleSetType.Product:
                    switch (operatorX)
                    {
                        case XRuleSetOperator.Equal:
                            isMatchTerm = part.name.Equals(MatchTerm);
                            break;
                        case XRuleSetOperator.StartWith:
                            isMatchTerm = part.name.StartsWith(MatchTerm);
                            break;
                        case XRuleSetOperator.EndWith:
                            isMatchTerm = part.name.EndsWith(MatchTerm);
                            break;
                        case XRuleSetOperator.Contains:
                            isMatchTerm = part.name.Contains(MatchTerm);
                            break;
                        default:
                            break;
                    }

                    break;
                case XRuleSetType.ProductCateogory:

                    isMatchTerm = applicableProductIDS.Contains(part.name);

                    break;
                case XRuleSetType.ProductType:
                    isMatchTerm = getXRuleSetProductType(part).ToString() == MatchTerm;
                    break;
                default:
                    break;
            }
            return isMatchTerm;
        }
        private XRuleSetProductType getXRuleSetProductType(Part part)
        {
            XRuleSetProductType itemtype = XRuleSetProductType.All;
            if (part is Product_Ctos)
                itemtype = XRuleSetProductType.System;
            else if (part is Product_Bundle)
                itemtype = XRuleSetProductType.Bundle;
            else if (part is Product)
                itemtype = XRuleSetProductType.StandardProduct;
            return itemtype;
        }

        public bool isMatched(Dictionary<Part, int> term)
        {
            bool rlt = false;
            foreach (var item in term)
            {
                if (isMatched(item.Key) && (UnitQty == 0 || item.Value <= UnitQty))
                {
                    rlt = true;
                    break;
                }

            }
            return rlt;
        }
     

       
        private String _mutex = "MUTEX";

        private HashSet<string> _applicableProductIDS;
        public HashSet<string> applicableProductIDS
        {
            get
            {
                lock (_mutex)
                {
                    if (_applicableProductIDS == null)
                    {

                        _applicableProductIDS = new HashSet<string>();

                        List<String> products = (new DAL.ProductCategoryHelper()).getAllProductsInHierarchicalCategories(this.StoreID, this.MatchTerm);
                        foreach (string p in products)
                            _applicableProductIDS.Add(p);

                    }
                }
                return _applicableProductIDS;
            }
        }


        public bool isLeaf()
        {
            return this.typeX != XRuleSetType.Group;
        }

    }
}
