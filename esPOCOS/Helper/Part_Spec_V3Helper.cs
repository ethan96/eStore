using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS;
using eStore.Utilities;

namespace eStore.POCOS.DAL
{
    public class Part_Spec_V3Helper : Helper
    {

        public List<Part_Spec_V3> getAllSpec()
        {
            return context.Part_Spec_V3.ToList();
        }


        public List<Product> getProductList(Part_Spec_V3 spec, string storeid)
        {
            if (spec == null || string.IsNullOrEmpty(storeid))
                return new List<Product>();

            var ls = (from c in context.Parts.OfType<Product>()
                      where c.SProductID == spec.PART_NO && c.StoreID == storeid
                      select c).ToList();

            return ls;
        }

        public List<Part_Spec_V3> getPartSpecV3ListbyIds(List<int> ids)
        {
            var ls = (from c in context.Part_Spec_V3
                     where ids.Contains(c.CATEGORY_ID)
                     select c).ToList();
            return ls;
        }

        public Part_Spec_V3 getPartSpecV3(int id, string partno)
        {
            var p = (from c in context.Part_Spec_V3
                    where c.CATEGORY_ID == id && c.PART_NO == partno
                    select c).FirstOrDefault();
            return p;
        }

        internal int save(Part_Spec_V3 part)
        {
            if (part == null || string.IsNullOrEmpty(part.PART_NO) || part.validate() == false)
                return 1;

            try
            {
                var p = (from c in context.Part_Spec_V3
                         where c.CATEGORY_ID == part.CATEGORY_ID && c.PART_NO == part.PART_NO
                         select c).FirstOrDefault();

                if (p == null)
                {
                    context.Part_Spec_V3.AddObject(part);
                    context.SaveChanges();
                }
                else
                {
                    p.PART_NO = part.PART_NO;
                    p.LAST_UPDATED_BY = part.LAST_UPDATED_BY;
                    p.LAST_UPDATED_DATE = part.LAST_UPDATED_DATE;
                    p.SEQUENCE = part.SEQUENCE;
                    context.Part_Spec_V3.ApplyCurrentValues(p);
                    context.SaveChanges();
                }

                return 0;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }
        }

        internal int delete(Part_Spec_V3 part)
        {
            if (part == null || string.IsNullOrEmpty(part.PART_NO))
                return 1;

            try
            {
                var p = (from c in context.Part_Spec_V3
                         where c.CATEGORY_ID == part.CATEGORY_ID && c.PART_NO == part.PART_NO
                         select c).FirstOrDefault();

                if (p != null)
                {
                    context.Part_Spec_V3.DeleteObject(p);
                    context.SaveChanges();
                }
                return 0;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }
        }

    }
}
