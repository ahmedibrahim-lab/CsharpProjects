using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebCalculator.Models;

namespace WebCalculator.Pages
{
    public class CalculatorModel : PageModel
    {
        [BindProperty]
        public string Expression { get; set; }

        public double? Result { get; set; }

        public void OnPost()
        {
            if (!string.IsNullOrEmpty(Expression))
            {
                try
                {
                    Result = Calculator.EvaluateExpression(Expression);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Error: {ex.Message}");
                }
            }
        }
    }
}
