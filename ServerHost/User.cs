namespace ServerHost
{
    public class User
    {
        #region Properties
        public string? Login { get; set; }
        public bool IsLoggedIn { get; set; }
        public string IPAddress { get; private set; }
        public int Port { get; private set; }
        public Task LoreTask { get; set; }
        public CancellationTokenSource TokenSource { get; set; }
        #endregion

        #region Constructors
        public User(string ipAddress, int port)
        {
            IPAddress = ipAddress;
            Port = port;
            TokenSource = new CancellationTokenSource();
        }
        #endregion

        #region Public Methods
        public void SetLogin(string login)
        {
            Login = login;
        }

        public void CancelLOR()
        {
            TokenSource?.Cancel();
        }
        public void ContinueLOR()
        {
            TokenSource = new CancellationTokenSource();
        }

        public string GetLogin() => Login;
        #endregion
    }
}
