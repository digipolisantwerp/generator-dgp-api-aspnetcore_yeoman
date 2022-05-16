using System.Linq.Expressions;

namespace StarterKit.DataAccess.Query
{
	public class ReplaceExpressionVisitor : ExpressionVisitor
	{
		private readonly Expression _newValue;
		private readonly Expression _oldValue;

		public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
		{
			_oldValue = oldValue;
			_newValue = newValue;
		}

		public override Expression Visit(Expression node)
		{
			if (node == _oldValue) return _newValue;
			return base.Visit(node);
		}
	}
}