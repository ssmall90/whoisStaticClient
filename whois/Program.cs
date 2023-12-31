﻿using MySql.Data;
using MySql.Data.MySqlClient;
using Microsoft.VisualBasic;
using System.Net.Sockets;
using System.Reflection.Metadata.Ecma335;
using whois;
//sw.WriteLine(line);   Need to remove this line after testing
//sw.Flush();           Need to remove this line after testing 


SQLDataManager databaseManager = new SQLDataManager();

//databaseManager.GetDump("ellen");

//databaseManager.GetLookup("cssbct", "location");
//databaseManager.UpdateNewUser("john", "surname", "smith");
//databaseManager.UpdateExistingUser("ellen", "forenames", "Ellen");

//databaseManager.Connection.Close();


Boolean debug = true;


if (args.Length == 0)
{

    Console.WriteLine("Starting Server....");
    RunServer();
    ProcessCommand(Console.ReadLine());

}
else
{
    for (int i = 0; i < args.Length; i++)
    {
        ProcessCommand(args[i]);
    }
}
//void RunServer()
//{
//    TcpListener listener;
//    Socket connection;
//    NetworkStream socketStream;
//    try
//    {
//        listener = new TcpListener(43);
//        while (true)
//        {
//            if (debug) Console.WriteLine("Server Waiting connection...");
//            connection = listener.AcceptSocket();
//            socketStream = new NetworkStream(connection);
//            doRequest(socketStream);
//            socketStream.Close();
//            connection.Close();
//        }
//    }
//    catch (Exception e)
//    {
//        Console.WriteLine(e.ToString());
//    }
//    if (debug)
//        Console.WriteLine("Terminating Server");
//}
void RunServer()
{
    TcpListener listener;
    Socket connection;
    NetworkStream socketStream;
    try
    {
        listener = new TcpListener(43);
        listener.Start();


        while (true)
        {

            connection = listener.AcceptSocket();
            connection.SendTimeout = 1000;
            connection.ReceiveTimeout = 1000;
            socketStream = new NetworkStream(connection);
            doRequest(socketStream);
            socketStream.Close();
            connection.Close();



        }
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.ToString());

    }
    if (debug)
    {
        Console.WriteLine("Terminating Server");
    }


}

void doRequest(NetworkStream socketStream)
{
    StreamWriter sw = new StreamWriter(socketStream);
    StreamReader sr = new StreamReader(socketStream);


    if (debug) Console.WriteLine("Waiting for input from client...");

    try
    {

        String line = sr.ReadLine();

        // Process the received line
        Console.WriteLine($"Received: {line}");


        if (line == null)
        {
            if (debug) Console.WriteLine("Ignoring null command");
            return;
        }



        Console.WriteLine($"Received Network Command: '{line}'");

        if (line == "POST / HTTP/1.1")
        {

            int contentLength = 0;

            while (line != "")
            {
                line = sr.ReadLine(); // Skip to blank line

                if (line.StartsWith("Content-Length: "))
                {
                    contentLength = Int32.Parse(line.Substring(16));
                }

                //line = sr.ReadLine();
                if (debug) Console.WriteLine($"Skipped Header Line: '{line}'");
            }

            line = "";
            for (int i = 0; i < contentLength; i++) line += (char)sr.Read();

            String[] slices = line.Split(new char[] { '&' }, 2);
            String ID = slices[0].Substring(5);
            String value = slices[1].Substring(9);

            if (debug) Console.WriteLine($"Received an update request for '{ID}' to '{value}'");


        }

        else if (line.StartsWith("GET /?name=") && line.EndsWith(" HTTP/1.1"))
        {

            String[] slices = line.Split(" ");  // Split into 3 pieces
            String ID = slices[1].Substring(7);  // start at the 7th letter of the middle slice - skip `/?name=
            string result = (databaseManager.GetLookup(ID, "location"));

            if (result is not null)
            {

                sw.WriteLine("HTTP/1.1 200 OK");
                sw.WriteLine("Content-Type: text/plain");
                sw.WriteLine();
                sw.WriteLine(result);
                sw.Flush();


                Console.WriteLine($"Performed Lookup on '{ID}' returning '{result}'");
            }

            else
            {
                sw.WriteLine("HTTP/1.1 404 Not Found");
                sw.WriteLine("Content-Type: text/plain");
                sw.WriteLine();
                sw.Flush();
                Console.WriteLine($"Performed Lookup on '{ID}' returning '404 Not Found'");
            }
        }

        else
        {
            // We have an error
            Console.WriteLine($"Unrecognised command: '{line}'");
            sw.WriteLine("HTTP/1.1 400 Bad Request");
            sw.WriteLine("Content-Type: text/plain");
            sw.WriteLine();
            sw.Flush();
        }

    }

    catch (Exception ex)
    {
        Console.WriteLine($"Fault in Command Processing: {ex.ToString()}");
    }
    finally
    {
        sw.Close();
        sr.Close();

    }

}

void ProcessCommand(string command)
{
    try
    {
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

        if (operation == null || operation == string.Empty)
        {

            if (databaseManager.GetLookup(ID,"loginId") is not null)
            {
                Dump(ID);
            }
            else
            {
                Console.WriteLine("User does not exist");
            }
        }

        else if (update == null)
        {
            Lookup(ID, field);
        }

        else
        {
            if (databaseManager.GetLookup(ID,"loginId") is not null)
            {
                string newID = command.Split("?")[0];
                //DataBase.Add(newID, new User
                //{
                //    UserID = newID,
                //    Surname = "",
                //    Forenames = "",
                //    Title = "",
                //    Position = "",
                //    Phone = "",
                //    Email = "",
                //    Location = ""
                //});
                update = command.Split("=")[1];
                databaseManager.UpdateNewUser(newID, field, update);

            }
            else
            {
                update = command.Split("=")[1];
                Update(ID, field, update);
            }

        }
    }

    catch (Exception e)
    {
        Console.WriteLine($"Fault in Command Processing: {e.ToString()}");
    }


}

void Dump(String ID)
{

    databaseManager.GetDump(ID);

    //if (debug) Console.WriteLine("output all fields");
    //Console.WriteLine($"UserID={DataBase[ID].UserID}");
    //Console.WriteLine($"Surname={DataBase[ID].Surname}");
    //Console.WriteLine($"Forenames={DataBase[ID].Forenames}");
    //Console.WriteLine($"Title={DataBase[ID].Title}");
    //Console.WriteLine($"Position={DataBase[ID].Position}");
    //Console.WriteLine($"Phone={DataBase[ID].Phone}");
    //Console.WriteLine($"Email={DataBase[ID].Email}");
    //Console.WriteLine($"location={DataBase[ID].Location}");
}

void Lookup(String ID, String field)
{
    if (databaseManager.GetLookup(ID, "loginId") is not null)
    {
        //if (debug)
        //    Console.WriteLine($" lookup field '{field}'");
        //String result = null;
        //switch (field.ToLower())
        //{
        //    case "location": result = DataBase[ID].Location; break;
        //    case "userid": result = DataBase[ID].UserID; break;
        //    case "surname": result = DataBase[ID].Surname; break;
        //    case "forenames": result = DataBase[ID].Forenames; break;
        //    case "title": result = DataBase[ID].Title; break;
        //    case "phone": result = DataBase[ID].Phone; break;
        //    case "position": result = DataBase[ID].Position; break;
        //    case "email": result = DataBase[ID].Email; break;
        //}
        Console.WriteLine(databaseManager.GetLookup(ID,field));
    }
    else
    {
        Console.WriteLine("User does not exist");
    }

}

void Update(String ID, String field, String update)
{

    if (databaseManager.GetLookup(ID, "loginId") is not null)
    {
        //if (debug)
        //    Console.WriteLine($" update field '{field}' to '{update}'");
        //switch (field.ToLower())
        //{
        //    case "location": DataBase[ID].Location = update; field = DataBase[ID].Location; break;
        //    case "userid": DataBase[ID].UserID = update; field = DataBase[ID].UserID; break;
        //    case "surname": DataBase[ID].Surname = update; field = DataBase[ID].Surname; break;
        //    case "forenames": DataBase[ID].Forenames = update; field = DataBase[ID].Forenames; break;
        //    case "title": DataBase[ID].Title = update; field = DataBase[ID].Title; break;
        //    case "phone": DataBase[ID].Phone = update; field = DataBase[ID].Phone; break;
        //    case "position": DataBase[ID].Position = update; field = DataBase[ID].Position; break;
        //    case "email": DataBase[ID].Email = update; field = DataBase[ID].Email; break;
        //}

        databaseManager.UpdateExistingUser(ID, field, update);

        Console.WriteLine(databaseManager.GetLookup(ID, field));
    }


    Console.WriteLine("OK");
}

