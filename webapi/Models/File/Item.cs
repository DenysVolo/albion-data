using System.Data;

public class Item {
    protected DataTable itemTable;

    protected const string NumIdColumn = "itemNumId";
    protected const string TextIdColumn = "itemTextId";
    protected const string DisplayNameColumn = "displayName";

    public Item() {
        itemTable  = new DataTable("Items");

        DataColumn column;

        // Item numerical ID
        column = new DataColumn(NumIdColumn, typeof(string))
        {
            Unique = true
        };
        itemTable.Columns.Add(column);

        // Item text ID
        column = new DataColumn(TextIdColumn, typeof(string))
        {
            Unique = true
        };
        itemTable.Columns.Add(column);

        // Display name
        column = new DataColumn(DisplayNameColumn, typeof(string));
        itemTable.Columns.Add(column);

        // Make the ID columns the primary key columns
        DataColumn[] PrimaryKeyColumns = [itemTable.Columns[NumIdColumn]!, itemTable.Columns[TextIdColumn]!];
        itemTable.PrimaryKey = PrimaryKeyColumns;
    }
}