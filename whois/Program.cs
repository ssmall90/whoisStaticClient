
using Microsoft.VisualBasic;
using whois;



if (args.Length == 0)
{
    Console.WriteLine("Starting Server....");
    RunServer();
}
else
{
    for (int i = 0; i < args.Length; i++)
    {
        ProcessCommand(args[i]);
    }
}

static void RunServer()
{
    // At the moment we do not have a server
}

static void ProcessCommand(string command)
{
    Boolean debug = true;

    Dictionary<string, User> DataBase = new Dictionary<string, User>
{
  {"cssbct",
   new User {UserID="cssbct",Surname="Tompsett",Fornames="Brian C",Title="Eur Ing",
            Position="Lecturer of Computer Science",
            Phone="+44 1482 46 5222",Email="B.C.Tompsett@hull.ac.uk",Location="in RB-336" }
   }
};

    //G:\551457\whois\bin\Debug\net6.0 >

    //  command - whois cssbct "cssbct?location=In The Lab" 123456 ? location
    //  cssbct
    //  cssbct? location = In The Lab
    //  123456 ? location

    Console.WriteLine($"\nCommand: {command}");
    String[] slice = command.Split(new char[] { '?' }, 2);
    String ID = slice[0];
    String operation = null;
    String update = null;
    String field = null;

    if (slice.Length == 2)
    {
        operation = slice[1];
        String[] pieces = operation.Split(new char[] { '=' }, 2);
        field = pieces[0];
        if (pieces.Length == 2) update = pieces[1];
    }

    if (operation == null)
    {
        Dump(ID);
    }
    else if (update == null)
    {
        Lookup(ID, field);
    }
    else
    {
        Update(ID, field, update);
    }

    void Dump(String ID)
    {
        if (debug) Console.WriteLine("output all fields");
        Console.WriteLine($"UserID={DataBase[ID].UserID}");
        Console.WriteLine($"Surname={DataBase[ID].Surname}");
        Console.WriteLine($"Fornames={DataBase[ID].Fornames}");
        Console.WriteLine($"Title={DataBase[ID].Title}");
        Console.WriteLine($"Position={DataBase[ID].Position}");
        Console.WriteLine($"Phone={DataBase[ID].Phone}");
        Console.WriteLine($"Email={DataBase[ID].Email}");
        Console.WriteLine($"location={DataBase[ID].Location}");
    }


    void Lookup(String ID, String field)
    {
        if (debug)
            Console.WriteLine($" lookup field '{field}'");
        String result = null;
        switch (field)
        {
            case "location": result = DataBase[ID].Location; break;
            case "UserID": result = DataBase[ID].UserID; break;
            case "Surname": result = DataBase[ID].Surname; break;
            case "Fornames": result = DataBase[ID].Fornames; break;
            case "Title": result = DataBase[ID].Title; break;
            case "Phone": result = DataBase[ID].Phone; break;
            case "Position": result = DataBase[ID].Position; break;
            case "Email": result = DataBase[ID].Email; break;
        }
        Console.WriteLine(result);
    }


    void Update(String ID, String field, String update)
    {
        if (debug)
            Console.WriteLine($" update field '{field}' to '{update}'");
        switch (field)
        {
            case "location": DataBase[ID].Location = update; break;
            case "UserID": DataBase[ID].UserID = update; break;
            case "Surname": DataBase[ID].Surname = update; break;
            case "Fornames": DataBase[ID].Fornames = update; break;
            case "Title": DataBase[ID].Title = update; break;
            case "Phone": DataBase[ID].Phone = update; break;
            case "Position": DataBase[ID].Position = update; break;
            case "Email": DataBase[ID].Email = update; break;
        }
        Console.WriteLine("OK");
    }
}


