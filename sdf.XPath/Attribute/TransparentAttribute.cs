using System;

namespace sdf.XPath
{
	/// <summary>
	/// Explicitely specifies if decorated type member must be a transparent node 
	/// or not.
	/// </summary>
	[AttributeUsage( AttributeTargets.Property | AttributeTargets.Field )]
	public sealed class TransparentAttribute : Attribute
	{
		private bool _isTransparent;

		/// <summary>
		/// Creates a new <see cref="TransparentAttribute"/> instance.
		/// </summary>
		/// <param name="isTransparent">Is transparent.</param>
		public TransparentAttribute( bool isTransparent )
		{
			_isTransparent = isTransparent;
		}

		/// <summary>
		/// Gets a value indicating whether node must be transparent.
		/// </summary>
		/// <value>
		/// <see langword="true"/> if transparent node is required; otherwise, 
		/// <see langword="false"/>.
		/// </value>
		public bool IsTransparent
		{
			get { return _isTransparent; }
		}
	}

}
