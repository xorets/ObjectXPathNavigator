using System;
using System.Collections;

namespace sdf.XPath.NodePolicy
{
	/// <summary>
	/// Custom <see cref="INodePolicy"/> implementations.
	/// </summary>
	public class EnumerableNodePolicy : CollectionNodePolicyBase
	{
		private object[] _elements;

		/// <summary>
		/// Gets the node policy object.
		/// </summary>
		/// <returns>Returns an instance of this node policy.</returns>
		public new static INodePolicy GetPolicy()
		{
			return new EnumerableNodePolicy();
		}

		/// <summary>
		/// Creates a new <see cref="DictionaryNodePolicy"/> instance.
		/// </summary>
		protected EnumerableNodePolicy() {}

		/// <summary>
		/// See <see cref="INodePolicy.GetAttributesCount"/> for details.
		/// </summary>
		public override int GetAttributesCount( Node node )
		{
			return 0;
		}

		/// <summary>
		/// See <see cref="INodePolicy.GetChildrenCount"/> for details.
		/// </summary>
		public override int GetChildrenCount( Node node )
		{
			if( _elements == null )
				PrepareElements( node.Object );

			return _elements.Length;
		}

		/// <summary>
		/// See <see cref="INodePolicy.GetChild"/> for details.
		/// </summary>
		public override Node GetChild( Node node, int index )
		{
			if( _elements == null )
				PrepareElements( node.Object );

			Node memberNode = null;
			if( index >= 0 && index < _elements.Length )
				memberNode = CreateChildNode( _elements[index], node );

			return memberNode;
		}

		/// <summary>
		/// Prepares the enumerated elements.
		/// </summary>
		/// <param name="source">An <see cref="IEnumerable"/> or 
		/// <see cref="IEnumerator"/>.</param>
		private void PrepareElements( object source )
		{
			var enumerable = source as IEnumerable;
			IEnumerator enumerator;

			if( enumerable != null )
				enumerator = enumerable.GetEnumerator();
			else
				enumerator = source as IEnumerator;

			if( enumerator == null )
				throw new ArgumentException( "Argument 'source' must be either IEnumerator or IEnumerable object.", "source" );

			var elements = new ArrayList();
			while( enumerator.MoveNext() )
				elements.Add( enumerator.Current );

			_elements = elements.ToArray();
		}
	}
}