﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using DBComparator.Models;

namespace DBComparator.Server
{
    public class FunctionComparator
    {

        public List<FunctionDiff> compareFunctions(SqlConnection conn1, SqlConnection conn2)
        {
            List<Function> functions1 = getFunctions(conn1);
            List<Function> functions2 = getFunctions(conn2);
            return getDiffFunctions(functions1, functions2);
        }

        private List<FunctionDiff> getDiffFunctions(List<Function> functions1, List<Function> functions2)
        {
            List<FunctionDiff> rtn = new List<FunctionDiff>();

            functions1.Sort((a, b) => a.name.CompareTo(b.name));
            functions2.Sort((a, b) => a.name.CompareTo(b.name));

            string dbname1 = functions1.Count() > 0 ? functions1[0].dbname : "";
            string dbname2 = functions2.Count() > 0 ? functions2[0].dbname : "";

            int pos1 = 0, pos2 = 0;
            while (pos1 < functions1.Count() && pos2 < functions2.Count())
            {
                Function function1 = functions1[pos1];
                Function function2 = functions2[pos2];

                if (function1.name == function2.name)
                {
                    if (!function1.compare(function2))
                    {
                        FunctionDiff funDiff = new FunctionDiff(function1.name, true);
                        funDiff.different.Add(function1);
                        funDiff.different.Add(function2);
                        rtn.Add(funDiff);
                    }
                    pos1++;
                    pos2++;
                }
                else if (function1.name.CompareTo(function2.name) < 0)
                {
                    FunctionDiff funDiff = new FunctionDiff(function1.name, false);
                    funDiff.different.Add(function1);
                    funDiff.different.Add(new Function(dbname2, function1.name, "", false));
                    rtn.Add(funDiff);
                    pos1++;

                }
                else if (function1.name.CompareTo(function2.name) > 0)
                {
                    FunctionDiff funDiff = new FunctionDiff(function2.name, false);
                    funDiff.different.Add(new Function(dbname1, function2.name, "", false));
                    funDiff.different.Add(function2);
                    rtn.Add(funDiff);
                    pos2++;
                }
            }
            while (pos1 < functions1.Count())
            {
                Function function1 = functions1[pos1];
                FunctionDiff funDiff = new FunctionDiff(function1.name, false);
                funDiff.different.Add(function1);
                funDiff.different.Add(new Function(dbname2, function1.name, "", false));
                rtn.Add(funDiff);
                pos1++;
            }
            while (pos2 < functions2.Count())
            {
                Function function2 = functions2[pos2];
                FunctionDiff funDiff = new FunctionDiff(function2.name, false);
                funDiff.different.Add(new Function(dbname1, function2.name, "", false));
                funDiff.different.Add(function2);
                rtn.Add(funDiff);
                pos2++;
            }
            return rtn;
        }

        private List<Function> getFunctions(SqlConnection connection)
        {
            List<Function> rtn = new List<Function>();

            // get function names
            List<string> functionNames = new List<string>();
            string command = "select name from sys.objects WHERE type = 'FN'";
            SqlDataReader dr = SqlServer.ExcuteSqlCommandReader(command, connection);
            while (dr.Read())
            {
                functionNames.Add(dr[0].ToString());
            }

            // get function statements
            foreach (string item in functionNames)
            {
                string commandStat = "select text from sys.syscomments where id = OBJECT_ID('" + item + "')";
                SqlDataReader drStat = SqlServer.ExcuteSqlCommandReader(commandStat, connection);

                drStat.Read();
                Function fun = new Function(connection.Database, item, drStat[0].ToString(), true);
                rtn.Add(fun);
            }
            return rtn;
        }
    }
}