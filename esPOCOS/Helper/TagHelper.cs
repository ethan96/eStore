using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;
namespace eStore.POCOS.DAL
{

    public partial class TagHelper : Helper
    {

        #region Business Read

        public List<int> getTagsByKey(string storeId, string type, int key)
        {
            return (from tm in context.TagMappings
                    where tm.StoreID == storeId
                    && tm.EntityType == type
                    && tm.EntityIntKey == key
                    select tm.TagId
                    ).ToList();
        }
        public List<Tag> getTags()
        {
            return (from tm in context.Tags
                    select tm
                    ).ToList();
        }
        public List<Tag> getTagsByStoreId(string storeId)
        {
            return (from tm in context.Tags
                    where tm.StoreID==storeId
                    select tm
                    ).ToList();
        }


        public List<int> getTagsByKey(string storeId, string type, string key)
        {
            return (from tm in context.TagMappings
                    where tm.StoreID == storeId
                    && tm.EntityType == type
                    && tm.EntityStringKey == key
                    select tm.TagId
                 ).ToList();
        }
        public List<TagMapping> getTagMappings(string storeId, string type)
        {
            return (from tm in context.TagMappings
                    where tm.StoreID == storeId
                    && tm.EntityType == type
                    select tm
                 ).ToList();
        }
        public void addTags(string storeId, string type, string key, List<int> tagIds)
        {
            bool hasnewTags = false;
            foreach (var id in tagIds)
            {
                if (!context.TagMappings.Any(tm => tm.StoreID == storeId
                     && tm.EntityType == type
                     && tm.EntityStringKey == key
                     && tm.TagId == id
                     ))
                {
                    hasnewTags = true;
                    var newtag = new TagMapping() { StoreID = storeId, EntityIntKey = 0, EntityStringKey = key, EntityType = type, TagId = id };
                    context.TagMappings.AddObject(newtag);
                }
            }

            if (hasnewTags)
                context.SaveChanges();
        }

        public void addTags(string storeId, string type, int key, List<int> tagIds)
        {
            bool hasnewTags = false;
            foreach (var id in tagIds)
            {
                if (!context.TagMappings.Any(tm => tm.StoreID == storeId
                     && tm.EntityType == type
                     && tm.EntityIntKey == key
                     && tm.TagId == id
                     ))
                {
                    hasnewTags = true;
                    var newtag = new TagMapping() { StoreID = storeId, EntityIntKey = key, EntityStringKey = "", EntityType = type, TagId = id };
                    context.TagMappings.AddObject(newtag);
                }
            }

            if (hasnewTags)
                context.SaveChanges();
        }

        public Tag get(int id)
        {
            return context.Tags.FirstOrDefault(x => x.Id == id);
        }

        public List<Tag> getByStore(string storeId)
        {
            return context.Tags.Where(x => x.StoreID == storeId && x.Status == true).ToList();
        }
        #endregion

        #region Creat Update Delete 
        public int save(Tag _tag)

        {
            //if parameter is null or validation is false, then return  -1 
            if (_tag == null || _tag.validate() == false) return 1;
            //Try to retrieve object from DB
            Tag _exist_tag = null;
            try
            {
                if (_exist_tag == null)  //object not exist 
                {
                    //Insert
                    context.Tags.AddObject(_tag);
                    context.SaveChanges();
                    return 0;
                }
                else

                {
                    //Update
                    context.Tags.ApplyCurrentValues(_tag);
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

        public int delete(Tag _tag)
        {

            if (_tag == null || _tag.validate() == false) return 1;
            try
            {
                context.DeleteObject(_tag);
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