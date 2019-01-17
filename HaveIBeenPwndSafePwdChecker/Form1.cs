using System;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using RestSharp;

namespace HaveIBeenPwndSafePwdChecker
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            String hash = Hash(textBox1.Text);
            var client = new RestClient("https://api.pwnedpasswords.com/");
            var request = new RestRequest("range/" + hash.Substring(0, 5), Method.GET);
            
            IRestResponse response = client.Execute(request);
            if (response.IsSuccessful)
            {
                var content = response.Content;
                if (content.Contains(hash.Substring(5)))
                {
                    label1.BackColor = Color.Red;
                    label1.Text = content.Substring(content.IndexOf(hash.Substring(5)) + 36, 15)
                        .Split(new[] { "\r\n" }, StringSplitOptions.None)[0];
                }
                else
                {
                    label1.BackColor = Color.Green;
                    label1.Text = "OK";
                }
            }
            else
            {
                label1.BackColor = Color.Yellow;
                label1.Text = "E: " + response.StatusCode;
            }

            button1.Enabled = true;
        }

        static string Hash(string input)
        {
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
                var sb = new StringBuilder(hash.Length * 2);

                foreach (byte b in hash)
                {
                    // can be "x2" if you want lowercase
                    sb.Append(b.ToString("X2"));
                }

                return sb.ToString();
            }
        }
    }
}
