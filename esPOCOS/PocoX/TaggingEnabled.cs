using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eStore.POCOS
{
    public abstract class TaggingEnabled<T>
    {
        public virtual string EntityStoreId
        {
            get
            {
                return string.Empty;
            }
        }

        public virtual object EntityKey
        {
            get
            {
                return null;
            }
        }
        private List<int> tagIds = null;
        private List<Tag> tags = null;
        public virtual List<Tag> Tags
        {
            get
            {
                if (tags == null)
                {

                    tagIds = TaggingManagement.getInstance().getTagIds(EntityStoreId, typeof(T).Name, EntityKey);
                    if (tagIds != null && tagIds.Any())
                    {
                        tags = TaggingManagement.getInstance().getTags(EntityStoreId).Where(x => tagIds.Contains(x.Id)).ToList();
                    }

                    if (tags == null)
                        tags = new List<POCOS.Tag>();
                }

                return tags;
            }

            set
            {
                if (EntityKey is int)
                {
                    new DAL.TagHelper().addTags(EntityStoreId, typeof(T).Name, (int)EntityKey, value.Select(x => x.Id).ToList());
                }
                else if (EntityKey is string)
                {
                    new DAL.TagHelper().addTags(EntityStoreId, typeof(T).Name, (string)EntityKey, value.Select(x => x.Id).ToList());
                }
            }
        }

    }
}
