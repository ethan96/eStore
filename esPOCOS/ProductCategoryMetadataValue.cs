//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using eStore.POCOS.PocoX;
 

namespace eStore.POCOS
{
    public partial class ProductCategoryMetadataValue
    {
        #region Primitive Properties
     
    	public List<ErrorMessage> error_message{
    		get;
    		set;
    	}
    
    
        public virtual int Id
        {
            get;
            set;
        }
    
        public virtual string FieldValue
        {
            get;
            set;
        }
    
        public virtual int ProductId
        {
            get { return _productId; }
            set
            {
                if (_productId != value)
                {
                    if (Product != null && Product.Id != value)
                    {
                        Product = null;
                    }
                    _productId = value;
                }
            }
        }
        private int _productId;
    
        public virtual int ProductCategoryMetadataId
        {
            get { return _productCategoryMetadataId; }
            set
            {
                if (_productCategoryMetadataId != value)
                {
                    if (ProductCategoryMetadata != null && ProductCategoryMetadata.Id != value)
                    {
                        ProductCategoryMetadata = null;
                    }
                    _productCategoryMetadataId = value;
                }
            }
        }
        private int _productCategoryMetadataId;

        #endregion

        #region Validation
    	
    	public bool validate() {
    		error_message = new List<ErrorMessage>();
    	   
    	   
    	   if(error_message.Count>0) {
    	   	return false;
    	   }else {
    	    return true;
    	   }
    	
    	}
    	
    	/* public void Write()
        {
            Type type = typeof(ProductCategoryMetadataValue); // Get type pointer
             PropertyInfo[] fields = type.GetProperties(); // Obtain all fields
                foreach (var pinfo in fields) // Loop through fields
                {
                    string name = pinfo.Name; // Get string name
                    object temp = pinfo.GetValue(pinfo.Name,null); // Get value
                if (temp is int) // See if it is an integer.
                {
                    int value = (int)temp;
                    Console.Write(name);
                    Console.Write(" (int) = ");
                    Console.WriteLine(value);
                }
                else if (temp is string) // See if it is a string.
                {
                    string value = temp as string;
                    Console.Write(name);
                    Console.Write(" (string) = ");
                    Console.WriteLine(value);
                }
            }
        }*/
     
    	
    	
        #endregion

        #region Navigation Properties
    
        public virtual ProductCategoryMetadata ProductCategoryMetadata
        {
            get { return _productCategoryMetadata; }
            set
            {
                if (!ReferenceEquals(_productCategoryMetadata, value))
                {
                    var previousValue = _productCategoryMetadata;
                    _productCategoryMetadata = value;
                    FixupProductCategoryMetadata(previousValue);
                }
            }
        }
        private ProductCategoryMetadata _productCategoryMetadata;
    
        public virtual PTDProduct Product
        {
            get { return _product; }
            set
            {
                if (!ReferenceEquals(_product, value))
                {
                    var previousValue = _product;
                    _product = value;
                    FixupProduct(previousValue);
                }
            }
        }
        private PTDProduct _product;

        #endregion

        #region Association Fixup
    
        private void FixupProductCategoryMetadata(ProductCategoryMetadata previousValue)
        {
            if (previousValue != null && previousValue.ProductCategoryMetadataValues.Contains(this))
            {
                previousValue.ProductCategoryMetadataValues.Remove(this);
            }
    
            if (ProductCategoryMetadata != null)
            {
                if (!ProductCategoryMetadata.ProductCategoryMetadataValues.Contains(this))
                {
                    ProductCategoryMetadata.ProductCategoryMetadataValues.Add(this);
                }
                if (ProductCategoryMetadataId != ProductCategoryMetadata.Id)
                {
                    ProductCategoryMetadataId = ProductCategoryMetadata.Id;
                }
            }
        }
    
        private void FixupProduct(PTDProduct previousValue)
        {
            if (previousValue != null && previousValue.ProductCategoryMetadataValues.Contains(this))
            {
                previousValue.ProductCategoryMetadataValues.Remove(this);
            }
    
            if (Product != null)
            {
                if (!Product.ProductCategoryMetadataValues.Contains(this))
                {
                    Product.ProductCategoryMetadataValues.Add(this);
                }
                if (ProductId != Product.Id)
                {
                    ProductId = Product.Id;
                }
            }
        }

        #endregion

    }
}
