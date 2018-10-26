﻿using System;
using Newtonsoft.Json;
using SQLite4Unity;

namespace Assets.SQLite4Unity.Scripts
{
    public class JsonConverter : IJsonConverter
    {
    public object FromBody(Type t, string body)
    {
        bool isError = false;
        string errorMessage = "";
        object data = JsonConvert.DeserializeObject(body, t,
            new JsonSerializerSettings
            {
                Error = (o, args) =>
                {
                    args.ErrorContext.Handled = true;
                    errorMessage = args.ErrorContext.Error.Message;
                    isError = true;
                }
            });
        if (isError)
        {
            throw new Exception(errorMessage);
        }
        return data;
    }

    public string ToBody(object data)
    {
        string dataString = "";
        dataString = JsonConvert.SerializeObject(data);
        return dataString;
    }
    }
}