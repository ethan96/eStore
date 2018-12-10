using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace eStore.POCOS
{
    public partial class ChangeLog
    {
        public ChangeLog(){}

        public ChangeLog(string userId, string moduleName, string docId, string active, string beforeUpdate, string afterUpdate,string storeid = "AUS")
        {
            this.UserID = userId;
            this.ModuleName = moduleName;
            this.CreatedDate= DateTime.Now;
            this.DocID = docId;
            this.Activity = active;
            this.BeforeUpdate = beforeUpdate;
            this.AfterUpdate = afterUpdate;
            this.StoreID = storeid;
        }

        public enum ModuleType
        {
            ProductCategory, ProductManagement, Orders, CtosProductCategory, ApplicationCategory,uStoreCategory,
            SyncJob, CTOSManagement, SOP
        };
        public enum ActiveType
        {
            Add_SubCategory,Add_CtosCategory, Delete_SubCategory,Delete_CtosCategory, Update_CategoryDetails,
            Update_CtosCategoryDetails, Add_CategoryProduct, Delete_CategoryProduct,Delete_SpecMask,
            Update_SpecMask, Delete_Product, Add_Product, Add_Local_Product,Update_Product, Update_Product_Publish, Update_Product_UnPublish, Add_ProductBaseType, Delete_Order,
            Update_OrderCartContact, Update_OrderContactBild, Update_OrderContactSold, Update_OrderContactShipping,
            Add_OrderItem, Delete_OrderItem, Update_OrderItem, Delete_OrderConfigDetail, Update_OrderConfigDetail, 
            Update_OrderSyncComment,Update_OrderSyncReseller,Add_OrderSyncToSAP,Update_OrderCreateCart,Delete_OrderPayment,
            Void_OrderPayment, Reauthorization_OrderPayment, Update_CreateERP, Delete_OrderBundleConfigDetail,Update_OrderBundleConfigDetail,
            Delete_ApplicationPoint
            ,ChnageDefaultOption,ChnagePromotionPrice,
            Upload_SOP,Delete_SOP

        };
    }
}
