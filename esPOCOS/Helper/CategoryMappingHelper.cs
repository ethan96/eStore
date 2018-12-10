using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;

namespace eStore.POCOS.DAL
{
    public partial class CategoryMappingHelper : Helper
    {
        /// <summary>
        /// 根据CategoryID找到关联的所有Category的ID
        /// </summary>
        /// <param name="CategoryID">CategoryID</param>
        /// <returns></returns>
        public List<int> getCategoryAssociationID(int CategoryID)
        {
            List<int> categoryAssociationId = (from c in context.CategoryMappings
                                               where c.FirstCategoryId == CategoryID 
                                               select c.SecondCategoryId).ToList();
            return categoryAssociationId;
        }

        /// <summary>
        /// 根据一个CategoryID找到所有FirstCategoruId为这个CategoryID的CategoryMapping项
        /// </summary>
        /// <param name="FirstCategoryID"></param>
        /// <returns></returns>
        public List<CategoryMapping> getAllCategoryMappingsByCategoryID(int CategoryID)
        {
            List<CategoryMapping> allCategoryMappings = (from c in context.CategoryMappings
                                                         where c.FirstCategoryId == CategoryID 
                                                         select c).ToList();
            return allCategoryMappings;
        }

        /// <summary>
        /// 根据两个关联的CategoryID获取CategoryMapping项
        /// </summary>
        /// <param name="FirstCategoryID"></param>
        /// <param name="SecondCategoryID"></param>
        /// <returns></returns>
        public CategoryMapping getCategoryMappingByCategoryID(int FirstCategoryID, int SecondCategoryID)
        {
            CategoryMapping cm = context.CategoryMappings.FirstOrDefault(c => c.FirstCategoryId == FirstCategoryID && c.SecondCategoryId == SecondCategoryID);
            return cm;
        }

        internal int save(CategoryMapping categoryMapping)
        {
            //if parameter is null or validation is false, then return  -1 
            if (categoryMapping == null || categoryMapping.validate() == false) return 1;
            //Try to retrieve object from DB
            CategoryMapping _exist_categoryMapping = getCategoryMapping(categoryMapping.Id);
            try
            {
                if (_exist_categoryMapping == null)  //object not exist 
                {
                    //Insert
                    context.CategoryMappings.AddObject(categoryMapping);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    //Update
                    context.CategoryMappings.ApplyCurrentValues(categoryMapping);
                    context.SaveChanges();
                    return 0;
                }
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }
        }

        public int delete(CategoryMapping categoryMapping)
        {

            if (categoryMapping == null || categoryMapping.validate() == false) return 1;
            POCOS.CategoryMapping cm = (from c in context.CategoryMappings
                                            where c.Id == categoryMapping.Id
                                            select c).FirstOrDefault();
            try
            {
                context = categoryMapping.helper.context;
                context.DeleteObject(cm);
                context.SaveChanges();
                return 0;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }
        }

        /// <summary>
        /// 通过categoruMappingID找到CategoryMapping项
        /// </summary>
        /// <param name="categoryMappingID"></param>
        /// <returns></returns>
        public CategoryMapping getCategoryMapping(int categoryMappingID)
        {
            CategoryMapping cm = context.CategoryMappings.FirstOrDefault(c => c.Id == categoryMappingID);
            return cm;
        }
    }
}
