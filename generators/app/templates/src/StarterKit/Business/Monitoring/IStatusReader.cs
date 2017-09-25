using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StarterKit.Business.Monitoring
{
    public interface IStatusReader
    {
        Task<Monitoring> GetStatus();
    }
}
