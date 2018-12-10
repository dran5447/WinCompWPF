using System;

namespace WinCompWPF
{
    /*
     * Base customer object with a selection of data
     */ 
    class Customer
    {
        private string id;
        private string firstname;
        private string lastname;
        private DateTime customerSince;
        private bool newslettersubscriber;
        private float[] data;

        public Customer(string id, string firstname, string lastname, DateTime customerSince, bool newslettersubscriber, float[] data)
        {
            this.firstname = firstname;
            this.lastname = lastname;
            this.id = id;
            this.customerSince = customerSince;
            this.newslettersubscriber = newslettersubscriber;
            this.data = data;
        }

        public float[] Data
        {
            get { return data; }
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
