using System;
using IdeaWeb.Controllers;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace IdeaWeb.Test
{
    [TestFixture]
    public class HomeControllerTests
    {
        HomeController _controller;

        [SetUp]
        public void SetUp()
        {
            _controller = new HomeController();
        }

        [Test]
        public void IndexReturnsIdeasView()
        {
            IActionResult result = _controller.Index();

            Assert.That(result, Is.Not.Null);
        }
    }
}
