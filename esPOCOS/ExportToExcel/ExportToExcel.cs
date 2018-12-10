using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NPOI.HSSF.UserModel;
using NPOI.HPSF;
using System.IO;
using NPOI.SS.UserModel;
using System.Reflection;
using System.Web;

namespace eStore.POCOS.ExportToExcel
{
    public class ExportToExcel
    {
        /// <summary>
        /// 导出excel
        /// </summary>
        /// <param name="hssfworkbook"></param>
        /// <returns></returns>
        public MemoryStream WriteToStream(HSSFWorkbook hssfworkbook)
        {
            //Write the stream data of workbook to the root directory
            MemoryStream file = new MemoryStream();
            hssfworkbook.Write(file);
            return file;
        }

        /// <summary>
        /// 获取需要导出的数据
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="hssfworkbook"></param>
        /// <param name="li">类型</param>
        /// <param name="fieldName">字段名</param>
        /// <param name="ColumnName">列名</param>
        /// <returns></returns>
        public HSSFWorkbook ListToWorkBook<T>(HSSFWorkbook hssfworkbook, List<T> li, List<string> fieldName, List<string> ColumnName=null) where T : new()
        {
            ISheet sheet = hssfworkbook.CreateSheet(DateTime.Now.ToString("yyyyMMdd"));//CreateSheet(sheetName);

            ColumnName = (ColumnName == null) ? fieldName : ColumnName;

            int rowIndex = 0;
            foreach (T t in li)
            {
                int ColumnIndex = 0;
                if (rowIndex == 0)//加表头
                {
                    IRow HeaderdataRow = sheet.CreateRow(rowIndex);
                    foreach (string colArry in fieldName)//每个属性循环
                    {
                        System.Reflection.PropertyInfo pi = t.GetType().GetProperty(colArry);
                        if (pi != null)
                        {
                            ICell hssfc = HeaderdataRow.CreateCell(ColumnIndex);
                            sheet.SetColumnWidth(ColumnIndex, 5000);
                            hssfc.SetCellValue(ColumnName[fieldName.IndexOf(colArry)]);
                            ColumnIndex++;
                            hssfc = null;
                        }
                    }
                    rowIndex++;
                    ColumnIndex = 0;
                    HeaderdataRow = null;

                }

                IRow dataRow = sheet.CreateRow(rowIndex);
                foreach (string colArry in fieldName)//获取字段的值的列表
                {
                    System.Reflection.PropertyInfo pi = t.GetType().GetProperty(colArry);
                    if (pi != null)
                    {
                        object _obj = t.GetType().GetProperty(pi.Name).GetValue(t, null);
                        dataRow.CreateCell(ColumnIndex).SetCellValue(_obj == null ? "" : _obj.ToString());
                        ColumnIndex++;
                    }
                }
                rowIndex++;
                dataRow = null;
            }
            sheet = null;
            return hssfworkbook;
        }
    }
}
