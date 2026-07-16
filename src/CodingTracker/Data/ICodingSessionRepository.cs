using CodingTracker.Models;

namespace CodingTracker.Data;

public interface ICodingSessionRepository
{
    void InitializeDatabase();
    int Create(CodingSession session);
    List<CodingSession> GetAll();
    bool Update(CodingSession session);
    bool Delete(int id);
}
