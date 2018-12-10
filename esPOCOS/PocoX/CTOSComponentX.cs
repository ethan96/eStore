using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eStore.POCOS
{
    public partial class CTOSComponent:IComparable 
    {
 
        public enum IntegrityCheckType { None, OS, Storage , SoftwareSubscription }
         
        private eStore.POCOS.CTOSBOM.COMPONENTTYPE _type = eStore.POCOS.CTOSBOM.COMPONENTTYPE.UNKNOWN;
        public eStore.POCOS.CTOSBOM.COMPONENTTYPE type
        {
            get 
            {
                switch (this.ComponentType.ToLower())
                {
                    case "list":
                        _type = eStore.POCOS.CTOSBOM.COMPONENTTYPE.OPTION;
                        break;
                    case "category":
                    case "extendedcategory":
                        _type = eStore.POCOS.CTOSBOM.COMPONENTTYPE.CATEGORY;
                        break;
                    case "addoncards":
                    case "extendedaddoncards":
                        _type = eStore.POCOS.CTOSBOM.COMPONENTTYPE.ADDONCARDS;
                        break;
                    case "addonmodules":
                    case "extendedaddonmodules":
                        _type = eStore.POCOS.CTOSBOM.COMPONENTTYPE.ADDONMODULES;
                        break;
                    case "addondevices":
                    case "extendedaddondevices":
                        _type = eStore.POCOS.CTOSBOM.COMPONENTTYPE.ADDONDEVICES;
                        break;
                    case "accessories"://this is get from cbom
                    case "extendedaccessories"://this get from standard product category
                        _type = eStore.POCOS.CTOSBOM.COMPONENTTYPE.ACCESSORIES;
                        break;
                    default:
                        if (this.ComponentParentID == null)
                            _type = CTOSBOM.COMPONENTTYPE.CATEGORY;
                        else
                            _type = CTOSBOM.COMPONENTTYPE.OPTION;
                        break;
                }
                return _type; 
            }
        }
        public bool isExtentedFromProductCategory()
        {
            return !string.IsNullOrEmpty(ComponentType) && isCategory() && ComponentType.StartsWith("extended", true, null);
        }
        public bool isCategory()
        {
            return type == eStore.POCOS.CTOSBOM.COMPONENTTYPE.CATEGORY
                    || type == eStore.POCOS.CTOSBOM.COMPONENTTYPE.ADDONCARDS
                    || type == eStore.POCOS.CTOSBOM.COMPONENTTYPE.ADDONMODULES
                    || type == eStore.POCOS.CTOSBOM.COMPONENTTYPE.ADDONDEVICES
                    || type == eStore.POCOS.CTOSBOM.COMPONENTTYPE.ACCESSORIES;
        }

        public string ComponentTypeX
        { 
            get
            {
                if (isExtentedFromProductCategory())
                    return ComponentType.Replace("extended", "");
                else
                    return ComponentType;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IntegrityCheckType integrityCheckTypeX
        {
            get
            {
                if (string.IsNullOrEmpty(CategoryClassification))
                    return IntegrityCheckType.None;
                var _type = IntegrityCheckType.None;
                Enum.TryParse(CategoryClassification, out _type);
                return _type;
            }
        }

        // ctos component image ico
        public virtual string thumbnail
        {
            get
            {
                if (string.IsNullOrEmpty(this.Thumbnail))
                    return "";
                if (this.Thumbnail.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
                    return this.Thumbnail;
                else
                    return "/resource/" + this.Thumbnail;
            }
        }

        public bool copyFrom(CTOSComponent source, Store targetStore, int parentid = -1)
        {
            this.ComponentName = source.ComponentName;
            this.ComponentDesc = source.ComponentDesc;
            if (parentid != -1)
            {
                this.ComponentParentID = parentid;
                this.ComponentType = "list";
            }
            else
                this.ComponentType = "category";
            this.DefaultSeq = source.DefaultSeq;
            this.MainPart = source.MainPart;
            this.CTOSComponentMappingsTo.Add(new CTOSComponentMapping()
            {
                StoreIDFrom = source.StoreID,
                ComponentIDFrom = source.ComponentID,
                StoreIDTo = this.StoreID
            });
            if (source.CTOSComponentDetails.Any())
            {
                foreach (var tail in source.CTOSComponentDetails)
                {
                    if (!string.IsNullOrEmpty(tail.SProductID))
                    {
                        eStore.POCOS.DAL.PartHelper partHelper = new eStore.POCOS.DAL.PartHelper();
                        var _p = partHelper.getPart(tail.SProductID, this.StoreID);
                        if (_p == null)
                            _p = partHelper.AddParttoStore(tail.SProductID, targetStore);
                        if (_p != null)
                        {
                            this.CTOSComponentDetails.Add(new CTOSComponentDetail()
                            {
                                DefaultSeq = tail.DefaultSeq,
                                ComponentDesc = _p.productDescX,
                                SProductID = _p.SProductID,
                                Qty = tail.Qty,
                                StoreID = targetStore.StoreID
                            });
                        }
                    }
                }
            }
            return this.save() == 0;
        }

        public int CompareTo(object obj)
        {
            if (obj is CTOSComponent)
            {
                return CompareTo(obj as CTOSComponent);
            }
            else
                return -1;
        }
        public int CompareTo(CTOSComponent component)
        {
            int result = 0;
            if (this.type != component.type)
                return -1;

            if (this.isCategory())
            {
                if ( this.ComponentName !=component.ComponentName)
                {
                    return -1;
                }
            }
            else if (this.isExtentedFromProductCategory())
            {
                if (this.ComponentName != component.ComponentName)
                {
                    return -1;
                }
                if (this.SProductID != component.SProductID)
                {
                    return -1;
                }
            
            }
            else if (component.CTOSComponentDetails.Count == this.CTOSComponentDetails.Count)
            {
                if (component.CTOSComponentDetails.Count == 0)
                {
                    if (this.ComponentName != component.ComponentName)
                    {
                        return -1;
                    }
                }
                else
                {
                    foreach (CTOSComponentDetail detail in this.CTOSComponentDetails)
                    {
                        if (!component.CTOSComponentDetails.Any(x => x.SProductID == detail.SProductID && x.Qty == detail.Qty))
                            return -1;
                    }
                }
            }
            else
            {
                return -1;
            }

            return result;
        }

        private Part _firstPart;
        public Part firstPart
        {
            get
            {
                POCOS.CTOSComponent optionDetail = null;
                if (this.isExtentedFromProductCategory())
                    optionDetail = this.CTOSComponent1.FirstOrDefault(c => c.CTOSComponentDetails != null && c.CTOSComponentDetails.Any());
                else
                    optionDetail = this;
                if (optionDetail != null && optionDetail.CTOSComponentDetails != null && optionDetail.CTOSComponentDetails.Any())
                {
                    eStore.POCOS.DAL.PartHelper partHelper = new eStore.POCOS.DAL.PartHelper();
                    _firstPart = partHelper.getPart(optionDetail.CTOSComponentDetails.FirstOrDefault().SProductID, this.StoreID);
                }
                return _firstPart;
            }
        }

        private List<string> _partids = null;
        public List<string> Partids
        {
            get 
            {
                if (_partids == null && this.CTOSComponentDetails != null)
                    _partids = this.CTOSComponentDetails.Select(c => c.SProductID).ToList();
                return _partids; 
            }
        }


        public bool isRootComponent
        {
            get
            {
                return !this.ComponentParentID.HasValue;
            }
        }

        public CTOSComponent rootComponent
        {
            get
            {
                if (this.isRootComponent)
                    return this;
                else
                {
                    return this.CTOSComponent2.rootComponent;
                }
            }
        }
    }
}
