using System;
using Xunit;
using Neo4j.Driver.V1;

[CollectionDefinition("DB")]
public class Neo4JCollection : ICollectionFixture<Neo4jTestingContext> { }

public class Neo4jTestingContext : IDisposable
{
  private IDriver _driver;
  public IDriver GetDriver() {
    return _driver ?? (_driver = GraphDatabase.Driver("bolt://localhost:7698", AuthTokens.None));
  }

  public void WithSession(Action<ISession> sessionAction) 
  {
    using (var session = GetDriver().Session()) 
    {
      sessionAction(session);
    }
  }
  public void Dispose()
  {
    _driver?.Dispose();   
  }
}