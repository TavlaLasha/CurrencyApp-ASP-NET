using DAL.EF;
using DAL.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class UnitOfWork
    {
        private LogRepository _logRepo;
        private CurrencyRepository _currencyRepo;
        private CurrencyDBContext _db;

        public UnitOfWork(CurrencyDBContext db)
        {
            _db = db;
        }
        public CurrencyRepository CurrencyRepo
        {
            get
            {
                if(_currencyRepo == null)
                {
                    _currencyRepo = new CurrencyRepository(_db);
                }
                return _currencyRepo;
            }
        }
        public LogRepository LogRepo
        {
            get
            {
                if (_logRepo == null)
                {
                    _logRepo = new LogRepository(_db);
                }
                return _logRepo;
            }
        }
        public bool Save()
        {
            bool jobDone = true;
            using (var dbContextTransaction = _db.Database.BeginTransaction())
            {
                try
                {
                    _db.SaveChanges();
                    dbContextTransaction.Commit();
                }
                catch (Exception)
                {
                    //Log Exception Handling message                      
                    jobDone = false;
                    dbContextTransaction.Rollback();
                }
            }

            return jobDone;
        }
    }
}
