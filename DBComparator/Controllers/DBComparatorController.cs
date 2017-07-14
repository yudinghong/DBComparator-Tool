﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data;
using System.Data.SqlClient;
using DBComparator.Server;
using DBComparator.Models;

namespace DBComparator.Controllers
{
    public class DBComparatorController : ApiController
    {

        /// <summary>
        /// POST
        /// /api/DBComparator
        /// </summary>
        /// <param name="twoDBStr"></param>
        /// <returns></returns>
        [HttpPost]
        public TableDiff compareLocal([FromBody]string twoDBStr)
        {
            // Init the basic info about response body
            TableDiff rtn = new TableDiff();
            rtn.code = ResponseCode.SUCCESS;
            rtn.msg = "Succecssful";

            // Split the input string to get the connection string of two databases
            string[] dbs = twoDBStr.Split('&');
            if (dbs.Length != 4)
            {
                rtn.code = ResponseCode.ERROR;
                rtn.msg = "Input error,please input four parameters splited with three &";
                return rtn;
            }

            // Connect to database
            SqlConnection conn1 = SqlServer.createConnectionLocal(dbs[0], dbs[1]);
            SqlConnection conn2 = SqlServer.createConnectionLocal(dbs[2], dbs[3]);
            if (conn1 == null || conn2 == null)
            {
                rtn.code = ResponseCode.SUCCESS;
                rtn.msg = "Database not found";
                return rtn;
            }

            // Compare the tables between two databases
            TableComparator tbCompare = new TableComparator();
            rtn.tables = tbCompare.compareTables(conn1, conn2);

            // Compare the stored procedures between two databases
            StoredProcedureComparator pc = new StoredProcedureComparator();
            rtn.storedProcedures = pc.compareStoredProcedure(conn1, conn2);

            // Compare the functions between two databases
            FunctionComparator fc = new FunctionComparator();
            rtn.functions = fc.compareFunctions(conn1, conn2);

            // close the database connection
            conn1.Close();
            conn2.Close();

            return rtn;
        }




       
    }
}
