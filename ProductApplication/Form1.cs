using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace ProductApplication
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        string connectionString = ConfigurationManager.ConnectionStrings["ProductManagementConnectionString"].ConnectionString;

        private bool isUpdateMode = false;
        private string productCode ;

       

        private void saveButton_Click(object sender, EventArgs e)
        {
            string proCode = productCodeTextBox.Text;
            string proDescription = productDiscriptionTextBox.Text;
            string proQuentity = productQuantityTextBox.Text;

            if (isUpdateMode)
            {
                SqlConnection connection = new SqlConnection(connectionString);

                string query = "UPDATE product_info_table SET productDescription='" + proDescription + "', productQuantity='" + proQuentity + "' WHERE productCode='" + productCode + "'";

                SqlCommand command = new SqlCommand(query, connection);

                connection.Open();
                int rowAffected = command.ExecuteNonQuery();
                connection.Close();

                if (rowAffected > 0)
                {
                    MessageBox.Show(@"Update successfull ..!");

                    saveButton.Text = "Save";
                    productCode ="";
                    isUpdateMode = false;
                    ShowAllProduct();
                }
                else
                {
                    MessageBox.Show(@"Update Fail..!");
                }
                productCodeTextBox.Clear();
                productDiscriptionTextBox.Clear();
                productQuantityTextBox.Clear();

            }
            else
            {
                if (proCode == "" && proDescription == "" && proQuentity == "")
                {
                    MessageBox.Show(@"Please insert Product Information");
                }
                else
                {
                    int proQuenti = Convert.ToInt32(proQuentity);
                    if (IsProductCodeExits(proCode))
                    {
                        MessageBox.Show(@"Product Code already exists!");
                        return;
                    }

                    if (proCode.Length == 3)
                    {
                        SqlConnection connection = new SqlConnection(connectionString);

                        string query = "INSERT INTO product_info_table(productCode, productDescription, productQuantity) VALUES('" + proCode + "','" + proDescription + "','" + proQuenti + "')";

                        SqlCommand command = new SqlCommand(query, connection);

                        connection.Open();
                        int rowAffected = command.ExecuteNonQuery();
                        connection.Close();

                        if (rowAffected > 0)
                        {
                            MessageBox.Show(@"Data successfully Insert...!");
                            ShowAllProduct();
                            productCodeTextBox.Clear();
                            productDiscriptionTextBox.Clear();
                            productQuantityTextBox.Clear();
                        }
                        else
                        {
                            MessageBox.Show(@"Data Insertion Fail..!");
                        }
                    }
                    else
                    {
                        MessageBox.Show(@"Please insert only 3 length code.");
                    }
                }
            }

            

        }

        public bool IsProductCodeExits(string proCode)
        {
           
            SqlConnection connection = new SqlConnection(connectionString);
            
            string query = "SELECT * FROM product_info_table where productCode='"+proCode+"'";

            SqlCommand command = new SqlCommand(query, connection);

            connection.Open();
            SqlDataReader reader = command.ExecuteReader();

            bool isProductCodeExits = false;
            while (reader.Read())
            {
                isProductCodeExits = true;
                break;
            }
            reader.Close();
            connection.Close();

            return isProductCodeExits;
        }

       

        public void ProductListView(List<Product> newProducts)
        {
            int quan = 0;
            int total = 0;
            showProductListView.Items.Clear();
            foreach (var product in newProducts)
            {
                ListViewItem item = new ListViewItem(product.prodCode);
                item.SubItems.Add(product.prodDescription);
                item.SubItems.Add(product.prodQuantity.ToString());
                showProductListView.Items.Add(item);
                quan = product.prodQuantity;
                total = total + quan;
            }
            totalQuantityTextBox.Text = Convert.ToString(total);
        }

        public void ShowAllProduct()
        {
            SqlConnection connection = new SqlConnection(connectionString);

            string query = "SELECT * FROM product_info_table";

            SqlCommand command = new SqlCommand(query, connection);

            connection.Open();
            SqlDataReader reader = command.ExecuteReader();

            List<Product> newProducts = new List<Product>();

            while (reader.Read())
            {
                Product product = new Product();
                product.prodCode = reader["productCode"].ToString();
                product.prodDescription = reader["productDescription"].ToString();
                product.prodQuantity = int.Parse(reader["productQuantity"].ToString());

                newProducts.Add(product);
            }
            reader.Close();
            connection.Close();

            ProductListView(newProducts);

        }

        private void showProductListView_DoubleClick(object sender, EventArgs e)
        {
            ListViewItem item = showProductListView.SelectedItems[0];

            string proCode = item.Text.ToString();

            Product product = GetProductByCode(proCode);

            if (product!=null)
            {
                isUpdateMode = true;
                saveButton.Text = @"Update";
                productCodeTextBox.ReadOnly = true;
                productCode = product.prodCode;

                productCodeTextBox.Text = product.prodCode;
                productDiscriptionTextBox.Text = product.prodDescription;
                productQuantityTextBox.Text = Convert.ToString(product.prodQuantity);
            }


        }

        public Product GetProductByCode(string proCode)
        {
            SqlConnection connection = new SqlConnection(connectionString);

            string query = "SELECT * FROM product_info_table where productCode ='"+proCode+"'";

            SqlCommand command = new SqlCommand(query, connection);

            connection.Open();
            SqlDataReader reader = command.ExecuteReader();


            List<Product> newProducts = new List<Product>();

            while (reader.Read())
            {
                Product product = new Product();
                product.prodCode = reader["productCode"].ToString();
                product.prodDescription = reader["productDescription"].ToString();
                product.prodQuantity = int.Parse(reader["productQuantity"].ToString());

                newProducts.Add(product);
            }
            reader.Close();
            connection.Close();

            return newProducts.FirstOrDefault();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ShowAllProduct();
        }
    }
}
