using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
namespace UtahCarSafety.Models
{
    public class Crash
    {
        [Required]
        [Key]
        public int CRASH_ID { get; set; }
        [Required]
        public string ROUTE { get; set; }
        [Required]
        public float MILEPOINT { get; set; }
        [Required]
        public float LAT_UTM_Y { get; set; }
        [Required]
        public float LONG_UTM_X { get; set; }
        public string MAIN_ROAD_NAME { get; set; }
        public string CITY { get; set; }
        public string COUNTY_NAME { get; set; }
        [Required]
        public int CRASH_SEVERITY_ID { get; set; }
        [Required]
        public bool WORK_ZONE_RELATED { get; set; } = false;
        [Required]
        public bool PEDESTRIAN_INVOLVED { get; set; } = false;
        [Required]
        public bool BICYCLIST_INVOLVED { get; set; } = false;
        [Required]
        public bool MOTORCYCLE_INVOLVED { get; set; } = false;
        [Required]
        public bool IMPROPER_RESTRAINT { get; set; } = false;
        [Required]
        public bool UNRESTRAINED { get; set; } = false;
        [Required]
        public bool DUI { get; set; } = false;
        [Required]
        public bool INTERSECTION_RELATED { get; set; } = false;
        [Required]
        public bool WILD_ANIMAL_RELATED { get; set; } = false;
        [Required]
        public bool DOMESTIC_ANIMAL_RELATED { get; set; } = false;
        [Required]
        public bool OVERTURN_ROLLOVER { get; set; } = false;
        [Required]
        public bool COMMERCIAL_MOTOR_VEH_INVOLVED { get; set; } = false;
        [Required]
        public bool TEENAGE_DRIVER_INVOLVED { get; set; } = false;
        [Required]
        public bool OLDER_DRIVER_INVOLVED { get; set; } = false;
        [Required]
        public bool NIGHT_DARK_CONDITION { get; set; } = false;
        [Required]
        public bool SINGLE_VEHICLE { get; set; } = false;
        [Required]
        public bool DISTRACTED_DRIVING { get; set; } = false;
        [Required]
        public bool DROWSY_DRIVING { get; set; } = false;
        [Required]
        public bool ROADWAY_DEPARTURE { get; set; } = false;
        [Required]
        public DateTime CRASH_DATETIME { get; set; }


        public string SeverityOutput()
        {
            if (CRASH_SEVERITY_ID == 1)
            {
                return ("1 - No Injury");
            }
            else if (CRASH_SEVERITY_ID == 2)
            {
                return ("2 - Possible Injury");
            }
            else if (CRASH_SEVERITY_ID == 3)
            {
                return ("3 - Suspected Minor Injury");
            }
            else if (CRASH_SEVERITY_ID == 4)
            {
                return ("4 - Suspected Serious Injury");
            }
            else
            {
                return ("5 - Fatal");
            }
        }



        public string WhatBoolean()
        {
            string output = "";
            if (WORK_ZONE_RELATED == true)
            {
                output += "Work-Zone Related, ";
            }
            if (PEDESTRIAN_INVOLVED == true)
            {
                output += "Pedestrian Involved, ";
            }
            if (BICYCLIST_INVOLVED == true)
            {
                output += "Bicyclist Involved, ";
            }
            if (MOTORCYCLE_INVOLVED == true)
            {
                output += "Motorcycle Involved, ";
            }
            if (IMPROPER_RESTRAINT == true)
            {
                output += "Improper Restraint, ";
            }
            if (UNRESTRAINED == true)
            {
                output += "Unrestrained, ";
            }
            if (DUI == true)
            {
                output += "Driving Under the Influence, ";
            }
            if (INTERSECTION_RELATED == true)
            {
                output += "Intersection Related, ";
            }
            if (WILD_ANIMAL_RELATED == true)
            {
                output += "Wild Animal Related, ";
            }
            if (DOMESTIC_ANIMAL_RELATED == true)
            {
                output += "Domestic Animal Related, ";
            }
            if (OVERTURN_ROLLOVER == true)
            {
                output += "Overturn/Rollover, ";
            }
            if (COMMERCIAL_MOTOR_VEH_INVOLVED == true)
            {
                output += "Commercial Vehicle Involved, ";
            }
            if (TEENAGE_DRIVER_INVOLVED == true)
            {
                output += "Teenage Driver Involved, ";
            }
            if (OLDER_DRIVER_INVOLVED == true)
            {
                output += "Older Driver Involved, ";
            }
            if (NIGHT_DARK_CONDITION == true)
            {
                output += "Night/Dark Condition, ";
            }
            if (SINGLE_VEHICLE == true)
            {
                output += "Single Vehicle, ";
            }
            if (DISTRACTED_DRIVING == true)
            {
                output += "Distracted Driving, ";
            }
            if (DROWSY_DRIVING == true)
            {
                output += "Drowsy Driving, ";
            }
            if (ROADWAY_DEPARTURE == true)
            {
                output += "Roadway Departure, ";
            }
            if (output.Length > 0)
            {
                output = output.Substring(0, output.Length - 2);
            }
            return output;
        }
    }
}