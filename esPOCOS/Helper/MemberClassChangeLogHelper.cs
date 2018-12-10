using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;

namespace eStore.POCOS.DAL
{

    public partial class MemberClassChangeLogHelper : Helper
    {
        #region Business Read

        public MemberClassChangeLog getMemberClassLogById(int classId)
        {
            var memberClassItem = (from m in context.MemberClassChangeLogs
                                   where m.ClassID == classId
                                   select m).FirstOrDefault();

            if (memberClassItem != null)
                memberClassItem.helper = this;
            return memberClassItem;
        }

        //或者用户的最后一个 等级信息
        public MemberClassChangeLog getUserMemberClassLog(string storeId,string userId)
        {
            var memberClassItem = (from m in context.MemberClassChangeLogs 
                                 where m.StoreId == storeId && m.UserID == userId 
                                 orderby m.ClassID descending
                                select m).FirstOrDefault();

            if (memberClassItem != null)
                memberClassItem.helper = this;
            return memberClassItem;
        }

        #endregion

        #region Creat Update Delete
        public int save(MemberClassChangeLog _memberClassItem)
        {
            if (_memberClassItem == null) return 1;

            MemberClassChangeLog _exists_memberClassItem = getMemberClassLogById(_memberClassItem.ClassID);
            try
            {
                if (_exists_memberClassItem == null)
                {
                    context.MemberClassChangeLogs.AddObject(_memberClassItem);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    if (_memberClassItem.helper != null && _memberClassItem.helper.context != null)
                        context = _memberClassItem.helper.context;
                    context.MemberClassChangeLogs.ApplyCurrentValues(_memberClassItem);
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

        public int delete(MemberClassChangeLog _memberClassItem)
        {
            if (_memberClassItem == null) return 1;

            try
            {
                if (_memberClassItem.helper != null && _memberClassItem.helper.context != null)
                    context = _memberClassItem.helper.context;
                context.MemberClassChangeLogs.DeleteObject(_memberClassItem);
                context.SaveChanges();
                return 0;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }
        }

        #endregion

        #region Others

        private static string myclassname()
        {
            return typeof(MemberClassChangeLogHelper).ToString();
        }
        #endregion
    }
}