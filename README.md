# SQLite4Unity
SQLite4Unity, which comes from [sqlite-net](https://github.com/praeclarum/sqlite-net).
I rewrite part of the code base on .Net 3.5, platform and use the sqlite3 lib downloaded from the [official website](https://www.sqlite.org/download.html) in Version 3250200.

> Note: SQLite4Unity uses only the synchronous part of sqlite-net, so all the calls to the database are synchronous.

## New Feature
Surport to serilize the class-type property into Json text, and deserilize the Json text into a class instance.

2 steps to use this feature:
### 1. Define IJsonConverter on initialization of a db connection
```
 SQLiteConnection _db = new SQLiteConnection("db_path",new SomeJsonConvert());

```
here is an example of how to impliment a JsonConvert using Newtonsoft.Json
```
using Newtonsoft.Json;

public class DefalutConvert : IJsonConverter
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
```
### 2. Place [SaveAsJson] attribute on the property which needed to be serilized/deserilized
```
public class Car
       {
           [PrimaryKey, AutoIncrement]
           public int Id { get; set; }
           public string Name { get; set; }
           [SaveAsJson]
           public Quality Quality { get; set; }
		}
 public class Quality
       {
           public int Speed { get; set; }
           public int Hardness { get; set; }

       }
```
Qulity is a class-type consist of some property in primitive-type, which can be serilized by Newtonsoft.Json.
whereby the Table named "car" will has an column named "Quality", and the data is saved as json text, like this:

Id|Name|Quality
---|:--:|---:
1|s300|{"Speed":280,"Hardness":180}
2|c300L|{"Speed":200,"Hardness":130}

