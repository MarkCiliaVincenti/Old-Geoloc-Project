using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using SmartFleet.Core.Domain.Movement;
using SmartFleet.Core.Domain.Vehicles;

namespace SmartFleet.Service.Common
{
    public class PdfService : IPdfService
    {
       public void CreatePdfReport(List<Position> positions, Vehicle vehicle, dynamic report,
            MemoryStream stream)
        {
            Document doc = new Document(PageSize.A4, 15, 15, 15, 15);

            string[] headers =
                {"Conduite", "Début", "Fin", "Distance", "Adresse départ", "Adresse d'arrivée", "Vitesse moy"};
            float[] columnWidths = { 140f, 140, 140, 120, 200f, 200f, 100 };
            PdfWriter.GetInstance(doc, stream).CloseStream = false;
            doc.Open();
            PdfPTable tblHeader = new PdfPTable(2);
            tblHeader.WidthPercentage = 100;

            PdfPCell leftCell = new PdfPCell();
            leftCell.Border = 0;
            Paragraph paragraph = new Paragraph($"Rapport de conduite",
                FontFactory.GetFont("Calibri", 18, BaseColor.BLACK));
            paragraph.Alignment = Element.ALIGN_LEFT;
            leftCell.AddElement(paragraph);
            leftCell.AddElement(null);
            leftCell.AddElement(null);
            PdfPCell rightCell = new PdfPCell();
            rightCell.Border = 0;
            var prg = new Paragraph("Véhicule : " + report.VehicleName);
            rightCell.AddElement(prg);
            prg = new Paragraph("Date : " + report.ReportDate);
            rightCell.AddElement(prg);
            PdfPCell emptyCell = new PdfPCell();
            emptyCell.Border = 0;
            tblHeader.AddCell(leftCell);
            tblHeader.AddCell(rightCell);
            doc.Add(tblHeader);
            doc.Add(new Chunk(Environment.NewLine));
            Paragraph p = new Paragraph(new Chunk(
                new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1)));
            // call the below to add the line when required.
            doc.Add(p);
            doc.Add(new Chunk(Environment.NewLine));
            doc.Add(new Chunk(Environment.NewLine));
            PdfPTable table = new PdfPTable(2);
            table.WidthPercentage = 90;
            var bodyLeftCell = new PdfPCell();
            bodyLeftCell.Border = 0;
            bodyLeftCell.AddElement(
                new Paragraph("Distance : " + report.Distance.ToString(CultureInfo.InvariantCulture) + " Km."));
            bodyLeftCell.AddElement(
                new Paragraph("Vitesse moyene: " + report.AvgSpeed.ToString(CultureInfo.InvariantCulture) + " Km/h"));
            bodyLeftCell.AddElement(new Paragraph("Vitesse max: " + report.MaxSpeed + " Km/h"));

            var bodyRightCell = new PdfPCell() ;
            bodyRightCell.Border = 0;
            table.AddCell(bodyLeftCell);
            // table.AddCell(emptyCell);
            table.AddCell(bodyRightCell);
            table.DefaultCell.Border = 0;
            doc.Add(table);
            doc.Add(new Chunk(Environment.NewLine));
            p.Alignment = Element.ALIGN_CENTER;


            doc.Add(new Chunk(Environment.NewLine));
            doc.Add(new Chunk(Environment.NewLine));

            PdfPTable pTable = new PdfPTable(headers.Length);
            pTable.WidthPercentage = 90;
            pTable.SetWidths(columnWidths);
            foreach (string t in headers)
            {
                var cell = new PdfPCell(new Phrase(t, FontFactory.GetFont("Calibri", 12, BaseColor.BLACK)))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    BackgroundColor = new BaseColor(230, 230, 250)
                };


                pTable.AddCell(cell);
            }

            foreach (var position in report.Positions)
            {
                pTable.AddCell(
                    ItextSharpHelper.Cell(position.MotionStatus == "Stopped" ? "Arrêt" : "Conduite", BaseColor.BLACK));
                pTable.AddCell(ItextSharpHelper.Cell(DateTime.Parse(position.StartPeriod).ToShortTimeString().ToString(),
                    BaseColor.BLACK));
                pTable.AddCell(ItextSharpHelper.Cell(DateTime.Parse(position.EndPeriod).ToShortTimeString(), BaseColor.BLACK));
                pTable.AddCell(ItextSharpHelper.Cell(position.Distance.ToString(CultureInfo.InvariantCulture),
                    BaseColor.BLACK));
                pTable.AddCell(ItextSharpHelper.Cell(position.StartAddres, BaseColor.BLACK));
                pTable.AddCell(ItextSharpHelper.Cell(position.ArrivalAddres, BaseColor.BLACK));
                pTable.AddCell(ItextSharpHelper.Cell(position.MotionStatus != "Stopped" ? position.AvgSpeed.ToString(CultureInfo.InvariantCulture):"0.0",
                    BaseColor.BLACK));
            }

            //   doc.Add(p);
            doc.Add(pTable);
            doc.Close();
        }

    }
}
