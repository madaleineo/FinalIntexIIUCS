using System;
using System.Linq;

namespace UtahCarSafety.Models
{
    public interface ICrashesRepository
    {
        IQueryable<Crash> Crashes { get; }
        IQueryable<City> Cities { get; }

        void EditCrash(Crash c);

        void DeleteCrash(Crash c);

        void CreateCrash(Crash c);

        void SaveCrash(Crash c);
    }
}