//Title: Ebay Cards
//Date: 08/20/2021
//Developer: Josh Thrash
//Purpose:  This Window Form Application allows the user to input items that are actively being sold on Ebay.
//          The user can keep track of starting price and type of selling method in order to see which type sells best
//          Once the card is sold the user is able to update the Sql database in order to show the final price of items sold.
//          The user can also Update and Delete entires whenever needed.  The user is also able to search the active database
//          With particular parameters, view total value of sold items, and view total number of active items

using System;
using System.Data;
using System.Configuration;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace EbayCards
{
    public partial class EbayCards : Form
    {
        //Connection String
        private string cs = ConfigurationManager.ConnectionStrings["CS"].ConnectionString;
        //string displayQuery = "display_total";

        //Setting the deleteClicked value to False to begin program
        bool deleteClicked = false;       

        public EbayCards()
        {
            InitializeComponent();
        }

        //Save button the user will press after entering card infrmation
        private void btnSave_Click(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection(cs);

            string saveQuery = "INSERT INTO cards(card_name, collection_num, listed_value, sold_value, style_listed, shipping) VALUES (@card_name, @collection_num, @listed_value, @sold_value, @style_listed, @shipping)";
            con.Open();
            SqlCommand cmd = new SqlCommand(saveQuery, con);

            cmd.Parameters.AddWithValue("@card_name", txtCardName.Text);
            cmd.Parameters.AddWithValue("@collection_num", txtCollectionNum.Text);
            cmd.Parameters.AddWithValue("@listed_value", Convert.ToDecimal(txtListValue.Text));
            cmd.Parameters.AddWithValue("@sold_value", string.IsNullOrEmpty(txtSoldValue.Text) ? (object)DBNull.Value : Convert.ToDecimal(txtSoldValue.Text));
            cmd.Parameters.AddWithValue("@style_listed", txtListStyle.Text);
            cmd.Parameters.AddWithValue("@shipping", Convert.ToDecimal(txtShipping.Text));

            cmd.ExecuteNonQuery();
            con.Close();

            ClearTextBoxes();
        }

        //If a user needs to make changs to a entry they will click this button after they made changes
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            string updateQuery = "UPDATE cards SET card_name = @card_name, collection_num = @collection_num, listed_value = @listed_value, sold_value = @sold_value, style_listed = @style_listed, shipping = @shipping WHERE card_num = @card_num";
            //string updateQuery = "update_cards";

            SqlConnection con = new SqlConnection(cs);
            con.Open();
            SqlCommand cmd = new SqlCommand(updateQuery, con);

            //cmd.Parameters.AddWithValue("?card_num", SqlDbType.Int).Value = txtCardNum.Text;
            //cmd.Parameters.AddWithValue("?card_name", txtCardName.Text);
            //cmd.Parameters.AddWithValue("?collection_num", txtCollectionNum.Text);
            //cmd.Parameters.AddWithValue("?listed_value", Convert.ToDecimal(txtListValue.Text));
            //cmd.Parameters.AddWithValue("?sold_value", string.IsNullOrEmpty(txtSoldValue.Text) ? (object)DBNull.Value : Convert.ToDecimal(txtSoldValue.Text));
            //cmd.Parameters.AddWithValue("?style_listed", txtListStyle.Text);
            //cmd.Parameters.AddWithValue("?shipping", Convert.ToDecimal(txtShipping.Text));

            //cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@card_num", SqlDbType.Int).Value = txtCardNum.Text;
            cmd.Parameters.AddWithValue("@card_name", txtCardName.Text);
            cmd.Parameters.AddWithValue("@collection_num", txtCollectionNum.Text);
            cmd.Parameters.AddWithValue("@listed_value", Convert.ToDecimal(txtListValue.Text));
            cmd.Parameters.AddWithValue("@sold_value", string.IsNullOrEmpty(txtSoldValue.Text) ? (object)DBNull.Value : Convert.ToDecimal(txtSoldValue.Text));
            cmd.Parameters.AddWithValue("@style_listed", txtListStyle.Text);
            cmd.Parameters.AddWithValue("@shipping", Convert.ToDecimal(txtShipping.Text));

            cmd.ExecuteNonQuery();
            con.Close();

            ClearTextBoxes();
        }

        //This button deltes entry.  A Message Box will appear in order to confirm
        private void btnDelete_Click(object sender, EventArgs e)
        {
            deleteClicked = true;

            //Message Box that makes sure you want to delete
            string messageTitle = "Delete Row";
            string message = "Are you sure you want to Delete this row?";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result = MessageBox.Show(message, messageTitle, buttons);

            string deleteQuery = "DELETE FROM cards WHERE card_num = @card_num";

            //Deletes the specified row after clicking on Message Box
            if (deleteClicked)
            {
                if (result == DialogResult.Yes)
                {
                    //string deleteQuery = "delete_query";

                    SqlConnection con = new SqlConnection(cs);
                    con.Open();
                    SqlCommand cmd = new SqlCommand(deleteQuery, con);
                    //cmd.CommandType = CommandType.StoredProcedure;
                    int convert = Convert.ToInt16(txtCardNum.Text);
                    cmd.Parameters.AddWithValue("@card_num", convert);

                    cmd.ExecuteNonQuery();
                    con.Close();
                }
                else if (result == DialogResult.No)
                {
                    //Close message box without deleting any entries
                }
            }
            ClearTextBoxes();
        }

        //This button will display the values of the databse in a DataGridView
        private void btnShow_Click(object sender, EventArgs e)
        {
            string displayQuery1 = "SELECT card_num AS 'Item Number', card_name AS 'Item Name', collection_num AS 'Collection Num', CONCAT('$', listed_value) AS 'Listed Value', CONCAT('$', sold_value) AS 'Sold Value', style_listed AS 'Style Listed', CONCAT('$', shipping) AS 'Shipping Cost' FROM cards";
            //string displayQuery = "display_query";
            DisplayTable(displayQuery1);
        }

        //Button that clears text box if user needs to before completing another task
        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearTextBoxes();
        }

        //Method us to clear all the textes boxes and set the focus to the top most text box
        private void ClearTextBoxes()
        {
            txtCardName.Text = "";
            txtCollectionNum.Text = "";
            txtListValue.Text = "";
            txtSoldValue.Text = "";
            txtListStyle.Text = "";
            txtCardName.Text = "";
            txtCardNum.Text = "";
            txtShipping.Text = "";

            //Brings the active text box to top most box
            txtCardName.Focus();
        }

        //Button that will display total of all items sold
        private void btnDisplayTotal_Click(object sender, EventArgs e)
        {
            //string displayQuery = "display_total";
            string displayQuery = "SELECT CONCAT('$',SUM(sold_value)) FROM cards";

            SqlConnection con = new SqlConnection(cs);
            SqlCommand MyCommand2 = new SqlCommand(displayQuery, con);
            con.Open();

            SqlDataAdapter MyAdapter = new SqlDataAdapter();
            MyAdapter.SelectCommand = MyCommand2;

            txtTotalSold.Text = MyCommand2.ExecuteScalar().ToString();


            con.Close();
        }


        //Method to display table when called
        private void DisplayTable(string displayQuery)
        {


            //string displayQuery1 = "SELECT card_num AS 'Item Number', card_name AS 'Item Name', collection_num AS 'Collection Num', CONCAT('$', listed_value) AS 'Listed Value', CONCAT('$', sold_value) AS 'Sold Value', style_listed AS 'Style Listed', CONCAT('$', shipping) AS 'Shipping Cost' FROM cards";

            SqlConnection con = new SqlConnection(cs);
            SqlCommand command = new SqlCommand(displayQuery, con);
            con.Open();

            SqlDataAdapter MyAdapter = new SqlDataAdapter();
            MyAdapter.SelectCommand = command;
            DataTable dTable = new DataTable();
            MyAdapter.Fill(dTable);
            dataGridView1.DataSource = dTable;

            //MyAdapter.SelectCommand.Parameters.AddWithValue("@collection_num", txtCollectionNum.Text);
            //command.Parameters.AddWithValue("@card_name", txtCardName.Text);

            con.Close();



        }


        //The user will select from several search choices to search the database for particular items, or items that have been sold
        // or that are still avaliable.  The user will then click display button to search database 
        private void btnDisplayComboBox_Click(object sender, EventArgs e)
        {
            string collectionNumQuery = "SELECT card_num AS 'Item Number', card_name AS 'Item Name', collection_num AS 'Collection Num', CONCAT('$', listed_value) AS 'Listed Value', CONCAT('$', sold_value) AS 'Sold Value', style_listed AS 'Style Listed', CONCAT('$', shipping) AS 'Shipping Cost' FROM cards WHERE collection_num = @collection_num";
            string cardNameQuery = "SELECT card_num AS 'Item Number', card_name AS 'Item Name', collection_num AS 'Collection Num', CONCAT('$', listed_value) AS 'Listed Value', CONCAT('$', sold_value) AS 'Sold Value', style_listed AS 'Style Listed', CONCAT('$', shipping) AS 'Shipping Cost' FROM cards WHERE card_name LIKE CONCAT('%', @card_name, '%')";       
            //string soldCardsQuery = "sold_cards";
            //string unsoldCardsQuery = "unsold_cards";

            string soldCardsQuery = "SELECT card_num AS 'Item Number', card_name AS 'Item Name', collection_num AS 'Collection Num', CONCAT('$', listed_value) AS 'Listed Value', CONCAT('$', sold_value) AS 'Sold Value', style_listed AS 'Style Listed', CONCAT('$', shipping) AS 'Shipping Cost' FROM cards WHERE sold_value IS NOT NULL";
            string unsoldCardsQuery = "SELECT card_num AS 'Item Number', card_name AS 'Item Name', collection_num AS 'Collection Num', CONCAT('$', listed_value) AS 'Listed Value', CONCAT('$', sold_value) AS 'Sold Value', style_listed AS 'Style Listed', CONCAT('$', shipping) AS 'Shipping Cost' FROM cards WHERE sold_value IS NULL";




            //txtComboBoxValue.Text changed
            if (cmbBox.Text == "Collection Number")
            {
                //ComboBox("@collection_num", collectionNumQuery);
                SqlConnection con = new SqlConnection(cs);
                SqlCommand command = new SqlCommand(collectionNumQuery, con);
                con.Open();

                SqlDataAdapter MyAdapter = new SqlDataAdapter();
                command.Parameters.AddWithValue("@collection_num", txtComboBoxValue.Text);
                MyAdapter.SelectCommand = command;
                DataTable dTable = new DataTable();
                MyAdapter.Fill(dTable);
                dataGridView1.DataSource = dTable;


                //SqlCommand cmd = new SqlCommand(collectionNumQuery, con);
                //cmd.Parameters.AddWithValue("@collection_num", txtCollectionNum.Text);

                //DisplayTable(collectionNumQuery);
                con.Close();


            }
            else if (cmbBox.Text == "Item Name")
            {
                //ComboBox("@card_name", cardNameQuery);

                SqlConnection con = new SqlConnection(cs);
                SqlCommand command = new SqlCommand(cardNameQuery, con);
                con.Open();

                SqlDataAdapter MyAdapter = new SqlDataAdapter();
                command.Parameters.AddWithValue("@card_name", txtComboBoxValue.Text);
                MyAdapter.SelectCommand = command;
                DataTable dTable = new DataTable();
                MyAdapter.Fill(dTable);
                dataGridView1.DataSource = dTable;

                con.Close();
            }
            else if (cmbBox.Text == "Sold Items")
            {
                DisplayTable(soldCardsQuery);
            }
            else if (cmbBox.Text == "Unsold Items")
            {
                DisplayTable(unsoldCardsQuery);
            }
        }


        //Method called to display results from Combo Box for Item Name and Collection Number

        private void ComboBox(string param, string query)
        {
            SqlConnection con = new SqlConnection(cs);
            con.Open();
            SqlCommand MyCommand2 = new SqlCommand(query, con);

            //MyCommand2.Parameters.AddWithValue(param, SqlDbType.Int).Value = txtCardNum.Text;
            //MyCommand2.Parameters.AddWithValue(param, txtComboBoxValue.Text);


            MyCommand2.Parameters.AddWithValue(param, txtCollectionNum.Text);
            MyCommand2.Parameters.AddWithValue(param, txtCardName.Text);


            DisplayTable(query);

        }

        //Button to display the total number of active items
        private void btnTotalActiveCards_Click(object sender, EventArgs e)
        {
            string displayTotalActiveCards = "SELECT COUNT(*) from cards WHERE sold_value IS NULL";
            //string displayTotalActiveCards = "active_cards";
            SqlConnection con = new SqlConnection(cs);
            SqlCommand MyCommand2 = new SqlCommand(displayTotalActiveCards, con);
            con.Open();

            SqlDataAdapter MyAdapter = new SqlDataAdapter();
            MyAdapter.SelectCommand = MyCommand2;

            txtTotalActiveCards.Text = MyCommand2.ExecuteScalar().ToString();
            con.Close();
        }

        //Can enter an items Item Number to fill in the Text Boxes when updating information 
        private void btnSearch_Click(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection(cs);
            string displaySelectedName = "SELECT card_name, collection_num, listed_value, sold_value, style_listed, shipping FROM cards WHERE card_num = '" + txtCardNum.Text + "'";

            SqlDataAdapter da = new SqlDataAdapter(displaySelectedName, con);
            DataTable dt = new DataTable();
            da.Fill(dt);

            txtCardName.Text = dt.Rows[0][0] + "";
            txtCollectionNum.Text = dt.Rows[0][1] + "";
            txtListValue.Text = dt.Rows[0][2] + "";
            txtSoldValue.Text = dt.Rows[0][3] + "";
            txtListStyle.Text = dt.Rows[0][4] + "";
            txtShipping.Text = dt.Rows[0][5] + "";
        }
    }
}