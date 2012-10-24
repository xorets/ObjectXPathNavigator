using System;

namespace sdf.XPath
{
	/// <summary>
	/// Provides data for <see cref="ObjectXPathContext.NodePolicyGet"/> event.
	/// </summary>
	public class NodePolicyGetEventArgs : EventArgs
	{
		private object _obj;
		private INodePolicy _policy;

		/// <summary>
		/// Creates a new <see cref="NodePolicyGetEventArgs"/> instance.
		/// </summary>
		/// <param name="obj">Object the policy is being created for.</param>
		/// <param name="policy">Proposed policy.</param>
		public NodePolicyGetEventArgs( object obj, INodePolicy policy )
		{
			_obj = obj;
			_policy = policy;
		}

		/// <summary>
		/// The object the policy is being obtained for.
		/// </summary>
		public object Obj
		{
			get { return _obj; }
		}

		/// <summary>
		/// The resulting policy.
		/// </summary>
		/// <remarks>
		/// When the event is fired, this property contains the reference to a policy
		/// which was selected by standard algorithm (or other consumers of this 
		/// event). Event handler could either specify other policy or leave it intact.
		/// </remarks>
		public INodePolicy Policy
		{
			get { return _policy; }
			set { _policy = value; }
		}
	}
}