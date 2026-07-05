using Newtonsoft.Json;
using System.Text;

var currentDirectory = Directory.GetCurrentDirectory();
var storesDirectory = Path.Combine(currentDirectory, "stores");

// Create output folder
var salesTotalDir = Path.Combine(currentDirectory, "salesTotalDir");
Directory.CreateDirectory(salesTotalDir);

// Find all JSON sales files
var salesFiles = FindFiles(storesDirectory);

// Calculate total sales and create report
var salesTotal = CalculateSalesTotal(salesFiles);

// Save the overall sales total
File.WriteAllText(
    Path.Combine(salesTotalDir, "totals.txt"),
    salesTotal.ToString("C")
);

IEnumerable<string> FindFiles(string folderName)
{
    List<string> salesFiles = new List<string>();

    var foundFiles = Directory.EnumerateFiles(
        folderName,
        "*",
        SearchOption.AllDirectories);

    foreach (var file in foundFiles)
    {
        if (Path.GetExtension(file) == ".json")
        {
            salesFiles.Add(file);
        }
    }

    return salesFiles;
}

double CalculateSalesTotal(IEnumerable<string> salesFiles)
{
    double salesTotal = 0;

    Dictionary<string, double> salesSummary = new Dictionary<string, double>();

    foreach (var file in salesFiles)
    {
        string salesJson = File.ReadAllText(file);

        SalesData? data =
            JsonConvert.DeserializeObject<SalesData>(salesJson);

        double total = data?.Total ?? 0;

        salesTotal += total;

        // Use the relative path so every key is unique
        string relativePath = Path.GetRelativePath(currentDirectory, file);

        salesSummary[relativePath] = total;
    }

    GenerateSalesSummary(salesSummary, salesTotal);

    return salesTotal;
}

void GenerateSalesSummary(
    Dictionary<string, double> salesSummary,
    double totalSales)
{
    StringBuilder report = new StringBuilder();

    report.AppendLine("Sales Summary");
    report.AppendLine("");
    report.AppendLine($"Total Sales: {totalSales:C}");
    report.AppendLine();
    report.AppendLine("Details:");

    foreach (var sale in salesSummary)
    {
        report.AppendLine($"{sale.Key}: {sale.Value:C}");
    }

    string reportPath = Path.Combine(
        salesTotalDir,
        "SalesSummary.txt");

    File.WriteAllText(reportPath, report.ToString());

    Console.WriteLine("Sales summary report created successfully.");
    Console.WriteLine($"Report saved to: {reportPath}");
}

record SalesData(double Total);