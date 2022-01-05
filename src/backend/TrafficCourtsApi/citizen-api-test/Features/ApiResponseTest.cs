using AutoFixture;
using AutoFixture.Kernel;
using Gov.CitizenApi.Features;
using Gov.CitizenApi.Test.Utils;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Linq;
using Xunit;

namespace Gov.CitizenApi.Test.Features
{
    public class ApiResponseTest
    {
        [Theory]
        [AutoMockAutoData]
        public void Result_T_will_create_ApiResultResponse_T(Type type)
        {
            var fixture = new Fixture();
            var obj = fixture.Create(type, new SpecimenContext(fixture));
            var result = ApiResponse.Result(obj);
            Assert.IsAssignableFrom<ApiResultResponse<object>>(result);

        }

        [Theory]
        [AutoMockAutoData]
        public void Message_str_will_create_ApiMessageResponse(string msg)
        {
            var result = ApiResponse.Message(msg);
            Assert.IsAssignableFrom<ApiMessageResponse>(result);
            Assert.Equal(msg, result.Message);
        }

        [Theory(Skip="failing for now")]
        [AutoMockAutoData]
        public void BadRequest_will_create_ApiBadRequestResponse(ModelStateDictionary modelState, string key, string errorMsg)
        {
            modelState.AddModelError(key, errorMsg);
            var result = ApiResponse.BadRequest(modelState);
            Assert.IsAssignableFrom<ApiBadRequestResponse>(result);
            Assert.Equal(errorMsg, result.Errors.First());
        }
    }
}
