//  Copyright 2017 The Nuclio Authors.
// 
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
// 
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using Nuclio.Sdk;
using Newtonsoft.Json;


public class Account
{
    public string Name { get; set; }
    public string Email { get; set; }
}


public class nuclio
{
    public void InitContext(Context context){
        context.Name = "Test";
        context.IsInitialized = true;
        context.UserData = new Dictionary<string, object>();
        context.UserData.Add("a", "b");
    }

    public object outputter(Context context, Event eventBase)
    {
        if (eventBase.Method != "POST")
            return eventBase.Method;

        var body = eventBase.GetBody();
        switch (body)
        {
            case "check_userdata":
                {                    
                    return JsonConvert.SerializeObject(context.UserData, Formatting.Indented);
                }
            case "return_string":
                return "a string";
            case "log":
                context.Logger.Debug("Debug message");
                context.Logger.Info("Info message");
                context.Logger.Warning("Warn message");
                context.Logger.Error("Error message");
                return "returned logs";
            case "return_response":
                var headers = new Dictionary<string,object>();
                headers.Add("a",eventBase.Headers["A"]);
                headers.Add("b",eventBase.Headers["B"]);
                headers.Add("h1","v1");
                headers.Add("h2","v2");
                var response = new Response();
                response.Headers = headers;
                response.Body = "response body";
                response.StatusCode = 201;
                response.ContentType = "text/plain; charset=utf-8";
                return response;
            case "panic":
                throw new Exception("Panicking, as per request");
            case "return_fields":
                var fields = eventBase.Fields;
                var listOfFields = new List<string>();
                foreach (var field in fields)
                {
                    listOfFields.Add($"{field.Key}={field.Value}");
                }
                listOfFields.Sort();
                return String.Join(",", listOfFields.Select(x => x.ToString()).ToArray());
            case "return_path":
                return eventBase.Path;
            case "json_convert":
                Account account = new Account
                {
                    Name = "John Doe",
                    Email = "john@iguazio.com"
                };
                string json = JsonConvert.SerializeObject(account, Formatting.Indented);
                return json;
        }

        if (body.StartsWith("long body")) {
            return body;
        }

        throw new Exception(string.Empty);

    }
}