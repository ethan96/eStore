using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eStore.POCOS
{
    public partial class KitTheme
    {
        private string _imagefileX;
        public string ImageFileX
        {
            get
            {
                if (_imagefileX == null)
                {
                    if (!string.IsNullOrEmpty(ImageFile))
                        _imagefileX = ImageFile.StartsWith("http", true, null) ?
                                        ImageFile : String.Format("{0}resource{1}", esUtilities.CommonHelper.GetStoreLocation(), ImageFile);
                    else
                        _imagefileX = "";
                }
                return _imagefileX;
            }
        }
    }
}
