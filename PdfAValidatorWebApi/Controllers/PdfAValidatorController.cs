﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PdfAValidatorWebApi.Controllers
{
    /// <summary>
    /// <see cref="PdfAValidatorController"/>
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class PdfAValidatorController : ControllerBase
    {
        /// <summary>
        /// Validates the compliance of a PdfA.
        /// </summary>
        /// <param name="pdfFile"></param>
        /// <returns>Compliance</returns>
        /// <response code="200">Returns the result</response>
        /// <response code="400">If the pdf is missing.</response>
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> Validate(IFormFile pdfFile)
        {
            if (pdfFile is null)
            {
                throw new ArgumentNullException(nameof(pdfFile));
            }

            var uploadedFile = Request.Form.Files.Single();
            var tempPdfFilePath = Path.Combine(Path.GetTempPath(), "VeraPdf" + Guid.NewGuid() + ".pdf");
            try
            {
                using var fs = new FileStream(tempPdfFilePath, FileMode.CreateNew, FileAccess.Write);
                using var pdfAValidator = new PdfAValidator.PdfAValidator();
                await uploadedFile.CopyToAsync(fs).ConfigureAwait(false);

                var result = pdfAValidator.Validate(tempPdfFilePath);
                return Ok(result);
            }
            finally
            {
                if (System.IO.File.Exists(tempPdfFilePath))
                {
                    System.IO.File.Delete(tempPdfFilePath);
                }
            }
        }

        /// <summary>
        /// Validates the compliance of a Pdf(A) and gives some validation detail.
        /// </summary>
        /// <param name="pdfFile"></param>
        /// <returns>Compliance</returns>
        /// <response code="200">Returns a report about the analyzed pdf, e.g. pdfa substandard and compliance violations</response>
        /// <response code="400">If the pdf is missing.</response>
        [HttpPost]
        [Route("DetailedReport")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> ValidateWithDetailedReport(IFormFile pdfFile)
        {
            var uploadedFile = Request.Form.Files.Single();
            var tempPdfFilePath = Path.Combine(Path.GetTempPath(), "VeraPdf" + Guid.NewGuid() + ".pdf");
            try
            {
                using var fs = new FileStream(tempPdfFilePath, FileMode.CreateNew, FileAccess.Write);
                using var pdfAValidator = new PdfAValidator.PdfAValidator();
                await uploadedFile.CopyToAsync(fs).ConfigureAwait(false);

                var result = pdfAValidator.ValidateWithDetailedReport(tempPdfFilePath);
                return Ok(result);
            }
            finally
            {
                if (System.IO.File.Exists(tempPdfFilePath))
                {
                    System.IO.File.Delete(tempPdfFilePath);
                }
            }
        }
    }
}