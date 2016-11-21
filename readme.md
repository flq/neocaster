
## Neocaster

Get Dapper-Style usage patterns, but for Neo4J

## Tests

dotnet test

You'll need to get a neo4j server going listening on 7698.
You can use the provided testrunner project, which you need to build with gradle:

gradle shadowJar
cd testrunner/build/libs
java -jar neocaster.testrunner-1.0-SNAPSHOT-fat.jar