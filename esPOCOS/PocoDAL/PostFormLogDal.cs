// -----------------------------------------------------------------------
// <copyright file="PostFormLogDal.cs" company="Microsoft">
// 添加Form post的日志
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;

namespace eStore.POCOS
{
    /// <summary>
    /// 添加Form post的日志
    /// </summary>
    public partial class PostFormLog
    {
        public PostFormLogHelper helper;

        public int save()
        {
            if (helper == null)
                helper = new PostFormLogHelper();

            return helper.save(this);
        }
    }
}
