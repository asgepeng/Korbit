using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Korbit
{

    internal class Program
    {
        static Models.DbClient db;
        static void Main(string[] args)
        {
            db = new Models.DbClient();
            var user = CollectUserData();

            Console.Write("Do you to save this data to database (Y/N)?");
            var keyPressed = Console.ReadKey();
            if (keyPressed.KeyChar == 'Y' || keyPressed.KeyChar == 'y')
            {
                var repository = new Models.UserRepository(db);
                user.Id = repository.Create(user);
                if (user.Id > 0)
                {
                    Console.WriteLine("User data have been succesfully saved to the database.");
                    Console.WriteLine("Thanks for trying this :-)");
                }
                else
                {
                    Console.WriteLine("Unable to complete the request. User data could not be saved to the database.");
                }
            }
            Console.WriteLine("Collected user data not saved! see you next month");
        }
        static Models.User CollectUserData()
        {
            Console.WriteLine("COLLECT DATA USER");
            List<string> fields = new List<string>();
            fields.Add("COLLECT DATA USER");
        getUsername:
            Console.Write("Username *    : ");
            var user = new Models.User();
            user.Name = Console.ReadLine();
            if (user.Name.Trim() == "")
            {
                RefreshScreen("ERROR: Username is required", fields);
                goto getUsername;
            }
            fields.Add($"Username *     : {user.Name}");
            RefreshScreen("", fields);
        getEmailAddress:
            Console.Write("Email Address *: ");
            var emailAddress = Console.ReadLine();
            if (emailAddress.Trim() == "")
            {
                RefreshScreen("ERROR: Email address is required", fields);
                goto getEmailAddress;
            }
            if (!IsValidEmail(emailAddress))
            {
                RefreshScreen("ERROR: Please enter valid email", fields);
                goto getEmailAddress;
            }
            fields.Add($"Email Address *: {emailAddress}");
            RefreshScreen("", fields);
        getEmailType:
            Console.Write("Email Type (1. Office, 2. Personal) *: ");
            var type = Console.ReadKey();
            if (type.KeyChar != '1' && type.KeyChar != '2')
            {
                RefreshScreen("ERROR: Please type 1 or 2 for email type", fields);
                goto getEmailType;
            }
            int.TryParse(type.KeyChar.ToString(), out int iType);
            var email = new Models.Email()
            {
                Address = emailAddress,
                EmailTypeID = iType
            };
            user.Emails.Add(email);
            fields.Add($"Email Type *   : {(email.EmailTypeID == 1 ? "Office" : "Personal")}");
            RefreshScreen("", fields);
        getDateOfBirth:
            Console.Write("Date of birth (dd/MM/yy) *: ");
            if (!TryGetDate(Console.ReadLine(), out DateTime dob))
            {
                RefreshScreen("ERROR: Please enter valid date format", fields);
                goto getDateOfBirth;
            }
            user.DateOfBirth = dob;
            fields.Add($"Date of Birth *: {user.DateOfBirth}");
            RefreshScreen("", fields);
        getPhoneNumber:
            Console.Write("Phone Number * : ");
            var sPhoneNumber = Console.ReadLine();
            if (!long.TryParse(sPhoneNumber, out long phoneNumber))
            {
                RefreshScreen("ERROR: Please enter valid phone number", fields);
                goto getPhoneNumber;
            }            
            fields.Add($"Phone Number * : {sPhoneNumber}");
            RefreshScreen("", fields);
        getPhoneType:
            Console.Write("Phone Type (1. Home, 2. Celluler)*: ");
            var phoneType = Console.ReadKey();
            if (phoneType.KeyChar != '1' && phoneType.KeyChar != '2')
            {
                RefreshScreen("ERROR: Please type 1 or 2 for phone type", fields);
                goto getPhoneType;
            }
            int.TryParse(type.KeyChar.ToString(), out int iPhoneType);
            var phone = new Models.Phone()
            {
                Number = sPhoneNumber,
                PhoneTypeId = iPhoneType
            };
            fields.Add($"Email Type *   : {(iPhoneType == 1 ? "Home" : "Celluler")}");
            RefreshScreen("", fields);
            return user;
        }
        static void RefreshScreen(string errorText, List<string> fields)
        {
            Console.Clear();
            if (errorText.Length > 0)
            {
                Console.WriteLine(errorText);
                Console.WriteLine("===========================================================");
            }
            foreach (var line in fields)
            {
                Console.WriteLine(line);
            }
        }
        static bool TryGetDate(string dateText, out DateTime result)
        {
            return DateTime.TryParse(dateText, out result);
        }
        static bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase);
        }
    }
}
