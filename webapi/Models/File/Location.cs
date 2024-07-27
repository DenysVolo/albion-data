using System.Data;

public class Location {
    public DataTable locationTable;

    protected const string LocationIdColumn = "locationId";
    protected const string DisplayNameColumn = "displayName";

    public Location() {
        locationTable  = new DataTable("Locations");

        DataColumn column;

        // Location ID
        column = new DataColumn(LocationIdColumn, typeof(string))
        {
            Unique = true
        };
        locationTable.Columns.Add(column);

        // Display name
        column = new DataColumn(DisplayNameColumn, typeof(string));
        locationTable.Columns.Add(column);

        // Make the ID column the primary key column
        DataColumn[] PrimaryKeyColumns = [locationTable.Columns[LocationIdColumn]!];
        locationTable.PrimaryKey = PrimaryKeyColumns;
    }
}