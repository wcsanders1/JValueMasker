# JValueMasker

A library offering an extension method on `JContainer` to mask values of elements with specified properties.
This can be useful when you want to log a `JContainer` containing sensitive information, such as passwords.

Compatible with the following:

- .NET Framework 4.5
- .NET Framework 4.6
- .NET Standard 1.6
- .NET Standard 2.0

## How To Use It

Call `MaskValues` on any `JContainer`, passing in a `List<string>` containing the names of the properties to mask.

Simple example:

```cs
var jProperty = new JProperty("password", "badPassword");
var maskedResult = jProperty.MaskValues(new List<string>("password"));

Console.WriteLine(maskedResult.ToString());
```

The above code will print the following: `"password": "***"`

Complex example:

```cs
var jObj = new JObject(
  new JProperty("name", "Bob"),
    new JProperty("something-neat", null),
    new JProperty(
      new JProperty("title", "War 'n Peace")),
    new JProperty(
      new JProperty(
        new JProperty("password", "badPassword"))),
    new JProperty("accountings",
      new JArray(
        new JValue("password"),
          new JValue(654645),
          new JValue("good times"),
          new JObject(
            new JProperty("read less", 444),
            new JProperty("password", 78789798.787),
            new JProperty("meanderings",
              new JObject(
              new JProperty("planets",
                new JArray(
                  new JValue(
                    new JValue(
                      new JValue(true)))),
                new JValue("Mars"),
                new JValue("Venus"),
                new JValue("Rectus 9"),
                new JObject(
                new JProperty("temperature", "mild"),
                new JProperty("PASSWORD", "GREAT-PASSWORD!!")))))))));

var maskedResult = jObj.MaskValues(new List<string>{"password"});

Console.WriteLine(maskedResult.ToString());
```

The above code will print the following:

```json
{
  "name": "Bob",
  "something-neat": null,
  "title": "War 'n Peace",
  "password": "***",
  "accountings": [
    "password",
    654645,
    "good times",
    {
      "read less": 444,
      "password": "***",
      "meanderings": {
        "planets": [
          [true],
          "Mars",
          "Venus",
          "Rectus 9",
          {
            "temperature": "mild",
            "PASSWORD": "***"
          }
        ]
      }
    }
  ]
}
```
