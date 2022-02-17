// Charles Simms
// CIS297
// Program to display a patient's id, name, and balance then show a minimum balance due
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;


namespace PatientRecordSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            FileOperations();
            DirectoryOperations();
            FileStreamOperations();
            SequentialAccessWriteOperation();
            ReadSequentialAccessOperation();
            FindPatients();
            SerializableDemonstration();

        }

        //File operations
        static void FileOperations()
        {

            string fileName;
            Write("Enter a filename >> ");
            fileName = ReadLine();
            if (File.Exists(fileName))
            {
                WriteLine("File exists");
                WriteLine("File was created " +
                   File.GetCreationTime(fileName));
                WriteLine("File was last written to " +
                   File.GetLastWriteTime(fileName));
            }
            else
            {
                WriteLine("File does not exist");
            }
        }

        //Directory Operations
        static void DirectoryOperations()
        {
            //Directory operations
            string directoryName;
            string[] listOfFiles;
            Write("Enter a folder >> ");
            directoryName = ReadLine();
            if (Directory.Exists(directoryName))
            {
                WriteLine("Directory exists, and it contains the following:");
                listOfFiles = Directory.GetFiles(directoryName);
                for (int x = 0; x < listOfFiles.Length; ++x)
                    WriteLine("   {0}", listOfFiles[x]);
            }
            else
            {
                WriteLine("Directory does not exist");
            }
        }

        //Using FileStream to create and write some text into it
        static void FileStreamOperations()
        {
            FileStream outFile = new
            FileStream("SomeText.txt", FileMode.Create,
            FileAccess.Write);
            StreamWriter writer = new StreamWriter(outFile);
            Write("Enter some text >> ");
            string text = ReadLine();
            writer.WriteLine(text);
            // Error occurs if the next two statements are reversed
            writer.Close();
            outFile.Close();
        }

        //Writing data to a Sequential Access text file
        static void SequentialAccessWriteOperation()
        {
            const int END = 999;
            const string DELIM = ",";
            const string FILENAME = "PatientData.txt";
            Patient pat = new Patient();
            FileStream outFile = new FileStream(FILENAME,
               FileMode.Create, FileAccess.Write);
            StreamWriter writer = new StreamWriter(outFile);
            Write("Enter patient id number or " + END +
               " to quit >> ");
            pat.PatNum = Convert.ToInt32(ReadLine());
            while (pat.PatNum != END)
            {
                Write("Enter last name of patient >> ");
                pat.Name = ReadLine();
                Write("Enter balance >> ");
                pat.Balance = Convert.ToDouble(ReadLine());
                writer.WriteLine(pat.PatNum + DELIM + pat.Name +
                   DELIM + pat.Balance);
                Write("Enter next patient number or " +
                   END + " to quit >> ");
                pat.PatNum = Convert.ToInt32(ReadLine());
            }
            writer.Close();
            outFile.Close();
        }

        //Read data from a Sequential Access File
        static void ReadSequentialAccessOperation()
        {
            const char DELIM = ',';
            const string FILENAME = "PatientData.txt";
            Patient pat = new Patient();
            FileStream inFile = new FileStream(FILENAME,
               FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(inFile);
            string recordIn;
            string[] fields;
            WriteLine("\n{0,-5}{1,-12}{2,8}\n",
               "Num", "Name", "Balance");
            recordIn = reader.ReadLine();
            while (recordIn != null)
            {
                fields = recordIn.Split(DELIM);
                pat.PatNum = Convert.ToInt32(fields[0]);
                pat.Name = fields[1];
                pat.Balance = Convert.ToDouble(fields[2]);
                WriteLine("{0,-5}{1,-12}{2,8}",
                   pat.PatNum, pat.Name, pat.Balance.ToString("C"));
                recordIn = reader.ReadLine();
            }
            reader.Close();
            inFile.Close();
        }

        //repeatedly searches a file to produce 
        //lists of Patients who meet a minimum balance requirement
        static void FindPatients()
        {
            const char DELIM = ',';
            const int END = 999;
            const string FILENAME = "PatientData.txt";
            Patient pat = new Patient();
            FileStream inFile = new FileStream(FILENAME,
               FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(inFile);
            string recordIn;
            string[] fields;
            double minBalance;
            Write("Enter minimum balance to find or " +
               END + " to quit >> ");
            minBalance = Convert.ToDouble(Console.ReadLine());
            while (minBalance != END)
            {
                WriteLine("\n{0,-5}{1,-12}{2,8}\n",
                   "Num", "Name", "Balance");
                inFile.Seek(0, SeekOrigin.Begin);
                recordIn = reader.ReadLine();
                while (recordIn != null)
                {
                    fields = recordIn.Split(DELIM);
                    pat.PatNum = Convert.ToInt32(fields[0]);
                    pat.Name = fields[1];
                    pat.Balance = Convert.ToDouble(fields[2]);
                    if (pat.Balance >= minBalance)
                        WriteLine("{0,-5}{1,-12}{2,8}", pat.PatNum,
                           pat.Name, pat.Balance.ToString("C"));
                    recordIn = reader.ReadLine();
                }
                Write("\nEnter minimum balance to find or " +
                   END + " to quit >> ");
                minBalance = Convert.ToDouble(Console.ReadLine());
            }
            reader.Close();  // Error occurs if
            inFile.Close(); //these two statements are reversed
        }

        //Serializable Demonstration
        /// <summary>
        /// writes Person class objects to a file and later reads them 
        /// from the file into the program
        /// </summary>
        static void SerializableDemonstration()
        {
            const int END = 999;
            const string FILENAME = "Data.ser";
            Person pat = new Person();
            FileStream outFile = new FileStream(FILENAME,
               FileMode.Create, FileAccess.Write);
            BinaryFormatter bFormatter = new BinaryFormatter();
            Write("Enter patient id number or " + END +
               " to quit >> ");
            pat.PatNum = Convert.ToInt32(ReadLine());
            while (pat.PatNum != END)
            {
                Write("Enter last name >> ");
                pat.Name = ReadLine();
                Write("Enter balance >> ");
                pat.Balance = Convert.ToDouble(ReadLine());
                bFormatter.Serialize(outFile, pat);
                Write("Enter patient id number or " + END +
                   " to quit >> ");
                pat.PatNum = Convert.ToInt32(ReadLine());
            }
            outFile.Close();
            FileStream inFile = new FileStream(FILENAME,
               FileMode.Open, FileAccess.Read);
            WriteLine("\n{0,-5}{1,-12}{2,8}\n",
               "Num", "Name", "Balance");
            while (inFile.Position < inFile.Length)
            {
                pat = (Person)bFormatter.Deserialize(inFile);
                WriteLine("{0,-5}{1,-12}{2,8}",
                   pat.PatNum, pat.Name, pat.Balance.ToString("C"));
            }
            inFile.Close();
        }
    }

    class Patient // renamed to Patient as Employee makes no sense anymore
    {
        public int PatNum { get; set; }
        public string Name { get; set; }
        public double Balance { get; set; }

    }

    class Person
    {
        public int PatNum { get; set; }
        public string Name { get; set; }
        public double Balance { get; set; }
    }

}
