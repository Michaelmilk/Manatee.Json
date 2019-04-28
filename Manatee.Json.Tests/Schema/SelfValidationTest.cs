﻿using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Net;
using Manatee.Json.Schema;
using Manatee.Json.Serialization;
using NUnit.Framework;

namespace Manatee.Json.Tests.Schema
{
	[TestFixture]
	public class SelfValidationTest
	{
		public static IEnumerable TestData
		{
			get
			{
				yield return new TestCaseData(JsonSchema04.MetaSchema);
				yield return new TestCaseData(JsonSchema06.MetaSchema);
				yield return new TestCaseData(JsonSchema07.MetaSchema);
			}
		}
		
		[TestCaseSource(nameof(TestData))]
		public void Hardcoded(IJsonSchema schema)
		{
			var json = schema.ToJson(null);
			var validation = schema.Validate(json);

			validation.AssertValid();
		}

		[TestCaseSource(nameof(TestData))]
		public void Online(IJsonSchema schema)
		{
			try
			{
				// TODO: Catch web exceptions and assert inconclusive.
				var localSchemaJson = schema.ToJson(null);

				var onlineSchemaText = JsonSchemaOptions.Download(schema.Id);
				var onlineSchemaJson = JsonValue.Parse(onlineSchemaText);
				var onlineSchema = JsonSchemaFactory.FromJson(onlineSchemaJson);

				var localValidation = schema.Validate(onlineSchemaJson);
				var onlineValidation = onlineSchema.Validate(localSchemaJson);

				Assert.AreEqual(onlineSchema, schema);

				onlineValidation.AssertValid();
				localValidation.AssertValid();

				Assert.AreEqual(onlineSchemaJson, localSchemaJson);

			}
			catch (WebException)
			{
				Assert.Inconclusive();
			}
			catch (AggregateException e)
			{
				if (e.InnerExceptions.OfType<WebException>().Any())
					Assert.Inconclusive();
				throw;
			}
		}


		[Test]
		public void ValidateIgn()
		{
			var schemaContent = File.ReadAllText(@"E:\code\github\DataIngestionEngine\Src\Tests\DataIngestionEngine.Tests.Study\Resources\Manatee\ign_schema.json");
			var schemaValue = JsonValue.Parse(schemaContent);
			var schema = JsonSchemaFactory.FromJson(schemaValue);
			//schema.FromJson(schemaValue, new JsonSerializer());

			var payload = File.ReadAllText(@"E:\code\github\DataIngestionEngine\Src\Tests\DataIngestionEngine.Tests.Study\Resources\Manatee\ign.json");
			var json = JsonValue.Parse(payload);
			var validateResult = schema.Validate(json);
			var errors = validateResult.Errors;
			Assert.IsNotNull(validateResult);
		}
	}
}
