namespace Chat.Logic.Log
{
    using Raven.Client;
    using Raven.Client.Embedded;

    public abstract class RavenDatabaseAware
    {
        private static readonly object syncObject = new object();

        private static IDocumentStore database;

        protected IDocumentStore Database
        {
            get
            {
                if (database == null)
                {
                    lock (syncObject)
                    {
                        if (database != null)
                        {
                            return database;
                        }

                        database = new EmbeddableDocumentStore()
                                       {
                                          DataDirectory = "App_Data"
                                       };

                        database.Initialize();
                        return database;
                    }
                }

                return database;
            }
        }

        protected IDocumentSession OpenSession()
        {
            return this.Database.OpenSession();
        }

    }
}