using System.ServiceModel;
namespace ServerLibrary
{
    public class ServerUser
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public OperationContext OperContext { get; set; }
    }
}
