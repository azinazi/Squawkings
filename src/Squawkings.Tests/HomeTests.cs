using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using FubuCore;
using Moq;
using NPoco;
using NUnit.Framework;
using Squawkings.Controllers;
using Squawkings.Models;

namespace Squawkings.Tests
{
    [TestFixture]
    public class HomeTests : ControllerTest
    {
        [Test]
        public void WhenNotAuthenticatedRedirectToGlobal()
        {
            // Arrange
            var dbmock = new Mock<IDatabase>();
            var controller = CB.Of(new HomeController(dbmock.Object)).Build();

            // Act
            var result = controller.Index() as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Global", result.RouteValues["controller"]);
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }

        [Test]
        public void WhenAuthenticatedReturnHomeView()
        {
            // Arrange
            var dbmock = new Mock<IDatabase>();
            dbmock.Setup(x => x.Fetch<SquawkDisp>(It.IsAny<Sql>())).Returns(new List<SquawkDisp>());
            var controller = CB.Of(new HomeController(dbmock.Object))
                .WithLoggedInId(1)
                .Build();

            // Act
            var result = controller.Index() as ViewResult;
            var squawks = result.Model.As<SquawksInputModel>().SquawksList;
            
            
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(typeof(SquawksInputModel), result.Model.GetType());
        }
    }

    public class CB
    {
        public static ControllerBuilder<T> Of<T>(T controller) where T : Controller
        {
            return new ControllerBuilder<T>(controller);
        }
    }

    public class ControllerBuilder<T> where T : Controller
    {
        private T _controller;

        public ControllerBuilder(T controller)
        {
            _controller = controller;
            _controller.SetFakeControllerContext();
        }

        public ControllerBuilder<T> WithLoggedInId(int id)
        {
            var user = Mock.Get(_controller.User);
            user.Setup(x => x.Identity).Returns(new GenericIdentity(id.ToString()));
            var request = Mock.Get(_controller.Request);
            request.Setup(x => x.IsAuthenticated).Returns(true);
            return this;
        }

        public T Build()
        {
            return _controller;
        }
    }

    public class ControllerTest
    {
        public ControllerTest()
        {
        }

        public T GetController<T>() where T : Controller, new()
        {
            var controller = new T();
            controller.SetFakeControllerContext();
            return controller;
        }

        public T GetController<T>(Func<T> controllerBuilder) where T : Controller
        {
            var controller = controllerBuilder();
            controller.SetFakeControllerContext();
            return controller;
        }

        public IEnumerable<string> GetModelErrors(Controller controller)
        {
            return controller.ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
        }
    }

    public static class MvcMockHelpers
    {
        public static HtmlHelper<T> GetHtmlHelper<T>(T model)
        {
            var viewdata = new ViewDataDictionary(model);
            var mockViewContext = MockViewContext(viewdata);

            var viewDataContainer = new Mock<IViewDataContainer>();
            viewDataContainer.Setup(x => x.ViewData).Returns(viewdata);

            var helper = new HtmlHelper<T>(mockViewContext, viewDataContainer.Object);

            return helper;
        }

        public static ViewContext MockViewContext(ViewDataDictionary viewDataDictionary)
        {
            var httpContext = FakeHttpContext();
            Mock.Get(httpContext).Setup(x => x.Items).Returns(new Dictionary<string, object>());

            var mockViewContext = new ViewContext(
                new ControllerContext(
                    httpContext,
                    new RouteData(),
                    new Mock<ControllerBase>().Object),
                new Mock<IView>().Object,
                viewDataDictionary,
                new TempDataDictionary(),
                new StringWriter());

            return mockViewContext;
        }

        public static HttpContextBase FakeHttpContext()
        {
            var context = new Mock<HttpContextBase>();
            var request = new Mock<HttpRequestBase>();
            var response = new Mock<HttpResponseBase>();
            var session = new Mock<HttpSessionStateBase>();
            var server = new Mock<HttpServerUtilityBase>();
            var user = new Mock<IPrincipal>();

            var identity = new Mock<IIdentity>();
            user.Setup(x => x.Identity).Returns(identity.Object);
            context.Setup(ctx => ctx.User).Returns(user.Object);
            context.Setup(ctx => ctx.Request).Returns(request.Object);
            context.Setup(ctx => ctx.Response).Returns(response.Object);
            context.Setup(ctx => ctx.Session).Returns(session.Object);
            context.Setup(ctx => ctx.Server).Returns(server.Object);

            response.Setup(x => x.ApplyAppPathModifier(It.IsAny<string>())).Returns((string p) => p);

            return context.Object;
        }

        public static HttpContextBase FakeHttpContext(string url)
        {
            HttpContextBase context = FakeHttpContext();
            context.Request.SetupRequestUrl(url);
            return context;
        }

        public static void SetFakeControllerContext(this Controller controller)
        {
            var httpContext = FakeHttpContext();
            ControllerContext context = new ControllerContext(new RequestContext(httpContext, new RouteData()), controller);
            controller.ControllerContext = context;
            controller.Url = new UrlHelper(new RequestContext(httpContext, new RouteData()), RouteTable.Routes);
        }

        public static void SetFakeControllerContext(this Controller controller, string url)
        {
            var httpContext = FakeHttpContext(url);
            ControllerContext context = new ControllerContext(new RequestContext(httpContext, new RouteData()), controller);
            controller.ControllerContext = context;
            controller.Url = new UrlHelper(new RequestContext(httpContext, new RouteData()), RouteTable.Routes);
        }

        static string GetUrlFileName(string url)
        {
            if (url.Contains("?"))
                return url.Substring(0, url.IndexOf("?"));
            else
                return url;
        }

        static NameValueCollection GetQueryStringParameters(string url)
        {
            if (url.Contains("?"))
            {
                NameValueCollection parameters = new NameValueCollection();

                string[] parts = url.Split("?".ToCharArray());
                string[] keys = parts[1].Split("&".ToCharArray());

                foreach (string key in keys)
                {
                    string[] part = key.Split("=".ToCharArray());
                    parameters.Add(part[0], part[1]);
                }

                return parameters;
            }
            else
            {
                return null;
            }
        }

        public static void SetHttpMethodResult(this HttpRequestBase request, string httpMethod)
        {
            Mock.Get(request)
                .Setup(req => req.HttpMethod)
                .Returns(httpMethod);
        }

        public static void SetupRequestUrl(this HttpRequestBase request, string url)
        {
            if (url == null)
                throw new ArgumentNullException("url");

            if (!url.StartsWith("~/"))
                throw new ArgumentException("Sorry, we expect a virtual url starting with \"~/\".");

            var mock = Mock.Get(request);

            mock.Setup(req => req.QueryString)
                .Returns(GetQueryStringParameters(url));
            mock.Setup(req => req.AppRelativeCurrentExecutionFilePath)
                .Returns(GetUrlFileName(url));
            mock.Setup(req => req.PathInfo)
                .Returns(String.Empty);

            mock.SetupGet(x => x.ApplicationPath).Returns("/");
            mock.SetupGet(x => x.Url).Returns(new Uri("http://localhost", UriKind.Absolute));
            mock.SetupGet(x => x.ServerVariables).Returns(new NameValueCollection());

        }
    }
}



