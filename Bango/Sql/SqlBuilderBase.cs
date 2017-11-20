using Myro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Myro.Cmd
{
    public class SqlBuilderBase<TModel> : ICmdBuilder
        where TModel : ModelBase, new()
    {
        #region Member Variables
        protected StringBuilder _sql = new StringBuilder();
        public StringBuilder Sql {
            get
            {
                return _sql;
            }
            set
            {
                _sql = value;
            }
        }
        public ModelBase Model { get; set; }
        public SqlTypes SqlType { get; set; }
        public IDbCommand Command { get; set; }
        #endregion Member Variables
        #region constructor / Distructor
        public SqlBuilderBase()
        {
        }
        #endregion constructor / Destructor
        #region events
        public event AfterSqlCreateEventHandler AfterQueryCreate;
        public event BeforeSqlCreateEventHandler BeforeQueryCreate;

        //public event BeforeClauseCreateEventHandler BeforeClauseCreate;
        //public event AfterClauseCreateEventHandler AfterClauseCreate;
        public event AfterClauseCreateEventHandler AfterColumnClauseCreate;
        public event AfterClauseCreateEventHandler AfterInsertValuesClauseCreate;
        public event AfterClauseCreateEventHandler AfterUpdateValuesClauseCreate;
        public event AfterClauseCreateEventHandler AfterFromClauseCreate;
        public event AfterClauseCreateEventHandler AfterWhereClauseCreate;
        public event AfterClauseCreateEventHandler AfterOrderByClauseCreate;

        public event BeforeClauseCreateEventHandler BeforeColumnClauseCreate;
        public event BeforeClauseCreateEventHandler BeforeInsertValuesClauseCreate;
        public event BeforeClauseCreateEventHandler BeforeUpdateValuesClauseCreate;
        public event BeforeClauseCreateEventHandler BeforeFromClauseCreate;
        public event BeforeClauseCreateEventHandler BeforeWhereClauseCreate;
        public event BeforeClauseCreateEventHandler BeforeOrderByClauseCreate;
        #endregion events
        #region functions/Menthods implementation
        public virtual IDbCommand BuildCmd()
        {
            return null;
        }
        public virtual IDbCommand GetCmd()
        {
            if (RaiseBeforeQueryCreateEvent(_sql) == false)
            {
                Command = BuildCmd();

                RaiseAfterQueryCreateEvent(_sql);
            }
            return Command;
        }
        public virtual IDbCommand GetCmd(ModelBase model)
        {
            Model = model;
            return GetCmd();
        }
        public void RaiseAfterQueryCreateEvent(StringBuilder sb)
        {
            if (AfterQueryCreate != null)
            {
                AfterSqlCreateEventArgs e = new AfterSqlCreateEventArgs(SqlType, sb);
                AfterQueryCreate(this, e);
                sb = e.Sql;
            }
        }
        public bool RaiseBeforeQueryCreateEvent(StringBuilder sb)
        {
            if (BeforeQueryCreate != null)
            {
                BeforeSqlCreateEventArgs e = new BeforeSqlCreateEventArgs(SqlType, sb);
                BeforeQueryCreate(this, e);
                sb = e.Sql;
                return e.Cancel;
            }
            return false;
        }
        public bool RaiseBeforeColumnClauseCreateEvent(StringBuilder clause)
        {
            if (BeforeColumnClauseCreate != null)
            {
                BeforeClauseCreateEventArgs e = new BeforeClauseCreateEventArgs(SqlType, clause);
                BeforeColumnClauseCreate(this, e);
                clause = e.SqlClause;
                return e.Cancel;
            }
            return false;
        }
        public bool RaiseBeforeInsertValuesClauseCreateEvent(StringBuilder clause)
        {
            if (BeforeInsertValuesClauseCreate != null)
            {
                BeforeClauseCreateEventArgs e = new BeforeClauseCreateEventArgs(SqlType, clause);
                BeforeInsertValuesClauseCreate(this, e);
                clause = e.SqlClause;
                return e.Cancel;
            }
            return false;
        }

        public bool RaiseBeforeUpdateValuesClauseCreateEvent(StringBuilder clause)
        {
            if (BeforeUpdateValuesClauseCreate != null)
            {
                BeforeClauseCreateEventArgs e = new BeforeClauseCreateEventArgs(SqlType, clause);
                BeforeUpdateValuesClauseCreate(this, e);
                clause = e.SqlClause;
                return e.Cancel;
            }
            return false;
        }
        public bool RaiseBeforeFromClauseCreateEvent(StringBuilder clause)
        {
            if (BeforeFromClauseCreate != null)
            {
                BeforeClauseCreateEventArgs e = new BeforeClauseCreateEventArgs(SqlType, clause);
                BeforeFromClauseCreate(this, e);
                clause = e.SqlClause;
                return e.Cancel;
            }
            return false;
        }
        public bool RaiseBeforeWhereClauseCreateEvent(StringBuilder clause)
        {
            if (BeforeWhereClauseCreate != null)
            {
                BeforeClauseCreateEventArgs e = new BeforeClauseCreateEventArgs(SqlType, clause);
                BeforeWhereClauseCreate(this, e);
                clause = e.SqlClause;
                return e.Cancel;
            }
            return false;
        }

        public bool RaiseBeforeOrderByClauseCreateEvent(StringBuilder clause)
        {
            if (BeforeOrderByClauseCreate != null)
            {
                BeforeClauseCreateEventArgs e = new BeforeClauseCreateEventArgs(SqlType, clause);
                BeforeOrderByClauseCreate(this, e);
                clause = e.SqlClause;
                return e.Cancel;
            }
            return false;
        }
        public void RaiseAfterColumnClauseCreateEvent(StringBuilder clause)
        {
            if (AfterColumnClauseCreate != null)
            {
                AfterClauseCreateEventArgs e = new AfterClauseCreateEventArgs(SqlType, clause);
                AfterColumnClauseCreate(this, e);
                clause = e.SqlClause;
            }
        }
        public void RaiseAfterInsertValuesClauseCreateEvent(StringBuilder clause)
        {
            if (AfterInsertValuesClauseCreate != null)
            {
                AfterClauseCreateEventArgs e = new AfterClauseCreateEventArgs(SqlType, clause);
                AfterInsertValuesClauseCreate(this, e);
                clause = e.SqlClause;
            }
        }
        public void RaiseAfterUpdateValuesClauseCreateEvent(StringBuilder clause)
        {
            if (AfterUpdateValuesClauseCreate != null)
            {
                AfterClauseCreateEventArgs e = new AfterClauseCreateEventArgs(SqlType, clause);
                AfterUpdateValuesClauseCreate(this, e);
                clause = e.SqlClause;
            }
        }
        public void RaiseAfterFromClauseCreateEvent(StringBuilder clause)
        {
            if (AfterFromClauseCreate != null)
            {
                AfterClauseCreateEventArgs e = new AfterClauseCreateEventArgs(SqlType, clause);
                AfterFromClauseCreate(this, e);
                clause = e.SqlClause;
            }
        }
        public void RaiseAfterWhereClauseCreateEvent(StringBuilder clause)
        {
            if (AfterWhereClauseCreate != null)
            {
                AfterClauseCreateEventArgs e = new AfterClauseCreateEventArgs(SqlType, clause);
                AfterWhereClauseCreate(this, e);
                clause = e.SqlClause;
            }
        }
        public void RaiseAfterOrderByClauseCreateEvent(StringBuilder clause)
        {
            if (AfterOrderByClauseCreate != null)
            {
                AfterClauseCreateEventArgs e = new AfterClauseCreateEventArgs(SqlType, clause);
                AfterOrderByClauseCreate(this, e);
                clause = e.SqlClause;
            }
        }
        public IDbCommand GetCommand()
        {
            return App.DB.GetConnection().GetCommand();
        }

        #endregion functions/Menthods implementation
    }
}
