﻿/***************************************************************************************

	Copyright 2012 Greg Dennis

	   Licensed under the Apache License, Version 2.0 (the "License");
	   you may not use this file except in compliance with the License.
	   You may obtain a copy of the License at

		 http://www.apache.org/licenses/LICENSE-2.0

	   Unless required by applicable law or agreed to in writing, software
	   distributed under the License is distributed on an "AS IS" BASIS,
	   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	   See the License for the specific language governing permissions and
	   limitations under the License.
 
	File Name:		JsonSerializationAbstractionMap.cs
	Namespace:		Manatee.Json.Serialization
	Class Name:		JsonSerializationAbstractionMap
	Purpose:		Maps interfaces and abstract classes to concrete classes and
					in order to provide instances during instantia.

***************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Manatee.Json.Enumerations;
using Manatee.Json.Exceptions;
using Manatee.Json.Serialization.Internal;

namespace Manatee.Json.Serialization
{
	/// <summary>
	/// Provides an interface to map abstract and interface types to
	/// concrete types for object instantiation during deserialization.
	/// </summary>
	public static class JsonSerializationAbstractionMap
	{
		private static readonly Dictionary<Type, Type> _registry;

		static JsonSerializationAbstractionMap()
		{
			_registry = new Dictionary<Type, Type>();
		}

		/// <summary>
		/// Applies a mapping from an abstraction to a concrete type.
		/// </summary>
		/// <typeparam name="TAbstract">The abstract type.</typeparam>
		/// <typeparam name="TConcrete">The concrete type.</typeparam>
		public static void Map<TAbstract, TConcrete>()
			where TConcrete : TAbstract, new()
		{
			if (typeof(TConcrete).IsAbstract || typeof(TConcrete).IsInterface)
				throw new JsonTypeMapException<TAbstract, TConcrete>();
			_registry[typeof (TAbstract)] = typeof (TConcrete);
		}
		/// <summary>
		/// Removes a previously-assigned mapping.
		/// </summary>
		/// <typeparam name="TAbstract">The type to remove.</typeparam>
		public static void RemoveMap<TAbstract>()
		{
			_registry.Remove(typeof (TAbstract));
		}
		/// <summary>
		/// Retrieves the map setting for an abstraction type.
		/// </summary>
		/// <param name="type">The abstraction type.</param>
		/// <returns>The mapped type if a mapping exists; otherwise the abstraction type.</returns>
		public static Type GetMap(Type type)
		{
			return _registry.ContainsKey(type) ? _registry[type] : type;
		}

		internal static T CreateInstance<T>(JsonValue json)
		{
			var type = typeof (T);
			if (type.IsAbstract || type.IsInterface)
			{
				if ((json.Type == JsonValueType.Object) && (json.Object.ContainsKey(Constants.TypeKey)))
				{
					var concrete = Type.GetType(json.Object[Constants.TypeKey].String);
					return (T) Activator.CreateInstance(concrete);
				}
				if (_registry.ContainsKey(type))
				{
					var concrete = _registry[type];
					return (T) Activator.CreateInstance(concrete);
				}
				if (type.IsInterface)
					return TypeGenerator.Default.Generate<T>();
			}
			return Activator.CreateInstance<T>();
		}
	}
}