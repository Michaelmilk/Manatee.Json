﻿using System;
using System.Reflection;
using Manatee.Json.Internal;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal abstract class GenericTypeSerializerBase : IPrioritizedSerializer
	{
		private readonly MethodInfo _encodeMethod;
		private readonly MethodInfo _decodeMethod;

		public virtual int Priority => 2;

		public virtual bool ShouldMaintainReferences => false;

		protected GenericTypeSerializerBase()
		{
			_encodeMethod = GetType().GetTypeInfo().GetDeclaredMethod("_Encode");
			_decodeMethod = GetType().GetTypeInfo().GetDeclaredMethod("_Decode");
		}

		public abstract bool Handles(SerializationContext context);

		public JsonValue Serialize(SerializationContext context)
		{
			PrepSource(context);
			var typeArguments = GetTypeArguments(context.Source.GetType());
			var toJson = _encodeMethod;
			if (toJson.IsGenericMethod)
				toJson = toJson.MakeGenericMethod(typeArguments);

			return (JsonValue) toJson.Invoke(null, new object[] {context});
		}

		public object Deserialize(SerializationContext context)
		{
			var typeArguments = GetTypeArguments(context.InferredType);
			var fromJson = _decodeMethod;
			if (fromJson.IsGenericMethod)
				fromJson = fromJson.MakeGenericMethod(typeArguments);

			return fromJson.Invoke(null, new object[] {context});
		}

		protected virtual Type[] GetTypeArguments(Type type)
		{
			return type.GetTypeInfo().IsGenericType
				? type.GetTypeArguments()
				: new[] {type};
		}

		protected virtual void PrepSource(SerializationContext context) { }
	}
}