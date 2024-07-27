using System.Reflection;

public class ItemCatalogue : Item, IItemCatalogue {

    private const string DefaultRelativeItemDataPath = @"Data\files\items.txt";

    public ItemCatalogue(string relativeFilePath = DefaultRelativeItemDataPath) {
        LoadItemTableFromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, relativeFilePath));
    }

    private void LoadItemTableFromFile(string filePath) {
        string[] lines;
        try {
            
            lines = File.ReadAllLines(filePath);
        }
        catch (FileNotFoundException ex) {
            throw new Exception($"File at path {filePath} could not be found/loaded: ", ex);
        }

        foreach (string line in lines) {
            string[] parts = line.Split(':');

            var row = itemTable.NewRow();
            row[NumIdColumn] = parts[0].Trim();
            row[TextIdColumn] = parts[1].Trim();
            if (parts.Length > 2) {
                row[DisplayNameColumn] = parts[2].Trim();
            }
            itemTable.Rows.Add(row);
        }
    }

    public (string, string, string?) GetItemByNumId(string numId) {
        var row = itemTable.Select($"{NumIdColumn} = '{numId}'").First();
        return (row[NumIdColumn].ToString()!, row[TextIdColumn].ToString()!, row[DisplayNameColumn].ToString());
    }

    public (string, string, string?) GetItemByTextId(string textId) {
        var row = itemTable.Select($"{TextIdColumn} = '{textId}'").First();
        return (row[NumIdColumn].ToString()!, row[TextIdColumn].ToString()!, row[DisplayNameColumn].ToString());
    }
}
