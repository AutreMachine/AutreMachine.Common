using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AutreMachine.Common.Samples.PaginatedList
{

    public class TestElementContext : IOrderedQueryable<TestElement>
    {
        public TestElementContext(int capacity)
        {
            Provider = new TestElementProvider(capacity);
            Expression = Expression.Constant(this);
        }

        internal TestElementContext(IQueryProvider provider, Expression expression)
        {
            Provider = provider;
            Expression = expression;
        }

        public IEnumerator<TestElement> GetEnumerator()
        {
            return Provider.Execute<IEnumerable<TestElement>>(Expression).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Type ElementType
        {
            get { return typeof(TestElement); }
        }

        public Expression Expression { get; private set; }
        public IQueryProvider Provider { get; private set; }
    }

    public class TestElement
    {
        public string Name
        {
            get; set;
        }
    }

    public class TestElementProvider : IQueryProvider
    {
        private readonly int capacity;

        public TestElementProvider(int capacity)
        {
            this.capacity = capacity;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return new TestElementContext(this, expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return (IQueryable<TElement>)new TestElementContext(this, expression);
        }

        public object Execute(Expression expression)
        {
            return Execute<TestElement>(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            var isEnumerable = (typeof(TResult).Name == "IEnumerable`1");
            return (TResult)TestElementQueryContext.Execute(expression, isEnumerable, capacity);
        }
    }

    public class TestElementQueryContext
    {
        internal static object Execute(Expression expression, bool isEnumerable, int capacity)
        {
            var queryableElements = GetAllTestElement(capacity);

            // Copy the expression tree that was passed in, changing only the first
            // argument of the innermost MethodCallExpression.
            var treeCopier = new ExpressionTreeModifier(queryableElements);
            var newExpressionTree = treeCopier.Visit(expression);

            // This step creates an IQueryable that executes by replacing Queryable methods with Enumerable methods.
            if (isEnumerable)
            {
                return queryableElements.Provider.CreateQuery(newExpressionTree);
            }
            else
            {
                return queryableElements.Provider.Execute(newExpressionTree);
            }
        }

        private static IQueryable<TestElement> GetAllTestElement(int capacity)
        {
            var list = new List<TestElement>();
            for (var i = 0; i < capacity; i++)
                list.Add(new TestElement { Name = "Joe" + i.ToString() });

            return list.AsQueryable();
        }
    }

    public class ExpressionTreeModifier : ExpressionVisitor
    {
        private IQueryable<TestElement> queryablePlaces;

        public ExpressionTreeModifier(IQueryable<TestElement> places)
        {
            this.queryablePlaces = places;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            // Replace the constant QueryableTerraServerData arg with the queryable Place collection. 
            if (node.Type == typeof(TestElementContext))
                return Expression.Constant(this.queryablePlaces);
            else
                return node;
        }
    }
}
