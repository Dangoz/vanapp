namespace Server.Models
{
    public class Connection
    {
        //@@231/239 Generated a default constructor to avoid errors 
        public Connection()
        {
        }

        public Connection(string connectionId, string username)
        {
            ConnectionId = connectionId;
            Username = username;
        }

        public string ConnectionId {get; set;}
        public string Username {get; set;}

    }
}