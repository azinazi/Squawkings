using System.Web.Helpers;
using System.Web.Mvc;
using Moq;
using NPoco;
using NUnit.Framework;
using Squawkings.Controllers;

namespace Squawkings.Tests
{
    [TestFixture]
    class LogonTests : ControllerTest
    {
        [Test]
        public void WhenModelNotValid()
        {
            //Arange
            var dmock = new Mock<IDatabase>();
            var controller = CB.Of(new LogonController(dmock.Object)).Build();
            controller.ModelState.AddModelError("key", "error");

            // Act
            var result = controller.Index(new LogonInputModel()) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            dmock.Verify(x => x.SingleOrDefault<LogonInputModel>(It.IsAny<string>(), It.IsAny<object[]>()), Times.Never());
        }

        [Test]
        public void WhenModelNotValidWithError()
        {
            //Arange
            var dmock = new Mock<IDatabase>();
            var controller = CB.Of(new LogonController(dmock.Object)).Build();

            // Act
            var result = controller.Index(new LogonInputModel()) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            dmock.Verify(x => x.SingleOrDefault<LogonInputModel>(It.IsAny<string>(), It.IsAny<object[]>()), Times.Once());

        }

        [Test]
        public void WhenModelValidAndPasswordCorrectRedirectToHomeIndex()
        {
            //Arange
            var dmock = new Mock<IDatabase>();
            var authmock = new Mock<IAuthentication>();

            var controller = CB.Of(new LogonController(dmock.Object, authmock.Object))
                .Build();

            var userInfo = new UserInfo();
            userInfo.Password = Crypto.HashPassword("password");
            userInfo.UserId = 1;

            var input = new LogonInputModel();
            input.Password = "password";
            input.rememberMe = true;

            dmock.Setup(x => x.SingleOrDefault<UserInfo>(It.IsAny<string>(), It.IsAny<object[]>())).Returns(userInfo);
            authmock.Setup(x => x.Authenticate(userInfo.UserId.ToString(), input.rememberMe));
            // Act
            var result = controller.Index(input) as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Home", result.RouteValues["controller"]);
            Assert.AreEqual("Index", result.RouteValues["action"]);

        }     

        [Test]
        public void SignOut()
        {
            //Arange
            var dmock = new Mock<IDatabase>();
            var authmock = new Mock<IAuthentication>();

            var controller = CB.Of(new LogonController(dmock.Object, authmock.Object))
                .Build();

            authmock.Setup(x => x.SignOut());

            // Act
            var result = controller.Logoff() as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Logon", result.RouteValues["controller"]);
            Assert.AreEqual("Index", result.RouteValues["action"]);

        }

    }
}
