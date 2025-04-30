using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korbit.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public DateTime DateOfBirth { get; set; }
        public EmailCollection Emails { get; } = new EmailCollection();
        public AddressCollection Addresses { get; } = new AddressCollection();
        public PhoneCollection Phones { get; set; } = new PhoneCollection();
    }

    public class Address
    {
        public int Id { get; set; } = 0;
        public string StreetLine1 { get; set; } = "";
        public string StreetLine2 { get; set; } = "";
        public string City { get; set; } = "";
        public int State { get; set; } = 0;
        public int Country { get; set; }
        public string ZipCode { get; set; } = "";
        public bool IsPrimaryAddress { get; set; }
    }

    public class AddressCollection : List<Address>
    {
        public Address Add(string streetline1, string streetline2, string city, int state, int country, string zipcode)
        {
            var address = new Address()
            {
                StreetLine1 = streetline1,
                StreetLine2 = streetline2,
                City = city,
                State = state,
                Country = country,
                ZipCode = zipcode
            };
            base.Add(address);
            return address;
        }
        public void SetPrimaryAddress(int id)
        {
            var primaryAddressFound = false;
            for (int i = 0; i < this.Count; i++)
            {
                this[i].IsPrimaryAddress = this[i].Id == id;
                if (this[i].IsPrimaryAddress) primaryAddressFound = true;
            }
            if (!primaryAddressFound && this.Count > 0)
            {
                this[0].IsPrimaryAddress = true;
            }
        }
        public void LoadFromDatabase(IDbClient db, int ownerID = 0, bool clearRecords = false)
        {
            if (clearRecords)
            {
                this.Clear();
            }
            var commandText = $@"SELECT [streetline1], [streetline2], [city], [state], [country], [zipcode]
FROM addresses WITH (NOLOCK)
WHERE isDeleted = 0 {(ownerID > 0 ? " AND ownerID = " + ownerID : "")}";
            using (var reader = db.ExecuteReader(commandText))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        this.Add(new Address()
                        {
                            Id = (int)reader["id"],
                            StreetLine1 = reader["streetline1"].ToString(),
                            StreetLine2 = reader["streetline2"].ToString(),
                            City = reader["city"].ToString(),
                            State = (int)reader["state"],
                            Country = (int)reader["country"],
                            ZipCode = reader["zipcode"].ToString()
                        });
                    }
                }
            }
        }
    }

    public class PhoneType
    {
        public int Id { get; set; } = 0;
        public string Name { get; set; } = "";
    }
    public class Phone
    {
        public int Id { get; set; } = 0;
        public string Number { get; set; } = "";
        public string Ext { get; set; } = "";
        public int PhoneTypeId { get; set; } = 0;
    }

    public class PhoneCollection : List<Phone>
    {
        public Phone Add(string number, string ext, int phoneTypeID)
        {
            var phone = new Phone()
            {
                Number = number,
                Ext = ext,
                PhoneTypeId = phoneTypeID
            };
            base.Add(phone);
            return phone;
        }
        public void LoadFromDatabase(IDbClient db, int ownerID = 0, bool clearRecords = false)
        {
            if (clearRecords)
            {
                this.Clear();
            }
            var commandText = $@"SELECT [id], [number], [phoneTypeId]
FROM phones WITH (NOLOCK)
WHERE isDeleted = 0 {(ownerID > 0 ? " AND ownerID = " + ownerID : "")}";
            using (var reader = db.ExecuteReader(commandText))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        this.Add(new Phone()
                        {
                            Id = (int)reader["id"],
                            Number = reader["address"].ToString(),
                            PhoneTypeId = (int)reader["emaylTypeId"]
                        });
                    }
                }
            }
        }
    }
    public class EmailType
    {
        public int Id { get; set; } = 0;
        public string Name { get; set; } = "";
    }

    public class Email
    {
        public int Id { get; set; } = 0;
        public string Address { get; set; } = "";
        public int EmailTypeID { get; set; } = 0;
        public bool IsPrimary { get; set; } = false;
    }

    public class EmailCollection : List<Email>
    {
        public Email Add(string address, int emailTypeID)
        {
            var email = new Email()
            {
                Address = address,
                EmailTypeID = emailTypeID
            };
            base.Add(email);
            return email;
        }
        public void LoadFromDatabase(IDbClient db, int ownerID = 0, bool clearRecords = false)
        {
            if (clearRecords)
            {
                this.Clear();
            }
            var commandText = $@"SELECT id, [address], [emailTypeId]
FROM emails WITH (NOLOCK)
WHERE isDeleted = 0 { (ownerID > 0 ? " AND ownerID = " + ownerID : "")}";
            using (var reader = db.ExecuteReader(commandText))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        this.Add(new Email()
                        {
                            Id = (int)reader["id"],
                            Address = reader["address"].ToString(),
                            EmailTypeID = (int)reader["emaylTypeId"]
                        });
                    }
                }
            }
        }
        public void SetPrimaryEmail(int emailId)
        {
            var primaryEmailId = false;
            for (int i = 0; i < this.Count; i++)
            {
                this[i].IsPrimary = this[i].Id == emailId;
                if (this[i].IsPrimary) primaryEmailId = true;
            }
            if (!primaryEmailId && this.Count > 0)
            {
                this[0].IsPrimary = true;
            }
        }
    }
}
