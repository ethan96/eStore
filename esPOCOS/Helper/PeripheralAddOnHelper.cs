using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.Utilities;

namespace eStore.POCOS.DAL
{
    public class PeripheralAddOnHelper : Helper
    {

        public PeripheralAddOn getPeripheralAddOnId(int p)
        {
            return context.PeripheralAddOns.FirstOrDefault(c => c.AddOnItemID == p);
        }

        internal int save(PeripheralAddOn peripheralAddOn)
        {
            if (peripheralAddOn == null || peripheralAddOn.validate() == false)
                return 1;
            try
            {
                var exitAddOn = getPeripheralAddOnId(peripheralAddOn.AddOnItemID);
                if (exitAddOn == null)
                {
                    context.PeripheralAddOns.AddObject(peripheralAddOn);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    context = peripheralAddOn.helper.context;
                    context.PeripheralAddOns.ApplyCurrentValues(peripheralAddOn);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="part"></param>
        /// <param name="addons"></param>
        public void margeSave(Part part,List<PeripheralAddOn> addons)
        {
            var oldAddons = getAddonByPart(part);

            foreach (var naddon in addons)
            {
                foreach (var oaddon in oldAddons)
                {
                    if (naddon.PeripheralAddOnBundleItems.Count == oaddon.PeripheralAddOnBundleItems.Count) //数量不等肯定不是同一个addon
                    {
                        bool isItem = true;
                        foreach (var item in naddon.PeripheralAddOnBundleItems)
                        {
                            if (oaddon.PeripheralAddOnBundleItems.FirstOrDefault(c => c.SProductID == item.SProductID) == null)
                            {
                                isItem = false; //如果下面的bundle 不匹配肯定不是同一个addon
                                break;
                            }
                        }
                        if (isItem) //如果匹配到addon ， 将 addonitemid标识为db的id
                        {
                            naddon.AddOnItemID = oaddon.AddOnItemID; 
                            break;
                        }
                    }
                }
            }

            List<PeripheralAddOn> addAddons = addons.Where(c => c.AddOnItemID == 0).ToList();
            List<PeripheralAddOn> deleteAddons = oldAddons.Where(c => c.Source.Equals("PIS",StringComparison.OrdinalIgnoreCase) && !addons.Select(t => t.AddOnItemID).Contains(c.AddOnItemID)).ToList();
            foreach (var n in addAddons)
                n.save();
            foreach (var d in deleteAddons)
                d.delete();


            #region


            //List<PeripheralAddOn> saveAddon = new List<PeripheralAddOn>();
            //List<PeripheralAddOn> deleteAddon = new List<PeripheralAddOn>();

            //foreach (var a in addons)
            //{
            //    bool isInOld = false;
            //    foreach (var ad in oldAddons)
            //    {
            //        bool itemOk = true;
            //        if(a.PeripheralAddOnBundleItems.Count == ad.PeripheralAddOnBundleItems.Count) //如果item数量相等才匹配item
            //        {
            //            foreach (var b in a.PeripheralAddOnBundleItems)
            //            {
            //                if (ad.PeripheralAddOnBundleItems.FirstOrDefault(c => c.SProductID == b.SProductID) == null)
            //                {
            //                    itemOk = false;
            //                    continue;
            //                }
            //            }
            //        }
            //        else
            //            itemOk = false;
            //        if(itemOk) //如果新addon的所有item都存在资料库则认为 addon存在 db
            //        {
            //            isInOld = true;
            //            continue;
            //        }
            //    }
            //    if (!isInOld) //不存在db中将被保存
            //        saveAddon.Add(a);
            //}

            //foreach (var ad in oldAddons)
            //{
            //    bool isInNew = true;
            //    foreach (var a in addons)
            //    {
            //        bool itemOk = true;
            //        if (ad.PeripheralAddOnBundleItems.Count == a.PeripheralAddOnBundleItems.Count)
            //        {
            //            foreach (var b in ad.PeripheralAddOnBundleItems)
            //            {
            //                if (a.PeripheralAddOnBundleItems.FirstOrDefault(c => c.SProductID == b.SProductID) == null) //如果old addon item中有
            //                {
            //                    itemOk = false;
            //                    continue;
            //                }
            //            }
            //        }
            //        else
            //            itemOk = false;
            //        if (itemOk)
            //        {
            //            isInNew = true;
            //            continue;
            //        }
            //    }
            //    if (!isInNew)
            //        deleteAddon.Add(ad);
            //}
            #endregion
        }

        public List<PeripheralAddOn> getAddonByPart(Part part)
        {
            var ls = (from p in context.PeripheralAddOns
                      where p.StoreID == part.StoreID && p.SProductID == part.SProductID
                      select p).ToList();
            return ls;

        }



        internal int delete(PeripheralAddOn peripheralAddOn)
        {
            var exitObj = getPeripheralAddOnId(peripheralAddOn.AddOnItemID);
            try
            {
                context.PeripheralAddOns.DeleteObject(exitObj);
                context.SaveChanges();
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
