using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eStore.Presentation.ApplicationModel
{
    public class Application
    {
        public string Name { get; set; }
        public string BackgroundImage { get; set; }
        public int BackgroundImageWidth { get; set; }
        public int BackgroundImageHeight { get; set; }
        public List<Spot> Spots { get; set; }
        public List<Scenario> Scenarios { get; set; }
        public bool isValid { get; set; }
        public void load(BusinessModules.Store store, string path)
        {
            try
            {
                POCOS.ProductCategory productCategory = store.getProductCategory(path);
                if (productCategory != null && productCategory is POCOS.Application)
                {
                    POCOS.Application app = productCategory as POCOS.Application;
                    Name = app.LocalCategoryName;

                    BackgroundImage = string.Format("{0}/Application/{1}", Presentation.eStoreContext.Current.Store.profile.Settings["ProductCategoryPicurePath"].ToString(), app.BackgroundImage);
                    BackgroundImageWidth = app.BackgroundImageSize.Width;
                    BackgroundImageHeight = app.BackgroundImageSize.Height;

                    Spots = (from s in app.ApplicationSpots
                             select new Spot
                             {
                                 ID = s.ApplicationSpotID,
                                 Top = s.PosY,
                                 Left = s.PosX,
                                 MessagePosition = s.MessagePosition == 0 ? "top" : "bottom"
                             }).ToList();
                    Scenarios = (from s in app.childCategoriesX
                                 select convertEntity(s as POCOS.Application)
                                 ).ToList();
                    isValid = true;
                             
                }
                else
                {
                    isValid = false;
                }
            }
            catch (Exception ex)
            {
                isValid = false;
                eStore.Utilities.eStoreLoger.Error("load application failed. appid: " + path, "", "", "", ex);
            }

        }
        private Scenario convertEntity(POCOS.Application scenario)
        {
            Scenario rlt = new Scenario();

            rlt.ID = scenario.CategoryID;
            rlt.Path = scenario.CategoryPath;
            rlt.Name = scenario.LocalCategoryName;
            rlt.Link = UrlRewriting.MappingUrl.getMappingUrl(scenario);
            rlt.Spots = scenario.ScenarioSpotMappings.Select(x => x.ApplicationSpotID).ToList();
            rlt.Categories = (from sc in scenario.scenarioCategoriesX
                              orderby sc.Sequence, sc.ProductCategory.Sequence
                              select new Category
                              {
                                  ID = sc.CategoryID,
                                  Path = sc.ProductCategory.CategoryPath,
                                  Name = string.IsNullOrEmpty(sc.ScenarioCategoryName)
                                  ? sc.ProductCategory.LocalCategoryName : sc.ScenarioCategoryName ?? string.Empty
                              }).ToList();
            rlt.SubScenarios = (from ss in scenario.childCategoriesX
                                select convertEntity(ss as POCOS.Application)).ToList();

            return rlt;
        }
    }
    public class Spot
    {
        public int ID
        {
            get;
            set;
        }
        public int Top
        {
            get;
            set;
        }

        public int Left
        {
            get;
            set;
        }

        public string MessagePosition
        {
            get;
            set;
        }
    }

    public class Scenario
    {
        public int ID { get; set; }
        public string Path { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
        public List<int> Spots { get; set; }
        public List<Scenario> SubScenarios { get; set; }
        public List<Category> Categories { get; set; }
    }

    public class Category
    {
        public int ID { get; set; }
        public string Path { get; set; }
        public string Name { get; set; }
    }

}
