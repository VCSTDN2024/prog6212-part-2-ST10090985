using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CMCS_ST10090985.Models;

namespace CMCS_ST10090985.Controllers
{
    public class ClaimsController : Controller
    {
        // A static list to store claims in memory
        private static List<Claim> claimsList = new List<Claim>();

        // GET: /Claims/AddClaim - Display the form
        public IActionResult AddClaim()
        {
            return View();
        }

        // POST: /Claims/SubmitClaim - Process the form submission
        [HttpPost]
        public async Task<IActionResult> SubmitClaim(Claim model, IFormFile UploadedFile)
        {
            if (UploadedFile != null && UploadedFile.Length > 0)
            {
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

            // Assign an ID and add the claim to the list
            model.Id = claimsList.Count + 1;

            // Set the Status to "Pending"
            model.Status = "Pending";

            claimsList.Add(model);

            // Redirect to the claims list page
            return RedirectToAction("L_ClaimsList", "Home");
        }

        // GET: /Claims/ClaimsList - Display the list of claims
        public IActionResult L_ClaimsList()
        {
            return View(claimsList);  // Redirects the user back to ViewClaims View //passes the claimslist List to the View
        }
    }
}
