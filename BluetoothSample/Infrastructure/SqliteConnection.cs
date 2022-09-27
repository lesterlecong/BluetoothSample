using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Shiny;
using SQLite;

namespace BluetoothSample.Infrastructure
{
    public class SqliteConnection : SQLiteAsyncConnection
    {
        public SqliteConnection(IPlatform platform) : base(Path.Combine(platform.AppData.FullName, "ble.db"))
        {
            this.GetConnection().CreateTable<ShinyEvent>();
        }
        public AsyncTableQuery<ShinyEvent> Events => this.Table<ShinyEvent>();
    }
}
