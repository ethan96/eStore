namespace eStore.POCOS
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public partial class MISUMI
    {
        #region Product identifiers
        //Required
        public string model_number { get; set; } //SproductID
        public string part_number { get; set; } //PartID
        #endregion

        #region Price and availability
        public string status { get; set; } //Stock status
        #endregion
        public string model_description { get; set; } //Product description
        public string model_Feature { get; set; } //Product Feature
        public string InventoryLevel { get; set; }
        public string StoreUrl { get; set; }
        public decimal? grossWeight { get; set; } //gross Weights
        public decimal? netWeight { get; set; } //net Weights
        public string dimensions { get; set; } //Product Feature
        public string DataSheet { get; set; } //Product Feature
    }
}
