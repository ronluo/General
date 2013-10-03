using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using GeneralDAC.Common;

namespace GeneralDAC
{
    public interface IDatabase
    {
        int ExecuteNonQuery(Command command);
        IDataReader ExecuteReader(Command command);
        object ExecuteScalar(Command command);
    }
}
