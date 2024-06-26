using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LogicLeapPiazzaRemake.Pages.Admin
{
    [Authorize(Roles = "Instructor")]
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
