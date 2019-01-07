using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Manatee.Json.Patch;
using Manatee.Json.Serialization;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace Manatee.Json.Tests.Patch.TestSuite
{
	[TestFixture]
	public class JsonPatchTestSuite
	{
		private const string _testFolder = @"..\..\..\..\json-patch-tests";
		private static readonly JsonSerializer _serializer;

		// ReSharper disable once MemberCanBePrivate.Global
		public static IEnumerable TestData => _LoadTests();

		private static IEnumerable<TestCaseData> _LoadTests()
		{
			JsonOptions.DuplicateKeyBehavior = DuplicateKeyBehavior.Overwrite;

			var testsPath = System.IO.Path.Combine(TestContext.CurrentContext.WorkDirectory, _testFolder).AdjustForOS();
			var fileNames = Directory.GetFiles(testsPath, "*tests*.json");

			foreach (var fileName in fileNames)
			{
				var contents = File.ReadAllText(fileName);
				var json = JsonValue.Parse(contents);

				foreach (var test in json.Array)
				{
					var testDescription = test.Object.TryGetString("comment") ?? "UNNAMED TEST";
					var testName = testDescription.Replace(' ', '_');
					yield return new TestCaseData(fileName, test) {TestName = testName};
				}
			}

			JsonOptions.DuplicateKeyBehavior = DuplicateKeyBehavior.Throw;
		}

		static JsonPatchTestSuite()
		{
			_serializer = new JsonSerializer();
		}

		[TestCaseSource(nameof(TestData))]
		public void Run(string fileName, JsonValue testJson)
		{
			JsonPatchResult result = null;
			try
			{
				var schemaValidation = JsonPatchTest.Schema.Validate(testJson);
				if (!schemaValidation.IsValid)
				{
					foreach (var error in schemaValidation.NestedResults)
					{
						Console.WriteLine(error);
					}
					return;
				}
				var test = _serializer.Deserialize<JsonPatchTest>(testJson);

				result = test.Patch.TryApply(test.Doc);

				Assert.AreNotEqual(test.ExpectsError, result.Success);
				if (test.HasExpectedValue)
					Assert.AreEqual(test.ExpectedValue, result.Patched);
			}
			catch (Exception e)
			{
				Console.WriteLine("File name: {0}", fileName);
				Console.WriteLine("Test: {0}", testJson.GetIndentedString());
				Console.WriteLine("Exception: {0}", e.Message);
				Console.WriteLine(e.StackTrace);
				if (result != null)
					Console.WriteLine("Error: {0}", result.Error);
				if (testJson.Object.TryGetBoolean("disabled") ?? false)
				{
					// This is a serious hack.  The .Net Core version of NUnit tracks the assertions as they occur
					// and this is causing the disabled tests to fail.  To remedy this we need to clear the assertions
					// record before we assert inconclusive.
					// It seems this was automatically done with the .Net Framework version.
					(TestContext.CurrentContext.Result.Assertions as ICollection<AssertionResult>)?.Clear();
					Assert.Inconclusive("Test is disabled.  Ignoring...");
				}
				throw;
			}
		}
	}
}