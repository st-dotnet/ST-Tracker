using Microsoft.Identity.Client;
using System;
using System.Windows.Forms;
using TimeTracker.Utilities;

namespace TimeTracker.Form
{
    public partial class Login : System.Windows.Forms.Form
    {
        private static IPublicClientApplication publicClientApp;
        private readonly UserInformationManager userManager;
        public Login()
        {
            InitializeComponent();
            publicClientApp = PublicClientApplicationBuilder
                .Create("7244e5a8-349a-4d53-81c1-ae8f35c37ddf")
                .WithAuthority("https://login.microsoftonline.com/04b31863-f802-42b0-a7a9-be8dde194ef5/oauth2/v2.0/token")
                .WithRedirectUri("http://localhost")
                .Build();
            userManager = new UserInformationManager("user_info.xml");
        }
        private void Login_Load(object sender, EventArgs e)
        {
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            //string[] scopes = new string[] { "https://graph.microsoft.com/.default" };
            //AuthenticationResult result = await publicClientApp.AcquireTokenInteractive(scopes)
            //       .WithParentActivityOrWindow(this)
            //       .WithUseEmbeddedWebView(false)
            //       .ExecuteAsync();
            //if (result != null)
            //{
            //    DBAccessContext dBAccessContext = new DBAccessContext();
            //    var empId = await dBAccessContext.GetEmployeeId(result.Account.Username);
            //    userManager.SaveUserInformation(result, empId);
                this.Hide(); // Hide the login form
                Application appForm = new Application();
                appForm.ShowDialog(); // Show the main application form
                this.Close(); // Close the login form
            //}
            //else
            //{
            //    MessageBox.Show("Authentication failed. Please try again.");
            //}
        }
    }
}
