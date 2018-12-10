using System;
using System.Collections.Generic;
using System.Linq;
using eStore.Utilities;
using System.Text;

namespace eStore.POCOS.DAL
{
    public class SolutionHelper:Helper
    {
        internal int save(Solution solution)
        {
            //if parameter is null or validation is false, then return  -1 
            if (solution == null || solution.validate() == false) return 1;
            //Try to retrieve object from DB
            Solution _exist_solution = getSolutionById(solution.Id, solution.StoreID);
            try
            {
                if (_exist_solution == null)  //object not exist 
                {
                    //Insert
                    context.Solutions.AddObject(solution);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    //Update
                    context.Solutions.ApplyCurrentValues(solution);
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

        public Solution getSolutionById(int id, string storeid)
        {
            Solution solution = context.Solutions.FirstOrDefault(s => s.Id == id && s.StoreID == storeid);
            return solution;
        }

        public List<Solution> getAllSolution(string storeid)
        {
            return context.Solutions.Include("SolutionsAssosociateItems").Where(s => s.StoreID == storeid).OrderBy(x=>x.Seq).ThenBy(x=>x.Name).ToList();
        }

        public int delete(Solution solution)
        {

            if (solution == null || solution.validate() == false) return 1;
            POCOS.Solution sl = (from c in context.Solutions
                                where c.Id == solution.Id
                                select c).FirstOrDefault();
            try
            {
                context = solution.helper.context;
                context.DeleteObject(sl);
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
