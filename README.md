# SQLite4Unity
SQLite4Unity, which comes from [sqlite-net](https://github.com/praeclarum/sqlite-net).
I rewrite part of the code base on .Net 3.5, platform and use the sqlite3 lib downloaded from the [official website](https://www.sqlite.org/download.html) in Version 3250200.

> Note: SQLite4Unity uses only the synchronous part of sqlite-net, so all the calls to the database are synchronous.

## New Feature
Surport to serilize the class-type property into Json text, and deseriliz the Json text into a class instance.

2 steps to impliment this:
### 1. Define IJsonConverter on initialize db
```
 _db = new TestDb("db_path",new SomeJsonConvert());

```
here is an example of how to impliment a JsonConvert using Newtonsoft.Json
```
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
### 2. Place [SaveAsJson] attribute on the property needed to serilize/deserilize
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
> Qulity is a class-type consist of some primitive-type property, which can be serilize by Newtonsoft.Json.
> whereby the Table named "car" will has an column named "Quality", and the data is save as json text, for example
> {"Speed":100,"Hardness":200}
