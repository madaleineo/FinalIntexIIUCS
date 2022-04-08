using System;
using System.Linq;
namespace UtahCarSafety.Models.ViewModels
{
    public class CrashViewModel
    {
        public IQueryable<Crash> Crashes { get; set; }
        public IQueryable<City> Cities { get; set; }
        public string CurrentCity { get; set; }
        public int? CurrentSeverity { get; set; }
        public string CurrentCounty { get; set; }
        public int? CurrentYear { get; set; }
        public int? CurrentMonth { get; set; }
        public int? CurrentHour { get; set; }
        public string CurrentRoad { get; set; }
        public int? CurrentTeenage { get; set; }
        public int? CurrentWorkZone { get; set; }
        public int? CurrentPedestrian { get; set; }
        public int? CurrentBicyclist { get; set; }
        public int? CurrentMotorcycle { get; set; }
        public int? CurrentImproper { get; set; }
        public int? CurrentUnrestrained { get; set; }
        public int? CurrentDUI { get; set; }
        public int? CurrentIntersection { get; set; }
        public int? CurrentWild { get; set; }
        public int? CurrentDomestic { get; set; }
        public int? CurrentOverturn { get; set; }
        public int? CurrentCommercial { get; set; }
        public int? CurrentOlder { get; set; }
        public int? CurrentNight { get; set; }
        public int? CurrentSingle { get; set; }
        public int? CurrentDistracted { get; set; }
        public int? CurrentDrowsy { get; set; }
        public int? CurrentRoadway { get; set; }
        public PageInfo PageInfo { get; set; }
    }
}