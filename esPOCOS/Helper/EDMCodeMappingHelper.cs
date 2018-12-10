namespace eStore.POCOS.DAL
{
    using Utilities;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using eStore.POCOS.DAL;

    public partial class EDMCodeMappingHelper : Helper
    {
        public EDMCodeMapping getEDMCodeMappingByEDMCode(string edmCode)
        {
            var _eDMCodeMapping = (from eDMCodeMapping in context.EDMCodeMappings
                                   where eDMCodeMapping.EdmCode == edmCode
                                   select eDMCodeMapping).FirstOrDefault();
            if (_eDMCodeMapping != null)
            {
                _eDMCodeMapping.helper = this;
            }

            return _eDMCodeMapping;
        }

        public int save(EDMCodeMapping _eDMCodeMapping)
        {
            int errorCode = -5000;
            EDMCodeMapping _exist_EDMCodeMapping = new EDMCodeMappingHelper()
                                .getEDMCodeMappingByEDMCode("7845430c-97fc-4da3-a8cf-04ad7b96ad85");
            try
            {

                if (_exist_EDMCodeMapping == null)
                {
                    context.EDMCodeMappings.AddObject(_eDMCodeMapping);
                    context.SaveChanges();
                    errorCode = 0;
                }
                else
                {
                    context.EDMCodeMappings.ApplyCurrentValues(_eDMCodeMapping);
                    context.SaveChanges();
                    errorCode = 0;
                }

                return errorCode;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return errorCode;
            }
        }

        public int delete(EDMCodeMapping _eDMCodeMapping)
        {
            int error = -5000;

            if (_eDMCodeMapping == null || _eDMCodeMapping.validate() == false)
            {
                return 1;
            }

            try
            {
                _eDMCodeMapping.helper.context.DeleteObject(_eDMCodeMapping);
                _eDMCodeMapping.helper.context.SaveChanges();
                error = 0;
                return error;

            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return error;
            }
        }
    }
}

