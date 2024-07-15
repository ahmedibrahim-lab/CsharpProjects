using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using FinancialPlanner.Services;

namespace FinancialPlanner.Pages
{
    public class GetTokenModel : PageModel
    {
        private readonly FinancialService _financialService;

        public GetTokenModel(FinancialService financialService)
        {
            _financialService = financialService;
        }

        public string TokenResponse { get; private set; }

        public async Task OnGetAsync()
        {
            var secretId = "cc1030b9-4c79-4038-aa7b-2c640506ead7";
            var secretKey = "6b8b12347ddbf1876bfdc6f19db5d2028d6e73e3a0491f511c85991707e6e4067a373136236469f06f5738591c0089deab2a8ce7aae95a22f99dd4295c689391";

            TokenResponse = await _financialService.GetTokenAsync(secretId, secretKey);
        }
    }
}
