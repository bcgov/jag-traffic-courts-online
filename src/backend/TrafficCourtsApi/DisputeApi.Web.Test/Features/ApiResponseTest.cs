using AutoFixture;
using AutoFixture.Kernel;
using DisputeApi.Web.Features;
using DisputeApi.Web.Test.Utils;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NUnit.Framework;
using System;
using System.Linq;

namespace DisputeApi.Web.Test.Features
{
    public class ApiResponseTest
    {
        [Test, TCOAutoData]
        public void Result_T_will_create_ApiResultResponse_T(Type type)
        {
            var fixture = new Fixture();
            var obj = fixture.Create(type, new SpecimenContext(fixture));
            var result = ApiResponse.Result(obj);
            Assert.IsInstanceOf<ApiResultResponse<object>>(result);

        }

        [Test, TCOAutoData]
        public void Message_str_will_create_ApiMessageResponse(string msg)
        {
            var result = ApiResponse.Message(msg);
            Assert.IsInstanceOf<ApiMessageResponse>(result);
            Assert.AreEqual(msg, result.Message);
        }

        [Test, TCOAutoData]
        public void BadRequest_will_create_ApiBadRequestResponse(ModelStateDictionary modelState, string key, string errorMsg)
        {
            modelState.AddModelError(key, errorMsg);
            var result = ApiResponse.BadRequest(modelState);
            Assert.IsInstanceOf<ApiBadRequestResponse>(result);
            Assert.AreEqual(errorMsg, result.Errors.First());
        }
    }
}
