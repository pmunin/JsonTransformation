[![NuGet version (JsonTransformation)](https://img.shields.io/nuget/v/JsonTransformation.svg?style=flat-square)](https://www.nuget.org/packages/JsonTransformation/)
[![Build status](https://ci.appveyor.com/api/projects/status/con1nroyjfv6id54?svg=true)](https://ci.appveyor.com/project/pmunin/jsontransformation)

# What is this

.NET Standard library allows to transform source [Newtonsoft.Json.Linq.JToken](https://www.newtonsoft.com/json/help/html/T_Newtonsoft_Json_Linq_JToken.htm) object recursively using extendable "Transformers"

Note: Current implementation actually MUTATES source node provided for transformation

## Examples:

### Loading linked files:
```
//entryFile.json:
{
    "linkedFile": { "$file": "shared.json" }
}

///shared.json:
{
    "myFile2Prop1": 123
}

// new JsonTransformationService(new LoadFileTransform()).Transform(entryFileJToken):
{
    "linkedFile": {
		"myFile2Prop1": 123
	}
}

```


### Merging by jpath:
```
//file.json:
{
    "templates": {
        "t1": {
            "tp1": 123,
            "tp2": "Hello world"
        }
    },
    "jobs": {
        "job1": {
            "job1Prop1": 1235,
            "$merge": "$.templates.t1"
        }
    }
}

//new JsonTransformationService(new MergeTransform()).Transform(fileJToken.SelectToken("$.jobs.job1")); 
{
    "job1Prop1": 1235,
    "tp1": 123,
    "tp2": "Hello world"
}

```

### Setting Parameters:
```
// file.json:
{
    "templates": {
        "t1": {
            "name": { "$useParameter": "jobName" },
            "tp1": 123,
            "tp2": "Hello world"
        }
    },
    "jobs": {
        "job1": {
            "job1Prop1": 1235,
            "$merge": "$.templates.t1",
            "$setParameters": {
                "jobName": "My Job 1"
            }
        }
    }
}

//new JsonTransformationService( new MergeTransform(), new ParametersTransform())
//	.Transform(fileJToken.SelectToken("$.jobs.job1")):
{
    "job1Prop1": 1235,
    "name": "My Job 1",
    "tp1": 123,
    "tp2": "Hello world"
}


```


## Extensibility

If you need additional transformers, just implement interface ICanTransformJson and add it in ctor parameters of your jsonTransformationService instance.

## Installation

Use nuget package: https://www.nuget.org/packages/JsonTransformation/