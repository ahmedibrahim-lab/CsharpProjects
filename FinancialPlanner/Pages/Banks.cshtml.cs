using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinancialPlanner.Services;

namespace FinancialPlanner.Pages
{
    public class BanksModel : PageModel
    {
        private readonly FinancialService _financialService;

        public BanksModel(FinancialService financialService)
        {
            _financialService = financialService;
        }

        public List<InstitutionViewModel> Institutions { get; private set; }

        public async Task OnGetAsync()
        {
            var secretId = "cc1030b9-4c79-4038-aa7b-2c640506ead7";
            var secretKey = "6b8b12347ddbf1876bfdc6f19db5d2028d6e73e3a0491f511c85991707e6e4067a373136236469f06f5738591c0089deab2a8ce7aae95a22f99dd4295c689391";

            // Ensure the token is fetched and set before making the institutions API call
            await _financialService.EnsureTokenAsync(secretId, secretKey);
            var allInstitutions = await _financialService.GetInstitutionsAsync();

            // Filter to only show the institution with the common prefix name for each unique logo
            Institutions = allInstitutions
                .GroupBy(i => i.Logo)
                .Select(g => new InstitutionViewModel
                {
                    Logo = g.Key,
                    Name = DetermineCommonPrefix(g.Select(i => i.Name).ToList())
                })
                .ToList();
        }

        private string DetermineCommonPrefix(List<string> names)
        {
            if (!names.Any()) return string.Empty;

            // Find the shortest name in the group
            var shortestName = names.OrderBy(n => n.Length).First();

            // Determine the common prefix
            for (int i = 0; i < shortestName.Length; i++)
            {
                char currentChar = shortestName[i];
                if (names.Any(name => name[i] != currentChar))
                {
                    return shortestName.Substring(0, i).Trim();
                }
            }

            return shortestName.Trim();
        }

        public class InstitutionViewModel
        {
            public string Name { get; set; }
            public string Logo { get; set; }
        }
    }
}
