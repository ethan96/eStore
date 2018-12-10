using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eStore.UI.Modules.Category
{
    public class CategoryTabBll
    {
        static void rollCategoryInt(POCOS.ProductCategory pc, ref List<VCategory> ls, int currLength)
        {
            if (pc != null)
            {
                VCategory vc = new VCategory() { Category = pc, Layer = currLength, HasImage = !string.IsNullOrEmpty(pc.ImageURL) };
                ls.Add(vc);
                if (pc.childCategoriesX != null && pc.childCategoriesX.Any())
                {
                    currLength++;
                    foreach (var c in pc.childCategoriesX)
                        rollCategoryInt(c, ref ls, currLength);
                }
            }
        }

        internal static string getShowType(POCOS.ProductCategory ProCategory)
        {
            if (ProCategory == null)
                return "None";
            List<VCategory> ls = new List<VCategory>();
            rollCategoryInt(ProCategory, ref ls, 1);
            int layer = ls.OrderByDescending(v => v.Layer).FirstOrDefault().Layer;
            if (layer == 1)
                return "WithNoSub";
            else if (layer == 2)
                return "WithSubOnly";
            else if (ls.Where(c=>c.Layer == 2 && c.HasImage).Any())
                return "WithSubPic";
            else if (!ls.Where(c => c.Layer == 3 && !c.IsCanShowWithPiece).Any())
                return "WithSubPiece";
            else
                return "WithSubList";
        }
    }

    class VCategory
    {
        private POCOS.ProductCategory _category;

        public POCOS.ProductCategory Category
        {
            get { return _category; }
            set { _category = value; }
        }

        private int _layer;

        public int Layer
        {
            get { return _layer; }
            set { _layer = value; }
        }

        private bool _hasImage;

        public bool HasImage
        {
            get { return _hasImage; }
            set { _hasImage = value; }
        }

        private bool? _isCanShowWithPiece = null;
        public bool IsCanShowWithPiece
        {
            get
            {
                if (_isCanShowWithPiece == null)
                {
                    if (Category != null)
                    {
                        string name = string.IsNullOrEmpty(Category.LocalCategoryName) ? Category.CategoryName : Category.LocalCategoryName;
                        return name.Length <= 10;
                    }
                }
                return _isCanShowWithPiece.Value;
            }
        }


    }
}