
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace _160805021_BankingApp
{
    public class Program
    {
        public static string path = @"C:\Programming\C# Projects\_160805021_BankingApp\_160805021_BankingApp\Accounts.csv";
        static List<Account> accountHolder = new List<Account>();
        public static void Main(string[] args)
        {
            Random generator = new Random();

            while (true)
            {
                // Read saved accounts from csv file and store them in a List
                using (var reader = new StreamReader(path))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(',');

                        accountHolder.Add(new Account(values[0], double.Parse(values[1]), values[2], values[3]));
                    }
                }
                Console.WriteLine("Welcome to our banking app. How may we help you?");
                Console.Write("1. Set up account\n" +
                              "2. Sign in\n" +
                              "Select Function: ");
                string ans = Console.ReadLine().ToUpper();
                if (ans == "1")
                {
                    Console.Write("Enter your full name: ");
                    string name = Console.ReadLine();
                    Console.Write("Enter initial deposit: ");
                    double deposit = double.Parse(Console.ReadLine());
                    String no = generateAccountNumber();

                    int i = 0;
                    while (i < accountHolder.Count)
                    {
                        if (accountHolder[i].accountNumber.Equals(no))
                        {
                            no = generateAccountNumber();
                            i = 0;
                        }
                        else { i++; }

                    }
                    String accountNumber = no;
                    Console.WriteLine("Enter your 4-digit PIN");
                    String PIN = ReadPassword();
                    accountHolder.Add(new Account(name, deposit, accountNumber, PIN));
                    saveAccountInCSV(name, deposit, accountNumber, PIN);

                    Console.WriteLine("Your bank account has been successfully created!");
                    Console.WriteLine("NAME: " + name +
                    "\nACCOUNT NUMBER: " + accountNumber +
                    "\nBALANCE: " + deposit
                    );
                    Console.ReadLine();
                }
                else if (ans == "2")
                {
                    int check = 0;
                    Console.Write("Enter your account number: ");
                    string accountNo = Console.ReadLine();
                    for (int i = 0; i < accountHolder.Count; i++)
                    {
                        if (accountHolder[i].accountNumber.Equals(accountNo))
                        {
                            check++;
                            while (true)
                            {
                                Console.Write("Enter your 4 digit PIN: ");
                                string Chk = ReadPassword();

                                if (accountHolder[i].getPIN == Chk)
                                {
                                    Console.WriteLine("Welcome " + accountHolder[i].accountName);
                                    Console.Write("1. Check balance\n" +
                                      "2. Deposit\n" +
                                      "3. Withdraw\n" +
                                      "4. Transfer\n" +
                                      "5. Buy Airtime\n" +
                                      "X. Terminate\n\n" +
                                      "Select Function: ");
                                    string num = Console.ReadLine();
                                    accountFunctions(num, accountHolder, i);
                                    //break;
                                }
                                else
                                {
                                    Console.WriteLine("Incorrect PIN");
                                    //break;
                                }
                            }
                        }
                    }
                    if (check < 1)
                    {
                        Console.WriteLine("Your account wasn't found");
                        Console.ReadLine();
                    }


                }
                else if (ans == "X")
                {
                    Environment.Exit(0);
                    Console.Clear();
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid Syntax!");
                    Console.ReadLine();
                }


            }

            Console.Write("Press any key to continue . . . ");
            Console.ReadKey(true);
        }

        public static string generateAccountNumber()
        {
            Random generator = new Random();
            int count = 0;
            int i = generator.Next(100000000, 999999999) + 1;
            return i.ToString();
        }

        public static string ReadPassword()
        {
            string password = "";
            ConsoleKeyInfo info = Console.ReadKey(true);
            while (info.Key != ConsoleKey.Enter)
            {
                if (info.Key != ConsoleKey.Backspace)
                {
                    Console.Write("*");
                    password += info.KeyChar;
                }
                else if (info.Key == ConsoleKey.Backspace)
                {
                    if (!string.IsNullOrEmpty(password))
                    {
                        // remove one character from the list of password characters
                        password = password.Substring(0, password.Length - 1);
                        // get the location of the cursor
                        int pos = Console.CursorLeft;
                        // move the cursor to the left by one character
                        Console.SetCursorPosition(pos - 1, Console.CursorTop);
                        // replace it with space
                        Console.Write(" ");
                        // move the cursor to the left by one character again
                        Console.SetCursorPosition(pos - 1, Console.CursorTop);
                    }
                }
                info = Console.ReadKey(true);
            }

            // add a new line because user pressed enter at the end of their password
            Console.WriteLine();
            return password;
        }

        public static void saveAccountInCSV(string name, double deposit, string accountNumber, string PIN)
        {
            var csv = new StringBuilder();
            var line = string.Format("{0},{1},{2},{3}", name, deposit, accountNumber, PIN);
            csv.AppendLine(line);
            File.AppendAllText(path, csv.ToString());
        }

        public static void updateCsv(List<Account> accountHolder, int i, double func)
        {
            List<String> lines1 = new List<String>();
            using (var reader = new StreamReader(path))
            {
                String line;
                string[] lines = File.ReadAllLines(path);
                for (int j = 0; j < lines.Length; j++)
                {
                    String[] values = lines[j].Split(',');
                    if (j == i)
                    {
                        if (func > 0)
                        {
                            values[1] = func.ToString();
                            line = String.Join(",", values);
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        line = String.Join(",", values);
                    }
                    lines1.Add(line);
                }
            }
            using (var writer = new StreamWriter(path, false))
            {
                foreach (String line in lines1)
                {

                    writer.WriteLine(line);
                }
            }
        }

        public static void accountFunctions(string num, List<Account> accountHolder, int i)
        {
            if (num.Equals("1"))
            {
                double func = accountHolder[i].showInfo();
                updateCsv(accountHolder, i, func);
                Console.ReadLine();
            }
            else if (num.Equals("2"))
            {
                Console.Write("Enter amount to Deposit: ");
                double deposit = double.Parse(Console.ReadLine());
                double func = accountHolder[i].deposit(deposit);
                if (func > 0)
                {
                    updateCsv(accountHolder, i, func);
                    accountHolder[i].balance = func;
                }

                Console.ReadLine();
            }
            else if (num.Equals("3"))
            {
                Console.Write("Enter amount(multiple of 500) to Withdraw: ");
                double amount = double.Parse(Console.ReadLine());
                double func = accountHolder[i].withdraw(amount);
                if (func > 0)
                {
                    updateCsv(accountHolder, i, func);
                    accountHolder[i].balance = func;
                }

                Console.ReadLine();
            }
            else if (num.Equals("4"))
            {
                Console.Write("Enter account number you wish to transfer to: ");
                String accountNo = Console.ReadLine();
                int l = 0;
                for (int j = 0; j < accountHolder.Count; j++)
                {
                    if (accountHolder[j].getAccountNumber().Equals(accountNo))
                    {
                        l++;
                        Console.Write("Enter amount you wish to transfer: ");
                        double amount = double.Parse(Console.ReadLine());
                        double func = accountHolder[i].transfer(accountNo, amount);

                        if (func > 0)
                        {
                            accountHolder[i].balance = func;
                            accountHolder[j].balance += amount;
                            updateCsv(accountHolder, i, func);
                            updateCsv(accountHolder, j, accountHolder[j].balance);
                        }

                        break;
                    }
                }
                if (l == 0)
                {
                    Console.WriteLine("Account number was not found!");
                }
                Console.ReadLine();
            }
            else if (num.Equals("5"))
            {
                Console.Write("Select your network\n1. MTN\n2. Airtel\n3. Etisalat\n4. Glo\n");
                string chk = Console.ReadLine().ToUpper();
                if (chk == "1" || chk == "2" || chk == "3" || chk == "4")
                {
                    Console.WriteLine("Enter your phone number: ");
                    String number = Console.ReadLine();
                    Console.WriteLine("Enter the amount you wish to load: ");
                    double amount = double.Parse(Console.ReadLine());
                    double func = accountHolder[i].loadAirtime(number, amount);
                    if (func > 0)
                    {
                        accountHolder[i].balance = func;
                        updateCsv(accountHolder, i, func);
                    }

                }
                else
                {
                    Console.WriteLine("Select a valid option");
                }
                Console.ReadLine();
            }
            else if (num.Equals("X"))
            {
                Environment.Exit(0);
            }
            else
            {
                Console.WriteLine("Invalid Syntax!");
                Console.ReadLine();
            }
        }
    }
}