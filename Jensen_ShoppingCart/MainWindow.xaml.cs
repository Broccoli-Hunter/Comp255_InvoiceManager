using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Jensen_ShoppingCart
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //set up Lists and current invoice/item pointers:
        //Invoice set
        private List<Invoice> InvoiceList = new List<Invoice>();
        private Invoice CurrentInvoice;
        private int CurrentInvoiceSelectedIndex;
        //InvoiceItem set
        private List<InvoiceItem> ItemsList = new List<InvoiceItem>();
        private InvoiceItem CurrentItem;
        private int CurrentItemSelectedIndex;

        public MainWindow()
        {
            InitializeComponent();

            //clear error labels when starting program
            InvoiceErrorLabel.Content = "";
            ItemErrorLabel.Content = "";

            //SET LISTBOX HEADER LABELS
            //I couldn't figure out how to have a non-selectable header... I thought of making an item in the listbox as a header
            //   but then it would be selectable. I found online something about item template/data templates, but it looked 
            //   very confusing and I felt it was beyond what you must be expecting from us. 
            //Using a label above the listbox is the solution I came up with, because I didn't want the user to be
            //   able to try deleting my header item from the listbox.
            InvoicesHeaderLabel.Content = $"{"Invoice ID",-15} {"Customer Name",-30} {"Email",-40} {"Shipped?",-15}";
            ItemsHeaderLabel.Content = $"{"Item ID",-10} {"Name",-26}{"Description",-30}{"Item Price",16}{"Quantity", 16} {"Price",15}";
      
            //initial load of invoices into invoice listbox
            LoadInvoices();

        }

        void DisplayInvoice()
        {   //purpose: display invoice selected object properties in labelled textboxes in user window

            if (InvoiceList.Count > 0)  //display contents of current selected invoice when there is something in list to display
            {
                if (CurrentInvoice != null) //if CurrentInvoice is NOT null, the record will display in the textboxes
                {
                    InvoiceIDTextBox.Text = CurrentInvoice.InvoiceID.ToString();
                    InvoiceDateTextBox.Text = CurrentInvoice.InvoiceDate.ToShortDateString();
                    IsShippedCheckBox.IsChecked = CurrentInvoice.Shipped;
                    CustomerNameTextBox.Text = CurrentInvoice.CustomerName;
                    CustomerAddressTextBox.Text = CurrentInvoice.CustomerAddress;
                    CustomerEmailTextBox.Text = CurrentInvoice.CustomerEmail;
                }
                else
                {
                    ClearInvoiceRecord(); //clear invoice text boxes
                }


            }
            else //if no invoices to select clear boxes (in the case you've deleted the last invoice)
            {
                ClearInvoiceRecord();
            }
        }

        void DisplayItem()
        {   //purpose: display item selected object properties in labelled textboxes in user window

            if (ItemsList.Count > 0)  //display contents of current selected item when there is something in list to display
            {
                if (CurrentItem != null)  //if CurrentItem is NOT null, the record will display in the textboxes
                {
                    ItemIDTextBox.Text = CurrentItem.ItemID.ToString();
                    ItemNameTextBox.Text = CurrentItem.ItemName;
                    ItemDescriptionTextBox.Text = CurrentItem.ItemDescription;
                    ItemPriceTextBox.Text = $"{CurrentItem.ItemPrice:N2}";
                    ItemQuantityTextBox.Text = CurrentItem.ItemQuantity.ToString();
                }
                else
                {
                    ClearItemRecord(); //clear item text boxes
                }
                
            }
            else //if no items to select clear boxes (in the case you've deleted the last item)
            {
                ClearItemRecord();
            }
        }

        void ClearInvoiceRecord()
        { //purpose: clear invoice record textboxes

            //clear invoice record data
            InvoiceIDTextBox.Clear();
            InvoiceDateTextBox.Clear();
            IsShippedCheckBox.IsChecked = false;
            CustomerNameTextBox.Clear();
            CustomerAddressTextBox.Clear();
            CustomerEmailTextBox.Clear();
        }

        void ClearItemRecord()
        {   //purpose: clear item record textboxes

            //clear item record data
            ItemIDTextBox.Clear();
            ItemNameTextBox.Clear();
            ItemDescriptionTextBox.Clear();
            ItemPriceTextBox.Clear();
            ItemQuantityTextBox.Clear();
        }

        bool IsDataValid(bool forInvoiceData, bool forItemData)
        {   //purpose: based on parameters, will check for either Invoice validation points or Item validation points

            //clear out previous error messages
            InvoiceErrorLabel.Content = "";
            ItemErrorLabel.Content = "";

            //variables to hold TryParse outputs (to check for acceptable numbers)
            DateTime Temp;
            decimal ItemPriceCheck = 0;
            int ItemQuantityCheck = 0;

            //check to see which part of form currently requires validation (based on paramater inputs):
            //Invoice Data:
            if (forInvoiceData == true)
            {
                if (CustomerNameTextBox.Text == "") //Is customer name field empty?
                {
                    InvoiceErrorLabel.Content = "You must enter a Customer Name.";
                    Keyboard.Focus(CustomerNameTextBox);
                    return false;
                }
                else if (CustomerEmailTextBox.Text == "") //Is customer email field empty?
                {
                    Keyboard.Focus(CustomerEmailTextBox);
                    InvoiceErrorLabel.Content = "You must enter a Customer Email Address.";
                    return false;
                }
                else if (DateTime.TryParse(InvoiceDateTextBox.Text, out Temp)  == false) //Is invoice date empty?
                {
                    Keyboard.Focus(InvoiceDateTextBox);
                    InvoiceErrorLabel.Content = "You must enter an Invoice Date (Month/Day/Year).";
                    return false;
                }
               
            }
            //Item Data:
            else if (forItemData == true)
            {
                if (ItemNameTextBox.Text == "") //Is item name field empty?
                {
                    Keyboard.Focus(ItemNameTextBox);
                    ItemErrorLabel.Content = "You must enter an Item Name.";
                    return false;
                }
                else if (decimal.TryParse(ItemPriceTextBox.Text, out ItemPriceCheck) == false) //is item price field empty or non-numerical?
                {
                    Keyboard.Focus(ItemPriceTextBox);
                    ItemErrorLabel.Content = "You must enter numerical value for Item Price.";
                    return false;
                }
                else if (ItemPriceCheck <= 0) //Is item price less than or equal to zero?
                {
                    Keyboard.Focus(ItemPriceTextBox);
                    ItemErrorLabel.Content = "You must enter a positive (non-zero) Item Price.";
                    return false;
                }
                else if (int.TryParse(ItemQuantityTextBox.Text, out ItemQuantityCheck) == false) //is item quantity field empty or non-numerical?
                {
                    Keyboard.Focus(ItemQuantityTextBox);
                    ItemErrorLabel.Content = "You must enter an Item Quantity.";
                    return false;
                }
                else if (ItemQuantityCheck < 1) //is item quantity less than 1?
                {
                    Keyboard.Focus(ItemQuantityTextBox);
                    ItemErrorLabel.Content = "You must enter a positive value for Item Quantity.";
                    return false;
                }

            }
            //by this point, all data meets requirements
            return true;
            
        }

        void LoadInvoices()
        {   //purpose: load InvoiceList<> and listbox with invoices from database Invoice table

            //make variables to preserve the index and object before reload
            int PreservedSelectedIndex;
            Invoice PreservedInvoice;
            // preserve current invoice record
            PreservedInvoice = (Invoice)InvoiceListBox.SelectedItem;
            PreservedSelectedIndex = InvoiceListBox.SelectedIndex;

            //clear out the list and listbox for reload            
            InvoiceList.Clear();
            InvoiceListBox.Items.Clear();

            //make a connection object:
            using (SqlConnection connection = new SqlConnection())
            {

                //assign connection String:
                connection.ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\ShoppingCart.mdf;Integrated Security=True";

                //open the connection:
                connection.Open();

                //create a SQL command, then place in a command object:
                string sql = "SELECT * FROM Invoices;";
                SqlCommand selectCommand = new SqlCommand(sql, connection);

                //execute the command, and obtain a data reader with the result:
                using (SqlDataReader reader = selectCommand.ExecuteReader())
                {
                    // loop over the reader results row by row
                    Invoice NewInvoice;

                    while (reader.Read())
                    {
                        //load the record into the NewAccount object, casting to appropriate datatype
                        NewInvoice = new Invoice((int)reader["InvoiceID"],
                                                    (DateTime)reader["InvoiceDate"],
                                                    (bool)reader["Shipped"],
                                                    (string)reader["CustomerName"],
                                                    (string)reader["CustomerAddress"],
                                                    (string)reader["CustomerEmail"]);

                        //add to List<> and listbox
                        InvoiceList.Add(NewInvoice);
                        InvoiceListBox.Items.Add(NewInvoice);
                    }
                    reader.Close();
                }
                connection.Close();

                //reassign selected item and current invoice index after loading
                InvoiceListBox.SelectedItem = PreservedInvoice;
                CurrentInvoiceSelectedIndex = PreservedSelectedIndex;
            }
        }

        void LoadInvoiceItems()
        {   //purpose: load ItemsList<> and listbox with items from database InvoiceItems table

            //make variables to preserve the index and object before reload
            int PreservedSelectedIndex;
            InvoiceItem PreservedItem;
            // preserve current invoice record
            PreservedItem = (InvoiceItem)InvoiceItemsListBox.SelectedItem;
            PreservedSelectedIndex = InvoiceItemsListBox.SelectedIndex;

            //clear out the list and listbox for reload            
            ItemsList.Clear();
            InvoiceItemsListBox.Items.Clear();

            //make a connection object:
            using (SqlConnection connection = new SqlConnection())
            {

                //assign connection String:
                connection.ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\ShoppingCart.mdf;Integrated Security=True";

                //open the connection:
                connection.Open();

                //create a SQL command, then place in a command object:
                string sql = $"SELECT * FROM InvoiceItems WHERE InvoiceID = {InvoiceList[CurrentInvoiceSelectedIndex].InvoiceID} ;";
                SqlCommand selectCommand = new SqlCommand(sql, connection);

                //execute the command, and obtain a data reader with the result:
                using (SqlDataReader reader = selectCommand.ExecuteReader())
                {
                    // loop over the reader results row by row
                    InvoiceItem NewItem;

                    while (reader.Read())
                    {
                        //load the record into the NewAccount object, casting to appropriate datatype
                        NewItem = new InvoiceItem((int)reader["ItemID"],
                                                    (int)reader["InvoiceID"],
                                                    (string)reader["ItemName"],
                                                    (string)reader["ItemDescription"],
                                                    (decimal)reader["ItemPrice"],
                                                    (int)reader["ItemQuantity"]);

                        //add to List<> and listbox
                        ItemsList.Add(NewItem);
                        InvoiceItemsListBox.Items.Add(NewItem);
                    }
                    reader.Close();
                }
                connection.Close();

                //reassign selected item and current item index after loading
                InvoiceItemsListBox.SelectedItem = PreservedItem;
                CurrentItemSelectedIndex = PreservedSelectedIndex;
            }
            CalculateCost(); //after reload of database, re-calculate information for Order Cost box
        }

        private void UpdateInvoice()
        {   //purpose: update(save) invoice information into database from previously existing record

            //update the current record in the database
            using (SqlConnection connection = new SqlConnection())
            {
                //make connection string
                connection.ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\ShoppingCart.mdf;Integrated Security=True";

                //open the connection:
                connection.Open();

                //create an UPDATE SQL query string
                string sql = $"UPDATE Invoices SET " +
                             $"InvoiceDate = '{InvoiceDateTextBox.Text}', " +
                             $"Shipped = '{IsShippedCheckBox.IsChecked.Value}', " +
                             $"CustomerName = '{CustomerNameTextBox.Text}', " +
                             $"CustomerEmail = '{CustomerEmailTextBox.Text}', " +
                             $"CustomerAddress = '{CustomerAddressTextBox.Text}' " +
                             $"WHERE InvoiceID = {InvoiceList[CurrentInvoiceSelectedIndex].InvoiceID} ;";

                //create a SQL command to hold the query string
                using (SqlCommand UpdateCommand = new SqlCommand(sql, connection))
                {
                    //execute the command
                    UpdateCommand.ExecuteNonQuery();

                }
                connection.Close();

                //reload from database
                LoadInvoices();  
                
                //reselect from database
                CurrentInvoice = InvoiceList[CurrentInvoiceSelectedIndex];
                InvoiceListBox.SelectedItem = CurrentInvoice;

            }
        }

        private void UpdateItem()
        {   //purpose: update(save) item information into database from previously existing record

            //update the current record in the database
            using (SqlConnection connection = new SqlConnection())
            {
                //make connection string
                connection.ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\ShoppingCart.mdf;Integrated Security=True";

                //open the connection:
                connection.Open();

                //creat an UPDATE SQL query string
                string sql = $"UPDATE InvoiceItems SET " +
                             $"ItemPrice = '{ItemPriceTextBox.Text}', " +
                             $"ItemDescription = '{ItemDescriptionTextBox.Text}', " +
                             $"ItemName = '{ItemNameTextBox.Text}', " +
                             $"ItemQuantity = '{ItemQuantityTextBox.Text}' " +
                             $"WHERE ItemID = {ItemsList[CurrentItemSelectedIndex].ItemID} ;";

                //create a SQL command to hold the query string
                using (SqlCommand UpdateCommand = new SqlCommand(sql, connection))
                {
                    //execute the command
                    UpdateCommand.ExecuteNonQuery();
                }
                connection.Close();

                //reload from database
                LoadInvoiceItems();

                //reselect current item
                CurrentItem = ItemsList[CurrentItemSelectedIndex];
                InvoiceItemsListBox.SelectedItem = CurrentItem;
            }
        }

        private void AddInvoice()
        {   //purpose: add invoice record to database (new record)

            string sql;
            Invoice NewInvoice = new Invoice();

            // get data from textboxes (except Invoice ID)
            NewInvoice.InvoiceDate = Convert.ToDateTime(InvoiceDateTextBox.Text);
            NewInvoice.CustomerName = CustomerNameTextBox.Text;
            NewInvoice.CustomerAddress = CustomerAddressTextBox.Text;
            NewInvoice.CustomerEmail = CustomerEmailTextBox.Text;
            NewInvoice.Shipped = IsShippedCheckBox.IsChecked.Value;

            //open a connection to database
            using (SqlConnection connection = new SqlConnection())
            {
                //make connection string and open connection to database
                connection.ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\ShoppingCart.mdf;Integrated Security=True";
                connection.Open();

                //create query to get largest (last in ascending order) Invoice ID
                sql = "SELECT MAX(InvoiceID) FROM Invoices;";

                int NewInvoiceID; //store incremented Invoice ID

                //make command object to execute query (returning single value)
                using (SqlCommand SelectDataCommand = new SqlCommand(sql, connection))
                {
                    var CurrentMaxInvoiceID = SelectDataCommand.ExecuteScalar(); //variable to store either valid result or null value
                    // https://docs.microsoft.com/en-us/dotnet/api/system.dbnull?view=net-5.0  MSDN for 'DBNull Class'
                    if (DBNull.Value.Equals(CurrentMaxInvoiceID)) //if database is empty (DBNull returned)
                    {
                        NewInvoiceID = 1000; //hardcode initial invoice ID to 1000
                    }
                    else //valid max number found
                    {
                        NewInvoiceID = Convert.ToInt32(CurrentMaxInvoiceID) + 1; //increment current max number by one
                    }

                }

                NewInvoice.InvoiceID = NewInvoiceID; //update new invoice ID in object

                //using same connection, make new query to insert record into database
                sql = "INSERT INTO Invoices " +
                      "(InvoiceID, InvoiceDate, Shipped, CustomerName, CustomerAddress, CustomerEmail) " +
                      "VALUES " +
                      "(" +
                      $"{NewInvoice.InvoiceID}, " +
                      $"'{NewInvoice.InvoiceDate}', " +
                      $"'{NewInvoice.Shipped}', " +
                      $"'{NewInvoice.CustomerName}', " +
                      $"'{NewInvoice.CustomerAddress}', " +
                      $"'{NewInvoice.CustomerEmail}');";

                //make command object to execute the query (no value to return)
                using (SqlCommand InsertRecordCommand = new SqlCommand(sql, connection))
                {
                    InsertRecordCommand.ExecuteNonQuery();
                }
                connection.Close();
            }

            //reload list from database
            LoadInvoices();
            
            //find new record in listbox
            int NewRecordIndex = InvoiceListBox.Items.IndexOf(NewInvoice);

            //select new item (and scroll to it if necessary)
            InvoiceListBox.SelectedIndex = NewRecordIndex;
            InvoiceListBox.ScrollIntoView(NewInvoice);
            InvoiceListBox.SelectedItem = NewInvoice;
            
        }

        private void AddItem()
        {   //purpose: add item record to database (new record)

            string sql;
            InvoiceItem NewItem = new InvoiceItem();

            // get data from textboxes (except Item ID)
            NewItem.ItemName = ItemNameTextBox.Text;
            NewItem.ItemDescription = ItemDescriptionTextBox.Text;
            NewItem.ItemPrice = Convert.ToDecimal(ItemPriceTextBox.Text);
            NewItem.ItemQuantity = Convert.ToInt32(ItemQuantityTextBox.Text);
            NewItem.InvoiceID = CurrentInvoice.InvoiceID; //not from textbox, but from current Invoice ID

            //open a connection to database
            using (SqlConnection connection = new SqlConnection())
            {
                //make connection string and open connection to database
                connection.ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\ShoppingCart.mdf;Integrated Security=True";
                connection.Open();

                //create query to get largest (last in ascending order) Item ID
                sql = "SELECT MAX(ItemID) FROM InvoiceItems;";

                int NewItemID; //store incremented Item ID

                //make command object to execute query (returning single value)
                using (SqlCommand SelectDataCommand = new SqlCommand(sql, connection))
                {
                    var CurrentMaxItemID = SelectDataCommand.ExecuteScalar(); //variable to store either valid result or null value
                    // https://docs.microsoft.com/en-us/dotnet/api/system.dbnull?view=net-5.0  MSDN for 'DBNull Class'
                    if (DBNull.Value.Equals(CurrentMaxItemID)) //if database is empty (DBNull returned)
                    {
                        NewItemID = 100; //hardcode initial item ID to 100
                    }
                    else //valid max number found
                    {
                        NewItemID = Convert.ToInt32(CurrentMaxItemID) + 1; //increment current max number by one
                    }

                }

                NewItem.ItemID = NewItemID; //update new item ID in object

                //using same connection, make new query to insert record into database
                sql = "INSERT INTO InvoiceItems " +
                      "(ItemID, InvoiceID, ItemName, ItemDescription, ItemPrice, ItemQuantity) " +
                      "VALUES " +
                      "(" +
                      $"{NewItem.ItemID}, " +
                      $"{NewItem.InvoiceID}, " +
                      $"'{NewItem.ItemName}', " +
                      $"'{NewItem.ItemDescription}', " +
                      $"{NewItem.ItemPrice}, " +
                      $"{NewItem.ItemQuantity});";

                //make command object to execute the query (no value to return)
                using (SqlCommand InsertRecordCommand = new SqlCommand(sql, connection))
                {
                    InsertRecordCommand.ExecuteNonQuery();
                }

                connection.Close();

                //reload items from database
                LoadInvoiceItems();

                //find new record in listbox
                int NewRecordIndex = InvoiceItemsListBox.Items.IndexOf(NewItem);

                //select new item (and scroll to it if necessary)
                InvoiceItemsListBox.SelectedIndex = NewRecordIndex;
                InvoiceItemsListBox.ScrollIntoView(NewItem);
                InvoiceItemsListBox.SelectedItem = NewItem;
            }
        }

        private void DeleteInvoice()
        {   //purpose: remove invoice record from database table (and associated items records from invoice items table)

            string sql;
            using (SqlConnection connection = new SqlConnection())
            {
                //make connection string and open connection
                connection.ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\ShoppingCart.mdf;Integrated Security=True";
                connection.Open();

                // create string to contain SQL delete query (need to delete invoice and all items associated with it)
                // logic: delete invoice item records first before invoiceID is deleted, then delete the invoice record
                sql = $"DELETE FROM InvoiceItems WHERE InvoiceID = {InvoiceList[CurrentInvoiceSelectedIndex].InvoiceID}; " +
                      $"DELETE FROM Invoices WHERE InvoiceID = {InvoiceList[CurrentInvoiceSelectedIndex].InvoiceID}; ";  

                //Create a command object and execute query (no return value expected)
                using (SqlCommand DeleteRecordCommand = new SqlCommand(sql, connection))
                {
                    try
                    {
                        DeleteRecordCommand.ExecuteNonQuery();
                    }
                    catch (SqlException ex)
                    {
                        Exception error = new Exception("Error: No matching records to delete", ex);
                        throw error;
                    }

                }
                connection.Close();

                //update displayed record
                int IndexToRemove = CurrentInvoiceSelectedIndex;  //take note of current index/item to delete
                Invoice InvoiceToRemove = CurrentInvoice;

                //remove item from listbox and invoice list
                InvoiceList.Remove(InvoiceToRemove);
                InvoiceListBox.Items.Remove(InvoiceToRemove);

                //reselect invoice in list
                if (IndexToRemove == InvoiceList.Count)  //deleted the last record in list
                {
                    //select "new" last record
                    CurrentInvoiceSelectedIndex = InvoiceList.Count - 1;
                }
                else   //deleted invoice from middle of list
                {
                    CurrentInvoiceSelectedIndex = IndexToRemove;
                }

                //change listbox selection to intended invoice
                InvoiceListBox.SelectedIndex = CurrentInvoiceSelectedIndex;

            }

        }

        private void DeleteItem()
        {   //purpose: remove item record from database table

            string sql;
            using (SqlConnection connection = new SqlConnection())
            {
                //make connection string and open connection
                connection.ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\ShoppingCart.mdf;Integrated Security=True";
                connection.Open();

                // create string to contain SQL delete query
                sql = $"DELETE FROM InvoiceItems WHERE ItemID = {ItemsList[CurrentItemSelectedIndex].ItemID};";
                      
                //Create a command object and execute query (no return value expected)
                using (SqlCommand DeleteRecordCommand = new SqlCommand(sql, connection))
                {
                    try
                    {
                        DeleteRecordCommand.ExecuteNonQuery();
                    }
                    catch (SqlException ex)
                    {
                        Exception error = new Exception("Error: No matching records to delete", ex);
                        throw error;
                    }
                }
                connection.Close();

                //update displayed record
                int IndexToRemove = CurrentItemSelectedIndex;  //take note of current index/item to delete
                InvoiceItem ItemToRemove = CurrentItem;

                //remove item from listbox and invoice list
                ItemsList.Remove(ItemToRemove);
                InvoiceItemsListBox.Items.Remove(ItemToRemove);

                //reselect item in list
                if (IndexToRemove == ItemsList.Count)  //deleted the last record in list
                {
                    //select "new" last record
                    CurrentItemSelectedIndex = ItemsList.Count - 1;
                }
                else   //deleted item from middle of list
                {
                    CurrentItemSelectedIndex = IndexToRemove;
                }

                //change listbox selection to intended item
                InvoiceItemsListBox.SelectedIndex = CurrentItemSelectedIndex;
            }

        }

        private void CalculateCost()
        {   //purpose: to calculate the cost of all items on the invoice, including taxes, and display in Order Cost box

            decimal Subtotal = 0, PST, GST, Total;
             
            //loop through items in ItemList and make running total of price property (I added price property to the class)
            foreach(InvoiceItem i in ItemsList)
            {
                Subtotal += i.Price;
            }

            //calculate other fields
            PST = Subtotal * 0.06M;
            GST = Subtotal * 0.05M;
            Total = Subtotal + GST + PST;

            //display in textboxes
            SubtotalTextBox.Text = $"${Subtotal:0.00}";
            PSTTextBox.Text = $"${PST:0.00}";
            GSTTextBox.Text = $"${GST:0.00}";
            OrderTotalTextBox.Text = $"${Total:0.00}";

        }

        private void SaveInvoice()
        {   //purpose: to validate current data in invoice record, then update database (reloads database into listbox and list), and display record

            //if data was valid, carry on with save process
            if (IsDataValid(true, false)) 
            { 
                //update data in database table
                UpdateInvoice();

                //display current record
                DisplayInvoice();
            }
        }

        private void SaveItem()
        {   //purpose: to validate current data in item record, then update database (reloads database into listbox and list), and display record

            //if data is valid, carry on with save procedures
            if (IsDataValid(false, true)) 
            { 
                //update data in database table
                UpdateItem();

                //display current record
                DisplayItem();

                //recalculate cost box
                CalculateCost();
            }
        }

        private void InvoiceListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            // note the current invoice record 
            CurrentInvoice = (Invoice)InvoiceListBox.SelectedItem;
            CurrentInvoiceSelectedIndex = InvoiceListBox.SelectedIndex;

            //display the current record in the form and load associated invoice items
            DisplayInvoice();

            if (CurrentInvoice != null) //if current object is not null
            { 
                //load associated items into invoice items listbox
                LoadInvoiceItems();

                //clear invoice item record (blank until new item is selected)
                ClearItemRecord();
            
                //calculate cost of current invoice
                CalculateCost();
            }

        }

        private void InvoiceItemsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //note current item record
            CurrentItem = (InvoiceItem)InvoiceItemsListBox.SelectedItem;
            CurrentItemSelectedIndex = InvoiceItemsListBox.SelectedIndex;

            //display the current item information
            DisplayItem();


        }

        private void InvoiceSaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveInvoice();
        }

        private void InvoiceDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            //delete record and associated items
            DeleteInvoice();

            //display current invoice
            DisplayInvoice();

            //clear record field from deleted user, clear items list and listbox, recalculate order total box (zeroes expected)
            InvoiceItemsListBox.Items.Clear();
            ItemsList.Clear();
            ClearItemRecord();
            CalculateCost();
            
        }

        private void InvoiceNewButton_Click(object sender, RoutedEventArgs e)
        {
            //if data is valid, proceed with adding record
            if (IsDataValid(true, false)) 
            { 
                //add new record
                AddInvoice();

                //display current invoice
                DisplayInvoice();
            }
        }

        private void ItemSaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveItem();
        }

        private void ItemDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            //delete item record
            DeleteItem();

            //clear deleted record
            ClearItemRecord();

            //display current item
            DisplayItem();

            //recalculate cost
            CalculateCost();
        }

        private void ItemNewButton_Click(object sender, RoutedEventArgs e)
        {
            //if data is valid, continue with adding new record
            if (IsDataValid(false,true))
            { 
                //add new item with current textboxes
                AddItem();

                //display current item
                DisplayItem();
            }
        }
    }
}
