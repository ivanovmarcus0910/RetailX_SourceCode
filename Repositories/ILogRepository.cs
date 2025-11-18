using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface ILogRepository
    {
        void LogCreate(string message, int staffId);

        void LogUpdate(string message, int staffId);

        void LogWarning(string message, int staffId);
    }
}
