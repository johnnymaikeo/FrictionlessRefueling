using FrictionlessRefueling.Biz.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrictionlessRefueling.Biz.Repository
{
    public class CarEntryRepository
    {
        public void Create(CarEntry entry)
        {
            using (var context = new DataModelContext())
            {
                context.CarEntries.Add(entry);
                context.SaveChanges();
            }
        }

        public IEnumerable<CarEntry> ListAll()
        {
            using (var context = new DataModelContext())
            {
                var query = from entry in context.CarEntries
                            select entry;
                return query.ToList();
            }
        }
    }
}
