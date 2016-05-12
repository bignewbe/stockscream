using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;

namespace StockScream.Services
{
    /// <summary>
    /// Unit of Work class responsible for DB transactions
    /// </summary>
    public class MyUserManager<TEntity, TContext> : IDisposable where TEntity : class where TContext : class
    {
        private DbContext _context;
        private GenericRepository<TEntity> _entityRepository;

        public MyUserManager()
        {
            _context = Activator.CreateInstance(typeof(TContext)) as DbContext;
        }

        /// <summary>
        /// Get/Set Property for user repository.
        /// </summary>
        public GenericRepository<TEntity> EntityRepository
        {
            get
            {
                if (this._entityRepository == null)
                    this._entityRepository = new GenericRepository<TEntity>(_context);
                return _entityRepository;
            }
        }

        /// <summary>
        /// Save method.
        /// </summary>
        public void Save()
        {
            try {
                _context.SaveChanges();
            }
            catch (DbEntityValidationException e) {
                var outputLines = new List<string>();
                foreach (var eve in e.EntityValidationErrors) {
                    outputLines.Add(string.Format("{0}: Entity of type \"{1}\" in state \"{2}\" has the following validation errors:", DateTime.Now, eve.Entry.Entity.GetType().Name, eve.Entry.State));
                    foreach (var ve in eve.ValidationErrors) {
                        outputLines.Add(string.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage));
                    }
                }
                System.IO.File.AppendAllLines(@"C:\errors.txt", outputLines);

                throw e;
            }
        }

        /// <summary>
        /// Dispose method
        /// </summary>
        public virtual void Dispose()
        {
            _context.Dispose();
        }
    }
}