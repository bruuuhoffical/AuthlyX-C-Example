using System;
using System.Windows.Forms;
using AuthlyXClient;
namespace AuthlyX_C__Example__Form_
{
    public partial class Home : Form
    {
        public Home()
        {
            InitializeComponent();
        }

        private void guna2ControlBox1_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
            Application.Exit();
        }

        private void Home_Load(object sender, EventArgs e)
        {
            var data = Login.AuthlyXApp.userData;
            username.Text = data.Username;
            email.Text = data.Email;
            license_key.Text = data.LicenseKey;
            ipaddress.Text = data.IpAddress;
            sub.Text = data.Subscription;
            registered_at.Text = data.RegisteredAt;
            expiry.Text = data.ExpiryDate;
            last_login.Text = data.LastLogin;
        }
    }
}
