using DBModels.Model;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TimeTracker.Utilities;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace TimeTracker.Form
{
    public partial class Login : System.Windows.Forms.Form
    {
        private const string stDomain = "@supremetechnologiesindia.com";
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

        private async void button1_Click(object sender, EventArgs e)
        {
            bool isAuthenticated = false; // Change to your actual authentication logic
            var email = textBox1.Text;
            var password = textBox2.Text;
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Email or password is required!");
            }
            else if (!email.Contains(stDomain))
            {
                MessageBox.Show("Please enter a valid email with company domain!");
            }
            else
            {
                isAuthenticated = await Authentication.GetATokenForGraph(email, password);
                if (isAuthenticated)
                {
                    // Authentication successful, close login form and open main application form
                    this.Hide(); // Hide the login form
                    Application appForm = new Application();
                    appForm.ShowDialog(); // Show the main application form
                    this.Close(); // Close the login form
                }
                else
                {
                    MessageBox.Show("Authentication failed. Please try again.");
                }
            }
        }
        private void Login_Load(object sender, EventArgs e)
        {

        }

        private async void button2_Click(object sender, EventArgs e)
        {
            string[] scopes = new string[] { "https://graph.microsoft.com/.default" };
            AuthenticationResult result = await publicClientApp.AcquireTokenInteractive(scopes).ExecuteAsync();
            if (result != null)
            {
                userManager.SaveUserInformation(result);
                this.Hide(); // Hide the login form
                Application appForm = new Application();
                appForm.ShowDialog(); // Show the main application form
                this.Close(); // Close the login form
            }
            else
            {
                MessageBox.Show("Authentication failed. Please try again.");
            }
        }
    }
}
