using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using CMCS_ST10090985.Models;

namespace CMCS_ST10090985.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        // A static list to store claims in memory
        private static List<Claim> claimsList = new List<Claim>();

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;

            // Initialize dummy data
            InitializeDummyData();
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AddClaim()
        {
            return View();
        }

        // POST: /Home/SubmitClaim: Processes the Claims submission
        public async Task<IActionResult> SubmitClaim(Claim model, IFormFile UploadedFile)
        {
            // Handle file upload
            if (UploadedFile != null && UploadedFile.Length > 0)
            {
                // Validate file size (optional)
                const long maxFileSize = 5 * 1024 * 1024; // 5 MB
                if (UploadedFile.Length > maxFileSize)
                {
                    ModelState.AddModelError("UploadedFile", "The file size must not exceed 5 MB.");
                    model.Status = "Pending"; // Set status to keep it consistent
                    return View("AddClaim", model); // Return to form with errors
                }

                // Save the uploaded file to a folder
                var fileName = Path.GetFileName(UploadedFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UploadedFiles", fileName);

                // Save the file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await UploadedFile.CopyToAsync(stream);
                }

                // Save the file name to the model (for display purposes)
                model.UploadedFileName = fileName;
            }
            else
            {
                // Set the UploadedFileName to indicate no file was added
                model.UploadedFileName = null;
            }

            // Assign an ID and add the claim to the list
            model.Id = claimsList.Count + 1;

            // Add the new claim to the static list
            claimsList.Add(model);

            // Redirect to the claims list page
            return RedirectToAction("L_ClaimsList");
        }



        // GET: /Home/L_ClaimsList - Display the list of claims
        public IActionResult L_ClaimsList()
        {
            _logger.LogInformation("Claims List Retrieved: {Count}", claimsList.Count); // Log the count of claims
            return View(claimsList);  // Pass the claims list to the view
        }



        private void InitializeDummyData()
        {
            if (claimsList.Count == 0) // Only add dummy data if the list is empty
            {
                claimsList.Add(new Claim
                {
                    Id = 1,
                    WorkedHours = 10,
                    HourlyRate = 20,
                    AdditionalNotes = "Worked on project A",
                    Status = "Pending",
                    UploadedFileName = null // No file uploaded
                });

                claimsList.Add(new Claim
                {
                    Id = 2,
                    WorkedHours = 15,
                    HourlyRate = 25,
                    AdditionalNotes = "Worked on project B",
                    Status = "Pending",
                    UploadedFileName = "LoyalFree_Home.jpg" // Dummy file name
                });

                claimsList.Add(new Claim
                {
                    Id = 3,
                    WorkedHours = 8,
                    HourlyRate = 30,
                    AdditionalNotes = "Worked on project C",
                    Status = "Pending",
                    UploadedFileName = null // No file uploaded
                });
            }
        }

        public IActionResult LecturerLogin()
        {
            return View();
        }

        public IActionResult AdminLogin()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public IActionResult ViewClaims()
        {
            var claims = claimsList; // Ensure this is your list of claims
            return View(claims);
        }


        /// <summary>
        /// Admin ViewClaims View
        /// This is where the admin can set the status to rejected or approved 
        /// </summary>
        /// <param name="claimID"></param>
        /// <returns></returns>

        // will set the status to Rejected
        [HttpPost]
        public IActionResult RejectClaim(int claimID)
        {
            var claim = claimsList.FirstOrDefault(c => c.Id == claimID);
            if (claim != null)
            {
                claim.Status = "Rejected";
            }
            return RedirectToAction("ViewClaims");
        }

        // will set the status to Approved 
        [HttpPost]
        public IActionResult ApproveClaim(int claimID)
        {
            var claim = claimsList.FirstOrDefault(c => c.Id == claimID);
            if (claim != null)
            {
                claim.Status = "Approved";
            }
            return RedirectToAction("ViewClaims");
        }

    }
}
