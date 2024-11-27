using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DemoEx
{
    public partial class EditForm : Form
    {
        string connectionString = @"Data Source=(localdb)\MSSqlLocalDb;Initial Catalog=DemoEx;Integrated Security=True;";
        private int userId;

        public EditForm()
        {
            InitializeComponent();
            LoadRoles();
        }

        public EditForm(int id, string name, string login, string password, string role) : this()
        {
            userId = id; // Сохраняем ID для редактирования
            textBoxName.Text = name;
            textBoxLogin.Text = login;
            textBoxPass.Text = password;
            comboBoxRole.SelectedItem = role;
        }

        private void LoadRoles()
        {
            // Предполагаем, что роли фиксированы
            comboBoxRole.Items.Add("Admin");
            comboBoxRole.Items.Add("User");
            comboBoxRole.Items.Add("Moder");
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            // Проверка ввода
            if (string.IsNullOrWhiteSpace(textBoxName.Text) ||  string.IsNullOrWhiteSpace(textBoxLogin.Text) || comboBoxRole.SelectedItem == null)
            {
                MessageBox.Show("Заполните все поля.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    if (userId == 0)  // Check for new user
                    {
                        // Получаем максимальное значение ID
                        string getMaxIdQuery = "SELECT ISNULL(MAX(ID), 0) + 1 FROM Users";
                        SqlCommand getMaxIdCommand = new SqlCommand(getMaxIdQuery, connection);
                        int newId = Convert.ToInt32(getMaxIdCommand.ExecuteScalar());

                        // Добавление нового пользователя
                        string query = "INSERT INTO Users (ID, Name, Login, Password, Role) VALUES (@ID, @Name, @Login,@Password, @Role)";
                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@ID", newId);
                        command.Parameters.AddWithValue("@Name", textBoxName.Text.Trim());
                        command.Parameters.AddWithValue("@Login", textBoxLogin.Text.Trim());
                        command.Parameters.AddWithValue("@Password", textBoxPass.Text.Trim());
                        command.Parameters.AddWithValue("@Role", comboBoxRole.SelectedItem.ToString());
                        command.ExecuteNonQuery();
                    }
                    else
                    {
                        // Редактирование существующего пользователя
                        string query = "UPDATE Users SET Name = @Name, Login = @Login,Password = @Password Role = @Role WHERE ID = @ID";
                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@ID", userId); // ID передаётся только как параметр WHERE
                        command.Parameters.AddWithValue("@Name", textBoxName.Text.Trim());
                        command.Parameters.AddWithValue("@Login", textBoxLogin.Text.Trim());
                        command.Parameters.AddWithValue("@Password", textBoxPass.Text.Trim());
                        command.Parameters.AddWithValue("@Role", comboBoxRole.SelectedItem.ToString());
                        command.ExecuteNonQuery();
                    }
                }

                // Закрываем форму и возвращаем результат
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            // Закрываем форму при нажатии на кнопку "Отмена"
            Close();
        }

        private void textBoxName_TextChanged(object sender, EventArgs e)
        {

        }

        private void comboBoxRole_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
