package rf.neocaster.testrunner

import org.neo4j.logging.*
import java.io.PrintWriter
import java.lang.System.out
import java.util.function.Consumer


class PrivateLogProvider : LogProvider {
    override fun getLog(name: String?): Log {
        return PrivateLog(name)
    }

    override fun getLog(loggingClass: Class<*>?): Log {
        return PrivateLog(loggingClass?.simpleName)
    }

}

class PrivateLog(val name: String?) : AbstractLog() {
    override fun infoLogger(): Logger {
        return PrivateLogger(name,  "INFO")
    }

    override fun debugLogger(): Logger {
        return PrivateLogger(name, "DEBUG")
    }

    override fun isDebugEnabled(): Boolean {
        return true;
    }

    override fun bulk(consumer: Consumer<Log>) {
        consumer.accept(this)
    }

    override fun warnLogger(): Logger {
        return PrivateLogger(name, "WARN")
    }

    override fun errorLogger(): Logger {
        return PrivateLogger(name, "ERROR")
    }

}

fun makeLock() : String {
    return String()
}

class PrivateLogger(val name: String?, val level: String) : AbstractPrintWriterLogger({ PrintWriter(out) }, makeLock(), true) {
    override fun getBulkLogger(writer: PrintWriter, lock: Any): Logger {
        return this
    }

    override fun writeLog(writer: PrintWriter, message: String) {

        writer.println("$name-$level: $message")
    }

    override fun writeLog(writer: PrintWriter, message: String, throwable: Throwable) {
        writer.println("$name-$level: $message")
        throwable.printStackTrace()
    }

}