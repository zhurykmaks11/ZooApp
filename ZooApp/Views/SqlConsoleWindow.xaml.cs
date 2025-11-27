using MongoDB.Driver;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using ZooApp.Data;
using ZooApp.Models;
using ZooApp.Services;

namespace ZooApp.Views
{
    public partial class SqlConsoleWindow : Window
    {
        private readonly MongoDbContext _context;
        private readonly string _role;
        private readonly string _username;
        private readonly LogService _log;

        public SqlConsoleWindow(string role, string username)
        {
            InitializeComponent();

            _role = role.ToLower();
            _username = username;
            
            if (_role != "admin" && _role != "operator")
            {
                MessageBox.Show("❌ Only admin or operator can use SQL Console!",
                    "Access denied", MessageBoxButton.OK, MessageBoxImage.Warning);
                Close();
                return;
            }

            _context = new MongoDbContext("mongodb://localhost:27017", "test");
            _log = new LogService(_context);
        }

        private void Execute_Click(object sender, RoutedEventArgs e)
        {
            string sql = SqlBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(sql))
            {
                MessageBox.Show("Enter SQL query.");
                return;
            }
            
            string[] forbidden = { "delete", "drop", "truncate", "insert", "update", "remove", "replace" };
            if (forbidden.Any(f => sql.ToLower().Contains(f)))
            {
                MessageBox.Show("❌ Forbidden command. Read-only SQL Console.");
                _log.Write(_username, "SQL Console - forbidden",
                    $"SQL={sql}");
                return;
            }

            if (!sql.ToLower().StartsWith("select"))
            {
                MessageBox.Show("❌ Only SELECT is allowed.");
                _log.Write(_username, "SQL Console - non-select",
                    $"SQL={sql}");
                return;
            }

            try
            {
                var result = ExecuteSelect(sql);
                
                ResultGrid.ItemsSource = null;
                ResultGrid.ItemsSource = result as IList;

                _log.Write(_username, "SQL Console - execute",
                    $"SQL={sql}, Rows={((IList)result).Count}");
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Error: " + ex.Message,
                    "SQL Error", MessageBoxButton.OK, MessageBoxImage.Error);

                _log.Write(_username, "SQL Console - error",
                    $"SQL={sql}, Error={ex.Message}");
            }
        }

        private IList ExecuteSelect(string sql)
        {
            var tableMatch = Regex.Match(sql, @"from\s+(\w+)", RegexOptions.IgnoreCase);
            if (!tableMatch.Success)
                throw new Exception("Table not found in query (missing FROM).");

            string table = tableMatch.Groups[1].Value.ToLower();

            IList list = table switch
            {
                "animals"   => _context.Animals.Find(_ => true).ToList(),
                "cages"     => _context.Cages.Find(_ => true).ToList(),
                "feeds"     => _context.Feeds.Find(_ => true).ToList(),
                "employees" => _context.Employees.Find(_ => true).ToList(),
                "suppliers" => _context.Suppliers.Find(_ => true).ToList(),
                "exchange"  => _context.ExchangeRecords.Find(_ => true).ToList(),
                _ => throw new Exception("Unknown table: " + table)
            };
            
            if (sql.ToLower().Contains("where"))
            {
                var whereMatch = Regex.Match(sql, @"where\s+(.+)", RegexOptions.IgnoreCase);
                if (whereMatch.Success)
                {
                    string condition = whereMatch.Groups[1].Value.Trim();

                    // Підтримка: field = 'value'
                    var equalMatch = Regex.Match(condition, @"(\w+)\s*=\s*'([^']+)'");

                    if (equalMatch.Success)
                    {
                        string field = equalMatch.Groups[1].Value;
                        string value = equalMatch.Groups[2].Value;

                        list = list.Cast<object>()
                            .Where(o =>
                            {
                                var prop = o.GetType().GetProperty(field);
                                if (prop == null) return false;
                                var v = prop.GetValue(o)?.ToString();
                                return v == value;
                            })
                            .ToList();
                    }
                }
            }
            
            var colMatch = Regex.Match(sql, @"select\s+(.+?)\s+from", RegexOptions.IgnoreCase);
            if (colMatch.Success)
            {
                string colPart = colMatch.Groups[1].Value.Trim();

                if (colPart != "*")
                {
                    var cols = colPart.Split(',')
                                      .Select(c => c.Trim())
                                      .ToList();

                    var projected = new List<Dictionary<string, object>>();

                    foreach (var item in list)
                    {
                        var dict = new Dictionary<string, object>();
                        foreach (var col in cols)
                        {
                            var prop = item.GetType().GetProperty(col);
                            if (prop != null)
                                dict[col] = prop.GetValue(item);
                        }
                        projected.Add(dict);
                    }

                    return projected;
                }
            }

            return list;
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow(_role, _username).Show();
            Close();
        }
    }
}
