using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eStore.UI.Models
{
    public class Solution
    {
        public Solution(POCOS.Solution s)
        {
            this._id = s.Id;
            this._name = s.Name;
            this._img = s.Image;
            this._link = s.Link;
            this._title = s.Title;
            this._description = s.Description;
            loadAssosociateItems(s);
        }

        private void loadAssosociateItems(POCOS.Solution s)
        {
            foreach (var a in s.SolutionsAssosociateItems)
            {

                if (a.partX!=null && a.partX is POCOS.Product)
                {
                    this.AssosociateItems.Add(new TodaysHighlight((POCOS.Product)a.partX));
                }
            }
        }



        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private string _img;

        public string Img
        {
            get { return _img; }
            set { _img = value; }
        }

        private int _id;

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        private string _link;

        public string Link
        {
            get { return this._link; }
            set { _link = value; }
        }

        private string _title;

        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        private string _description;

        public string Description
        {
            get {
                if (string.IsNullOrEmpty(_description))
                    return _name;
                else
                    return _description;
            }
            set { _description = value; }
        }
        

        private List<TodaysHighlight> _assosociateItems = new List<TodaysHighlight>();

        public List<TodaysHighlight> AssosociateItems
        {
            get { return _assosociateItems; }
            set { _assosociateItems = value; }
        }






        public class AssosociateItem
        {
            private string _img;

            public string Img
            {
                get { return _img; }
                set { _img = value; }
            }


            private string _sproductid;

            public string Sproductid
            {
                get { return _sproductid; }
                set { _sproductid = value; }
            }

            private string _des;

            public string Des
            {
                get { return _des; }
                set { _des = value; }
            }

            private string _price;

            public string Price
            {
                get { return _price; }
                set { _price = value; }
            }

        }
    }
}
 