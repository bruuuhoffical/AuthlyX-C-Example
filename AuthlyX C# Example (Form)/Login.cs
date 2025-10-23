using AuthlyXClient;
using Guna.UI2.WinForms;
using System;
using System.Net.Sockets;
using System.Windows.Forms;
namespace AuthlyX_C__Example__Form_
{
    public partial class Login : Form
    {
        public static auth AuthlyXApp = new auth(
            ownerId: "469e4d9235d1",
            appName: "BASIC",
            version: "1.0.0",
            secret: "iqcmyagw1skGdgk6Nq7OxxpX5iAmC2Hlwq7iNwyG"
        );
        public Login()
        {
            InitializeComponent();
        }

        private async void Login_Load(object sender, EventArgs e)
        {
            await AuthlyXApp.Init();
            if (AuthlyXApp.response.success)
            {
                //MessageBox.Show("Successfully connected to AuthlyX!", "Success",MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show($"Initialization failed: {AuthlyXApp.response.message}","Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
            await AuthlyXApp.Log("Initialized");
        }

        private async void btn_Login_Click(object sender, EventArgs e)
        {
            string username = txtUser.Text;
            string password = txtPass.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both username and password",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            this.Enabled = false;
            btn_Login.Text = "Logging in...";

            try
            {
                await AuthlyXApp.Login(username, password);

                if (AuthlyXApp.response.success)
                {
                    MessageBox.Show($"Welcome {AuthlyXApp.userData.Username}!",
                        "Login Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    Home home = new Home();
                    home.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show($"Login failed: {AuthlyXApp.response.message}",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during login: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Enabled = true;
                btn_Login.Text = "Login";
            }
        }

        private async void btn_Register_Click(object sender, EventArgs e)
        {
            string username = txt_regUser.Text;
            string password = txt_regPass.Text;
            string key = txt_regKey.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(key))
            {
                MessageBox.Show("Please enter both username, password and key",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            this.Enabled = false;
            btn_Login.Text = "Registering...";

            try
            {
                await AuthlyXApp.Register(username, password, key);

                if (AuthlyXApp.response.success)
                {
                    MessageBox.Show($"Welcome {AuthlyXApp.userData.Username}!",
                        "Registration Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    Home home = new Home();
                    home.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show($"Registration failed: {AuthlyXApp.response.message}",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during Registration: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Enabled = true;
                btn_Login.Text = "Register";
            }
        }

        private async void btn_simpleLogin_Click(object sender, EventArgs e)
        {
            string key = txt_Key.Text;

            if (string.IsNullOrEmpty(key))
            {
                MessageBox.Show("Please enter a key to continue",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            this.Enabled = false;
            btn_Login.Text = "Logging in...";

            try
            {
                await AuthlyXApp.LicenseLogin(key);

                if (AuthlyXApp.response.success)
                {
                    MessageBox.Show($"Welcome {AuthlyXApp.userData.LicenseKey}!",
                        "Login Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    Home home = new Home();
                    home.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show($"Login in failed: {AuthlyXApp.response.message}",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during Login: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Enabled = true;
                btn_Login.Text = "Login";
            }
        }
    }
}
