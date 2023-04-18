using hotel_management_app.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace hotel_management_app.Forms.Authenticate
{
    public partial class LoginForm : Form
    {
        private readonly HttpClient _client = new HttpClient();
        public LoginForm()
        {
            InitializeComponent();
        }
        private void LoginForm_Load(object sender, EventArgs e)
        {
            _client.BaseAddress = new Uri(SystemConfig.BaseApi);
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        /// <summary>
        /// Button login click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnLogin_ClickAsync(object sender, EventArgs e)
        {
            try
            {
                string username = txtUsername.Text;
                string password = txtPassword.Text;

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    MessageBox.Show("Vui lòng điền đầy đủ thông tin đăng nhập?", "HotelYY", MessageBoxButtons.OK);
                    return;
                }
                var loginRequest = new
                {
                    username = username,
                    password = password
                };
                var byteContent = GetByteArrayContentFromObject(loginRequest);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                HttpResponseMessage response = await _client.PostAsync("api/UserAuthenticate/UserLogin", byteContent);
                response.EnsureSuccessStatusCode();

                dynamic jsonRes = JObject.Parse(await response.Content.ReadAsStringAsync());
                if(jsonRes.code == "Oke")
                {
                    var token = (string)jsonRes.data.token;
                    Properties.Settings.Default.token = token;
                    Properties.Settings.Default.Save();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        /// <summary>
        /// Convert object to ByteArrayContent
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public ByteArrayContent GetByteArrayContentFromObject(object obj) 
        {
            var objByJson = JsonConvert.SerializeObject(obj);
            var buffer = Encoding.UTF8.GetBytes(objByJson);
            var byteContent = new ByteArrayContent(buffer);
            return byteContent;
        }
    }
}
