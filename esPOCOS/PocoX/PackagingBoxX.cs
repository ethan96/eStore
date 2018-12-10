using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eStore.POCOS
{
    public partial class PackagingBox
    {
        /// <summary>
        /// Box information
        /// </summary>
        #region Attributes
        public string BoxId
        {
            get;
            set;
        }
        
        //Box's desceription
        public string PackageDesc
        {
            get;
            set;
        }        
        
        //Max capacity of the packaging box, Qty of items
        public int BoxCapacity
        {
            get;
            set;
        }

        //Remain capacity of the packaging box, Qty of items
        public int RemainCapacity
        {
            get;
            set;
        }

        //Packaging Box's dimension and weight
        private MeasureUnit _measure;
        public MeasureUnit Measure
        {
            get
            {
                return _measure;
            }
            set
            {
                _measure = value;
                this.Length = _measure.Length;
                this.Width = _measure.Width;
                this.Height = _measure.Height;
                this.Weight = (_measure.Weight < 0.5m) ? 0.5m : _measure.Weight;    //minimun weight is 0.5 (KG or LB)
                this.DimensionUnit = _measure.DimensionUnit;
                this.WeightUnit = _measure.WeightUnit;
            }
        }

        //Packaging Box's dimensional Weight
        private decimal _dimensionalWeight;
        public decimal DimensionalWeight
        {
            get { return _dimensionalWeight; }
        }

        private decimal _dimensionWeightBase;
        public decimal DimensionWeightBase
        {
            get { return _dimensionWeightBase; }
        }

        //Main shipping cost base,  2 type, Dimensional Weight Base, Actual Weight
        public enum shippingCostBase {DimensionalWeight, ActualWeight};
        public shippingCostBase ShippingCostBase
        {
            get
            {
                if (DimensionalWeight > this.Weight)
                    return shippingCostBase.DimensionalWeight;
                else
                    return shippingCostBase.ActualWeight;
            }
        }

        //Number of items in the packaging box
        public int QtyOfItemInBox
        {
            get;
            set;
        }        

        //Insured value, the total price of the items in the packaging box
        public decimal InsuredValue
        {
            get;
            set;
        }

        //Insured currency
        public string InsuredCurrency
        {
            get;
            set;
        }

        //Allow other items merge into this box
        public bool AllowToMerge
        {
            get 
            {
                if (RemainCapacity > 0)
                    return true;
                else
                    return false;
            }
        }
        #endregion
        

        #region Methods
        /// <summary>
        /// Default constructor usualy is used by entity framework
        /// </summary>
        public PackagingBox()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="measure"></param>
        /// <param name="dimensionWeightBase"></param>
        public PackagingBox(MeasureUnit measure, decimal dimensionWeightBase = 0)
        {
            this.Measure = measure;
            if (dimensionWeightBase > 0)
                calculateDimensionWeight(dimensionWeightBase);
        }
   
        /// <summary>
        /// This method can calculate the dimension weight of a box.
        /// </summary>
        /// <param name="dimensionWeightBase"></param>
        public void calculateDimensionWeight(decimal dimensionWeightBase)
        {
            _dimensionWeightBase = dimensionWeightBase;
            _dimensionalWeight = Math.Round(this.Length * this.Width * this.Height / dimensionWeightBase,3);
        }
        #endregion
    }
}