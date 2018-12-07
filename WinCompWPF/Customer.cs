using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinCompWPF
{
    class Customer
    {
        private string id;
        private string firstname;
        private string lastname;
        private DateTime customerSince;
        private bool newslettersubscriber;

        public Customer(string id, string firstname, string lastname, DateTime customerSince, bool newslettersubscriber)
        {
            this.firstname = firstname;
            this.lastname = lastname;
            this.id = id;
            this.customerSince = customerSince;
            this.newslettersubscriber = newslettersubscriber;
        }


        public string ID
        {
            get { return id; }
            set { id = value; }
        }
        public string FirstName
        {
            get { return firstname; }
            set { firstname = value; }
        }
        public string LastName
        {
            get { return lastname; }
            set { lastname = value; }
        }
        public DateTime CustomerSince
        {
            get { return customerSince; }
            set { customerSince = value; }
        }
        public bool NewsletterSubscriber
        {
            get { return newslettersubscriber; }
            set { newslettersubscriber = value; }
        }
    }
}
