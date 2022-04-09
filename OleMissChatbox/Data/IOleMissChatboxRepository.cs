using OleMissChatbox.Data.Entities;

namespace OleMissChatbox.Data
{
    public interface IOleMissChatboxRepository
    {
        User GetUserByEmail(string email);
        bool AddUser(string firstname, string lastname, string email, string password);
    }
}
