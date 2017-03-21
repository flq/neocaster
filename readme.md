## NeoCaster - Load Neo4J Data into .NET Objects quick and easy.

[![Build Status](https://travis-ci.org/flq/neocaster.svg?branch=master)](https://travis-ci.org/flq/neocaster)

When I started writing this library only the basic [Neo4J-Driver](https://neo4j.com/developer/dotnet/) existed in the .NET space. I thought that it is kind of sad that only JVM-developers would
have good programmatic access to the fun that writing
programs with [NEO4J](https://neo4j.com) is.

Neo4J is arguably one of the most important (& interesting) graph databases we can get our hands on.

It helps you work with data where the relationships between data is as important as the actual data itself. People use it to build product databases or gather insights into datasets.

That's why in Neo4J we have not only **nodes** (with properties) as first-class citizens, but **relationships**, too!

Based on the provided driver which allows access to a Neo4J database through their binary bolt-Protocol, this library sets out to bridge the raw data coming out of Cypher (The SQL pendant in Neo4J-parlor) into .NET classes. Here's a simple example:

    IEnumerable<Person> people = 
      driver.OpenSession().Query<Person>("MATCH (p:Person) RETURN p");

This library is heavily inspired by [Dapper](https://github.com/StackExchange/Dapper), the _Micro-ORM_ provided by the wonderful devs at StackExchange, which makes it easy to use your SQL-superpowers and quickly map the results into .NET classes.

## Usage

## Documentation

## Contributing

A number of tests depend on a running neo4j instance. You can do that fairly easily through e.g. their Docker container or their standalone installation. Make sure that
the default "neo4j" user gets the password "password".