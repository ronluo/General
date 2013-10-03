using System;
using System.Collections.Generic;
using System.Text;

namespace $NAMESPACE$
{
    public partial class _$OBJECTNAME$ : RaisingStudio.Data.Expressions.TableExpression, RaisingStudio.Data.Expressions.IQueryExpression<_$OBJECTNAME$>
    {
        #region Constructor (s) / Destructor
        /// <summary>
        /// Constructor (s) / Destructor.
        /// </summary>
        public _$OBJECTNAME$() : base("$OBJECTNAME$")
        {
        }
        #endregion
        #region Members
$$DATAOBJECTCOLUMNDEFINITION$$
		#endregion

        public static implicit operator RaisingStudio.Data.Expressions.ConditionExpression(_$OBJECTNAME$ $OBJECTNAME$)
        {
            return new RaisingStudio.Data.Expressions.ConditionExpression();
        }

        public RaisingStudio.Data.Expressions.ConditionExpression OrderBy(RaisingStudio.Data.Expressions.ColumnExpression columnExpression)
        {
             RaisingStudio.Data.Expressions.ConditionExpression conditionExpression = this;
             return conditionExpression.OrderBy(columnExpression);
        }

        public RaisingStudio.Data.Expressions.ConditionExpression OrderBy(RaisingStudio.Data.Expressions.ColumnExpression columnExpression, RaisingStudio.Data.Expressions.SortingDirection sortingDirection)
        {
            RaisingStudio.Data.Expressions.ConditionExpression conditionExpression = this;
            return conditionExpression.OrderBy(columnExpression, sortingDirection);
        }

        public RaisingStudio.Data.Expressions.ConditionExpression Where(RaisingStudio.Data.Expressions.ExpressionElement expressionElement)
        {
            RaisingStudio.Data.Expressions.ConditionExpression conditionExpression = this;
            return conditionExpression.Where(expressionElement);
        }

        public static implicit operator RaisingStudio.Data.Expressions.ColumnExpressionCollection(_$OBJECTNAME$ $OBJECTNAME$)
        {
            return new RaisingStudio.Data.Expressions.ColumnExpressionCollection($OBJECTNAME$);
        }

        public static implicit operator RaisingStudio.Data.Expressions.ColumnExpression[](_$OBJECTNAME$ $OBJECTNAME$)
        {
            return new RaisingStudio.Data.Expressions.ColumnExpression[]
                        {                            
$$OBJECTCOLUMNSDECLARE$$                        };
        }

        public RaisingStudio.Data.Expressions.ColumnExpressionCollection Except(params RaisingStudio.Data.Expressions.ColumnExpression[] columns)
        {
            RaisingStudio.Data.Expressions.ColumnExpressionCollection columnExpressionCollection = this;
            return columnExpressionCollection.Except(columns);
        }

    
        public static implicit operator RaisingStudio.Data.Expressions.QueryExpression<_$OBJECTNAME$>(_$OBJECTNAME$ $OBJECTNAME$)
        {
            return new RaisingStudio.Data.Expressions.QueryExpression<_$OBJECTNAME$>($OBJECTNAME$);
        }


        #region IQueryExpression<_$OBJECTNAME$> 成员

        _$OBJECTNAME$ RaisingStudio.Data.Expressions.IQueryExpression<_$OBJECTNAME$>.Value
        {
            get { return $OBJECTNAME$._; }
        }

        #endregion

        #region IQueryExpression 成员

        RaisingStudio.Data.Expressions.TableExpression RaisingStudio.Data.Expressions.IQueryExpression.Table
        {
            get { return this; }
        }

        RaisingStudio.Data.Expressions.ConditionExpression RaisingStudio.Data.Expressions.IQueryExpression.Condition
        {
            get { return this; }
        }

        RaisingStudio.Data.Expressions.ColumnExpressionCollection RaisingStudio.Data.Expressions.IQueryExpression.Columns
        {
            get { return null; }
        }

        #endregion
    }

    partial class $OBJECTNAME$
    {
        #region Members
        public static readonly _$OBJECTNAME$ _ = new _$OBJECTNAME$();
		#endregion
    }
}
