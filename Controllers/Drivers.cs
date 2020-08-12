using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Driver.Controllers
{
    
    [ApiController]
    [Route("[controller]")]
    public class DriversController : ControllerBase
    {
        [EnableCors("CorsPolicy")]
        [HttpGet]
        public IEnumerable<Drivers> Get()
        {
            //Reads in input file
            string[] lines = System.IO.File.ReadAllLines(@"Drivers.txt");
            //Two possible commands, two lists.
            ArrayList drivers = new ArrayList();
            ArrayList trips = new ArrayList();

            //Initial file read in and sorting of data
            foreach (string line in lines)
            {
                //Itemizes the input line by breaking parts by the space separating them.
                string[] lineElements = line.Split(" ");
                if (lineElements[0].Equals("Driver"))
                {
                    //try-catch to make sure file is formatted correctly enough
                    try
                    {
                        drivers.Add(lineElements[1]);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error occurred writing driver: " + e);
                    }
                }
                else if(lineElements[0].Equals("Trip"))
                {
                    //try-catch to make sure file is formatted correctly enough
                    try { 
                    //these are the 4 elements that follow a 'trip' command
                    trips.Add(lineElements[1]);
                    string[] lists = lineElements[2].Split(":");
                    if (lists[0].Substring(0, 1).Equals("0"))
                    {
                        lists[0] = lists[0].Substring(1);
                    }
                    trips.Add(lists[0]);
                    trips.Add(lists[1]);
                    lists = lineElements[3].Split(":");
                    if(lists[0].Substring(0,1).Equals("0"))
                        {
                            lists[0] = lists[0].Substring(1);
                        }
                    trips.Add(lists[0]);
                    trips.Add(lists[1]);  
                    trips.Add(lineElements[4]);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error occurred writing trip data: "+ e);
                    }

                }
                else 
                {
                    Console.WriteLine("\t Invalid command");
                }
            }
            //Now, we have all the data saved from the input, time to process the data for output.
            trips.TrimToSize();
            drivers.TrimToSize();
            string currDriver = "", parser = "", parser3 = "";
            int minutes = 0, parser2 =0;
            ArrayList data = new ArrayList();
            for (int i=0; i < trips.Capacity; i++)
            {
                //assume driver name
                if (i % 6 == 0)
                {
                    //to ensure a driver from our list is only chosen
                    for (int j = 0; j < drivers.Capacity; j++)
                    if (trips[i].Equals(drivers[j]))
                    {
                        currDriver = (string)drivers[j];
                    }
                }
                //assume start time, hour
                if (i % 6 == 1)
                {
                    parser = (string)trips[i];
                    minutes -= int.Parse(parser) * 60;
                }
                //assume start time, minute
                if (i % 6 == 2)
                {
                    parser = (string)trips[i];
                    minutes -= int.Parse(parser);
                }
                //assume stop time, hour
                if (i % 6 == 3)
                {
                    parser = (string)trips[i];
                    minutes += int.Parse(parser) * 60;
                }
                //assume stop time, minute
                if (i % 6 == 4)
                {
                    parser = (string)trips[i];
                    minutes += int.Parse(parser);                    
                }
                //assume distance traveled
                if (i % 6 == 5)
                {
                    //check if below or above threshold before adding to records
                    double distance = 0, time;
                    parser = (string)trips[i];
                    distance += double.Parse(parser)*60;
                    time = (double)minutes ;
                    if (distance / time > 5 && distance / time < 100)
                    {
                        if (data.Contains(currDriver))
                        {
                            //add minutes to existing driver
                            int k = data.IndexOf(currDriver) + 1;
                            parser2 = (int)data[k];
                            int sumMins = 0;
                            sumMins += minutes + parser2;
                            data[k] = sumMins;
                            //add miles to existing driver
                            k = data.IndexOf(currDriver) + 2;
                            parser = (string)trips[i];
                            parser3 = (string)data[k];
                            double sum = 0;
                            sum += double.Parse(parser) + double.Parse(parser3);
                            data[k] = sum;
                            minutes = 0;
                        }
                        else
                        {
                            data.Add(currDriver);
                            data.Add(minutes);
                            minutes = 0;
                            data.Add(trips[i]);
                        }
                    }
                }
            }
            //Now, we sum all the drivers to reduce redundancy
            data.TrimToSize();
            ArrayList finalSet = new ArrayList();
            double most = 0, temp = 0 ;
            int index = -1;
            bool sorted = false;
            while(!sorted)
            {
                //The miles driven sorter
                data.TrimToSize();
                if (data.Capacity<=3)
                {
                    sorted = true;
                }
                for (int j = 2; j < data.Capacity; j += 3)
                {
                    parser = data[j].ToString();
                    temp = double.Parse(parser);
                    if (most < temp)
                    {
                        most = temp;
                        index = j;
                        j= 2;
                    }
                    if (j == (data.Capacity-1))
                    {
                        finalSet.Add(data[index - 2]);
                        finalSet.Add(data[index - 1]);
                        finalSet.Add(data[index]);
                        data.RemoveAt(index - 2);
                        data.RemoveAt(index - 2);
                        data.RemoveAt(index - 2);
                        most = 0;
                        j = 2;
                        break;
                    }
                }
            }            
            //Convert to mph
            finalSet.TrimToSize();
            for (int i = 1; i < finalSet.Capacity; i+=3)
            {
                double distance = 0, time = 0, mph=0;
                parser = finalSet[i+1].ToString();
                distance += double.Parse(parser)*60;
                parser3 = finalSet[i].ToString();
                time = double.Parse(parser3);
                mph = distance / time;
                //Round to nearest whole number
                finalSet[i] = Math.Round(mph,0);
                parser = finalSet[i + 1].ToString();
                distance = double.Parse(parser);
                finalSet[i+1] = Math.Round(distance, 0);
            }
            //In case someone is instantiated as driver, but never mentioned
            for (int i = 0; i < drivers.Capacity; i++)
            {
                if (!finalSet.Contains(drivers[i]))
                {
                    finalSet.Add(drivers[i]);
                    finalSet.Add(0);
                    finalSet.Add(0);
                }
            }
            finalSet.TrimToSize();
            for(int i=0; i< finalSet.Capacity; i++)
            {
                Console.WriteLine("final set: "+ finalSet[i].ToString());
            }
            //limits array range to number of elements contained (i.e. the numebr of drivers in the input file) 
            return Enumerable.Range(0, drivers.Capacity).Select(index => new Drivers
            {
                Driver = finalSet[drivers.Capacity*index].ToString(),
                MPH = finalSet[drivers.Capacity * index+1].ToString(),
                Miles = finalSet[drivers.Capacity * index + 2].ToString(),
            })
            .ToArray();
        }
    }
}
