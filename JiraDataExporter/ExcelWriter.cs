using ClosedXML.Excel;
using System.Diagnostics;
using System.IO;
using static JiraDataExporter.Data;

namespace JiraDataExporter
{
    public class ExcelWriter
    {
        private readonly XLWorkbook _wb;
        private readonly IXLWorksheet _ws;

        public ExcelWriter()
        {
            _wb = new XLWorkbook();
            _ws = _wb.AddWorksheet();

        }

        public void WriteWorkBook(List<JiraDto> data)
        {
            WriteHeader();
            WriteTable(data);

            _ws.Columns().AdjustToContents();
            SaveAndOpenExcel(_wb);
        }

        private void WriteTable(List<JiraDto> data)
        {
            int Row = 2;

            foreach (var jira in data)
            {
                _ws.Cell(Row, 1).Value = jira.IssueKey;
                _ws.Cell(Row, 2).Value = jira.Status;
                _ws.Cell(Row, 3).Value = jira.SecurityLevel;
                
                _ws.Cell(Row, 4).Value = jira.UpdateDate;
                _ws.Cell(Row, 4).Style.NumberFormat.Format = "dd.MM.yyyy HH:mm";

                Row++;
            }
        }

        private void WriteHeader()
        {
            _ws.Cell("A1").Value = "Jira Issue";
            _ws.Cell("A1").Style.Font.Bold = true;

            _ws.Cell("B1").Value = "Status";
            _ws.Cell("B1").Style.Font.Bold = true;

            _ws.Cell("C1").Value = "Security";
            _ws.Cell("C1").Style.Font.Bold = true;

            _ws.Cell("D1").Value = "Update date";
            _ws.Cell("D1").Style.Font.Bold = true;
        }

        private static string SaveAndOpenExcel(XLWorkbook wb)
        {
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string fileName = $"Jira_Data_{timestamp}.xlsx";

            string filePath = Path.Combine(Path.GetTempPath(), fileName);

            try
            {
                wb.SaveAs(filePath);

                Process.Start(new ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true
                });

                return filePath;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saving or opening file: {ex.Message}", ex);
            }
        }
    }
}
