using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinancialPlanner.Services;

namespace FinancialPlanner.Pages
{
    public class GetInstitutionsModel : PageModel
    {
        private readonly FinancialService _financialService;

        public GetInstitutionsModel(FinancialService financialService)
        {
            _financialService = financialService;
        }

        public List<Institution> Institutions { get; private set; }
        public string AgreementResponse { get; private set; }

        [BindProperty]
        public string SelectedInstitutionId { get; set; }

        public async Task OnGetAsync()
        {
            var secretId = "cc1030b9-4c79-4038-aa7b-2c640506ead7";
            var secretKey = "6b8b12347ddbf1876bfdc6f19db5d2028d6e73e3a0491f511c85991707e6e4067a373136236469f06f5738591c0089deab2a8ce7aae95a22f99dd4295c689391";

            // Ensure the token is fetched and set before making the institutions API call
            await _financialService.EnsureTokenAsync(secretId, secretKey);
            Institutions = await _financialService.GetInstitutionsAsync();
        }

        public async Task<IActionResult> OnPostCreateAgreementAsync()
        {
            await _financialService.EnsureTokenAsync("your_secret_id", "your_secret_key"); // Ensure the token is set
            AgreementResponse = await _financialService.CreateAgreementAsync(SelectedInstitutionId);
            return Page();
        }
    }
}

