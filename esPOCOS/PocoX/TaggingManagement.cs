using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eStore.POCOS
{
    public class TaggingManagement
    {
        private static TaggingManagement intance;
        public static TaggingManagement getInstance()
        {
            if (intance == null)
                intance = new POCOS.TaggingManagement();

            return intance;
        }

        public List<int> getTagIds(string storeId, string type, object key)
        {
            List<int> ids = new List<int>();
            string cachekey = $"{storeId}.TagMappings.{type}";
            var tagMappings = CachePool.getInstance().getObject(cachekey);
            if (tagMappings == null)
            {
                List<TagMapping> mappings = new DAL.TagHelper().getTagMappings(storeId, type);
                if (mappings != null && mappings.Any())
                {
                    if (key is int)
                    {
                        tagMappings = (from tm in mappings
                                       group tm.TagId by tm.EntityIntKey.GetValueOrDefault() into g
                                       select g).ToDictionary(x => x.Key, y => y.ToList());
                    }
                    else
                    {
                        tagMappings = (from tm in mappings
                                       group tm.TagId by tm.EntityStringKey into g
                                       select g).ToDictionary(x => x.Key, y => y.ToList());
                    }

                }
                else
                {
                    if (key is int)
                    {
                        tagMappings = new Dictionary<int, List<int>>();
                    }
                    else
                    {
                        tagMappings = new Dictionary<string, List<int>>();
                    }
                }

                CachePool.getInstance().cacheObject(cachekey, tagMappings);

            }

            if (key is int)
            {
                Dictionary<int, List<int>> mappings = (Dictionary<int, List<int>>)tagMappings;

                if (mappings.ContainsKey((int)key))
                {
                    ids = mappings[(int)key];
                }
            }
            else
            {
                Dictionary<string, List<int>> mappings = (Dictionary<string, List<int>>)tagMappings;

                if (mappings.ContainsKey((string)key))
                {
                    ids = mappings[(string)key];
                }
            }

            return ids;
        }

        public List<Tag> getTags(string storeId)
        {
            string cachekey = $"{storeId}.Tags";
            List<Tag> tags = (List<Tag>)CachePool.getInstance().getObject(cachekey);
            if (tags == null)
            {
                tags = new DAL.TagHelper().getByStore(storeId);
                if (tags == null)
                    tags = new List<Tag>();
                CachePool.getInstance().cacheObject(cachekey, tags);
            }
            return tags;
        }
    }
}
