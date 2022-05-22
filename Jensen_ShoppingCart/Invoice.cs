using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jensen_ShoppingCart
{
    public class Invoice
    {   
        //constructors
        public Invoice() { }
        public Invoice(int InvoiceID, DateTime InvoiceDate, bool Shipped, string CustomerName, string CustomerAddress, string CustomerEmail)
        {
            this.InvoiceID = InvoiceID;
            this.InvoiceDate = InvoiceDate;
            this.Shipped = Shipped;
            this.CustomerName = CustomerName;
            this.CustomerAddress = CustomerAddress;
            this.CustomerEmail = CustomerEmail;
        }

        //using auto-implemented properties
        //will validate data at the button click event level, before assigning acceptable data to the class property
        public int InvoiceID { get; set; }
        public DateTime InvoiceDate { get; set; }
        public bool Shipped { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerEmail { get; set; }

        //property to return Yes/No string value to show in listbox
        public string IsShipped
        {
            get
            {
                if (Shipped == true)
                {
                    return "Yes";
                }
                else
                {
                    return "No";
                }
            }
        }

        //Override ToString() so listbox can read our object as text
        public override string ToString() => $"{InvoiceID,-15} {CustomerName,-30} {CustomerEmail,-40} {IsShipped,-15}";

        //Equals and Hashcode overrides used when adding records
        public override bool Equals(object obj)
        {
            //nulls are not equal
            if (obj == null) return false;

            //can compare primary key only rather than comparing every property value
            //two objects are equal if they have the same Customer Number
            if (this.InvoiceID == ((Invoice)obj).InvoiceID)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        public override int GetHashCode()
        {
            return this.InvoiceID;

        }
    }
}