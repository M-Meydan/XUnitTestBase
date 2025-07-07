using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XUnitTestBase.UnitTest.Helpers;

public abstract class EventTracker
{
    public abstract void TrackUserEvent(string userEmail, string action);
}
