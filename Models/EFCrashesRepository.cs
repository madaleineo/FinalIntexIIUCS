using System;
using System.Linq;
namespace UtahCarSafety.Models
{
    public class EFCrashesRepository : ICrashesRepository
    {
        private CrashesDbContext _context { get; set; }

        public EFCrashesRepository(CrashesDbContext temp)
        {
            _context = temp;
        }

        public IQueryable<Crash> Crashes => _context.Crashes;
        public IQueryable<City> Cities => _context.Cities;


        public void EditCrash(Crash c)
        {
            _context.Update(c);
        }

        public void SaveCrash(Crash c)
        {
            _context.SaveChanges();
        }

        public void CreateCrash(Crash c)
        {
            _context.Add(c);
            _context.SaveChanges();
        }

        public void DeleteCrash(Crash c)
        {
            _context.Remove(c);
            _context.SaveChanges();
        }
    }
}
