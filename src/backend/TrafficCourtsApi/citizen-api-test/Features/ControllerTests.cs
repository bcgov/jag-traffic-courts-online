using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Xunit;

namespace Gov.CitizenApi.Test.Features
{
    public class ControllerTests
    {
        [Theory]
        [MemberData(nameof(GetControllers))]
        public void Controller_should_inherit_from_correct_base_class(Type type)
        {
            // should be based on ControllerBase
            Assert.True(typeof(ControllerBase).IsAssignableFrom(type));

            // should NOT be based on Controller cause this is MVC type
            Assert.False(typeof(Controller).IsAssignableFrom(type));
        }

        public static IEnumerable<object[]> GetControllers()
        {
            return MemberData.GetTypesAssignableFrom<ControllerBase>();
        }
    }
}
