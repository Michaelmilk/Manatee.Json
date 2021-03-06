﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Manatee.Json.Internal;
using Manatee.Json.Patch;
using Manatee.Json.Pointer;
using Manatee.Json.Schema;
using Manatee.Json.Serialization;
using Manatee.Json.Tests.Test_References;
using NUnit.Framework;

namespace Manatee.Json.Tests
{
	[TestFixture]
	// TODO: Add categories to exclude this test.
	[Ignore("This test fixture for development purposes only.")]
	public class DevTest
	{
		[Test]
		public void Test()
		{
			var serializer = new JsonSerializer();
			var target = new Container {Value = new Mass {Value = 5}};
			JsonValue expected = new JsonObject {["Value"] = new JsonObject {["Value"] = 5}};

			var actual = serializer.Serialize(target);

			Assert.AreEqual(expected, actual);
		}
	}
}