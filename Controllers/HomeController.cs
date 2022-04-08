using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using UtahCarSafety.Models;
using UtahCarSafety.Models.ViewModels;

namespace UtahCarSafety.Controllers
{
    public class HomeController : Controller
    {

        private ICrashesRepository repo { get; set; }

        private UserManager<IdentityUser> userManager;
        private SignInManager<IdentityUser> signInManager;
        private InferenceSession _session;

        //Constructor
        public HomeController(UserManager<IdentityUser> um, SignInManager<IdentityUser> sim, ICrashesRepository temp, InferenceSession session)
        {
            userManager = um;
            signInManager = sim;
            repo = temp;

            _session = session;
        }

        private bool? CheckBoolean(int input)
        {
            if (input == 1)
            {
                return (true);
            }
            else if (input == 0)
            {
                return (false);
            }
            else
            {
                return (null);
            }
        }

        private int getNumWithFilters(string city, int? severity, string county, int? year, int? month, int? hour, bool? boolTeen,
        bool? boolWork, bool? boolPed, bool? boolBike, bool? boolMotor, bool? boolImproper, bool? boolUnrest,
        bool? boolDUI, bool? boolInter, bool? boolWild, bool? boolDom, bool? boolOver, bool? boolComm,
        bool? boolOld, bool? boolNight, bool? boolSingle, bool? boolDist, bool? boolDrowsy, bool? boolRoadway)
        {
            var crashes = repo.Crashes;
            if (city != null)
            {
                crashes = crashes.Where(x => x.CITY == city);
            }
            if (severity != null)
            {
                crashes = crashes.Where(x => x.CRASH_SEVERITY_ID == severity);
            }
            if (county != null)
            {
                crashes = crashes.Where(x => x.COUNTY_NAME == county);
            }
            if (year != null)
            {
                crashes = crashes.Where(x => x.CRASH_DATETIME.Year == year);
            }
            if (month != null)
            {
                crashes = crashes.Where(x => x.CRASH_DATETIME.Month == month);
            }
            if (hour != null)
            {
                crashes = crashes.Where(x => x.CRASH_DATETIME.Hour == hour);
            }
            if (boolWork != null)
            {
                crashes = crashes.Where(x => x.WORK_ZONE_RELATED == boolWork);
            }
            if (boolPed != null)
            {
                crashes = crashes.Where(x => x.PEDESTRIAN_INVOLVED == boolPed);
            }
            if (boolBike != null)
            {
                crashes = crashes.Where(x => x.BICYCLIST_INVOLVED == boolBike);
            }
            if (boolMotor != null)
            {
                crashes = crashes.Where(x => x.MOTORCYCLE_INVOLVED == boolMotor);
            }
            if (boolImproper != null)
            {
                crashes = crashes.Where(x => x.IMPROPER_RESTRAINT == boolImproper);
            }
            if (boolUnrest != null)
            {
                crashes = crashes.Where(x => x.UNRESTRAINED == boolUnrest);
            }
            if (boolDUI != null)
            {
                crashes = crashes.Where(x => x.DUI == boolDUI);
            }
            if (boolInter != null)
            {
                crashes = crashes.Where(x => x.INTERSECTION_RELATED == boolInter);
            }
            if (boolWild != null)
            {
                crashes = crashes.Where(x => x.WILD_ANIMAL_RELATED == boolWild);
            }
            if (boolDom != null)
            {
                crashes = crashes.Where(x => x.DOMESTIC_ANIMAL_RELATED == boolDom);
            }
            if (boolOver != null)
            {
                crashes = crashes.Where(x => x.OVERTURN_ROLLOVER == boolOver);
            }
            if (boolComm != null)
            {
                crashes = crashes.Where(x => x.COMMERCIAL_MOTOR_VEH_INVOLVED == boolComm);
            }
            if (boolTeen != null)
            {
                crashes = crashes.Where(x => x.TEENAGE_DRIVER_INVOLVED == boolTeen);
            }
            if (boolOld != null)
            {
                crashes = crashes.Where(x => x.OLDER_DRIVER_INVOLVED == boolOld);
            }
            if (boolNight != null)
            {
                crashes = crashes.Where(x => x.NIGHT_DARK_CONDITION == boolNight);
            }
            if (boolSingle != null)
            {
                crashes = crashes.Where(x => x.SINGLE_VEHICLE == boolSingle);
            }
            if (boolDist != null)
            {
                crashes = crashes.Where(x => x.DISTRACTED_DRIVING == boolDist);
            }
            if (boolDrowsy != null)
            {
                crashes = crashes.Where(x => x.DROWSY_DRIVING == boolDrowsy);
            }
            if (boolRoadway != null)
            {
                crashes = crashes.Where(x => x.ROADWAY_DEPARTURE == boolRoadway);
            }
            return crashes.Count();
        }

        private string ActiveFilters(string city, int? severity, string county, int? year, int? month, int? hour, bool? boolTeen,
        bool? boolWork, bool? boolPed, bool? boolBike, bool? boolMotor, bool? boolImproper, bool? boolUnrest,
        bool? boolDUI, bool? boolInter, bool? boolWild, bool? boolDom, bool? boolOver, bool? boolComm,
        bool? boolOld, bool? boolNight, bool? boolSingle, bool? boolDist, bool? boolDrowsy, bool? boolRoadway)
        {
            string output = "";
            if (city != null)
            {
                output += "City = " + city + " | ";
            }
            if (county != null)
            {
                output += "County = " + county + " | ";
            }
            if (severity != null)
            {
                output += "Severity = " + severity.ToString() + " | ";
            }
            if (year != null)
            {
                output += "Year = " + year.ToString() + " | ";
            }
            if (month != null)
            {
                output += "Month = " + month.ToString() + " | ";
            }
            if (hour != null)
            {
                output += "Hour = " + hour.ToString() + " | ";
            }
            if (boolTeen == true)
            {
                output += "Teenager Involved | ";
            }
            else if (boolTeen == false)
            {
                output += "No Teenager Involved | ";
            }
            if (boolWork == true)
            {
                output += "Work-Zone Related | ";
            }
            else if (boolWork == false)
            {
                output += "Not Work-Zone Related | ";
            }
            if (boolPed == true)
            {
                output += "Pedestrian Involved | ";
            }
            else if (boolPed == false)
            {
                output += "No Pedestrian Involved | ";
            }
            if (boolBike == true)
            {
                output += "Bicyclist Involved | ";
            }
            else if (boolBike == false)
            {
                output += "No Bicyclist Involved | ";
            }
            if (boolMotor == true)
            {
                output += "Motorcycle Involved | ";
            }
            else if (boolMotor == false)
            {
                output += "No Motorcycle Involved | ";
            }
            if (boolImproper == true)
            {
                output += "Improper Restraints Used | ";
            }
            else if (boolImproper == false)
            {
                output += "No Improper Restraints | ";
            }
            if (boolUnrest == true)
            {
                output += "Unrestrained Person Involved | ";
            }
            else if (boolUnrest == false)
            {
                output += "No Unrestrained People Involved | ";
            }
            if (boolDUI == true)
            {
                output += "Driving Under the Influence | ";
            }
            else if (boolDUI == false)
            {
                output += "No Driving Under the Influence | ";
            }
            if (boolInter == true)
            {
                output += "Intersection Related | ";
            }
            else if (boolInter == false)
            {
                output += "Not Intersection Related | ";
            }
            if (boolWild == true)
            {
                output += "Wild Animal Involved | ";
            }
            else if (boolWild == false)
            {
                output += "No Wild Animal Involved | ";
            }
            if (boolDom == true)
            {
                output += "Domestic Animal Involved | ";
            }
            else if (boolDom == false)
            {
                output += "No Domestic Animal Involved | ";
            }
            if (boolOver == true)
            {
                output += "Overturn/Rollover | ";
            }
            else if (boolOver == false)
            {
                output += "No Overturn/Rollover | ";
            }
            if (boolOld == true)
            {
                output += "Older Driver Involved | ";
            }
            else if (boolOld == false)
            {
                output += "No Older Driver Involved | ";
            }
            if (boolComm == true)
            {
                output += "Commercial Vehicle Involved | ";
            }
            else if (boolComm == false)
            {
                output += "No Commercial Vehicle Involved | ";
            }
            if (boolNight == true)
            {
                output += "Night/Dark Conditions | ";
            }
            else if (boolNight == false)
            {
                output += "No Night/Dark Conditions | ";
            }
            if (boolSingle == true)
            {
                output += "Single Vehicle | ";
            }
            else if (boolSingle == false)
            {
                output += "Multiple Vehicles | ";
            }
            if (boolDist == true)
            {
                output += "Distracted Driver Involved | ";
            }
            else if (boolDist == false)
            {
                output += "No Distracted Driver Involved | ";
            }
            if (boolDrowsy == true)
            {
                output += "Drowsy Driver Involved | ";
            }
            else if (boolDrowsy == false)
            {
                output += "No Drowsy Driver Involved | ";
            }
            if (boolRoadway == true)
            {
                output += "Roadway Departure | ";
            }
            else if (boolRoadway == false)
            {
                output += "No Roadway Departure | ";
            }
            if (output != "")
            {
                output = output.Substring(0, output.Length - 2);
            }
            else
            {
                output = "";
            }
            return (output);
        }

        public async Task<RedirectResult> Logout(string returnUrl = "/")
        {
            await signInManager.SignOutAsync();

            return Redirect(returnUrl);
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [Authorize]
        public IActionResult Filters()
        {
            ViewBag.Cities = repo.Crashes.Select(x => x.CITY).Distinct().OrderBy(x => x);
            ViewBag.Severities = repo.Crashes.Select(x => x.CRASH_SEVERITY_ID).Distinct().OrderBy(x => x);
            ViewBag.Counties = repo.Crashes.Select(x => x.COUNTY_NAME).Distinct().OrderBy(x => x);
            ViewBag.Years = new List<int>() { 2016, 2017, 2018, 2019 };
            ViewBag.Months = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 }; ;
            ViewBag.Hours = repo.Crashes.Select(x => x.CRASH_DATETIME.Hour).Distinct().OrderBy(x => x);
            return View();
        }

        public IActionResult FiltersGuest()
        {
            ViewBag.Cities = repo.Crashes.Select(x => x.CITY).Distinct().OrderBy(x => x);
            ViewBag.Severities = repo.Crashes.Select(x => x.CRASH_SEVERITY_ID).Distinct().OrderBy(x => x);
            ViewBag.Counties = repo.Crashes.Select(x => x.COUNTY_NAME).Distinct().OrderBy(x => x);
            ViewBag.Years = new List<int>() { 2016, 2017, 2018, 2019 };
            ViewBag.Months = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 }; ;
            ViewBag.Hours = repo.Crashes.Select(x => x.CRASH_DATETIME.Hour).Distinct().OrderBy(x => x);
            return View();
        }

        [Authorize]
        public IActionResult Dataset(string CITY, string county, int? severity, int? year, int? month, int? hour, string road,
            int teen = 2, int work = 2, int ped = 2, int bike = 2, int motor = 2, int improper = 2, int unrest = 2, int dui = 2,
            int inter = 2, int wild = 2, int dom = 2, int over = 2, int comm = 2, int old = 2, int night = 2, int single = 2,
            int dist = 2, int drowsy = 2, int roadway = 2, int pageNum = 1)
        {
            int numResults = 50;
            ViewBag.Cities = repo.Crashes.Select(x => x.CITY).Distinct().OrderBy(x => x);
            ViewBag.Severities = repo.Crashes.Select(x => x.CRASH_SEVERITY_ID).Distinct().OrderBy(x => x);
            ViewBag.Counties = repo.Crashes.Select(x => x.COUNTY_NAME).Distinct().OrderBy(x => x);
            ViewBag.Years = new List<int>() { 2016, 2017, 2018, 2019 };
            bool? boolTeen = CheckBoolean(teen);
            bool? boolWork = CheckBoolean(work);
            bool? boolPed = CheckBoolean(ped);
            bool? boolBike = CheckBoolean(bike);
            bool? boolMotor = CheckBoolean(motor);
            bool? boolImproper = CheckBoolean(improper);
            bool? boolUnrest = CheckBoolean(unrest);
            bool? boolDUI = CheckBoolean(dui);
            bool? boolInter = CheckBoolean(inter);
            bool? boolWild = CheckBoolean(wild);
            bool? boolDom = CheckBoolean(dom);
            bool? boolOver = CheckBoolean(over);
            bool? boolComm = CheckBoolean(comm);
            bool? boolOld = CheckBoolean(old);
            bool? boolNight = CheckBoolean(night);
            bool? boolSingle = CheckBoolean(single);
            bool? boolDist = CheckBoolean(dist);
            bool? boolDrowsy = CheckBoolean(drowsy);
            bool? boolRoadway = CheckBoolean(roadway);
            if (year == 0)
            {
                year = null;
            }
            if (severity == 0)
            {
                severity = null;
            }
            if (month == 0)
            {
                month = null;
            }
            if (hour == 30)
            {
                hour = null;
            }
            if (road != null)
            {
                road = road.ToUpper();
            }
            var c = new CrashViewModel
            {
                Crashes = repo.Crashes
                .Where(c => c.CITY == CITY || CITY == null)
                .Where(c => c.CRASH_SEVERITY_ID == severity || severity == null)
                .Where(c => c.COUNTY_NAME == county || county == null)
                .Where(c => c.CRASH_DATETIME.Year == year || year == null)
                .Where(c => c.CRASH_DATETIME.Month == month || month == null)
                .Where(c => c.CRASH_DATETIME.Hour == hour || hour == null)
                .Where(c => c.MAIN_ROAD_NAME == road || road == null)
                .Where(c => c.WORK_ZONE_RELATED == boolWork || boolWork == null)
                .Where(c => c.PEDESTRIAN_INVOLVED == boolPed || boolPed == null)
                .Where(c => c.BICYCLIST_INVOLVED == boolBike || boolBike == null)
                .Where(c => c.MOTORCYCLE_INVOLVED == boolMotor || boolMotor == null)
                .Where(c => c.IMPROPER_RESTRAINT == boolImproper || boolImproper == null)
                .Where(c => c.UNRESTRAINED == boolUnrest || boolUnrest == null)
                .Where(c => c.DUI == boolDUI || boolDUI == null)
                .Where(c => c.INTERSECTION_RELATED == boolInter || boolInter == null)
                .Where(c => c.WILD_ANIMAL_RELATED == boolWild || boolWild == null)
                .Where(c => c.DOMESTIC_ANIMAL_RELATED == boolDom || boolDom == null)
                .Where(c => c.OVERTURN_ROLLOVER == boolOver || boolOver == null)
                .Where(c => c.COMMERCIAL_MOTOR_VEH_INVOLVED == boolComm || boolComm == null)
                .Where(c => c.TEENAGE_DRIVER_INVOLVED == boolTeen || boolTeen == null)
                .Where(c => c.OLDER_DRIVER_INVOLVED == boolOld || boolOld == null)
                .Where(c => c.NIGHT_DARK_CONDITION == boolNight || boolNight == null)
                .Where(c => c.SINGLE_VEHICLE == boolSingle || boolSingle == null)
                .Where(c => c.DISTRACTED_DRIVING == boolDist || boolDist == null)
                .Where(c => c.DROWSY_DRIVING == boolDrowsy || boolDrowsy == null)
                .Where(c => c.ROADWAY_DEPARTURE == boolRoadway || boolRoadway == null)
                .OrderByDescending(c => c.CRASH_DATETIME)
                .Skip((pageNum - 1) * numResults)
                .Take(numResults),
                CurrentCity = CITY,
                CurrentSeverity = severity,
                CurrentCounty = county,
                CurrentYear = year,
                CurrentMonth = month,
                CurrentHour = hour,
                CurrentRoad = road,
                CurrentTeenage = teen,
                CurrentWorkZone = work,
                CurrentPedestrian = ped,
                CurrentBicyclist = bike,
                CurrentMotorcycle = motor,
                CurrentImproper = improper,
                CurrentUnrestrained = unrest,
                CurrentDUI = dui,
                CurrentIntersection = inter,
                CurrentWild = wild,
                CurrentDomestic = dom,
                CurrentOverturn = over,
                CurrentCommercial = comm,
                CurrentOlder = old,
                CurrentNight = night,
                CurrentSingle = single,
                CurrentDistracted = dist,
                CurrentDrowsy = drowsy,
                CurrentRoadway = roadway,
                PageInfo = new PageInfo
                {
                    TotalNumRecords = getNumWithFilters(CITY, severity, county, year, month, hour, boolTeen,
                        boolWork, boolPed, boolBike, boolMotor, boolImproper, boolUnrest,
                        boolDUI, boolInter, boolWild, boolDom, boolOver, boolComm,
                        boolOld, boolNight, boolSingle, boolDist, boolDrowsy, boolRoadway),
                    RecordsPerPage = numResults,
                    CurrentPage = pageNum,
                }
            };
            //ViewData["TeamName"] = teamName;
            ViewBag.Records = c.PageInfo.TotalNumRecords;
            ViewBag.ActiveFilters = ActiveFilters(CITY, severity, county, year, month, hour, boolTeen,
                        boolWork, boolPed, boolBike, boolMotor, boolImproper, boolUnrest,
                        boolDUI, boolInter, boolWild, boolDom, boolOver, boolComm,
                        boolOld, boolNight, boolSingle, boolDist, boolDrowsy, boolRoadway);
            return View(c);
        }

        public IActionResult DatasetGuest(string CITY, string county, int? severity, int? year, int? month, int? hour, string road,
            int teen = 2, int work = 2, int ped = 2, int bike = 2, int motor = 2, int improper = 2, int unrest = 2, int dui = 2,
            int inter = 2, int wild = 2, int dom = 2, int over = 2, int comm = 2, int old = 2, int night = 2, int single = 2,
            int dist = 2, int drowsy = 2, int roadway = 2, int pageNum = 1)
        {
            int numResults = 50;
            ViewBag.Cities = repo.Crashes.Select(x => x.CITY).Distinct().OrderBy(x => x);
            ViewBag.Severities = repo.Crashes.Select(x => x.CRASH_SEVERITY_ID).Distinct().OrderBy(x => x);
            ViewBag.Counties = repo.Crashes.Select(x => x.COUNTY_NAME).Distinct().OrderBy(x => x);
            ViewBag.Years = new List<int>() { 2016, 2017, 2018, 2019 };
            bool? boolTeen = CheckBoolean(teen);
            bool? boolWork = CheckBoolean(work);
            bool? boolPed = CheckBoolean(ped);
            bool? boolBike = CheckBoolean(bike);
            bool? boolMotor = CheckBoolean(motor);
            bool? boolImproper = CheckBoolean(improper);
            bool? boolUnrest = CheckBoolean(unrest);
            bool? boolDUI = CheckBoolean(dui);
            bool? boolInter = CheckBoolean(inter);
            bool? boolWild = CheckBoolean(wild);
            bool? boolDom = CheckBoolean(dom);
            bool? boolOver = CheckBoolean(over);
            bool? boolComm = CheckBoolean(comm);
            bool? boolOld = CheckBoolean(old);
            bool? boolNight = CheckBoolean(night);
            bool? boolSingle = CheckBoolean(single);
            bool? boolDist = CheckBoolean(dist);
            bool? boolDrowsy = CheckBoolean(drowsy);
            bool? boolRoadway = CheckBoolean(roadway);
            if (year == 0)
            {
                year = null;
            }
            if (severity == 0)
            {
                severity = null;
            }
            if (month == 0)
            {
                month = null;
            }
            if (hour == 30)
            {
                hour = null;
            }
            if (road != null)
            {
                road = road.ToUpper();
            }
            var c = new CrashViewModel
            {
                Crashes = repo.Crashes
                .Where(c => c.CITY == CITY || CITY == null)
                .Where(c => c.CRASH_SEVERITY_ID == severity || severity == null)
                .Where(c => c.COUNTY_NAME == county || county == null)
                .Where(c => c.CRASH_DATETIME.Year == year || year == null)
                .Where(c => c.CRASH_DATETIME.Month == month || month == null)
                .Where(c => c.CRASH_DATETIME.Hour == hour || hour == null)
                .Where(c => c.MAIN_ROAD_NAME == road || road == null)
                .Where(c => c.WORK_ZONE_RELATED == boolWork || boolWork == null)
                .Where(c => c.PEDESTRIAN_INVOLVED == boolPed || boolPed == null)
                .Where(c => c.BICYCLIST_INVOLVED == boolBike || boolBike == null)
                .Where(c => c.MOTORCYCLE_INVOLVED == boolMotor || boolMotor == null)
                .Where(c => c.IMPROPER_RESTRAINT == boolImproper || boolImproper == null)
                .Where(c => c.UNRESTRAINED == boolUnrest || boolUnrest == null)
                .Where(c => c.DUI == boolDUI || boolDUI == null)
                .Where(c => c.INTERSECTION_RELATED == boolInter || boolInter == null)
                .Where(c => c.WILD_ANIMAL_RELATED == boolWild || boolWild == null)
                .Where(c => c.DOMESTIC_ANIMAL_RELATED == boolDom || boolDom == null)
                .Where(c => c.OVERTURN_ROLLOVER == boolOver || boolOver == null)
                .Where(c => c.COMMERCIAL_MOTOR_VEH_INVOLVED == boolComm || boolComm == null)
                .Where(c => c.TEENAGE_DRIVER_INVOLVED == boolTeen || boolTeen == null)
                .Where(c => c.OLDER_DRIVER_INVOLVED == boolOld || boolOld == null)
                .Where(c => c.NIGHT_DARK_CONDITION == boolNight || boolNight == null)
                .Where(c => c.SINGLE_VEHICLE == boolSingle || boolSingle == null)
                .Where(c => c.DISTRACTED_DRIVING == boolDist || boolDist == null)
                .Where(c => c.DROWSY_DRIVING == boolDrowsy || boolDrowsy == null)
                .Where(c => c.ROADWAY_DEPARTURE == boolRoadway || boolRoadway == null)
                .OrderByDescending(c => c.CRASH_DATETIME)
                .Skip((pageNum - 1) * numResults)
                .Take(numResults),
                CurrentCity = CITY,
                CurrentSeverity = severity,
                CurrentCounty = county,
                CurrentYear = year,
                CurrentMonth = month,
                CurrentHour = hour,
                CurrentRoad = road,
                CurrentTeenage = teen,
                CurrentWorkZone = work,
                CurrentPedestrian = ped,
                CurrentBicyclist = bike,
                CurrentMotorcycle = motor,
                CurrentImproper = improper,
                CurrentUnrestrained = unrest,
                CurrentDUI = dui,
                CurrentIntersection = inter,
                CurrentWild = wild,
                CurrentDomestic = dom,
                CurrentOverturn = over,
                CurrentCommercial = comm,
                CurrentOlder = old,
                CurrentNight = night,
                CurrentSingle = single,
                CurrentDistracted = dist,
                CurrentDrowsy = drowsy,
                CurrentRoadway = roadway,
                PageInfo = new PageInfo
                {
                    TotalNumRecords = getNumWithFilters(CITY, severity, county, year, month, hour, boolTeen,
                        boolWork, boolPed, boolBike, boolMotor, boolImproper, boolUnrest,
                        boolDUI, boolInter, boolWild, boolDom, boolOver, boolComm,
                        boolOld, boolNight, boolSingle, boolDist, boolDrowsy, boolRoadway),
                    RecordsPerPage = numResults,
                    CurrentPage = pageNum,
                }
            };
            //ViewData["TeamName"] = teamName;
            ViewBag.Records = c.PageInfo.TotalNumRecords;
            ViewBag.ActiveFilters = ActiveFilters(CITY, severity, county, year, month, hour, boolTeen,
                        boolWork, boolPed, boolBike, boolMotor, boolImproper, boolUnrest,
                        boolDUI, boolInter, boolWild, boolDom, boolOver, boolComm,
                        boolOld, boolNight, boolSingle, boolDist, boolDrowsy, boolRoadway);
            return View(c);
        }

        [HttpGet]
        public IActionResult CrashDetail(int crashid)
        {
            var crashdetail = repo.Crashes.Single(x => x.CRASH_ID == crashid);
            CrashData cd = new CrashData();
            cd.CITY_OGDEN = 0f;
            cd.CITY_PROVO = 0f;
            cd.CITY_WEST_JORDAN = 0f;
            cd.PEDESTRIAN_INVOLVED_True = 0f;
            cd.BICYCLIST_INVOLVED_True = 0f;
            cd.MOTORCYCLE_INVOLVED_True = 0f;
            cd.IMPROPER_RESTRAINT_True = 0f;
            cd.UNRESTRAINED_True = 0f;
            cd.DUI_True = 0f;
            cd.INTERSECTION_RELATED_True = 0f;
            cd.OVERTURN_ROLLOVER_True = 0f;
            cd.SINGLE_VEHICLE_True = 0f;
            cd.DISTRACTED_DRIVING_True = 0f;
            cd.DROWSY_DRIVING_True = 0f;
            cd.ROADWAY_DEPARTURE_True = 0f;
            if (crashdetail.CITY == "PROVO")
            {
                cd.CITY_PROVO = 1f;
            }
            else if (crashdetail.CITY == "OGDEN")
            {
                cd.CITY_OGDEN = 1f;
            }
            else if (crashdetail.CITY == "WEST JORDAN")
            {
                cd.CITY_WEST_JORDAN = 1f;
            }
            if (crashdetail.PEDESTRIAN_INVOLVED == true)
            {
                cd.PEDESTRIAN_INVOLVED_True = 1f;
            }
            if (crashdetail.BICYCLIST_INVOLVED == true)
            {
                cd.BICYCLIST_INVOLVED_True = 1f;
            }
            if (crashdetail.MOTORCYCLE_INVOLVED == true)
            {
                cd.MOTORCYCLE_INVOLVED_True = 1f;
            }
            if (crashdetail.IMPROPER_RESTRAINT == true)
            {
                cd.IMPROPER_RESTRAINT_True = 1f;
            }
            if (crashdetail.UNRESTRAINED == true)
            {
                cd.UNRESTRAINED_True = 1f;
            }
            if (crashdetail.DUI == true)
            {
                cd.DUI_True = 1f;
            }
            if (crashdetail.INTERSECTION_RELATED == true)
            {
                cd.INTERSECTION_RELATED_True = 1f;
            }
            if (crashdetail.OVERTURN_ROLLOVER == true)
            {
                cd.OVERTURN_ROLLOVER_True = 1f;
            }
            if (crashdetail.SINGLE_VEHICLE == true)
            {
                cd.SINGLE_VEHICLE_True = 1f;
            }
            if (crashdetail.DISTRACTED_DRIVING == true)
            {
                cd.DISTRACTED_DRIVING_True = 1f;
            }
            if (crashdetail.DROWSY_DRIVING == true)
            {
                cd.DROWSY_DRIVING_True = 1f;
            }
            if (crashdetail.ROADWAY_DEPARTURE == true)
            {
                cd.ROADWAY_DEPARTURE_True = 1f;
            }
            var result = _session.Run(new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("float_input", cd.AsTensor())
            });
            Tensor<float> score = result.First().AsTensor<float>();
            var prediction = new Prediction { PredictedValue = score.First() };
            ViewBag.Prediction = prediction.RoundedValue();
            result.Dispose();
            ViewBag.prediction2 = "";


            if (cd.IMPROPER_RESTRAINT_True == 1)
            {
                cd.IMPROPER_RESTRAINT_True = 0;
                result = _session.Run(new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("float_input", cd.AsTensor())
            });
                score = result.First().AsTensor<float>();
                prediction = new Prediction { PredictedValue = score.First() };
                string pred2 = prediction.RoundedValue();
                result.Dispose();
                ViewBag.prediction2 = "If everyone in this accident wore their seatbelt properly, the severity of the crash would've been " + pred2;
            }
            else if (cd.BICYCLIST_INVOLVED_True == 1)
            {
                cd.BICYCLIST_INVOLVED_True = 0;
                result = _session.Run(new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("float_input", cd.AsTensor())
            });
                score = result.First().AsTensor<float>();
                prediction = new Prediction { PredictedValue = score.First() };
                string pred2 = prediction.RoundedValue();
                result.Dispose();
                ViewBag.prediction2 = "If this driver was more cautious of bicyclists, the severity of the crash would've been " + pred2;
            }
            else if (cd.PEDESTRIAN_INVOLVED_True == 1)
            {
                cd.PEDESTRIAN_INVOLVED_True = 0;
                result = _session.Run(new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("float_input", cd.AsTensor())
            });
                score = result.First().AsTensor<float>();
                prediction = new Prediction { PredictedValue = score.First() };
                string pred2 = prediction.RoundedValue();
                result.Dispose();
                ViewBag.prediction2 = "If this driver was more cautious of pedestrians, the severity of the crash would've been " + pred2;
            }
            else if (cd.INTERSECTION_RELATED_True == 1)
            {
                cd.INTERSECTION_RELATED_True = 0;
                result = _session.Run(new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("float_input", cd.AsTensor())
            });
                score = result.First().AsTensor<float>();
                prediction = new Prediction { PredictedValue = score.First() };
                string pred2 = prediction.RoundedValue();
                result.Dispose();
                ViewBag.prediction2 = "If this driver was more cautious at intersections, the severity of the crash would've been " + pred2;
            }
            else if (cd.UNRESTRAINED_True == 1)
            {
                cd.UNRESTRAINED_True = 0;
                result = _session.Run(new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("float_input", cd.AsTensor())
            });
                score = result.First().AsTensor<float>();
                prediction = new Prediction { PredictedValue = score.First() };
                string pred2 = prediction.RoundedValue();
                result.Dispose();
                ViewBag.prediction2 = "If everyone involved wore seatbelts, the severity of the crash would've been " + pred2;
            }
            else if (cd.DISTRACTED_DRIVING_True == 1)
            {
                cd.DISTRACTED_DRIVING_True = 0;
                result = _session.Run(new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("float_input", cd.AsTensor())
            });
                score = result.First().AsTensor<float>();
                prediction = new Prediction { PredictedValue = score.First() };
                string pred2 = prediction.RoundedValue();
                result.Dispose();
                ViewBag.prediction2 = "If this driver was not distracted, the severity of the crash would've been " + pred2;
            }
            else if (cd.DROWSY_DRIVING_True == 1)
            {
                cd.DROWSY_DRIVING_True = 0;
                result = _session.Run(new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("float_input", cd.AsTensor())
            });
                score = result.First().AsTensor<float>();
                prediction = new Prediction { PredictedValue = score.First() };
                string pred2 = prediction.RoundedValue();
                result.Dispose();
                ViewBag.prediction2 = "If this driver was not drowsy, the severity of the crash would've been " + pred2;
            }
            else if (cd.DUI_True == 1)
            {
                cd.DUI_True = 0;
                result = _session.Run(new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("float_input", cd.AsTensor())
            });
                score = result.First().AsTensor<float>();
                prediction = new Prediction { PredictedValue = score.First() };
                string pred2 = prediction.RoundedValue();
                result.Dispose();
                ViewBag.prediction2 = "If this driver was not under the influence, the severity of the crash would've been " + pred2;
            }
            return View(crashdetail);
        }

        public IActionResult KeyStats()
        {
            return View();
        }

        [HttpGet]
        public IActionResult CrashScenarios()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CrashScenarios(CrashData cd, string city)
        {
            cd.CITY_PROVO = 0f;
            cd.CITY_OGDEN = 0f;
            cd.CITY_WEST_JORDAN = 0f;
            if (city == "provo")
            {
                cd.CITY_PROVO = 1f;
            }
            else if (city == "ogden")
            {
                cd.CITY_OGDEN = 1f;
            }
            else if (city == "westJordan")
            {
                cd.CITY_WEST_JORDAN = 1f;
            }
            var result = _session.Run(new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("float_input", cd.AsTensor())
            });
            Tensor<float> score = result.First().AsTensor<float>();
            var prediction = new Prediction { PredictedValue = score.First() };
            ViewBag.Prediction = prediction.RoundedValue();
            result.Dispose();
            return View();
        }

        public IActionResult SeverityCalculator()
        {
            return View();
        }

        public IActionResult ViewStories()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public IActionResult AddCrash()
        {
            ViewBag.Cities = repo.Cities.ToList();

            return View();
        }

        [HttpPost]
        public IActionResult AddCrash(Crash c)
        {
            ViewBag.Cities = repo.Cities.ToList();

            if (ModelState.IsValid)
            {
                //c.MAIN_ROAD_NAME.ToUpper();
                repo.CreateCrash(c);
                repo.SaveCrash(c);

                return RedirectToAction("Dataset");
            }
            else
            {
                return View();
            }
        }

        [HttpGet]
        [Authorize]
        public IActionResult EditCrash(int crashid)
        {
            ViewBag.Cities = repo.Cities.ToList();

            var editCrash = repo.Crashes.Single(x => x.CRASH_ID == crashid);
            return View("EditCrash", editCrash);
        }

        [HttpPost]
        public IActionResult EditCrash(Crash c)
        {
            ViewBag.Cities = repo.Cities.ToList();

            if (ModelState.IsValid)
            {
                c.MAIN_ROAD_NAME.ToUpper();
                repo.EditCrash(c);
                repo.SaveCrash(c);
                return RedirectToAction("Dataset");
            }
            else
            {
                return View();
            }
        }

        [HttpGet]
        [Authorize]
        public IActionResult DeleteCrash(int crashid)
        {

            var deleteCrash = repo.Crashes.Single(x => x.CRASH_ID == crashid);
            return View(deleteCrash);
        }

        [HttpPost]
        public IActionResult DeleteCrash(Crash c)
        {

            repo.DeleteCrash(c);
            return RedirectToAction("Dataset");
        }


    }
}