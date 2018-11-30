using System;
using System.Data.Entity;
using System.Data.Entity.Validation;

namespace DataManagement.Repositories
{
    public class BaseRepository
    {
        protected virtual bool SaveChanges(DbContext db)
        {
            bool bSuccess = true;
            
            try
            {
                db.SaveChanges();
            }
            catch (DbEntityValidationException)
            {
                //     The save was aborted because validation of entity property values failed.

                bSuccess = false;
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateConcurrencyException)
            {
                //     A database command did not affect the expected number of rows. This usually
                //     indicates an optimistic concurrency violation; that is, a row has been changed
                //     in the database since it was queried.

                bSuccess = false;
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException)
            {
                //     An error occurred sending updates to the database.
                
                bSuccess = false;
            }
            catch (System.NotSupportedException ex)
            {
                //     An attempt was made to use unsupported behavior such as executing multiple
                //     asynchronous commands concurrently on the same context instance.
                
                bSuccess = false;
            }
            catch (System.ObjectDisposedException)
            {
                //     The context or connection have been disposed.
                bSuccess = false;
            }
            catch (System.InvalidOperationException)
            {
                //     Some error occurred attempting to process entities in the context either
                //     before or after sending commands to the database.

                bSuccess = false;
            }
            catch (Exception)
            {
                // made it pretty far - something went terribly wrong.

                bSuccess = false;
            }
            
            return bSuccess;
        }
    }
}
