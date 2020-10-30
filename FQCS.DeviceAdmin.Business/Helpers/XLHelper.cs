using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Text;

namespace FQCS.DeviceAdmin.Business.Helpers
{
    public static class XLHelper
    {

        public static IXLRow SetRowData(this IXLWorksheet sheet, int row, params object[] data)
        {
            var length = data.Length;
            var dRow = sheet.Row(row);
            for (var i = 0; i < length; i++)
                dRow.Cell(i + 1).Value = data[i];
            return dRow;
        }
    }
}
