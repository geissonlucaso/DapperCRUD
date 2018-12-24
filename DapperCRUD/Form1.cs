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
using Dapper;

namespace DapperCRUD
{
    public partial class Form1 : Form
    {
        private SqlConnection con = new SqlConnection(@"Data Source=(localdb)\ProjectsV13;Initial Catalog=DapperDB;Integrated Security=True");
        int empId = 0;

        public Form1()
        {
            InitializeComponent();
        }

        //Carregamento do Form1
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                FillDataGridView();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Clear();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                //Se a conexão com o banco estiver fechada, abrir.
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                //Paramentros dinâmicos para executar procedure de EmpAddOrEdit.
                DynamicParameters param = new DynamicParameters();
                param.Add("@EmpID", empId);
                param.Add("@Name", txtName.Text.Trim());
                param.Add("@Mobile", txtMobile.Text.Trim());
                param.Add("@Address", txtAddress.Text.Trim());
                con.Execute("EmpAddOrEdit", param, commandType: CommandType.StoredProcedure);

                if (empId == 0)
                {
                    MessageBox.Show("Saved Successfully!");
                }
                else
                {
                    MessageBox.Show("Updated Successfully!");
                }

                FillDataGridView();

                Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        //Botão Search
        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataGridView();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        
        //Botão cancelar
        private void btnCancel_Click(object sender, EventArgs e)
        {
            Clear();
        }

        //Método para retornar a lista de empregados
        void FillDataGridView()
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@SearchText", txtSearch.Text.Trim());

            List<Employee> list = con.Query<Employee>("EmpViewOrSearch", param,
                commandType: CommandType.StoredProcedure).ToList<Employee>();
            dgvEmployee.DataSource = list;
            dgvEmployee.Columns[0].Visible = false;
        }

        //Método Clear() - para limpar campos
        void Clear()
        {
            txtName.Text = "";
            txtMobile.Text = "";
            txtAddress.Text = "";
            txtSearch.Text = "";
            empId = 0;
            btnSave.Text = "Save";
            btnDelete.Enabled = false;
        }

        //Comando executado ao dar double click no dgvEmployee
        private void dgvEmployee_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                if (dgvEmployee.CurrentRow.Index != -1)
                {
                    empId = Convert.ToInt32(dgvEmployee.CurrentRow.Cells[0].Value.ToString());
                    txtName.Text = dgvEmployee.CurrentRow.Cells[1].Value.ToString();
                    txtMobile.Text = dgvEmployee.CurrentRow.Cells[2].Value.ToString();
                    txtAddress.Text = dgvEmployee.CurrentRow.Cells[3].Value.ToString();
                    btnSave.Text = "Update";
                    btnDelete.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Are you sure you want to delete this record?", "Message", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@EmpID", empId);
                    con.Execute("EmpDeleteByID", param, commandType: CommandType.StoredProcedure);
                    Clear();
                    FillDataGridView();

                    MessageBox.Show("Deleted Successfully");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
