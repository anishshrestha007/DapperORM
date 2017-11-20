using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bango.Base.List;
using System.Data;
using System.Reflection;
using Bango.Helpers;
using Dapper;
using Bango.Base;
using Bango.Models.Attributes;
using Bango.Base.Log;
using System.Dynamic;

namespace Bango.Repo
{
    public class CrudRepo<TModel, TKey> : ICrudRepo<TModel, TKey>
        where TModel : class, Models.IModel, new()

    {
        public bool CheckClientID { get; set; } = true;
        public bool DisplayMasterDataFromSystem { get; set; } = false;
        public string MainTableAlias { get; set; } = "c";
        public List<string> Errors { get; set; } = new List<string>();
        public bool TrackChanges { get; set; } = true;
        public bool Is_Child_Records_Exists { get; set; } = false;

        public virtual BangoCommand GetItemCommand(DynamicDictionary data_param, string tableAlias = null)
        {
            BangoCommand cmd = new BangoCommand();
            TModel mdl = new TModel();
            
            tableAlias = tableAlias == null ? MainTableAlias : tableAlias;
            cmd.Template = cmd.SqlBuilder.AddTemplate(string.Format(@"
SELECT * 
FROM {0} {1} 
/**where**/
/**orderby**/"
                , mdl.GetTableName(), tableAlias));

            //PropertyInfo prop = mdl.GetKeyPropertyInfo();
            //cmd.SqlBuilder.Where(string.Format("{0}=@{0}", prop.Name), DbServiceUtility.ToDynamicParameter(prop.Name, id, DbServiceUtility.TypeMap[prop.PropertyType]));
            DbServiceUtility.BindDeleteParameter(cmd, mdl, tableAlias);
            DbServiceUtility.BindParameters(cmd, mdl, data_param, tableAlias, SearchTypes.Equal);

            if (CheckClientID)
            {
                PropertyInfo client_id = mdl.GetType().GetProperty("client_id");
                if (client_id != null)
                {
                    DbServiceUtility.BindClientIdParameter(cmd, mdl, tableAlias, DisplayMasterDataFromSystem);
                    //string col = DbServiceUtility.SetColumnAlias(tableAlias, "client_id");
                    //cmd.SqlBuilder.Where("(client_id = 1 OR client_id = @client_id)");
                    //DynamicParameters param = new DynamicParameters();
                    //param.Add("@client_id", SessionData.client_id, DbType.Int32);
                    //cmd.SqlBuilder.AddParameters(param);
                }
            }
            return cmd;
        }
        public virtual DynamicDictionary Get(TKey id)
        {
            using (DbConnect con = new DbConnect())
            {
                return Get(con, id);
            }
        }

        public virtual DynamicDictionary Get(DbConnect con, DynamicDictionary param, string tableAlias = null)
        {
            if (con == null)
            {
                throw new DbConnectionNotPassed();
            }
            BangoCommand cmd = GetItemCommand(param, tableAlias);
            string finalSql = cmd.FinalSql;
            if (finalSql.Length > 0)
            {
                IEnumerable<SqlMapper.DapperRow> items = null;
                try
                {
                    items = con.DB.Query<SqlMapper.DapperRow>(finalSql, cmd.FinalParameters, true);
                }
                catch(Exception ex)
                {
                    LogTrace.WriteErrorLog(ex.ToString());
                }
                Errors = con.DB.GetErros();
                if (items != null && items.Count() > 0)
                {
                    return Conversion.ToDynamicDictionary(items.FirstOrDefault());
                }

                return null;
            }
            return null;
        }
        public virtual DynamicDictionary Get(DbConnect con, TKey id)
        {

            DynamicDictionary data_param = new DynamicDictionary();
            TModel mdl = new TModel();
            data_param.Add(mdl.GetKeyPropertyName(), id);
            dynamic item = Get(con, data_param);
            if(item != null)
            {
                return item;
            }
            else
            {
                TModel itm = GetAsModel(con, id);
                if (itm == null)
                    return null;
                else
                    return Models.ModelService.ToDictionary(itm);
            }
        }

        public virtual TModel GetAsModel(TKey id)
        {
            using (DbConnect con = new DbConnect())
            {
                return GetAsModel(con, id);
            }
        }

        public virtual TModel GetAsModel(DbConnect con, TKey id)
        {
            TModel item;
            ITable<TModel, TKey> tbl = con.GetModelTable<TModel, TKey>();
            item = tbl.Get(id);
            //checking if the item is already deleted (soft deleted)
            if (item != null)
            {
                TableDetailAttribute tableDetail = item.GetTableDetail();
                PropertyInfo is_deleted = item.GetType().GetProperty(tableDetail.DeleteFlagField);

                if (is_deleted != null)
                {
                    bool dele = Convert.ToBoolean(is_deleted.GetValue(item));
                    if (dele == true)
                    {
                        Errors.Add("Record already deleted.");
                        return null;
                    }
                }

                if (CheckClientID)
                {
                    PropertyInfo client_id = item.GetType().GetProperty("client_id");
                    if (client_id != null)
                    {
                        if ((int?)client_id.GetValue(item) != SessionData.client_id)
                        {
                            Errors.Add("Record doesnot belong.");
                            return null;
                        }
                            
                    }
                }
            }
            return item;
        }

        public virtual bool Insert(DynamicDictionary data)
        {
            using (DbConnect con =new DbConnect())
            {
                return Insert(con, data);
            }
        }

        public virtual bool Insert(DbConnect con, DynamicDictionary data)
        {
            //create empty object
            TModel empty = new TModel();
            ChangeHistoryHelper<TModel> chngHlpr = null;

            chngHlpr = new ChangeHistoryHelper<TModel>(AuditActivityTypes.INSERT);
            PropertyInfo key = Models.ModelService.GetKeyPropertyInfo(empty);
            if (data.ContainsKey(key.Name))
            {
                data.Remove(key.Name);
            }
            //if CREATED_BY, CREATED_on field exists then update those fields
            PropertyInfo by = empty.GetType().GetProperty("created_by");
            if (by != null)
            {
                data.SetValue("created_by", SessionData.user_id);
            }
            PropertyInfo on = empty.GetType().GetProperty("created_on");
            if (on != null)
            {
                data.SetValue("created_on", DateTime.Now);
            }
            if (CheckClientID)
            {
                PropertyInfo client_id = empty.GetType().GetProperty("client_id");
                if (client_id != null)
                {
                    if (!data.ContainsKey("client_id"))
                    {
                        data.SetValue("client_id", SessionData.client_id);
                    }
                }
            }
            
            chngHlpr.CheckChangeChanges(new TModel(), data);
            ITable<TModel, int?> tbl = con.GetModelTable<TModel>();
            int? id = null;
            try
            {
                id = tbl.Insert(chngHlpr.Diff);
                //set the primary key in the object

                if (key != null)
                {
                    data.SetValue(key.Name, id);
                }

                if (TrackChanges)
                {
                   // chngHlpr.Changes.pkey_value = Convert.ToString(id);
                    //save the changes
                    chngHlpr.LogChanges(con);
                }
            }
            catch (Npgsql.NpgsqlException ex)
            {
                LogTrace.WriteErrorLog(ex.ToString());
                LogTrace.WriteDebugLog(string.Format("sql which gave exception:\r{0}", ex.Routine));
                throw ex;
            }
            if(id > 0)
            {
                return true;
            }
            return false;
        }

        public virtual bool SoftDelete(TKey id)
        {
            using (DbConnect con = new DbConnect())
            {
                return SoftDelete(con, id);
            }
        }

        public virtual bool SoftDelete(DbConnect con, TKey id)
        {
            Is_Child_Records_Exists = false; //Default child record off
            //pull old data
            TModel oldData = new TModel();
            ITable<TModel, TKey> tbl = con.GetModelTable<TModel, TKey>();
            oldData = tbl.Get(id);
            //checking if the data is editable by current login or not
            if (CheckClientID)
            {
                if (ValidateForClientData(oldData) == false)
                    return false;
            }
            TableDetailAttribute tableDatail = oldData.GetTableDetail();

            //09 feb 2016  Foreign key refrence table data exists or not checke..
            #region todo:shivahwor
            StringBuilder Sql = new StringBuilder();
            Sql.AppendFormat(@"
                    SELECT 
                --tc.table_schema, tc.constraint_name, 
                tc.table_name, kcu.column_name
                --,ccu.table_name AS foreign_table_name, ccu.column_name AS foreign_column_name
                    FROM information_schema.table_constraints tc
                    JOIN information_schema.key_column_usage kcu ON tc.constraint_name = kcu.constraint_name
                    JOIN information_schema.constraint_column_usage ccu ON ccu.constraint_name = tc.constraint_name
                    WHERE constraint_type = 'FOREIGN KEY'
                    AND ccu.table_name='{0}' ", tableDatail.Name );

                IEnumerable<DynamicDictionary> foreignkey_Table = DbServiceUtility.ExecuteList(con, Sql.ToString());

                Sql.Length = 0;
                DynamicDictionary rec= null;
                if (foreignkey_Table != null)
                {
                    foreach (DynamicDictionary item in foreignkey_Table)
                    {
                        object tbl_Name = item.GetValue("table_name");
                        object col_name = item.GetValue("column_name");

                        if (Sql.Length > 0)
                            Sql.AppendLine("\n\t UNION ALL ");

                        Sql.AppendFormat(" SELECT 1 from {0} Where {1}={2}  AND is_deleted ='F' ", tbl_Name.ToString(), col_name.ToString(), id);
                    }
                    rec = DbServiceUtility.ExecuteItem(con, Sql.ToString());
                }

                if (rec != null)
                {
                    if (rec.KeyList.Count > 0)
                    {
                        Is_Child_Records_Exists = true; //if child records exists than do not remove...
                        return false;
                    }
                }

            #endregion

            //data = new TModel();
            DynamicDictionary data_param = new DynamicDictionary();
            ChangeHistoryHelper<TModel> chngHlpr = null;
            chngHlpr = new ChangeHistoryHelper<TModel>(AuditActivityTypes.SOFTDELETE);
            //if CREATED_BY, CREATED_on field exists then update those fields
            PropertyInfo by = oldData.GetType().GetProperty("deleted_by");
            if (by != null)
            {
                data_param.SetValue("deleted_by", SessionData.user_id);
                //by.SetValue(data, Bango.GetCurrentUserId());
            }
            PropertyInfo on = oldData.GetType().GetProperty("deleted_on");
            if (on != null)
            {
                //on.SetValue(data, DateTime.Now);
                data_param.SetValue("deleted_on", DateTime.Now);
            }

            PropertyInfo uq_code = oldData.GetType().GetProperty("deleted_uq_code");
            if (on != null)
            {
                //on.SetValue(data, DateTime.Now);

                data_param.SetValue("deleted_uq_code", DateTime.Now.ToString("MMddHHmmss"));
            }

            PropertyInfo is_deleted = oldData.GetType().GetProperty(tableDatail.DeleteFlagField);
            if (is_deleted != null)
            {
                //is_deleted.SetValue(data, true);
                data_param.SetValue(tableDatail.DeleteFlagField, true);
            }

            chngHlpr.CheckChangeChanges(oldData, data_param);
            //chngHlpr.Diff.Add("id", id, DbType.Int32, ParameterDirection.Input);
            DynamicParameters where = new DynamicParameters();
            where.Add("id", id);
            if (CheckClientID)
            {
                PropertyInfo client_id = oldData.GetType().GetProperty("client_id");
                if (client_id != null)
                {
                    where.Add("client_id", SessionData.client_id);
                }
            }
            try
            {
                
                int? savedId = tbl.Update(where, chngHlpr.Diff);

                if (TrackChanges)
                {
                    //save the changes
                    chngHlpr.LogChanges(con);
                }
            }
            catch (Npgsql.NpgsqlException ex)
            {
                LogTrace.WriteErrorLog(ex.ToString());
                LogTrace.WriteDebugLog(string.Format("SQL which gave exception:\r{0}", ex.Routine));
                throw ex;
            }

            return true;
        }


        public virtual bool HardDelete(TKey id)
        {
            using (DbConnect con = new DbConnect())
            {
                return HardDelete(con, id);
            }
        }

        public virtual bool HardDelete(DbConnect con, TKey id)
        {
            //pull old data
            TModel oldData = new TModel();
            TableDetailAttribute tableDatail = oldData.GetTableDetail();
            //data = new TModel();
            DynamicDictionary data_param = new DynamicDictionary();
           // ChangeHistoryHelper<TModel> chngHlpr = null;
            ITable<TModel, int?> tbl = con.GetModelTable<TModel>();
            bool status = false;
            //chngHlpr.Diff.Add("id", id, DbType.Int32, ParameterDirection.Input);
            DynamicParameters where = new DynamicParameters();
            where.Add("id", id);
            if (CheckClientID)
            {
                PropertyInfo client_id = oldData.GetType().GetProperty("client_id");
                if (client_id != null)
                {
                    where.Add("client_id", SessionData.client_id);
                }
            }
            try
            {
                
                //int? pk = (int?)Convert.ChangeType(id, typeof(int?));
                status = tbl.Delete((int?)Base.Conversion.ToInt32(id));
            }
            catch (Npgsql.NpgsqlException ex)
            {
                LogTrace.WriteErrorLog(ex.ToString());
                LogTrace.WriteDebugLog(string.Format("SQL which gave exception:\r{0}", ex.Routine));
                throw ex;
            }

            return status;
        }
        public virtual bool Update(TKey id, DynamicDictionary item)
        {
            throw new NotImplementedException();
        }
        protected virtual bool ValidateForClientData(TModel oldData)
        {
            Models.ModelBase lng = new Models.ModelBase();
            PropertyInfo client_id = oldData.GetType().GetProperty("client_id");
            if (client_id != null)
            {
                int data_client_id = Conversion.ToInt32(oldData.GetValue("client_id"));
                if (data_client_id != SessionData.client_id)
                {
                    //check if the data being edited is master data or not
                    if (data_client_id == 1 && SessionData.client_id != 1)
                    {
                        //Errors.Add("Master data can't be edited.");
                        Errors.Add(lng.GetLang("master_data_edit"));
                        return false;
                    }
                    else
                    {
                        Errors.Add(lng.GetLang("master_data_edit_right"));
                        return false;
                    }
                }
            }
            return true;
        }
        public virtual bool Update(DbConnect con, TKey id, DynamicDictionary data)
        {
            //pull old data
            TModel oldData = new TModel();
            ITable<TModel, TKey> tbl = con.GetModelTable<TModel, TKey>();
            oldData = tbl.Get(id);
            //checking if the data is editable by current login or not
            if (CheckClientID)
            {
                if (ValidateForClientData(oldData) == false)
                    return false;
            }


            ChangeHistoryHelper<TModel> chngHlpr = null;
            chngHlpr = new ChangeHistoryHelper<TModel>(AuditActivityTypes.UPDATE);
            //if CREATED_BY, CREATED_on field exists then update those fields
            PropertyInfo by = oldData.GetType().GetProperty("updated_by");
            if (by != null)
            {
                data.SetValue("updated_by", SessionData.user_id);
            }
            PropertyInfo on = oldData.GetType().GetProperty("updated_on");
            if (on != null)
            {
                data.SetValue("updated_on", DateTime.Now);
            }

            
            
            dynamic cloned = data.Clone();
            chngHlpr.CheckChangeChanges(oldData, data);
            //if no changes then return true
            if(chngHlpr.Diff.ParameterNames.Count() == 0)
            {
                return true;
            }

            int? savedId = null;

            //chngHlpr.Diff.Add("id", id, DbType.Int32, ParameterDirection.Input);
            DynamicParameters where = new DynamicParameters();
            where.Add("id", id, DbServiceUtility.GetDbType(typeof(TKey)));
            if (CheckClientID)
            {
                PropertyInfo client_id = oldData.GetType().GetProperty("client_id");
                if (client_id != null)
                {
                    where.Add("client_id", SessionData.client_id, DbServiceUtility.GetDbType(client_id.PropertyType));
                }
            }
            try
            {
                savedId = tbl.Update(where, chngHlpr.Diff);

                if (TrackChanges)
                {
                    //save the changes
                    chngHlpr.LogChanges(con);
                }
            }
            catch (Npgsql.NpgsqlException ex)
            {
                LogTrace.WriteErrorLog(ex.ToString());
                LogTrace.WriteDebugLog(string.Format("SQL which gave exception:\r{0}", ex.Routine));
                throw ex;
            }
            if (savedId > 0)
                return true;
            else
                return false;
        }
    }
}
//TODO: