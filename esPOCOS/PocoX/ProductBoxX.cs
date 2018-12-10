using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eStore.POCOS
{
    public partial class ProductBox: IComparable
    {
        //Max numbers of items in the box
        public virtual int Capacity
        {
            get;
            set;
        }

        private decimal? _volumn; 
        public Decimal volumn
        {
            get {
                if(!_volumn.HasValue)
                    _volumn=LengthINCH * WidthINCH * HighINCH;
                return _volumn.GetValueOrDefault();
            }
        }
        public Decimal volumnWeight
        {
            get { return volumn / 139; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"> A ProductBox object to compare.</param>
        /// <returns>
        ///         A signed number indicating the relative values of this instance and the value
        ///         parameter.Value Description Less than zero This instance is smaller than
        ///         value. Zero This instance is the same as value. Greater than zero This instance
        ///         is bigger than value.
        /// </returns>
        public int CompareTo(ProductBox value)
        {
            int rlt = 0;
            this.sortDimension();
            value.sortDimension();
            decimal[] diff = { this.WidthINCH - value.WidthINCH, this.LengthINCH - value.LengthINCH, this.HighINCH - value.HighINCH };
            if (this.WidthINCH > value.WidthINCH
                 || this.LengthINCH > value.LengthINCH
                 || this.HighINCH > value.HighINCH)
            {
                rlt = (int)Math.Ceiling(diff.Max());
            }
            else if (this.WidthINCH == value.WidthINCH
                 && this.LengthINCH == value.LengthINCH
                 && this.HighINCH == value.HighINCH)
            {

            }
            else
            {
                rlt = -(int)Math.Ceiling(diff.Max(x => Math.Abs(x)));
            }

            return rlt;
        }
      
        public int CompareTo(object value)
        {
            if (value is ProductBox)
            {
                return CompareTo((ProductBox)value);
            }
            else
            {
                throw new System.ArgumentException("value type must be ProductBox");
            }
        }
        private bool isStorted = false;
        public  void sortDimension()
        {
            if (!isStorted)
            {
                decimal[] dim = { this.LengthINCH, this.HighINCH, this.WidthINCH };
                dim = dim.OrderBy(x => x).ToArray();
                if (this.HighINCH != dim[0] ||
                this.LengthINCH != dim[1] ||
                this.WidthINCH != dim[2])
                {
                    this.HighINCH = dim[0];
                    this.LengthINCH = dim[1];
                    this.WidthINCH = dim[2];
                }
                isStorted = true;
            }
        }

    }
}
