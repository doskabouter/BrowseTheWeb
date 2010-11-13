using System;
using System.Data;
using System.Runtime.InteropServices;

namespace BrowseTheWeb
{
  public class SQLiteException : Exception
  {
    public SQLiteException(string message) :
      base(message)
    {

    }
  }

  public class SQLite
  {
    const int SQLITE_OK = 0;
    const int SQLITE_ROW = 100;
    const int SQLITE_DONE = 101;
    const int SQLITE_INTEGER = 1;
    const int SQLITE_FLOAT = 2;
    const int SQLITE_TEXT = 3;
    const int SQLITE_BLOB = 4;
    const int SQLITE_NULL = 5;

    //const string path = @"C:\Program Files\Team MediaPortal\MediaPortal\Plugins\Windows\";
    const string path = "";

    [DllImport(path + "sqlite3.dll", EntryPoint = "sqlite3_open")]
    static extern int sqlite3_open(string filename, out IntPtr db);

    [DllImport(path + "sqlite3.dll", EntryPoint = "sqlite3_close")]
    static extern int sqlite3_close(IntPtr db);

    [DllImport(path + "sqlite3.dll", EntryPoint = "sqlite3_prepare_v2")]
    static extern int sqlite3_prepare_v2(IntPtr db, string zSql,
        int nByte, out IntPtr ppStmpt, IntPtr pzTail);

    [DllImport(path + "sqlite3.dll", EntryPoint = "sqlite3_step")]
    static extern int sqlite3_step(IntPtr stmHandle);

    [DllImport(path + "sqlite3.dll", EntryPoint = "sqlite3_finalize")]
    static extern int sqlite3_finalize(IntPtr stmHandle);

    [DllImport(path + "sqlite3.dll", EntryPoint = "sqlite3_errmsg")]
    static extern string sqlite3_errmsg(IntPtr db);

    [DllImport(path + "sqlite3.dll", EntryPoint = "sqlite3_column_count")]
    static extern int sqlite3_column_count(IntPtr stmHandle);

    [DllImport(path + "sqlite3.dll", EntryPoint = "sqlite3_column_origin_name")]
    static extern string sqlite3_column_origin_name(
        IntPtr stmHandle, int iCol);

    [DllImport(path + "sqlite3.dll", EntryPoint = "sqlite3_column_type")]
    static extern int sqlite3_column_type(IntPtr stmHandle, int iCol);

    [DllImport(path + "sqlite3.dll", EntryPoint = "sqlite3_column_int")]
    static extern int sqlite3_column_int(IntPtr stmHandle, int iCol);

    [DllImport(path + "sqlite3.dll", EntryPoint = "sqlite3_column_text")]
    static extern string sqlite3_column_text(IntPtr stmHandle, int iCol);

    [DllImport(path + "sqlite3.dll", EntryPoint = "sqlite3_column_double")]
    static extern double sqlite3_column_double(IntPtr stmHandle, int iCol);

    private IntPtr _db; //pointer to SQLite database
    private bool _open; //whether or not the database is open

    /// <summary>
    /// Opens or creates SQLite database with the specified path
    /// </summary>
    /// <param name="path">Path to SQLite database</param>
    public void OpenDatabase(string path)
    {
      if (sqlite3_open(path, out _db) != SQLITE_OK)
        throw new SQLiteException("Could not open database file: " + path);

      _open = true;
    }

    /// <summary>
    /// Closes the SQLite database
    /// </summary>
    public void CloseDatabase()
    {
      if (_open)
        sqlite3_close(_db);

      _open = false;
    }

    /// <summary>
    /// Executes a query that returns no results
    /// </summary>
    /// <param name="query">SQL query to execute</param>
    public void ExecuteNonQuery(string query)
    {
      if (!_open)
        throw new SQLiteException("SQLite database is not open.");

      //prepare the statement
      IntPtr stmHandle = Prepare(query);

      if (sqlite3_step(stmHandle) != SQLITE_DONE)
        throw new SQLiteException("Could not execute SQL statement.");

      Finalize(stmHandle);
    }

    /// <summary>
    /// Executes a query and stores the results in
    /// a DataTable
    /// </summary>
    /// <param name="query">SQL query to execute</param>
    /// <returns>DataTable of results</returns>
    public DataTable ExecuteQuery(string query)
    {
      if (!_open)
        throw new SQLiteException("SQLite database is not open.");

      //prepare the statement
      IntPtr stmHandle = Prepare(query);

      //get the number of returned columns
      int columnCount = sqlite3_column_count(stmHandle);

      //create datatable and columns
      DataTable dTable = new DataTable();
      for (int i = 0; i < columnCount; i++)
        dTable.Columns.Add(sqlite3_column_origin_name(stmHandle, i));

      //populate datatable
      while (sqlite3_step(stmHandle) == SQLITE_ROW)
      {
        object[] row = new object[columnCount];
        for (int i = 0; i < columnCount; i++)
        {
          switch (sqlite3_column_type(stmHandle, i))
          {
            case SQLITE_INTEGER:
              row[i] = sqlite3_column_int(stmHandle, i);
              break;
            case SQLITE_TEXT:
              row[i] = sqlite3_column_text(stmHandle, i);
              break;
            case SQLITE_FLOAT:
              row[i] = sqlite3_column_double(stmHandle, i);
              break;
          }
        }

        dTable.Rows.Add(row);
      }

      Finalize(stmHandle);

      return dTable;
    }

    /// <summary>
    /// Prepares a SQL statement for execution
    /// </summary>
    /// <param name="query">SQL query</param>
    /// <returns>Pointer to SQLite prepared statement</returns>
    private IntPtr Prepare(string query)
    {
      IntPtr stmHandle;

      if (sqlite3_prepare_v2(_db, query, query.Length,
            out stmHandle, IntPtr.Zero) != SQLITE_OK)
        throw new SQLiteException(sqlite3_errmsg(_db));

      return stmHandle;
    }

    /// <summary>
    /// Finalizes a SQLite statement
    /// </summary>
    /// <param name="stmHandle">
    /// Pointer to SQLite prepared statement
    /// </param>
    private void Finalize(IntPtr stmHandle)
    {
      if (sqlite3_finalize(stmHandle) != SQLITE_OK)
        throw new SQLiteException("Could not finalize SQL statement.");
    }
  }
}

