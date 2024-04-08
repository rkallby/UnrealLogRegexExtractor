using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Net;

namespace LoadLogging
{
    internal class Program
    {
        class LogInItem
        {
            private string identifierValue;
            private string ipValue;
            private string startValue;
            private string resourceValue;
            private string endValue;
            private string dataValue;
            private string siteValue;
            private string connectionTime;

            public string Field1
            {
                get => identifierValue; set => identifierValue = value;
            }
            public string Field2
            {
                get => ipValue; set => ipValue = value;
            }
            public string Field3
            {
                get => startValue; set => startValue = value;
            }
            public string Field4
            {
                get => resourceValue; set => resourceValue = value;
            }

            public string Field5
            {
                get => endValue; set => endValue = value;
            }

            public string Field6
            {
                get => dataValue; set => dataValue = value;
            }

            public string Field7
            {
                get => siteValue; set => siteValue = value;
            }

            public string Field8
            {
                get => connectionTime; set => connectionTime = value;
            }
        }

        class tempLogout
        {
            private string identifierValue;
            private string endValue;
            private string dataValue;

            public string Field1
            {
                get => identifierValue; set { identifierValue = value; }
            }

            public string Field2
            {
                get => endValue; set { endValue = value; }
            }

            public string Field3
            {
                get => dataValue; set { dataValue = value; }
            }
                    
}
        static void Main(string[] args)
        {
            //Read in the "Configuration" file that will have connection string/mysql connection information.


            //==================== End of MySQL Connection Read-In Parameters Section ================//


            //Read in the file path of the file from the last day prior//
            //Get todays date
            //Use date data to make the full name in format unreal media puts it
            //Assign to filepath Variable.
            string filePath = @"C:\tmp\LogFiles\27 March 2024.log"; //Path will be from above code section//
            //======= End of logic to get the day priors file to upload =====//




            //Spawn an array for login events and logout events
            ArrayList arrayLogin        = new ArrayList();
            ArrayList arrayLogoff       = new ArrayList();
            ArrayList arrayLoginClean   = new ArrayList();
            ArrayList arrayLogoutClean  = new ArrayList();

            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        //If Login Event Put In Login Array
                        if (line.Contains("LogIn"))
                        {
                            arrayLogin.Add(line);
                        }
                        //If LogOut Event Put In LogOut Array
                        else if (line.Contains("LogOut"))
                        {
                            arrayLogoff.Add(line);
                        }
                        //Something is wrong with that data line.
                        else
                        {
                            System.Console.WriteLine("Something messed up");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }

            //Compare the number of login and logout events. They should match...
            Console.WriteLine("Number of events in Login: " + arrayLogin.Count);
            Console.WriteLine("Number of events in LogOff: " + arrayLogoff.Count);


            //Loop through all the login events and strip the information. Then find the 
            //AnonymousID of the matching event in the logout array. 
            foreach (string temp in arrayLogin)
            {
                var tempObj = new LogInItem();
                string idValue;
                string ipValue;
                string startValue;
                string resourceValue;

                //Regex to get the ID =========================================//
                Match match1 = Regex.Match(temp, @"Anonymous\-ID\:(\d+)");
                if (match1.Success)
                {
                    idValue = match1.Groups[1].Value;
                    tempObj.Field1 = idValue;
                    //Console.WriteLine(idValue);
                }
                //Regex to get the IP ==========================================//
                MatchCollection match2 = Regex.Matches(temp, @"\b(?:[0-9]{1,3}\.){3}[0-9]{1,3}\b");
                if (match2.Count > 0)
                {
                    ipValue = match2[0].Value;
                    tempObj.Field2 = ipValue;
                    //Console.WriteLine("Extracted IP Address: " + ipAddress);
                }

                //Regex to get the StartTime
                int index1 = temp.IndexOf("Time:", StringComparison.OrdinalIgnoreCase);
                if (index1 >= 0)
                {
                    
                    string substring = temp.Substring(index1 + 6);

                    //Cut off after the ;//
                    int index2 = substring.IndexOf(";", StringComparison.OrdinalIgnoreCase);
                    if (index2 >= 0)
                    {
                        startValue = substring.Substring(0, index2);
                        tempObj.Field3 = startValue;
                        //Console.WriteLine(startValue);
                    }
                }

                //Regex to get the resource Value
                int index = temp.IndexOf("Resource:", StringComparison.OrdinalIgnoreCase);
                if (index >= 0)
                {
                    resourceValue = temp.Substring((index + 10));
                    tempObj.Field4 = resourceValue;
                    //Console.WriteLine(resourceValue);
                }


                arrayLoginClean.Add(tempObj);
            }


            //Load all the sent items into objects to parse through
            foreach (string temp in arrayLogoff)
            {
                var tempObj = new tempLogout();
                string uniqueID;
                string endValue;
                string dataValue;

                //REGEX EXPRESSION TO GET THE IDENTITY VALUE
                Match match1 = Regex.Match(temp, @"Anonymous\-ID\:(\d+)");
                if (match1.Success)
                {
                    uniqueID = match1.Groups[1].Value;
                    tempObj.Field1 = uniqueID;
                    //Console.WriteLine(idValue);
                }

                //REGEX EXPRESSION TO GET DATE DISCONECT
                int index1 = temp.IndexOf("Time:", StringComparison.OrdinalIgnoreCase);
                if (index1 >= 0)
                {

                    string substring = temp.Substring(index1 + 6);

                    //Cut off after the ;//
                    int index2 = substring.IndexOf(";", StringComparison.OrdinalIgnoreCase);
                    if (index2 >= 0)
                    {
                        endValue = substring.Substring(0, index2);
                        tempObj.Field2 = endValue;
                        //Console.WriteLine(startValue);
                    }
                }

                //REGEX Expression to get Data Used For Session. 
                int index = temp.IndexOf("transfered:", StringComparison.OrdinalIgnoreCase);
                if (index >= 0)
                {
                    dataValue = temp.Substring((index + 12));
                    tempObj.Field3 = dataValue;
                }
                arrayLogoutClean.Add(tempObj);
            }

            //foreach (LogInItem item in arrayLoginClean)
            //{
            //    Console.WriteLine("ID: " + item.Field1 + "  IP: " + item.Field2 + "  Start:" + item.Field3 + "  Resource: " + item.Field4);
            //    
            //}

            //foreach(tempLogout item in arrayLogoutClean)
            //{
            //    Console.WriteLine("ID: " + item.Field1 + "  End: " + item.Field2 + "  Data (KB's): " + item.Field3);
            //}

            //Combine to master object
            foreach(LogInItem item in arrayLoginClean)
            {

                foreach(tempLogout subitem in arrayLogoutClean)
                {
                    if (item.Field1 == subitem.Field1) 
                    {
                        item.Field5 = subitem.Field2;
                        item.Field6 = subitem.Field3;
                    }
                    else
                    {
                        //Console.WriteLine("ID: " + item.Field1 + " doesn't match: " + subitem.Field1);
                    }
                }
            }

            //Extrapolate the site code data.
            foreach(LogInItem item in arrayLoginClean)
            {
                //REGEX the IP Second Subnet
                string ipAddress = item.Field2;

                string pattern = @"^\d+\.(\d+)\.\d+\.\d+$";
                Match match = Regex.Match(ipAddress, pattern);

                if (match.Success)
                {
                    int secondSubnet = Convert.ToInt32(match.Groups[1].Value);
                    
                    if (secondSubnet == 198)
                    {
                        //Console.WriteLine("Site1 Address");
                        item.Field7 = "SITE1";
                    } 
                    else if (secondSubnet == 117)
                    {
                        //Console.WriteLine("Site2 Address");
                        item.Field7 = "SITE2";
                    }
                    else if (secondSubnet == 116)
                    {
                        //Console.WriteLine("Site3 Address");
                        item.Field7 = "SITE3";
                    }
                    else if (secondSubnet == 115)
                    {
                        //Console.WriteLine("Site4 Address");
                        item.Field7 = "SITE4";
                    }
                    else if (secondSubnet == 114)
                    {
                        //Console.WriteLine("Site5 Address");
                        item.Field7 = "SITE5";
                    }
                    else if (secondSubnet == 113)
                    {
                        //Console.WriteLine("Site6 Address");
                        item.Field7 = "SITE6";
                    }
                    else if (secondSubnet == 112)
                    {
                        //Console.WriteLine("Site7 Address");
                        item.Field7 = "SITE7";
                    }
                    else if (secondSubnet == 111)
                    {
                        //Console.WriteLine("Site8 Address");
                        item.Field7 = "SITE8";
                    } 
                    else if (secondSubnet == 110)
                    {
                        //Console.WriteLine("Site9 Address");
                        item.Field7 = "SITE9";
                    }
                    else if (secondSubnet == 109)
                    {
                        //Console.WriteLine("Site10 Address");
                        item.Field7 = "SITE10";
                    }
                    else if (secondSubnet == 108)
                    {
                        //Console.WriteLine("Site11 Address");
                        item.Field7 = "SITE11";
                    } 
                    else if (secondSubnet == 107)
                    {
                        //Console.WriteLine("Site12 Address");
                        item.Field7 = "SITE12";
                    }
                    else if (secondSubnet == 106)
                    {
                        //Console.WriteLine("Site13 Address");
                        item.Field7 = "SITE13";
                    }
                    else
                    {
                        //Console.WriteLine("Not a valid site address in refining");
                        item.Field7 = "NONREF";
                    }
                }
                else
                {
                    //Console.WriteLine("Invalid IP address");
                }


                //===== Code to calculate connection time (Minutes) ========//
                //Get the start time
                //Format to standard time units
                //Get the end time
                //Format to standard time units
                //Make calculation of time delta. 
                //===== End of code to calculate the connection time =======//


                //Calculate time delta values//

                if (item.Field3 != null && item.Field5 != null)
                {
                    string startTime = item.Field3;
                    string endTime   = item.Field5;

                    //Console.WriteLine(item.Field3);
                    //Console.WriteLine(item.Field5);


                    string format = "HH:mm:ss d MMMM yyyy";

                    DateTime early = DateTime.ParseExact(startTime, format, null);
                    DateTime late = DateTime.ParseExact(endTime, format, null);

                    TimeSpan diff = late - early;

                    item.Field8 = (diff.TotalMinutes).ToString();
                }
                else
                {
                    Console.WriteLine("Error getting start and stop times, skipping calculation and setting time to 0");
                    item.Field8 = "0";
                }

                Console.WriteLine("ID: " + item.Field1 + "  IP: " + item.Field2 + "  Start:" + item.Field3 + "  Resource: " + item.Field4 + "  End: " + item.Field5 + "  Data KB: " + item.Field6 + "  SITE: " + item.Field7 + "  Connection (Mins): " + item.Field8);

            }

            //If we wanted to get extra fancy, add NSLookup Query and Query to SCCM to get the User Accessing (Create a unique users list)====//
            //Loop through the master object
            //Read the value for IP Address
            //Do an NSLOOKUP to get Device Name
            //Do a lookup for owner/last login account to the device name in SCCM.===============================================================// 

            //Other thing is we could tie each of the resource names to a site (or include it...) and then we could track requests to each site from diff sites.

            //MYSQL QUERY TO WRITE DATA TO THE DATABASE TABLE FOR THE DAY




            int i = 0;
            while(i == 0)
            {

            }
        }
    }
}
