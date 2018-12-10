using System;
using System.Collections.Generic;
using eStore.POCOS;
using System.Linq;
using eStore.POCOS.DAL;

namespace eStore.POCOS.Sync
{
    class CTOSComponentSync
    {
        AdvStoreEntities acontext = new AdvStoreEntities();

        public void syncCTOScomponent(Store store) {

            var _sharebomtree = from s in acontext.es_sharebomtree
                                where s.node_type.ToUpper()=="LIST" && string.IsNullOrEmpty(s.node_partno_list)==false
                                select s;

            foreach (es_sharebomtree es in _sharebomtree) {
                 
                    string[] partnolist = es.node_partno_list.Trim().Split('|');

                    var p = from x in partnolist
                            group x by x into g
                            select new {partno=g.Key , count= g.Count()};

                    
                    PartHelper phelper = new PartHelper();

                    foreach(var s  in p){                        
                        Part part = phelper.getPart(s.partno.Trim(),store );
                        bool savepart = false;
                        if (part == null)
                        {
                            PISSync pissync = new PISSync();
                            Part ctospart = (Part) pissync.constructRelatedPart(s.partno.Trim(), store, true);
                            if (ctospart != null)
                            {
                                ctospart.VendorProductDesc= string.IsNullOrEmpty(ctospart.VendorProductDesc) == true ? es.node_desc : ctospart.VendorProductDesc;
                                ctospart.ProductType = "CTOSPART";
                                pissync.save(ctospart);
                                savepart = true;
                            }
                        }

                        if (part != null || savepart == true)
                        {
                            CTOSComponentDetail ccdetail = new CTOSComponentDetail();
                            ccdetail.SProductID = s.partno;
                            ccdetail.ComponentID = es.nodeid ;
                            ccdetail.ComponentDesc = part != null ? part.VendorProductDesc : es.node_desc;
                            ccdetail.DefaultSeq = es.node_seq.HasValue? es.node_seq.Value:999999 ;
                            ccdetail.Qty = s.count;

                           ccdetail.save();
                        }

                    }

                 

                

            }
        
        }

          

        
    }
}
