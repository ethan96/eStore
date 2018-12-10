using eStore.POCOS.PocoX;
using eStore.POCOS.DAL;


namespace eStore.POCOS
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public partial class WidgetRequest
    {
        public WidgetRequestHelper helper;

        public int save()
        {
            if (helper == null)
                helper = new WidgetRequestHelper();
            return helper.save(this);
        }
    }
}
