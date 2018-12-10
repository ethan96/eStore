using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;

namespace eStore.POCOS
{
    public partial class WidgetPage
    {

        private List<Menu> _menulist = null;

        #region OM Extension Methods

        /// <summary>
        /// Get menu reference
        /// </summary>
        public List<Menu> menulist
        {
            get
            {
                if (_menulist == null)
                {
                    MenuHelper helper = new MenuHelper();
                    _menulist = helper.getMenusByWidgetID(this.WidgetPageID);
                }
                return _menulist;
            }

        }

        public Boolean validForDelete
        {
            get
            {
                //additional validation
                if (this.menulist == null || this.menulist.Count == 0)
                {
                    error_message = new List<PocoX.ErrorMessage>();
                    error_message.Add(new PocoX.ErrorMessage("menuList", "Refer to Menu List"));
                }
                if (error_message != null && error_message.Count > 0)
                    return false;
                else
                    return true;

            }
        }

        /// <summary>
        /// This property will return the short name of original Widget zip package file
        /// </summary>
        public String packageNameX
        {
            get
            {
                if (String.IsNullOrEmpty(FileName))
                    return "";
                else
                {
                    int iIndex = FileName.LastIndexOf("\\");
                    if (iIndex == -1)
                        return FileName;
                    else
                    {
                        String name =  FileName.Substring(iIndex + 1);
                        return name;
                    }
                }
            }
        }
     
        #endregion
    }
}
