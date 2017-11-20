
using Dynamitey;
using Bango.Attributes;
using Bango.Base;
using Bango.Base.List;
using Bango.Base.Log;
using Bango.Models;
using Bango.Models.Attributes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using LightInject;
using Dapper.Rainbow;
using Dapper;

namespace Bango.Helpers
{
    public static class DbServiceUtility
    {
        static IDbExpression _dbExp;
        static Dictionary<Type, DbType> _typeMap;

        static DbServiceUtility()
        {

        }
        public static IDbExpression DbExp
        {
            get
            {
                if (_dbExp == null)
                    _dbExp = App.Container.GetInstance<IDbExpression>();
                return _dbExp;
            }
        }

        static object TypeMapUpdateFlag = new object();
        private static Dictionary<Type, DbType> TypeMap
        {
            get
            {
                if (_typeMap == null)
                {
                    lock (TypeMapUpdateFlag)
                    {
                        if (_typeMap == null)
                            InitTypeMap();
                    }
                }
                return _typeMap;
            }
        }

        public static DbType GetDbType(Type typ, PropertyInfo prop = null)
        {
            //if()
            DbType t = TypeMap[typ];
            if (prop?.GetCustomAttributes(typeof(TimeOnlyAttribute), true).Length > 0)
            {
                t = DbType.Time;
            }
            return t;

        }
        public static BangoCommand BindOrderBy(BangoCommand cmd, string orderBy)
        {
            orderBy = orderBy == null ? string.Empty : orderBy.Trim();
            if (orderBy.Length > 0)
            {
                if (cmd.CommandType == MyroCommandTypes.SqlBuilder)
                    cmd.SqlBuilder.OrderBy(string.Format("{0}", orderBy));
                else
                    cmd.SqlString.AppendLine(string.Format(" ORDER BY {0}", orderBy));
            }
            return cmd;
        }
        public static BangoCommand BindPagination(BangoCommand cmd, int page, int pageSize)
        {
            //if page given is less than 0 means disable pagination
            //so do nothing for pagination
            if (page <= 0)
                return cmd;
            if (pageSize < 1)
                return cmd;
            //page caculation
            int from = 0, to = 0;
            from = (pageSize * page) - pageSize + 1;
            to = pageSize * page;
            if (cmd.CommandType == MyroCommandTypes.SqlBuilder)
            {
                StringBuilder sb = new StringBuilder(cmd.Template.tplSql);
                sb.Insert(0, string.Format(@"SELECT *
  FROM (SELECT {0}
              ,a.*
          FROM (", DbExp.RowNumber()));
                sb.AppendFormat(@") a) pgd
 WHERE rnum BETWEEN {0} AND {1}", from, to);
                cmd.Template.tplSql = sb.ToString();
            }
            else
            {
                cmd.SqlString.Insert(0, string.Format(@"SELECT *
  FROM (SELECT {0}
              ,a.*
          FROM (", DbExp.RowNumber()));
                cmd.SqlString.AppendFormat(@") a) pgd
 WHERE rnum BETWEEN {0} AND {1}", from, to);
            }

            return cmd;
        }
        public static BangoCommand BindParameter(BangoCommand cmd, string fieldName, DynamicDictionary data_param
            , Type type = null, string tableAlias = null, SearchTypes searchType = SearchTypes.Like
            , string actualFieldName = null
            , bool checkNull = false, PropertyInfo prop = null)
        {
            if (type == null)
                type = Type.GetType("string");
            return BindParameter(cmd, fieldName, data_param, GetDbType(type, prop), tableAlias, searchType, actualFieldName, checkNull, prop);

        }
        public static BangoCommand BindParameter(BangoCommand cmd, string fieldName, DynamicDictionary data_param
            , DbType dbType = DbType.String, string tableAlias = null, SearchTypes searchType = SearchTypes.Like
            , string actualFieldName = null
            , bool checkNull = false, PropertyInfo prop = null)
        {
            //throw new Exception("Don't use this, consult with shakti.shrestha@gmail.com before using this");

            fieldName = fieldName.Trim().ToLower();
            actualFieldName = actualFieldName == null ? string.Empty : actualFieldName.Trim().ToLower();
            //if the property doesnot exists in the data parameter then continue.
            if (data_param.ContainsKeyStartWith(fieldName) == true
                || (actualFieldName.Trim().Length > 0 &&
                    data_param.ContainsKeyStartWith(actualFieldName + "___")) == true
                || data_param.ContainsKeyStartWith(fieldName + "___")
                )
            {
            }
            else
            {
                return cmd;
            }
            string append = GetTableAliasForColumn(tableAlias);
            string propName = fieldName
                , column = string.Format("{0}{1}", append, fieldName)
                , clause = string.Empty;
            if (data_param.ContainsKey(actualFieldName + "___multi"))
            {
                propName = actualFieldName + "___multi";
            }

            switch (ConvertDbTypeCodeToBaseType(dbType))
            {
                case BaseDataTypes.String:


                    //ActualFieldName actualField = prop.GetCustomAttribute<ActualFieldName>();
                    //string actualFieldName = actualField == null ? string.Empty : actualField.FieldName;
                    if ((searchType & SearchTypes.CaseSensetive) != SearchTypes.CaseSensetive)
                    {
                        if (!checkNull)
                        {
                            column = string.Format("lower({0})", column);
                            data_param[propName] = data_param[propName].ToString().ToLower();
                        }

                    }
                    if (data_param.GetValueAsString(propName) == null)
                    {
                        if (checkNull)
                        {
                            clause = string.Format("{0} is null", column);
                            if (cmd.CommandType == MyroCommandTypes.SqlBuilder)
                            {
                                cmd.SqlBuilder.Where(clause);
                            }
                            else
                            {
                                cmd.SqlString.AppendLine(" AND " + clause);
                            }
                        }
                        break;

                    }

                    if (data_param.GetValueAsString(propName).Trim().Length == 0)
                    {
                        break;
                    }

                    if (data_param.ContainsKeyStartWith(actualFieldName + "___multi"))
                    {
                        switch (searchType)
                        {
                            case SearchTypes.StartWith:
                            case SearchTypes.EndWith:
                            case SearchTypes.Like:
                                clause = string.Format(" (lower({2}{0}_en) like @{1} OR lower({2}{0}_np) like @{1})", actualFieldName, propName, append);
                                break;
                            case SearchTypes.Equal:
                                clause = string.Format(" (lower({2}{0}_en) = @{1} OR lower({2}{0}_np) = @{1})", actualFieldName, propName, append);
                                break;
                            case SearchTypes.StartWith | SearchTypes.CaseSensetive:
                            case SearchTypes.EndWith | SearchTypes.CaseSensetive:
                            case SearchTypes.Like | SearchTypes.CaseSensetive:
                                clause = string.Format(" ({2}{0}_en like @{1} OR {2}{0}_np like @{1})", actualFieldName, propName, append);
                                break;
                            case SearchTypes.Equal | SearchTypes.CaseSensetive:
                                clause = string.Format(" ({2}{0}_en = @{1} OR {2}{0}_np = @{1})", actualFieldName, propName, append);
                                break;
                            case SearchTypes.isNull:
                                clause = string.Format("{0} IS NULL OR {0}='' ", column, propName);
                                break;
                            case SearchTypes.isNotNull:
                                clause = string.Format("{0} IS NOT NULL ", column, propName);
                                break;
                            case SearchTypes.IN_Search:
                                clause = string.Format("{0} IN ( {1} ) ", column, data_param[propName]);
                                break;
                        }
                    }
                    else
                    {
                        switch (searchType)
                        {
                            case SearchTypes.StartWith:
                            case SearchTypes.EndWith:
                            case SearchTypes.Like:
                                clause = string.Format("lower({0}) like @{1}", column, propName);
                                break;
                            case SearchTypes.Equal:
                                clause = string.Format("lower({0}) = @{1}", column, propName);
                                break;
                            case SearchTypes.StartWith | SearchTypes.CaseSensetive:
                            case SearchTypes.EndWith | SearchTypes.CaseSensetive:
                            case SearchTypes.Like | SearchTypes.CaseSensetive:
                                clause = string.Format("{0} like @{1}", column, propName);
                                break;
                            case SearchTypes.Equal | SearchTypes.CaseSensetive:
                                clause = string.Format("{0} = @{1}", column, propName);
                                break;
                            case SearchTypes.isNull:
                                clause = string.Format("{0} IS NULL OR {0}='' ", column, propName);
                                break;
                            case SearchTypes.isNotNull:
                                clause = string.Format("{0} IS NOT NULL", column, propName);
                                break;
                            case SearchTypes.IN_Search:
                                clause = string.Format("{0} IN ( {1} ) ", column, data_param[propName]);
                                break;

                        }
                    }



                    if (cmd.CommandType == MyroCommandTypes.SqlBuilder)
                    {
                        DynamicParameters param = new DynamicParameters();
                        switch (searchType)
                        {
                            case SearchTypes.StartWith:
                            case SearchTypes.StartWith | SearchTypes.CaseSensetive:
                                param.Add(propName, data_param.GetValueAsString(propName) + "%");
                                break;
                            case SearchTypes.EndWith:
                            case SearchTypes.EndWith | SearchTypes.CaseSensetive:
                                param.Add(propName, "%" + data_param.GetValueAsString(propName));
                                break;
                            case SearchTypes.Like:
                            case SearchTypes.Like | SearchTypes.CaseSensetive:
                                param.Add(propName, "%" + data_param.GetValueAsString(propName) + "%");
                                break;
                            case SearchTypes.Equal:
                            case SearchTypes.Equal | SearchTypes.CaseSensetive:
                                param.Add(propName, data_param[propName]);
                                break;
                        }
                        cmd.SqlBuilder.Where(clause, param);
                    }
                    else
                    {
                        //value or parameter
                        cmd.SqlString.AppendLine(" AND " + clause);
                        switch (searchType)
                        {
                            case SearchTypes.StartWith:
                            case SearchTypes.StartWith | SearchTypes.CaseSensetive:
                                cmd.Parameters.Add("@" + propName, data_param.GetValueAsString(propName) + "%", dbType);
                                break;
                            case SearchTypes.EndWith:
                            case SearchTypes.EndWith | SearchTypes.CaseSensetive:
                                cmd.Parameters.Add("@" + propName, "%" + data_param.GetValueAsString(propName), dbType);
                                break;
                            case SearchTypes.Like:
                            case SearchTypes.Like | SearchTypes.CaseSensetive:
                                cmd.Parameters.Add("@" + propName, "%" + data_param.GetValueAsString(propName) + "%", dbType);
                                break;
                            case SearchTypes.Equal:
                            case SearchTypes.Equal | SearchTypes.CaseSensetive:
                                cmd.Parameters.Add("@" + propName, data_param[propName], dbType);
                                break;
                        }
                    }
                    //remove the param
                    data_param.Remove(propName);
                    break;
                case BaseDataTypes.Boolean:
                    //TODO 

                    clause = string.Format("{1}{0} = @{0}", propName, append);
                    if (cmd.CommandType == MyroCommandTypes.SqlBuilder)
                    {
                        DynamicParameters param = new DynamicParameters();
                        param.Add(propName, data_param.GetValueAsBoolean(propName), dbType);
                        cmd.SqlBuilder.Where(clause, param);
                    }
                    else
                    {
                        cmd.SqlString.AppendLine(" AND " + clause);
                        cmd.Parameters.Add("@" + propName, data_param.GetValueAsBoolean(propName), DbType.Boolean);
                    }
                    data_param.Remove(propName);
                    break;
                case BaseDataTypes.DateTime:
                    //TODO
                    if (data_param.GetValueAsString(propName)?.Trim().Length == 0)
                    {
                        //if (checkNull)
                        //{
                        clause = string.Format("{0} is null", column);
                        if (cmd.CommandType == MyroCommandTypes.SqlBuilder)
                        {
                            cmd.SqlBuilder.Where(clause);
                        }
                        else
                        {
                            cmd.SqlString.AppendLine(" AND " + clause);
                        }
                        //}
                        break;

                    }
                    if (data_param.ContainsKeyStartWith(propName + "___"))
                    {
                        //between
                        cmd = DbServiceUtility.GetBetweenClause(cmd, propName
                            , Conversion.ToString(data_param[propName + "___from"])
                            , Conversion.ToString(data_param[propName + "___to"])
                            , tableAlias, dbType == DbType.Time ? dbType : DbType.Date);

                        //remove the param
                        data_param.Remove(propName + "___from");
                        data_param.Remove(propName + "___to");
                    }
                    else
                    {
                        DbType typ = DbType.Date;
                        if (prop?.GetCustomAttributes(typeof(TimeOnlyAttribute), true).Length > 0)
                        {
                            typ = DbType.DateTime;
                            clause = $"{append}{DbExp.TimeOnlyTruncate(propName)} = @{DbExp.TimeOnlyTruncate(propName)}";
                            data_param[propName] = DbExp.ToDbTime(data_param.GetValueAsString(propName));
                        }
                        else
                        {
                            clause = string.Format("{1}{0} = @{0}"
                            , DbExp.DateTruncate(propName), append);
                        }


                        if (cmd.CommandType == MyroCommandTypes.SqlBuilder)
                        {
                            DynamicParameters param = new DynamicParameters();
                            param.Add(propName, data_param[propName], typ);
                            cmd.SqlBuilder.Where(clause, param);
                        }
                        else
                        {
                            cmd.SqlString.AppendLine(" AND " + clause);
                            cmd.Parameters.Add("@" + propName, data_param[propName], typ);
                        }
                        data_param.Remove(propName);
                    }
                    break;
                case BaseDataTypes.DecimalNumber:
                case BaseDataTypes.Number:
                    if (data_param.ContainsKeyStartWith(propName + "___"))
                    {
                        //between
                        cmd = DbServiceUtility.GetBetweenClauseInteger(cmd, propName
                            , Conversion.ToString(data_param[propName + "___from"])
                            , Conversion.ToString(data_param[propName + "___to"])
                            , tableAlias);

                        //remove the param
                        data_param.Remove(propName + "___from");
                        data_param.Remove(propName + "___to");
                    }
                    else
                    {
                        if (searchType == SearchTypes.NotEqual)
                            clause = string.Format("{1}{0} <> @{0}", propName, append); //not equal for duplicagte value check in edit mode
                        else if (searchType == SearchTypes.IN_Search)
                        {

                            clause = string.Format("{0} IN ({1}) ", column, data_param[propName]);
                            data_param.Remove(propName); //TODO:Shivashwor 24 Oct 2016 

                            //clause = string.Format("{0} IN ( @{1} ) ", column, propName);

                            //DynamicParameters param = new DynamicParameters();
                            //param.Add(propName, data_param.GetValueAsBoolean(propName), dbType);
                            //cmd.SqlBuilder.Where(clause, param);

                            //data_param.Remove(propName);
                        }
                        else
                            clause = string.Format("{1}{0} = @{0}", propName, append);
                        //{
                        //    if (propName.ToLower().Trim() == "client_id")
                        //        clause = string.Format("({1}{0} = @{0} or client_id=1) ", propName, append);
                        //    else
                        //        clause = string.Format("{1}{0} = @{0}", propName, append);
                        //}

                        if (cmd.CommandType == MyroCommandTypes.SqlBuilder)
                        {
                            DynamicParameters param = new DynamicParameters();

                            if (data_param.ContainsKey(propName))
                                param.Add(propName, data_param[propName], dbType);

                            cmd.SqlBuilder.Where(clause, param);
                        }
                        else
                        {
                            cmd.SqlString.AppendLine(" AND " + clause);
                            cmd.Parameters.Add("@" + propName, data_param[propName], dbType);
                        }
                        data_param.Remove(propName);
                    }
                    break;
            }
            return cmd;
        }
        public static BangoCommand BindParameter(BangoCommand cmd, PropertyInfo prop
            , DynamicDictionary data_param, string tableAlias = null
            , SearchTypes searchType = SearchTypes.Like, string actualFieldName = null, bool checkNull = false)
        {
            return BindParameter(cmd, prop.Name, data_param, GetDbType(prop.PropertyType, prop), tableAlias, searchType, actualFieldName, checkNull, prop);
        }
        public static BangoCommand BindParameter2(BangoCommand cmd, PropertyInfo prop
            , DynamicDictionary data_param, string tableAlias = null
            , SearchTypes searchType = SearchTypes.Like)
        {

            throw new Exception("Use BindParameter");
            string append = GetTableAliasForColumn(tableAlias);

            string actualFieldName = string.Empty;
            if (prop.GetCustomAttribute<ActualFieldName>() != null)
            {
                actualFieldName = prop.GetCustomAttribute<ActualFieldName>().FieldName;
            }
            //if the property doesnot exists in the data parameter then continue.
            if (data_param.ContainsKeyStartWith(prop.Name.ToLower()) == true
                || (actualFieldName.Trim().Length > 0 &&
                    data_param.ContainsKeyStartWith(actualFieldName + "___")) == true
                || data_param.ContainsKeyStartWith(prop.Name.ToLower() + "___")
                )
            {
            }
            else
            {
                return cmd;
            }
            string propName = prop.Name.ToLower()
                , column = string.Format("{0}{1}", append, prop.Name)
                , clause = string.Empty;


            switch (ConvertTypeCodeToBaseType(prop.PropertyType))
            {
                case BaseDataTypes.String:
                    //ActualFieldName actualField = prop.GetCustomAttribute<ActualFieldName>();
                    //string actualFieldName = actualField == null ? string.Empty : actualField.FieldName;
                    if ((searchType & SearchTypes.CaseSensetive) != SearchTypes.CaseSensetive)
                    {
                        column = string.Format("lower({0})", column);
                        data_param[propName] = data_param[propName].ToString().ToLower();
                    }

                    if (data_param.GetValueAsString(propName).Trim().Length == 0)
                    {
                        break;
                    }

                    if (data_param.ContainsKeyStartWith(actualFieldName + "___multi"))
                    {
                        propName = actualFieldName + "___multi";

                        switch (searchType)
                        {
                            case SearchTypes.StartWith:
                            case SearchTypes.EndWith:
                            case SearchTypes.Like:
                                clause = string.Format(" ({2}{0}_en like @{1} OR {2}{0}_np like @{1})", actualFieldName, propName, append);
                                break;
                            case SearchTypes.Equal:
                                clause = string.Format(" ({2}{0}_en = @{1} OR {2}{0}_np = @{1})", actualFieldName, propName, append);
                                break;
                        }
                    }
                    else
                    {
                        switch (searchType)
                        {
                            case SearchTypes.StartWith:
                            case SearchTypes.EndWith:
                            case SearchTypes.Like:
                                clause = string.Format("{0} like @{1}", column, propName);
                                break;
                            case SearchTypes.Equal:
                                clause = string.Format("{0} = @{1}", column, propName);
                                break;
                        }
                    }



                    if (cmd.CommandType == MyroCommandTypes.SqlBuilder)
                    {
                        DynamicParameters param = new DynamicParameters();
                        switch (searchType)
                        {
                            case SearchTypes.StartWith:
                                param.Add(propName, data_param.GetValueAsString(propName) + "%");
                                break;
                            case SearchTypes.EndWith:
                                param.Add(propName, "%" + data_param.GetValueAsString(propName));
                                break;
                            case SearchTypes.Like:
                                param.Add(propName, "%" + data_param.GetValueAsString(propName) + "%");
                                break;
                            case SearchTypes.Equal:
                                param.Add(propName, data_param[propName]);
                                break;
                        }
                        cmd.SqlBuilder.Where(clause, param);
                    }
                    else
                    {
                        //value or parameter
                        cmd.SqlString.AppendLine(" AND " + clause);
                        switch (searchType)
                        {
                            case SearchTypes.StartWith:
                                cmd.Parameters.Add("@" + propName, data_param.GetValueAsString(propName) + "%");
                                break;
                            case SearchTypes.EndWith:
                                cmd.Parameters.Add("@" + propName, "%" + data_param.GetValueAsString(propName));
                                break;
                            case SearchTypes.Like:
                                cmd.Parameters.Add("@" + propName, "%" + data_param.GetValueAsString(propName) + "%");
                                break;
                            case SearchTypes.Equal:
                                cmd.Parameters.Add("@" + propName, data_param[propName]);
                                break;
                        }
                    }
                    //remove the param
                    data_param.Remove(propName);
                    break;
                case BaseDataTypes.Boolean:
                    //TODO 
                    cmd.Parameters.Add("@" + propName, data_param.GetValueAsBoolean(propName), DbType.Boolean);
                    data_param.Remove(propName);
                    break;
                case BaseDataTypes.DateTime:
                    //TODO
                    if (data_param.ContainsKeyStartWith(propName + "___"))
                    {
                        //between
                        cmd = DbServiceUtility.GetBetweenClauseDate(cmd, propName
                            , Conversion.ToString(data_param[propName + "___from"])
                            , Conversion.ToString(data_param[propName + "___to"])
                            , tableAlias);

                        //remove the param
                        data_param.Remove(propName + "___from");
                        data_param.Remove(propName + "___to");
                    }
                    else
                    {
                        clause = string.Format("{1}{0} = @{0}"
                            , DbExp.DateTruncate(propName), append);
                        if (cmd.CommandType == MyroCommandTypes.SqlBuilder)
                        {
                            DynamicParameters param = new DynamicParameters();
                            param.Add(propName, data_param[propName], DbType.Date);
                            cmd.SqlBuilder.Where(clause, param);
                        }
                        else
                        {
                            cmd.SqlString.AppendLine(" AND " + clause);
                            cmd.Parameters.Add("@" + propName, data_param[propName], DbType.Date);
                        }
                        data_param.Remove(propName);
                    }
                    break;
                case BaseDataTypes.DecimalNumber:
                case BaseDataTypes.Number:
                    if (data_param.ContainsKeyStartWith(propName + "___"))
                    {
                        //between
                        cmd = DbServiceUtility.GetBetweenClauseInteger(cmd, propName
                            , Conversion.ToString(data_param[propName + "___from"])
                            , Conversion.ToString(data_param[propName + "___to"])
                            , tableAlias);

                        //remove the param
                        data_param.Remove(propName + "___from");
                        data_param.Remove(propName + "___to");
                    }
                    else
                    {
                        clause = string.Format("{1}{0} = @{0}", propName, append);
                        if (cmd.CommandType == MyroCommandTypes.SqlBuilder)
                        {
                            DynamicParameters param = new DynamicParameters();
                            param.Add(propName, data_param[propName]);
                            cmd.SqlBuilder.Where(clause, param);
                        }
                        else
                        {
                            cmd.SqlString.AppendLine(" AND " + clause);
                            cmd.Parameters.Add("@" + propName, data_param[propName]);
                        }
                        data_param.Remove(propName);
                    }
                    break;
            }
            return cmd;
        }
        public static string GetTableAliasForColumn(string tableAlias)
        {
            if (tableAlias == null)
                tableAlias = string.Empty;

            string append = string.Empty;//, append2 = string.Empty;
            tableAlias = tableAlias.Trim();
            if (tableAlias.EndsWith("."))
            {
                return tableAlias;
            }
            if (tableAlias.Trim().Length > 0)
            {
                append = tableAlias + ".";
            }
            return append;
        }

        public static string SetColumnAlias(string tableAlias, string columns)
        {
            string fnl = string.Empty;
            string[] arr = columns.Split(',');
            if (arr?.Length > 0)
            {
                string append = GetTableAliasForColumn(tableAlias);
                for (int i = 0; i < arr.Length; i++)
                {
                    if (!arr[i].Contains("."))
                    {
                        arr[i] = append + arr[i];
                    }
                }
                columns = string.Join(",", arr);
            }
            return columns;
        }

        public static string GetTableAliasForTable(string tableAlias)
        {
            if (tableAlias == null)
                tableAlias = string.Empty;
            return tableAlias;
        }
        public static BangoCommand BindParameters(BangoCommand cmd, Models.IModel model
            , Base.List.DynamicDictionary data_param, string tableAlias = null
            , SearchTypes searchType = SearchTypes.Like)
        {

            if (data_param.GetCount() == 0)
                return cmd;
            string append = GetTableAliasForColumn(tableAlias);
            string actualFieldName = string.Empty;
            foreach (PropertyInfo prop in model.GetType().GetProperties())
            {
                actualFieldName = string.Empty;
                if (prop.GetCustomAttribute<ActualFieldName>() != null)
                {
                    actualFieldName = prop.GetCustomAttribute<ActualFieldName>().FieldName;
                }
                else
                {
                    actualFieldName = prop.Name.ToLower();
                }

                //if the property doesnot exists in the data parameter then continue.
                if ((data_param.ContainsKey(actualFieldName)
                    || data_param.ContainsKeyStartWith(actualFieldName + "___")
                    ) == true)
                {
                    cmd = BindParameter(cmd, prop, data_param, tableAlias, searchType, actualFieldName);
                }

            }
            return cmd;
        }

        public static BangoCommand BindClientIdParameter(BangoCommand cmd, Models.IModel model, string tableAlias = "c", bool DisplayMasterDataFromSystem = false)
        {
            PropertyInfo field_client_id = model.GetType().GetProperty("client_id");
            //if no field with the field client_id then just return command
            if (field_client_id == null)
                return cmd;

            TableDetailAttribute tableDetail = model.GetTableDetail();


            Dapper.Rainbow.IDbExpression dbExp = App.Container.GetInstance<Dapper.Rainbow.IDbExpression>();
            string append = DbServiceUtility.GetTableAliasForColumn(tableAlias);
            append = append + "client_id";
            BaseDataTypes btype = DbServiceUtility.ConvertTypeCodeToBaseType(field_client_id.PropertyType);
            //int client_id = Conversion.ToInt32();
            string condition = string.Empty;

            switch (btype)
            {
                case BaseDataTypes.Number:
                case BaseDataTypes.DecimalNumber:
                    condition = $"({append}) = {SessionData.client_id}";
                    if (DisplayMasterDataFromSystem)
                        condition = $"({condition} OR {append} = 1)";
                    break;
                case BaseDataTypes.Guid:
                case BaseDataTypes.String:
                    condition = $"({append}) = '{SessionData.client_id}'";
                    if (DisplayMasterDataFromSystem)
                        condition = $"({condition} OR {append} = 1)";
                    break;
                default:
                    throw new Exception("invalid type");
            }
            if (cmd.CommandType == MyroCommandTypes.SqlBuilder)
            {
                cmd.SqlBuilder.Where(condition);
            }
            else
            {
                if (cmd.SqlStringHasWhere)
                    cmd.SqlString.Append(" AND ");
                else
                    cmd.SqlString.Append(" WHERE ");
                cmd.SqlString.Append(condition);
            }
            return cmd;
        }
        public static BangoCommand BindDeleteParameter(BangoCommand cmd, Models.IModel model, string tableAlias = "c")
        {
            TableDetailAttribute tableDetail = model.GetTableDetail();
            if (tableDetail.DeleteFlagField.Trim().Length > 0)
            {
                PropertyInfo delFlag = Models.ModelService.GetDeleteFieldProperty(model);
                Dapper.Rainbow.IDbExpression dbExp = App.Container.GetInstance<Dapper.Rainbow.IDbExpression>();
                string append = DbServiceUtility.GetTableAliasForColumn(tableAlias);
                if (delFlag != null)
                {
                    BaseDataTypes btype = DbServiceUtility.ConvertTypeCodeToBaseType(delFlag.PropertyType);
                    if (cmd.CommandType == MyroCommandTypes.SqlBuilder)
                    {
                        switch (btype)
                        {
                            case BaseDataTypes.Boolean:
                                cmd.SqlBuilder.Where(string.Format("({0}) = false", dbExp.IfNull(append + tableDetail.DeleteFlagField, false)));
                                break;
                            case BaseDataTypes.Number:
                            case BaseDataTypes.DecimalNumber:
                                cmd.SqlBuilder.Where(string.Format("({0}) = 0", dbExp.IfNull(append + tableDetail.DeleteFlagField, 0)));
                                break;
                            case BaseDataTypes.Guid:
                            case BaseDataTypes.String:
                                cmd.SqlBuilder.Where(string.Format("({0}) = ''", dbExp.IfNull(append + tableDetail.DeleteFlagField, "")));
                                break;
                            default:
                                throw new Exception("invalid type");
                        }
                    }
                    else
                    {
                        if (cmd.SqlStringHasWhere)
                            cmd.SqlString.Append(" AND ");
                        else
                            cmd.SqlString.Append(" WHERE ");
                        switch (btype)
                        {
                            case BaseDataTypes.Boolean:
                                cmd.SqlString.Append(string.Format(" ({0}) = false", dbExp.IfNull(append + tableDetail.DeleteFlagField, false)));
                                break;
                            case BaseDataTypes.Number:
                            case BaseDataTypes.DecimalNumber:
                                cmd.SqlString.Append(string.Format(" ({0}) = 0", dbExp.IfNull(append + tableDetail.DeleteFlagField, 0)));
                                break;
                            case BaseDataTypes.Guid:
                            case BaseDataTypes.String:
                                cmd.SqlString.Append(string.Format(" ({0}) = ''", dbExp.IfNull(append + tableDetail.DeleteFlagField, "")));
                                break;
                            default:
                                throw new Exception("invalid type");
                        }
                    }

                }

            }
            return cmd;
        }
        public static DynamicParameters ToDynamicParameter(dynamic param)
        {
            DynamicParameters para = new DynamicParameters();
            para.AddDynamicParams(param);
            return para;
        }
        public static DynamicParameters ToDynamicParameter(string name, object value, DbType? dbType = null)
        {
            DynamicParameters para = new DynamicParameters();
            para.Add(name, value, dbType);
            return para;
        }

        public static DynamicParameters ToDynamicParameter(string name, object value, Type type)
        {
            DynamicParameters para = new DynamicParameters();
            para.Add(name, value, TypeMap[type]);
            return para;
        }
        delegate object BetweenColDelegate(string s);
        private static BangoCommand GetBetweenClause(BangoCommand cmd, string fieldName, object valueFrom, object valueTo, string tableAlias, DbType dbType = DbType.Int32)
        {
            string append = GetTableAliasForColumn(tableAlias);
            string valueFromStr = string.Empty, valueToStr = string.Empty;
            string fieldNameFrom = fieldName + "___from", fieldNameTo = fieldName + "___to";
            BetweenColDelegate betweenCol;
            switch (ConvertDbTypeCodeToBaseType(dbType))
            {
                case BaseDataTypes.Number:
                    betweenCol = delegate (string val)
                    {
                        return val;
                        //return Conversion.ToInt32(val);
                    };
                    valueFrom = Conversion.ToInt32(valueFrom);
                    valueTo = Conversion.ToInt32(valueTo);
                    valueFromStr = Conversion.ToString(valueFrom).Trim();
                    valueToStr = Conversion.ToString(valueTo).Trim();
                    break;
                case BaseDataTypes.DecimalNumber:
                    betweenCol = delegate (string val)
                    {
                        return val;
                        //return Conversion.ToDecimal(val);
                    };

                    valueFrom = (object)Conversion.ToDecimal(valueFrom);
                    valueTo = (object)Conversion.ToDecimal(valueTo);
                    valueFromStr = Conversion.ToString(valueFrom).Trim();
                    valueToStr = Conversion.ToString(valueTo).Trim();
                    break;
                case BaseDataTypes.DateTime:
                    switch (dbType)
                    {
                        case DbType.Time:
                            betweenCol = delegate (string val)
                            {
                                return DbExp.TimeOnlyTruncate(val);
                                //return DbExp.ToDbTime(val);
                                //return Conversion.ToDateTime(val);
                            };
                            if (valueFrom?.ToString().Length > 0)
                                valueFrom = DbExp.ToDbTime(Conversion.ToString(valueFrom));
                            if (valueTo?.ToString().Length > 0)
                                valueTo = DbExp.ToDbTime(Conversion.ToString(valueTo));
                            dbType = DbType.DateTime;
                            break;
                        default:
                            betweenCol = delegate (string val)
                            {
                                return DbExp.DateTruncate(val);
                                //return Conversion.ToDateTime(val);
                            };
                            DateTime tmp = Conversion.ToDateTime(valueFrom);
                            if (tmp.Year > 1000)
                                valueFrom = (object)tmp;
                            else
                                valueFrom = string.Empty;
                            tmp = Conversion.ToDateTime(valueTo);
                            if (tmp.Year > 1000)
                                valueTo = (object)tmp;
                            else
                                valueTo = string.Empty;
                            break;
                    }

                    valueFromStr = Conversion.ToString(valueFrom).Trim();
                    valueToStr = Conversion.ToString(valueTo).Trim();
                    break;
                case BaseDataTypes.String:
                    betweenCol = delegate (string val)
                    {
                        return val;
                        //return Conversion.ToDecimal(val);
                    };
                    valueFromStr = Conversion.ToString(valueFrom).Trim();
                    valueToStr = Conversion.ToString(valueTo).Trim();
                    break;
                default:
                    throw new NotSupportedException("DbType not nupport. passed DbType is = {0}" + dbType.ToString());
                    break;
            }
            //string valueFromStr = Conversion.ToString(valueFrom).Trim(),
            //    valueToStr = Conversion.ToString(valueTo).Trim();
            if (valueFromStr.Length == 0 && valueToStr.Length == 0)
                return cmd;
            StringBuilder sb = new StringBuilder();
            if (valueFromStr.Trim().Length > 0 && valueToStr.Trim().Length > 0)
            {
                //sb.AppendFormat(" ({0} >= {1} AND {0} <= {2}) ", fieldName, valueFrom, valueTo);
                sb.AppendFormat(" ({3}{0} >= @{1} AND {3}{0} <= @{2}) "
                    , betweenCol(fieldName)
                    , betweenCol(fieldName + "___from")
                    , betweenCol(fieldName + "___to")
                    , append);
                if (cmd.CommandType == MyroCommandTypes.SqlBuilder)
                {

                    DynamicParameters p = ToDynamicParameter(string.Format("@{0}___from", fieldName), valueFrom, dbType);
                    p.Add(string.Format("@{0}___to", fieldName), valueTo, dbType);
                    cmd.SqlBuilder.Where(sb.ToString(), p);
                }
                else
                {
                    cmd.SqlString.AppendLine(" AND " + sb.ToString());
                    cmd.Parameters.Add(string.Format("@{0}___from", fieldName), valueFrom, dbType);
                    cmd.Parameters.Add(string.Format("@{0}___to", fieldName), valueTo, dbType);
                }
            }
            else
            {
                if (valueFromStr.Length > 0)
                {
                    sb.AppendFormat(" ({2}{0} >= @{1}) "
                        , betweenCol(fieldName)
                        , betweenCol(fieldName + "___from")
                        , append);
                    if (cmd.CommandType == MyroCommandTypes.SqlBuilder)
                    {
                        DynamicParameters p = ToDynamicParameter(string.Format("@{0}___from", fieldName), valueFrom, dbType);
                        cmd.SqlBuilder.Where(sb.ToString(), p);
                    }
                    else
                    {
                        cmd.SqlString.AppendLine(" AND " + sb.ToString());
                        cmd.Parameters.Add(string.Format("@{0}___from", fieldName), valueFrom, dbType);
                    }
                }
                else if (valueToStr.Length > 0)
                {
                    sb.AppendFormat(" ({2}{0} <= @{1}) "
                        , betweenCol(fieldName)
                        , betweenCol(fieldName + "___to")
                        , append);
                    if (cmd.CommandType == MyroCommandTypes.SqlBuilder)
                    {

                        DynamicParameters p = ToDynamicParameter(string.Format("@{0}___to", fieldName), valueTo, dbType);
                        cmd.SqlBuilder.Where(sb.ToString(), p);
                    }
                    else
                    {
                        cmd.SqlString.AppendLine(" AND " + sb.ToString());
                        cmd.Parameters.Add(string.Format("@{0}___to", fieldName), valueTo, dbType);
                    }
                }
            }
            return cmd;
        }
        public static BangoCommand GetBetweenClauseInteger(BangoCommand cmd, string fieldName, string valueFrom, string valueTo, string tableAlias)
        {
            return GetBetweenClause(cmd, fieldName, valueFrom, valueTo, tableAlias, DbType.Int32);
        }

        public static BangoCommand GetBetweenClauseDate(BangoCommand cmd, string fieldName, string dateFrom, string dateTo, string tableAlias)
        {
            return GetBetweenClause(cmd, fieldName, dateFrom, dateTo, tableAlias, DbType.Date);
        }
        public static BangoCommand GetBetweenClauseTime(BangoCommand cmd, string fieldName, string dateFrom, string dateTo, string tableAlias)
        {
            return GetBetweenClause(cmd, fieldName, dateFrom, dateTo, tableAlias, DbType.Time);
        }

        public static BaseDataTypes ConvertTypeCodeToBaseType(Type type)
        {
            DbType dbType = new DbType();

            //TODO:SHIVA
            if (TypeMap.ContainsKey(type))
                dbType = TypeMap[type];

            return ConvertDbTypeCodeToBaseType(dbType);
        }

        public static BaseDataTypes ConvertDbTypeCodeToBaseType(DbType dbType)
        {

            BaseDataTypes bType = BaseDataTypes.Other;
            switch (dbType)
            {
                case DbType.Byte:
                case DbType.Int16:
                case DbType.UInt16:
                case DbType.Int32:
                case DbType.UInt32:
                case DbType.Int64:
                case DbType.UInt64:
                case DbType.SByte:
                    bType = BaseDataTypes.Number;
                    break;
                case DbType.Single:
                case DbType.Double:
                case DbType.Decimal:
                    bType = BaseDataTypes.DecimalNumber;
                    break;
                case DbType.Boolean:
                    bType = BaseDataTypes.Boolean;
                    break;
                case DbType.String:
                case DbType.StringFixedLength:
                    bType = BaseDataTypes.String;
                    break;
                case DbType.Guid:
                    bType = BaseDataTypes.Guid;
                    break;
                case DbType.Time:
                case DbType.Date:
                case DbType.DateTime:
                case DbType.DateTimeOffset:
                    bType = BaseDataTypes.DateTime;
                    break;
                case DbType.Binary:
                    bType = BaseDataTypes.Binary;
                    break;
                default:
                    bType = BaseDataTypes.Other;
                    break;
            }

            return bType;
        }

        private static void InitTypeMap()
        {
            _typeMap = new Dictionary<Type, DbType>();
            _typeMap.Clear();

            //if (!_typeMap.ContainsKey(typeof(byte)))
            if (!_typeMap.ContainsKey(typeof(byte)))
                _typeMap[typeof(byte)] = DbType.Byte;
            if (!_typeMap.ContainsKey(typeof(sbyte)))
                _typeMap[typeof(sbyte)] = DbType.SByte;
            if (!_typeMap.ContainsKey(typeof(short)))
                _typeMap[typeof(short)] = DbType.Int16;
            if (!_typeMap.ContainsKey(typeof(ushort)))
                _typeMap[typeof(ushort)] = DbType.UInt16;
            if (!_typeMap.ContainsKey(typeof(int)))
                _typeMap[typeof(int)] = DbType.Int32;
            if (!_typeMap.ContainsKey(typeof(uint)))
                _typeMap[typeof(uint)] = DbType.UInt32;
            if (!_typeMap.ContainsKey(typeof(long)))
                _typeMap[typeof(long)] = DbType.Int64;
            if (!_typeMap.ContainsKey(typeof(ulong)))
                _typeMap[typeof(ulong)] = DbType.UInt64;
            if (!_typeMap.ContainsKey(typeof(float)))
                _typeMap[typeof(float)] = DbType.Single;
            if (!_typeMap.ContainsKey(typeof(double)))
                _typeMap[typeof(double)] = DbType.Double;
            if (!_typeMap.ContainsKey(typeof(decimal)))
                _typeMap[typeof(decimal)] = DbType.Decimal;
            if (!_typeMap.ContainsKey(typeof(bool)))
                _typeMap[typeof(bool)] = DbType.Boolean;
            if (!_typeMap.ContainsKey(typeof(Boolean)))
                _typeMap[typeof(Boolean)] = DbType.Boolean;
            if (!_typeMap.ContainsKey(typeof(string)))
                _typeMap[typeof(string)] = DbType.String;
            if (!_typeMap.ContainsKey(typeof(String)))
                _typeMap[typeof(String)] = DbType.String;
            if (!_typeMap.ContainsKey(typeof(char)))
                _typeMap[typeof(char)] = DbType.StringFixedLength;
            if (!_typeMap.ContainsKey(typeof(Guid)))
                _typeMap[typeof(Guid)] = DbType.Guid;
            if (!_typeMap.ContainsKey(typeof(DateTime)))
                _typeMap[typeof(DateTime)] = DbType.DateTime;
            if (!_typeMap.ContainsKey(typeof(DateTimeOffset)))
                _typeMap[typeof(DateTimeOffset)] = DbType.DateTimeOffset;
            if (!_typeMap.ContainsKey(typeof(byte[])))
                _typeMap[typeof(byte[])] = DbType.Binary;
            if (!_typeMap.ContainsKey(typeof(byte?)))
                _typeMap[typeof(byte?)] = DbType.Byte;
            if (!_typeMap.ContainsKey(typeof(sbyte?)))
                _typeMap[typeof(sbyte?)] = DbType.SByte;
            if (!_typeMap.ContainsKey(typeof(short?)))
                _typeMap[typeof(short?)] = DbType.Int16;
            if (!_typeMap.ContainsKey(typeof(ushort?)))
                _typeMap[typeof(ushort?)] = DbType.UInt16;
            if (!_typeMap.ContainsKey(typeof(int?)))
                _typeMap[typeof(int?)] = DbType.Int32;
            if (!_typeMap.ContainsKey(typeof(uint?)))
                _typeMap[typeof(uint?)] = DbType.UInt32;
            if (!_typeMap.ContainsKey(typeof(long?)))
                _typeMap[typeof(long?)] = DbType.Int64;
            if (!_typeMap.ContainsKey(typeof(ulong?)))
                _typeMap[typeof(ulong?)] = DbType.UInt64;
            if (!_typeMap.ContainsKey(typeof(float?)))
                _typeMap[typeof(float?)] = DbType.Single;
            if (!_typeMap.ContainsKey(typeof(double?)))
                _typeMap[typeof(double?)] = DbType.Double;
            if (!_typeMap.ContainsKey(typeof(decimal?)))
                _typeMap[typeof(decimal?)] = DbType.Decimal;
            if (!_typeMap.ContainsKey(typeof(bool?)))
                _typeMap[typeof(bool?)] = DbType.Boolean;
            if (!_typeMap.ContainsKey(typeof(Boolean?)))
                _typeMap[typeof(Boolean?)] = DbType.Boolean;
            if (!_typeMap.ContainsKey(typeof(char?)))
                _typeMap[typeof(char?)] = DbType.StringFixedLength;
            if (!_typeMap.ContainsKey(typeof(Guid?)))
                _typeMap[typeof(Guid?)] = DbType.Guid;
            if (!_typeMap.ContainsKey(typeof(DateTime?)))
                _typeMap[typeof(DateTime?)] = DbType.DateTime;
            if (!_typeMap.ContainsKey(typeof(DateTimeOffset?)))
                _typeMap[typeof(DateTimeOffset?)] = DbType.DateTimeOffset;
            //TypeMap[typeof(System.Data.Linq.Binary)] = DbType.Binary;
        }

        public static string SafeDBString(string inputValue)
        {
            if (inputValue == null)
                return string.Empty;
            //return "'" + inputValue.Replace("'", "''") + "'";
            return inputValue.Replace("'", "''");
        }

        public static List<TModel> ExecuteList<TModel>(DbConnect con, string sql, DynamicParameters finalParameters = null)
        {
            if (sql.Length > 0)
            {
                IEnumerable<TModel> items = null;
                finalParameters = finalParameters == null ? new DynamicParameters() : finalParameters;
                try
                {
                    items = con.DB.Query<TModel>(sql, finalParameters, true);
                }
                catch (Npgsql.NpgsqlException ex)
                {
                    LogTrace.WriteErrorLog(ex.ToString());
                    LogTrace.WriteDebugLog(string.Format("Select SQL which gave exception:\r{0}", ex.Routine));
                    throw ex;
                }

                //Errors = con.DB.GetErros();
                if (items != null && items.Count() > 0)
                {
                    return items.ToList<TModel>();
                }
                return null;
            }
            throw new Exception("SQL not passed in the command.");
        }

        public static IEnumerable<DynamicDictionary> ExecuteList(DbConnect con, string sql, DynamicParameters finalParameters = null)
        {
            if (sql.Length > 0)
            {
                IEnumerable<SqlMapper.DapperRow> items = null;
                finalParameters = finalParameters == null ? new DynamicParameters() : finalParameters;
                try
                {
                    items = con.DB.Query<SqlMapper.DapperRow>(sql, finalParameters, true);
                }
                catch (Npgsql.NpgsqlException ex)
                {
                    LogTrace.WriteErrorLog(ex.ToString());
                    LogTrace.WriteDebugLog(string.Format("Select SQL which gave exception:\r{0}", ex.Routine));
                    throw ex;
                }

                //Errors = con.DB.GetErros();
                if (items != null && items.Count() > 0)
                {
                    return Conversion.ToDynamicDictionaryList(items);
                }
                return null;
            }
            throw new Exception("SQL not passed in the command.");
        }
        public static List<TModel> ExecuteList<TModel>(DbConnect con, BangoCommand cmd)
        {
            return ExecuteList<TModel>(con, cmd.FinalSql, cmd.FinalParameters);
        }

        public static IEnumerable<DynamicDictionary> ExecuteList(DbConnect con, BangoCommand cmd)
        {
            return ExecuteList(con, cmd.FinalSql, cmd.FinalParameters);
        }

        public static DynamicDictionary ExecuteItem(DbConnect con, string sql, DynamicParameters finalParameters = null)
        {
            if (sql.Length > 0)
            {
                IEnumerable<SqlMapper.DapperRow> items = null;
                finalParameters = finalParameters == null ? new DynamicParameters() : finalParameters;
                try
                {
                    items = con.DB.Query<SqlMapper.DapperRow>(sql, finalParameters, true);
                }
                catch (Npgsql.NpgsqlException ex)
                {
                    LogTrace.WriteErrorLog(ex.ToString());
                    LogTrace.WriteDebugLog(string.Format("Select SQL which gave exception:\r{0}", ex.Routine));
                    throw ex;
                }

                //Errors = con.DB.GetErros();
                if (items != null && items.Count() > 0)
                {
                    return Conversion.ToDynamicDictionary(items.FirstOrDefault());
                }
                return null;
            }
            throw new Exception("SQL not passed in the command.");
        }

        public static DynamicDictionary ExecuteItem(DbConnect con, BangoCommand cmd)
        {
            return ExecuteItem(con, cmd.FinalSql, cmd.FinalParameters);
        }

        public static DataTable ExecuteDataTable(string commandText)
        {
            DbConnect con = new DbConnect();
            Npgsql.NpgsqlDataAdapter dataAdapter = null;
            DataTable dt = new DataTable();
            try
            {
                dataAdapter = new Npgsql.NpgsqlDataAdapter(commandText, (Npgsql.NpgsqlConnection)con.Connection);
                //load the data into the dataTable.
                dt.BeginLoadData();
                dataAdapter.Fill(dt);
                dt.EndLoadData();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public static DataTable ExecuteDataTable(string commandText, DbConnect CON)
        {
            //DbConnect con = new DbConnect();

            Npgsql.NpgsqlDataAdapter dataAdapter = null;
            DataTable dt = new DataTable();
            try
            {
                dataAdapter = new Npgsql.NpgsqlDataAdapter(commandText, (Npgsql.NpgsqlConnection)CON.Connection);
                //load the data into the dataTable.
                dt.BeginLoadData();
                dataAdapter.Fill(dt);
                dt.EndLoadData();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }


        public static int PrepareTreeNodes<TNode>(IEnumerable<TNode> source, List<TNode> destination, int currentIndx, string[] fields_to_sum = null)
            where TNode : class, ITreeNode<TNode>, new()
        {
            int currentLvl = 0, nextLvl = 0, nextIndex = 0, totalItems = source.Count();
            TNode currentItem = source.ElementAt(currentIndx), nextItem = null;
            if (currentItem == null)
                return -1;

            currentLvl = Convert.ToInt32(Dynamic.InvokeGet(currentItem, "node_level"));
            nextIndex = currentIndx;
            do
            {
                //Dictionary<string, object> dic = ToDictionary(currentItem);
                destination.Add(currentItem);
                //fetch the next item
                nextIndex++;
                if (nextIndex >= totalItems)
                {
                    return totalItems;
                }
                nextItem = source.ElementAt(nextIndex);
                if (nextItem == null)
                {
                    return -1;
                }
                else
                {
                    nextLvl = Convert.ToInt32(Dynamic.InvokeGet(nextItem, "node_level"));
                    if (nextLvl < currentLvl)
                    {
                        if (currentItem.children_count > 0)
                            currentItem.leaf = false;
                        return nextIndex;
                        //exit the process;

                    }
                    else if (nextLvl > currentLvl)
                    {
                        //fetch child items
                        currentItem.children = new List<TNode>();

                        //dic["childs"] = children;
                        nextIndex = PrepareTreeNodes(source, currentItem.children, nextIndex, fields_to_sum);
                        //calculate total here.
                        if (fields_to_sum?.Length > 0 && currentItem.children_count > 0)
                        {
                            Dictionary<string, double> tots = new Dictionary<string, double>();
                            for (int i = 0, len = fields_to_sum.Length; i < len; i++)
                            {
                                tots.Add(fields_to_sum[i], 0);
                            }
                            foreach (TNode itm in currentItem.children)
                            {
                                for (int i = 0, len = fields_to_sum.Length; i < len; i++)
                                {
                                    tots[fields_to_sum[i]] += Convert.ToDouble(Dynamic.InvokeGet(itm, fields_to_sum[i]));
                                }
                            }
                            if (tots.Count > 0)
                            {
                                for (int i = 0, len = fields_to_sum.Length; i < len; i++)
                                {
                                    Dynamic.InvokeSet(currentItem, fields_to_sum[i], tots[fields_to_sum[i]]);
                                }
                            }

                        }


                        if (currentItem.children_count > 0)
                            currentItem.leaf = false;
                        if (nextIndex == -1 || nextIndex == totalItems)
                        {
                            return nextIndex;
                        }

                        nextItem = source.ElementAt(nextIndex);
                        nextLvl = Convert.ToInt32(Dynamic.InvokeGet(nextItem, "node_level"));
                        if (nextLvl < currentLvl)
                        {
                            return nextIndex;
                        }
                    }
                    else
                    {
                        //continue as same level
                    }
                }
                currentIndx = nextIndex;
                currentItem = nextItem;

            } while (1 == 1);

        }
    }
}