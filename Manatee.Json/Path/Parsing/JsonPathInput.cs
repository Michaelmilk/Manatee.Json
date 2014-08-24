﻿/***************************************************************************************

	Copyright 2014 Greg Dennis

	   Licensed under the Apache License, Version 2.0 (the "License");
	   you may not use this file except in compliance with the License.
	   You may obtain a copy of the License at

		 http://www.apache.org/licenses/LICENSE-2.0

	   Unless required by applicable law or agreed to in writing, software
	   distributed under the License is distributed on an "AS IS" BASIS,
	   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	   See the License for the specific language governing permissions and
	   limitations under the License.
 
	File Name:		JsonPathInput.cs
	Namespace:		Manatee.Json.Path.Parsing
	Class Name:		JsonPathInput
	Purpose:		DESCRIPTION

***************************************************************************************/

namespace Manatee.Json.Path.Parsing
{
	internal enum JsonPathInput
	{
		Dollar,
		OpenBracket,
		CloseBracket,
		Period,
		Star,
		OpenParenth,
		Question,
		Number,
		Letter,
		Colon,
		Comma,
		End
	}
}