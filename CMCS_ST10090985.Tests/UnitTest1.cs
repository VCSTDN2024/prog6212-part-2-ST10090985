using CMCS_ST10090985.Controllers;
using CMCS_ST10090985.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CMCS_ST10090985.Tests
{
    public class HomeControllerTests
    {
        private readonly HomeController _controller;
        private readonly Mock<ILogger<HomeController>> _mockLogger;

        public HomeControllerTests()
        {
            _mockLogger = new Mock<ILogger<HomeController>>();
            _controller = new HomeController(_mockLogger.Object);
        }

        [Fact]
        public async Task SubmitClaim_ValidClaimWithFile_ReturnsRedirectToL_ClaimsList()
        {
            // Arrange
            var claim = new Claim
            {
                WorkedHours = 10,
                HourlyRate = 25,
                AdditionalNotes = "Sample note",
                Status = "Pending"
            };

            var mockFile = CreateMockFile("testfile.pdf", "File content");

            // Act
            var result = await _controller.SubmitClaim(claim, mockFile.Object);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("L_ClaimsList", redirectToActionResult.ActionName);
           

        }

        [Fact]
        public async Task SubmitClaim_NoFileUploaded_ReturnsViewWithModelError()
        {
            // Arrange
            var claim = new Claim
            {
                WorkedHours = 10,
                HourlyRate = 25,
                AdditionalNotes = "Sample note",
                Status = "Pending"
            };

            // Act
            var result = await _controller.SubmitClaim(claim, null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("AddClaim", viewResult.ViewName); // Should return to the AddClaim view
            Assert.False(_controller.ModelState.IsValid); // Assert model state is invalid
        }

        [Fact]
        public void L_ClaimsList_ReturnsViewWithClaims()
        {
            // Act
            var result = _controller.L_ClaimsList();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<Claim>>(viewResult.Model);
            Assert.Equal(3, model.Count); // Initially, there are 3 dummy claims
        }

        private Mock<IFormFile> CreateMockFile(string fileName, string content)
        {
            var mockFile = new Mock<IFormFile>();
            var memoryStream = new MemoryStream();
            using (var writer = new StreamWriter(memoryStream, leaveOpen: true))
            {
                writer.Write(content);
                writer.Flush();
                memoryStream.Position = 0; // Reset stream position for reading
            }

            mockFile.Setup(_ => _.OpenReadStream()).Returns(memoryStream);
            mockFile.Setup(_ => _.FileName).Returns(fileName);
            mockFile.Setup(_ => _.Length).Returns(memoryStream.Length);

            return mockFile;
        }
    }
}
