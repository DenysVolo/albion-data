using System.Data;

public interface ISessionDatabase {
    bool DoesSessionIdExist(string sessionId);

    bool IsSessionIdActive(string sessionId);

    string GetRequestType(string sessionId);

    string GetRequestDetails(string sessionId);

    DataTable GetRequestData(string sessionId);

    string CreateSession(
        string requestType,
        string requestDetails,
        DataTable requestData);

    void UpsertSession(
        string sessionId,
        string requestType,
        string requestDetails,
        DataTable requestData);
        
    void SafeRemoveSession(string sessionId);
}