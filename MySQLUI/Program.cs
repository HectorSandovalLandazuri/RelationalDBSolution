// See https://aka.ms/new-console-template for more information
using DataAccessLibrary;
using DataAccessLibrary.Models;
using Microsoft.Extensions.Configuration;

MySQLCrud sql = new MySQLCrud(GetConnectionString());
ReadContact(sql, 1);
//CreateContact(sql);
//UpdateContactName(sql);
//ReadAllContacts(sql);
//RemovePhoneNumberFromContact(sql, 2, 1);
Console.WriteLine("Done Processing MySQL");


static string GetConnectionString(string connectionStringName = "Default")
{
    string output = "";
    var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json");
    var config = builder.Build();
    output = config.GetConnectionString(connectionStringName);
    return output;
}

static void CreateContact(MySQLCrud sql)
{
    FullContactModel user = new FullContactModel
    {
        BasicInfo = new BasicContactModel
        {
            FirstName = "Ricardo",
            LastName = "Sandoval"
        }
    };

    user.EmailAddresses.Add(new EmailAddressModel { EmailAddress = "ricardo@hotmail.com" });
    user.EmailAddresses.Add(new EmailAddressModel { Id = 2, EmailAddress = "hola@hola1.com" });
    user.PhoneNumbers.Add(new PhoneNumberModel { Id = 1, PhoneNumber = "5555-333-333" });
    user.PhoneNumbers.Add(new PhoneNumberModel { PhoneNumber = "8888-333-333" });
    sql.CreateContact(user);
}

static void UpdateContactName(MySQLCrud sql)
{
    BasicContactModel user = new BasicContactModel
    {
        Id = 2,
        FirstName = "Nicolás",
        LastName = "Sand"
    };
    sql.UpdateContactName(user);

}

static void RemovePhoneNumberFromContact(MySQLCrud sql, int contactId, int phoneNumberId)
{
    sql.RemovePhoneNumberFromContact(contactId, phoneNumberId);
}

static void ReadAllContacts(MySQLCrud sql)
{
    var rows = sql.GetAllContacts();
    foreach (var row in rows)
    {
        Console.WriteLine($"{row.Id}: {row.FirstName} {row.LastName}");
    }
}

static void ReadContact(MySQLCrud sql, int contactId)
{
    var contact = sql.GetFullContactById(contactId);
    Console.WriteLine($"{contact.BasicInfo.Id}: {contact.BasicInfo.FirstName} {contact.BasicInfo.LastName}");



}

