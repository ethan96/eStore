using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;
namespace eStore.POCOS.DAL
{

    public partial class TagMappingHelper : Helper
    {

        #region Business Read
        #endregion

        #region Creat Update Delete 
        public int save(TagMapping _tagmapping)

        {
            //if parameter is null or validation is false, then return  -1 
            if (_tagmapping == null || _tagmapping.validate() == false) return 1;
            //Try to retrieve object from DB
            TagMapping _exist_tagmapping = null;
            _exist_tagmapping = (from tm in context.TagMappings
                                 where tm.StoreID == _tagmapping.StoreID
                                 && tm.TagId == _tagmapping.TagId
                                 && tm.EntityIntKey == _tagmapping.EntityIntKey
                                 && tm.EntityType == _tagmapping.EntityType
                                 && tm.EntityStringKey == _tagmapping.EntityStringKey
                                 select tm).ToList().FirstOrDefault();
            try
            {
                if (_exist_tagmapping == null)  //object not exist 
                {
                    //Insert
                    context.TagMappings.AddObject(_tagmapping);
                    context.SaveChanges();
                    return 0;
                }
                else

                {
                    //Update
                    //context.TagMappings.ApplyCurrentValues(_tagmapping);
                    //context.SaveChanges();
                    //return 0;
                    return 0;
                }
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }
        }

        public List<TagMapping> getTagMappingByEntityidStoreidEntitytype(int entityid, string storeid,string entitytype)
        {
            return (from tm in context.TagMappings
                    where tm.EntityIntKey == entityid
                    && tm.StoreID == storeid
                    &&tm.EntityType== entitytype
                    select tm).ToList();
        }

        public string getTagname(int id)
        {
            return (from tag in context.Tags
                    where tag.Id == id
                    select tag.TagName).ToList().FirstOrDefault();
        }
        public int delete(TagMapping _tagmapping)
        {

            if (_tagmapping == null || _tagmapping.validate() == false) return 1;
            try
            {
                context.DeleteObject(_tagmapping);
                context.SaveChanges();
                return 0;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }
        }
        public int deleteById(int id)
        {
            TagMapping tagmp = new TagMapping();
            tagmp= (from tm in context.TagMappings
                    where tm.Id == id
                    select tm).ToList().FirstOrDefault();

            if (tagmp == null || tagmp.validate() == false) return 1;
            try
            {
                context.DeleteObject(tagmp);
                context.SaveChanges();
                return 0;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }
        }
        #endregion
    }
}