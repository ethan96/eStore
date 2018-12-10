using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS;
using eStore.Utilities;

namespace eStore.POCOS.DAL
{
    public class Spec_CategoryHelper : Helper
    {

        internal int save(Spec_Category spec_Category)
        {
            //if parameter is null or validation is false, then return  -1 
            if (spec_Category == null || spec_Category.validate() == false) return 1;
            //Try to retrieve object from DB
            Spec_Category _exist_spec_Category = getSpecCategoryById(spec_Category.CATEGORY_ID);
            try
            {
                if (_exist_spec_Category == null)  //object not exist 
                {
                    //Insert
                    context.sp_Add_Spec_Category(spec_Category.Parent.CATEGORY_ID, spec_Category.CATEGORY_DISPLAYNAME, spec_Category.CATEGORY_TYPE,
                        spec_Category.SEQUENCE, spec_Category.CONDITIONS, spec_Category.CREATED_BY, spec_Category.CREATED_DATE, spec_Category.LAST_UPDATED_BY
                        , spec_Category.LAST_UPDATED_DATE, spec_Category.DisplayType);
                    return 0;
                }
                else
                {
                    //Update
                    context.sp_Update_Spec_Category(spec_Category.CATEGORY_ID, spec_Category.CATEGORY_DISPLAYNAME, spec_Category.CATEGORY_TYPE, spec_Category.SEQUENCE
                        , spec_Category.CONDITIONS, spec_Category.LAST_UPDATED_BY, spec_Category.LAST_UPDATED_DATE, spec_Category.DisplayType, spec_Category.NodeTree = "");
                    return 0;

                }
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }
        }

        public List<Spec_Category> getAllSpecCategoryByRootId(string name, string storeid)
        {
            var ls = context.getSpecCategoryTree(name , storeid).ToList();
            foreach (var l in ls)
                l.StoreID = storeid;
            return ls;
        } 

        public List<Spec_Category> getAllSpecCategoryByRootId(int id, string storeid)
        {
            var ls = context.getSpecCategoryTreeByID(id).ToList();

            return ls;
        }

        public Spec_Category getSpecCategoryById(int id)
        {
            return context.Spec_Category.FirstOrDefault(c => c.CATEGORY_ID == id);
        }


        public Spec_Category getSpecCategoryByName(string name)
        {
            return context.Spec_Category.FirstOrDefault(c => c.CATEGORY_DISPLAYNAME == name);
        }

        public List<Spec_Category> getCategoryListByIds(List<int> ids)
        {
            var ls = (from c in context.Spec_Category
                      where ids.Contains(c.CATEGORY_ID)
                      orderby c.SEQUENCE
                      select c).ToList();
            return ls;
        }

        public List<Product> getProductByCategory(Spec_Category category, string storeid)
        {
            if (category == null || string.IsNullOrEmpty(storeid))
                return new List<Product>();

            var ls = (from c in category.Part_Spec_V3
                      from p in context.Parts.OfType<Product>()
                      where p.StoreID == storeid && p.SProductID == c.PART_NO
                      select p).ToList();
            return ls;
        }


        //get Part_Spec_V3 by category list
        public List<Part_Spec_V3> getSpecByCategoryIdsWithMarg(List<int> categoryids, string includStoreProduct = "")
        {
            if (categoryids == null || !categoryids.Any())
                return new List<Part_Spec_V3>();

            List<CategoryCondition> tree = new List<CategoryCondition>();
            var categoryls = getCategoryListByIds(categoryids).OrderByDescending(c => c.CATEGORY_ID);

            foreach (var c in categoryls)
            {
                var pcate = tree.FirstOrDefault(t => t.ParentCategory.CATEGORY_ID == (c.ParentCategory == null ? 0 : c.ParentCategory.CATEGORY_ID));
                if (pcate == null)
                {
                    pcate = new CategoryCondition()
                    {
                        ParentCategory = c.ParentCategory,
                        SubCategories = new List<Spec_Category>() { c }
                    };

                    foreach (var p in tree)
                    {
                        if (c.ParentCategory.children.FirstOrDefault(t => t.CATEGORY_ID == p.ParentCategory.CATEGORY_ID) != null && !pcate.isInclud(p))
                        {
                            pcate.SubCategories.Add(p.ParentCategory);
                            pcate.Childens.Add(p);
                        }
                        if (pcate.isInclud(p))
                            p.Parent = pcate;
                    }
                    tree.Add(pcate);
                }
                else
                    pcate.SubCategories.Add(c);
            }


            CategoryCondition root = new CategoryCondition()
            {
                Childens = tree.Where(c => c.Parent == null).ToList(), StoreID = includStoreProduct
            };


            var ls = root.Specs;

            if (!string.IsNullOrEmpty(includStoreProduct))
            {
                List<Part_Spec_V3> ll = new List<Part_Spec_V3>();
                PartHelper parthelper = new PartHelper();
                List<Part> prefetched = parthelper.prefetchPartList(includStoreProduct, ls.Where(p => string.IsNullOrEmpty(p.StoreID) || p.StoreID == includStoreProduct).Select(x => x.PART_NO).ToList());
                foreach (var s in ls)
                {
                    var p = prefetched.FirstOrDefault(c => c.SProductID.Equals(s.PART_NO, StringComparison.OrdinalIgnoreCase));
                    if (p != null && p is Product && (p as Product).isOrderable())
                    {
                        s.part = (p as Product);
                        ll.Add(s);
                    }
                }
                ls = ll;
            }

            return ls;
        }

        public System.Data.DataTable GetDataTable(string sqlcmd)
        {
            System.Data.EntityClient.EntityConnection ec = context.Connection as System.Data.EntityClient.EntityConnection;
            if (string.IsNullOrEmpty(ec.StoreConnection.ConnectionString))
                return null;

            System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(ec.StoreConnection.ConnectionString);
            System.Data.DataTable dt = new System.Data.DataTable();
            System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(sqlcmd, conn);
            da.SelectCommand.CommandTimeout = 5 * 60;

            try
            {
                da.Fill(dt);
            }
            catch
            {
                return null;
            }
            finally
            {
                if (conn.State == System.Data.ConnectionState.Open)
                    conn.Close();
                conn.Dispose();
            }
            return dt;
        }


        protected class CategoryCondition
        {
            private string storeid;
            public string StoreID
            {
                get
                {
                    return this.storeid;
                }
                set
                {
                    this.storeid = value;
                }
            }

            private Spec_Category _parentCategory = new Spec_Category() { CATEGORY_ID = 0, CONDITIONS = "And" };

            public Spec_Category ParentCategory
            {
                get
                { return _parentCategory; }
                set { _parentCategory = value; }
            }

            private List<Part_Spec_V3> _specs;

            public List<Part_Spec_V3> Specs
            {
                get
                {
                    if (_specs == null)
                    {
                        if (_subCategories.Any())
                            margPartSpec(this.StoreID);
                        else
                            margSpec(this.StoreID);
                    }
                    return _specs.OrderBy(x => x.SEQUENCE).ToList();
                }
            }


            private List<Spec_Category> _subCategories = new List<Spec_Category>();

            public List<Spec_Category> SubCategories
            {
                get { return _subCategories; }
                set { _subCategories = value; }
            }


            private CategoryCondition _parent;

            public CategoryCondition Parent
            {
                get { return _parent; }
                set { _parent = value; }
            }

            private List<CategoryCondition> _childens = new List<CategoryCondition>();

            public List<CategoryCondition> Childens
            {
                get { return _childens; }
                set { _childens = value; }
            }



            public bool isInclud(CategoryCondition sub)
            {
                if (sub.ParentCategory.CATEGORY_ID == 0)
                    return false;
                return _subCategories.Select(f => f.CATEGORY_ID).Contains(sub.ParentCategory.CATEGORY_ID);
            }

            protected void margSpec(string storeID)
            {
                List<Part_Spec_V3> AllCatels = new List<Part_Spec_V3>();
                List<Part_Spec_V3> cateLs = new List<Part_Spec_V3>();

                foreach (var sc in Childens)
                {
                    sc.StoreID = storeID;
                    foreach (var ps in sc.Specs)
                    {
                        if (string.IsNullOrEmpty(ps.StoreID) || ps.StoreID == sc.StoreID)
                        {
                            if (AllCatels.FirstOrDefault(c => c.PART_NO.Equals(ps.PART_NO)) == null)
                                AllCatels.Add(ps);
                        }
                    }
                }
                switch (_parentCategory.categoryContion)
                {
                    case Spec_Category.CateContion.Or:
                        cateLs = AllCatels;
                        break;
                    case Spec_Category.CateContion.And:
                    default:
                        bool isInAllCategory = true;
                        foreach (var c in AllCatels)
                        {
                            isInAllCategory = true;
                            foreach (var s in Childens)
                            {
                                if (s.Specs.FirstOrDefault(t => t.PART_NO == c.PART_NO) == null)
                                {
                                    isInAllCategory = false;
                                    break;
                                }
                            }
                            if (isInAllCategory)
                                cateLs.Add(c);
                        }
                        break;
                }
                _specs = cateLs;
            }


            protected void margPartSpec(string storeID)
            {

                List<Part_Spec_V3> AllCatels = new List<Part_Spec_V3>();
                List<Part_Spec_V3> cateLs = new List<Part_Spec_V3>();

                foreach (var sc in _subCategories)
                {
                    sc.StoreID = storeID;
                    foreach (var ps in sc.Part_Spec_V3)
                    {
                        if (string.IsNullOrEmpty(ps.StoreID) || ps.StoreID == sc.StoreID)
                        {
                            if (AllCatels.FirstOrDefault(c => c.PART_NO.Equals(ps.PART_NO)) == null)
                                AllCatels.Add(ps);
                        }
                    }
                }

                switch (_parentCategory.categoryContion)
                {
                    case Spec_Category.CateContion.Or:
                        cateLs = AllCatels;
                        break;
                    case Spec_Category.CateContion.And:
                    default:
                        bool isInAllCategory = true;
                        foreach (var c in AllCatels)
                        {
                            isInAllCategory = true;
                            foreach (var s in _subCategories)
                            {
                                if (s.Part_Spec_V3.FirstOrDefault(t => t.PART_NO == c.PART_NO) == null)
                                {
                                    isInAllCategory = false;
                                    break;
                                }
                            }

                            if (isInAllCategory)
                                cateLs.Add(c);
                        }
                        break;
                }
                _specs = cateLs;
            }
        }


        //get Part_Spec_V3 by category list
        public List<Part> getPartsByCategoryIds(List<int> categoryids, string includStoreProduct = "")
        {
            //if (categoryids == null || !categoryids.Any())
            //    return new List<Part_Spec_V3>();

            var ls = (from c in context.Part_Spec_V3
                      where categoryids.Contains(c.CATEGORY_ID)
                      select c).GroupBy(c => c.PART_NO).Where(c => c.Count() == categoryids.Count).Select(c => c.Key).ToList();

            if (!string.IsNullOrEmpty(includStoreProduct))
            {
                PartHelper parthelper = new PartHelper();
                return parthelper.prefetchPartList(includStoreProduct, ls);
                //foreach (var s in ls)
                //    s.part = prefetched.FirstOrDefault(c => c.SProductID.Equals(s.PART_NO, StringComparison.OrdinalIgnoreCase));
            }

            return new List<Part>();
        }

        public Tuple<bool, string> UpdateSpecCategory(string sqlcmd, List<System.Data.SqlClient.SqlParameter> parameters)
        {
            System.Data.EntityClient.EntityConnection ec = context.Connection as System.Data.EntityClient.EntityConnection;
            if (string.IsNullOrEmpty(ec.StoreConnection.ConnectionString))
                return null;

            System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(ec.StoreConnection.ConnectionString);
            System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(sqlcmd, conn);
            cmd.CommandTimeout = 5 * 30;

            if (parameters != null && parameters.Count > 0)
                cmd.Parameters.AddRange(parameters.ToArray());

            try
            {
                conn.Open();
                cmd.ExecuteNonQuery();
                return new Tuple<bool, string>(true, string.Empty);
            }
            catch (Exception ex)
            {
                return new Tuple<bool, string>(false, ex.Message);
            }
            finally
            {
                cmd.Dispose();
                if (conn.State == System.Data.ConnectionState.Open)
                    conn.Close();
                conn.Dispose();
            }
        }

        public Tuple<bool, string> AddSpecCategory(Spec_Category category)
        {
            System.Data.EntityClient.EntityConnection ec = context.Connection as System.Data.EntityClient.EntityConnection;
            if (string.IsNullOrEmpty(ec.StoreConnection.ConnectionString))
                return new Tuple<bool, string>(false, "Connection failed");

            if (category == null || category.Parent == null)
                return new Tuple<bool, string>(false, "Category or parent category is null");

            System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(ec.StoreConnection.ConnectionString);
            System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand("dbo.sp_Add_Spec_Category", conn);

            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            cmd.Parameters.Add("@ParentCategoryID", System.Data.SqlDbType.Int);
            cmd.Parameters["@ParentCategoryID"].Value = category.Parent.CATEGORY_ID;

            cmd.Parameters.Add("@DisplayName", System.Data.SqlDbType.VarChar, 300);
            cmd.Parameters["@DisplayName"].Value = category.CATEGORY_DISPLAYNAME;

            cmd.Parameters.Add("@CateogryType", System.Data.SqlDbType.VarChar, 30);
            cmd.Parameters["@CateogryType"].Value = category.CATEGORY_TYPE;

            cmd.Parameters.Add("@Sequence", System.Data.SqlDbType.Int);
            cmd.Parameters["@Sequence"].Value = category.SEQUENCE;

            cmd.Parameters.Add("@Conditions", System.Data.SqlDbType.NVarChar, 30);
            cmd.Parameters["@Conditions"].Value = category.CONDITIONS;

            cmd.Parameters.Add("@CreatedBy", System.Data.SqlDbType.NVarChar, 100);
            cmd.Parameters["@CreatedBy"].Value = category.CREATED_BY;

            cmd.Parameters.Add("@CreatedDate", System.Data.SqlDbType.DateTime);
            cmd.Parameters["@CreatedDate"].Value = category.CREATED_DATE;

            cmd.Parameters.Add("@DesplayType", System.Data.SqlDbType.NVarChar, 50);
            cmd.Parameters["@DesplayType"].Value = category.DisplayType;

            cmd.Parameters.Add("@NodeTree", System.Data.SqlDbType.NVarChar, 50);
            cmd.Parameters["@NodeTree"].Value = string.Empty;

            System.Data.SqlClient.SqlParameter returnData = cmd.Parameters.Add("@OutputID", System.Data.SqlDbType.NVarChar, 200);
            returnData.Direction = System.Data.ParameterDirection.Output;

            try
            {
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                return new Tuple<bool, string>(false, ex.Message);
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
            return new Tuple<bool, string>(true, string.Empty);
        }
    }
}
