using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;

 namespace eStore.POCOS{

     public partial class Product : Part
     {
         //public PartHelper _helper;

         public int save()
         {
             /*
             if (_helper == null)
                 _helper = new PartHelper();
             int result = _helper.save(this);
              * */
             if (parthelper == null)
                 parthelper = new PartHelper();
             int result = parthelper.save(this);
             if (result >= 0)
             {
                 this.hasProductInfoChanged = false;
                 this.hasProductPriceChanged = false;
                 this.hasProductStockStatusChanged = false;
             }

             return result;
         }

         public int delete()
         {
             /*
             if (_helper == null)
                 _helper = new PartHelper();
             return _helper.delete(this);
              * */
             if (this.parthelper == null)
                 parthelper = new PartHelper();
             return parthelper.delete(this);
         }
     }
 }