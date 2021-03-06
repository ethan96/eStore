﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Data.Objects;
using System.Data.EntityClient;

namespace eStore.POCOS.Sync
{
    public partial class PISEntities : ObjectContext
    {
        public const string ConnectionString = "name=PISEntities";
        public const string ContainerName = "PISEntities";
    
        #region Constructors
    
        public PISEntities()
            : base(ConnectionString, ContainerName)
        {
            this.ContextOptions.LazyLoadingEnabled = true;
        }
    
        public PISEntities(string connectionString)
            : base(connectionString, ContainerName)
        {
            this.ContextOptions.LazyLoadingEnabled = true;
        }
    
        public PISEntities(EntityConnection connection)
            : base(connection, ContainerName)
        {
            this.ContextOptions.LazyLoadingEnabled = true;
        }
    
        #endregion
    
        #region ObjectSet Properties
    
        public ObjectSet<SAP_PRODUCT_STATUS> SAP_PRODUCT_STATUS
        {
            get { return _sAP_PRODUCT_STATUS  ?? (_sAP_PRODUCT_STATUS = CreateObjectSet<SAP_PRODUCT_STATUS>("SAP_PRODUCT_STATUS")); }
        }
        private ObjectSet<SAP_PRODUCT_STATUS> _sAP_PRODUCT_STATUS;
    
        public ObjectSet<PRODUCT> PRODUCTs
        {
            get { return _pRODUCTs  ?? (_pRODUCTs = CreateObjectSet<PRODUCT>("PRODUCTs")); }
        }
        private ObjectSet<PRODUCT> _pRODUCTs;
    
        public ObjectSet<PAP> PAPS
        {
            get { return _pAPS  ?? (_pAPS = CreateObjectSet<PAP>("PAPS")); }
        }
        private ObjectSet<PAP> _pAPS;
    
        public ObjectSet<V_Spec_V2> V_Spec_V2
        {
            get { return _v_Spec_V2  ?? (_v_Spec_V2 = CreateObjectSet<V_Spec_V2>("V_Spec_V2")); }
        }
        private ObjectSet<V_Spec_V2> _v_Spec_V2;
    
        public ObjectSet<SAP_PRODUCT> SAP_PRODUCT
        {
            get { return _sAP_PRODUCT  ?? (_sAP_PRODUCT = CreateObjectSet<SAP_PRODUCT>("SAP_PRODUCT")); }
        }
        private ObjectSet<SAP_PRODUCT> _sAP_PRODUCT;

        #endregion

        #region Function Imports
        public ObjectResult<sp_GetLiteratureInfo_Result> sp_GetLiteratureInfo(string iD)
        {
    
            ObjectParameter iDParameter;
    
            if (iD != null)
            {
                iDParameter = new ObjectParameter("ID", iD);
            }
            else
            {
                iDParameter = new ObjectParameter("ID", typeof(string));
            }
            return base.ExecuteFunction<sp_GetLiteratureInfo_Result>("sp_GetLiteratureInfo", iDParameter);
        }
        public ObjectResult<sp_GetLiteratureTable_Result> sp_GetLiteratureTable(string product_ID)
        {
    
            ObjectParameter product_IDParameter;
    
            if (product_ID != null)
            {
                product_IDParameter = new ObjectParameter("Product_ID", product_ID);
            }
            else
            {
                product_IDParameter = new ObjectParameter("Product_ID", typeof(string));
            }
            return base.ExecuteFunction<sp_GetLiteratureTable_Result>("sp_GetLiteratureTable", product_IDParameter);
        }
        public ObjectResult<sp_GetModelInfo_estore_Result> sp_GetModelInfo_estore(string model_ID, string lang_ID)
        {
    
            ObjectParameter model_IDParameter;
    
            if (model_ID != null)
            {
                model_IDParameter = new ObjectParameter("Model_ID", model_ID);
            }
            else
            {
                model_IDParameter = new ObjectParameter("Model_ID", typeof(string));
            }
    
            ObjectParameter lang_IDParameter;
    
            if (lang_ID != null)
            {
                lang_IDParameter = new ObjectParameter("Lang_ID", lang_ID);
            }
            else
            {
                lang_IDParameter = new ObjectParameter("Lang_ID", typeof(string));
            }
            return base.ExecuteFunction<sp_GetModelInfo_estore_Result>("sp_GetModelInfo_estore", model_IDParameter, lang_IDParameter);
        }
        public ObjectResult<sp_GetRelatedProducts_Result> sp_GetRelatedProducts(string pART_NO)
        {
    
            ObjectParameter pART_NOParameter;
    
            if (pART_NO != null)
            {
                pART_NOParameter = new ObjectParameter("PART_NO", pART_NO);
            }
            else
            {
                pART_NOParameter = new ObjectParameter("PART_NO", typeof(string));
            }
            return base.ExecuteFunction<sp_GetRelatedProducts_Result>("sp_GetRelatedProducts", pART_NOParameter);
        }
        public ObjectResult<spGetModelByPN_estore_Result> spGetModelByPN_estore(string iD1, string iD2, string pN)
        {
    
            ObjectParameter iD1Parameter;
    
            if (iD1 != null)
            {
                iD1Parameter = new ObjectParameter("ID1", iD1);
            }
            else
            {
                iD1Parameter = new ObjectParameter("ID1", typeof(string));
            }
    
            ObjectParameter iD2Parameter;
    
            if (iD2 != null)
            {
                iD2Parameter = new ObjectParameter("ID2", iD2);
            }
            else
            {
                iD2Parameter = new ObjectParameter("ID2", typeof(string));
            }
    
            ObjectParameter pNParameter;
    
            if (pN != null)
            {
                pNParameter = new ObjectParameter("PN", pN);
            }
            else
            {
                pNParameter = new ObjectParameter("PN", typeof(string));
            }
            return base.ExecuteFunction<spGetModelByPN_estore_Result>("spGetModelByPN_estore", iD1Parameter, iD2Parameter, pNParameter);
        }
        public ObjectResult<sp_GetProductFeature_Result> sp_GetProductFeature(string modelName, string lang_ID)
        {
    
            ObjectParameter modelNameParameter;
    
            if (modelName != null)
            {
                modelNameParameter = new ObjectParameter("ModelName", modelName);
            }
            else
            {
                modelNameParameter = new ObjectParameter("ModelName", typeof(string));
            }
    
            ObjectParameter lang_IDParameter;
    
            if (lang_ID != null)
            {
                lang_IDParameter = new ObjectParameter("Lang_ID", lang_ID);
            }
            else
            {
                lang_IDParameter = new ObjectParameter("Lang_ID", typeof(string));
            }
            return base.ExecuteFunction<sp_GetProductFeature_Result>("sp_GetProductFeature", modelNameParameter, lang_IDParameter);
        }
        public ObjectResult<sp_GetLiteratureTableByLANG_Result> sp_GetLiteratureTableByLANG(string model_ID, string iD1, string iD2, string lANG)
        {
    
            ObjectParameter model_IDParameter;
    
            if (model_ID != null)
            {
                model_IDParameter = new ObjectParameter("model_ID", model_ID);
            }
            else
            {
                model_IDParameter = new ObjectParameter("model_ID", typeof(string));
            }
    
            ObjectParameter iD1Parameter;
    
            if (iD1 != null)
            {
                iD1Parameter = new ObjectParameter("ID1", iD1);
            }
            else
            {
                iD1Parameter = new ObjectParameter("ID1", typeof(string));
            }
    
            ObjectParameter iD2Parameter;
    
            if (iD2 != null)
            {
                iD2Parameter = new ObjectParameter("ID2", iD2);
            }
            else
            {
                iD2Parameter = new ObjectParameter("ID2", typeof(string));
            }
    
            ObjectParameter lANGParameter;
    
            if (lANG != null)
            {
                lANGParameter = new ObjectParameter("LANG", lANG);
            }
            else
            {
                lANGParameter = new ObjectParameter("LANG", typeof(string));
            }
            return base.ExecuteFunction<sp_GetLiteratureTableByLANG_Result>("sp_GetLiteratureTableByLANG", model_IDParameter, iD1Parameter, iD2Parameter, lANGParameter);
        }
        public ObjectResult<getPublishedModelByDate_Result> getPublishedModelByDate(Nullable<System.DateTime> startDate, Nullable<System.DateTime> endDate)
        {
    
            ObjectParameter startDateParameter;
    
            if (startDate.HasValue)
            {
                startDateParameter = new ObjectParameter("StartDate", startDate);
            }
            else
            {
                startDateParameter = new ObjectParameter("StartDate", typeof(System.DateTime));
            }
    
            ObjectParameter endDateParameter;
    
            if (endDate.HasValue)
            {
                endDateParameter = new ObjectParameter("EndDate", endDate);
            }
            else
            {
                endDateParameter = new ObjectParameter("EndDate", typeof(System.DateTime));
            }
            return base.ExecuteFunction<getPublishedModelByDate_Result>("getPublishedModelByDate", startDateParameter, endDateParameter);
        }
        public ObjectResult<spGetModelByPN_estore_Result> spGetModelByPN_estore_NEW(string iD1, string iD2, string pN)
        {
    
            ObjectParameter iD1Parameter;
    
            if (iD1 != null)
            {
                iD1Parameter = new ObjectParameter("ID1", iD1);
            }
            else
            {
                iD1Parameter = new ObjectParameter("ID1", typeof(string));
            }
    
            ObjectParameter iD2Parameter;
    
            if (iD2 != null)
            {
                iD2Parameter = new ObjectParameter("ID2", iD2);
            }
            else
            {
                iD2Parameter = new ObjectParameter("ID2", typeof(string));
            }
    
            ObjectParameter pNParameter;
    
            if (pN != null)
            {
                pNParameter = new ObjectParameter("PN", pN);
            }
            else
            {
                pNParameter = new ObjectParameter("PN", typeof(string));
            }
            return base.ExecuteFunction<spGetModelByPN_estore_Result>("spGetModelByPN_estore_NEW", iD1Parameter, iD2Parameter, pNParameter);
        }
        public ObjectResult<sp_GetCertificateByModelID_Result> sp_GetCertificateByModelID(string model_ID)
        {
    
            ObjectParameter model_IDParameter;
    
            if (model_ID != null)
            {
                model_IDParameter = new ObjectParameter("model_ID", model_ID);
            }
            else
            {
                model_IDParameter = new ObjectParameter("model_ID", typeof(string));
            }
            return base.ExecuteFunction<sp_GetCertificateByModelID_Result>("sp_GetCertificateByModelID", model_IDParameter);
        }
        public ObjectResult<GetPISFullCategory_Result> GetPISFullCategory()
        {
            return base.ExecuteFunction<GetPISFullCategory_Result>("GetPISFullCategory");
        }
        public ObjectResult<sp_GetBBAllPartNo_Result> sp_GetBBAllPartNo()
        {
            return base.ExecuteFunction<sp_GetBBAllPartNo_Result>("sp_GetBBAllPartNo");
        }
        public ObjectResult<GetPISFullCategory_Result> GetPISFullBBCategory()
        {
            return base.ExecuteFunction<GetPISFullCategory_Result>("GetPISFullBBCategory");
        }

        #endregion

    }
}
