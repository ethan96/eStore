// -----------------------------------------------------------------------
// <copyright file="ProductBatchStatusDal.cs" company="Adv">
// Product Batch Status internal x
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;

namespace eStore.POCOS
{
    

    /// <summary>
    /// Product Batch Status save and update
    /// </summary>
    public partial class ProductBatchStatu
    {
        private ProductBatchStatusHelper _helper = null;

        public ProductBatchStatusHelper helper
        {
            get { return _helper; }
            set { _helper = value; }
        }

        public int save()
        {
            if (_helper == null)
                _helper = new ProductBatchStatusHelper();
            return _helper.save(this);
        }

        public int delete()
        {
            if (_helper == null)
                _helper = new ProductBatchStatusHelper();
            return _helper.delete(this);
        }
    }
}
