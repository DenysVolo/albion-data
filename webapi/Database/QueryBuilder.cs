public class QueryBuilder {
    // List of colums to select from, default to *
    public List<string> SelectColumns { get; set; }

    // List of tables to select from
    public List<string> FromTables { get; set; }

    // List of where attributes (e.g. item=@p1 | item>@p1 | item BETWEEN @p1 AND @p2)
    public List<string> WhereAttribute { get; set; }

    // Limit on query
    public int? Limit { get; set; }

    public QueryBuilder(){
        SelectColumns = [];
        FromTables = [];
        WhereAttribute = [];
    }

    public string BuildQuery() {
        // At least one FROM table must be present
        if (FromTables.Count == 0) {
            throw new MissingFieldException("FromTables must contain at least one table to select from");
        }

        // Select clause
        var query = "SELECT";

        if (SelectColumns.Count == 0) {
            query += " *";
        }
        else {
            query += $" ({string.Join(",", SelectColumns)})";
        }

        // From clause
        query += $" FROM {string.Join(",", FromTables)}";


        // Where clause
        if (WhereAttribute.Count == 1) {
            query += $" WHERE ({WhereAttribute[0]})";
        }

        if (WhereAttribute.Count > 1) {
            query += $" WHERE ({string.Join(") AND (", WhereAttribute)})";
        }

        // Limit clause
        if (Limit != null) {
            query += $" LIMIT {Limit}";
        }

        return query;
    }
}