using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jensen_ShoppingCart
{
    public class InvoiceItem
    {
        //constructors
        public InvoiceItem() { }

        public InvoiceItem (int ItemID, int InvoiceID, string ItemName, string ItemDescription, decimal ItemPrice, int ItemQuantity)
        {
            this.ItemID = ItemID;
            this.InvoiceID = InvoiceID;
            this.ItemName = ItemName;
            this.ItemDescription = ItemDescription;
            this.ItemPrice = ItemPrice;
            this.ItemQuantity = ItemQuantity;
        }

        //using auto-implemented properties
        //will validate data at the button click event level, before assigning acceptable data to the class property
        public int ItemID { get; set; }
        public int InvoiceID { get; set; }
        public string ItemName { get; set; }
        public string ItemDescription { get; set; }
        public decimal ItemPrice { get; set; }
        public int ItemQuantity { get; set; }

        //property to calculate and store price to show in listbox
        public decimal Price
        {
            get
            {
                return ItemPrice * ItemQuantity;
            }
        }

        //Override ToString() so listbox can read our object as text
        public override string ToString() => $"{ItemID,-10} {ItemName,-25} {ItemDescription,-30} {ItemPrice,15:N2} {ItemQuantity, 15} {Price,15:N2}";

        //Equals and Hashcode overrides used when adding records
        public override bool Equals(object obj)
        {
            //nulls are not equal
            if (obj == null) return false;

            //can compare primary key only rather than comparing every property value
            //two objects are equal if they have the same Customer Number
            if (this.ItemID == ((InvoiceItem)obj).ItemID)
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
            return this.ItemID;

        }
    }
}