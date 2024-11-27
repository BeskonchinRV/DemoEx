using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DemoEx
{
    public partial class MainForm : Form
    {
        string connectionString = @"Data Source=(localdb)\MSSqlLocalDb;Initial Catalog=DemoEx;Integrated Security=True;";

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            comboBoxRole.Items.Add("Все");
            comboBoxRole.Items.Add("Admin");
            comboBoxRole.Items.Add("User");
            comboBoxRole.Items.Add("Moder");
            comboBoxRole.SelectedItem = "Все";

            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            LoadUsersData();
        }

        private void LoadUsersData()
        {
            string query = BuildQuery();
            ExecuteQuery(query);
        }

        // Строим SQL запрос с фильтрами и сортировкой
        private string BuildQuery()
        {
            string searchTerm = textBoxSearch.Text.Trim();
            string selectedRole = comboBoxRole.SelectedItem.ToString();
            string sortOrder = GetSortOrder();

            string query = "SELECT * FROM Users WHERE 1 = 1";

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query += " AND Name LIKE @SearchTerm";
            }

            if (selectedRole != "Все")
            {
                query += " AND Role = @Role";
            }

            if (!string.IsNullOrEmpty(sortOrder))
            {
                query += " ORDER BY Name " + sortOrder;
            }

            return query;
        }

        // Выполняем SQL-запрос и выводим данные в DataGridView
        private void ExecuteQuery(string query)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@SearchTerm", "%" + textBoxSearch.Text.Trim() + "%");
                command.Parameters.AddWithValue("@Role", comboBoxRole.SelectedItem.ToString());

                SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                try
                {
                    dataAdapter.Fill(dataTable);
                    dataGridView1.DataSource = dataTable;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка загрузки данных: " + ex.Message);
                }
            }
        }

        private string GetSortOrder()
        {
            if (radioButtonAZ.Checked)
            {
                return "ASC";
            }
            else if (radioButtonZA.Checked)
            {
                return "DESC";
            }

            return string.Empty;
        }

        private void textBoxSearch_TextChanged(object sender, EventArgs e)
        {
            LoadUsersData();
        }

        private void comboBoxRole_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadUsersData();
        }

        private void radioButtonAZ_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonAZ.Checked)
            {
                LoadUsersData();
            }
        }

        private void radioButtonZA_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonZA.Checked)
            {
                LoadUsersData();
            }
        }

        private void buttonAddUser_Click(object sender, EventArgs e)
        {
            EditForm editForm = new EditForm();
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                // После добавления обновляем данные
                LoadUsersData();
            }
        }

        private void buttonEditUser_Click(object sender, EventArgs e)
        {
            // Проверяем, выбрана ли строка
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите строку для редактирования.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Получаем данные выбранной строки
            DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
            int id = Convert.ToInt32(selectedRow.Cells[0].Value);
            string name = selectedRow.Cells[1].Value.ToString();
            string login = selectedRow.Cells[2].Value.ToString();
            string password = selectedRow.Cells[3].Value.ToString();
            string role = selectedRow.Cells[4].Value.ToString();

            // Открываем форму для редактирования данных
            EditForm editForm = new EditForm(id, name, login, password, role);
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                // После редактирования обновляем данные
                LoadUsersData();
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            // Проверяем, выбрана ли строка
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите строку для удаления.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Получаем данные выбранной строки
            DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
            int id = Convert.ToInt32(selectedRow.Cells[0].Value);

            // Подтверждение удаления
            DialogResult dialogResult = MessageBox.Show($"Вы уверены, что хотите удалить пользователя с ID {id}?",
                                                         "Подтверждение удаления",
                                                         MessageBoxButtons.YesNo,
                                                         MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                string connectionString = @"Data Source=(localdb)\MSSqlLocalDb;Initial Catalog=DemoEx;Integrated Security=True;";

                // SQL-запрос для удаления строки
                string query = "DELETE FROM Users WHERE ID = @ID";

                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@ID", id);
                            command.ExecuteNonQuery();
                        }
                    }

                    // Обновляем данные в DataGridView
                    LoadUsersData();

                    MessageBox.Show("Пользователь успешно удалён.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
