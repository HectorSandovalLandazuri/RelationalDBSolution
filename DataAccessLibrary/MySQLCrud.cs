using DataAccessLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary
{
    public class MySQLCrud
    {
        private readonly string _connectionString;
        private MySQLDataAccess db = new MySQLDataAccess();

        public MySQLCrud(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<BasicContactModel> GetAllContacts()
        {
            string sql = "Select * from Contacts";
            return db.LoadData<BasicContactModel, dynamic>(sql, new { }, _connectionString);
        }

        public FullContactModel GetFullContactById(int id)
        {
            string sql = "Select * from Contacts Where Id=@id";
            FullContactModel output = new FullContactModel();
            output.BasicInfo = db.LoadData<BasicContactModel, dynamic>(sql, new { Id = id }, _connectionString).FirstOrDefault();
            if (output.BasicInfo == null)
            {
                return null;
            }
            sql = "Select EmailAddresses.Id,EmailAddresses.EmailAddress from EmailAddresses inner join ContactEmail on EmailAddresses.Id = ContactEmail.EmailAddressId Where ContactEmail.ContactId=@id";
            output.EmailAddresses = db.LoadData<EmailAddressModel, dynamic>(sql, new { Id = id }, _connectionString).ToList();
            sql = "Select PhoneNumbers.Id,PhoneNumbers.PhoneNumber from PhoneNumbers inner join ContactPhoneNumbers on PhoneNumbers.Id = ContactPhoneNumbers.PhoneNumberId Where ContactPhoneNumbers.ContactId=@id";
            output.PhoneNumbers = db.LoadData<PhoneNumberModel, dynamic>(sql, new { Id = id }, _connectionString).ToList();
            return output;
        }

        public void CreateContact(FullContactModel contact)
        {
            //Save the basic contact
            string sql = "Insert into Contacts (FirstName, LastName) Values (@FirstName,@LastName);";
            db.SaveData(sql,
                        new { contact.BasicInfo.FirstName, contact.BasicInfo.LastName },
                        _connectionString);
            //Get the Id number of the contact
            sql = "Select Id From Contacts Where FirstName=@FirstName And LastName=@LastName";
            int contactId = db.LoadData<IdLookupModel, dynamic>(sql,
                                                              new { contact.BasicInfo.FirstName, contact.BasicInfo.LastName },
                                                              _connectionString).First().Id;
            //Identify if phone number exists
            foreach (var phoneNumber in contact.PhoneNumbers)
            {
                if (phoneNumber.Id == 0)
                {
                    sql = "Insert Into PhoneNumbers (PhoneNumber) values(@PhoneNumber)";
                    db.SaveData(sql, new { phoneNumber.PhoneNumber }, _connectionString);
                    sql = "Select Id From PhoneNumbers Where PhoneNumber=@PhoneNumber";
                    phoneNumber.Id = db.LoadData<IdLookupModel, dynamic>(sql,
                                                                      new { phoneNumber.PhoneNumber },
                                                                      _connectionString).First().Id;
                }
                sql = "Insert into ContactPhoneNumbers (ContactId,PhoneNumberId) values(@ContactId,@PhoneNumberId)";
                db.SaveData(sql, new { ContactId = contactId, PhoneNumberId = phoneNumber.Id }, _connectionString);

            }

            //do the same thing for email
            foreach (var emailAddress in contact.EmailAddresses)
            {
                if (emailAddress.Id == 0)
                {
                    sql = "Insert Into EmailAddresses (EmailAddress) values(@EmailAddress)";
                    db.SaveData(sql, new { emailAddress.EmailAddress }, _connectionString);
                    sql = "Select Id From EmailAddresses Where EmailAddress=@EmailAddress";
                    emailAddress.Id = db.LoadData<IdLookupModel, dynamic>(sql,
                                                                      new { emailAddress.EmailAddress },
                                                                      _connectionString).First().Id;
                }
                sql = "Insert into ContactEmail (ContactId,EmailAddressId) values(@ContactId,@EmailAddressId)";
                db.SaveData(sql, new { ContactId = contactId, EmailAddressId = emailAddress.Id }, _connectionString);

            }

        }

        public void UpdateContactName(BasicContactModel contact)
        {
            string sql = "Update Contacts Set FirstName=@FirstName,LastName=@LastName Where Id=@Id;";
            db.SaveData(sql,
                        contact,
                        _connectionString);

        }

        public void RemovePhoneNumberFromContact(int contactId, int phoneNumberId)
        {
            string sql = "Delete From ContactPhoneNumbers Where PhoneNumberId=@PhoneNumberId And ContactId=@ContactId;";
            db.SaveData(sql,
                        new { ContactId = contactId, PhoneNumberId = phoneNumberId },
                        _connectionString);

            sql = "Select Id,ContactId,PhoneNumberId From ContactPhoneNumbers Where PhoneNumberId=@PhoneNumberId";
            var links = db.LoadData<ContactPhoneNumbersModel, dynamic>(sql, new { PhoneNumberId = phoneNumberId }, _connectionString);
            if (links.Count == 0)
                sql = "Delete From PhoneNumbers Where Id=@PhoneNumberId;";
            db.SaveData(sql,
                    new { PhoneNumberId = phoneNumberId },
                    _connectionString);
            {

            }

        }
    }
}
