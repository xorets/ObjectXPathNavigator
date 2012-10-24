using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Reflection;

namespace sdf.XPath
{
	/// <summary>
	/// Class for creating node policies on request.
	/// </summary>
	internal class NodePolicyFactory
	{
		private static readonly ConcurrentDictionary<Type, PolicyInfo> PolicyInfoCache = new ConcurrentDictionary<Type, PolicyInfo>();
		private Hashtable _typePolicy;
		private ObjectXPathContext _context;

		/// <summary>
		/// Creates a new <see cref="NodePolicyFactory"/> instance.
		/// </summary>
		/// <param name="context">Context to which this factory will belong.</param>
		public NodePolicyFactory( ObjectXPathContext context )
		{
			_typePolicy = new Hashtable();
			_context = context;
		}

		/// <summary>
		/// Creates the policy of given type.
		/// </summary>
		/// <param name="policyType">Type of policy to create.</param>
		/// <returns>An instance of the requested policy type.</returns>
		/// <remarks>This could be either new instance, created especially for
		/// this request, or instance shared by other nodes. This behavior is
		/// solely determined by GetPolicy method of given policy type.</remarks>
		public INodePolicy CreatePolicy( Type policyType )
		{
			var pi = PolicyInfoCache.GetOrAdd( policyType, t => new PolicyInfo( t ) );
			return pi.GetNodePolicy();
		}

		/// <summary>
		/// Gets the policy for specific type of objects.
		/// </summary>
		/// <param name="forType">The type of objects which will be serverd by the
		/// policy.</param>
		/// <returns>An policy responsible for handling object of specified type.</returns>
		/// <remarks>This could be either new instance, created especially for
		/// this request, or instance shared by other nodes. This behavior is
		/// solely determined by GetPolicy method of given policy type.</remarks>
		public INodePolicy GetPolicy( Type forType )
		{
			// Look in the type policies
			var policyType = (Type)_typePolicy[forType];
			if( policyType == null )
			{
				// Look for policies defined for base types
				var baseType = forType.BaseType;
				while( baseType != null )
				{
					policyType = (Type)_typePolicy[baseType];
					if( policyType != null )
					{
						RegisterPolicy( forType, policyType );
						goto PolicyFound;
					}
					baseType = baseType.BaseType;
				}

				// Look for interfaces
				foreach( var ifaceType in forType.GetInterfaces() )
				{
					policyType = (Type)_typePolicy[ifaceType];
					if( policyType != null )
					{
						RegisterPolicy( forType, policyType );
						goto PolicyFound;
					}
				}

				// If no overrides found, look into type information
				policyType = _context.TypeInfoCache.GetTypeInfo( forType ).PolicyType;
			}

			PolicyFound:

			return CreatePolicy( policyType );
		}

		/// <summary>
		/// Registers the type of policies for handling object of specified type.
		/// </summary>
		/// <param name="forType">For which type of object this policy will be used.</param>
		/// <param name="policyType">Policy type.</param>
		public void RegisterPolicy( Type forType, Type policyType )
		{
			lock( _typePolicy.SyncRoot )
				_typePolicy[forType] = policyType;
		}

		private delegate INodePolicy GetNodePolicyDelegate();

		private class PolicyInfo
		{
			public GetNodePolicyDelegate GetNodePolicy { get; private set; }

			public PolicyInfo( Type policyType )
			{
				var getPolicy = policyType.GetMethod( "GetPolicy", BindingFlags.Static | BindingFlags.Public );
				if( getPolicy == null || getPolicy.GetParameters().Length > 0 )
					throw new ArgumentException( "NodePolicy type must have public static GetPolicy method.", "policyType" );

				GetNodePolicy = (GetNodePolicyDelegate)Delegate.CreateDelegate( typeof( GetNodePolicyDelegate ), getPolicy );
			}
		}
	}
}
