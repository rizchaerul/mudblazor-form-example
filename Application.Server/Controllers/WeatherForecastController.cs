using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;

namespace Application.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    [HttpGet]
    public ActionResult Get()
    {
        for (int i = 0; i < 1; i++)
        {
            GeneratePDF($"Test\\Hello{i}.pdf");
        }

        return Ok();
    }

    void GeneratePDF(string fileName)
    {
        // Create a new document
        Document doc = new Document();
        try
        {
            // Create a PdfWriter
            PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(fileName, FileMode.Create));

            // Open the document
            doc.Open();

            // Add a title
            Paragraph title = new Paragraph("Monthly Sales Report", FontFactory.GetFont(FontFactory.HELVETICA, 24f, Font.BOLD));
            title.Alignment = Element.ALIGN_CENTER;
            doc.Add(title);

            // Add current date
            Paragraph date = new Paragraph(DateTime.Now.ToString("MMMM dd, yyyy"), FontFactory.GetFont(FontFactory.HELVETICA, 12f));
            date.Alignment = Element.ALIGN_RIGHT;
            doc.Add(date);

            // Add sales data
            PdfPTable table = new PdfPTable(3);
            table.WidthPercentage = 100;
            table.SpacingBefore = 20f;
            table.SpacingAfter = 20f;

            table.AddCell("Product");
            table.AddCell("Quantity");
            table.AddCell("Revenue");

            table.AddCell("Product A");
            table.AddCell("100");
            table.AddCell("$10,000");

            table.AddCell("Product B");
            table.AddCell("150");
            table.AddCell("$15,000");

            table.AddCell("Product C");
            table.AddCell("200");
            table.AddCell("$20,000");

            doc.Add(table);

            // Add QR code
            BarcodeQRCode qrCode = new BarcodeQRCode("https://www.example.com", 100, 100, null);
            Image qrImage = qrCode.GetImage();
            qrImage.Alignment = Element.ALIGN_CENTER;
            doc.Add(qrImage);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            // Close the document
            doc.Close();
        }

        Console.WriteLine("PDF created successfully.");
    }
}
