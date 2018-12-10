using eStore.POCOS.DAL;

namespace eStore.POCOS
{
    public partial class KitThemeMapping
    {
        public KitThemeMappingHelper helper;

        public int save()
        {
            if (helper == null)
                helper = new KitThemeMappingHelper();

            return helper.save(this);
        }
        public int delete()
        {
            if (helper == null)
                helper = new KitThemeMappingHelper();

            return helper.delete(this);
        }
    }
}
