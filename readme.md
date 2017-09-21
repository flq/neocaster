## NeoCaster - Load Neo4J Data into .NET Objects quick and easy.

[![Build status](https://ci.appveyor.com/api/projects/status/mm2enoy06ce4w07v?svg=true)](https://ci.appveyor.com/project/flq/neocaster)

At the time of this writing only the basic [Neo4J-Driver](https://neo4j.com/developer/dotnet/) exists in the .NET space. 
I thought that it is kind of sad that only JVM-developers would have good programmatic access to the fun that writing
programs with [NEO4J](https://neo4j.com) is.

Neo4J is arguably one of the most important (& interesting) graph databases we can get our hands on.

It helps you work with data where the relationships between data is as important as the actual data itself. People use it to build product databases or gather insights into datasets.

That's why in Neo4J we have not only **nodes** (with properties) as first-class citizens, but **relationships**, too!

Based on the provided driver which allows access to a Neo4J database through their binary bolt-Protocol, this library sets out to bridge the raw data coming out of Cypher (The SQL pendant in Neo4J-parlor) into .NET classes. Here's a simple example:

    IEnumerable<Person> people = 
      driver.OpenSession().Query<Person>("MATCH (p:Person) RETURN p");

This library is heavily inspired by [Dapper](https://github.com/StackExchange/Dapper), the _Micro-ORM_ provided by the wonderful devs at StackExchange, which makes it easy to use your SQL-superpowers and quickly map the results into .NET classes.

## Usage

## Contributing

### Requirements

Development has been tested with __Visual Studio 2017__ so far. Also, it is advised to install the .NET core SDK v2.0, 
which you can [download here](https://www.microsoft.com/net/download/core).

### Using the tests

A number of tests depend on a running neo4j instance. Neocaster recommends using a docker container for tests as
it will ensure that any data created during tests is removed after testing.

First, create a fitting container:

`docker run --publish=7474:7474 --publish=7687:7687 --env=NEO4J_AUTH=none --name neocaster_tests neo4j`

Then, you can start and stop that container for testing:

`docker start neocaster_tests`, `docker stop neocaster_tests`