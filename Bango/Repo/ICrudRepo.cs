using Bango.Base.List;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bango.Repo
{
    public interface ICrudRepo<TModel, TKey>
     where TModel : class, Bango.Models.IModel, new()
    {
        /// <summary>
        /// Will be saving all the errors that rose in the repository while interacting with database
        /// </summary>
        List<string> Errors { get; set; }
        /// <summary>
        /// Fetches data returns it as Model
        /// Note: it will create a new connection to fetch the data
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Instance of TModel</returns>
        TModel GetAsModel(TKey id);
        /// <summary>
        /// Fetches data returns it as Model
        /// Note: it will create a new connection to fetch the data
        /// </summary>
        /// <param name="id"></param>
        /// <param name="con">The database connection to be used</param>
        /// <returns>Instance of TModel</returns>
        TModel GetAsModel(DbConnect con, TKey id);
        /// <summary>
        /// Fetches data returns it as Model
        /// Note: it will create a new connection to fetch the data
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Returns data in dynamic object.</returns>
        DynamicDictionary Get(TKey id);
        /// <summary>
        /// Fetches data returns it as Model
        /// Note: it will create a new connection to fetch the data
        /// </summary>
        /// <param name="con">The database connection to be used</param>
        /// <param name="id"></param>
        /// <returns>Returns data in dynamic object.</returns>
        DynamicDictionary Get(DbConnect con, TKey id);
        DynamicDictionary Get(DbConnect con, DynamicDictionary param, string tableAlias = null);
        /// <summary>
        /// Performs soft delete of data. Sets the is_deleted flag = true.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Returns the status of the delete operation</returns>
        bool SoftDelete(TKey id);

        /// <summary>
        /// Performs soft delete of data. Sets the is_deleted flag = true.
        /// </summary>
        /// <param name="con">The database connection to be used</param>
        /// <param name="id"></param>
        /// <returns>Returns the status of the delete operation</returns>
        bool SoftDelete(DbConnect con, TKey id);
        /// <summary>
        /// Performs hard delete of data
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Returns the status of the delete operation</returns>
        bool HardDelete(TKey id);

        /// <summary>
        /// Performs hard delete of data.
        /// </summary>
        /// <param name="con">The database connection to be used</param>
        /// <param name="id"></param>
        /// <returns>Returns the status of the delete operation</returns>
        bool HardDelete(DbConnect con, TKey id);
        /// <summary>
        /// Sets flag if the chagnes are to be tracked or not. The value is passed from ServiceLayer so if any changes on the TrackChanges property has to be maintained in Service Layer.
        /// Default value = true.
        /// </summary>
        bool TrackChanges { get; set; }
        /// <summary>
        /// Inserts the data into the database.
        /// </summary>
        /// <param name="data">Data which need to be inserted</param>
        /// <returns>Returns the status of insert operation.</returns>
        bool Insert(DynamicDictionary data);
        /// <summary>
        /// Inserts the data into the database.
        /// </summary>
        /// <param name="con">The database connection to be used</param>
        /// <param name="data">Data which need to be inserted</param>
        /// <returns>Returns the status of insert operation.</returns>
        bool Insert(DbConnect con, DynamicDictionary data);

        /// <summary>
        /// Updates the data into the database.
        /// </summary>
        /// <param name="id">The key value of which the data need to be updated.</param>
        /// <param name="item">Data which need to be Updated</param>
        /// <returns>Returns the status of Update operation.</returns>
        bool Update(TKey id, DynamicDictionary item);
        /// <summary>
        /// Updates the data into the database.
        /// </summary>
        /// <param name="con">The database connection to be used</param>
        /// <param name="id">The key value of which the data need to be updated.</param>
        /// <param name="data">Data which need to be Updated</param>
        /// <returns>Returns the status of Update operation.</returns>
        bool Update(DbConnect con, TKey id, DynamicDictionary data);
        bool CheckClientID { get; set; }
        bool DisplayMasterDataFromSystem { get; set; }
        bool Is_Child_Records_Exists { get; set; }
    }
}
