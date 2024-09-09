using System.Data;

public interface ISessionDatabase {
    bool DoesSessionIdExist(string sessionId);

    bool IsSessionIdActive(string sessionId);

    string GetRequestType(string sessionId);

    string GetRequestDetails(string sessionId);

    DataTable GetRequestData(string sessionId);

    int GetSessionLimit(string sessionId);

    string CreateSession(
        string requestType,
        string requestDetails,
        DataTable requestData,
        int limit);

    void UpsertSession(
        string sessionId,
        string requestType,
        string requestDetails,
        DataTable requestData,
        int limit);
        
    void SafeRemoveSession(string sessionId);
}