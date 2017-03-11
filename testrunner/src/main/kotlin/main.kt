package rf.neocaster.testrunner

import org.neo4j.graphdb.GraphDatabaseService
import org.neo4j.graphdb.factory.GraphDatabaseFactory
import java.io.File


fun main(args: Array<String>) {
    println("Hello, Tester")
    val newEmbeddedDatabase = createGraphDB()
    println("db running on port 7698, press enter to shutdown")
    readLine()
    println("shutting down...")
    newEmbeddedDatabase?.shutdown()
    println("Ciao")
}

private fun createGraphDB(): GraphDatabaseService? {
    val newEmbeddedDatabase = GraphDatabaseFactory()
            .setUserLogProvider(PrivateLogProvider())
            .newEmbeddedDatabaseBuilder(File("data"))
            .loadPropertiesFromFile("neo4j.conf")
            .newGraphDatabase()
    return newEmbeddedDatabase
}
