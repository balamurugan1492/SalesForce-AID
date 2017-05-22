using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.IO;
using Microsoft.Win32;
using System.Web.UI;
using System.Data;

namespace Pointel.Interactions.Contact.Helpers
{
    public class DataExportHelper
    {
        public static void ExportAndSavePDF(List<ExportData> data, string pathTostoreFile)
        {
            if (data == null || string.IsNullOrEmpty(pathTostoreFile))
                throw new Exception("Data to export or path is null");

            PdfPTable pdfTable = new PdfPTable(data[0].Keys.Count);
            pdfTable.WidthPercentage = 100;
            foreach (string keyName in data[0].Keys)
            {
                pdfTable.AddCell(GetPdfCell(keyName, true));
            }


            for (int index = 0; index < data.Count; index++)
            {
                foreach (string keyName in data[0].Keys)
                {
                    if (data[index].Keys.Contains(keyName))
                        pdfTable.AddCell(GetPdfCell(data[index][keyName]));
                    else
                        pdfTable.AddCell(GetPdfCell(string.Empty));
                }
            }

            Document document = new Document();
            PdfWriter.GetInstance(document, new FileStream(pathTostoreFile, FileMode.Create));
            document.SetPageSize(iTextSharp.text.PageSize.A3.Rotate());
            document.Open();
            document.Add(pdfTable);
            document.Close();

        }

        private static PdfPCell GetPdfCell(string text, bool isFontBold = false)
        {
            PdfPCell objPdfCell = new PdfPCell(new Phrase(text));
            if (isFontBold)
            {
                BaseFont objBaseFont = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);
                Font objFont = new Font(objBaseFont, 12, Font.BOLD, Color.BLACK);
                objPdfCell = new PdfPCell(new Phrase(text, objFont));
            }
            objPdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
            objPdfCell.MinimumHeight = 35;
            return objPdfCell;
        }

        public static void ExportAndSaveExcell(List<ExportData> data, string pathTostoreFile)
        {
            if (data == null || string.IsNullOrEmpty(pathTostoreFile))
                throw new Exception("Data to export or path is null");

            DataTable objDatatable = new DataTable();
            foreach (string keyName in data[0].Keys)
                objDatatable.Columns.Add(keyName);

            for (int index = 0; index < data.Count; index++)
            {
                DataRow objDataRow = objDatatable.NewRow();
                foreach (string keyName in data[0].Keys)
                    if (data[index].Keys.Contains(keyName))
                        objDataRow[keyName] = data[index][keyName];
                    else
                        objDataRow[keyName] = string.Empty;
                objDatatable.Rows.Add(objDataRow);
            }

            System.Web.UI.WebControls.DataGrid grid = new System.Web.UI.WebControls.DataGrid();
            grid.HeaderStyle.Font.Bold = true;
            grid.ItemStyle.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Left;
            grid.DataSource = objDatatable;

            grid.DataBind();

            using (StreamWriter sw = new StreamWriter(pathTostoreFile))
            {
                using (HtmlTextWriter hw = new HtmlTextWriter(sw))
                {
                    grid.RenderControl(hw);
                }
            }

        }

        public static void ExportAndSaveCSV(List<ExportData> data, string pathTostoreFile)
        {
            StringBuilder _tempString = new StringBuilder();
            for (int index = 0; index < data.Count; index++)
            {
                if (index == 0)
                    foreach (string keyName in data[index].Keys)
                    {
                        if (!string.IsNullOrEmpty(keyName))
                        {
                            var formatedData = keyName;
                            formatedData = formatedData.Replace("\"", "\"\"");
                            formatedData = "\"" + formatedData + "\"";
                            formatedData = formatedData + ",";
                            _tempString.Append(formatedData);
                        }
                        else
                            _tempString.Append("\"" + string.Empty + "\",");
                    }
                else
                    foreach (string keyName in data[index].Keys)
                    {
                        if (data[index].Keys.Contains(keyName))
                        {
                            var formatedData = data[index][keyName];
                            formatedData = formatedData.Replace("\"", "\"\"");
                            formatedData = "\"" + formatedData + "\"";
                            formatedData = formatedData + ",";
                            _tempString.Append(formatedData);
                        }
                        else
                            _tempString.Append("\"" + string.Empty + "\",");
                    }
                _tempString.Append(Environment.NewLine);
            }
            var finalFormatedData = _tempString.ToString().Substring(0, _tempString.ToString().Length - 2);
            System.IO.File.WriteAllText(pathTostoreFile, finalFormatedData);
        }
    }
}
