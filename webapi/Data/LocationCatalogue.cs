using System.Reflection;

public class LocationCatalogue : Location, ILocationCatalogue {

    private const string DefaultRelativeItemDataPath = @"Data\files\world.txt";

    public LocationCatalogue(string relativeFilePath = DefaultRelativeItemDataPath) {
        LoadLocationTableFromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, relativeFilePath));
    }

    private void LoadLocationTableFromFile(string filePath) {
        string[] lines;
        try {
            
            lines = File.ReadAllLines(filePath);
        }
        catch (FileNotFoundException ex) {
            throw new Exception($"File at path {filePath} could not be found/loaded: ", ex);
        }

        foreach (string line in lines) {
            string[] parts = line.Split(':');

            var row = locationTable.NewRow();
            row[LocationIdColumn] = parts[0].Trim();
            if (parts.Length > 1) {
                row[DisplayNameColumn] = parts[1].Trim();
            }
            locationTable.Rows.Add(row);
        }
    }

    public (string, string?) GetLocationById(string locationId) {
        var row = locationTable.Select($"{LocationIdColumn} = '{locationId}'").First();
        return (row[LocationIdColumn].ToString()!, row[DisplayNameColumn].ToString());
    }
}
