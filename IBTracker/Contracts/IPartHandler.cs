using System;
using System.Collections.Generic;
using IBTracker.Data;

namespace IBTracker.Contracts
{
    public interface IPartHandler
    {
        Type PartType { get; }
        IEnumerable<BasePart> Read(IDictionary<int, SchoolInfo> schools);
        int Link(IDictionary<int, SchoolInfo> schools, IDictionary<int, BasePart> parts);
    }
}