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
        int EmpId = 0;

        public Form1()
        {
            InitializeComponent();
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
                param.Add("@EmpID", EmpId);
                param.Add("@Name", txtName.Text.Trim());
                param.Add("@Mobile", txtMobile.Text.Trim());
                param.Add("@Address", txtAddress.Text.Trim());
                con.Execute("EmpAddOrEdit", param, commandType: CommandType.StoredProcedure);
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

        void FillDataGridView()
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@SearchText", txtSearch.Text.Trim());

            List<Employee> list = con.Query<Employee>("EmpViewOrSearch", param, 
                commandType: CommandType.StoredProcedure).ToList<Employee>();
        }

        class Employee
        {
            public int EmpID { get; set; }
            public string Name { get; set; }
            public string Mobile { get; set; }
            public string Address { get; set; }
        }
    }
}
