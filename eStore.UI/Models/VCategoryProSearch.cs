using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eStore.UI.Models
{
    public class VCategoryProSearch
    {
        
        public string Id { get; set; }
        public int Page { get; set; }
        public int Pagesize { get; set; }
        public string SortType { get; set; }
        public string FilterType { get; set; }
        public bool MatrixPage { get; set; }
        public bool Paps { get; set; }
        public List<VCategoryAttr> CategoryAttrs { get; set; }

        public VCategoryProSearch()
        {
            CategoryAttrs = new List<VCategoryAttr>();
        }
    }

    public class VCategoryAttr
    {
        public string Id
        {
            get
            {
                return $"{CateId}-{AttrId}-{ValueId}";
            }
        }
        public int CateId { get; set; }
        public int AttrId { get; set; }
        public int ValueId { get; set; }
        public VCategoryAttr()
        { }
        public VCategoryAttr(ProductMatrix matrix)
        {
            this.CateId = matrix.CatID;
            this.AttrId = matrix.AttrId;
            this.ValueId = matrix.AttrValueId;
        }
    }
    

    public class VCategoryFilterResult
    {
        public int Count { get; set; }
        public IEnumerable<Models.Product> Products { get; set; }
        public List<VCategoryAttr> ProductAttrs { get; set; }
        public string SortType { get; set; }

        public VCategoryFilterResult() { }
    }

    public class AttrGP_Result
    {
        public AttrGP_Result()
        {
            Values = new List<VCategoryAttr>();
        }
        public int CateId { get; set; }
        public int AttrId { get; set; }
        public List<VCategoryAttr> Values { get; set; }

        public bool IsMach { get; set; }
    }
}