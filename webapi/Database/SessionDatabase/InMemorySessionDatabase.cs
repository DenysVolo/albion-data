using System.Data;

public class InMemorySessionDatabase : ISessionDatabase{
    protected DataTable sessionTable;

    protected const string SessionIdColumn = "sessionId";
    protected const string RequestTypeColumn = "requestType";
    protected const string RequestDetailsColumn = "requestDetails";
    protected const string RequestDataColumn = "requestData";
    protected const string SessionStaleColumn = "sessionStale";

    public InMemorySessionDatabase() {
        sessionTable  = new DataTable("Sessions");

        DataColumn column;

        // Session UID
        column = new DataColumn(SessionIdColumn, typeof(string))
        {
            Unique = true
        };
        sessionTable.Columns.Add(column);

        // Request type (market, order)
        column = new DataColumn(RequestTypeColumn, typeof(string));
        sessionTable.Columns.Add(column);

        // Request details
        column = new DataColumn(RequestDetailsColumn, typeof(string));
        sessionTable.Columns.Add(column);

        // Returned request data
        column = new DataColumn(RequestDataColumn, typeof(DataTable));
        sessionTable.Columns.Add(column);

        // Is session stale
        column = new DataColumn(SessionStaleColumn, typeof(bool));
        sessionTable.Columns.Add(column);

        // Make the Session UID the primary key
        sessionTable.PrimaryKey = [sessionTable.Columns[SessionIdColumn]!];
    }

    public bool DoesSessionIdExist(string sessionId) {
        return sessionTable.Select($"{SessionIdColumn} = '{sessionId}'").Length > 0;
    }

    public bool IsSessionIdActive(string sessionId) {
        return sessionTable.Select($"{SessionIdColumn} = '{sessionId}' AND {SessionStaleColumn} = 'false'").Length > 0;
    }

    public string GetRequestType(string sessionId) {
        var row = sessionTable.Select($"{SessionIdColumn} = '{sessionId}'").First();
        return (string)row[RequestTypeColumn];
    }

    public string GetRequestDetails(string sessionId) {
        var row = sessionTable.Select($"{SessionIdColumn} = '{sessionId}'").First();
        return (string)row[RequestDetailsColumn];
    }

    public DataTable GetRequestData(string sessionId) {
        if (!IsSessionIdActive(sessionId)) {
            throw new Exception("Data from given sessionId is stale");
        }
        var row = sessionTable.Select($"{SessionIdColumn} = '{sessionId}'").First();
        return (DataTable)row[RequestDataColumn];
    }

    public string CreateSession(
        string requestType,
        string requestDetails,
        DataTable requestData) 
    {
        string sessionId;
        for (int i = 0; i < 5; i++) {
            sessionId =  Guid.NewGuid().ToString();
            if (!DoesSessionIdExist(sessionId)) {
                UpsertSession(sessionId, requestType, requestDetails, requestData);
                return sessionId;
            }
        }
        throw new Exception("Somehow 5 sessionId guid clashes happened 5 times in a row...");

    }

    public void UpsertSession(
        string sessionId,
        string requestType,
        string requestDetails,
        DataTable requestData) 
    {        
        var row = sessionTable.NewRow();
        row[SessionIdColumn] = sessionId;
        row[RequestTypeColumn] = requestType;
        row[RequestDetailsColumn] = requestDetails;
        row[RequestDataColumn] = requestData;
        row[SessionStaleColumn] = false;

        if (DoesSessionIdExist(sessionId)) {
            SafeRemoveSession(sessionId);
        }
        sessionTable.Rows.Add(row);
    }

    public void SafeRemoveSession(string sessionId) {
        if(DoesSessionIdExist(sessionId)) {
            var row = sessionTable.Select($"{SessionIdColumn} = '{sessionId}'").First();
            sessionTable.Rows.Remove(row);
        }
    }
}