using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace tpl_tutorial.models
{
    public interface ISampleInput
    {
        int Id { get; }
        Task Process();
    }
}
