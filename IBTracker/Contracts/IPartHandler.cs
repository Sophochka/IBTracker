using System;
using System.Collections.Generic;
using IBTracker.Data;

namespace IBTracker.Contracts
{
    public interface IPartHandler
    {
        IEnumerable<BasePart> Read(ICollection<SchoolInfo> schools);
        void Link(ICollection<SchoolInfo> schools, IEnumerable<BasePart> parts);
    }
}