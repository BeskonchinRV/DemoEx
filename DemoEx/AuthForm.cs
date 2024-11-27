using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace DemoEx
{
    public partial class AuthForm : Form
    {
        string connectionString = @"Data Source=(localdb)\MSSqlLocalDb;Initial Catalog=DemoEx;Integrated Security=True;";
        public AuthForm()
        {
            InitializeComponent();
        }

        private void AuthBtn_Click(object sender, EventArgs e)
        {
            string login = textBoxLogin.Text;
            string password = textBoxPass.Text;

            if (CheckLoginCredentials(login, password))
            {
                // Открываем MainForm, если логин и пароль верны
                MainForm mainForm = new MainForm();
                mainForm.Show();
                this.Hide(); // Скрыть окно авторизации
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль!");
            }
        }
        private bool CheckLoginCredentials(string login, string password)
        {
            bool isValid = false;
            string query = "SELECT COUNT(*) FROM Users WHERE Login = @Login AND Password = @Password";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Login", login);
                command.Parameters.AddWithValue("@Password", password);

                try
                {
                    connection.Open();
                    int result = (int)command.ExecuteScalar();
                    if (result > 0)
                    {
                        isValid = true;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка подключения к базе данных: " + ex.Message);
                }
            }

            return isValid;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
