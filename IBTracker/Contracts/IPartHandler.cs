using System;
using System.Collections.Generic;
using IBTracker.Data;

namespace IBTracker.Contracts
{
    public interface IPartHandler
    {
        Type PartType { get; }
        IEnumerable<BasePart> Read(ICollection<SchoolInfo> schools);
        int Link(ICollection<SchoolInfo> schools, IEnumerable<BasePart> parts);
    }
}