using InkNoteLite.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace InkNoteLite.Service
{
    public class DataService
    {
        public static bool Add(InkModel ink)
        {
            int result = 0;
            using(var db = DbContext.GetDbConnection())
            {
                result = db.Insert(ink);
            }
            return result > 0;
        }

        public static bool Update(InkModel ink)
        {
            int result = 0;
            using (var db = DbContext.GetDbConnection())
            {
                result = db.Update(ink);
            }
            return result > 0;
        }

        public static bool Delete(int id)
        {
            int result = 0;
            using (var db = DbContext.GetDbConnection())
            {
                result = db.Delete<InkModel>(id);
            }
            return result > 0;
        }

        public static bool DeleteAll()
        {
            int result = 0;
            using (var db = DbContext.GetDbConnection())
            {
                result = db.DeleteAll<InkModel>();
            }
            return result > 0;
        }

        public static List<InkModel> GetEntities(Expression<Func<InkModel,bool>> wherelambda)
        {
            List<InkModel> InkList = new List<InkModel>();
            using (var db = DbContext.GetDbConnection())
            {
                InkList = db.Table<InkModel>().Where(wherelambda).ToList();
            }
            return InkList;
        }
    }
}
