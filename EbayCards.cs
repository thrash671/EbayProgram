//Title: Ebay Cards
//Date: 08/20/2021
//Developer: Josh Thrash
//Purpose:  This Window Form Application allows the user to input items that are actively being sold on Ebay.
//          The user can keep track of starting price and type of selling method in order to see which type sells best
//          Once the card is sold the user is able to update the MySql database in order to show the final price of items sold.
//          The user can also Update and Delete entires whenever needed.  The user is also able to search the active database
//          With particular parameters, view total value of sold items, and view total number of active items

using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace EbayCards
{
    public partial class EbayCards : Form
    {
        //Connection String
        private string cs = "server=mysql.thwackgolf.com; Database=ebay_cards; uid=jtt192; pwd=Thwack671!";

        //Setting the deleteClicked value to False to begin program
        bool deleteClicked = false;

        

        public EbayCards()
        {
            InitializeComponent();

        }

        //Save button the user will press after entering card infrmation
        private void btnSave_Click(object sender, EventArgs e)
        {

            MySqlConnection con = new MySqlConnection(cs);

            string saveQuery = "INSERT INTO cards(card_name, collection_num, listed_value, sold_value, style_listed, shipping) VALUES (?card_name, ?collection_num, ?listed_value, ?sold_value, ?style_listed, ?shipping)";
            con.Open();
            MySqlCommand cmd = new MySqlCommand(saveQuery, con);

            cmd.Parameters.AddWithValue("?card_name", txtCardName.Text);
            cmd.Parameters.AddWithValue("?collection_num", txtCollectionNum.Text);
            cmd.Parameters.AddWithValue("?listed_value", Convert.ToDecimal(txtListValue.Text));
            cmd.Parameters.AddWithValue("?sold_value", string.IsNullOrEmpty(txtSoldValue.Text) ? (object)DBNull.Value : Convert.ToDecimal(txtSoldValue.Text));
            cmd.Parameters.AddWithValue("?style_listed", txtListStyle.Text);
            cmd.Parameters.AddWithValue("?shipping", Convert.ToDecimal(txtShipping.Text));

            cmd.ExecuteNonQuery();
            con.Close();

            ClearTextBoxes();
        }

        //If a user needs to make changs to a entry they will click this button after they made changes
        private void btnUpdate_Click(object sender, EventArgs e)
        {

            string updateQuery = "UPDATE cards SET card_name = ?card_name, collection_num = ?collection_num, listed_value = ?listed_value, sold_value = ?sold_value, style_listed = ?style_listed, shipping = ?shipping WHERE card_num = ?card_num";
            MySqlConnection con = new MySqlConnection(cs);
            con.Open();
            MySqlCommand cmd = new MySqlCommand(updateQuery, con);

            cmd.Parameters.AddWithValue("?card_num", MySqlDbType.Int16).Value = txtCardNum.Text;
            cmd.Parameters.AddWithValue("?card_name", txtCardName.Text);
            cmd.Parameters.AddWithValue("?collection_num", txtCollectionNum.Text);
            cmd.Parameters.AddWithValue("?listed_value", Convert.ToDecimal(txtListValue.Text));
            cmd.Parameters.AddWithValue("?sold_value", string.IsNullOrEmpty(txtSoldValue.Text) ? (object)DBNull.Value : Convert.ToDecimal(txtSoldValue.Text));
            cmd.Parameters.AddWithValue("?style_listed", txtListStyle.Text);
            cmd.Parameters.AddWithValue("?shipping", Convert.ToDecimal(txtShipping.Text));


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
            MessageBoxButtons buttons = MessageBoxButtons.OK;
            DialogResult result = MessageBox.Show(message, messageTitle, buttons);

            //Deletes the specified row after clicking on Message Box
            if (deleteClicked)
            {
                if (result == DialogResult.OK)
                {
                    string deleteQuery = "DELETE FROM cards WHERE card_num = ?card_num";

                    MySqlConnection con = new MySqlConnection(cs);
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand(deleteQuery, con);

                    cmd.Parameters.AddWithValue("?card_num", MySqlDbType.Int16).Value = txtCardNum.Text;

                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }

            ClearTextBoxes();
        }

        //This button will display the values of the databse in a DataGridView
        private void btnShow_Click(object sender, EventArgs e)
        {
            string displayQuery = "SELECT card_num AS 'Item Number', card_name AS 'Item Name', collection_num AS 'Collection Num', CONCAT('$', listed_value) AS 'Listed Value', CONCAT('$', sold_value) AS 'Sold Value', style_listed AS 'Style Listed', CONCAT('$', shipping) AS 'Shipping Cost' FROM cards";
            DisplayTable(displayQuery);
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
            string displayQuery = "SELECT CONCAT('$',SUM(sold_value)) FROM cards";
            MySqlConnection con = new MySqlConnection(cs);
            MySqlCommand MyCommand2 = new MySqlCommand(displayQuery, con);
            con.Open();

            MySqlDataAdapter MyAdapter = new MySqlDataAdapter();
            MyAdapter.SelectCommand = MyCommand2;

            txtTotalSold.Text = MyCommand2.ExecuteScalar().ToString();

        }

        //The user will select from several search choices to search the database for particular items, or items that have been sold
        // or that are still avaliable.  The user will the n click display button to search databse 
        private void btnDisplayComboBox_Click(object sender, EventArgs e)
        {
            string collectionNumQuery = "SELECT card_num AS 'Item Number', card_name AS 'Item Name', collection_num AS 'Collection Num', CONCAT('$', listed_value) AS 'Listed Value', CONCAT('$', sold_value) AS 'Sold Value', style_listed AS 'Style Listed', CONCAT('$', shipping) AS 'Shipping Cost' FROM cards WHERE collection_num = ?collection_num";
            string cardNameQuery = "SELECT card_num AS 'Item Number', card_name AS 'Item Name', collection_num AS 'Collection Num', CONCAT('$', listed_value) AS 'Listed Value', CONCAT('$', sold_value) AS 'Sold Value', style_listed AS 'Style Listed', CONCAT('$', shipping) AS 'Shipping Cost' FROM cards WHERE card_name LIKE CONCAT('%', ?card_name, '%')";
            string soldCardsQuery = "SELECT card_num AS 'Item Number', card_name AS 'Item Name', collection_num AS 'Collection Num', CONCAT('$', listed_value) AS 'Listed Value', CONCAT('$', sold_value) AS 'Sold Value', style_listed AS 'Style Listed', CONCAT('$', shipping) AS 'Shipping Cost' FROM cards WHERE sold_value IS NOT NULL";
            string unsoldCardsQuery = "SELECT card_num AS 'Item Number', card_name AS 'Item Name', collection_num AS 'Collection Num', CONCAT('$', listed_value) AS 'Listed Value', CONCAT('$', sold_value) AS 'Sold Value', style_listed AS 'Style Listed', CONCAT('$', shipping) AS 'Shipping Cost' FROM cards WHERE sold_value IS NULL";

            if (cmbBox.Text == "Collection Number")
            {
                MySqlConnection con = new MySqlConnection(cs);
                MySqlCommand MyCommand2 = new MySqlCommand(collectionNumQuery, con);
                con.Open();
                MyCommand2.Parameters.AddWithValue("?collection_num", txtComboBoxValue.Text);

                MySqlDataAdapter MyAdapter = new MySqlDataAdapter();
                MyAdapter.SelectCommand = MyCommand2;

                DataTable dTable = new DataTable();
                MyAdapter.Fill(dTable);
                dataGridView1.DataSource = dTable;
                con.Close();
            }
            if (cmbBox.Text == "Item Name")
            {
                MySqlConnection con = new MySqlConnection(cs);
                MySqlCommand MyCommand2 = new MySqlCommand(cardNameQuery, con);
                con.Open();
                MyCommand2.Parameters.AddWithValue("?card_name", txtComboBoxValue.Text);

                MySqlDataAdapter MyAdapter = new MySqlDataAdapter();
                MyAdapter.SelectCommand = MyCommand2;

                DataTable dTable = new DataTable();
                MyAdapter.Fill(dTable);
                dataGridView1.DataSource = dTable;
                con.Close();
            }
            if (cmbBox.Text == "Sold Items")
            {
                DisplayTable(soldCardsQuery);
            }
            if (cmbBox.Text == "Unsold Items")
            {
                DisplayTable(unsoldCardsQuery);
            }
        }

        //Method to display table when called
        private void DisplayTable(string displayQuery)
        {
            MySqlConnection con = new MySqlConnection(cs);
            MySqlCommand MyCommand2 = new MySqlCommand(displayQuery, con);
            con.Open();

            MySqlDataAdapter MyAdapter = new MySqlDataAdapter();
            MyAdapter.SelectCommand = MyCommand2;
            DataTable dTable = new DataTable();
            MyAdapter.Fill(dTable);
            dataGridView1.DataSource = dTable;
            con.Close();
        }

        //Button to display the total number of active items
        private void btnTotalActiveCards_Click(object sender, EventArgs e)
        {
            string displayTotalActiveCards = "SELECT COUNT(*) from cards WHERE sold_value IS NULL";
            MySqlConnection con = new MySqlConnection(cs);
            MySqlCommand MyCommand2 = new MySqlCommand(displayTotalActiveCards, con);
            con.Open();

            MySqlDataAdapter MyAdapter = new MySqlDataAdapter();
            MyAdapter.SelectCommand = MyCommand2;

            txtTotalActiveCards.Text = MyCommand2.ExecuteScalar().ToString();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            MySqlConnection con = new MySqlConnection(cs);
            string displaySelectedName = "SELECT card_name, collection_num, listed_value, sold_value, style_listed, shipping FROM cards WHERE card_num = '" + txtCardNum.Text + "'";

            MySqlDataAdapter da = new MySqlDataAdapter(displaySelectedName, con);
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